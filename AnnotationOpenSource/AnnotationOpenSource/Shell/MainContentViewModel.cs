using AnnotationOpenSource.Framework;
using Base.ViewModels.Base;
using Microsoft.Win32;
using OpenCvSharp;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using OpenCvSharp.Extensions;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using Prism.Commands;
using System.ComponentModel;
using Base.Vision.Tool.Base;
using Base.Vision;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Base.Vision.Tool;
using Base.Vision.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;
using Rect = OpenCvSharp.Rect;
using System.Xml.Linq;
using static AnnotationOpenSource.Shell.MainContentViewModel;
using Image = System.Windows.Controls.Image;
using System.Windows.Forms;
using static System.Windows.Forms.ImageList;
using System.Linq;
using System.Windows.Shapes;
using Base.Common;
using System.Xml;
using System.Data.SqlTypes;
using System.Windows.Media.Media3D;
using Base.ConfigServer;
using static Tensorflow.Keras.Engine.InputSpec;
using System.Threading;
using System.Reflection;
using Base.Sequence.Framework;
using Serilog.Core;
using static Tensorflow.ApiDef.Types;
using Tensorflow.Util;
using Base.Sequence;

namespace AnnotationOpenSource.Shell
{
    public class MainContentViewModel : BaseViewModel
    {
        private AnnotationToolConfig _configuration;

        public AnnotationToolConfig SystemSetting
        {
            get { return _configuration; }
            set { SetProperty(ref _configuration, value); }
        }
        bool captured = false;
        private string _ocrResult;
        public string OCRResult
        {
            get { return _ocrResult; }
            set { SetProperty(ref _ocrResult, value); }
        }
        private int _processimage;
        public int ProcessImage
        {
            get { return _processimage; }
            set { SetProperty(ref _processimage, value); }
        }
        private int _totalimage;
        public int TotalImage
        {
            get { return _totalimage; }
            set { SetProperty(ref _totalimage, value); }
        }
        public DelegateCommand<Object> ClickProductionCommand { get; set; }
        public DelegateCommand<Object> ClickTeachCommand { get; set; }
        public DelegateCommand<Object> LeftMouseButtonDown { get; set; }
        public DelegateCommand<Object> LeftMouseButtonUp { get; set; }
        public DelegateCommand<Object> PreviewMouseMove { get; set; }
        public DelegateCommand<object> ClickRunCommand { get; set; }
        public DelegateCommand<object> ClickNextBoxCommand { get; set; }
        public DelegateCommand<object> ClickFileCommand { get; set; }
        public DelegateCommand<object> ClickCreateAnnotationCommand { get; set; }
        public DelegateCommand<object> ClickNewBoxCommand { get; set; }
        public DelegateCommand<object> ClickDeleteBoxCommand { get; set; }
        public DelegateCommand<object> ClickDeleteImageCommand { get; set; }
        public DelegateCommand<object> ClickUpCommand { get;set; }
        public DelegateCommand<object> ClickDownCommand { get; set; }

        public DelegateCommand<object> ClickLeftCommand { get; set; }
        public DelegateCommand<object> ClickRightCommand { get; set; }
        public DelegateCommand<object> ClickIncreaseWidthCommand { get; set; }
        public DelegateCommand<object> ClickDecreaseWidthCommand { get; set; }
        public DelegateCommand<object> ClickIncreaseHeightCommand { get; set; }
        public DelegateCommand<object> ClickDecreaseHeightCommand { get; set; }
        public DelegateCommand<object> ClickRemoveCorruptImageCommand { get; set; }
        public DelegateCommand<object> ClickRunAsynCommand { get; set; }
        public DelegateCommand<object> ClickStopRunAsynCommand { get; set; }
        public ObservableCollection<EnableRegionCollector> EnableRegionCollection { get; set; }
        public ObservableCollection<DisplayObject> DisplayCollection { get; private set; }
        public ObservableCollection<string> CharBox { get; private set; }
        public ObservableCollection<FileCollector> FileBox { get; private set; }
        public ObservableCollection<LabelCount> LabelCounter { get; private set; }
        private static readonly object lock_RunAsyn = new object();
        public OCRShapeMatchTool OCRTool;
        private int CharCount = 0;
       // public AnnotationToolConfig SystemSetting;
        private string FileDirectory = "";
        public string SystemSettingLoc = "";

