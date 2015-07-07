
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class ConvexHull 
    {
        private class PointComparerForMonotoneChainAlgorithm : IComparer<ICartesianCoordinate>
        {
            public int Compare(ICartesianCoordinate a, ICartesianCoordinate b)
            {
                return a.X < b.X || (a.X == b.X && a.Y < b.Y) ? -1 : 1;
            }
        }

        // Monotone Chain algoritm by Andrew 1979
        public ICartesianCoordinate[] Algorithm(IEnumerable<ICartesianCoordinate> pointcloud)
        {
            var pointcloudArray = pointcloud.ToArray();
            
            if (pointcloudArray.Length > 1)
            {
                var hull = new ICartesianCoordinate[2 * pointcloudArray.Length];

                Array.Sort(pointcloudArray, new PointComparerForMonotoneChainAlgorithm());

                int k = 0;

                // Lower hull
                for (int i = 0; i < pointcloudArray.Length; i++)
                {
                    while (k >= 2 && GoniometryAlgorithms.CrossProduct(hull[k - 2], hull[k - 1], pointcloudArray[i]) <= 0)
                        k--;
                    hull[k++] = pointcloudArray[i];
                }

                // Upper hull
                for (int i = pointcloudArray.Length - 2, t = k + 1; i >= 0; i--)
                {
                    while (k >= t && GoniometryAlgorithms.CrossProduct(hull[k - 2], hull[k - 1], pointcloudArray[i]) <= 0)
                        k--;
                    hull[k++] = pointcloudArray[i];
                }

                Array.Resize(ref hull, k - 1);

                return hull;
            }
            else if (pointcloudArray.Length <= 1)
            {
                return pointcloudArray;
            }
            else
            {
                return null;
            }
        }
    }
}
