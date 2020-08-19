

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// Unique namespace, so visual studio won't throw any errors about duplicate definitions.
namespace ns2a478
{
    /// <summary>
    /// This class will be instantiated on demand by the Script component.
    /// </summary>
    public class Script_Instance : GH_ScriptInstance
    {
	    /// This method is added to prevent compiler errors when opening this file in visual studio (code) or rider.
	    public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
        {
            throw new NotImplementedException();
        }

        #region Utility functions
        /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
        /// <param name="text">String to print.</param>
        private void Print(string text) { /* Implementation hidden. */ }
        /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
        /// <param name="format">String format.</param>
        /// <param name="args">Formatting parameters.</param>
        private void Print(string format, params object[] args) { /* Implementation hidden. */ }
        /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
        /// <param name="obj">Object instance to parse.</param>
        private void Reflect(object obj) { /* Implementation hidden. */ }
        /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
        /// <param name="obj">Object instance to parse.</param>
        private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
        #endregion
        #region Members
        /// <summary>Gets the current Rhino document.</summary>
        private readonly RhinoDoc RhinoDocument;
        /// <summary>Gets the Grasshopper document that owns this script.</summary>
        private readonly GH_Document GrasshopperDocument;
        /// <summary>Gets the Grasshopper script component that owns this script.</summary>
        private readonly IGH_Component Component;
        /// <summary>
        /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
        /// Any subsequent call within the same solution will increment the Iteration count.
        /// </summary>
        private readonly int Iteration;
        #endregion
        /// <summary>
        /// This procedure contains the user code. Input parameters are provided as regular arguments,
        /// Output parameters as ref arguments. You don't have to assign output parameters,
        /// they will have a default value.
        /// </summary>
        #region Runscript
        private void RunScript(List<object> Goals, bool Reset, bool Step, ref object A, ref object B, ref object C, ref object D, ref object E, ref object F)
        {

            if(Reset)
            {
                PS = new KangarooSolver.PhysicalSystem();
                counter = 0;
                GoalList.Clear();
                foreach(IGoal G in Goals) //Assign indexes to the particles in each Goal:
                {
                    PS.AssignPIndex(G, 0.0001); // the second argument is the tolerance distance below which points combine into a single particle
                    GoalList.Add(G);
                }
            }
            if(Step)
            {
                PS.SimpleStep(GoalList);
                counter++;
            }


            A = PS.GetOutput(GoalList);
            B = counter;
            C = PS.GetAllMoves(GoalList);
            D = PS.GetAllWeightings(GoalList);
            E = PS.GetPositionArray();

            var Names = new List<String>[PS.ParticleCount()];

            for(int i = 0; i < PS.ParticleCount();i++)
            {
                Names[i] = new List<String>();
            }

            for(int i = 0;i < GoalList.Count;i++)
            {
                var FullName = GoalList[i].ToString();
                Char splitter = '.';
                var Name = FullName.Split(splitter);
                if(Name[0] != "KangarooSolver")
                {Name = FullName.Split('_','+');}
                var G = GoalList[i] as IGoal;

                for(int j = 0; j < G.PIndex.Count();j++)
                {
                    Names[G.PIndex[j]].Add(Name[2]);
                }
            }


            F = Names;
        }
        #endregion

        #region Additional

        KangarooSolver.PhysicalSystem PS = new KangarooSolver.PhysicalSystem();
        List<IGoal> GoalList = new List<IGoal>();
        int counter = 0;
        #endregion
    }
}