        public BackgroundWorker BGWorker;
        public bool InspAsyn = false;
        public bool InspDone = false;
        private ISeqEngine m_SeqEngine;
        //public bool BGDone = false;
        private static object m_SyncObj = new object();
        private int counter = 0;
        public MainContentViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService, ISeqEngine seqEngine) : base(containerExtension, eventAggregator, regionManager, dialogService)
        {
            ClickProductionCommand = new DelegateCommand<object>(OnClickProductionCommand);
            ClickTeachCommand = new DelegateCommand<object>(OnTeachCommand);
            LeftMouseButtonDown = new DelegateCommand<object>(OnLeftMouseButtonDown);
            LeftMouseButtonUp = new DelegateCommand<object>(OnLeftMouseButtonUp);
            PreviewMouseMove = new DelegateCommand<object>(OnPreviewMouseMove);
            ClickRunCommand = new DelegateCommand<object>(OnClickRunCommand);
            ClickNextBoxCommand = new DelegateCommand<object>(OnNextBox);
            ClickFileCommand = new DelegateCommand<object>(OnOpenFile);
            ClickCreateAnnotationCommand = new DelegateCommand<object>(OnCreateAnnotations);
            ClickDeleteImageCommand = new DelegateCommand<object>(OnDeleteImage);
            ClickNewBoxCommand = new DelegateCommand<object>(OnAddNewBox);
            ClickDeleteBoxCommand = new DelegateCommand<object>(OnDeleteBox);
            ClickUpCommand = new DelegateCommand<object>(OnUpCommand);
            ClickDownCommand = new DelegateCommand<object>(OnDownCommand);
            ClickLeftCommand = new DelegateCommand<object>(OnLeftCommand);
            ClickRightCommand = new DelegateCommand<object>(OnRightCommand);
            ClickIncreaseWidthCommand = new DelegateCommand<object>(OnIncWidthCommand);
            ClickDecreaseWidthCommand = new DelegateCommand<object>(OnDecWidthCommand);
            ClickIncreaseHeightCommand = new DelegateCommand<object>(OnIncHeightCommand);
            ClickDecreaseHeightCommand = new DelegateCommand<object>(OnDecHeightCommand);
            ClickRemoveCorruptImageCommand = new DelegateCommand<object>(OnRemoveCorruptedImage);
            ClickRunAsynCommand = new DelegateCommand<object>(OnRunAsyn);
            ClickStopRunAsynCommand = new DelegateCommand<object>(OnStopRunAsyn);
            // Configuration = new AnnotationToolConfig();
            EnableRegionCollection = new ObservableCollection<EnableRegionCollector>();
          
            DisplayCollection = new ObservableCollection<DisplayObject>();
            FileBox = new ObservableCollection<FileCollector>();

            CharBox = new ObservableCollection<string>();
            LabelCounter = new ObservableCollection<LabelCount>();
            IsTrain = true;
            m_SeqEngine = seqEngine;
            m_SeqEngine.SeqCallBack(new EventHandler(OnSeqEvent));

            m_SeqEngine.BeginMainSeq();

            SystemSetting = Containers.Resolve<AnnotationToolConfig>();
            /*   string config_directory = System.IO.Path.GetFullPath(@"..\") + "System Setting";
               Directory.CreateDirectory(config_directory);

               SystemSettingLoc = string.Format("{0}\\{1}", config_directory, "SystemSetting.Config");

               if (Extension.CheckFileExist(SystemSettingLoc))
               {
                   SystemSetting = (AnnotationToolConfig)Serializer.XmlLoad(typeof(AnnotationToolConfig), SystemSettingLoc);
               }
               else
               {
                   SystemSetting = new AnnotationToolConfig();

                   Serializer.XmlSave(SystemSetting, SystemSettingLoc);


               }*/

            // OCRTool = new OCRShapeMatchTool(SystemSetting);
            /*  BGWorker = new BackgroundWorker();
              BGWorker.DoWork += backgroundWorker1_DoWork;
              BGWorker.WorkerSupportsCancellation = true;
              BGWorker.RunWorkerCompleted += BGWorker_RunWorkerCompleted;*/

        }

