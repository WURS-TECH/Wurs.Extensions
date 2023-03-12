using Microsoft.Extensions.Options;
using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection.Atributtes;
/// <summary>
/// An <see cref="Attribute"/> used to identify a <see cref="Type"/>
/// as an <see cref="IOptions{TOptions}"/> to be registered.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class RegisterOptionAttribute : Attribute
{
    internal readonly OptionType RegisterOptionType;
    internal readonly bool UseDataAnnotations;
    internal readonly bool ValidateOnStart;
    /// <summary>
    /// <see cref="RegisterOptionAttribute"/> constructor.
    /// </summary>
    /// <param name="registerOptionType"><see cref="OptionType"/></param>
    /// <param name="useDataAnnotations">Marks an <see cref="IOptions{TOptions}"/> to use <c>Data Annotations</c></param>
    /// <param name="validateOnStart">Marks an <see cref="IOptions{TOptions}"/>to validate <c>Data Annonations</c> on start</param>
    public RegisterOptionAttribute(OptionType registerOptionType,
                                    bool useDataAnnotations = false,
                                    bool validateOnStart = false)
    {
        RegisterOptionType = registerOptionType;
        UseDataAnnotations = useDataAnnotations;
        ValidateOnStart = validateOnStart;
    }
}
