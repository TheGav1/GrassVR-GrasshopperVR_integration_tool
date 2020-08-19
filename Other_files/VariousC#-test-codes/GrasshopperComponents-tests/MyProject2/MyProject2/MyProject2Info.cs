using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace MyProject2
{
    public class MyProject2Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "MyProject2";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("799b6f23-e9ab-4827-9e69-848b777e2a6c");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
