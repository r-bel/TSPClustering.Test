using BasicDatastructures;
using HierarchicalClustering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = HierarchicalClustering.Point;
using GDIPoint = System.Drawing.Point;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        TSPLIBInterpreter problemfileXml;
        AxisAlignedRectangle rect;
        IList<IList<ICartesianCoordinate>> clusters;

        public Form1()
        {
            InitializeComponent();

//            const string dropboxlocation = @"C:\Users\Dirk";
            const string dropboxlocation = @"D:\";
            
            using (var file = new FileStream(Path.Combine(dropboxlocation, @"Dropbox\SharedDevelopment\TSP Problem instances\dantzig42.xml"), FileMode.Open))
            {
                problemfileXml = new TSPLIBInterpreter(file);
            }
            using (var file = new FileStream(Path.Combine(dropboxlocation, @"Dropbox\SharedDevelopment\TSPLIB-master\OsmSharp.TSPLIB.Benchmark\Problems\TSP\dantzig42.tsp"), FileMode.Open))
            {
                problemfileXml.ReadFieldsFromText(file);
            }

            rect = problemfileXml.DisplayCoordinates.BoundingRectangle();

            var costmatrix = new CostMatrix<ICartesianCoordinate>(problemfileXml.Instance.graph, problemfileXml.DisplayCoordinates);

            clusters = DBScan<ICartesianCoordinate>.Algorithm(problemfileXml.DisplayCoordinates, (double)0.5, 2, costmatrix);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            var board = new GDI.Drawingboard(e.Graphics);

            board.DrawGrid();
            board.DrawAxes();

            e.Graphics.DrawLine(new Pen(Color.Red, 0.01F), new GDIPoint(0, 0), new GDIPoint(9, 9));

            //world.Draw(graphics);



            //e.Graphics.ScaleTransform(4, 4);

            //e.Graphics.DrawRectangle(Pens.Black, rect.ToRectangle());

            //foreach (var c in problemfileXml.DisplayCoordinates)
            //    e.Graphics.FillEllipse(Brushes.Red, (float)c.X - 1, (float)c.Y - 1, (float)5, (float)3);                
            
            //foreach (var cluster in clusters)
            //{
            //    ICartesianCoordinate prev = null;

            //    foreach(var p in cluster)
            //    {
            //        if (prev != null)
            //            e.Graphics.DrawLine(Pens.Green, (float)prev.X, (float)prev.Y, (float)p.X, (float)p.Y);
            //        prev = p;
            //    }
                    
            //}
        }
    }
}
