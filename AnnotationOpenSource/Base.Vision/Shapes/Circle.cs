using Base.Vision.Shapes.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Base.Vision.Shapes
{
    public class Circle : Shape
    {
        // Fields.
        private double m_Row;
        private double m_Column;
        private double m_Radius;
        // Properties.
        [DisplayName("Row")]
        public double Row
        {
            get { return this.m_Row; }
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
                return this.m_Column;
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
        [DisplayName("Radius")]
        public double Radius
        {
            get
            {
                return this.m_Radius;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Radius != value)
                {
                    m_Radius =
                        value > RadiusMax ? RadiusMax :
                        value < RadiusMin ? RadiusMin :
                        value;
                    SetProperty(ref m_Radius, value);
                }
            }
        }

        // ctor.
        public Circle()
        {
            Name = Const.Circle;
            // Default values.
            Row = 50;
            Column = 50;
            Radius = 50;
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
                return new List<double>() { Row, Column, Radius };
            }
        }
    }
}
