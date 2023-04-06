using AnnotationOpenSource.Shell;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Sequence;
using Base.Sequence.ThreadServer;
using Base.ConfigServer;
using Base.Common;
using System.Threading;
using System.Windows.Media.Media3D;
using Timer = Base.Common.Timer;
using System.IO;
using Base.Vision.Tool;

namespace AnnotationOpenSource.Module
{
    public class ShellModuleLocator : IModule
    {
        public AnnotationToolConfig SystemSetting;
        //public RecipeSetting RecipeSettings;
        public ShellModuleLocator()
        {

        }
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Shell.Shell, ShellViewModel>();
            containerRegistry.RegisterForNavigation<MainContent, MainContentViewModel>();

            string config_directory = System.IO.Path.GetFullPath(@"..\") + "System Setting";
            Directory.CreateDirectory(config_directory);

            var SystemSettingLoc = string.Format("{0}\\{1}", config_directory, "SystemSetting.Config");

            if (Extension.CheckFileExist(SystemSettingLoc))
            {
                SystemSetting = (AnnotationToolConfig)Serializer.XmlLoad(typeof(AnnotationToolConfig), SystemSettingLoc);
            }
            else
            {
                SystemSetting = new AnnotationToolConfig();

                Serializer.XmlSave(SystemSetting, SystemSettingLoc);
            }
            containerRegistry.RegisterInstance<AnnotationToolConfig>(SystemSetting);
            //--------------------------//
            // --- Sequence Setting --- //
            //--------------------------//
            List<SequenceSetting> seqCfgs = new List<SequenceSetting>();
            containerRegistry.RegisterInstance<List<SequenceSetting>>(seqCfgs);
            seqCfgs.Add(new SequenceSetting() { SeqID = 0, SeqName = "First Thread", PollInterval = 1, DelayTimers= new List<Timer>(), ErrorTimers=new List<Timer>() });
          /*  foreach (SequenceConfig seqCfg in SystemSetting.SequenceConfigList)
            {
                // Sequence Setting
                seqCfgs.Add((SequenceSetting)Serializer.XmlLoad(typeof(SequenceSetting), seqCfg.Reference));
            }*/
            //------------------------------//
            // --- Thread Server object --- //
            //------------------------------//
            int totalModule = 1;
            IThreadEngine IThreadEngine = new CThreadEngine(totalModule);
            containerRegistry.RegisterInstance<IThreadEngine>(IThreadEngine);
            //----------------------------//
            //  --- Seq Server Object --- //
            //----------------------------//
            Base.Sequence.Framework.ISeqEngine ISeqEngine = new SeqEngine();
       
            containerRegistry.RegisterInstance<Base.Sequence.Framework.ISeqEngine>(ISeqEngine);
            // ISeqEngine.SetRecipeCfg(RecipeSettings);
           

            ISeqEngine.BuildSeqEngine();
            ISeqEngine.SetAnnotationCfg(0, SystemSetting);
            #region [9] Thread Server component object initialization
            /******************************************************************/
            /**  Thread Server component object initialization				 **/
            /******************************************************************/

            for (int i = 0; i < totalModule; i++)
            {
                IThreadEngine.Loop_Entry = ISeqEngine.GetLoopEntry(i);
                IThreadEngine.Module_Name = seqCfgs[i].SeqName;
                IThreadEngine.Interval = seqCfgs[i].PollInterval;
                IThreadEngine.Priority_Level = ThreadPriority.AboveNormal;

            }
            IThreadEngine.Start("a");
            #endregion
            /*  containerRegistry.RegisterForNavigation<Workspace.Production.Production, Workspace.Production.ProductionViewModel>();
              containerRegistry.RegisterForNavigation<VisionUI.VisionToolControl, VisionUI.VisionToolControlViewModel>();
              containerRegistry.RegisterInstance(new Base.Vision.HWindow.HWindowInspectionViewModel(), ScreenEntity.LeftInspHWindow);
              containerRegistry.RegisterInstance(new Base.Vision.HWindow.HWindowInspectionViewModel(), ScreenEntity.RightInspHWindow);
              string config_directory = Path.GetFullPath(@"..\") + "Vision Setting\\System Setting\\";
              Directory.CreateDirectory(config_directory);
              //------------------------//
              // --- System Setting --- //
              //------------------------//
              var SystemSettingLoc = string.Format("{0}\\{1}", config_directory, "SystemSetting.Config");

              if (Extension.CheckFileExist(SystemSettingLoc))
              {
                  SystemSetting = (SystemSetting)Serializer.XmlLoad(typeof(SystemSetting), SystemSettingLoc);
              }
              else
              {
                  SystemSetting = new SystemSetting();
                  SystemSetting.Machine.ModuleName.Add(("Cam1"));
                  SystemSetting.SequenceConfigList.Add(new SequenceConfig() { ID = 0, Reference = @"..\Vision Setting\System Setting\Sequence\Cam1.Config" });
                  Serializer.XmlSave(SystemSetting, SystemSettingLoc);

                  Extension.CreateFolder(string.Format("{0}\\{1}", config_directory, "Sequence"));
                  #region First Init Settings
                  SequenceSetting seqsetting = new SequenceSetting() { SeqID = 0, SeqName = "Cam1", PollInterval = 1 };
                  seqsetting.ErrorTimers.Add(new Base.Common.Timer() { ID = 0, TimeOut = 1, Description = "Error Timer", UoM = "s" });
                  seqsetting.ErrorTimers.Add(new Base.Common.Timer() { ID = 1, TimeOut = 1, Description = "Error Timer", UoM = "s" });
                  seqsetting.ErrorTimers.Add(new Base.Common.Timer() { ID = 2, TimeOut = 1, Description = "Error Timer", UoM = "s" });
                  seqsetting.DelayTimers.Add(new Base.Common.Timer() { ID = 0, TimeOut = 1, Description = "Delay Timer", UoM = "s" });
                  seqsetting.DelayTimers.Add(new Base.Common.Timer() { ID = 1, TimeOut = 1, Description = "Delay Timer", UoM = "s" });
                  seqsetting.DelayTimers.Add(new Base.Common.Timer() { ID = 2, TimeOut = 1, Description = "Delay Timer", UoM = "s" });

                  #endregion
                  Serializer.XmlSave(seqsetting, SystemSetting.SequenceConfigList[0].Reference);
              }

              containerRegistry.RegisterInstance(typeof(SystemSetting), SystemSetting);

              //--------------------------//
              // --- Sequence Setting --- //
              //--------------------------//
              List<SequenceSetting> seqCfgs = new List<SequenceSetting>();
              containerRegistry.RegisterInstance<List<SequenceSetting>>(seqCfgs);
              foreach (SequenceConfig seqCfg in SystemSetting.SequenceConfigList)
              {
                  // Sequence Setting
                  seqCfgs.Add((SequenceSetting)Serializer.XmlLoad(typeof(SequenceSetting), seqCfg.Reference));
              }



           

              //------------------------//
              // --- Recipe Settings--- //
              //------------------------//
              var RecipeSettingLoc = SystemSetting.FolderPath.InspRecipeFolder;

              if (!Extension.CheckFolderExist(RecipeSettingLoc))
              {
                  Extension.CreateFolder(RecipeSettingLoc);
                  Extension.CreateFolder(string.Format("{0}\\{1}", RecipeSettingLoc, "Default"));

                  RecipeSettings = new RecipeSetting();
                  RecipeSettings.CreateDefaultRecipeSetting();

                  Serializer.XmlSave(RecipeSettings, string.Format("{0}\\{1}\\{2}", RecipeSettingLoc, "Default", "Default.Config"));
              }
              else
              {
                  string[] fileEntries = Directory.GetDirectories(RecipeSettingLoc);
                  int NumberOfFolder = fileEntries.Count();

                  for (int i = 0; i < NumberOfFolder; i++)
                  {
                      string[] files = Directory.GetFiles(fileEntries[i], "*.Config");
                      RecipeSettings = (RecipeSetting)Serializer.XmlLoad(typeof(RecipeSetting), files[0]);

                  }
              }
              containerRegistry.RegisterInstance<RecipeSetting>(RecipeSettings);*/
        }
    }
}
