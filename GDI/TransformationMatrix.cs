using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDI
{
    public class TransformationMatrix
    {
        public Matrix Transformation { get; private set; }
        public Rectangle Viewport { get; private set; }

        public TransformationMatrix(Control control, Rectangle viewport)
        {
            Transformation = new Matrix();

            Viewport = viewport;

            Recalculate(control);
            
            control.ClientSizeChanged += control_ClientSizeChanged;
        }

        public TransformationMatrix (Control control)
        {
            Transformation = new Matrix();

            Viewport = new Rectangle (0,0,control.ClientSize.Width-1, control.ClientSize.Height-1);

            Recalculate(control);
            
            control.ClientSizeChanged += control_ClientSizeChanged;
        }

        public Point Tx(Point point)
        {
            var pp = new[] { point };

            Transformation.TransformPoints(pp);

            return pp[0];
        }

        private void Recalculate(Control control)
        {
            Transformation.Reset();

            Transformation.Scale((float)(control.ClientSize.Width - 1) / (Viewport.Width), -(float)(control.ClientSize.Height - 1) / Viewport.Height);
            Transformation.Translate(-Viewport.X, -(Viewport.Height + Viewport.Y), MatrixOrder.Prepend);
        }

        private void control_ClientSizeChanged(object sender, EventArgs e)
        {
            Recalculate(sender as Control);
        }
    }
}
