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
        public DelegateCommand<Object> ClickProductionCommand { get; set; }
        public DelegateCommand<Object> LeftMouseButtonDown { get; set; }
        public DelegateCommand<Object> LeftMouseButtonUp { get; set; }
        public DelegateCommand<Object> PreviewMouseMove { get; set; }
        public ObservableCollection<EnableRegionCollector> EnableRegionCollection { get; set; }
        public ObservableCollection<DisplayObject> DisplayCollection { get; private set; }

        public OCRShapeMatchTool OCRTool;
        public MainContentViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService) : base(containerExtension, eventAggregator, regionManager, dialogService)
        {
            ClickProductionCommand = new DelegateCommand<object>(OnClickProductionCommand);
            LeftMouseButtonDown = new DelegateCommand<object>(OnLeftMouseButtonDown);
            LeftMouseButtonUp = new DelegateCommand<object>(OnLeftMouseButtonUp);
            PreviewMouseMove = new DelegateCommand<object>(OnPreviewMouseMove);
            Configuration = new OCRShapeMatchConfig();
            EnableRegionCollection = new ObservableCollection<EnableRegionCollector>();
            Configuration = new OCRShapeMatchConfig();
            OCRTool = new OCRShapeMatchTool(Configuration);
            DisplayCollection = new ObservableCollection<DisplayObject>();
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
                for (int i = 0; i < test.ResultOutput.Count; i++)
                {
                    DisplayCollection.Add(new DisplayObject(test.ResultOutput[i].Description, test.ResultOutput[i].MatObject, test.ResultOutput[i].Color));
                }
            }
        
        }

        public class RectItem
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
        #region EnableRegion
        public class EnableRegionCollector : BindableBase
        {
            private string m_ShapeName;
            private RectItem m_rect;
            public string Key
            {
                get { return this.m_ShapeName; }
                set { SetProperty(ref m_ShapeName, value); }
            }
            public RectItem Shape
            {
                get { return this.m_rect; }
                set
                {
                    SetProperty(ref m_rect, value);
                }
            }

            public EnableRegionCollector(string name, RectItem enable)
            {
                Key = name;
                Shape = enable;
            }
        }
        private EnableRegionCollector m_EnabledShape;
        public EnableRegionCollector SelectedRegion
        {
            get { return this.m_EnabledShape; }
            set 
            { 
                SetProperty(ref m_EnabledShape, value);
                RectWidth = value.Shape.Width;
                RectHeight = value.Shape.Height;
                PanelX = RectWidth;
                PanelY = RectHeight;
                RectX = value.Shape.X;
                RectY = value.Shape.Y;
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
            set { SetProperty(ref this._stationAWindow, value); }
        }
        private double _panelX;
        private double _panelY;
        private double _rectX;
        private double _rectY;
        private double _rectWidth;
        private double _rectHeight;

        public double RectWidth
        {
            get { return _rectWidth; }
            set
            {
                if (value.Equals(_rectWidth)) return;
                SetProperty(ref this._rectWidth, value);
            }
        }
        public double RectHeight
        {
            get { return _rectHeight; }
            set
            {
                if (value.Equals(_rectHeight)) return;
                SetProperty(ref this._rectHeight, value);
            }
        }
        public double RectX
        {
            get { return _rectX; }
            set
            {
                if (value.Equals(_rectX)) return;
                SetProperty(ref this._rectX, value);
            }
        }

        public double RectY
        {
            get { return _rectY; }
            set
            {
                if (value.Equals(_rectY)) return;
                SetProperty(ref this._rectY, value);
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
