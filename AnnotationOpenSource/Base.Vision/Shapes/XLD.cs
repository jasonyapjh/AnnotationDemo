using Base.Vision.Shapes.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Base.Vision.Shapes
{
    public class XLD : Shape
    {
        // Properties.
        [DisplayName("Points")]
        public List<Point> Points
        {
            get;
            set;
        }

        // ctor.
        public XLD()
        {
            Name = Const.XLD;
            Points = new List<Point>()
            {
                // Default values.
                new Point(100, 0),
                new Point(0, 0),
                new Point(0, 100),
                new Point(100, 100)
            };
        }

        // Overrides.
        [Browsable(false)]
        [XmlIgnore]
        public override List<double> Parameters
        {
            get
            {
                var rows = Points.Select(p => p.Y);
                var cols = Points.Select(p => p.X);

                // IMPORTANT:
                // For XLD, the first half of the values will be the y-positions (rows),
                // and the remaining half of the values will be the x-positions (columns).
                var list = new List<double>();
                list.AddRange(rows);
                list.AddRange(cols);

                return list;
            }
        }
    }
}
