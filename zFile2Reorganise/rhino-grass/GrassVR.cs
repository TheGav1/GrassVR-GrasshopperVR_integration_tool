using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Linq;



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
  private void RunScript(List<Point3d> PTList, string INmsg, bool Run, bool Reset, ref object Count, ref object NewList)
  {
        List<string> Msg;
    Count = c / 2;
    Point3d PT = new Point3d(1, 1, 1);

    if(Run)
    {
      Msg = RecivedData(INmsg);
      if(Reset)
      {
        Static.Clear();
        c = -1;
      }

      List1 = LoadList();





      switch (Msg[0])
      {
        case "Add":
          Add(Msg[1]);
          break;
        case "Del":
          Del(Msg[1]);
          break;
        case "Res0":
          Static.Clear();
          c = -1;
          break;
        default:
          Console.WriteLine("null message");
          break;
      }
    }

    NewList = List1;
    Static = List1;
  }

  // <Custom additional code> 
  public int c;
public static List<Point3d> Static;
public bool hasStarted;
public List<Point3d> List1 = new List<Point3d>();
string[] ToRemove = new string[] {"{", "}"};

List<string> RecivedData(string text)
{
  if(text != null)
    return text.Split('\n').ToList();
  else
    return null;

}
List<Point3d> LoadList()
{

  if (!hasStarted && Static.Count <= 0)
  {
    return PTList;
  }
  else
  {
    return Static;
  }
}
void Add(string Data)
{
  Point3d Pt = Strg2Pt(Data);
  List1.Add(Pt);
}
void Del(string Data)
{
  int index = Int32.Parse(Data);
  List1.RemoveAt(index);
}
Point3d Strg2Pt(string String)
{
  foreach (var c in ToRemove)
  {
    String = String.Replace(c, string.Empty);
  }
  float[] Position = new float[3];
  return new Point(Position[1], Position[2], Position[3]);
}


  /// <summary>
  /// This method will be called once every solution, before any calls to RunScript.
  /// </summary>
public override void BeforeRunScript()
{if (c <= 0)
    hasStarted = false;
  else
    hasStarted = true;
}
  /// <summary>
  /// This method will be called once every solution, after any calls to RunScript.
  /// </summary>
public override void AfterRunScript()
{c++;}

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
        List<Point3d> PTList = null;
    if (inputs[0] != null)
    {
      PTList = GH_DirtyCaster.CastToList<Point3d>(inputs[0]);
    }
    string INmsg = default(string);
    if (inputs[1] != null)
    {
      INmsg = (string)(inputs[1]);
    }

    bool Run = default(bool);
    if (inputs[2] != null)
    {
      Run = (bool)(inputs[2]);
    }

    bool Reset = default(bool);
    if (inputs[3] != null)
    {
      Reset = (bool)(inputs[3]);
    }



    //3. Declare output parameters
      object Count = null;
  object NewList = null;


    //4. Invoke RunScript
    RunScript(PTList, INmsg, Run, Reset, ref Count, ref NewList);
      
    try
    {
      //5. Assign output parameters to component...
            if (Count != null)
      {
        if (GH_Format.TreatAsCollection(Count))
        {
          IEnumerable __enum_Count = (IEnumerable)(Count);
          DA.SetDataList(1, __enum_Count);
        }
        else
        {
          if (Count is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(Count));
          }
          else
          {
            //assign direct
            DA.SetData(1, Count);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }
      if (NewList != null)
      {
        if (GH_Format.TreatAsCollection(NewList))
        {
          IEnumerable __enum_NewList = (IEnumerable)(NewList);
          DA.SetDataList(2, __enum_NewList);
        }
        else
        {
          if (NewList is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(2, (Grasshopper.Kernel.Data.IGH_DataTree)(NewList));
          }
          else
          {
            //assign direct
            DA.SetData(2, NewList);
          }
        }
      }
      else
      {
        DA.SetData(2, null);
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