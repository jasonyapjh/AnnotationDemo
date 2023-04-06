using Base.Vision.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sequence.Framework
{
    public class ImageInspectedEventArgs : System.EventArgs
    {
        public InspectionData InspectionData
        {
            get;
            set;
        }
        public int Seq_ID
        {
            get; set;
        }
      
        // ctor.
        public ImageInspectedEventArgs(int seqID, InspectionData inspectionData)
        {
            Seq_ID = seqID;
            InspectionData = inspectionData;
        }
    }
    public class ImageRequestEventArgs : System.EventArgs
    {
     
        public int Seq_ID
        {
            get; set;
        }

        // ctor.
        public ImageRequestEventArgs(int seqID)
        {
            Seq_ID = seqID;
        }
    }
}
