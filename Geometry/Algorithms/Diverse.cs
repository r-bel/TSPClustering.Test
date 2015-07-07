using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Geometry
{
    public static class Diverse
    {
        public static AxisAlignedRectangle BoundingRectangle(this IEnumerable<ICartesianCoordinate> pointcloud)
        {
            if (pointcloud == null)
                throw new ArgumentNullException("pointcloud");

            var minX = pointcloud.Min(c => c.X);
            var minY = pointcloud.Min(c => c.Y);
            var maxX = pointcloud.Max(c => c.X);
            var maxY = pointcloud.Max(c => c.Y);

            return AxisAlignedRectangle.FromLTRB(minX, maxY, maxX, minY);
        }

        public static double DistanceSquared(ICartesianCoordinate p1, ICartesianCoordinate p2)
        {
            var diffX = p2.X - p1.X;
            var diffY = p2.Y - p1.Y;
            return diffX * diffX + diffY * diffY;
        }

        public static ICartesianCoordinate Mean(this IEnumerable<ICartesianCoordinate> pointcloud)
        {
            var X = pointcloud.Average(p => p.X);
            var Y = pointcloud.Average(p => p.Y);

            return new Point(X, Y);
        }
    }
}
