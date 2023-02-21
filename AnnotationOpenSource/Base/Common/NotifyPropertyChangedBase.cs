using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Base.Common
{
    public class NotifyPropertyChangedBase : BindableBase
    {
        protected bool SetDoubleFieldToSixSignificantFigure(ref double field, double value, [CallerMemberName] string propertyName = null)
        {
            var roundedValue = Math.Round(value, 6);
            if (EqualityComparer<double>.Default.Equals(field, roundedValue)) return false;
            field = roundedValue;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
