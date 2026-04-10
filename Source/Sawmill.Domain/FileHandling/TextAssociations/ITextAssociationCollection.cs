using DynamicData;
using DynamicData.Kernel;

namespace Sawmill.Domain.FileHandling.TextAssociations;

public interface ITextAssociationCollection
{
    IObservableList<TextAssociation> Items { get; }

    void MarkAsChanged(TextAssociation file);

    Optional<TextAssociation> Lookup(string key);

}