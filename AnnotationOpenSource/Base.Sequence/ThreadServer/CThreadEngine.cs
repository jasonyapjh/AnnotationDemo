using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Sequence.ThreadServer
{
    public sealed class CThreadEngine : IThreadEngine
    {
        private string m_ErrMsg = "";
        private IList m_ModuleNameList;
        private IList m_IntervalList;
        private IList m_ThreadPriorityList;
        private EventHandler m_LoopEntry;
        private ManualResetEvent[] m_EvKillEngine;
        private Delegate[] m_LoopList;
        private object m_PollSync;
        private int m_PollIdx;
        private Thread[] m_PollEngine;

        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(uint period);

        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(uint period);

        public CThreadEngine(int totalModule)
        {
            this.m_ModuleNameList = (IList)new ArrayList(totalModule);
            this.m_IntervalList = (IList)new ArrayList(totalModule);
            this.m_ThreadPriorityList = (IList)new ArrayList(totalModule);
        }

        bool IThreadEngine.Start(string secCode)
        {
            CThreadEngine.timeBeginPeriod(1U);
            this.m_PollSync = new object();
            this.m_EvKillEngine = new ManualResetEvent[this.m_ModuleNameList.Count];
            this.m_PollEngine = new Thread[this.m_ModuleNameList.Count];
            this.m_LoopList = this.m_LoopEntry.GetInvocationList();
            for (int index = 0; index < this.m_ModuleNameList.Count; ++index)
            {
                ThreadStart start = new ThreadStart(this.LoopSeq);
                this.m_EvKillEngine[index] = new ManualResetEvent(false);
                this.m_PollEngine[index] = new Thread(start);
                this.m_PollEngine[index].Priority = (ThreadPriority)this.m_ThreadPriorityList[index];
                this.m_PollEngine[index].Name = this.m_ModuleNameList[index].ToString();
                this.m_PollEngine[index].Start();
                Trace.WriteLine("New Polling Engine starts...", this.m_PollEngine[index].Name);
            }
            return true;
        }

        bool IThreadEngine.Stop()
        {
            return this.TerminateThread(1000);
        }

        bool IThreadEngine.Stop(int timeOut)
        {
            return this.TerminateThread(timeOut);
        }

        private bool TerminateThread(int timeOut)
        {
            for (int index = 0; index < this.m_ModuleNameList.Count; ++index)
            {
                this.m_EvKillEngine[index].Set();
                this.m_PollEngine[index].Join(timeOut);
                this.m_EvKillEngine[index].Close();
            }
            CThreadEngine.timeEndPeriod(1U);
            this.m_PollSync = (object)null;
            return true;
        }

        bool IThreadEngine.IsAlive(int moduleIdx)
        {
            if (moduleIdx >= 0 && moduleIdx < this.m_ModuleNameList.Count)
                return this.m_PollEngine[moduleIdx].IsAlive;
            this.m_ErrMsg = "Invalid Module Index. Must be >= 0 and < Total # of Modules";
            return false;
        }

        EventHandler IThreadEngine.Loop_Entry
        {
            set
            {
                this.m_LoopEntry += value;
            }
        }

        string IThreadEngine.Module_Name
        {
            set
            {
                this.m_ModuleNameList.Add((object)value);
            }
        }

        int IThreadEngine.Interval
        {
            set
            {
                this.m_IntervalList.Add((object)value);
            }
        }

        ThreadPriority IThreadEngine.Priority_Level
        {
            set
            {
                this.m_ThreadPriorityList.Add((object)value);
            }
        }

        string IThreadEngine.Err_Msg
        {
            get
            {
                return this.m_ErrMsg;
            }
        }

        private void LoopSeq()
        {
            int index = 0;
            CThreadEvArg cpollEvArg = new CThreadEvArg();
            lock (this.m_PollSync)
            {
                index = this.m_PollIdx;
                ++this.m_PollIdx;
            }
            EventHandler loop = (EventHandler)this.m_LoopList[index];
            int int32 = Convert.ToInt32(this.m_IntervalList[index]);
            try
            {
                while (!this.m_EvKillEngine[index].WaitOne(int32, true))
                    loop((object)this, (EventArgs)cpollEvArg);
            }
            catch (Exception ex)
            {
                string message = "Module Name: " + this.m_PollEngine[index].Name + "\nSeqNum: " + cpollEvArg.SN + "\nError Message: " + ex.Message + "\nException Detail:\n" + ex.ToString();
                EventLog.WriteEntry("Thread Server", message, EventLogEntryType.Error);
                Trace.WriteLine(message);
            }
        }
    }
    public class CThreadEvArg : EventArgs
    {
        private StringBuilder m_SeqNum = new StringBuilder("Unassigned", 15);

        public string SN
        {
            get
            {
                return this.m_SeqNum.ToString();
            }
            set
            {
                this.m_SeqNum.Remove(0, this.m_SeqNum.Length);
                this.m_SeqNum.Append(value);
            }
        }
    }
}
