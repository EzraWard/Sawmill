using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using TailBlazer.Domain.FileHandling.Search;
using TailBlazer.Domain.FileHandling.TextAssociations;
using TailBlazer.Domain.Formatting;
using TailBlazer.Domain.Infrastructure;
using TailBlazer.Views.Formatting;

namespace TailBlazer.Views.Searching;

public class SearchProxyCollection : ISearchProxyCollection
{
    private readonly IDisposable _cleanUp;
    public IProperty<int> Count { get; }

    public ReadOnlyObservableCollection<SearchOptionsProxy> Included { get; }
    public ReadOnlyObservableCollection<SearchOptionsProxy> Excluded { get; }

    public SearchProxyCollection(ISearchMetadataCollection metadataCollection,
        Guid id,
        Action<SearchMetadata> changeScopeAction,
        ISchedulerProvider schedulerProvider,
        IColourProvider colourProvider,
        IIconProvider iconsProvider,
        ITextAssociationCollection textAssociationCollection,
        IThemeProvider themeProvider)
    {
        var proxyItems = metadataCollection.Metadata.Connect()
            .WhereReasonsAre(ChangeReason.Add, ChangeReason.Remove) //ignore updates because we update from here
            .Transform(meta =>
            {
                return new SearchOptionsProxy(meta,
                    changeScopeAction,
                    colourProvider,
                    themeProvider,
                    new IconSelector(iconsProvider, schedulerProvider),
                    m => metadataCollection.Remove(m.SearchText),
                    iconsProvider.DefaultIconSelector,
                    id);
            })
            .SubscribeMany(so =>
            {
                //when a value changes, write the original value back to the metadata collection
                var anyPropertyHasChanged = so.WhenAnyPropertyChanged()
                    .Select(_ => (SearchMetadata) so)
                    .Subscribe(metadataCollection.AddorUpdate);

                //when an icon or colour has changed we need to record user choice so 
                //the same choice can be used again
                var iconChanged = so.WhenValueChanged(proxy => proxy.IconKind, false).ToUnit();
                var colourChanged = so.WhenValueChanged(proxy => proxy.HighlightHue, false).ToUnit();
                var ignoreCaseChanged = so.WhenValueChanged(proxy => proxy.CaseSensitive, false).ToUnit();

                var textAssociationChanged = iconChanged.Merge(colourChanged).Merge(ignoreCaseChanged)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .Select(_ => new TextAssociation(so.Text, so.CaseSensitive, so.UseRegex, so.HighlightHue.Swatch,
                        so.IconKind.ToString(), so.HighlightHue.Name, DateTime.UtcNow))
                    .Subscribe(textAssociationCollection.MarkAsChanged);

                return new CompositeDisposable(anyPropertyHasChanged, textAssociationChanged);
            })
            .AsObservableCache();

        Count = proxyItems.CountChanged.StartWith(0).ForBinding();

        //load data onto grid
        var collection = new ObservableCollectionExtended<SearchOptionsProxy>();

        var includedLoader = proxyItems
            .Connect(proxy => !proxy.IsExclusion)
            .Sort(SortExpressionComparer<SearchOptionsProxy>.Ascending(proxy => proxy.Position))
            .ObserveOn(schedulerProvider.MainThread)
            //force reset for each new or removed item dues to a bug in the underlying dragablz control which inserts in an incorrect position
            .Bind(collection, new ObservableCollectionAdaptor<SearchOptionsProxy, string>(0))
            .DisposeMany()
            .Subscribe();

        var excludedLoader = proxyItems
            .Connect(proxy => proxy.IsExclusion)
            .Sort(SortExpressionComparer<SearchOptionsProxy>.Ascending(proxy => proxy.Text))
            .ObserveOn(schedulerProvider.MainThread)
            //force reset for each new or removed item dues to a bug in the underlying dragablz control which inserts in an incorrect position
            .Bind(out var excluded)
            .DisposeMany()
            .Subscribe();


        Excluded = excluded;
        Included = new ReadOnlyObservableCollection<SearchOptionsProxy>(collection);

        _cleanUp = new CompositeDisposable(proxyItems, includedLoader, excludedLoader);
    }

    public void Dispose()
    {
        _cleanUp.Dispose();
    }
}
