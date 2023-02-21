using Base.Common;
using Base.Vision.Shapes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Vision.Tool.Base
{
    public class BaseConfig : NotifyPropertyChangedBase
    {
        public BaseConfig()
        {

        }
        public virtual void UpdateTeachRegion() { }
        public virtual void UpdateEnableRegions() { }
        public virtual void ChangeShape(string Name, Shape Shape) { }
        public virtual void ChangeEnableShape(string Name, bool state) { }

    }
}
