using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class AxisAlignedRectangle : IEnumerable<ICartesianCoordinate>
    {
        private readonly ICartesianCoordinate point1, point2, point3, point4;
        
        private AxisAlignedRectangle(ICartesianCoordinate point1, ICartesianCoordinate point2, ICartesianCoordinate point3, ICartesianCoordinate point4)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
            this.point4 = point4;
        }

        public static AxisAlignedRectangle FromLTRB(double left, double top, double right, double bottom)
        {
            return new AxisAlignedRectangle(Point.From(left, bottom), Point.From(left, top), Point.From(right, top), Point.From(right, bottom));
        }
        public double Area
        {
            get
            {
                return Width * Height;
            }
        }

        public double Height
        {
            get
            {
                return System.Math.Abs(point1.Y - point3.Y) + 1;
            }
        }
        public double Width
        {
            get
            {
                return System.Math.Abs(point1.X - point3.X) + 1;
            }
        }

        public ICartesianCoordinate LowerLeft
        {
            get { return point1; }
        }

        public ICartesianCoordinate UpperLeft
        {
            get { return point2; }
        }

        public ICartesianCoordinate UpperRight
        {
            get { return point3; }
        }

        public ICartesianCoordinate LowerRight
        {
            get { return point4; }
        }

        public override string ToString()
        {
            return string.Format("[P1 {0}; P2 {1}; P3 {2}; P4 {3}]", point1, point2, point3, point4);
        }

        public IEnumerator<ICartesianCoordinate> GetEnumerator()
        {
            foreach (var polygonPoint in new [] { point1, point2, point3, point4 })
                yield return polygonPoint;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
