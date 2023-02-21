using Base.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Base.Vision.Tool.Base
{
    public abstract class BaseTool : NotifyPropertyChangedBase
    {
        protected bool m_ReadyToInspect = false;
        [Browsable(false)]
        [XmlIgnore]
        public bool ReadyToInspect
        {
            get
            {
                return m_ReadyToInspect;
            }
            set
            {
                m_ReadyToInspect = value;
            }
        }
    }
}
