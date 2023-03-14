using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Wurs.Extensions.ServiceCollection.Atributtes;
using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection;
/// <summary>
/// Registers as <see cref="IOptions{TOptions}"/> every <see cref="Type"/>
/// marked with <see cref="RegisterOptionAttribute"/> in the provided array of <see cref="Assembly"/>
/// </summary>
public static class RegisterOptionsPatternExtension
{
    private const string MICROSOFT = "microsoft";
    private const string SYSTEM = "system";
    private static readonly MethodInfo _configureGenericMethod =
        GetBaseMethod(nameof(RegisterOptionsPatternExtension.ConfigureGeneric));

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
        ArgumentNullException.ThrowIfNull(assemblies);
        ArgumentNullException.ThrowIfNull(configuration);

        if (assemblies.Length <= 0)
            throw new ArgumentException("Need at least one assembly to register Options pattern from assemblies");

        GetTypesToRegister(assemblies).ToList().ForEach(type =>
        {
            var attribute = type.GetCustomAttribute<RegisterOptionAttribute>()!;
            Configure(attribute, type, services, configuration);
        });
    }

    private static IEnumerable<Type> GetTypesToRegister(IEnumerable<Assembly> assemblies)
    {
        assemblies = ExcludeAssemblies(assemblies);

        foreach (var assembly in assemblies)
        {
            var types = ExcludeTypes(assembly.GetTypes());

            foreach (var type in types)
                yield return type;
        }
    }

    private static void Configure(RegisterOptionAttribute attribute, Type type, IServiceCollection services, IConfiguration configuration)
        => _configureGenericMethod.MakeGenericMethod(type)
                     .Invoke(null, new object[] { services, configuration, attribute });

    private static void ConfigureGeneric<T>(IServiceCollection services, IConfiguration configuration,
        RegisterOptionAttribute attribute) where T : class
    {
        var builder = services.AddOptions<T>();

        if (attribute.RegisterOptionType == OptionType.Environment)
            builder
            .Bind(configuration);
        else
            builder.Bind(GetConfigurationSection(typeof(T), configuration));

        builder.ShouldUseDataAnnotations(attribute.UseDataAnnotations)
            .ShouldValidateOnStart(attribute.ValidateOnStart);
    }

    private static OptionsBuilder<T> ShouldUseDataAnnotations<T>(this OptionsBuilder<T> optionsBuilder, bool useDataAnnotations) where T : class
        => useDataAnnotations is true ? optionsBuilder.ValidateDataAnnotations() : optionsBuilder;

    private static OptionsBuilder<T> ShouldValidateOnStart<T>(this OptionsBuilder<T> optionsBuilder, bool validateOnStart) where T : class
        => validateOnStart is true ? optionsBuilder.ValidateOnStart() : optionsBuilder;

    private static IConfigurationSection GetConfigurationSection<T>(T type, IConfiguration configuration) where T : class
        => configuration.GetSection(type.GetType().Name);

    private static MethodInfo GetBaseMethod(string methodName)
        => typeof(RegisterOptionsPatternExtension)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)!;

    private static IEnumerable<Assembly> ExcludeAssemblies(IEnumerable<Assembly> assemblies)
        => assemblies.Where(a =>
        {
            var assemblyName = a.GetName().Name!.ToLower();
            return !assemblyName.Contains(MICROSOFT) &&
                              !assemblyName.ToLower().Contains(SYSTEM);
        });

    private static IEnumerable<Type> ExcludeTypes(IEnumerable<Type> types)
        => types.Where(t => t.GetCustomAttribute<RegisterOptionAttribute>() is not null &&
                            t.GetCustomAttribute<RegisterOptionAttribute>()!.RegisterOptionType
                            .HasFlag(OptionType.Settings | OptionType.Environment));
}