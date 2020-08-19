using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    /// <summary>
    /// Fix a point in place
    /// </summary>   
    public class GrassVRAnchor : GoalObject
    {
        public double Strength;
        public Point3d Pt;
        public Point3d InsPt;
        public int[] PIndex;
        public double Range;

        public GrassVRAnchor()
        {
        }

        /// <summary>
        /// Construct a new Anchor object by particle index and target position
        /// </summary>
        /// <param name="Id">The integer index of the particle to anchor.</param>
        /// <param name="P">The target position to keep the particle at.</param>
        /// <param name="K">Strength of the Anchor. For an absolute anchor, you can use double.MaxValue here.</param>
        public GrassVRAnchor(Point3d BaseP,  Point3d P, double R, double k)
        {
            PIndex = new int[1] { -1 };
            Move = new Vector3d[1];
            Weighting = new double[1] { k };
            Strength = k;
            Range = R;
            Pt = P;
            InsPt = BaseP;
        }

        /// <summary>
        /// Construct a new Anchor object by position.
        /// </summary>        
        /// <param name="P">Particle starting position. Also used as the target position to keep the particle at.</param>
        /// <param name="K">Strength of the Anchor. For an absolute anchor, you can use double.MaxValue here.</param>
        public GrassVRAnchor(Point3d P, double R, double k)
        {
            PPos = new Point3d[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1]{k};
            Strength = k;
            Range = R;
            Pt = P;
        }

        public override void Calculate(List<KangarooSolver.Particle> p)
        {
            //search index only on the first iteration with PIndex==-1
            if (PIndex[0] == -1)
            { PIndex[0] = SearchIndex(p); }
            if (PIndex[0] == -1)
            {

            }
            //problem if change pt without restart iteration => change force base point need to call the restart of the iteration for kangaroo (chek if automatic or not)
            
            Move[0] = Pt - p[PIndex[0]].Position;
            Weighting[0] = Strength;
        }
        public int SearchIndex(List<KangarooSolver.Particle> p)
        {
            int L = p.Length;
            floa[] dist = new float[L];

            for (int i = 0; i < L - 1; i++)
            {
                dist[i] = abs(InsPt.Position - p[i].Position);
            }
            //base Point as minimum distance from insertion Pt
            if (Array.IndexOf(dist, dist.Min()) <= Range)
            { PIndex[0] = Array.IndexOf(dist, dist.Min()); }
            else
            {PIndex[0]=-1}
            
        }
    }
}
