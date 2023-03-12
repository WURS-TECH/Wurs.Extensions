using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection.Atributtes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class RegisterOptionAttribute : Attribute
{
    internal readonly OptionType RegisterOptionType;
    internal readonly bool UseDataAnnotations;
    internal readonly bool ValidateOnStart;

    public RegisterOptionAttribute(OptionType registerOptionType,
                                    bool useDataAnnotations = false,
                                    bool validateOnStart = false)
    {
        RegisterOptionType = registerOptionType;
        UseDataAnnotations = useDataAnnotations;
        ValidateOnStart = validateOnStart;
    }
}
