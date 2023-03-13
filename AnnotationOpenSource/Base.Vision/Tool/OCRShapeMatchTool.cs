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
using System.Drawing.Text;
using System.Windows.Media.Media3D;
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Media;
using OpenCvSharp.ML;
//using Tensorflow;
using Shape = Base.Vision.Shapes.Base.Shape;

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
        public string OcrFormat { get; set; } = "AAANNNNNNNNAN";
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
        public Point[][] CharContours = new Point[26][];
        public Point[][] NumContours = new Point[10][];
        public double[][] CharMoments = new double[26][];
        public double[][] NumMoments = new double[10][];
        public int char_checker = 0;
        public int num_checker = 0;
        public string OcrAvailAlphabet = new string(("ABCDEFGHIJKLMNOPQRSTUVWXYZ").ToArray());
        public string OcrAvailNumber = new string(("1234567890").ToArray());
        public string OutputString = "";
        public OpenCvSharp.Size ResizeFormat = new OpenCvSharp.Size(150, 150);
        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReadyToInspect = false;
        }
        public OCRShapeMatchTool(OCRShapeMatchConfig config)
        {
            this.Config = config;
            config.PropertyChanged += Config_PropertyChanged;
        }
        public string CharacterConverter(int i)
        {
            return (string)OcrAvailAlphabet[i].ToString();
        }
        public string CompareCharMoment(Point[] contour)
        {
            var huMoments = Cv2.Moments(contour).HuMoments();

            for (int j = 0; j < 7; j++)
            {
                huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
            }

            int result = 0;
            double check = 10000000;
            double sum = 0;
            //double sumMoment = huMoments.Sum();
            for (int i = 0; i < CharMoments.Count(); i++)
            {
                sum = 0;
                for (int j = 0; j < 6; j++)
                {
                    sum += Math.Pow((CharMoments[i][j] - huMoments[j]), 2);
                }
                sum = Math.Sqrt(sum);
                if (sum < check)
                {
                    result = i;
                    check = sum;
                }
            }
            return (string)OcrAvailAlphabet[result].ToString();

        }
        public string CompareNumMoment(Point[] contour)
        {
            var huMoments = Cv2.Moments(contour).HuMoments();
            for (int j = 0; j < 7; j++)
            {
                huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
            }

            int result = 0;
            double check = 10000000;
            double sum = 0;
            List<double> EachSum = new List<double>();
            //double sumMoment = huMoments.Sum();
            for (int i = 0; i < NumMoments.Count(); i++)
            {
                sum = 0;
                for (int j = 0; j < 7; j++)
                {
                    sum += Math.Pow((NumMoments[i][j] - huMoments[j]), 2);
                }
                sum = Math.Sqrt(sum);
                EachSum.Add(sum);
                if (sum < check)
                {
                    result = i;
                    check = sum;
                }
            }

            return (string)OcrAvailNumber[result].ToString();

        }
        public string CompareCharResults(int hull, double moments)
        {
            int result = 0;
            if (hull == 7)
            {
                // E, H, I, L, M, N, U, X, Z
                List<double> AvaibleList = new List<double> { MomentsValue[4], MomentsValue[7], MomentsValue[8], MomentsValue[11], MomentsValue[12], MomentsValue[13],
                    MomentsValue[20], MomentsValue[23], MomentsValue[25] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 4;
                else if (index == 1) result = 7;
                else if (index == 2) result = 8;
                else if (index == 3) result = 11;
                else if (index == 4) result = 12;
                else if (index == 5) result = 13;
                else if (index == 6) result = 20;
                else if (index == 7) result = 23;
                else if (index == 8) result = 25;
            }
            if (hull == 8)
            {
                // E, F, H, I, L, M, N, T, U, V, X, Y, Z
                List<double> AvaibleList = new List<double> { MomentsValue[4], MomentsValue[5], MomentsValue[7], MomentsValue[8], MomentsValue[11], MomentsValue[12], MomentsValue[13], 
                    MomentsValue[19], MomentsValue[20], MomentsValue[21], MomentsValue[23], MomentsValue[24], MomentsValue[25] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 4;
                else if (index == 1) result = 5;
                else if (index == 2) result = 7;
                else if (index == 3) result = 8;
                else if (index == 4) result = 11;
                else if (index == 5) result = 12;
                else if (index == 6) result = 13;
                else if (index == 7) result = 19;
                else if (index == 8) result = 20;
                else if (index == 9) result = 21;
                else if (index == 10) result = 23;
                else if (index == 11) result = 24;
                else if (index == 12) result = 25;
            }
            if (hull == 9)
            {
                // F, P, T, V, Y
                List<double> AvaibleList = new List<double> { MomentsValue[5], MomentsValue[15], MomentsValue[19], MomentsValue[21], MomentsValue[24]};

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 5;
                else if (index == 1) result = 15;
                else if (index == 2) result = 19;
                else if (index == 3) result = 21;
                else if (index == 4) result = 24;
            }
            if (hull == 10)
            {
                // C, F, K, P, T, V, Y,
                List<double> AvaibleList = new List<double> { MomentsValue[2], MomentsValue[5], MomentsValue[10], MomentsValue[15], MomentsValue[19], MomentsValue[21], MomentsValue[24] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 2;
                else if (index == 1) result = 5;
                else if (index == 2) result = 10;
                else if (index == 3) result = 15;
                else if (index == 4) result = 19;
                else if (index == 5) result = 21;
                else if (index == 6) result = 24;
            }

            if (hull == 11)
            {
                // A, B, C, K, P, R
                List<double> AvaibleList = new List<double> { MomentsValue[0], MomentsValue[1], MomentsValue[2], MomentsValue[10], MomentsValue[15], MomentsValue[17]};

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 0;
                else if (index == 1) result = 1;
                else if (index == 2) result = 2;
                else if (index == 3) result = 10;
                else if (index == 4) result = 15;
                else if (index == 5) result = 17;
            }
            if (hull == 12)
            {
                // A, B, C, D, J, K, R, W
                List<double> AvaibleList = new List<double> { MomentsValue[0], MomentsValue[1], MomentsValue[2], MomentsValue[3], MomentsValue[9], MomentsValue[10], MomentsValue[17], MomentsValue[22] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 0;
                else if (index == 1) result = 1;
                else if (index == 2) result = 2;
                else if (index == 3) result = 3;
                else if (index == 4) result = 9;
                else if (index == 5) result = 10;
                else if (index == 6) result = 17;
                else if (index == 7) result = 22;
            }
            if (hull == 13)
            {
                // A, B, D, J, R, W
                List<double> AvaibleList = new List<double> { MomentsValue[0], MomentsValue[1], MomentsValue[3], MomentsValue[9], MomentsValue[17], MomentsValue[22] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 0;
                else if (index == 1) result = 1;
                else if (index == 2) result = 3;
                else if (index == 3) result = 9;
                else if (index == 4) result = 17;
                else if (index == 5) result = 22;
            }
            if (hull == 14)
            {
                // D, G, J, S, W
                List<double> AvaibleList = new List<double> { MomentsValue[3], MomentsValue[6], MomentsValue[9], MomentsValue[18], MomentsValue[22] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 3;
                else if (index == 1) result = 6;
                else if (index == 2) result = 9;
                else if (index == 3) result = 18;
                else if (index == 4) result = 22;
            }
            if (hull >= 15)
            {
                // G, O, Q, S
                List<double> AvaibleList = new List<double> { MomentsValue[6], MomentsValue[14], MomentsValue[16], MomentsValue[18] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 3;
                else if (index == 1) result = 6;
                else if (index == 2) result = 9;
                else if (index == 3) result = 18;
                else if (index == 4) result = 22;
            }

            return (string)OcrAvailAlphabet[result].ToString();

        }
        public string CompareNumResults(int hull, double moments)
        {
            int result = 0;
            if (hull <= 7)
            {
                // 1
                List<double> AvaibleList = new List<double> { NumMomentsValue[0] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 0;

            }
            if (hull == 8)
            {
                // 1, 4, 7, 8
                List<double> AvaibleList = new List<double> { NumMomentsValue[0], NumMomentsValue[3], NumMomentsValue[6], NumMomentsValue[7] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 0;
                else if (index == 1) result = 3;
                else if (index == 2) result = 6;
                else if (index == 3) result = 7;
            }
            if (hull == 9)
            {
                // 1, 4, 6, 7, 8
                List<double> AvaibleList = new List<double> { NumMomentsValue[0], NumMomentsValue[3], NumMomentsValue[5], NumMomentsValue[6], NumMomentsValue[7] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 0;
                else if (index == 1) result = 3;
                else if (index == 2) result = 5;
                else if (index == 3) result = 6;
                else if (index == 4) result = 7;
            }
            if (hull == 10)
            {
                // 3, 4, 5, 6, 7, 8, 9
                List<double> AvaibleList = new List<double> { NumMomentsValue[2], NumMomentsValue[3], NumMomentsValue[4], NumMomentsValue[5], NumMomentsValue[6], NumMomentsValue[7], NumMomentsValue[8] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 2;
                else if (index == 1) result = 3;
                else if (index == 2) result = 4;
                else if (index == 3) result = 5;
                else if (index == 4) result = 6;
                else if (index == 5) result = 7;
                else if (index == 6) result = 8;
            }
            if (hull == 11 || hull == 12)
            {
                // 3, 5, 6, 9
                List<double> AvaibleList = new List<double> { NumMomentsValue[2], NumMomentsValue[4], NumMomentsValue[5], NumMomentsValue[8]};

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 2;
                else if (index == 1) result = 4;
                else if (index == 2) result = 5;
                else if (index == 3) result = 8;
            }
            if (hull >= 13 )
            {
                // 2, 0
                List<double> AvaibleList = new List<double> { NumMomentsValue[1], NumMomentsValue[9] };

                double closest = AvaibleList.Aggregate((x, y) => Math.Abs(x - moments) < Math.Abs(y - moments) ? x : y);

                int index = AvaibleList.IndexOf(closest);
                if (index == 0) result = 1;
                else if (index == 1) result = 9;

            }
            return (string)OcrAvailNumber[result].ToString();

        }
        public bool Setup(out InspectionData inspectionData)
        {
            InspectionData SetupResult = new InspectionData();

            double RefCharHeight = 0;
            double RefCharWidth = 0;
            double RealCharHeight = 0;
            double RealCharWidth = 0;
            char_checker = 0;
            num_checker = 0;
            OutputString = "";
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

            // Get the First Alphabet from Trained Image
            Rect FirstAlpahbetRect = Rect.FromLTRB(9, 16, 37, 63);
            Mat Display_TrainImage = TrainImage.Clone();
            Cv2.Rectangle(Display_TrainImage, FirstAlpahbetRect, new Scalar(0, 0, 255, 255), 2);
            SetupResult.ResultOutput.Add(new MatInfo(Display_TrainImage, "", "Sample"));

            // Crop First Alphabet
            Mat FirstAlphabetMat = new Mat(TrainImage, FirstAlpahbetRect);
            SetupResult.ResultOutput.Add(new MatInfo(FirstAlphabetMat, "", "FirstAlphabetMat"));

            // Edge Detection and Find Contours
           /* Mat Canny = new Mat();
            Cv2.Canny(FirstAlphabetMat, Canny, 0, 100);

            Point[][] FirstAlphabetContours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(Canny, out FirstAlphabetContours, out hierarchyIndexes, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < FirstAlphabetContours.Length; i++)
            {
                var Display_CharDomain = FirstAlphabetMat.Clone();
                var biggestContourRect = Cv2.BoundingRect(FirstAlphabetContours[i]);
                Cv2.Rectangle(Display_CharDomain, biggestContourRect, new Scalar(0, 0, 255, 255), 1);
                SetupResult.ResultOutput.Add(new MatInfo(Display_CharDomain, "", "First Contour " + i.ToString()));
                RefCharHeight = biggestContourRect.Height;
                RefCharWidth = biggestContourRect.Width;
            }
           
            Mat Display_BrightRefImage = BrightFieldRef.Clone();

            // Get one Character from Real Prod Image
            Rect RealCharROI = Rect.FromLTRB(668, 378, 715, 459);
            Cv2.Rectangle(Display_BrightRefImage, RealCharROI, new Scalar(0, 255, 0, 255), 2);
            RealCharHeight = RealCharROI.Height;
            RealCharWidth = RealCharROI.Width;
            SetupResult.ResultOutput.Add(new MatInfo(Display_BrightRefImage, "", "Display_BrightRefImage"));

            // Find Scale
            double ScaleHeight = RealCharHeight / RefCharHeight;
            double ScaleWidth = RealCharWidth / RefCharWidth;
            double scale_percent = 0;
            if (ScaleHeight > ScaleWidth)
            {
                scale_percent = ScaleHeight;
            }
            else
                scale_percent = ScaleWidth;

            OpenCvSharp.Size NewImageSize = new OpenCvSharp.Size((TrainImage.Width * scale_percent), (TrainImage.Height * scale_percent));
           */
            // Get the ROI for Character and Numbers
            Rect ROI_Char = Rect.FromLTRB(10, 2, 430, 68);
            Rect ROI_Char_2 = Rect.FromLTRB(10, 68, 430, 129);
            Rect ROI_Number = Rect.FromLTRB(7, 125, 330, 190);

            Cv2.Rectangle(Display_TrainImage, ROI_Char, new Scalar(0, 0, 255, 255), 2);
            Cv2.Rectangle(Display_TrainImage, ROI_Char_2, new Scalar(0, 0, 255, 255), 2);
            Cv2.Rectangle(Display_TrainImage, ROI_Number, new Scalar(0, 0, 255, 255), 2);

            Mat CharDomain = new Mat(TrainImage, ROI_Char);
            Mat CharDomain2 = new Mat(TrainImage, ROI_Char_2);
            Mat NumberDomain = new Mat(TrainImage, ROI_Number);

            SetupResult.ResultOutput.Add(new MatInfo(CharDomain, "", "Char Domain"));
            SetupResult.ResultOutput.Add(new MatInfo(CharDomain2, "", "Char Domain 2"));
            SetupResult.ResultOutput.Add(new MatInfo(NumberDomain, "", "Number Domain"));

            //Mat ResizedCharDomain = new Mat();
            // Cv2.Resize(CharDomain, ResizedCharDomain, OpenCvSharp.Size.Zero, scale_percent, scale_percent);
            // SetupResult.ResultOutput.Add(new MatInfo(ResizedCharDomain, "", "ResizedChar Domain"));
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(3, 3));

            HullValue.Clear();
            NumHullValue.Clear();
            NumMomentsValue.Clear();
            MomentsValue.Clear();
            Mat Canny = new Mat();
            Cv2.MorphologyEx(CharDomain, CharDomain, MorphTypes.Open, kernel);
            //Cv2.MorphologyEx(CharDomain, CharDomain, MorphTypes.Open, kernel);
            Cv2.Canny(CharDomain, Canny, 0, 100);

            FindCharContours(CharDomain, Canny, SetupResult,10);

           // Mat ResizedCharDomain2 = new Mat();
           // Cv2.Resize(CharDomain2, ResizedCharDomain2, OpenCvSharp.Size.Zero, scale_percent, scale_percent);
           // SetupResult.ResultOutput.Add(new MatInfo(ResizedCharDomain2, "", "ResizedChar Domain 2"));

            Canny = new Mat();
            Cv2.MorphologyEx(CharDomain2, CharDomain2, MorphTypes.Open, kernel);
            Cv2.Canny(CharDomain2, Canny, 0, 100);
            FindCharContours(CharDomain2, Canny, SetupResult, 7);

         //   Mat ResizedNumberDomain = new Mat();
         //   Cv2.Resize(NumberDomain, ResizedNumberDomain, OpenCvSharp.Size.Zero, scale_percent, scale_percent);
            Canny = new Mat();
            Cv2.MorphologyEx(NumberDomain, NumberDomain, MorphTypes.Open, kernel);
            Cv2.Canny(NumberDomain, Canny, 0, 100);
            FindNumContours(NumberDomain, Canny, SetupResult, 10);


            // Convert to Grayscale



            if (BrightFieldRef.Channels() > 1)
            {
                Cv2.CvtColor(BrightFieldRef, BrightFieldRef, ColorConversionCodes.BGRA2GRAY);
            }



            Mat Display_BrightRefImage = BrightFieldRef.Clone();

            Rect RealSearchROI = Rect.FromLTRB(492, 353, 1604, 494);
            Cv2.Rectangle(Display_BrightRefImage, RealSearchROI, new Scalar(0, 255, 0, 255), 2);
            Mat Cropped_Char = new Mat(BrightFieldRef, RealSearchROI);


            Cropped_Char.Threshold(0, 100, ThresholdTypes.Binary);
            SetupResult.ResultOutput.Add(new MatInfo(Cropped_Char, "", "Cropped_Char"));
            kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
            Mat Morphed_Char = Cropped_Char.MorphologyEx(MorphTypes.Open, kernel, null, 2);
            SetupResult.ResultOutput.Add(new MatInfo(Morphed_Char, "", "Morphed_Char"));
            Canny = new Mat();
            Cv2.Canny(Morphed_Char, Canny, 0, 50);


          //  SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));
            SetupResult.ResultOutput.Add(new MatInfo(Canny, "", "Canny Check"));

            Point[][] contours2;
            HierarchyIndex[] hierarchyIndexes3;
            Cv2.FindContours(Canny, out contours2, out hierarchyIndexes3, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);
            var orderedContours2 = contours2.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();
            string CharFormat = new string(config.OcrFormat.ToArray());

            FoundHullValue.Clear();
            Point[][] hull = new Point[13][];
            List<int> ResultHull = new List<int>();
            List<double> ResultMoment = new List<double>();
            int foundFormat = 0;
            string OutputChar = "";
            int hullmomentCount = 0;
            List<double> TestMomentsValue = new List<double>();
            for (int i = 0; i < orderedContours2.Length; i++)
            {
                var biggestContourRect = Cv2.BoundingRect(orderedContours2[i]);
                if (biggestContourRect.Height > 30 && biggestContourRect.Width > 15)
                {
                    Mat DisplayImage = Cropped_Char.Clone();
                  
                    var contour = orderedContours2[i];

                    int offset = 0;

                    if ((biggestContourRect.X - 2) > -1)
                        biggestContourRect.X = biggestContourRect.X - offset;
                    else
                        biggestContourRect.X = 0;

                    if ((biggestContourRect.Y - 2) > -1)
                        biggestContourRect.Y = biggestContourRect.Y - offset;
                    else
                        biggestContourRect.Y = 0;

                    biggestContourRect.Width = biggestContourRect.Width + (offset * 2);
                    biggestContourRect.Height = biggestContourRect.Height + (offset * 2);

                    Cv2.Rectangle(DisplayImage, biggestContourRect, new Scalar(255, 0, 255, 255), 2);
                    SetupResult.ResultOutput.Add(new MatInfo(DisplayImage, "", "Real Contour " + i.ToString()));

                    var roi = new Mat(Cropped_Char, biggestContourRect);

                    var resizedImage = new Mat();

                    Cv2.Resize(roi, resizedImage, ResizeFormat); //resize to 10X10

                    //     kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
                    // Mat Morphed_Char_2 = thres.MorphologyEx(MorphTypes.Open, kernel);
                    //     Mat Morphed_Char_2 = resizedImage.MorphologyEx(MorphTypes.Open, kernel,null,3);

                    //     Mat Canny2 = new Mat();
                    //     Cv2.Canny(Morphed_Char_2, Canny2, 0, 100);

                    //     Mat thres = new Mat();
                    //     Cv2.Threshold(Canny2, thres, 0, 100, ThresholdTypes.Binary);


                    double[] huMoments = new double[7];
                    SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));

                    // for (int j = 0; j < CharacterContours.Length; j++)
                    //{
                    if (hullmomentCount < 13)
                    {
                        hull[hullmomentCount] = Cv2.ConvexHull(orderedContours2[i], false);
                        FoundHullValue.Add(hull[hullmomentCount].Count());
                        hullmomentCount++;
                    }
                    huMoments = Cv2.Moments(orderedContours2[i]).HuMoments();
                    double sum = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
                    }
                    for (int j = 0; j < 4; j++)
                    {

                        sum += huMoments[j];
                    }
                    TestMomentsValue.Add(sum);

                    if (foundFormat < CharFormat.Count())
                    {

                        if (CharFormat[foundFormat] == 'A')
                        {
                            OutputString = OutputString + CompareCharResults(FoundHullValue[foundFormat], sum);
                        }
                        else
                        {
                            OutputString = OutputString + CompareNumResults(FoundHullValue[foundFormat], sum);

                        }
                        foundFormat++;
                    }

                }
            }
            Mat Tester = Cropped_Char.Clone();
            for (int j = 0; j < 1; j++)
            {
                Cv2.DrawContours(Tester, orderedContours2, (int)j, new Scalar(0, 0, 255));
                Cv2.DrawContours(Tester, hull, (int)j, new Scalar(0, 255, 0));
                SetupResult.ResultOutput.Add(new MatInfo(Tester, "", "Hull " + j.ToString()));
            }

            SetupResult.ResultTuple = OutputString;



           // var testkeras = Keras.Models.Model.LoadModel(@"C:\Users\jason.yap\Desktop\ODOCR\saved_model");
          


            inspectionData = SetupResult;
            return true;     
        }
        public bool Run(Mat source, out InspectionData inspectionData)
        {
            InspectionData RunResult = new InspectionData();

            Mat image = source.Clone();

            if (image.Channels() > 1)
            {
                Cv2.CvtColor(image, image, ColorConversionCodes.BGRA2GRAY);
            }

            Mat Display_BrightRefImage = image.Clone();

            Rect RealSearchROI = Rect.FromLTRB(492, 353, 1604, 494);
            Cv2.Rectangle(Display_BrightRefImage, RealSearchROI, new Scalar(0, 255, 0, 255), 2);
            Mat Cropped_Char = new Mat(image, RealSearchROI);
          //  Mat zeros = Mat.Zeros(image.Size(), MatType.CV_8UC1);
          
         //   Point2f OriPoint = new Point2f(RealSearchROI.X, RealSearchROI.Y);
          //  Point2f DestinationPoint = new Point2f(0, 0);
         //   Cv2.GetAffineTransform(RealSearchROI, Cropped_Char);

            Cropped_Char.Threshold(0, 100, ThresholdTypes.Binary);
            RunResult.ResultOutput.Add(new MatInfo(Cropped_Char, "", "Cropped_Char"));
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
            Mat Morphed_Char = Cropped_Char.MorphologyEx(MorphTypes.Open, kernel, null, 3);
            RunResult.ResultOutput.Add(new MatInfo(Morphed_Char, "", "Morphed_Char"));
            Mat Canny = new Mat();
            Cv2.Canny(Morphed_Char, Canny, 10, 30);
            

            //  SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));
            RunResult.ResultOutput.Add(new MatInfo(Canny, "", "Canny Check"));

            // Process whole image
            /*     Display_BrightRefImage.Threshold(0, 100, ThresholdTypes.Binary);
                 Mat TestingMorphed_Char = Display_BrightRefImage.MorphologyEx(MorphTypes.Open, kernel, null, 3);
                 RunResult.ResultOutput.Add(new MatInfo(TestingMorphed_Char, "", "Testing Morphed_Char"));
                 Mat TestCanny = new Mat();
                 Cv2.Canny(TestingMorphed_Char, TestCanny, 10, 30);
                 RunResult.ResultOutput.Add(new MatInfo(TestCanny, "", "Whole Canny Check"));*/


            //


        




            Point[][] contours2;
            HierarchyIndex[] hierarchyIndexes3;
            Cv2.FindContours(Canny, out contours2, out hierarchyIndexes3, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);
            var orderedContours2 = contours2.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();
            string CharFormat = new string(config.OcrFormat.ToArray());

            FoundHullValue.Clear();
            Point[][] hull = new Point[13][];
            List<int> ResultHull = new List<int>();
            List<double> ResultMoment = new List<double>();
            int foundFormat = 0;
            string OutputChar = "";
            int hullmomentCount = 0;
            OutputString = "";
           // List<RectInfo> ObtainedRect = new List<RectInfo>();

            Mat DisplayImage = image.Clone();

            for (int i = 0; i < orderedContours2.Length; i++)
            {
                var biggestContourRect = Cv2.BoundingRect(orderedContours2[i]);
                if (biggestContourRect.Height > 40 && biggestContourRect.Width > 20)
                {
                    var contour = orderedContours2[i];

                    int offset = 5;
                    if ((biggestContourRect.X - 2) > -1)
                        biggestContourRect.X = biggestContourRect.X - offset;
                    else
                        biggestContourRect.X = 0;

                    if ((biggestContourRect.Y - 2) > -1)
                        biggestContourRect.Y = biggestContourRect.Y - offset;
                    else
                        biggestContourRect.Y = 0;
                    biggestContourRect.X = biggestContourRect.X + 492;
                    biggestContourRect.Y = biggestContourRect.Y + 353;
                    biggestContourRect.Width = biggestContourRect.Width + (offset * 2);
                    biggestContourRect.Height = biggestContourRect.Height + (offset * 2);
                   // ObtainedRect.Add(new RectInfo(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height));
                    Cv2.Rectangle(DisplayImage, biggestContourRect, new Scalar(0, 0, 255, 255), 3);
                    
               

                    double[] huMoments = new double[7];
                 //   RunResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));

                    // for (int j = 0; j < CharacterContours.Length; j++)
                    //{
                    if (hullmomentCount < 13)
                    {
                        hull[hullmomentCount] = Cv2.ConvexHull(orderedContours2[i], false);
                        FoundHullValue.Add(hull[hullmomentCount].Count());
                        hullmomentCount++;
                    }
                    huMoments = Cv2.Moments(orderedContours2[i]).HuMoments();
                    double sum = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
                    }
                    for (int j = 0; j < 4; j++)
                    {

                        sum += huMoments[j];
                    }
                    //TestMomentsValue.Add(sum);

                    if (foundFormat < CharFormat.Count())
                    {

                        if (CharFormat[foundFormat] == 'A')
                        {
                            var str = CompareCharResults(FoundHullValue[foundFormat], sum);
                            OutputString = OutputString + CompareCharResults(FoundHullValue[foundFormat], sum);
                            RunResult.ResultOutputRect.Add(new RectInfo(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height, str));
                        }
                        else
                        {
                            var str = CompareNumResults(FoundHullValue[foundFormat], sum);
                            OutputString = OutputString + CompareNumResults(FoundHullValue[foundFormat], sum);
                            RunResult.ResultOutputRect.Add(new RectInfo(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height, str));
                        }
                        foundFormat++;
                    }
                    else
                    {
                        RunResult.ResultOutputRect.Add(new RectInfo(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height, "A"));
                    }
                 
                }
            }
            RunResult.ResultOutput.Add(new MatInfo(DisplayImage, "", "Found"));
            //RunResult.ResultOutputRect = ObtainedRect;
            RunResult.ResultTuple = OutputString;

            inspectionData = RunResult;
            return true;
        }

        public List<double> ContoursArea = new List<double>();
        public List<double> Contourslength = new List<double>();
        public List<double> CharDistance = new List<double>();
        public List<int> HullValue = new List<int>();
        public List<int> NumHullValue = new List<int>();
        public List<int> FoundHullValue = new List<int>();
        
        public List<double> MomentsValue = new List<double>();
        public List<double> NumMomentsValue = new List<double>();
        public void FindCharContours2(Mat image, Mat Canny, InspectionData SetupResult, int length)
        {
            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(Canny, out contours, out hierarchyIndexes, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);
            var orderedContours = contours.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();

            for (int i = 0; i < orderedContours.Length; i++)
            {
                var biggestContourRect = Cv2.BoundingRect(orderedContours[i]);
                if (biggestContourRect.Height > 15 && biggestContourRect.Width > 15)
                {

                    var Display_CharDomain = image.Clone();
                    //   Cv2.Rectangle(Display_CharDomain, biggestContourRect, new Scalar(0, 0, 255, 255), 1);
                    //   SetupResult.ResultOutput.Add(new MatInfo(Display_CharDomain, "", "Contour " + i.ToString()));

                    Mat CroppedChar = new Mat(image, biggestContourRect);
                    Cv2.Resize(CroppedChar, CroppedChar, ResizeFormat, 0, 0, InterpolationFlags.Area);

                    Point[][] CharacterContours;
                    HierarchyIndex[] hierarchyIndices;

                    Cv2.FindContours(CroppedChar, out CharacterContours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    Mat whiteimage = new Mat(ResizeFormat, MatType.CV_8UC3, new Scalar(255, 255, 255, 255));

                    double[] huMoments = new double[10];
                    if (CharacterContours.Length != 1)
                    {
                        IComparer myComparer = new PointArrayComparer();
                        Array.Sort(CharacterContours, myComparer);


                        Cv2.DrawContours(whiteimage, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        // Cv2.DrawContours(CroppedChar, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        CharContours[char_checker] = CharacterContours[0];
                        SetupResult.ResultOutput.Add(new MatInfo(whiteimage, "", "Contour " + i.ToString()));
                        //SetupResult.ResultOutput.Add(new MatInfo(CroppedChar, "", "Contour " + i.ToString()));
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();
                        /*for (int j = 0; j < CharacterContours.Length; j++)
                         {
                           
                            //CharacterContours.OrderByDescending(x => x).First();
                            if (CharacterContours[j].Length > length)
                             {
                                 Cv2.DrawContours(CroppedChar, CharacterContours, j, new Scalar(255, 0, 0, 255), 1);
                                 CharContours[char_checker] = CharacterContours[j];
                                 SetupResult.ResultOutput.Add(new MatInfo(CroppedChar, "", "Contour " + i.ToString()));
                                 huMoments = Cv2.Moments(CharacterContours[j]).HuMoments();
                             }
                         }*/
                    }
                    else
                    {
                        Cv2.DrawContours(whiteimage, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        //Cv2.DrawContours(CroppedChar, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        CharContours[char_checker] = CharacterContours[0];
                        //SetupResult.ResultOutput.Add(new MatInfo(CroppedChar, "", "Contour " + i.ToString()));
                        SetupResult.ResultOutput.Add(new MatInfo(whiteimage, "", "Contour " + i.ToString()));
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();
                    }

                    //SetupResult.ResultOutput.Add(new MatInfo(CroppedChar, "", "Contour " + i.ToString()));

                    // Get HuMoments

                    for (int j = 0; j < 7; j++)
                    {
                        huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
                    }
                    CharMoments[char_checker] = huMoments;
                    char_checker++;
                    /* Cv2.Rectangle(Display_CharDomain, biggestContourRect, new Scalar(0, 0, 255, 255), 1);
                     SetupResult.ResultOutput.Add(new MatInfo(Display_CharDomain, "", "Contour " + i.ToString()));

                     ContoursArea.Add(Cv2.ContourArea(orderedContours[i]));
                     Contourslength.Add(Cv2.ArcLength(orderedContours[i], true));
                    

                     double h0 = 0.00162663;
                     double h1 = -1 * Math.Sign(h0) * Math.Log10(Math.Abs(h0));
                     CharMoments[char_checker] = huMoments;
                     //CharMoments[char_checker] = Cv2.Moments(orderedContours[i]).HuMoments();
                     char_checker++;*/
                    /* double sumCharDistance = 0;
                     for(int j = 1; j < orderedContours[i].Length; j++)
                     {

                         Point p1 = orderedContours[i][j-1];
                         Point p2 = orderedContours[i][j];
                         //  CharDistance = +GetDistance(p1.X, p1.Y, p2.X, p2.Y);
                         sumCharDistance = + Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y),2));

                     }
                     CharDistance.Add(sumCharDistance);*/

                }

            }
        }
        public void FindCharContours(Mat image, Mat Canny, InspectionData SetupResult, int length)
        {
            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(Canny, out contours, out hierarchyIndexes, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);
            var orderedContours = contours.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();

            for (int i = 0; i < orderedContours.Length; i++)
            {
                Rect biggestContourRect = Cv2.BoundingRect(orderedContours[i]);
                if (biggestContourRect.Height > 15 && biggestContourRect.Width > 15)
                {
                    Mat DisplayImage = image.Clone();

                    var contour = orderedContours[i];

                    int offset = 1;

                    if ((biggestContourRect.X - 2) > -1)
                        biggestContourRect.X = biggestContourRect.X - offset;
                    else
                        biggestContourRect.X = 0;

                    if ((biggestContourRect.Y - 2) > -1)
                        biggestContourRect.Y = biggestContourRect.Y - offset;
                    else
                        biggestContourRect.Y = 0;

                    biggestContourRect.Width = biggestContourRect.Width + (offset * 2);
                    biggestContourRect.Height = biggestContourRect.Height + (offset * 2);

                    var roi = new Mat(image, biggestContourRect);

                    var resizedImage = new Mat();

                    Cv2.Resize(roi, resizedImage, ResizeFormat); //resize to 10X10

                    Mat thres = new Mat();
                    Cv2.Threshold(resizedImage, thres, 0, 150, ThresholdTypes.Binary);
                    Mat Canny2 = new Mat();
                    Cv2.Canny(thres, Canny2, 0, 100);

                   //  SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));
                    //  SetupResult.ResultOutput.Add(new MatInfo(Canny2, "", "Canny " + i.ToString()));

                    Point[][] CharacterContours;
                    HierarchyIndex[] hierarchyIndices;

                    Cv2.FindContours(Canny2, out CharacterContours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    Mat whiteimage2 = new Mat(resizedImage.Size(), MatType.CV_8UC3, new Scalar(255, 255, 255, 255));

                    double[] huMoments = new double[7];
                    if (CharacterContours.Length != 1)
                    {
                        IComparer myComparer = new PointArrayComparer();
                        Array.Sort(CharacterContours, myComparer);


                        Cv2.DrawContours(whiteimage2, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        CharContours[char_checker] = CharacterContours[0];
                        SetupResult.ResultOutput.Add(new MatInfo(whiteimage2, "", "Contour " + i.ToString()));
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();

                        //
                        Point[][] hull = new Point[CharacterContours.Length][];

                        Mat drawing = Mat.Zeros(Canny2.Size(), MatType.CV_8UC3);

                        for (int j = 0; j < CharacterContours.Length; j++)
                        {
                            hull[j] = Cv2.ConvexHull(CharacterContours[j], false);
                            HullValue.Add(hull[j].Count());
                        }
                        for (int j = 0; j < hull.Length; j++)
                        {
                            Cv2.DrawContours(drawing, CharacterContours, (int)j, new Scalar(0, 0, 255));
                            Cv2.DrawContours(drawing, hull, (int)j, new Scalar(0, 255, 0));
                            //SetupResult.ResultOutput.Add(new MatInfo(drawing, "", "Hull " + i.ToString()));
                        }
                        //
                    }
                    else
                    {
                        Cv2.DrawContours(whiteimage2, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        CharContours[char_checker] = CharacterContours[0];
                        SetupResult.ResultOutput.Add(new MatInfo(whiteimage2, "", "Contour " + i.ToString()));
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();

                        //
                        Point[][] hull = new Point[CharacterContours.Length][];

                        Mat drawing = Mat.Zeros(Canny2.Size(), MatType.CV_8UC3);

                        for (int j = 0; j < CharacterContours.Length; j++)
                        {
                            hull[j] = Cv2.ConvexHull(CharacterContours[j], true);
                            HullValue.Add(hull[j].Count());
                        }
                        for (int j = 0; j < hull.Length; j++)
                        {
                            Cv2.DrawContours(drawing, CharacterContours, (int)j, new Scalar(0, 0, 255));
                            Cv2.DrawContours(drawing, hull, (int)j, new Scalar(0, 255, 0));
                          //  SetupResult.ResultOutput.Add(new MatInfo(drawing, "", "Hull " + i.ToString()));
                        }
                        //
                    }


                    // Get HuMoments
                    double sum = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
                    }
                    for (int j = 0; j < 4; j++)
                    {

                        sum += huMoments[j];
                    }
                    MomentsValue.Add(sum);
                    CharMoments[char_checker] = huMoments;
                    char_checker++;

                }

            }
        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        public void FindNumContours(Mat image, Mat Canny, InspectionData SetupResult, int length)
        {
            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(Canny, out contours, out hierarchyIndexes, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);
            var orderedContours = contours.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();

            for (int i = 0; i < orderedContours.Length; i++)
            {
                var biggestContourRect = Cv2.BoundingRect(orderedContours[i]);
                if (biggestContourRect.Height > 20 && biggestContourRect.Width > 20)
                {
                    //
                    Mat DisplayImage = image.Clone();

                    var contour = orderedContours[i];

                    int offset = 1;

                    if ((biggestContourRect.X - 2) > -1)
                        biggestContourRect.X = biggestContourRect.X - offset;
                    else
                        biggestContourRect.X = 0;

                    if ((biggestContourRect.Y - 2) > -1)
                        biggestContourRect.Y = biggestContourRect.Y - offset;
                    else
                        biggestContourRect.Y = 0;

                    biggestContourRect.Width = biggestContourRect.Width + (offset * 2);
                    biggestContourRect.Height = biggestContourRect.Height + (offset * 2);


                    biggestContourRect.Width = biggestContourRect.Width + (offset * 2);
                    biggestContourRect.Height = biggestContourRect.Height + (offset * 2);

                    var roi = new Mat(image, biggestContourRect);

                    var resizedImage = new Mat();

                    Cv2.Resize(roi, resizedImage, ResizeFormat); //resize to 10X10

                    Mat thres = new Mat();
                    Cv2.Threshold(resizedImage, thres, 0, 150, ThresholdTypes.Binary);
                    Mat Canny2 = new Mat();
                    Cv2.Canny(thres, Canny2, 0, 100);

                    //  SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));
                    //  SetupResult.ResultOutput.Add(new MatInfo(Canny2, "", "Canny " + i.ToString()));

                    Point[][] CharacterContours;
                    HierarchyIndex[] hierarchyIndices;

                    Cv2.FindContours(Canny2, out CharacterContours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    Mat whiteimage2 = new Mat(resizedImage.Size(), MatType.CV_8UC3, new Scalar(255, 255, 255, 255));

                    double[] huMoments = new double[7];
                    if (CharacterContours.Length != 1)
                    {
                        IComparer myComparer = new PointArrayComparer();
                        Array.Sort(CharacterContours, myComparer);

                        Cv2.DrawContours(whiteimage2, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        NumContours[num_checker] = CharacterContours[0];
                        SetupResult.ResultOutput.Add(new MatInfo(whiteimage2, "", "Num Contour " + i.ToString()));
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();

                        //
                        Point[][] hull = new Point[CharacterContours.Length][];

                        Mat drawing = Mat.Zeros(Canny2.Size(), MatType.CV_8UC3);

                        for (int j = 0; j < CharacterContours.Length; j++)
                        {
                            hull[j] = Cv2.ConvexHull(CharacterContours[j], false);
                            NumHullValue.Add(hull[j].Count());
                        }
                        for (int j = 0; j < hull.Length; j++)
                        {
                            Cv2.DrawContours(drawing, CharacterContours, (int)j, new Scalar(0, 0, 255));
                            Cv2.DrawContours(drawing, hull, (int)j, new Scalar(0, 255, 0));
                            //SetupResult.ResultOutput.Add(new MatInfo(drawing, "", "Hull " + i.ToString()));
                        }
                        //
                    }
                    else
                    {
                        Cv2.DrawContours(whiteimage2, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        NumContours[num_checker] = CharacterContours[0];
                        SetupResult.ResultOutput.Add(new MatInfo(whiteimage2, "", "Contour " + i.ToString()));
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();

                        //
                        Point[][] hull = new Point[CharacterContours.Length][];

                        Mat drawing = Mat.Zeros(Canny2.Size(), MatType.CV_8UC3);

                        for (int j = 0; j < CharacterContours.Length; j++)
                        {
                            hull[j] = Cv2.ConvexHull(CharacterContours[j], true);
                            NumHullValue.Add(hull[j].Count());
                        }
                        for (int j = 0; j < hull.Length; j++)
                        {
                            Cv2.DrawContours(drawing, CharacterContours, (int)j, new Scalar(0, 0, 255));
                            Cv2.DrawContours(drawing, hull, (int)j, new Scalar(0, 255, 0));
                            //  SetupResult.ResultOutput.Add(new MatInfo(drawing, "", "Hull " + i.ToString()));
                        }
                        //
                    }


                    // Get HuMoments
                    double sum = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
                    }
                    for (int j = 0; j < 4; j++)
                    {

                        sum += huMoments[j];
                    }
                    NumMomentsValue.Add(sum);
                    NumMoments[num_checker] = huMoments;
                    num_checker++;

                    //
               
                }
            }
        }
    }
}
