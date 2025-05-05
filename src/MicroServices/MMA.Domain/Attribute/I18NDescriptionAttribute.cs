using System.ComponentModel;

namespace MMA.Domain
{
    public class I18NDescriptionAttribute : DescriptionAttribute
    {
        public I18NDescriptionAttribute(string description) : base(description)
        {
        }
    }
}