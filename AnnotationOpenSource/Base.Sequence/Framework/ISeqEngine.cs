using Base.Common;
using Base.Vision.Tool;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sequence.Framework
{
    public interface ISeqEngine
    {
        bool IsMachineReady();
        bool IsSequenceReady(int idx);
        bool IsOperationComplete();
        void ResumeRequest(int idx);
        void SeqSingleTrigger(int idx);
        void SeqLoadModel(int idx);
        void BuildSeqEngine();
        void EndMachineLot(bool state);
        void SetSystemCfg(SystemSetting Syscfg);
        void SetAnnotationCfg(int idx, AnnotationToolConfig annotationToolConfig);
        EventHandler GetLoopEntry(int idx);
        void BeginMainSeq();
        void SeqCallBack(EventHandler seqEvent);
        void SendMatImage(int idx,Mat image);
        void StopRequest();
        void InitRequest(bool state);
        void ChangeRecipeRequest(bool state);


    }
}
