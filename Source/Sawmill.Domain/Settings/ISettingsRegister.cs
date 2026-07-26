using Sawmill.Domain.Annotations;

namespace Sawmill.Domain.Settings;

public interface ISettingsRegister
{
    void Register<T>([NotNull] IConverter<T> converter, [NotNull] string key);
}