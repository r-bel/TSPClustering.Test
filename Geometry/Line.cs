using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    // Maintains a line with function format  y = F(x) = Slope * x + OffsetY
    // For horizontal lines formula gets y = OffsetY
    // This is not defined for vertical lines.
    // For vertical lines: x = OffsetX
    public class Line
    {
        private double? slope; // null if vertcial line and so rico is not defined

        private double? offsetY; // Not for verticals
        
        private double? offsetX; // Not for horizontals

        private Line(double? slope, ICartesianCoordinate point)
        {
            this.slope = slope;

            offsetX = slope.HasValue ? (slope.Value.Equals(0.0) ? (double?) null : point.X - point.Y / slope.Value) : point.X;
            offsetY = slope.HasValue ? point.Y - slope.Value * point.X : (double?)null;
        }

        public static Line CreateFromTwoPoints(ICartesianCoordinate pointA, ICartesianCoordinate pointB)
        {
            double? slope = pointB.X.Equals(pointA.X) ? (double?)null : (pointB.Y - pointA.Y) / (pointB.X - pointA.X);

            return new Line(slope, pointA);
        }
        public static Line CreateVerticalThroughPoint(ICartesianCoordinate point)
        {
            return new Line(null, point);
        }
        public static Line CreateHorizontalThroughPoint(ICartesianCoordinate point)
        {
            return new Line(0.0, point);
        }

        public static Line CreateFromSlopeAndPoint(double slope, ICartesianCoordinate point)
        {
            return new Line(slope, point);
        }

        public static Line CreateHorizontal(double offsetY)
        {
            return new Line(0.0, Point.From(0, offsetY));
        }

        public static Line CreateVertical(double offsetX)
        {
            return new Line(null, Point.From(offsetX, 0));
        }

        public double YByX(double x)
        {
            return Slope * x + OffsetY;
        }

        public Point PointByX(double x)
        {
            return Point.From(x, YByX(x));
        }

        public Point PointByY(double y)
        {
            return Point.From(OffsetX, y);
        }

        public double Slope 
        { 
            get
            {
                if (IsVertical)
                    throw new ApplicationException("Slope is undefined for vertical line. Please check first");
                return slope.Value;
            }
        } 

        public double OffsetX
        { 
            get
            {
                if (IsHorizontal)
                    throw new ApplicationException("Offset X is undefined for horizontal line. Please check first");
                return offsetX.Value;
            }
        } 

        public double OffsetY
        { 
            get
            {
                if (IsVertical)
                    throw new ApplicationException("Offset Y is undefined for vertical line. Please check first");
                return offsetY.Value;
            }
        }

        public bool IsVertical
        {
            get { return !slope.HasValue; }
        }
        
        public bool IsHorizontal
        {
            get { return slope.HasValue && slope.Value.Equals(0.0); }
        }
    }
}
