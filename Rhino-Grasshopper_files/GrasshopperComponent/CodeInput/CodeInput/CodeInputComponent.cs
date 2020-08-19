using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;
using ScriptComponents;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace CodeInput
{
    public class CodeInputComponent : GH_Component
    {
        Bitmap bitmap = new Bitmap("/Bitmap2.bmp");//update to curent location
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CodeInputComponent()
          : base("CodeInput", "CodIn",
              "Uses a text file as the code of a scripting component.",
              "Maths", "Script")
        {
        }
        public override void CreateAttributes()
        {
            base.CreateAttributes();
            base.m_attributes = new Attributes_Custom(this);
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("File", "F", "Path to the code file. Use the File Path parameter with the Syncronize option enabled.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string file = string.Empty;
            DA.GetData(0, ref file);

            Rectangle bounds = Rectangle.Ceiling(this.Attributes.Bounds);
            var point = new System.Drawing.Point(bounds.X + bounds.Width / 2, bounds.Y - 40);
            var scriptComponent = (Component_CSNET_Script)Grasshopper.Instances.ActiveCanvas.Document.FindComponent(point);

            if (scriptComponent == null)
            {
                this.Message = string.Empty;
                this.AddRuntimeMessage
                    (GH_RuntimeMessageLevel.Warning,
                    string.Format("No scripting component attached."));
            }
            else
                this.Message = scriptComponent.Name;

            try
            {
                string code;
                using (StreamReader sr = new StreamReader(file))
                    code = sr.ReadToEnd();

                if (scriptComponent != null)
                {
                    scriptComponent.SourceCodeChanged(new Grasshopper.GUI.Script.GH_ScriptEditor(Grasshopper.GUI.Script.GH_ScriptLanguage.CS));
                    var splitLines = new List<string>();
                    //0
                    splitLines.Add("// <Custom usign>");// 1
                    splitLines.Add("// </Custom usign>");//2
                    splitLines.Add("// <Custom code>");//3
                    splitLines.Add("// </Custom code>");//5
                    splitLines.Add("// <Custom additional code>");//5
                    splitLines.Add("// </Custom additional code>");//6

                    string[] codes = code.Split(splitLines.ToArray(), StringSplitOptions.None);
                    scriptComponent.ScriptSource.UsingCode = codes[1];
                    scriptComponent.ScriptSource.ScriptCode = codes[3];
                    scriptComponent.ScriptSource.AdditionalCode = codes[5];
                    scriptComponent.ExpireSolution(true);
                }
            }
            catch (Exception e)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return bitmap;
            }
        }

        public class Attributes_Custom : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
        {
            public Attributes_Custom(GH_Component owner) : base(owner) { }

            protected override void Layout()
            {
                base.Layout();
            }

            protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
            {
                base.Render(canvas, graphics, channel);

                if (channel == GH_CanvasChannel.Objects)
                {
                    Rectangle rectangle = new Rectangle();
                    int diameter = 30;

                    rectangle.Y = (int)Bounds.Y - 40 - diameter / 2;
                    rectangle.X = (int)Bounds.X + (int)Bounds.Width / 2 - diameter / 2;
                    rectangle.Width = diameter;
                    rectangle.Height = diameter;

                    Pen pen = new Pen(Color.Black, 1);
                    pen.DashPattern = new float[] { 1.0f, 3.0f };
                    graphics.DrawArc(pen, rectangle, 0, 360);

                    var font = new Font(FontFamily.GenericSansSerif, 4.0f, FontStyle.Regular);
                    StringFormat format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;
                    graphics.DrawString("place\r\ncomponent\r\nhere", font, Brushes.Black, rectangle, format);
                }
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d66b2002-36e0-430d-8280-32b4c07fd661"); }
        }
    }
}


