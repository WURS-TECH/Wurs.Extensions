using Wurs.Extensions.ServiceCollection.Enums;

namespace Wurs.Extensions.ServiceCollection.Models
{
    internal class RegisterOptionModel
    {
        internal Type Type { get; set; }

        internal bool UseDataAnnotations { get; set; }

        internal bool ValidateOnStart { get; set; }

        internal OptionType OptionType { get; set; }
    }
}
