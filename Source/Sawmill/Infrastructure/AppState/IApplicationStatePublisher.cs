namespace Sawmill.Infrastructure.AppState;

public interface IApplicationStatePublisher
{
    void Publish(ApplicationState state);
}