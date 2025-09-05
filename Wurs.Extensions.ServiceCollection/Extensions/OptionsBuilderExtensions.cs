using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wurs.Extensions.ServiceCollection.Enums;
using Wurs.Extensions.ServiceCollection.Attributes;
using System.Reflection;

namespace Wurs.Extensions.ServiceCollection.Extensions;

internal static class OptionsBuilderExtensions
{
    internal static OptionsBuilder<T> Bind<T>(this OptionsBuilder<T> builder, IConfiguration configuration, OptionType optionType) where T : class
    {
        if (optionType == OptionType.Environment)
            return builder.Bind(configuration);

        var keyName = typeof(T).GetCustomAttribute<ConfigurationKeyNameAttribute>()?.Name
            ?? typeof(T).Name;

        var section = configuration.GetSection(keyName);
        return builder.Bind(section);
    }

    internal static OptionsBuilder<T> ConfigureDataAnnotations<T>(this OptionsBuilder<T> optionsBuilder,
                                                                 bool useDataAnnotations) where T : class
        => useDataAnnotations ? optionsBuilder.ValidateDataAnnotations() : optionsBuilder;

    internal static OptionsBuilder<T> ConfigureValidateOnStart<T>(this OptionsBuilder<T> optionsBuilder,
        bool validateOnStart) where T : class
        => validateOnStart ? optionsBuilder.ValidateOnStart() : optionsBuilder;
}