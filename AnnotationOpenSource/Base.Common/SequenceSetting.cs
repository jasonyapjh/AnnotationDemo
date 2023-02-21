using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Common
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SequenceSetting
    {
        public List<Timer> ErrorTimers;
        public List<Timer> DelayTimers;
        public SequenceSetting()
        {
            ErrorTimers = new List<Timer>();
            DelayTimers = new List<Timer>();

        }
        [DisplayName("Sequence ID")]
        public int SeqID { get; set; } = 0;
        [DisplayName("Sequence Name")]
        public string SeqName { get; set; } = "Default";
        [DisplayName("Poll Interval")]
        public int PollInterval { get; set; } = 1;
        /* public static SequenceSetting Open(string path)
         {
             if (string.IsNullOrEmpty(path))
             {
                 return null;
             }

             SequenceSetting sequenceSetting = Open("SequenceSetting", path) as SequenceSetting;
             if (!m_ConfigTbl.Keys.Contains(path))
             {
                 m_ConfigTbl.Add(path, configObj);
                 m_InstanceTbl.Add(path, sequenceSetting);
             }

             return sequenceSetting;
         }*/

    }
    public class Timer
    {
        #region Timer
        public Timer()
        {
        }


        [DisplayName("ID")]
        public int ID { get; set; } = 0;
        [DisplayName("TimeOut")]
        public float TimeOut { get; set; } = 1f;
        [DisplayName("Description")]
        public string Description { get; set; } = "Empty";
        [DisplayName("Max")]
        public float Max { get; set; } = 100f;
        [DisplayName("Min")]
        public float Min { get; set; } = 0f;

        [DisplayName("UoM")]
        public string UoM { get; set; } = "s";

        public float PrevTimeOut
        {
            set;
            get;
        }
        #endregion
    }
}
