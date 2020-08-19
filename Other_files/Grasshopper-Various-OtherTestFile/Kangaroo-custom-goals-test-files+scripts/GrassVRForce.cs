using System.Collections.Generic;
using Rhino.Geometry;

namespace KangarooSolver.Goals
{
    public class GaviVRForce : GoalObject
    {
        public Vector3d Force;        

        public GaviVRForce()
        {
        }

        public GaviVRForce(int u, Vector3d v)
        {
            PIndex = new int[1] { -1 };
            Move = new Vector3d[1];
            Weighting = new double[1];
            Force = v;
        }

        public GaviVRForce(Point3d P, Vector3d v)
        {
            PPos = new Point3d[1] { P };
            Move = new Vector3d[1];
            Weighting = new double[1];
            Force = v;
        }

        public override void Calculate(List<Particle> p)
        {      
            PI
            Move[0] = Force;                  
            Weighting[0] = 1.0;
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
            PIndex = Array.IndexOf(dist, dist.Min());

        }
    }    
}
