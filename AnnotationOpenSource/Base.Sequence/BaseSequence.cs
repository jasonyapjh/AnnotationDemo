using Base.Common;
using Base.Sequence.Framework;
using Base.Vision.Tool;
using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sequence
{
    public abstract class BaseSequence : NotifyPropertyChangedBase
    {
        public enum SN
        {
            Init,
            // ----
            Begin,
            ChangeRecipe,
            ChangeRecipeBeforeHand,
            LoadModel,
            // ----
            WaitEndLot,
            // ----
            BeginScan,
            OpenInitDev,
            EvalImageQueueCnt,
            SetReady,
            WaitSOT,
            WaitResume,
            //  SetSingleImage,
            // SetMultipleImage,
            SetBusy,
            SetLight,
            // ----
            WaitImageReady,
            // ----
            WaitLightDelay,
            GrabImage,
            // ---- For Concurrent
            CheckNumberOfGrab,
            DeQueueImage,
            WaitDeQueueImage,
            ResetTool,
            // ----
            WaitBusyLatch,
            CombineImage,
            MatchPattern,
            FindDataMatrix,
            InspectImage,
            SetResult,
            SetEOT,
            WaitEOTDelay,
            WaitACK,
            EndLot,
            EOS,

            // ---
            TestAllSeq,


        }


        //protected static SystemConfig m_SysCfg = null;
        //private EventPool m_EventPool = new EventPool();
        private event EventHandler OnSeq2UI = null;
        protected CSeqEventArg m_SeqEvArg = new CSeqEventArg();

        protected object m_SyncSN = new object();

        protected SequenceSetting m_SeqCfg = null;

        private SN m_SeqNum = SN.EOS;
        private bool m_IsSequenceReady = false;
        private bool m_IsSequenceBusy = false;

        private double m_Rate;
        protected bool m_IsSOTStateChanged = false;
        protected bool m_IsACKStateChanged = false;
        protected const bool ASYNC = true;
        protected bool m_UIStopRequest = false;
        protected bool m_UIResumeRequest = false;
        protected bool m_UIEOLRequest = false;
        public bool RecipeChangeDone = true;
        private bool m_IsBypass = false;
        protected Stopwatch m_PollWatch = new Stopwatch();
        public AnnotationToolConfig AnnotationToolConfig;
  
        #region Load Config , Recipe
        static internal void OnLoadSysCfg()
        {
            //SysCfg = (SequenceSetting)Serializer.XmlLoad(typeof(SequenceSetting), seqCfgRef);
        }

        internal virtual void OnLoadCfg(string seqCfgRef)
        {
           // m_SeqCfg = (SequenceSetting)Serializer.XmlLoad(typeof(SequenceSetting), seqCfgRef);
        }
       /* internal virtual void OnLoadRecipeCfg(RecipeSetting recipecfg)
        {
            m_InspCfg = recipecfg;
        }*/

        #endregion

        #region Events
        internal event EventHandler UI_Event
        {
            add { OnSeq2UI += value; }
            remove { OnSeq2UI -= value; }
        }
        protected void RaiseEvent2UI(EventArgs evArg)
        {
            if (OnSeq2UI != null && evArg != null)
            {
                OnSeq2UI(this, evArg);
            }
        }
        protected void RaiseEvent2UI()
        {
            // Call the overloaded function.
            RaiseEvent2UI(m_SeqEvArg);
        }

        // Individual module seq's event argument can be passed in here as a parameter
        protected void RaiseEvent2UI(EventArgs evArg, bool isAsync)
        {
            if (isAsync)
            {
                // Perform Asynchronous calling
                if (OnSeq2UI != null && evArg != null)
                {
                    // it is possible now that more than one view can subscribe to this event making it a multicast delegate.
                    var receivers = OnSeq2UI.GetInvocationList();
                    foreach (EventHandler receiver in receivers)
                    {
                        receiver.BeginInvoke(this, evArg, null, null);
                    }
                }
            }
            else
            {
                // Perform Synchronous calling.
                RaiseEvent2UI(evArg);
            }
        }

        // Depend on what get passed into m_SeqEvArg (default).
        protected void RaiseEvent2UI(bool isAsync)
        {
            if (isAsync)
            {
                // Perform Asynchronous calling
                // Call the overloaded function. Default argument object is m_SeqEvArg.
                RaiseEvent2UI(m_SeqEvArg, ASYNC);
            }
            else
            {
                // Perform Synchronous calling.
                // Call the overloaded function. Default argument object is m_SeqEvArg.
                RaiseEvent2UI(m_SeqEvArg);
            }
        }
        protected bool SeqTrigger()
        {
            if (SequenceTrigger)
            {
                SequenceTrigger = false;
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region Virtual Implementation
        internal abstract void OnRunSeq(object sender, EventArgs args);
        // internal virtual void OnBegin(object sender, EventArgs args) { }
        #endregion
        #region Init, Start, Stop, Reset, Endlot void

        internal void Init()
        {
            lock (m_SyncSN)
            {
                m_SeqNum = SN.Init;
            }
        }
        internal void Start()
        {
            lock (m_SyncSN)
            {
                // Reset counter when starting a new lot.
                // 
                // NOTE:
                // We reset the counters here instead of after end lot
                // because user may want to look at the counters after 
                // the lot has ended.
                // CounterInfos.ForEach(i => i.Count = 0);

                m_SeqNum = SN.Begin;
            }
        }
        internal void ChangeRecipeBeforeHand()
        {
            lock (m_SyncSN)
            {
                // Reset counter when starting a new lot.
                // 
                // NOTE:
                // We reset the counters here instead of after end lot
                // because user may want to look at the counters after 
                // the lot has ended.
                // CounterInfos.ForEach(i => i.Count = 0);

                m_SeqNum = SN.ChangeRecipeBeforeHand;
            }
        }
        internal void ChangeRecipes()
        {
            lock (m_SyncSN)
            {
                // Reset counter when starting a new lot.
                // 
                // NOTE:
                // We reset the counters here instead of after end lot
                // because user may want to look at the counters after 
                // the lot has ended.
                // CounterInfos.ForEach(i => i.Count = 0);

                m_SeqNum = SN.ChangeRecipe;
            }
        }
        internal void Resume()
        {
            m_UIResumeRequest = true;
        }
        internal void Stop()
        {
            m_UIStopRequest = true;
        }
        internal void ResetTool()
        {
            lock (m_SyncSN)
            {
                m_SeqNum = SN.ResetTool;
            }
        }
        internal void Reset()
        {
            lock (m_SyncSN)
            {
                m_SeqNum = SN.SetReady;
            }
        }
        internal void EndLot(bool canEndLot)
        {
            // Request for EOL by setting a flag.
            m_UIEOLRequest = canEndLot;
        }
        internal void LoadModel()
        {
            lock (m_SyncSN)
            {
                m_SeqNum = SN.LoadModel;
            }
        }
        internal void ForceStop()
        {
            lock (m_SyncSN)
            {
                // Force the sequence to EOS.
                m_SeqNum = SN.EOS;
            }
        }
        #endregion
      
        #region Properties
        /*public EventPool EventPool
        {
            get { return m_EventPool; }
        }*/
        internal static SystemSetting SysCfg
        {
            set;
            get;
        }
        /* internal static RecipeSetting m_Recipe
         {
             get; set;
         }*/
     
        public SN SeqNum
        {
            get { return this.m_SeqNum; }
            set { SetProperty(ref m_SeqNum, value); }
        }
        public double Rate
        {
            get { return this.m_Rate; }
            set { SetProperty(ref m_Rate, value); }
        }
        public bool IsBypass
        {
            get { return this.m_IsBypass; }
            set { SetProperty(ref m_IsBypass, value); }
        }
        public string ErrorMessage
        {
            get;
            private set;
        }
        internal bool IsSequenceReady
        {
            get { return this.m_IsSequenceReady; }
            set { SetProperty(ref m_IsSequenceReady, value); }
        }
        internal bool IsSequenceBusy
        {
            get { return this.m_IsSequenceBusy; }
            set { SetProperty(ref m_IsSequenceBusy, value); }
        }
        internal bool ManualSingleTrigger
        {
            get;
            set;
        }
        internal bool ManualContinuousTrigger
        {
            get;
            set;
        }
        internal bool SequenceTrigger
        { get; set; }
        private Mat hImage;
        public Mat GrabbedImage
        {
            get { return this.hImage; }
            set { SetProperty(ref hImage, value); }
        }
    
        #endregion

    }
}
