using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Wurs.Extensions.ServiceCollection.Attributes;
using Wurs.Extensions.ServiceCollection.Exceptions;
using Wurs.Extensions.ServiceCollection.Extensions;
using Wurs.Extensions.ServiceCollection.Helpers;

namespace Wurs.Extensions.ServiceCollection;
/// <summary>
/// Registers as <see cref="IOptions{TOptions}"/> every <see cref="Type"/>
/// marked with <see cref="RegisterOptionAttribute"/> in the provided array of <see cref="Assembly"/>
/// </summary>
public static class RegisterOptionsPatternExtension
{
    private const BindingFlags DefaultConfiguredFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static readonly MethodInfo? _configureOptionsMethodInfo = typeof(RegisterOptionsPatternExtension)
        .GetMethod(nameof(ConfigureOptions), DefaultConfiguredFlags);

    /// <summary>
    /// Registers as <see cref="IOptions{TOptions}"/> every <see cref="Type"/>
    /// marked with <see cref="RegisterOptionAttribute"/> in the provided array of <see cref="Assembly"/>
    /// </summary>
    /// <param name="services">The Microsoft <see cref="IServiceCollection"/></param>
    /// <param name="configuration">The Microsoft <see cref="IConfiguration"/></param>
    /// <param name="assemblies">Assemblies where to look for the types to register</param>
    /// <exception cref="ArgumentException">If no assembly has been passed as an argument</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="assemblies"/> or <paramref name="configuration"/>are null</exception>
    public static void AddMyOptions(this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        if (_configureOptionsMethodInfo == null)
        {
            throw new RegisterOptionException("Fatal error, contact the contributors on github");
        }

        ThrowHelper.ThrowIfNull(services, configuration, assemblies);

        if (assemblies.Length <= 0)
        {
            throw new RegisterOptionException("Need at least one assembly to register OptionsPattern from assemblies",
                new ArgumentException("Need at least one assembly to register OptionsPattern from assemblies"));
        }

        foreach (var type in GetValidTypesAsDictionary(assemblies))
        {
            _configureOptionsMethodInfo.MakeGenericMethod(type.Key)
                .Invoke(null, [services, configuration, type.Value]);
        }
    }

    private static Dictionary<Type, RegisterOptionAttribute> GetValidTypesAsDictionary(Assembly[] assemblies)
        => assemblies.SelectMany(assembly => assembly.GetTypes())?
        .Filter()?
        .ToDictionary(type => type, type => type.GetCustomAttribute<RegisterOptionAttribute>()!) ?? [];

    private static void ConfigureOptions<T>(IServiceCollection services,
        IConfiguration configuration,
        RegisterOptionAttribute attribute) where T : class
        => services.AddOptions<T>()
                    .Bind(configuration, attribute.RegisterOptionType)
                    .ConfigureDataAnnotations(attribute.UseDataAnnotations)
                    .ConfigureValidateOnStart(attribute.ValidateOnStart);
}