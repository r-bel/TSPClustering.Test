using Clustering.Algorithms;
using Geometry;
using SpeedTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TSP;
using CostFunction;
using Clustering;
using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSP.GA.EAX;
using OsmSharp.Logistics.Solvers.GA;
using OsmSharp.Logistics.Solutions.TSP;
using OsmSharp.Logistics.Solvers;

namespace WindowsFormsApplication1
{
    internal class AlgorithmSet<TNode> where TNode : class
    {
        public event EventHandler ClusterAfterIterationEvent;

        public enum TSPAlgorithm
        {
            //LKH,
            //NN,
            //OLC,
            //RAI,
            EAX,
//            EAXWITHCLUSTERING,
        };
        
        public enum ClusterAlgorithm
        {
            //DBSCAN,
            //KMEDOID,
            //KMEAN,
            //DIRK,
            //DIRK2,
            DIRK3,
        };

        //public TSPResult<TNode, double> LastTSPResult { get; private set; }
        public IRoute LastRoute { get; private set; }
        public IList<Cluster<TNode, double>> Clusters { get; private set; }
        public RunningTime LastBenchmark { get; private set; }

        public IList<TNode> Nodes { get; set; }

        private CostMatrix<double> costMatrix;
        public CostMatrix<double> CostMatrix
        {
            get
            {
                return costMatrix;
            }
            set
            {
                costMatrix = value;
                this.costByReference = new CostMatrixByRef<TNode, double>(costMatrix, Nodes);
                this.costAnalyzer = new CostAnalyzer<double>(costMatrix);
            }
        }
        public ICostByReference<TNode, double> costByReference;
        public CostAnalyzer<double> costAnalyzer;

        //private DBScan<TNode>  dbscan = new DBScan<TNode>();
        //private KMeans<TNode> kmean = new KMeans<TNode>(Diverse.Mean);
        //private KMedoid<TNode> kmedoid = new KMedoid<TNode>();
        //private Testje<TNode> testje = new Testje<TNode>();
        //private OneDirectioning<TNode> oneDirectioning = new OneDirectioning<TNode>();
        private LargeATSPPreClustering<TNode> dirk3 = new LargeATSPPreClustering<TNode>();
        
        //private LinKernighan<TNode> lkh = new LinKernighan<TNode>();
        private ISolver<ITSP, IRoute> eax = new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            });
        //private NearestNeighbor<TNode> nearestNeighbor = new NearestNeighbor<TNode>();
        //private OneLoopCheapestInsertion<TNode> oneLoopCheapestInsertion = new OneLoopCheapestInsertion<TNode>();
        //private RandomArbitraryInsertion<TNode> randomArbitraryInsertion = new RandomArbitraryInsertion<TNode>();

        public AlgorithmSet()
        {
            //dbscan.AfterIterationEvent += cluster_AfterIterationEvent;
            //kmean.AfterIterationEvent += cluster_AfterIterationEvent;
            //kmedoid.AfterIterationEvent += cluster_AfterIterationEvent;
            //testje.AfterIterationEvent += cluster_AfterIterationEvent;
            //randomArbitraryInsertion.AfterIterationEvent += cluster_AfterIterationEvent;
        }

        public AlgorithmSet(IList<TNode> nodes, CostMatrix<double> costMatrix) : this()
        {
            this.Nodes = nodes;
            CostMatrix = costMatrix;
        }

        public void RunTSP(TSPAlgorithm algorithm)
        {
            //LastTSPResult = null;
            LastRoute = null;
            IList<TNode> result = null;

            LastBenchmark = RunningTime.TestNow(() =>
            {
                switch (algorithm)
                {
                    //case TSPAlgorithm.LKH:
                    //    result = lkh.Solve(Nodes, costAnalyzer);
                    //    break;
                    case TSPAlgorithm.EAX:
                        var problem = new TSPProblem(0, costMatrix.Matrix);
                        LastRoute = eax.Solve(problem);
                        break;
                    //case TSPAlgorithm.NN:
                    //    result = nearestNeighbor.Solve(Nodes, costAnalyzer);
                    //    break;
                    //case TSPAlgorithm.OLC:
                    //    result = oneLoopCheapestInsertion.Solve(Nodes, costByReference, costAnalyzer);
                    //    break;
                    //case TSPAlgorithm.RAI:
                    //    result = randomArbitraryInsertion.Solve(Nodes, costByReference, costAnalyzer);
                    //    break;
                }
            });

            //if (result != null)
            //    LastTSPResult = new TSPResult<TNode, double>(result);

            cluster_AfterIterationEvent(this, EventArgs.Empty);
        }

        public void RunCluster(ClusterAlgorithm algorithm, int parameter1, int parameter2)
        {
            switch(algorithm)
            {
                //case ClusterAlgorithm.DBSCAN:
                //    const double eps = 20;
                //    Clusters = dbscan.Solve(Nodes, eps, 2, costByReference);
                //    break;
                //case ClusterAlgorithm.KMEAN:
                //    Clusters = kmean.Solve(Nodes, 10, Diverse.DistanceSquared);
                //    break;
                //case ClusterAlgorithm.KMEDOID:
                //    Clusters = kmedoid.Solve(Nodes, 10, costByReference, costAnalyzer);
                //    break;
                //case ClusterAlgorithm.DIRK:
                //    Clusters = testje.Algorithm(Nodes, costByReference);
                //    break;
                //case ClusterAlgorithm.DIRK2:
                //    Clusters = oneDirectioning.Solve(Nodes, costAnalyzer, new ObjectToIndexMapper<TNode>(Nodes), parametervalue);
                //    break;
                case ClusterAlgorithm.DIRK3:
                    Clusters = dirk3.Solve(Nodes, costAnalyzer, parameter1, parameter2);
                    break;
            }

            cluster_AfterIterationEvent(this, EventArgs.Empty);
        }

        void cluster_AfterIterationEvent(object sender, EventArgs e)
        {
            ClusterAfterIterationEvent(sender, e);
        }        
    }
}
