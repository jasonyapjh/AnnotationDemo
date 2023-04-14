using Base.Common;
using Base.Sequence.Framework;
using Base.Vision.Framework;
using Base.Vision.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Base.Sequence.InspSeq
{
    public class SampleSeq : BaseSequence
    {
        private InspectionData m_InspectionData = null;
        private ImageInfo m_ImageInfo;
        public OCRShapeMatchTool OCRShapeMatchTool { get; set; }
        public bool InitDone = false;

        public enum DelayTimer
        {
            FirstDelay = 0,
            SecondDelay = 1,
            ThirdDelay = 2,
        }
        public enum ErrorTimer
        {
            ErrTime1 = 0,
            ErrTime2 = 1,
            ErrTime3 = 2,
        }
        public enum ExtSeqNum
        {
            None = 0,
            Ready = 1,
            CaptureImage = 2,
            WaitImage = 3,
            CheckImageCount = 4,
            Idle = 5,
            ExtInspectImage = 6,
            //ProcessImage=2,
        }
        public ExtSeqNum ExtSeq = ExtSeqNum.None;
        public SampleSeq(string seqConfigRef)
        {
           // m_ImgAcq = new VisionCamera(Const.StationA, SysCfg.CameraList[Const.StationA].FullName, SysCfg.CameraList[Const.StationA].EnableRotation, SysCfg.CameraList[Const.StationA].RotationAngle);
            OnLoadCfg(seqConfigRef);
            GrabbedImage = null;
            //OCRTool = new OCRInspectionTool(SysCfg, InspCfg, 0);
            SeqNum = SN.EOS;
           // m_InspCfg = InspCfg;
            //ImageToCaptures = InspCfg.InspectorPosition[0].NumberOfCapture;
            //tokenSource = new CancellationTokenSource();
            //token = tokenSource.Token;
            //Encoder = new AdlinkEncoder8124(SysCfg.EncoderCfg[0].CardID, SysCfg.EncoderCfg[0].ChannelID);
            InitDone = false;
        }

        internal override void OnRunSeq(object sender, EventArgs args)
        {
            m_PollWatch.Restart();

            lock (m_SyncSN)
            {
                switch (SeqNum)
                {
                    case SN.Begin: // <-- START / RESUME
                        SeqNum =
                            m_UIEOLRequest ? SN.EndLot :
                            IsBypass ? SN.WaitEndLot :
                            SN.BeginScan;
                        break;

                    // [Polling] --------------------------------
                    case SN.WaitEndLot:
                        if (m_UIEOLRequest)
                        {
                            // NOTE: Flag will get reset at case SN.EndLot.
                            SeqNum = SN.EndLot;
                            InitDone = false;
                        }
                        else if (m_UIStopRequest)
                        {
                            // Reset variable.
                            m_UIStopRequest = false;

                            // Put the sequence into waiting state.
                            SeqNum = SN.WaitResume;
                        }
                        break;

                    case SN.ChangeRecipe:

                        SeqNum = SN.Begin;
                        break;
                    case SN.ChangeRecipeBeforeHand:

                        SeqNum = SN.EOS;
                        break;
                    // ------------------------------------------
                    case SN.LoadModel:
                        //OCRShapeMatchTool.Setup(out InspectionData test);
                       SeqNum = SN.SetReady;
                        break;
                    case SN.BeginScan:
                        m_IsSOTStateChanged = false;
                        m_IsACKStateChanged = false;
                        SeqNum = SN.SetReady;
                        OCRShapeMatchTool = new OCRShapeMatchTool(AnnotationToolConfig);
                        OCRShapeMatchTool.Setup(out InspectionData test);
                        break;

                    case SN.SetReady: // <-- LOOP BACK
                        m_ImageInfo = new ImageInfo();
                        m_InspectionData = null;
                      
                        SeqNum = SN.WaitSOT;

                        IsSequenceReady = true;
                        IsSequenceBusy = false;

                        break;

                    case SN.ResetTool:


                        SeqNum = SN.SetReady;
                        break;

                    case SN.WaitSOT:
                        if (m_UIEOLRequest)
                        {
                            SeqNum = SN.EndLot;
                        }
                        else if (m_UIStopRequest)
                        {
                            m_UIStopRequest = false;
                            SeqNum = SN.WaitResume;
                        }
    
                        else if (SeqTrigger())
                        {
                            m_IsACKStateChanged = true;

                            SeqNum = SN.GrabImage;
                            IsSequenceReady = false;

                        }
                        else
                        {
                            SeqNum = SN.WaitSOT;

                        }
                        break;

              



                    // [Polling] --------------------------------
                    case SN.WaitResume:
                        if (m_UIResumeRequest)
                        {
                            // Reset variable.
                            m_UIResumeRequest = false;
                            // Always resume to SN.Begin.
                            SeqNum = SN.Begin;
                        }
                        break;
                    // ------------------------------------------

                    case SN.GrabImage:
                        RaiseEvent2UI(new ImageRequestEventArgs(0));

                        if (GrabbedImage != null)
                        {
                            if (GrabbedImage.Width == 0 || GrabbedImage.Height == 0)
                            {
                                SeqNum = SN.SetReady;
                            }
                            else
                                SeqNum = SN.InspectImage;
                        }
                        else
                        {
                            SeqNum = SN.GrabImage;
                        }
                        break;

                

                    case SN.InspectImage: // <-- RETEST
                        OCRShapeMatchTool.Run(GrabbedImage, out InspectionData resultdata);

                        RaiseEvent2UI(new ImageInspectedEventArgs(0, resultdata));

                        if (resultdata.Result == Result.Pass)
                            SeqNum = SN.SetResult;
                        else
                            SeqNum = SN.EOS;

                        break;
                    case SN.SetResult: // <--
                        {

                            //Change Here

                            SeqNum = SN.SetReady;
                            //SeqNum = SN.EOS;
                            GrabbedImage = null;
                        }
                        break;
                    case SN.WaitACK:
                        if (m_IsACKStateChanged)
                        {
                            // Reset variables.
                            m_IsACKStateChanged = false;
                            // Turn OFF EOT bit.
                            //WriteBit(OUT.EOT, S.OFF);

                            // Loop back.
                            SeqNum = SN.SetReady;
                        }
                        break;
                    case SN.EndLot:
                        {
                          
                            m_UIEOLRequest = false;
                          //  var msg = string.Format("{0} - Sequence End.", SysCfg.Machine.ModuleName[Const.StationA]);

                           // RaiseEvent2UI(new MessageEventArgs(Const.StationA, msg, MessageType.Info) { HasStopRequest = true, SeqState = SequenceState.SeqEnd });
                            SeqNum = SN.EOS;
                        }
                        break;
                }
            }

            Rate = TimerBenchmark.ResolveTicks(Units.Millisecond, m_PollWatch.ElapsedTicks);
        }

    }
}
