namespace Sawmill.Domain.Formatting;

public interface ILineMatches
{
    IObservable<LineMatchCollection> GetMatches(string inputText);
}