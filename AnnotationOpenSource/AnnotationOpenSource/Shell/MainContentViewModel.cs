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

namespace AnnotationOpenSource.Shell
{
    public class MainContentViewModel : BaseViewModel
    {
        private BaseConfig _configuration;

        public BaseConfig Configuration
        {
            get { return _configuration; }
            set { SetProperty(ref _configuration, value); }
        }
        bool captured = false;
        public DelegateCommand<Object> ClickProductionCommand { get; set; }
        public DelegateCommand<Object> LeftMouseButtonDown { get; set; }
        public DelegateCommand<Object> LeftMouseButtonUp { get; set; }
        public DelegateCommand<Object> PreviewMouseMove { get; set; }
        public MainContentViewModel(IContainerExtension containerExtension, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService) : base(containerExtension, eventAggregator, regionManager, dialogService)
        {
            ClickProductionCommand = new DelegateCommand<object>(OnClickProductionCommand);
            LeftMouseButtonDown = new DelegateCommand<object>(OnLeftMouseButtonDown);
            LeftMouseButtonUp = new DelegateCommand<object>(OnLeftMouseButtonUp);
            PreviewMouseMove = new DelegateCommand<object>(OnPreviewMouseMove);
            Configuration = new BaseConfig();
        }

        private void OnPreviewMouseMove(object obj)
        {
            if (captured)
            {
                RectX = PanelX - 50.0;
                RectY = PanelY - 50.0;
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
            PanelX = 100;
            PanelY = 100;
            RectX = PanelX - 50.0;
            RectY = PanelY - 50.0;
            //ContentControl CC = new ContentControl();
            Regions.RequestNavigate(RegionManagerEntity.WorkspaceRegionSource, ScreenEntity.ProductionView);
            OpenFileDialog openPic = new OpenFileDialog();
            if (openPic.ShowDialog() == true)
            {

                var img = new Mat(openPic.FileName, ImreadModes.Unchanged);
                Bitmap bitmap = img.ToBitmap();
                StationAWindow = OpenCV.ConvertBitmapToBitmapSource(bitmap);

                Rect rect = new Rect(10, 10, 30, 30);
                // StationAWindow = OpenCV.MatToBitmapImage(img);
                // Bitmap test = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
                // test.ToBitmapSource();
                // StationAWindow = BitmapSourceConverter
                //OpenCV.AttachImage(StationAWindow, bitmap);

                //CC.Content = new ImageInfo() { StationAWindow = MatToBitmapImage(img) };

                //Cv2
                //    Image<Gray, byte> gambarAbu = gambar.Convert<Gray, byte>();
                //   myGreyImage.Source = Emgu.CV.WPF.BitmapSourceConvert.ToBitmapSource(gambarAbu);
                /*using (var iplImage = new Mat(@"..\..\Images\Penguin.png", ImreadModes.AnyDepth | ImreadModes.AnyColor))
                {
                    Cv2.Dilate(iplImage, iplImage, new Mat());

                    myImage.Source = iplImage.ToWriteableBitmap(PixelFormats.Bgr24);
                }*/
            }
        }


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
