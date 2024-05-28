using Microsoft.Extensions.Options;
using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection.Atributtes;
/// <summary>
/// An <see cref="Attribute"/> used to identify a <see cref="Type"/>
/// as an <see cref="IOptions{TOptions}"/> to be registered.
/// </summary>
/// <remarks>
/// <see cref="RegisterOptionAttribute"/> constructor.
/// </remarks>
/// <param name="registerOptionType"><see cref="OptionType"/></param>
/// <param name="useDataAnnotations">Marks an <see cref="IOptions{TOptions}"/> to use <c>Data Annotations</c></param>
/// <param name="validateOnStart">Marks an <see cref="IOptions{TOptions}"/>to validate <c>Data Annonations</c> on start</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class RegisterOptionAttribute(OptionType registerOptionType,
                                bool useDataAnnotations = false,
                                bool validateOnStart = false) : Attribute
{
    internal readonly OptionType RegisterOptionType = registerOptionType;
    internal readonly bool UseDataAnnotations = useDataAnnotations;
    internal readonly bool ValidateOnStart = validateOnStart;
}
