using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sequence.Framework
{
    [Serializable]
    public class CSeqEventArg : EventArgs
    {
        public CSeqEventArg()
        {
            Seq_Msg = string.Empty;
            Stop_Enum = string.Empty;
            Err_Enum = string.Empty;
        }

        // Shared property do not require any thread synchronization
        // because each of them appear as a sepearte instance in its own thread

        public string Seq_Msg
        {
            get;
            set;
        }

        public object Stop_Code
        {
            get;
            set;
        }

        public string Stop_Enum
        {
            get;
            set;
        }

        public object Err_Code
        {
            get;
            set;
        }

        public string Err_Enum
        {
            get;
            set;
        }
        public int Seq_ID
        {
            get;
            set;
        }
      
        public string SeqNum
        {
            get;
            set;
        }

     

    }
}
