using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GaviVRPlugin.Resources
{
    public class GaviVRLoads : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GaviVRLoads()
          : base("GaviVRLoads", "LoadsVR",
              "Loads container: main distributed + n point",
              "GaviVR", "Kangaroo")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("PTList", "Pts", "ForcePositions", GH_ParamAccess.list);
            pManager.AddTextParameter("INMsg", "INMsg","Message from Unity", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "Run", "True = run", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Rst", "Reset Toggle", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("NewList", "Lst", "Out Position List", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Count", "C", "Runs counter", GH_ParamAccess.item);
            
        }

        public int c;
        public static List<Point3d> Static;
        public bool hasStarted;
        public List<Point3d> List1 = new List<Point3d>();
        string[] ToRemove = new string[] { "{", "}" };

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point> NewList;
            int Count;

            if (c <= 0)
                hasStarted = false;
            else
                hasStarted = true;

            List<string> Msg;
            Count = c / 2;
            Point3d PT = new Point3d(1, 1, 1);

            if (Run)
            {
                Msg = RecivedData(INmsg);
                if (Reset)
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

        List<string> RecivedData(string text)
        {
            if (text != null)
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
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("dadfe9e0-5822-4bbe-9fe9-6d47428a78ce"); }
        }
    }
}