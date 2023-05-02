using Base.Common;
using Base.Sequence.Framework;
using Base.Sequence.InspSeq;
using Base.Vision.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sequence
{
    public class SeqEngine : ISeqEngine
    {
        public int m_TotalModule = 0;
     
        private BaseSequence[] m_BaseSeq = null;
        //public RecipeSetting m_InspCfg;
        private bool[] ExecuteSeq;
        private bool InitRequests = false;
        private bool RecipeRequest = false;
        private bool[] ExeceuteReady;
        // OCR
        //private int[] TriggerPosition;
        private int[] OCRResult;
        private string[] OCRStrings;
        public void BuildSeqEngine()
        {
            // m_TotalModule = BaseSequence.SysCfg.Machine.NumCoreModule + BaseSequence.SysCfg.Machine.NumOfSeq;
            m_TotalModule = 1;
             m_BaseSeq = new BaseSequence[m_TotalModule];
            ExecuteSeq = new bool[m_TotalModule];
            ExeceuteReady = new bool[m_TotalModule];
            #region Core Module Sequence
            //m_BaseSeq[0] = new SampleSeq(BaseSequence.SysCfg.SequenceConfigList[0]);
            m_BaseSeq[0] = new SampleSeq("");

            #endregion
        }
        public void BeginMainSeq()
        {
            InitRequests = false;
            for (int i = 0; i < m_TotalModule; i++)
            {
                m_BaseSeq[i].Start();
            }
        }

        public void ResumeRequest(int idx)
        {
            for (int i = 0; i < m_TotalModule; i++)
            {
                m_BaseSeq[i].Resume();
            }
        }
        public void StopRequest()
        {
            for (int i = 0; i < m_TotalModule; i++)
            {
                m_BaseSeq[i].Stop();
            }
        }
        public void ChangeRecipeRequest(bool state)
        {
         //   throw new NotImplementedException();
        }

        public void EndMachineLot(bool state)
        {
            for (int i = 0; i < m_TotalModule; i++)
            {
                m_BaseSeq[i].EndLot(state);
            }
        }

        public EventHandler GetLoopEntry(int idx)
        {
            return new EventHandler(m_BaseSeq[idx].OnRunSeq);
        }

        public void InitRequest(bool state)
        {
            InitRequests = state;
        }

        public bool IsMachineReady()
        {
            bool bState = true;

            for (int i = 0; i < m_TotalModule; i++)
            {
                if (!m_BaseSeq[i].IsBypass && !m_BaseSeq[i].IsSequenceReady)
                {

                    bState = false;
                }

            }
          
            return bState;
        }
        public void SendMatImage(int i,Mat image)
        {
            m_BaseSeq[i].GrabbedImage = image;
        }
        public bool IsOperationComplete()
        {
            bool bState = true;
            for (int i = 0; i < m_TotalModule; i++)
            {
                if (m_BaseSeq[i].IsSequenceBusy)
                {
                    bState = false;
                }
            }
            return bState;
        }

        public bool IsSequenceReady(int idx)
        {
            bool bState = true;

            if (!m_BaseSeq[idx].IsBypass && !m_BaseSeq[idx].IsSequenceReady)
            {
                bState = false;
            }

            return bState;
        }

        public void SeqCallBack(EventHandler seqEvent)
        {
            for (int i = 0; i < m_TotalModule; i++)
            {
                m_BaseSeq[i].UI_Event += seqEvent;
            }
        }

        public void SeqSingleTrigger(int idx)
        {
            
            m_BaseSeq[idx].SequenceTrigger = true;
            //m_BaseSeq[i].Start();
        }

    
        public void SetSystemCfg(SystemSetting Syscfg)
        {
            BaseSequence.SysCfg = Syscfg;
        }

        public void SetAnnotationCfg(int i, AnnotationToolConfig annotationToolConfig)
        {
            m_BaseSeq[i].AnnotationToolConfig = annotationToolConfig;
        }

        public void SeqLoadModel(int idx)
        {
            m_BaseSeq[idx].LoadModel();
        }
    }
}
