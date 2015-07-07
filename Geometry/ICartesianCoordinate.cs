using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public interface ICartesianCoordinate : IEquatable<ICartesianCoordinate>
    {
        double X { get; }
        double Y { get; }
    }
}
