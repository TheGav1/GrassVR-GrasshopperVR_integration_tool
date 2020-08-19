using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace CodeInput
{
    public class CodeInputInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "CodeInput";
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
                return new Guid("565bc3b4-f6e4-4a12-b8ba-c490a86836dd");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Gavi";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "github @TheGavi1";
            }
        }
    }
}
