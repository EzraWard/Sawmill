namespace Sawmill.Infrastructure.AppState;

public interface IApplicationStateNotifier
{
    IObservable<ApplicationState> StateChanged { get; }
}