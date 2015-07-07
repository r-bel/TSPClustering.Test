using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public struct Triangle
    {
        private ICartesianCoordinate point1, point2, point3;

        public Triangle(ICartesianCoordinate point1, ICartesianCoordinate point2, ICartesianCoordinate point3)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
        }
        
        public double Orientation()
        {
            return (point2.X - point1.X) * (point3.Y - point1.Y) - (point2.Y - point1.Y) * (point3.X - point1.X);
        }

        public bool RightOrientation()
        {
            return Orientation() < 0;
        }
        public bool IsFlattened()
        {
            return (point2.X - point1.X) * (point3.Y - point1.Y) - (point2.Y - point1.Y) * (point3.X - point1.X) == 0;
        }
        public override string ToString()
        {
            return string.Format("[{0};{1};{2}]", point1, point2, point3);
        }

    }
}
