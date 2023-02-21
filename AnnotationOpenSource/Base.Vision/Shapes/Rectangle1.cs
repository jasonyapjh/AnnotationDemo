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
    public class Rectangle1 : Shape
    {
        // Fields.
        private double m_Row1;
        private double m_Column1;
        private double m_Row2;
        private double m_Column2;
        // Properties.
        [DisplayName("Row 1")]
        public double Row1
        {
            get
            {
                return this.m_Row1;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Row1 != value)
                {
                    m_Row1 =
                        value > RowMax ? RowMax :
                        value < RowMin ? RowMin :
                        value;
                    SetProperty(ref m_Row1, value);
                }
            }
        }
        [DisplayName("Column 1")]
        public double Column1
        {
            get
            {
                return m_Column1;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Column1 != value)
                {
                    m_Column1 =
                        value > ColumnMax ? ColumnMax :
                        value < ColumnMin ? ColumnMin :
                        value;
                    SetProperty(ref m_Column1, value);
                }
            }
        }
        [DisplayName("Row 2")]
        public double Row2
        {
            get
            {
                return m_Row2;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Row2 != value)
                {
                    m_Row2 =
                        value > RowMax ? RowMax :
                        value < RowMin ? RowMin :
                        value;
                    SetProperty(ref m_Row2, value);
                }
            }
        }
        [DisplayName("Column 2")]
        public double Column2
        {
            get
            {
                return m_Column2;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_Column2 != value)
                {
                    m_Column2 =
                        value > ColumnMax ? ColumnMax :
                        value < ColumnMin ? ColumnMin :
                        value;
                    SetProperty(ref m_Column2, value);
                }
            }
        }

        // ctor.
        public Rectangle1()
        {
            Name = Const.Rectangle1;
            // Default values.
            Row1 = 0;
            Column1 = 0;
            Row2 = 100;
            Column2 = 100;
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
                return new List<double>() { Row1, Column1, Row2, Column2 };
            }
        }
    }
}
