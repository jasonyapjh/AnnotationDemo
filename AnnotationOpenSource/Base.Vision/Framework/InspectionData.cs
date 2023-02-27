using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Base.Vision.Framework
{
    public enum Result
    {
        Undefined,
        Pass,
        Fail,
    }
    public class InspectionData //: IDisposable
    {
        // NOTE: This class should contain the inspection data of the image,
        // such as the ROI, analysis result, etc. ROI can be displayed
        // to the UI.

        // Properties.
        /*public ImageInfo ImageInfo
        {
            get;
            set;
        }*/
        public Result Result
        {
            get;
            set;

        }
        public double InspectImageCycle
        {
            get;
            set;
        }

        public List<MatInfo> ResultOutput
        {
            get; set;
        }
        public string ResultTuple { get; set; }
       /* public HTuple ResultTuple
        {
            get; set;
        }
        public HTuple ResultDescription
        {
            get; set;
        }*/
        /*  public List<Judgement> Judgements
          {
              get;
              set;
          }*/

        // ctor.

        public InspectionData(Result result = Result.Undefined)
        {
            Result = result;
            ResultOutput = new List<MatInfo>();
           // ResultTuple = new HTuple();
           // ResultDescription = new HTuple();
        }

        #region IDisposable members.
        /* public virtual void Dispose()
         {
             if (ImageInfo != null)
             {
                 ImageInfo.Dispose();
                 ImageInfo = null;
             }
         }*/
        #endregion
    }

    public class MatInfo
    {
        public Mat MatObject
        {
            get; set;
        }
        public string Color
        {
            get; set;
        }
        public string Description
        { get; set; }
        public MatInfo(Mat matobject, string color = Const.Color_Red, string desc = "null")
        {
            this.MatObject = matobject;
            this.Color = color;
            this.Description = desc;
        }
    }
}
