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
    public class Drawingboard
    {
        private Graphics graphics;
        private TransformationMatrix transf;

        public Drawingboard(Graphics graphics)
        {
            this.transf = null;
            this.graphics = graphics;
        }

        public Drawingboard(TransformationMatrix transf, Graphics graphics)
        {
            this.transf = transf;
            this.graphics = graphics;
            graphics.Transform = transf.Transformation;
        }

        public void DrawUntransformed(Action drawaction)
        {
            var previousTransformation = graphics.Transform;

            graphics.ResetTransform();

            try
            {
                drawaction();
            }
            finally
            {
                graphics.Transform = previousTransformation;
            }
        }

        public void DrawCoordinate(Point coordinate, bool withLabels = true, string prefix = "")
        {
            if (withLabels)
            {
                var text = string.Format("({0};{1})", coordinate.X, coordinate.Y);
                if (prefix != null)
                    text = prefix + text;
                DrawText(text, coordinate);
            }

            DrawUntransformed(() =>
            {
                var tx = transf.Tx(coordinate);

                graphics.FillEllipse(Brushes.Red, tx.X - 2, tx.Y - 2, 4, 4);
            });
        }

        public void DrawText(string text, Point position)
        {
            DrawUntransformed(() =>
            {
                using (var font = new Font(FontFamily.GenericSansSerif, 8))
                {
                    graphics.DrawString(text, font, Brushes.Black, transf.Tx(position));
                    var size = graphics.MeasureString(text, font);
                }
            });
        }

        public void DrawAxes()
        {
            using (var pen = new Pen(Color.Green, 0.01F))
            {
                graphics.DrawLine(pen, new Point(transf.Viewport.X, 0), new Point(transf.Viewport.Width + transf.Viewport.X, 0));
                graphics.DrawLine(pen, new Point(0, transf.Viewport.Y), new Point(0, transf.Viewport.Height + transf.Viewport.Y));
            }

            DrawCoordinate(new Point(0, 0));

            DrawCoordinate(new Point(transf.Viewport.Right-4, 0));
            DrawCoordinate(new Point(0, transf.Viewport.Bottom));
        }

        public void DrawGrid()
        {
            using (var pen = new Pen(Color.LightGray, 0))
            {
                for (int x = transf.Viewport.Left; x <= transf.Viewport.Right; x++)
                    graphics.DrawLine(pen, new Point(x, transf.Viewport.Bottom), new Point(x, transf.Viewport.Top));
                for (int y = transf.Viewport.Top; y <= transf.Viewport.Bottom; y++)
                    graphics.DrawLine(pen, new Point(transf.Viewport.Left, y), new Point(transf.Viewport.Right, y));
            }
        }
    }
}
