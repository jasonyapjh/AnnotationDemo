using Base.Vision.Shapes.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Base.Vision.Shapes
{
    public class Ellipse : Shape
    {
        // Fields.
        private double m_Row;
        private double m_Column;
        private double m_AngleDeg;
        private double m_Radius1;
        private double m_Radius2;
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
        [DisplayName("Radius 1")]
        public double Radius1
        {
            get
            {
                return m_Radius1;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Radius1 != value)
                {
                    m_Radius1 =
                        value > RadiusMax ? RadiusMax :
                        value < RadiusMin ? RadiusMin :
                        value;
                    SetProperty(ref m_Radius1, value);
                }
            }
        }
        [DisplayName("Radius 2")]
        public double Radius2
        {
            get
            {
                return m_Radius2;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Radius2 != value)
                {
                    m_Radius2 =
                        value > RadiusMax ? RadiusMax :
                        value < RadiusMin ? RadiusMin :
                        value;
                    SetProperty(ref m_Radius2, value);
                }
            }
        }

        // ctor.
        public Ellipse()
        {
            Name = Const.Ellipse;
            // Default values.
            Row = 50;
            Column = 50;
            AngleDeg = 0;
            Radius1 = 50;
            Radius2 = 50;
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
                return new List<double>() { Row, Column, AngleRad, Radius1, Radius2 };
            }
        }
    }
}
