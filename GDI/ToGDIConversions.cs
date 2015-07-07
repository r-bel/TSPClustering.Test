using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Geometry;
using GDIPoint = System.Drawing.Point;

namespace GDI
{
    public static class ToGDIConversions
    {
        public static Rectangle ToGDI(this AxisAlignedRectangle rectangle)
        {
            return new Rectangle(Convert.ToInt32(rectangle.LowerLeft.X), Convert.ToInt32(rectangle.LowerLeft.Y), Convert.ToInt32(rectangle.Width), Convert.ToInt32(rectangle.Height));
        }

        public static GDIPoint ToGDI(this ICartesianCoordinate coordinate)
        {
            return new GDIPoint(Convert.ToInt32(coordinate.X), Convert.ToInt32(coordinate.Y));
        }
    }
}
