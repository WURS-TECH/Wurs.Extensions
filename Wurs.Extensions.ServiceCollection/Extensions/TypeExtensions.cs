using System.Reflection;
using Wurs.Extensions.ServiceCollection.Attributes;
using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection.Extensions;

internal static class TypeExtensions
{
    internal static IEnumerable<Type> Filter(this IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<RegisterOptionAttribute>();
            if (attr is not null &&
                (attr.RegisterOptionType == OptionType.Settings || attr.RegisterOptionType == OptionType.Environment))
            {
                yield return type;
            }
        }
    }
}