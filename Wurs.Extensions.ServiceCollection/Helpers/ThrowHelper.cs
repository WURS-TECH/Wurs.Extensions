using Wurs.Extensions.ServiceCollection.Exceptions;

namespace Wurs.Extensions.ServiceCollection.Helpers;

internal static class ThrowHelper
{
    internal static void ThrowIfNull(params object[] arguments)
    {
        for (int i = 0; i < arguments.Length; i++)
        {
            ArgumentNullException.ThrowIfNull(arguments[i]);
        }
    }
}
