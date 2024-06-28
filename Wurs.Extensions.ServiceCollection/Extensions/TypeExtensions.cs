using System.Reflection;
using Wurs.Extensions.ServiceCollection.Attributes;
using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection.Extensions;

internal static class TypeExtensions
{
    internal static IEnumerable<Type>? Filter(this IEnumerable<Type> types) => types.Where(IsValidType);

    private static bool IsValidType(Type type) => type?.GetCustomAttribute<RegisterOptionAttribute>()?.RegisterOptionType
        .HasFlag(OptionType.Settings | OptionType.Environment) ?? false;
}
