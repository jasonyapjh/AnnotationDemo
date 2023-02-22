using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Vision.Framework
{
    public class DisplayObject
    {
        public DisplayObject(string Name, Mat Objects, string Color = "red", string Lut = "default")
        {
            //HObject Image,
            this.Name = Name;
            //this.Image = Image;
            this.MatObjects = Objects;
            this.Color = Color;
            this.Lut = Lut;
        }
        private string name;
        public Mat Image;
     
        public Mat MatObjects;
       
        public string Color;
        public string Lut;

        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            return Name;
        }
    }
}
