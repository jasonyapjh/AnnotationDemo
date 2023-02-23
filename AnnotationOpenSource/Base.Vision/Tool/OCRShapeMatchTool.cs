using Base.Vision.Shapes.Base;
using Base.Vision.Shapes;
using Base.Vision.Tool.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenCvSharp;
using System.Drawing;
using OpenCvSharp.Extensions;
using Base.Vision.Framework;
using static System.Net.Mime.MediaTypeNames;
using Point = OpenCvSharp.Point;
using System.Runtime.Remoting.Channels;

namespace Base.Vision.Tool
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OCRShapeMatchConfig : BaseConfig
    {
        public string OCRTrainedImage { get; set; } = @"..\System Setting\bcssemi2.png";
        public string OCRBrightFieldRefImage { get; set; } = @"..\System Setting\BrightFieldRef.jpg";
        public string OCRDarkFieldRefImage { get; set; } = @"..\System Setting\DarkFieldRef.jpg";
        public int OCRDarkFieldThreshold { get; set; } = 128;
        public int OCRCharWidth { get; set; } = 30;
        public int OCRCharHeight { get; set; } = 30;
        public string OcrFormat { get; set; } = "AAANNNNNN-nNAA";
        public OCRShapeMatchConfig()
        {
            _search_ROI = new Rectangle1();
            _char_ROI = new Rectangle1();
        }
        [XmlIgnore]
        public IDictionary<string, Shape> TeachRegions = new Dictionary<string, Shape>();
        private bool use_Search_ROI;

        [Browsable(false)]
        public Shape Search_ROI
        {
            get => _search_ROI;
            set
            {
                SetProperty(ref _search_ROI, value, "Search_ROI");
                UpdateTeachRegion();
            }
        }
        [Browsable(false)]
        public Shape Character_ROI
        {
            get => _char_ROI;
            set
            {
                SetProperty(ref _char_ROI, value, "Character_ROI");
                UpdateTeachRegion();
            }
        }

        private Shape _search_ROI;
        private Shape _char_ROI;
        public override void UpdateTeachRegion()
        {
            TeachRegions.Clear();
            TeachRegions.Add("Search_ROI", Search_ROI);
            TeachRegions.Add("Character_ROI", Character_ROI);
            UpdateTeachRegionEvent?.Invoke();
        }
        public event Action UpdateTeachRegionEvent;

        public event Action<bool, string, Shape> UpdateSingleTeachRegionEvent;

        public override string ToString()
        {
            return "";
        }
    }
    public class OCRShapeMatchTool : BaseTool
    {
        private OCRShapeMatchConfig config;
        public OCRShapeMatchConfig Config
        {
            get => config;
            set
            {
                if (config != null)
                    config.PropertyChanged -= Config_PropertyChanged;
                ReadyToInspect = false;
                config = value;
                if (config != null)
                    config.PropertyChanged += Config_PropertyChanged;
            }
        }
        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReadyToInspect = false;
        }
        public OCRShapeMatchTool(OCRShapeMatchConfig config)
        {
            this.Config = config;
            config.PropertyChanged += Config_PropertyChanged;
        }
        public bool Setup(out InspectionData inspectionData)
        {
            InspectionData SetupResult = new InspectionData();
            Mat ReadTrainingImage = new Mat(config.OCRTrainedImage);
            Mat TrainImage = new Mat();
            Mat BrightFieldRef = new Mat(config.OCRBrightFieldRefImage);
            Mat DarkFieldRef = new Mat(config.OCRDarkFieldRefImage);
            
            if (ReadTrainingImage.Channels()> 1)
            {
                Cv2.CvtColor(ReadTrainingImage, TrainImage, ColorConversionCodes.BGRA2GRAY);
            }
            else
            {
                ReadTrainingImage.CopyTo(TrainImage);
            }

            Rect ROI_Char = Rect.FromLTRB(10, 2, 430, 129);
            Rect ROI_Number = Rect.FromLTRB(7, 125, 330, 190);

            Mat Display_TrainImage = TrainImage.Clone();
            

            Cv2.Rectangle(Display_TrainImage, ROI_Char, new Scalar(0, 0, 255, 255), 2);
            Cv2.Rectangle(Display_TrainImage, ROI_Number, new Scalar(0, 0, 255, 255), 2);

            Mat CharDomain = new Mat(TrainImage, ROI_Char);
            Mat NumberDomain = new Mat(TrainImage, ROI_Number);

            CharDomain.Threshold(0, 60, ThresholdTypes.Binary);
          

            Mat Closing = new Mat();
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));
            Cv2.MorphologyEx(CharDomain, CharDomain, MorphTypes.Open, kernel);
            Mat Canny = new Mat();
            Cv2.Canny(CharDomain, Canny, 0, 100);

            NumberDomain.Threshold(0, 60, ThresholdTypes.Binary);
            
            SetupResult.ResultOutput.Add(new MatInfo(Display_TrainImage, "", "Sample"));
            SetupResult.ResultOutput.Add(new MatInfo(BrightFieldRef, "", "Bright"));
            SetupResult.ResultOutput.Add(new MatInfo(DarkFieldRef, "", "Dark"));
            SetupResult.ResultOutput.Add(new MatInfo(CharDomain, "", "Char Domain"));
            SetupResult.ResultOutput.Add(new MatInfo(Canny, "", "Canny"));

            Mat Resized = new Mat();
            Cv2.Resize(CharDomain, Resized, OpenCvSharp.Size.Zero, 2, 2);
            SetupResult.ResultOutput.Add(new MatInfo(Resized, "", "Resized"));
            SetupResult.ResultOutput.Add(new MatInfo(NumberDomain, "", "Number Domain"));
       
            //Mat Display_CharDomain = CharDomain.Clone();
            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(Canny, out contours, out hierarchyIndexes, mode: RetrievalModes.External, 
                method: ContourApproximationModes.ApproxSimple);

            var orderedContours = contours.OrderBy(c => Cv2.BoundingRect(c).X).ToArray();
            for (int i = 0; i < orderedContours.Length; i++)
            {
                if (orderedContours[i].Length > 7)
                {
                    var Display_CharDomain = CharDomain.Clone();
                    //Cv2.DrawContours(Display_CharDomain, contours, i, new Scalar(0, 0, 255, 255));
                    var biggestContourRect = Cv2.BoundingRect(orderedContours[i]);
                    Cv2.Rectangle(Display_CharDomain, biggestContourRect, new Scalar(0, 0, 255, 255), 2);
                    SetupResult.ResultOutput.Add(new MatInfo(Display_CharDomain, "", "Contour " + i.ToString()));
                }

            }

            
            inspectionData = SetupResult;
            return true;     
        }
    }
}
