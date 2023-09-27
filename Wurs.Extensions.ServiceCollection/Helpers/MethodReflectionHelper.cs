using System.Reflection;

namespace Wurs.Extensions.ServiceCollection.Helpers;

internal static class MethodReflectionHelper
{
    internal static MethodInfo GetMethodInfo(Type type, string methodName, BindingFlags bindingFlags)
        => type.GetMethod(methodName, bindingFlags)!;

    internal static void InvokeGenericMethod(Type type, MethodInfo methodInfo, params object[] parameters)
        => methodInfo.MakeGenericMethod(type).Invoke(null, parameters);
}
