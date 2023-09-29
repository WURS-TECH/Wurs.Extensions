namespace Wurs.Extensions.ServiceCollection.Enums;
/// <summary>
/// Used to identify whether the type should be registered 
/// from the environment or from any configuration file.
/// </summary>
public enum OptionType
{
    /// <summary>
    /// Identifies an option to be registered from configuration
    /// </summary>
    Settings,
    /// <summary>
    /// Identifies an option to be registered from environment
    /// </summary>
    Environment
}
