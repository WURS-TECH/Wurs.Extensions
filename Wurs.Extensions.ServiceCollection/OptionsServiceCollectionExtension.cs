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
public static class OptionsServiceCollectionExtension
{
    private const string MICROSOFT = "microsoft";
    private const string SYSTEM = "system";
    private static readonly MethodInfo _configureGenericMethod =
        GetBaseMethod(nameof(OptionsServiceCollectionExtension.ConfigureGeneric));

    /// <summary>
    /// Registers as <see cref="IOptions{TOptions}"/> every <see cref="Type"/>
    /// marked with <see cref="RegisterOptionAttribute"/> in the provided array of <see cref="Assembly"/>
    /// </summary>
    /// <param name="services">The Microsoft <see cref="IServiceCollection"/></param>
    /// <param name="configuration">The Microsoft <see cref="IConfiguration"/></param>
    /// <param name="assemblies">Assemblies where to look for the types to register</param>
    /// <exception cref="ArgumentException">If no assembly has been passed as argument</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="assemblies"/> or <paramref name="configuration"/>are null</exception>
    public static void RegisterOptionsPatterns(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);
        ArgumentNullException.ThrowIfNull(configuration);

        if (assemblies.Length <= 0)
            throw new ArgumentException("Need at least one assembly to register Options pattern from assemblies");

        if (assemblies.Any())
            GetTypesToRegister(assemblies).ToList().ForEach(type =>
            {
                var attribute = type.GetCustomAttribute<RegisterOptionAttribute>()!;
                Configure(attribute, type, services, configuration);
            });
    }

    private static IEnumerable<Type> GetTypesToRegister(Assembly[] assemblies)
    {
        assemblies = ExcludeAssemblies(assemblies);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<RegisterOptionAttribute>() is not null &&
                    t.GetCustomAttribute<RegisterOptionAttribute>()!.RegisterOptionType
                    .HasFlag(OptionType.Settings | OptionType.Environment));

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

    private static OptionsBuilder<T> ShouldUseDataAnnotations<T>(this OptionsBuilder<T> optionsBuilder, bool UseDataAnnotations) where T : class
        => UseDataAnnotations is true ? optionsBuilder.ValidateDataAnnotations() : optionsBuilder;

    private static OptionsBuilder<T> ShouldValidateOnStart<T>(this OptionsBuilder<T> optionsBuilder, bool ValidateOnStart) where T : class
        => ValidateOnStart is true ? optionsBuilder.ValidateOnStart() : optionsBuilder;

    private static IConfigurationSection GetConfigurationSection<T>(T type, IConfiguration configuration) where T : class
        => configuration.GetSection(type.GetType().Name);

    private static MethodInfo GetBaseMethod(string methodName)
        => typeof(OptionsServiceCollectionExtension)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)!;

    private static Assembly[] ExcludeAssemblies(Assembly[] assemblies)
        => assemblies.Where(a =>
                    !a.GetName().Name!.ToLower().Contains(MICROSOFT) &&
                    !a.GetName().Name!.ToLower().Contains(SYSTEM))
                    .ToArray();
}