using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GaviVRPlugin
{
    public class GaviVRPluginInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GaviVRPlugin";
            }
        }
        public override Bitmap Icon =>
                //Return a 24x24 pixel bitmap to represent this GHA library.
                new Bitmap(@"C:\Users\giaco\Documents\tesi\scripts\Grasshopper\GaviVRPlugin\GaviVRPlugin\Resources\GaviVR.bmp", true);


        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Unity VR Connectivity Commands";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("76da32e3-3762-4e25-84f4-e4537a8ebf53");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "TheGavi";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "guithub @TheGavi";
            }
        }

        public object Properties { get; private set; }
    }
}
