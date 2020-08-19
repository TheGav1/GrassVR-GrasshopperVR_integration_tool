using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

// <Custom usign>
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Linq;
//in reconstruction from last working version befor test for Rhino.Inside
// 1 reciver for all the control element types
// internal UDP reciver in parallel thread
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
    private void RunScript(List<Point3d> ModelPtList, List<Point3d> AnchorP, List<Point3d> AnchorT, List<float> AnchorS, object SupportFrame, List<Boolean> SupportX, List<Boolean> SupportY, List<Boolean> SupportZ, List<Boolean> SupportXX, List<Boolean> SupportYY, List<Boolean> SupportZZ, List<float> SupportS, List<Point3d> LoadP, List<Vector3> LoasV, List<float> LoadW, float SnapR, int RecPort, ref object AncP, ref object AncT, ref object AncS, ref object Frame, ref object X, ref object Y, ref object Z, ref object XX, ref object YY, ref object ZZ, ref object SupS, ref object LP, ref object LV, ref object LW)
    {
        //start Anchor list
        AnchorList(AnchorP, AnchorT, AnchorS);
        ForceList(LoadP, LoadV, LoadW);
        SupportList(AnchorP, AnchorT, AnchorS);
        ForceFieldList(LoadP, LoadV, LoadW);
        //list to results
        

    }

    // <Custom additional code> 
    //class definition class data structure
    #region Class+create public lists
    public class Anchor
    {
        public Point3d p;
        public Point3d t;
        public float s;
        //constructor
        public Anchor(Point3d pt, Point3d ptT, float S)
        {
            p = pt;
            t = ptT;
            s = S;
        }
    }
    public class Support
    {
        public Point3d p { get; set; }
        public Boolean x { get; set; }
        public Boolean y { get; set; }
        public Boolean z { get; set; }
        public Boolean xx { get; set; }
        public Boolean yy { get; set; }
        public Boolean zz { get; set; }
        public float s { get; set; }
        //constructor
        public Support(Point3d pt, Boolean X, Boolean Y, Boolean Z, Boolean XX, Boolean YY, Boolean ZZ, float S)
        {
            p = pt;
            x = X;
            y = Y;
            z = Z;
            xx = XX;
            yy = YY;
            zz = ZZ;
            s = S;
        }
    }
    public class Force
    {
        public Point3d p;
        public Vector3D v;
        public float s;
        //constructor
        public Force(Point3d pt, Vector3d V, float S)
        {
            p = pt;
            v = V;
            s = S;
        }
    }
    public class ForceField
    {
        public Vector3d v;
        public float s;
        //constructor
        public ForceField(Vector3d V, float S)
        {
            v = V;
            s = S;
        }
    }

    public List<Anchor> Anchors;
    public List<Support> Supports;
    public List<Force> Forces;
    public List<ForceField> ForceFields;
    #endregion
    #region olddata and functions
    //old version data
    //public int c;
    //public static List<Point3d> Static;
    //public bool hasStarted;
    //public List<Point3d> List1 = new List<Point3d>();

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
    #endregion

    string[] ToRemove = new string[] {"{", "}"};

List<string> RecivedData(string text)
{
  if(text != null)
    return text.Split('\n').ToList();
  else
    return null;

}

    void Add(string[] Data)
{
        //[0]=type [1]=Add [3]=position
        Pioint3d pt = Strg2Pt(Data[3]);
        switch (Data[0])
        {
            case "Force":
                NewForce(pt);
                break;
            case "ForceField":
                NewForceField(pt);
                break;
            case "Anchor":
                NewAnchor(pt);
                break;
            case "Support":
                NewSupport(pt);
                break;
            default:
                Console.WriteLine("null message");
                break;
        }

    }
    public void Del(string[] Data)
    {
        //[0]=type [1]=Del [3]=index
        switch (Data[0])
        {
            case "Force":
                DelForce(Data[3]);
                break;
            case "ForceField":
                DelForceField(Data[3]);
                break;
            case "Anchor":
                DelAnchor(Data[3]);
                break;
            case "Support":
                DelSupport(Data[3]);
                break;
            default:
                Console.WriteLine("null message");
                break;
        }
    }
    public void Update(string[] Data)
    {
        switch (Data[0])
        {
            case "Force":
                UpdateForce(Data[3]);
                break;
            case "ForceField":
                UpdateForceField(Data[3]);
                break;
            case "Anchor":
                DUpdateAnchor(Data[3]);
                break;
            case "Support":
                UpdateSupport(Data[3]);
                break;
            default:
                Console.WriteLine("null message");
                break;
        }
    }
    #region TypeComandRegion
    public void NewForce(Point3d Data)
    {
        Force f = (Data, new Vector3D(1, 0, 0), 1f);

        Forces.Add(f);
    }
    public void NewForceField()
    {
        ForceField f = new ForceField();
        f.v = new Vector3D(1, 0, 0);
        f.s = 1f;
        Forces.Add(f);
    }
    public void NewAnchor(Point3d Data)
    { }
    public void NewSupport(Point3d Data)
    {

    }
    public void UpdateForce(string[] Data)
    { }
    public void UpdateForceField(string[] Data)
    { }
    public void UpdateAnchor(string[] Data)
    { }
    public void UpdateSupport(string[] Data)
    {

    }
    public void DelForce(string index)
    {
        int i = int.Parse(index);
        Forces.RemoveAt(i);
    }
    public void DelForceField(string index)
    {
        int i = int.Parse(index);
        Forces.RemoveAt(i);
    }
    public void DelAnchor(string index)
    {
        int i = int.Parse(index);
        Forces.RemoveAt(i);
    }
    public void DelSupport(string index)
    {
        int i = int.Parse(index);
        Forces.RemoveAt(i);
    }
#endregion


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
    {
        //Old check for external listener
        //{if (c <= 0)
        //    hasStarted = false;
        //  else
        //    hasStarted = true;
        //}
    }
  /// <summary>
  /// This method will be called once every solution, after any calls to RunScript.
  /// </summary>
public override void AfterRunScript()
{//c++;//no count any more
    }

   public void Data2Comand(string Msg)
    {
        string[] splitdata = RecivedData(data);
        //Initial connection
        List<string> Msg;
        //Count = c / 2;
        Point3d PT = new Point3d(1, 1, 1);
        string dataType;

        switch (Msg[1])
            {
                case "Add":
                    Add(Msg);
                    break;
                case "Uppdate":
                    Uppdate(Msg);
                    break;
                case "Del":
                    Del(Msg);
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