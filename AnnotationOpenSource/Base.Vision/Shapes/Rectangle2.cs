using Base.Vision.Shapes.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Base.Vision.Shapes
{
    public class Rectangle2 : Shape
    {
        // Fields.
        private double m_Row;
        private double m_Column;
        private double m_AngleDeg;
        private double m_Length1;
        private double m_Length2;
        // Properties.
        [DisplayName("Row")]
        public double Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Row != value)
                {
                    m_Row =
                        value > RowMax ? RowMax :
                        value < RowMin ? RowMin :
                        value;
                    SetProperty(ref m_Row, value);
                }
            }
        }
        [DisplayName("Column")]
        public double Column
        {
            get
            {
                return m_Column;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Column != value)
                {
                    m_Column =
                        value > ColumnMax ? ColumnMax :
                        value < ColumnMin ? ColumnMin :
                        value;
                    SetProperty(ref m_Column, value);
                }
            }
        }
        [DisplayName("Angle (°)")]
        public double AngleDeg
        {
            get
            {
                return m_AngleDeg;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_AngleDeg != value)
                {
                    m_AngleDeg =
                        value > AngleDegMax ? AngleDegMax :
                        value < AngleDegMin ? AngleDegMin :
                        value;
                    SetProperty(ref m_AngleDeg, value);
                }
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public double AngleRad
        {
            get
            {
                return AngleDeg * Math.PI / 180;
            }
        }
        [DisplayName("Length 1")]
        public double Length1
        {
            get
            {
                return m_Length1;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Length1 != value)
                {
                    m_Length1 =
                        value > LengthMax ? LengthMax :
                        value < LengthMin ? LengthMin :
                        value;
                    SetProperty(ref m_Length1, value);
                }
            }
        }
        [DisplayName("Length 2")]
        public double Length2
        {
            get
            {
                return m_Length2;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Length2 != value)
                {
                    m_Length2 =
                        value > LengthMax ? LengthMax :
                        value < LengthMin ? LengthMin :
                        value;
                    SetProperty(ref m_Length2, value);
                }
            }
        }

        // ctor.
        public Rectangle2()
        {
            Name = Const.Rectangle2;
            // Default values.
            Row = 0;
            Column = 0;
            AngleDeg = 0;
            Length1 = 100;
            Length2 = 150;
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
                return new List<double>() { Row, Column, AngleRad, Length1, Length2 };
            }
        }
    }
}