        private void OnSeqEvent(object sender, EventArgs args)
        {
            lock (m_SyncObj)
            {
                if (args is ImageInspectedEventArgs)
                {
                    ImageInspectedEventArgs evArg = (ImageInspectedEventArgs)args;

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        switch (evArg.Seq_ID)
                        {
                            case 0:
                                DisplayCollection.Clear();
                                //EnableRegionCollection.Clear();
                                for (int i = 0; i < evArg.InspectionData.ResultOutput.Count; i++)
                                {
                                    DisplayCollection.Add(new DisplayObject(evArg.InspectionData.ResultOutput[i].Description, evArg.InspectionData.ResultOutput[i].MatObject, evArg.InspectionData.ResultOutput[i].Color));
                                }
                                OCRResult = evArg.InspectionData.ResultTuple;
                                EnableRegionCollection.Clear();
                                foreach (var item in evArg.InspectionData.ResultOutputRect)
                                {
                                    EnableRegionCollection.Add(new EnableRegionCollector(item));
                                }
                                if (evArg.InspectionData.Result == Result.Pass)
                                {
                                    CreateAnnotations();
                                    ProcessImage++;
                                    if (SelectedFileIndex <= counter)
                                    {
                                        SelectedFileIndex++;
                                    }
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("Insp Done");
                                    }
                                }
                                else
                                {
                                    System.Windows.Forms.MessageBox.Show("Insp Fail");
                                }
                                break;
                        }
                    });
                }
                else if (args is ImageRequestEventArgs)
                {
                    ImageRequestEventArgs evArg = (ImageRequestEventArgs)args;

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        switch (evArg.Seq_ID)
                        {
                            case 0:
                                m_SeqEngine.SendMatImage(evArg.Seq_ID, Images);
                                break;
                        }
                    });
                }
            }
        }

        private void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // BGDone = true;
            if ((SelectedFileIndex <= FileBox.Count()) && InspDone)
            {
                BGWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
         
            //            RunAsyn();
            try
            {
                if (InspDone)
                {
                    InspDone = false;
                    /*  if (SelectedFileIndex <= counter)
                      {
                          SelectedFileIndex++;
                      }*/
                    App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (RunInspection())
                            {
                                CreateAnnotations();
                                ProcessImage++;
                                if (SelectedFileIndex <= counter)
                                {
                                    SelectedFileIndex++;
                                }
                                else
                                {
                                    InspAsyn = false;
                                    BGWorker.CancelAsync();
                                    System.Windows.Forms.MessageBox.Show("Insp Done");
                                }
                                // RunAsynThread.Abort();

                                InspDone = true;
                            }
                            else
                            {

                                InspAsyn = false;
                                BGWorker.CancelAsync();
                                System.Windows.Forms.MessageBox.Show("Insp Fail");
                            }


                        }));

                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(Extension.GetDetailMessage(ex)); }
        }

        private void OnStopRunAsyn(object obj)
        {
            
           // BGWorker.CancelAsync();
            InspAsyn = false;
            m_SeqEngine.EndMachineLot(true);
        }

        private void OnRunAsyn(object obj)
        {

             InspAsyn = true;
            //  InspDone = true;

            // BGWorker.RunWorkerAsync();
         
            m_SeqEngine.SeqSingleTrigger(0);
            m_SeqEngine.SendMatImage(0, Images);
        }

        public void RunAsyn()
        {
            bool lockTaken = false;
            int counter = FileBox.Count();
            try
            {
                Monitor.TryEnter(lock_RunAsyn, ref lockTaken);
                if(lockTaken)
                {
                    if (RunInspection())
                    {
                        CreateAnnotations();
                        ProcessImage++;
                        if (SelectedFileIndex <= counter)
                        {
                            SelectedFileIndex++;
                        }
                        else
                        {
                            InspAsyn = false;
                            BGWorker.CancelAsync();
                            System.Windows.Forms.MessageBox.Show("Insp Done");
                        }
                        // RunAsynThread.Abort();

                        InspDone = true;
                    }
                    else
                    {
                        InspDone = false;
                        InspAsyn = false;
                        BGWorker.CancelAsync();
                        System.Windows.Forms.MessageBox.Show("Insp Fail");
                    }
                    //RunAsynThread.Abort();
                    
                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(Extension.GetDetailMessage(ex)); }
        }
        private void OnRemoveCorruptedImage(object obj)
        {
            foreach(var item in FileBox.ToList())
            {
                Images = new Mat(item.FileName, ImreadModes.Unchanged);
                if (Images.Width == 0 || Images.Height == 0)
                {
                    var str = item.FileName;
                    SelectedFileIndex++;
                    FileBox.RemoveAt(SelectedFileIndex - 1);
                    File.Delete(str);
                   
                    TotalImage--;
                }
            }
            System.Windows.MessageBox.Show("Scan Completed!");

        }
        #region Shape adjustment
        private void OnDecHeightCommand(object obj)
        {
            SelectedRegion.Height--;
            UpdateRectangle();
        }

        private void OnIncHeightCommand(object obj)
        {
            SelectedRegion.Height++;
            UpdateRectangle();
        }

        private void OnDecWidthCommand(object obj)
        {
            SelectedRegion.Width--;
            UpdateRectangle();
        }

        private void OnIncWidthCommand(object obj)
        {
            SelectedRegion.Width++;
            UpdateRectangle();
        }

        private void OnRightCommand(object obj)
        {
            SelectedRegion.X++;
            UpdateRectangle();
        }

        private void OnLeftCommand(object obj)
        {
            SelectedRegion.X--;
            UpdateRectangle();
        }

        private void OnDownCommand(object obj)
        {
            SelectedRegion.Y++;
            UpdateRectangle();
        }

        private void OnUpCommand(object obj)
        {
            SelectedRegion.Y--;
            UpdateRectangle();
        }
        #endregion
        private void OnDeleteImage(object obj)
        {
            var str = SelectedFile.FileName;
            SelectedFileIndex++;
            FileBox.RemoveAt(SelectedFileIndex-1);
            File.Delete(str);
            //SelectedFileIndex++;
            TotalImage--;
        }

        private void OnDeleteBox(object obj)
        {
            EnableRegionCollection.RemoveAt(SelectedRegionIndex);
        }

        private void OnAddNewBox(object obj)
        {
            EnableRegionCollection.Add(new EnableRegionCollector(new RectInfo(40, 40, 40, 40, "A")));
        }
        private void CreateAnnotations()
        {
            FileDirectory = System.IO.Path.ChangeExtension(SelectedFile.FileName, ".xml");

            var config = new AnnotationConfig();

            config.folder = TrainMode;
            config.filename = System.IO.Path.GetFileName(SelectedFile.FileName);
            config.path = SelectedFile.FileName;
            config.size.width = Images.Width;
            config.size.height = Images.Height;
            config.size.depth = Images.Channels();

            // Add rect here

            foreach (var item in EnableRegionCollection)
            {
                // Start --> object
                config.objects.Add(new Objects() { name = item.Key, bndbox = new BoundingBox((int)item.X, (int)item.Y, (int)(item.X + item.Width), (int)(item.Y + item.Height)) });

                if (LabelCounter.Any(x => x.Label == item.Key))
                {
                    var c = LabelCounter.Where(x => x.Label == item.Key);

                    c.FirstOrDefault<LabelCount>().Count++;
                }
                else
                {
                    LabelCounter.Add(new LabelCount(item.Key, 1));
                    CharBox.Add(item.Key);
                }
            }

            Serializer.XmlSave(config, FileDirectory);


            if (Extension.CheckFileExist(FileDirectory))
            {
                SelectedFile.Done = true;
            }
        }
        private void OnCreateAnnotations(object obj)
        {
            FileDirectory = System.IO.Path.ChangeExtension(SelectedFile.FileName, ".xml");

            var config = new AnnotationConfig();

            config.folder = TrainMode;
            config.filename = System.IO.Path.GetFileName(SelectedFile.FileName);
            config.path = SelectedFile.FileName;
            config.size.width = Images.Width;
            config.size.height = Images.Height;
            config.size.depth = Images.Channels();

            // Add rect here

            foreach (var item in EnableRegionCollection)
            {
                // Start --> object
                config.objects.Add(new Objects() { name = item.Key, bndbox = new BoundingBox((int)item.X, (int)item.Y, (int)(item.X + item.Width), (int)(item.Y + item.Height)) });

                if (LabelCounter.Any(x => x.Label == item.Key))
                {
                    var c = LabelCounter.Where(x => x.Label == item.Key);

                    c.FirstOrDefault<LabelCount>().Count++;
                }
                else
                {
                    LabelCounter.Add(new LabelCount(item.Key, 1));
                    CharBox.Add(item.Key);
                }
            }

            Serializer.XmlSave(config, FileDirectory);
         
            
            if (Extension.CheckFileExist(FileDirectory))
            {
                SelectedFile.Done = true;
            }

            SelectedFileIndex++;
            ProcessImage++;
         //   RunInspection();
        }

        private void OnOpenFile(object obj)
        {
            string filename = "";
            using (var dialog = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                filename = dialog.SelectedPath;
            }
            if (filename != null)
            {
               // SelectedFile = null;
                //SelectedFileIndex = 0;
                FileBox.Clear();
                string[] supportedFileExt = new string[] { "*.jpg", "*.tiff", "*.tif", "*.gif", "*.bmp", "*.jpeg", "*.png" };

                int SuppCount = supportedFileExt.Count();

                TotalImage = 0;
                ProcessImage++;
                LabelCounter.Clear();
                for (int j = 0; j < SuppCount; j++)
                {
                    string[] files = Directory.GetFiles(filename, supportedFileExt[j], SearchOption.AllDirectories);

                    int CheckFileCount = files.Count();
                    if (CheckFileCount > 0)
                    {
                        for (int i = 0; i < CheckFileCount; i++)
                        {
                            TotalImage = +CheckFileCount;
                            var file = System.IO.Path.ChangeExtension(files[i].ToString(), ".xml");
                            if(Extension.CheckFileExist(file))
                            {
                                FileBox.Add(new FileCollector(files[i].ToString()) { Done = true });
                                ProcessImage++;
                                using (AnnotationConfig config = (AnnotationConfig)Serializer.XmlLoad(typeof(AnnotationConfig), file))
                                {
                                    foreach(var item in config.objects)
                                    {
                                        if (LabelCounter.Any(x => x.Label == item.name))
                                        {
                                            var c = LabelCounter.Where(x => x.Label == item.name);

                                            c.FirstOrDefault<LabelCount>().Count++;
                                        }
                                        else
                                        {
                                            LabelCounter.Add(new LabelCount(item.name, 1));
                                            CharBox.Add(item.name);
                                        }
                                    }
                                }
                                
                            }
                            else
                                FileBox.Add(new FileCollector(files[i].ToString()));

                        }
                    }
                }
            }
            counter = FileBox.Count();
        }

        private void OnNextBox(object obj)
        {
            /* var check = EnableRegionCollection;
             SelectedRegion = EnableRegionCollection[CharCount];
             Rect rect = Rect.FromLTRB((int)SelectedRegion.X, (int)SelectedRegion.Y, (int)(SelectedRegion.Width+ SelectedRegion.X) , (int)(SelectedRegion.Height+ SelectedRegion.Y));
             var clone = Images.Clone();
             clone.Rectangle(rect, new Scalar(0, 0, 255, 255), 3);
             Bitmap bitmap = clone.ToBitmap();
             StationAWindow = OpenCV.ConvertBitmapToBitmapSource(bitmap);
             CharCount++;*/
            SelectedFileIndex++;
        }
        private void UpdateRectangle()
        {
            // if (SetXDone && SetYDone && SetWidthDone && SetHeightDone)
            // {
            Rect rect = Rect.FromLTRB((int)SelectedRegion.X, (int)SelectedRegion.Y, (int)(SelectedRegion.Width + SelectedRegion.X), (int)(SelectedRegion.Height + SelectedRegion.Y));
            var clone = Images.Clone();
            if (SystemSetting.RectColor == RectColor.Black)
            {
                clone.Rectangle(rect, new Scalar(0, 0, 255, 255), 3);
            }
            else
                clone.Rectangle(rect, new Scalar(255, 255, 255, 255), 3);
            Bitmap bitmap = clone.ToBitmap();
            StationAWindow = OpenCV.ConvertBitmapToBitmapSource(bitmap);
            //   SetAllModeFalse();
            // }

        }
        private bool RunInspection()
        {
            if(Images.Width==0 || Images.Height==0)
            {
                System.Windows.MessageBox.Show("File is corrupted!");
                return false;
            }

            bool AutoCreate = false;
            if (OCRTool.Run(Images, out InspectionData test))
            {
                DisplayCollection.Clear();
                //EnableRegionCollection.Clear();
                for (int i = 0; i < test.ResultOutput.Count; i++)
                {
                    DisplayCollection.Add(new DisplayObject(test.ResultOutput[i].Description, test.ResultOutput[i].MatObject, test.ResultOutput[i].Color));
                }
                OCRResult = test.ResultTuple;
                EnableRegionCollection.Clear();
                foreach (var item in test.ResultOutputRect)
                {
                    EnableRegionCollection.Add(new EnableRegionCollector(item));
                }
                //EnableRegionCollection = test.ResultOutputRect;
                if (test.Result == Result.Pass)
                    AutoCreate = true;
            }
            Bitmap bitmap = DisplayCollection[DisplayCollection.Count - 1].MatObjects.ToBitmap();
            StationAWindow = OpenCV.ConvertBitmapToBitmapSource(bitmap);

           
            return AutoCreate;
        }
        private void OnClickRunCommand(object obj)
        {
            /*LabelCounter.Clear();
            string folderDirectory = System.IO.Path.GetDirectoryName(FileBox[0].FileName);
            foreach (var file in FileBox)
            {
                if (file.Done)
                {
                    var fileXml = System.IO.Path.ChangeExtension(file.FileName.ToString(), ".xml");

                    if (Extension.CheckFileExist(fileXml))
                    {
                        using (AnnotationConfig config = (AnnotationConfig)Serializer.XmlLoad(typeof(AnnotationConfig), fileXml))
                        {
                            Mat image = new Mat(config.path, ImreadModes.Unchanged);

                            foreach (var item in config.objects)
                            {
                                if (LabelCounter.Any(x => x.Label == item.name))
                                {
                                    var c = LabelCounter.Where(x => x.Label == item.name);

                                    c.FirstOrDefault<LabelCount>().Count++;

                                    var shape = Rect.FromLTRB(item.bndbox.xmin, item.bndbox.ymin, item.bndbox.xmax, item.bndbox.ymax);
                                    Mat cropped = new Mat(image, shape);

                                    string folderpath = $"{folderDirectory}/{item.name}";
                                    string savefile = string.Format("{0}\\{1}\\{2}_{3}.jpg", folderDirectory, item.name, item.name, c.FirstOrDefault<LabelCount>().Count);
                                    cropped.ImWrite(savefile);
                                }
                                else
                                {
                                    string folderpath = $"{folderDirectory}/{item.name}";
                                    Extension.CreateFolder(folderpath);
                                    LabelCounter.Add(new LabelCount(item.name, 1));
                                    var shape = Rect.FromLTRB(item.bndbox.xmin, item.bndbox.ymin, item.bndbox.xmax, item.bndbox.ymax);
                                    Mat cropped = new Mat(image, shape);

                                    string savefile = string.Format("{0}\\{1}\\{2}_{3}.jpg", folderDirectory, item.name, item.name, 1);
                                    cropped.ImWrite(savefile);
                                    
                                }
                            }
                        }

                    }
                }
            }
            System.Windows.MessageBox.Show("Done Creation!");*/
              RunInspection();
        }

        private void OnTeachCommand(object obj)
        {
            Serializer.XmlSave(SystemSetting, SystemSettingLoc);
        }

        private void OnPreviewMouseMove(object obj)
        {
           /* if (captured)
            {
                RectX = PanelX - 10.0;
                RectY = PanelY - 10.0;
            }*/
        }

        private void OnLeftMouseButtonUp(object obj)
        {
            captured = false;
        }

        private void OnLeftMouseButtonDown(object obj)
        {
            captured = true;
        }

        private void OnClickProductionCommand(object obj)
        {
            m_SeqEngine.SeqLoadModel(0);
           /* if (OCRTool.Setup(out InspectionData test))
            {
                DisplayCollection.Clear();
                EnableRegionCollection.Clear();
                for (int i = 0; i < test.ResultOutput.Count; i++)
                {
                    DisplayCollection.Add(new DisplayObject(test.ResultOutput[i].Description, test.ResultOutput[i].MatObject, test.ResultOutput[i].Color));
                }
                OCRResult = test.ResultTuple;
                //EnableRegionCollection.Add(new EnableRegionCollector("OCR Rect", new RectItem() { X = 10, Y = 10, Width = 30, Height = 30 }));
            }*/
        
        }

        /* public class RectItem
         {
             public double X { get; set; }
             public double Y { get; set; }
             public double Width { get; set; }
             public double Height { get; set; }
         }*/
        #region Label count
        public class LabelCount : BindableBase
        {
            private string _label;
            private int _count;
            public string Label
            {
                get { return this._label; }
                set { SetProperty(ref _label, value); }
            }
            public int Count
            {
                get { return this._count; }
                set { SetProperty(ref _count, value); }
            }
            public LabelCount(string a, int i)
            {
                this.Label = a;
                this.Count = i;
            }
        }
        #endregion
        #region FileBox
        public class FileCollector : BindableBase
        {
            private string m_fileName;
            private bool m_done = false;
            public string FileName
            {
                get { return this.m_fileName; }
                set { SetProperty(ref m_fileName, value); }
            }
            public bool Done
            {
                get { return this.m_done; }
                set { SetProperty(ref m_done, value); }
            }
            public FileCollector(string name)
            {
                FileName = name;
            }
        }
        private FileCollector m_selectedfile;
        public FileCollector SelectedFile
        {
            get { return this.m_selectedfile; }
            set 
            {
                bool Exists = false;
                SetProperty(ref m_selectedfile, value);
                if (value != null)
                {
                    Images = new Mat(SelectedFile.FileName, ImreadModes.Unchanged);
                    if (Images.Width != 0 || Images.Height != 0)
                    {
                        Bitmap bitmap = Images.ToBitmap();
                        StationAWindow = OpenCV.ConvertBitmapToBitmapSource(bitmap);

                        var file = System.IO.Path.ChangeExtension(SelectedFile.FileName, ".xml");
                        if (Extension.CheckFileExist(file))
                        {
                            Exists = true;
                            EnableRegionCollection.Clear();
                            var config = (AnnotationConfig)Serializer.XmlLoad(typeof(AnnotationConfig), file);
                            if (config.folder == "train")
                            {
                                IsTrain = true;
                                IsTest = false;
                                IsValid = false;
                            }
                            else if (config.folder == "test")
                            {
                                IsTrain = false;
                                IsTest = true;
                                IsValid = false;
                            }
                            else
                            {
                                IsTrain = false;
                                IsTest = false;
                                IsValid = true;
                            }
                            foreach (var item in config.objects)
                            {

                                EnableRegionCollection.Add(new EnableRegionCollector(new RectInfo(item.bndbox.xmin, item.bndbox.ymin, (item.bndbox.xmax - item.bndbox.xmin), (item.bndbox.ymax - item.bndbox.ymin), item.name)));
                            }
                        }
                    }
                    else
                        System.Windows.MessageBox.Show("File is corrupted!");
                }
                if(InspAsyn && !Exists)
                {
                    //m_SeqEngine.SendMatImage(0, Images);
                    m_SeqEngine.SeqSingleTrigger(0);
                  
                    //   InspDone = false;
                    //    RunAsyn();
                    //   InspDone = true;
                }
                else if(InspAsyn && Exists)
                {
                    SelectedFileIndex++;
                }
            }
        }
        private int m_selectedfileindex;
        public int SelectedFileIndex
        {
            get { return this.m_selectedfileindex; }
            set { SetProperty(ref m_selectedfileindex, value); }
        }
        #endregion
        #region EnableRegion
        public class EnableRegionCollector : BindableBase
        {
            private string m_ShapeName;
            private RectInfo m_rect;
            private double m_X;
            private double m_Y;
            private double m_Width;
            private double m_Height;
            public string Key
            {
                get { return this.m_ShapeName; }
                set { SetProperty(ref m_ShapeName, value); }
            }
            
            public RectInfo Shape
            {
                get { return this.m_rect; }
                set
                {
                    SetProperty(ref m_rect, value);
                    X = value.TLX;
                    Y = value.TLY;
                    Width = value.Width;
                    Height = value.Height;
                    Key = value.Char;
                }
            }
            public double X
            {
                get { return this.m_X; }
                set { SetProperty(ref m_X, value); }
            }
            public double Y
            {
                get { return this.m_Y; }
                set { SetProperty(ref m_Y, value); }
            }
            public double Width
            {
                get { return this.m_Width; }
                set { SetProperty(ref m_Width, value); }
            }
            public double Height
            {
                get { return this.m_Height; }
                set { SetProperty(ref m_Height, value); }
            }
            public EnableRegionCollector(RectInfo enable)
            {
                Shape = enable;
            }
        
        }
        private EnableRegionCollector m_EnabledShape;
        public EnableRegionCollector SelectedRegion
        {
            get { return this.m_EnabledShape; }
            set 
            {
                if (value != null)
                {
                    if(StationAWindow!=null)
                    {
                        var width = StationAWindow.Width;
                        var height = StationAWindow.Height;
                        //VerticalRes = ActualHeight / height;
                        //HorizontalRes = ActualWidth / width;
                        VerticalRes = 1;
                        HorizontalRes = 1;
                    }
                    SetProperty(ref m_EnabledShape, value);
                    UpdateRectangle();
                //    RectWidth = value.Shape.Width * HorizontalRes;
                //    RectHeight = value.Shape.Height * VerticalRes;
                //    PanelX = RectWidth;
                 //   PanelY = RectHeight;
                //    RectX = value.Shape.TLX * HorizontalRes;
                //    RectY = value.Shape.TLY * VerticalRes;

              


                   
                }
            }
        }
        private int m_SelectedRegionIndex;
        public int SelectedRegionIndex
        {
            get { return this.m_SelectedRegionIndex; }
            set 
            { 
                SetProperty(ref m_SelectedRegionIndex, value);
                CharCount = value;
            }
        }
        #endregion
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            IsTrain = true;

        }

        #region Properties
        #region Display View Collector
        private DisplayObject m_SelectedDisplay;
        public DisplayObject SelectedDisplay
        {
            get { return this.m_SelectedDisplay; }
            set
            {
                SetProperty(ref m_SelectedDisplay, value);
              
                if (SelectedDisplay != null)
                {
                    Bitmap bitmap = SelectedDisplay.MatObjects.ToBitmap();
                    StationAWindow = OpenCV.ConvertBitmapToBitmapSource(bitmap); 
                }
            }
        }
        #endregion

        private BitmapSource _stationAWindow;

        public BitmapSource StationAWindow
        {
            get { return _stationAWindow; }
            set 
            { 
                SetProperty(ref this._stationAWindow, value);
            
            }
        }
        private double _panelX;
        private double _panelY;
        private double _rectX;
        private double _rectY;
        private double _rectWidth;
        private double _rectHeight;
        private double VerticalRes =1 ;
        private double HorizontalRes = 1;
        private double m_ActualWidth;
        private double m_ActualHeight;
        private string m_charkey;
        private string m_SelectedChar;
        private string m_currectimageDir;
        private bool m_IsTrain = true;
        private bool m_IsTest;
        private bool m_IsValid;
        public string TrainMode = "";
      
        public bool IsTrain
        {
            get { return m_IsTrain; }
            set
            {
                SetProperty(ref this.m_IsTrain, value);
                if (value)
                {
                    TrainMode = "train";
                }
            }
        }
        public bool IsTest
        {
            get { return m_IsTest; }
            set 
            { 
                SetProperty(ref this.m_IsTest, value);
                if (value) TrainMode = "test";
            }
        }
        public bool IsValid
        {
            get { return m_IsValid; }
            set 
            { 
                SetProperty(ref this.m_IsValid, value);
                if (value) TrainMode = "valid";
            }
        }


        private Mat m_Images;
        public Mat Images
        {
            get { return m_Images; }
            set { SetProperty(ref this.m_Images, value); }
        }
        public string CurrectImageDir
        {
            get { return m_currectimageDir; }
            set { SetProperty(ref this.m_currectimageDir, value); }
        }
        public string SelectedChar
        {
            get { return m_SelectedChar; }
            set 
            { 
                SetProperty(ref this.m_SelectedChar, value);
                if(SelectedRegion!=null)
                {
                    SelectedRegion.Key = value;
                    EnableRegionCollection[SelectedRegionIndex].Shape.Char = value;
                }
            }
        }
        public double ActualWidth
        {
            get { return m_ActualWidth; }
            set { SetProperty(ref this.m_ActualWidth, value); }
        }
        public double ActualHeight
        {
            get { return m_ActualHeight; }
            set { SetProperty(ref this.m_ActualHeight, value); }
        }
        public string Charkey
        {
            get { return m_charkey; }
            set
            {
                if (value.Equals(m_charkey)) return;
                SetProperty(ref this.m_charkey, value);
                if (SelectedRegion != null)
                {
                    SelectedRegion.Key = value;
                    EnableRegionCollection[SelectedRegionIndex].Shape.Char = value;
                    
                }
            }
        }
    

        public double PanelX
        {
            get { return _panelX; }
            set
            {
                if (value.Equals(_panelX)) return;
                SetProperty(ref this._panelX, value);
            }
        }

        public double PanelY
        {
            get { return _panelY; }
            set
            {
                if (value.Equals(_panelY)) return;
                SetProperty(ref this._panelY, value);
            }
        }
        #endregion


    }
}
