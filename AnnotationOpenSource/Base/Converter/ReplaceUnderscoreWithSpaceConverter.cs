using System;
using System.ComponentModel;
using System.Globalization;

namespace Base.Converter
{
    public class ReplaceUnderscoreWithSpaceConverter : EnumConverter
    {
        public ReplaceUnderscoreWithSpaceConverter(Type type) : base(type)
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {

            return value.ToString().Replace('_', ' ');
        }
    }
}
