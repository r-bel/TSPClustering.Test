using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDI
{
    public interface IDrawable
    {
        void Draw(Graphics graphics, Point position);
    }
}
