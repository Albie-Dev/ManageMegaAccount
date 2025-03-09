using System.ComponentModel;
using System.Reflection;

namespace MMA.Domain
{
    public static class EnumExtension
    {
        public static string ToDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}