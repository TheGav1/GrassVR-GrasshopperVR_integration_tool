using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
// <Custom usign>
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;

using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;
using KPlankton;
using KangarooSolver;
using KangarooSolver.Goals;
// </Custom usign>


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { __out.Add(text); }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private IGH_Component Component; 
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments, 
  /// Output parameters as ref arguments. You don't have to assign output parameters, 
  /// they will have a default value.
  /// </summary>
  

  private void RunScript(List<object> Goals, bool Reset, bool Step, ref object A, ref object B, ref object C, ref object D, ref object E, ref object F)
  {
      // <Custom code>
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
      if (Name[0] != "KangarooSolver")
            { Name = FullName.Split('_', '+'); }
      var G = GoalList[i] as IGoal;

      for(int j = 0; j < G.PIndex.Count();j++)
      {
        Names[G.PIndex[j]].Add(Name[2]);
      }
    }


    F = Names;
        // </Custom code>       
    }


    // <Custom additional code> 

    KangarooSolver.PhysicalSystem PS = new KangarooSolver.PhysicalSystem();
  List<IGoal> GoalList = new List<IGoal>();
  int counter = 0;
  // </Custom additional code> 

  private List<string> __err = new List<string>(); //Do not modify this list directly.
  private List<string> __out = new List<string>(); //Do not modify this list directly.
  private RhinoDoc doc = RhinoDoc.ActiveDoc;       //Legacy field.
  private IGH_ActiveObject owner;                  //Legacy field.
  private int runCount;                            //Legacy field.
  
  public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
  {
    //Prepare for a new run...
    //1. Reset lists
    this.__out.Clear();
    this.__err.Clear();

    this.Component = owner;
    this.Iteration = iteration;
    this.GrasshopperDocument = owner.OnPingDocument();
    this.RhinoDocument = rhinoDocument as Rhino.RhinoDoc;

    this.owner = this.Component;
    this.runCount = this.Iteration;
    this. doc = this.RhinoDocument;

    //2. Assign input parameters
        List<object> Goals = null;
    if (inputs[0] != null)
    {
      Goals = GH_DirtyCaster.CastToList<object>(inputs[0]);
    }
    bool Reset = default(bool);
    if (inputs[1] != null)
    {
      Reset = (bool)(inputs[1]);
    }

    bool Step = default(bool);
    if (inputs[2] != null)
    {
      Step = (bool)(inputs[2]);
    }



    //3. Declare output parameters
      object A = null;
  object B = null;
  object C = null;
  object D = null;
  object E = null;
  object F = null;


    //4. Invoke RunScript
    RunScript(Goals, Reset, Step, ref A, ref B, ref C, ref D, ref E, ref F);
      
    try
    {
      //5. Assign output parameters to component...
            if (A != null)
      {
        if (GH_Format.TreatAsCollection(A))
        {
          IEnumerable __enum_A = (IEnumerable)(A);
          DA.SetDataList(1, __enum_A);
        }
        else
        {
          if (A is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(A));
          }
          else
          {
            //assign direct
            DA.SetData(1, A);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }
      if (B != null)
      {
        if (GH_Format.TreatAsCollection(B))
        {
          IEnumerable __enum_B = (IEnumerable)(B);
          DA.SetDataList(2, __enum_B);
        }
        else
        {
          if (B is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(2, (Grasshopper.Kernel.Data.IGH_DataTree)(B));
          }
          else
          {
            //assign direct
            DA.SetData(2, B);
          }
        }
      }
      else
      {
        DA.SetData(2, null);
      }
      if (C != null)
      {
        if (GH_Format.TreatAsCollection(C))
        {
          IEnumerable __enum_C = (IEnumerable)(C);
          DA.SetDataList(3, __enum_C);
        }
        else
        {
          if (C is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(3, (Grasshopper.Kernel.Data.IGH_DataTree)(C));
          }
          else
          {
            //assign direct
            DA.SetData(3, C);
          }
        }
      }
      else
      {
        DA.SetData(3, null);
      }
      if (D != null)
      {
        if (GH_Format.TreatAsCollection(D))
        {
          IEnumerable __enum_D = (IEnumerable)(D);
          DA.SetDataList(4, __enum_D);
        }
        else
        {
          if (D is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(4, (Grasshopper.Kernel.Data.IGH_DataTree)(D));
          }
          else
          {
            //assign direct
            DA.SetData(4, D);
          }
        }
      }
      else
      {
        DA.SetData(4, null);
      }
      if (E != null)
      {
        if (GH_Format.TreatAsCollection(E))
        {
          IEnumerable __enum_E = (IEnumerable)(E);
          DA.SetDataList(5, __enum_E);
        }
        else
        {
          if (E is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(5, (Grasshopper.Kernel.Data.IGH_DataTree)(E));
          }
          else
          {
            //assign direct
            DA.SetData(5, E);
          }
        }
      }
      else
      {
        DA.SetData(5, null);
      }
      if (F != null)
      {
        if (GH_Format.TreatAsCollection(F))
        {
          IEnumerable __enum_F = (IEnumerable)(F);
          DA.SetDataList(6, __enum_F);
        }
        else
        {
          if (F is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(6, (Grasshopper.Kernel.Data.IGH_DataTree)(F));
          }
          else
          {
            //assign direct
            DA.SetData(6, F);
          }
        }
      }
      else
      {
        DA.SetData(6, null);
      }

    }
    catch (Exception ex)
    {
      this.__err.Add(string.Format("Script exception: {0}", ex.Message));
    }
    finally
    {
      //Add errors and messages... 
      if (owner.Params.Output.Count > 0)
      {
        if (owner.Params.Output[0] is Grasshopper.Kernel.Parameters.Param_String)
        {
          List<string> __errors_plus_messages = new List<string>();
          if (this.__err != null) { __errors_plus_messages.AddRange(this.__err); }
          if (this.__out != null) { __errors_plus_messages.AddRange(this.__out); }
          if (__errors_plus_messages.Count > 0) 
            DA.SetDataList(0, __errors_plus_messages);
        }
      }
    }
  }
}