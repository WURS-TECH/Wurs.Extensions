using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Wurs.Extensions.ServiceCollection.Atributtes;
using Wurs.Extensions.ServiceCollection.Enums;
using Wurs.Extensions.ServiceCollection.Helpers;

namespace Wurs.Extensions.ServiceCollection;
/// <summary>
/// Registers as <see cref="IOptions{TOptions}"/> every <see cref="Type"/>
/// marked with <see cref="RegisterOptionAttribute"/> in the provided array of <see cref="Assembly"/>
/// </summary>
public static class RegisterOptionsPatternExtension
{
    private const string MICROSOFT_NAMESPACE = "microsoft";
    private const string SYSTEM_NAMESPACE = "system";
    private static readonly MethodInfo _configureOptionsMethodInfo =
        MethodReflectionHelper.GetMethodInfo(typeof(RegisterOptionsPatternExtension),
            nameof(RegisterOptionsPatternExtension.ConfigureOptions),
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

    /// <summary>
    /// Registers as <see cref="IOptions{TOptions}"/> every <see cref="Type"/>
    /// marked with <see cref="RegisterOptionAttribute"/> in the provided array of <see cref="Assembly"/>
    /// </summary>
    /// <param name="services">The Microsoft <see cref="IServiceCollection"/></param>
    /// <param name="configuration">The Microsoft <see cref="IConfiguration"/></param>
    /// <param name="assemblies">Assemblies where to look for the types to register</param>
    /// <exception cref="ArgumentException">If no assembly has been passed as an argument</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="assemblies"/> or <paramref name="configuration"/>are null</exception>
    public static void RegisterOptionsPatterns(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);
        ArgumentNullException.ThrowIfNull(configuration);

        if (assemblies.Length <= 0)
            throw new ArgumentException("Need at least one assembly to register OptionsPattern from assemblies");

        var optionsPatternTypes = GetOptionsPatternTypes(assemblies);

        foreach (var type in optionsPatternTypes)
        {
            MethodReflectionHelper.InvokeGenericMethod(type, _configureOptionsMethodInfo, services, configuration,
                type.GetCustomAttribute<RegisterOptionAttribute>()!);
        }
    }

    private static IEnumerable<Type> GetOptionsPatternTypes(IEnumerable<Assembly> assemblies)
    {
        return assemblies.Where(a =>
        {
            var assemblyName = a.GetName().Name;
            return !string.IsNullOrWhiteSpace(assemblyName) && !(IsMicrosoftAssembly(assemblyName) && IsSystemAssembly(assemblyName));
        }).SelectMany(assembly => assembly.GetTypes().Where(t => IsOptionsPatternType(t)));
    }

    private static void ConfigureOptions<T>(IServiceCollection services, IConfiguration configuration, RegisterOptionAttribute attribute) where T : class
        => services.AddOptions<T>().Bind(attribute.RegisterOptionType is OptionType.Environment ?
                configuration : GetConfigurationSection(typeof(T), configuration))
                .ConfigureDataAnnotations(attribute.UseDataAnnotations)
                .ConfigureValidateOnStart(attribute.ValidateOnStart);

    private static OptionsBuilder<T> ConfigureDataAnnotations<T>(this OptionsBuilder<T> optionsBuilder, bool useDataAnnotations) where T : class
        => useDataAnnotations ? optionsBuilder.ValidateDataAnnotations() : optionsBuilder;

    private static OptionsBuilder<T> ConfigureValidateOnStart<T>(this OptionsBuilder<T> optionsBuilder, bool validateOnStart) where T : class
        => validateOnStart ? optionsBuilder.ValidateOnStart() : optionsBuilder;

    private static IConfigurationSection GetConfigurationSection<T>(T type, IConfiguration configuration) where T : class
        => configuration.GetSection(type.GetType().Name);

    private static bool IsMicrosoftAssembly(string assemblyName)
        => assemblyName.StartsWith(MICROSOFT_NAMESPACE, StringComparison.OrdinalIgnoreCase);

    private static bool IsSystemAssembly(string assemblyName)
        => assemblyName.StartsWith(SYSTEM_NAMESPACE, StringComparison.OrdinalIgnoreCase);

    private static bool IsOptionsPatternType(Type type)
        => type.GetCustomAttribute<RegisterOptionAttribute>() is not null &&
                            type.GetCustomAttribute<RegisterOptionAttribute>()!.RegisterOptionType
                            .HasFlag(OptionType.Settings | OptionType.Environment);
}