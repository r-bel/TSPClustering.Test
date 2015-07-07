using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Point : ICartesianCoordinate
    {
        double x, y;

        public Point (double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point From(double x, double y)
        {
            return new Point(x, y);
        }

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }

        public override string ToString()
        {
            return string.Format("({0};{1})", x, y);
        }

        public bool Equals(ICartesianCoordinate other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}
