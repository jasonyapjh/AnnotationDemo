using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnnotationOpenSource
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlRoot("annotation")]
    public class AnnotationConfig
    {
        public string folder { get; set; } = "";
        public string filename { get; set; } = "";
        public string path { get; set; } = "";
        public AnnotationConfig()
        {
            folder = "null";
            filename = "null";
            path = "null";
            source = new Source();
            size = new Size();
            segmented = 0;
            objects = new List<Objects>();
            //objects.Add(new Objects() { bndbox = new BoundingBox(10,10,100,100)});
            //objects.Add(new Objects() { bndbox = new BoundingBox(20, 20, 200, 200) });
        }
        public Source source { get; set; }
        public Size size { get; set; }
        public int segmented { get; set; } = 0;
        [XmlElement("object", Type = typeof(Objects))]
        public List<Objects> objects { get; set; }
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlRoot("source")]
    public class Source
    {
        public Source()
        {

        }
        public string database { get; set; } = "Unknown";
    }
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlRoot("size")]
    public class Size
    {
        public Size()
        {

        }
        public int width { get; set; } = 0;
        public int height { get; set; } = 0;
        public int depth { get; set; } = 0;
    }
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlType("object")]
    [XmlRoot("object")]
    public class Objects
    {
        public Objects()
        {
            name = "null";
            pose = "Unspecified";
            truncated = 0;
            difficult = 0;
            bndbox = new BoundingBox();
        }
        public string name { get; set; } = null;
        public string pose { get; set; } = "Unspecified";
        public int truncated { get; set; } = 0;
        public int difficult { get; set; } = 0;
        public BoundingBox bndbox { get; set; }
    }
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [XmlRoot("bndbox")]
    public class BoundingBox
    {
        public BoundingBox()
        {
        }

        public BoundingBox(int xmin, int ymin, int xmax, int ymax)
        {
            this.xmin = xmin;
            this.ymin = ymin;
            this.xmax = xmax;
            this.ymax = ymax;
        }
 
        public int xmin { get; set; } = 0;
        public int ymin { get; set; } = 0;
        public int xmax { get; set; } = 0;
        public int ymax { get; set; } = 0;
    }
}
