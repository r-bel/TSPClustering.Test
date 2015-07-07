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
        public Rectangle Viewport { get; private set; }

        public Drawingboard(Graphics graphics, Rectangle viewport)
        {
            this.transf = null;
            this.graphics = graphics;
            this.Viewport = viewport;
        }

        public Drawingboard(TransformationMatrix transf, Rectangle viewport, Graphics graphics)
        {
            this.transf = transf;
            this.graphics = graphics;
            this.Viewport = viewport;
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

        public void DrawCoordinate(Point coordinate)
        {
            DrawText(string.Format("({0};{1})", coordinate.X, coordinate.Y), coordinate);

            DrawUntransformed(() =>
            {
                var tx = transf.Tx(coordinate);

                graphics.FillEllipse(Brushes.Red, tx.X - 3, tx.Y - 3, 6, 6);
            });
        }

        public void DrawText(string text, Point position)
        {
            DrawUntransformed(() =>
            {
                using (var font = new Font(FontFamily.GenericSansSerif, 8))
                {
                    graphics.DrawString(text, font, Brushes.Black, transf == null ? position : transf.Tx(position));
                    var size = graphics.MeasureString(text, font);
                }
            });
        }

        public void DrawAxes()
        {
            if (Viewport != null)
            {
                using (var pen = new Pen(Color.Green, 0.01F))
                {
                    graphics.DrawLine(pen, new Point(Viewport.X, 0), new Point(Viewport.Width + Viewport.X, 0));
                    graphics.DrawLine(pen, new Point(0, Viewport.Y), new Point(0, Viewport.Height + Viewport.Y));
                }

                DrawCoordinate(new Point(0, 0));

                DrawCoordinate(new Point(Viewport.Right - 4, 0));
                DrawCoordinate(new Point(0, Viewport.Bottom));
            }
        }

        public void DrawGrid()
        {
            if (Viewport != null)
                using (var pen = new Pen(Color.LightGray, 0))
                {
                    for (int x = Viewport.Left; x <= Viewport.Right; x++)
                        graphics.DrawLine(pen, new Point(x, Viewport.Bottom), new Point(x, Viewport.Top));
                    for (int y = Viewport.Top; y <= Viewport.Bottom; y++)
                        graphics.DrawLine(pen, new Point(Viewport.Left, y), new Point(Viewport.Right, y));
                }
        }
    }
}
