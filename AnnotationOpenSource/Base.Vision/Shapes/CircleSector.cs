using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Base.Vision.Shapes
{
    public class CircleSector : Circle
    {
        // Fields.
        private double m_StartAngleDeg;
        private double m_EndAngleDeg;
        // Properties.
        [DisplayName("Start angle (°)")]
        public double StartAngleDeg
        {
            get
            {
                return m_StartAngleDeg;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_StartAngleDeg != value)
                {
                    m_StartAngleDeg =
                        value > AngleDegMax ? AngleDegMax :
                        value < AngleDegMin ? AngleDegMin :
                        value;
                    SetProperty(ref m_StartAngleDeg, value);
                }
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public double StartAngleRad
        {
            get
            {
                return StartAngleDeg * Math.PI / 180;
            }
        }
        [DisplayName("End angle (°)")]
        public double EndAngleDeg
        {
            get
            {
                return m_EndAngleDeg;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_EndAngleDeg != value)
                {
                    m_EndAngleDeg =
                        value > AngleDegMax ? AngleDegMax :
                        value < AngleDegMin ? AngleDegMin :
                        value;
                    SetProperty(ref m_EndAngleDeg, value);
                }
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public double EndAngleRad
        {
            get
            {
                return EndAngleDeg * Math.PI / 180;
            }
        }

        // ctor.
        public CircleSector()
        {
            Name = Const.CircleSector;
            // Default values.
            Row = 50;
            Column = 50;
            Radius = 50;
            StartAngleDeg = 0;
            EndAngleDeg = 180;
        }

        // Overrides.
        [Browsable(false)]
        [XmlIgnore]
        public override List<double> Parameters
        {
            get
            {
                // IMPORTANT:
                // Ordering of the value does matters as they
                // are corresponding to HALCON's counterpart.
                // Also, the UoM of angle related value is in radian.
                return new List<double>() { Row, Column, Radius, StartAngleRad, EndAngleRad };
            }
        }
    }
}
