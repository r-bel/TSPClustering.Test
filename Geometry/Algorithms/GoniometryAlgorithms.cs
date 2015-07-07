using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Geometry
{
    public static class GoniometryAlgorithms
    {
        private class RingWalker : IEnumerable<RingWalker.Position>
        {
            public struct Position
            {
                public ICartesianCoordinate prevprev, prev, curr;
            }


            private IEnumerable<ICartesianCoordinate> ring;
            public RingWalker(IEnumerable<ICartesianCoordinate> ring)
            {
                this.ring = ring;
            }

            public IEnumerator<Position> GetEnumerator()
            {
                Position pos = new Position();
                pos.prevprev = ring.First();
                pos.prev = ring.Skip(1).First();
                foreach (var r in ring.Skip(2).Union(new[] { pos.prevprev, pos.prev }))
                {
                    pos.curr = r;
                    yield return pos;
                    pos.prevprev = pos.prev;
                    pos.prev = r;
                }                    
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
        

        //// TODO check gedoe met includeborder
        //public static bool IsPointInsideConvexRing(IEnumerable<ICartesianCoordinate> convexRing, ICartesianCoordinate point, bool includeBorder = true)
        //{
        //    //var boundingRectangle = GetBoundingRectangle(convexRing);

        //    //// obvious cases
        //    //if (point.X < boundingRectangle.LowerLeft.X || point.X > boundingRectangle.UpperRight.X)
        //    //    return false;

        //    //if (point.Y < boundingRectangle.LowerLeft.Y || point.Y > boundingRectangle.UpperRight.Y)
        //    //    return false;

        //    foreach (var coordinate in new RingWalker(convexRing)) // .Skip(1).Union(new [] { prev}))
        //    {
        //        var t = new Triangle(coordinate.prevprev, coordinate.prev, point);

        //        var isOnBorder = t.IsFlattened();

        //        // TODO optimaliseer deze ifs
        //        if ((!point.Equals(coordinate.prevprev) || !includeBorder) && (!point.Equals(coordinate) || !includeBorder) && !t.RightOrientation() && (!isOnBorder || !includeBorder))
        //            return false;
        //    }
        //    return true;
        //}

        //public static bool IsPointInsideRing_Trivial(IEnumerable<ICartesianCoordinate> ring, ICartesianCoordinate point)
        //{
        //    var convexring = new RingWalker(ring).Where(t => new Triangle(t.prevprev, t.prev, t.curr).RightOrientation());
        //    var concavePoints = new RingWalker(ring).Where(t => !new Triangle(t.prevprev, t.prev, t.curr).RightOrientation());

        //    if (IsPointInsideConvexRing(convexring.Select(t => t.prev), point))
        //    {
        //        foreach(var concavePoint in concavePoints)
        //        {
        //            if (IsPointInsideConvexRing(new[] {concavePoint.prevprev, concavePoint.curr, concavePoint.prev}, point, false))
        //                return false;
        //        }
        //        return true;
        //    }

        //    return false;
        //}
        public static double CrossProduct(ICartesianCoordinate origin, ICartesianCoordinate P1, ICartesianCoordinate P2)
        {
            return ( (P1.X - origin.X) * (P2.Y - origin.Y) - (P2.X -  origin.X) * (P1.Y - origin.Y) );
        }

        //public static bool IsPointInsideRing_WindingNumber(IEnumerable<ICartesianCoordinate> ring, ICartesianCoordinate point)
        //{

        //    //var boundingRectangle = GetBoundingRectangle(ring);

        //    //// obvious cases
        //    //if (point.X < boundingRectangle.LowerLeft.X || point.X > boundingRectangle.UpperRight.X)
        //    //    return false;

        //    //if (point.Y < boundingRectangle.LowerLeft.Y || point.Y > boundingRectangle.UpperRight.Y)
        //    //    return false;
            
            
            
        //    int windingNumberCount = 0;    // the  winding number counter

        //    var ringwalker = new RingWalker(ring);

        //    foreach(var pointInRing in ringwalker)
        //    {
        //        if (pointInRing.curr.Equals(point)) 
        //            return true;

        //        if (pointInRing.prev.Y <= point.Y)
        //        {          // start y <= P.y
        //            if (pointInRing.curr.Y > point.Y &&      // an upward crossing
        //                !new Triangle(pointInRing.prev, pointInRing.curr, point).RightOrientation())  // P left of  edge
        //                    ++windingNumberCount;            // have  a valid up intersect
        //        }
        //        else
        //        {                        // start y > P.y (no test needed)
        //            if (pointInRing.curr.Y <= point.Y &&     // a downward crossing
        //                 new Triangle(pointInRing.prev, pointInRing.curr, point).RightOrientation())  // P right of  edge
        //                    --windingNumberCount;            // have  a valid down intersect
        //        }
        //    }
        //    return windingNumberCount != 0;
        //}

        // Winding ring algoritm with few extensions and optimalizations. Very fast.
        // Bounding rectangle can be used in combination with intelligent polygon but otherwise too time consuming.
        // Other optimalizations still possible. Same remark. Ex. Monotone polygons etc...
        public static bool IsPointInsideRing(IList<ICartesianCoordinate> ring, ICartesianCoordinate point/*, bool includeBorder = true*/)
        {
            bool flipflop = false;
            const bool includeBorder = true; // Algoritm could be parameterized to optionally include the border.

            for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
            {
                if (ring[j].Equals(point))
                    return includeBorder;
                
                bool b = ring[i].Y <= point.Y;
                if (b != (ring[j].Y <= point.Y))
                {
                    var triangularOrientation = (ring[j].X - ring[i].X) * (point.Y - ring[i].Y) - (ring[j].Y - ring[i].Y) * (point.X - ring[i].X);
                    //var triangularOrientation = new Triangle(ring[i], ring[j], point).Orientation();
                    if (triangularOrientation > 0 && b || triangularOrientation < 0 && !b)
                        flipflop = !flipflop;
                    else if (triangularOrientation == 0)
                        return includeBorder;
                }
            }
            return flipflop;
        }

        //public static bool Contains(IList<GeoCoordinate> Coordinates, GeoCoordinate coordinate)
        //{
        //    bool flipflop = false;
        //    const bool includeBorder = true; // Algoritm could be parameterized to optionally include the border.

        //    for (int i = 0, j = Coordinates.Count - 1; i < Coordinates.Count; j = i++)
        //    {
        //        if (Coordinates[j].Equals(coordinate))
        //            return includeBorder;

        //        bool b = Coordinates[i].Latitude <= coordinate.Latitude;
        //        if (b != (Coordinates[j].Latitude <= coordinate.Latitude))
        //        {
        //            var triangularOrientation = (Coordinates[j].Longitude - Coordinates[i].Longitude) * (coordinate.Latitude - Coordinates[i].Latitude) - (Coordinates[j].Latitude - Coordinates[i].Latitude) * (coordinate.Longitude - Coordinates[i].Longitude);
        //            if (triangularOrientation > 0 && b || triangularOrientation < 0 && !b)
        //                flipflop = !flipflop;
        //            else if (triangularOrientation == 0)
        //                return includeBorder;
        //        }
        //    }
        //    return flipflop;
        //}


        //public static bool Contains2(IList<PointF2D> Coordinates, GeoCoordinate coordinate)
        //{
        //    bool flipflop = false;
        //    const bool includeBorder = true; // Algoritm could be parameterized to optionally include the border.

        //    for (int i = 0, j = Coordinates.Count - 1; i < Coordinates.Count; j = i++)
        //    {
        //        if (Coordinates[j].Equals(coordinate))
        //            return includeBorder;

        //        bool b = Coordinates[i][1] <= coordinate.Latitude;
        //        if (b != (Coordinates[j][1] <= coordinate.Latitude))
        //        {
        //            var triangularOrientation = (Coordinates[j][0] - Coordinates[i][0]) * (coordinate.Latitude - Coordinates[i][1]) - (Coordinates[j][1] - Coordinates[i][1]) * (coordinate.Longitude - Coordinates[i][0]);
        //            if (triangularOrientation > 0 && b || triangularOrientation < 0 && !b)
        //                flipflop = !flipflop;
        //            else if (triangularOrientation == 0)
        //                return includeBorder;
        //        }
        //    }
        //    return flipflop;
        //}


        public static bool IsLineSegmentInsideRing(IList<ICartesianCoordinate> ring, ICartesianCoordinate pointA, ICartesianCoordinate pointB, bool includeBorder = true)
        {
            for (int i = 0; i < ring.Count - 1; i++)
            {
                var test1 = new Triangle(ring[i], ring[i + 1], pointA).Orientation() * new Triangle(ring[i], ring[i+1], pointB).Orientation();
                var test2 = new Triangle(pointA, pointB, ring[i]).Orientation() * new Triangle(pointA, pointB, ring[i + 1]).Orientation();
                if ((test1 <= 0) && (test2 <= 0))
                    return false;
            }

            return IsPointInsideRing(ring, pointA/*, includeBorder*/) && IsPointInsideRing(ring, pointB/*, includeBorder*/);
        }

        //public static bool IsRectangleInsideRing(ICartesianCoordinate[] ring, IRectangle rectangle)
        //{
        //    return (IsPointInsideRing(ring, rectangle.UpperLeft) &&
        //        IsPointInsideRing(ring, rectangle.UpperRight) &&
        //        IsPointInsideRing(ring, rectangle.LowerLeft) &&
        //        IsPointInsideRing(ring, rectangle.LowerRight));
        //}

        public static bool IsPerpendicular(ICartesianCoordinate pointA, ICartesianCoordinate pointB, ICartesianCoordinate pointC, ICartesianCoordinate pointD)
        {
            if (pointB.Y.Equals(pointA.Y) && pointD.X.Equals(pointC.X) || pointB.X.Equals(pointA.X) && pointD.Y.Equals(pointC.Y))
                return true;                        
            
            var rico1 = (pointB.Y - pointA.Y) / (pointB.X - pointA.X);
            var rico2 = (pointD.Y - pointC.Y) / (pointD.X - pointC.X);

            return rico1 * rico2 == -1;
        }

        public static double AreaOfRing(ICartesianCoordinate[] ring)
        {
            double area = 0;
            
            for (int i = 0, j = ring.Length - 1; i < ring.Length; j = i++)
            {
                area += (ring[i].X - ring[j].X) * System.Math.Abs(ring[j].Y + ring[i].Y) / 2;
            }

            return area;
        }

        //public static IList<ICartesianCoordinate[]> CalculateLargestInnerRectangle_BruteLatticeMethod(IList<ICartesianCoordinate> ring)
        //{
        //    const int partitionsize_X = 20;

        //    var Rmin = GetBoundingRectangle(ring);

        //    var mazeSize = Rmin.Width / partitionsize_X;
        //    int partitionsize_Y = Convert.ToInt32(Rmin.Height / mazeSize);

        //    var lattice = new Point[partitionsize_X * partitionsize_Y];

        //    int N = 0;

        //    for (int x = 0 ; x < partitionsize_X; x++)
        //    {
        //        for (int y = 0 ; y < partitionsize_Y; y++)
        //        {
        //            var point = Point.From(Rmin.UpperLeft.X + x * mazeSize, Rmin.LowerLeft.Y + y * mazeSize);
        //            if (IsPointInsideRing(ring, point/*, true*/))
        //            {
        //                //N++;
        //                lattice[N++] = point;
        //            }
        //        }
        //    }
        //    double maxArea = 0;

        //    var results = new List<ICartesianCoordinate[]>();

        //    for (int i = 0; i < N-2; i++)
        //    {
        //        for (int j = i+1; j < N-1; j++)
        //        {                    
        //            if (IsLineSegmentInsideRing(ring, lattice[i], lattice[j]))
        //            {
        //                for (int k = j+1; k < N; k++)
        //                {
        //                    if (IsLineSegmentInsideRing(ring, lattice[i], lattice[k]))
        //                    {
        //                        if (IsPerpendicular(lattice[i], lattice[j], lattice[i], lattice[k]))
        //                        {
        //                            var fourthPoint = Point.From(lattice[j].X - lattice[i].X + lattice[k].X, lattice[j].Y - lattice[i].Y + lattice[k].Y);
        //                            if (IsLineSegmentInsideRing(ring, lattice[k], fourthPoint) && IsLineSegmentInsideRing(ring, lattice[j], fourthPoint))
        //                            {
        //                                var rectangle = new ICartesianCoordinate[] { lattice[i], lattice[j], fourthPoint, lattice[k] };

        //                                var area = AreaOfRing(rectangle);
        //                                if (area > maxArea)
        //                                {
        //                                    maxArea = area;
        //                                    results.Clear();
        //                                    results.Add(rectangle);
        //                                }
        //                                else if(area == maxArea)
        //                                {
        //                                    results.Add(rectangle);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return results;
        //}

        //public static ICartesianCoordinate[] CalculateLargestInnerRectangle_DirksMethod(IList<ICartesianCoordinate> ring)
        //{
        //    // Should be possible in O(n)?
        //    //

        //    int leftmostLowestPoint = 0;
        //    int rightmostLowestPoint = 0;
        //    for (int i = 1; i < ring.Count; i++ )
        //    {
        //        if (ring[i].Y <= ring[leftmostLowestPoint].Y)   
        //        {
        //            if (ring[i].Y < ring[leftmostLowestPoint].Y || ring[i].X < ring[leftmostLowestPoint].X)
        //                leftmostLowestPoint = i;
        //            if (ring[i].Y < ring[rightmostLowestPoint].Y || ring[i].X > ring[rightmostLowestPoint].X)
        //                rightmostLowestPoint = i;
        //        }
        //    }

        //    //int pointLowerLeft_Index = leftmostLowestPoint;
        //    //int pointLowerRight_Index = rightmostLowestPoint;

        //    var rectangle = new ICartesianCoordinate[] { ring[leftmostLowestPoint], ring[leftmostLowestPoint], ring[rightmostLowestPoint], ring[rightmostLowestPoint] };

        //    for (int pointLowerLeft_Index = leftmostLowestPoint, pointLowerRight_Index = rightmostLowestPoint; ; )
        //    {
        //        var upperLeft = ring[pointLowerLeft_Index + 1];
        //        var upperRight = ring[pointLowerRight_Index - 1];
        //        var lowerLeft = ring[pointLowerLeft_Index];
        //        var lowerRight = ring[pointLowerRight_Index];

        //        if (upperLeft.X > lowerLeft.X && upperRight.X < lowerRight.X) // Monotone narrowing polygons
        //        {
        //            var prevH = rectangle.Max(r => r.Y) - rectangle.Min(r => r.Y);
                    
        //            var mF = (upperLeft.Y - lowerLeft.Y) / (upperLeft.X - lowerLeft.X);
        //            var mG = (upperRight.Y - lowerRight.Y) / (upperRight.X - lowerRight.X);
        //            var mFmG = mF * mG;

        //            //var x = System.Math.Min((2 * mF * lowerLeft.X - mG * (lowerRight.X + lowerLeft.X)) / (2 * (mF - mG)), upperLeft.X);

        //            var x_teller = 2 * mF * mF * lowerLeft.X - mFmG * (lowerRight.X + lowerLeft.X) - (mF - mG) * prevH;
        //            var x_noemer = 2 * (mF * mF - mFmG);
        //            var x = x_teller / x_noemer;

        //            //var a = mF * (x - lowerLeft.X);
        //            var y = mF * (x - lowerLeft.X) + lowerLeft.Y;

        //            if (y > upperLeft.Y)
        //            {
        //                y = upperLeft.Y;
        //                x = upperLeft.X;
        //                //x = (y - lowerLeft.Y + mF * lowerLeft.X) / mF;
        //            }

        //            var xx = (mF * (x - lowerLeft.X)) / mG + lowerRight.X;

        //            if ((xx - x) * (y - lowerLeft.Y) > AreaOfRing(rectangle))
        //                rectangle = new ICartesianCoordinate[] { Point.From(x, lowerLeft.Y), Point.From(x, y), Point.From(xx, y), Point.From(xx, lowerRight.Y) };
        //        }
        //    }
        //    return rectangle;
        //}

        //public static ICartesianCoordinate[] CalculateLargestInnerRectangle_DirksMethod2(Line L1, Line L2, Line L3, Line L4)
        //{
        //    var m1 = L1.Slope;
        //    var m2 = L2.Slope;
        //    //var m3 = L3.Slope;
        //    var m4 = L4.Slope;
        //    var A1 = L1.OffsetY;
        //    var A2 = L2.OffsetY;
        //    //var A3 = L3.OffsetY;
        //    var A4 = L4.OffsetY;

        //    var right = (A2*m1+A1*m4-m4*A2-m1*A4) / (2*m1*m2*(m4-m1));
        //    var top = m2 * right + A2;

        //    //GeometryFactory.CreateAxisAlignedRectangleFromLRTB()
            
        //    return null;
        //}
    }
}
