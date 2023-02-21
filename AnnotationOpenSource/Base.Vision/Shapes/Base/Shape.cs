using Base.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Base.Vision.Shapes.Base
{
    [Serializable]
    [XmlInclude(typeof(Rectangle1))]
    [XmlInclude(typeof(Rectangle2))]
    [XmlInclude(typeof(Circle))]
    [XmlInclude(typeof(CircleSector))]
    [XmlInclude(typeof(Ellipse))]
    [XmlInclude(typeof(EllipseSector))]
    [XmlInclude(typeof(XLD))]
    [XmlInclude(typeof(Line))]
    public abstract class Shape : NotifyPropertyChangedBase
    {
        // Static.
        public static Type[] DerivedTypes
        {
            // The purpose of this static properties is to provide a
            // common source for serializer to get the types that they
            // will serialize.
            get
            {
                return new Type[]
                {
                    typeof(Rectangle1),
                    typeof(Rectangle2),
                    typeof(Circle),
                    typeof(CircleSector),
                    typeof(Ellipse),
                    typeof(EllipseSector),
                    typeof(XLD),
                    typeof(Line)
                };
            }
        }

        // Constants.
        protected const int Decimals = 2;
        protected const double RowMin = 0;
        protected const double RowMax = 9999;
        protected const double ColumnMin = 0;
        protected const double ColumnMax = 9999;
        protected const double RadiusMin = 0;
        protected const double RadiusMax = 9999;
        protected const double LengthMin = 0;
        protected const double LengthMax = 9999;
        protected const double AngleDegMin = -360;
        protected const double AngleDegMax = 360;

        // Properties.
        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }
        // Abstract Properties.
        [XmlIgnore]
        public abstract List<double> Parameters
        {
            get;
        }
        // Overrides.
        /*[Browsable(false)]
        public override bool IsNotifying
        {
            // Overriden just for the sake of decorating it with BrowsableAttribute.
            get
            {
                return base.IsNotifying;
            }
            set
            {
                base.IsNotifying = value;
            }
        }*/

        // Methods.
        /// <summary>
        /// Update properties of current instance from the specified source instance.
        /// </summary>
        /// <param name="sourceShape">Source instance which its properties will be updated to current instance.</param>
        public void UpdateFrom(Shape sourceShape)
        {
            Debug.Assert(this.GetType() == sourceShape.GetType(),
                string.Format("Source instance's type must matches current instance type. Expected: {0}, Actual: {1}", this.GetType(), sourceShape.GetType()));

            if (Name == Const.Circle)
            {
                var target = this as Circle;
                var source = sourceShape as Circle;
                // Update data.
                target.Row = source.Row;
                target.Column = source.Column;
                target.Radius = source.Radius;
            }
            else if (Name == Const.CircleSector)
            {
                var target = this as CircleSector;
                var source = sourceShape as CircleSector;
                // Update data.
                target.Row = source.Row;
                target.Column = source.Column;
                target.Radius = source.Radius;
                target.StartAngleDeg = source.StartAngleDeg;
                target.EndAngleDeg = source.EndAngleDeg;
            }
            else if (Name == Const.Ellipse)
            {
                var target = this as Ellipse;
                var source = sourceShape as Ellipse;
                // Update data.
                target.Row = source.Row;
                target.Column = source.Column;
                target.AngleDeg = source.AngleDeg;
                target.Radius1 = source.Radius1;
                target.Radius2 = source.Radius2;
            }
            else if (Name == Const.EllipseSector)
            {
                var target = this as EllipseSector;
                var source = sourceShape as EllipseSector;
                // Update data.
                target.Row = source.Row;
                target.Column = source.Column;
                target.AngleDeg = source.AngleDeg;
                target.Radius1 = source.Radius1;
                target.Radius2 = source.Radius2;
                target.StartAngleDeg = source.StartAngleDeg;
                target.EndAngleDeg = source.EndAngleDeg;
            }
            else if (Name == Const.Line)
            {
                var target = this as Line;
                var source = sourceShape as Line;
                // Update data.
                target.RowBegin = source.RowBegin;
                target.ColumnBegin = source.ColumnBegin;
                target.RowEnd = source.RowEnd;
                target.ColumnEnd = source.ColumnEnd;
            }
            else if (Name == Const.Rectangle1)
            {
                var target = this as Rectangle1;
                var source = sourceShape as Rectangle1;
                // Update data.
                target.Row1 = source.Row1;
                target.Column1 = source.Column1;
                target.Row2 = source.Row2;
                target.Column2 = source.Column2;
            }
            else if (Name == Const.Rectangle2)
            {
                var target = this as Rectangle2;
                var source = sourceShape as Rectangle2;
                // Update data.
                target.Row = source.Row;
                target.Column = source.Column;
                target.AngleDeg = source.AngleDeg;
                target.Length1 = source.Length1;
                target.Length2 = source.Length2;
            }
            else if (Name == Const.XLD)
            {
                throw new NotSupportedException();
            }
        }

        // ctor.
        public Shape()
        {
        }

    }
}
