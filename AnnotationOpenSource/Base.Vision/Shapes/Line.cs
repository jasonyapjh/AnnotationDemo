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
    public class Line : Shape
    {
        // Fields.
        private double m_RowBegin;
        private double m_ColumnBegin;
        private double m_RowEnd;
        private double m_ColumnEnd;
        // Properties.
        [DisplayName("Row begin")]
        public double RowBegin
        {
            get
            {
                return m_RowBegin;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_RowBegin != value)
                {
                    m_RowBegin =
                        value > RowMax ? RowMax :
                        value < RowMin ? RowMin :
                        value;
                    SetProperty(ref m_RowBegin, value);
                }
            }
        }
        [DisplayName("Column begin")]
        public double ColumnBegin
        {
            get
            {
                return m_ColumnBegin;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_ColumnBegin != value)
                {
                    m_ColumnBegin =
                        value > ColumnMax ? ColumnMax :
                        value < ColumnMin ? ColumnMin :
                        value;
                    SetProperty(ref m_ColumnBegin, value);
                }
            }
        }
        [DisplayName("Row end")]
        public double RowEnd
        {
            get
            {
                return m_RowEnd;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_RowEnd != value)
                {
                    m_RowEnd =
                        value > RowMax ? RowMax :
                        value < RowMin ? RowMin :
                        value;
                    SetProperty(ref m_RowEnd, value);
                }
            }
        }
        [DisplayName("Column end")]
        public double ColumnEnd
        {
            get
            {
                return m_ColumnEnd;
            }
            set
            {
                value = Math.Round(value, Decimals);

                if (m_ColumnEnd != value)
                {
                    m_ColumnEnd =
                        value > ColumnMax ? ColumnMax :
                        value < ColumnMin ? ColumnMin :
                        value;
                    SetProperty(ref m_ColumnEnd, value);
                }
            }
        }

        // ctor.
        public Line()
        {
            Name = Const.Line;
            // Default values.
            RowBegin = 0;
            ColumnBegin = 0;
            RowEnd = 100;
            ColumnEnd = 100;
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
                return new List<double>() { RowBegin, ColumnBegin, RowEnd, ColumnEnd };
            }
        }
    }
}
