using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Wurs.Extensions.ServiceCollection.Atributtes;
using Wurs.Extensions.ServiceCollection.Enums;
using Wurs.Extensions.ServiceCollection.Models;

namespace Wurs.Extensions.ServiceCollection;

public static class OptionsServiceCollectionExtension
{
    private const string MICROSOFT = "microsoft";
    private const string SYSTEM = "system";

    public static void RegisterOptionsPatterns(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);
        ArgumentNullException.ThrowIfNull(configuration);

        if (assemblies.Length <= 0)
            throw new ArgumentException("Need at least one assembly to register Options pattern from assemblies");

        var configureFromEnvironment = GetBaseMethod(nameof(OptionsServiceCollectionExtension.ConfigureFromEnvironment));
        var configureFromSettings = GetBaseMethod(nameof(OptionsServiceCollectionExtension.ConfigureFromSettings));

        assemblies = ExcludeAssemblies(assemblies);

        foreach (var assembly in assemblies)
        {
            var registerModels = MapToModels(assembly);

            if (registerModels.Any())
                registerModels.ToList()
                    .ForEach(model =>
                    _ = model.OptionType == OptionType.Environment ?
                    configureFromEnvironment!
                    .MakeGenericMethod(model.Type)
                    .Invoke(null, new object[] { services, configuration, model })
                    : configureFromSettings
                    .MakeGenericMethod(model.Type)
                    .Invoke(null, new object[] { services, GetConfigurationSection(model.Type, configuration) }));
        }
    }

    private static void ConfigureFromEnvironment<T>(IServiceCollection services, IConfiguration configuration, RegisterOptionModel model) where T : class
        => services.AddOptions<T>()
            .Bind(configuration)
            .ShouldUseDataAnnotations(model)
            .ShouldValidateOnStart(model);

    private static OptionsBuilder<T> ShouldUseDataAnnotations<T>(this OptionsBuilder<T> optionsBuilder, RegisterOptionModel model) where T : class
        => model.UseDataAnnotations is true ? optionsBuilder.ValidateDataAnnotations() : optionsBuilder;

    private static OptionsBuilder<T> ShouldValidateOnStart<T>(this OptionsBuilder<T> optionsBuilder, RegisterOptionModel model) where T : class
        => model.ValidateOnStart is true ? optionsBuilder.ValidateOnStart() : optionsBuilder;

    private static void ConfigureFromSettings<T>(RegisterOptionModel model, IServiceCollection services, IConfigurationSection configurationSection) where T : class
        => services.AddOptions<T>()
            .Bind(configurationSection)
            .ShouldUseDataAnnotations(model)
            .ShouldValidateOnStart(model);

    private static IConfigurationSection GetConfigurationSection<T>(T type, IConfiguration configuration) where T : class
        => configuration.GetSection(type.GetType().Name);

    private static MethodInfo GetBaseMethod(string methodName)
        => typeof(OptionsServiceCollectionExtension)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)!;

    private static IEnumerable<RegisterOptionModel> MapToModels(Assembly assembly)
    {
        List<RegisterOptionModel> models = new();
        var types = GetTypesToRegister(assembly);

        models.AddRange(MapToModels(types));

        return models;
    }

    private static IEnumerable<Type> GetTypesToRegister(Assembly assembly) =>
        assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<RegisterOptionAttribute>() is not null &&
                t.GetCustomAttribute<RegisterOptionAttribute>()!.RegisterOptionType
                .HasFlag(OptionType.Settings | OptionType.Environment));

    private static IEnumerable<RegisterOptionModel> MapToModels(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RegisterOptionAttribute>();
            yield return new RegisterOptionModel()
            {
                Type = type,
                UseDataAnnotations = attribute!.UseDataAnnotations,
                ValidateOnStart = attribute!.ValidateOnStart,
                OptionType = attribute.RegisterOptionType
            };
        }
    }

    private static Assembly[] ExcludeAssemblies(Assembly[] assemblies)
    {
        return assemblies.Where(a =>
                        !a.GetName().Name!.ToLower().Contains(MICROSOFT) &&
                        !a.GetName().Name!.ToLower().Contains(SYSTEM))
                        .ToArray();
    }
}