using Clustering;
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
using Point = Geometry.Point;
using GDIPoint = System.Drawing.Point;
using GDI;
using Geometry;
//using TSPLIBFormat;
using CostFunction;
using Clustering.Algorithms;
//using TSP;
using FileStuff;
using OsmSharp.Math.TSP;
using OsmSharp.Math.TSP.EdgeAssemblyGenetic;
using OsmSharp.Math.TSP.Genetic.Solver.Operations.Generation;
using OsmSharp.Math.TSP.Genetic.Solver.Operations.CrossOver;
using OsmSharp.Math.VRP.MultiSalesman.Problems;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Math.VRP.Core;
using OsmSharp.Math.TSP.Problems;
using OsmSharp.Math.TSP.LK;
//using Benchmark;
//using SpeedTest;
using System.Globalization;
using Iteration;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {
        const string dropboxlocation = @"C:\temp\clustering";

        AxisAlignedRectangle boundingRectangle;
        TransformationMatrix matrix;
        private AlgorithmSet<ICartesianCoordinate> algorithmSet;
        private AlgorithmSet<Cluster<ICartesianCoordinate, double>> algorithmSetClusters;
        private IList<ICartesianCoordinate> lastTSPResultWithClusters;

        public MainForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            comboBox1.DataSource = Enum.GetValues(typeof(AlgorithmSet<ICartesianCoordinate>.TSPAlgorithm));
            comboBox2.DataSource = Enum.GetValues(typeof(AlgorithmSet<ICartesianCoordinate>.ClusterAlgorithm));

            panel1.Paint += Form1_Paint;
        }

        void algorithmSet_ClusterAfterIterationEvent(object sender, EventArgs e)
        {
            if (algorithmSet.LastBenchmark != null)
            {
                var ts = TimeSpan.FromMilliseconds(Convert.ToInt64(algorithmSet.LastBenchmark.RunningTimeInMicroseconds/1000));
                Report(string.Format("Run.time without clustering = {0}", ts.ToString()));
            }

            if (algorithmSet.Clusters != null)
            {
                Report(string.Format("Found clusters = {0} of total {1} points and {2} singleton clusters.", algorithmSet.Clusters.Count, 
                    algorithmSet.Clusters.Sum(c => c.Nodes.Count()),
                    algorithmSet.Clusters.Count(c => c.Nodes.Count() == 1)));
            }
            
            panel1.Refresh();
        }

        private void Report(string message)
        {
            richTextBox1.AppendText(DateTime.Now.ToString() + " " + message + "\n");
            richTextBox1.Select(richTextBox1.Text.Length, 0);
            richTextBox1.ScrollToCaret();
        }

        private List<ICartesianCoordinate> LoadCSV_Coordinates(string filename)
        {
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var csvInterpreter = new CsvInterpreter();

                var coordinates = new List<ICartesianCoordinate>();

                int limit;
                if (!int.TryParse(textBox1.Text, out limit))
                    limit = -1;
                
                var linesInCSV = csvInterpreter.Deserialize(file).Skip(1);
                if (limit > -1)
                    linesInCSV = linesInCSV.Take(limit);
                
                foreach (var line in linesInCSV)
                {
                    float x, y;

                    if (float.TryParse(line[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x)
                        && float.TryParse(line[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                    {
                        x *= 100000;
                        y *= 100000;

                        coordinates.Add(new Point(x, y));
                    }
                }
                coordinates = coordinates.ToList();

                Report(coordinates.Count.ToString() + " coordinates loaded from " + filename);
    
                return coordinates;
            }
        }

        private CostMatrix<double> LoadCSV_Costs(string filename, int dimension)
        {
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var csvInterpreter = new CsvInterpreter();

                var weightmatrix = new List<double[]>();
                foreach (var line in csvInterpreter.Deserialize(file).Take(dimension))
                {
                    var costs = line.Select(col => double.Parse(col, NumberStyles.Float, CultureInfo.InvariantCulture)).Take(dimension);
                    weightmatrix.Add(costs.ToArray());
                }

                Report("costmatrix loaded from " + filename);

                return new CostMatrix<double>(weightmatrix.ToArray());
            }
        }

        #region Click events

        private void btnLoadScene_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            IList<ICartesianCoordinate> coordinates = null;
            CostMatrix<double> costs = null;

            if (radioButton1.Checked)
            {
                coordinates = LoadCSV_Coordinates(Path.Combine(dropboxlocation, @"10001.csv"));
            }
            else if (radioButton2.Checked)
            {
                coordinates = new[] { new Point(3, 0), new Point(3, 5), new Point(5, 2), new Point(1, 1), new Point(4, 6) };
            }

            if (radioButton4.Checked)
            {
                costs = LoadCSV_Costs(Path.Combine(dropboxlocation, @"dump.csv"), coordinates.Count);
            }
            else if (radioButton3.Checked)
            {
                var costWithFunction = new CostWithFunction<ICartesianCoordinate>(coordinates, Diverse.DistanceSquared);
                costs = new CostMatrix<double>(costWithFunction, coordinates.Count);
            }

            boundingRectangle = coordinates.BoundingRectangle(); // for viewport

            algorithmSet = new AlgorithmSet<ICartesianCoordinate>(coordinates, costs);
            algorithmSet.ClusterAfterIterationEvent += algorithmSet_ClusterAfterIterationEvent;
            algorithmSetClusters = null;
            lastTSPResultWithClusters = null;

            panel1.Refresh();
        }

        private void btnRunTSP_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (algorithmSetClusters != null) // We work based on clusters so GTSP
            {
                algorithmSetClusters.RunTSP((AlgorithmSet<Cluster<ICartesianCoordinate, double>>.TSPAlgorithm)comboBox1.SelectedItem);

                lastTSPResultWithClusters = clusteredGraph.ClusterPathToFullPath(algorithmSetClusters.LastRoute);

                //lastTSPResultWithClusters = new TSPResult<ICartesianCoordinate, double>(routeReconstruction);
                
                double totalCost = 0;
                foreach (var edge in lastTSPResultWithClusters.Tuples())
                {
                    totalCost += algorithmSet.costByReference.Cost(edge.Item1, edge.Item2);
                }

                Report(string.Format("TSP (clustering) total cost = {0}", totalCost));

            }
            else if (algorithmSet != null)
            {
                algorithmSet.RunTSP((AlgorithmSet<ICartesianCoordinate>.TSPAlgorithm)comboBox1.SelectedItem);

                double totalCost = 0;
                foreach(var edge in algorithmSet.LastRoute.Pairs())
                {
                    totalCost += algorithmSet.CostMatrix.Cost(edge.From, edge.To);
                }
                Report(string.Format("TSP (no clustering) total cost = {0}", totalCost));
            }
            else
                Report("First load a scene");

            panel1.Refresh();
        }

        private ClusteredGraphToCostMatrix<ICartesianCoordinate> clusteredGraph;

        private void btnRunClustering_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (algorithmSet != null)
            {
                algorithmSet.RunCluster((AlgorithmSet<ICartesianCoordinate>.ClusterAlgorithm)comboBox2.SelectedItem, int.Parse(textBox2.Text), int.Parse(textBox3.Text));

                clusteredGraph = new ClusteredGraphToCostMatrix<ICartesianCoordinate>(algorithmSet.costByReference, algorithmSet.Clusters);
                
                var costMatrixClusters = clusteredGraph.CostMatrix;
                
                algorithmSetClusters = new AlgorithmSet<Cluster<ICartesianCoordinate, double>>(algorithmSet.Clusters, costMatrixClusters);
                algorithmSetClusters.ClusterAfterIterationEvent += algorithmSetClusters_ClusterAfterIterationEvent;
            }
            else
                Report("First load a scene");

            panel1.Refresh();
        }

        void algorithmSetClusters_ClusterAfterIterationEvent(object sender, EventArgs e)
        {
            if (algorithmSetClusters.LastBenchmark != null)
            {
                var ts = TimeSpan.FromMilliseconds(Convert.ToInt64(algorithmSetClusters.LastBenchmark.RunningTimeInMicroseconds / 1000));
                Report(string.Format("Run.time with clustering = {0}", ts.ToString()));
            }
        }

        #endregion

        #region Paint stuff

        void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            if (algorithmSet == null || algorithmSet.Nodes == null)
                return;

            matrix = new TransformationMatrix(sender as Control, boundingRectangle.ToGDI());
            var board = new GDI.Drawingboard(matrix, e.Graphics);

            if (checkBox1.Checked)
            {
                var counter = 0;
                foreach (var c in algorithmSet.Nodes)
                {
                    board.DrawCoordinate(c.ToGDI(), algorithmSet.Nodes.Count < 100, (++counter).ToString());
                }
            }

            if (algorithmSet.LastRoute != null && checkBox3.Checked)
            {
                using (var pen = new Pen(Color.Blue, 0))
                    e.Graphics.DrawLines(pen, algorithmSet.LastRoute.Concat(new[] { algorithmSet.LastRoute.First() }).Select(h => algorithmSet.Nodes[h].ToGDI()).ToArray());
            }
            //else if (algorithmSet.LastTSPResult != null && checkBox3.Checked)
            //{
            //    using (var pen = new Pen(Color.Blue, 0))
            //    {
            //        e.Graphics.DrawLines(pen, algorithmSet.LastTSPResult.TourOrPath.Concat(new[] { algorithmSet.LastTSPResult.TourOrPath.First() }).Select(h => h.ToGDI()).ToArray());
            //    }
            //}

            if (lastTSPResultWithClusters != null && checkBox4.Checked)
            {
                using (var pen = new Pen(Color.Red, 0))
                {
                    e.Graphics.DrawLines(pen, lastTSPResultWithClusters.Concat(new[] { lastTSPResultWithClusters.First() }).Select(h => h.ToGDI()).ToArray());
                }
            }

          
            if (algorithmSet.Clusters != null && checkBox2.Checked)
            {
                using (var pen = new Pen(Color.Green, 0))
                    foreach (var cluster in algorithmSet.Clusters)
                    {
                        if (cluster.Nodes.Count() > 1)
                        {
                            //var hul = new ConvexHull().Algorithm(cluster);
                            //e.Graphics.DrawPolygon(pen, hul.Select(h => h.ToGDI()).ToArray());
                            e.Graphics.DrawLines(pen, cluster.Nodes.Select(h => h.ToGDI()).ToArray());
                        }
                    }
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Refresh();
        }

        #endregion
    }
}
