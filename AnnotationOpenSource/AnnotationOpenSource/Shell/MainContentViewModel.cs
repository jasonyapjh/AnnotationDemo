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

namespace AnnotationOpenSource.Shell
{
    public class MainContentViewModel : BaseViewModel
    {
        private OCRShapeMatchConfig _configuration;

        public OCRShapeMatchConfig Configuration
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
        public DelegateCommand<Object> ClickProductionCommand { get; set; }
        public DelegateCommand<Object> ClickTeachCommand { get; set; }
        public DelegateCommand<Object> LeftMouseButtonDown { get; set; }
        public DelegateCommand<Object> LeftMouseButtonUp { get; set; }
        public DelegateCommand<Object> PreviewMouseMove { get; set; }
        public DelegateCommand<object> ClickRunCommand { get; set; }
        public ObservableCollection<EnableRegionCollector> EnableRegionCollection { get; set; }
        public ObservableCollection<DisplayObject> DisplayCollection { get; private set; }

        public OCRShapeMatchTool OCRTool;
        public MainContentViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService) : base(containerExtension, eventAggregator, regionManager, dialogService)
        {
            ClickProductionCommand = new DelegateCommand<object>(OnClickProductionCommand);
            ClickTeachCommand = new DelegateCommand<object>(OnTeachCommand);
            LeftMouseButtonDown = new DelegateCommand<object>(OnLeftMouseButtonDown);
            LeftMouseButtonUp = new DelegateCommand<object>(OnLeftMouseButtonUp);
            PreviewMouseMove = new DelegateCommand<object>(OnPreviewMouseMove);
            ClickRunCommand = new DelegateCommand<object>(OnClickRunCommand);
            Configuration = new OCRShapeMatchConfig();
            EnableRegionCollection = new ObservableCollection<EnableRegionCollector>();
            Configuration = new OCRShapeMatchConfig();
            OCRTool = new OCRShapeMatchTool(Configuration);
            DisplayCollection = new ObservableCollection<DisplayObject>();
            //ImageBorder = new Image();
        }

        private void OnClickRunCommand(object obj)
        {
            EnableRegionCollection.Add(new EnableRegionCollector(new RectInfo(10, 10, 30, 30, "C")));
            EnableRegionCollection.Add(new EnableRegionCollector(new RectInfo(20, 60, 50, 50, "A")));
        }

        private void OnTeachCommand(object obj)
        {
            EnableRegionCollection.Clear();
            var str = @"C:\Users\jason.yap\source\repos\AnnotationDemo\AnnotationOpenSource\AnnotationOpenSource\bin\x64\System Setting\BrightFieldRef.jpg";
            Mat image = new Mat(str);
            if (OCRTool.Run(image, out InspectionData test))
            {
                DisplayCollection.Clear();
                //EnableRegionCollection.Clear();
                for (int i = 0; i < test.ResultOutput.Count; i++)
                {
                    DisplayCollection.Add(new DisplayObject(test.ResultOutput[i].Description, test.ResultOutput[i].MatObject, test.ResultOutput[i].Color));
                }
                OCRResult = test.ResultTuple;
                foreach(var item in test.ResultOutputRect)
                {
                    EnableRegionCollection.Add(new EnableRegionCollector(item));
                }
                //EnableRegionCollection = test.ResultOutputRect;
            }

        }

        private void OnPreviewMouseMove(object obj)
        {
            if (captured)
            {
                RectX = PanelX - 10.0;
                RectY = PanelY - 10.0;
            }
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

            if (OCRTool.Setup(out InspectionData test))
            {
                DisplayCollection.Clear();
                EnableRegionCollection.Clear();
                for (int i = 0; i < test.ResultOutput.Count; i++)
                {
                    DisplayCollection.Add(new DisplayObject(test.ResultOutput[i].Description, test.ResultOutput[i].MatObject, test.ResultOutput[i].Color));
                }
                OCRResult = test.ResultTuple;
                //EnableRegionCollection.Add(new EnableRegionCollector("OCR Rect", new RectItem() { X = 10, Y = 10, Width = 30, Height = 30 }));
            }
        
        }

       /* public class RectItem
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }*/
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
                    Key = Shape.Char;
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
                    SetProperty(ref m_EnabledShape, value);
                    RectWidth = value.Shape.Width;
                    RectHeight = value.Shape.Height;
                    PanelX = RectWidth;
                    PanelY = RectHeight;
                    RectX = value.Shape.TLX;
                    RectY = value.Shape.TLY;
                }
            }
        }
        private int m_SelectedRegionIndex;
        public int SelectedRegionIndex
        {
            get { return this.m_SelectedRegionIndex; }
            set { SetProperty(ref m_SelectedRegionIndex, value); }
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
                if (value != null)
                {
                    var width = value.Width;
                    var height = value.Height;
                    var test = ActualWidth;
                    //OpenCV.ResizeImageToHWindow(ImageBorder, width, height);
                    //var test = ImageBorder.Width;
                }
                SetProperty(ref this._stationAWindow, value); 
            }
        }
        private double _panelX;
        private double _panelY;
        private double _rectX;
        private double _rectY;
        private double _rectWidth;
        private double _rectHeight;
        private double VerticalRes;
        private double HorizontalRes;
        private double m_ActualWidth;
        public double ActualWidth
        {
            get { return m_ActualWidth; }
            set { SetProperty(ref this.m_ActualWidth, value); }
        }
        public double RectWidth
        {
            get { return _rectWidth; }
            set
            {
                if (value.Equals(_rectWidth)) return;
                SetProperty(ref this._rectWidth, value);
                if(SelectedRegion!=null)
                {
                    SelectedRegion.Width = value;
                    EnableRegionCollection[SelectedRegionIndex].Shape.Width= value;
                }
            }
        }
        public double RectHeight
        {
            get { return _rectHeight; }
            set
            {
                if (value.Equals(_rectHeight)) return;
                SetProperty(ref this._rectHeight, value);
                if (SelectedRegion != null)
                {
                    SelectedRegion.Height = value;
                    EnableRegionCollection[SelectedRegionIndex].Shape.Height = value;
                }
            }
        }
        public double RectX
        {
            get { return _rectX; }
            set
            {
                if (value.Equals(_rectX)) return;
                SetProperty(ref this._rectX, value);
                if (SelectedRegion != null)
                {
                    SelectedRegion.X = value;
                    EnableRegionCollection[SelectedRegionIndex].Shape.TLX = value;
                }
            }
        }

        public double RectY
        {
            get { return _rectY; }
            set
            {
                if (value.Equals(_rectY)) return;
                SetProperty(ref this._rectY, value);
                if (SelectedRegion != null)
                {
                    SelectedRegion.Y = value;
                    EnableRegionCollection[SelectedRegionIndex].Shape.TLY = value;
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
