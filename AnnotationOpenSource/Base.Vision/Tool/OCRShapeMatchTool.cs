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
        public string CompareCharResults(Point[] contour)
        {
            int result = 0;
            double check = 1000000;
            double check2 = 1000000;
            double check3 = 100000;
            for (int i=0;i<CharContours.Count();i++)
            {
                double matching = Cv2.MatchShapes(CharContours[i], contour, ShapeMatchModes.I3);

                var area = Cv2.ContourArea(contour);
                var perimeter = Cv2.ArcLength(contour, true);

                var area_result = ContoursArea[i] - area;
                //var area_perimter = Contourslength[i] - perimeter;
                //var area_perimter = 10.0;
                if (matching < check3)
                {
                    //result = i;
                    check3 = matching;
                    if (area_result > 0)
                    {
                        if ((area_result < check))
                        {
                            result = i;
                            check = area_result;
   
                        }
                    }
                    /* if (area_result > 0 && area_perimter > 0)
                     {
                         if ((area_result < check) && (area_perimter < check2))
                         {
                             result = i;
                             check = area_result;
                             check2 = area_perimter;
                         }
                     }*/
                }
            }
            return (string)OcrAvailAlphabet[result].ToString();

        }
        public string CompareNumResults(Point[] contour)
        {
            int result = 0;
            double check = 1000;
            for (int i = 0; i < NumContours.Count(); i++)
            {
                double matching = Cv2.MatchShapes(NumContours[i], contour, ShapeMatchModes.I3);
                if (matching < check)
                {
                    result = i;
                    check = matching;
                }
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
          

            Mat Canny = new Mat();
            Cv2.MorphologyEx(CharDomain, CharDomain, MorphTypes.Open, kernel);
            Cv2.MorphologyEx(CharDomain, CharDomain, MorphTypes.Open, kernel);
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
            Mat Morphed_Char = Cropped_Char.MorphologyEx(MorphTypes.Open, kernel);
            SetupResult.ResultOutput.Add(new MatInfo(Morphed_Char, "", "Morphed_Char"));
            Canny = new Mat();
            Cv2.Canny(Morphed_Char, Canny, 0, 60);


          //  SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));
            SetupResult.ResultOutput.Add(new MatInfo(Canny, "", "Canny Check"));

            Point[][] contours2;
            HierarchyIndex[] hierarchyIndexes3;
            Cv2.FindContours(Canny, out contours2, out hierarchyIndexes3, mode: RetrievalModes.External,
                method: ContourApproximationModes.ApproxSimple);
            var orderedContours2 = contours2.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();
            string CharFormat = new string(config.OcrFormat.ToArray());


            Point[][] hull = new Point[5][];
            List<int> ResultHull = new List<int>();
            List<double> ResultMoment = new List<double>();
            int foundFormat = 0;
            string OutputChar = "";
            int hullmomentCount = 0;
            for (int i = 0; i < orderedContours2.Length; i++)
            {
                var biggestContourRect = Cv2.BoundingRect(orderedContours2[i]);
                if (biggestContourRect.Height > 15 && biggestContourRect.Width > 15)
                {
                    Mat DisplayImage = Cropped_Char.Clone();
                  
                    var contour = orderedContours2[i];

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

                    Cv2.Rectangle(DisplayImage, biggestContourRect, new Scalar(0, 0, 255, 255), 1);
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



                    SetupResult.ResultOutput.Add(new MatInfo(resizedImage, "", "ROI_2 " + i.ToString()));

                    // for (int j = 0; j < CharacterContours.Length; j++)
                    //{
                    if (hullmomentCount < 5)
                    {
                        hull[hullmomentCount] = Cv2.ConvexHull(orderedContours2[i], false);
                        HullValue.Add(hull[hullmomentCount].Count());
                        hullmomentCount++;
                    }
                    //}

                    //  for (int j = 0; j < hull.Length; j++)
                    //  {
                    // Cv2.DrawContours(Tester, orderedContours2, (int)i, new Scalar(0, 0, 255));
                    //  Cv2.DrawContours(Tester, hull, (int)i, new Scalar(0, 255, 0));
                    // SetupResult.ResultOutput.Add(new MatInfo(Tester, "", "Hull " + i.ToString()));
                    // }

                    /* Point[][] CharacterContours;
                     HierarchyIndex[] hierarchyIndices;

                     Cv2.FindContours(resizedImage, out CharacterContours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                     Mat whiteimage2 = new Mat(resizedImage.Size(), MatType.CV_8UC3, new Scalar(255, 255, 255, 255));

                     Mat drawing = Mat.Zeros(Cropped_Char.Size(), MatType.CV_8UC3);

                     Cv2.DrawContours(drawing, CharacterContours, -1, new Scalar(0, 255, 0));

                     SetupResult.ResultOutput.Add(new MatInfo(drawing, "", "Drawing " + i.ToString()));*/


                    //   SetupResult.ResultOutput.Add(new MatInfo(Morphed_Char_2, "", "Moprh_2 " + i.ToString()));
                    //   SetupResult.ResultOutput.Add(new MatInfo(Canny2, "", "Canny " + i.ToString()));
                    //   SetupResult.ResultOutput.Add(new MatInfo(thres, "", "Thres " + i.ToString()));
                    //    Point[][] CharacterContours;
                    //    HierarchyIndex[] hierarchyIndices;

                    //    Cv2.FindContours(Canny2, out CharacterContours, out hierarchyIndices, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                    //   Mat whiteimage2 = Mat.Zeros(Canny2.Size(), MatType.CV_8UC3);
                    // Mat whiteimage2 = new Mat(Canny2.Size(), MatType.CV_8UC3, new Scalar(0, 0, 0, 255));
                    //   Cv2.DrawContours(drawing, CharacterContours, -1, new Scalar(0, 0, 255));
                    // SetupResult.ResultOutput.Add(new MatInfo(drawing, "", "Contour " + i.ToString()));
                    /*  if (CharacterContours.Length != 1)
                      {
                          //IComparer myComparer = new PointArrayComparer();
                         // Array.Sort(CharacterContours, myComparer);


                          Cv2.DrawContours(whiteimage2, CharacterContours, 0, new Scalar(0, 0, 255, 255), 1);
                        //  CharContours[char_checker] = CharacterContours[0];
                          SetupResult.ResultOutput.Add(new MatInfo(whiteimage2, "", "Found Contour " + i.ToString()));
                      }
                     else
                     {
                         Cv2.DrawContours(whiteimage2, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);

                         SetupResult.ResultOutput.Add(new MatInfo(whiteimage2, "", "Contour " + i.ToString()));

                         //
                     }*/
                    //   Cv2.Rectangle(Display_CharDomain, biggestContourRect, new Scalar(0, 0, 255, 255), 1);
                    //  SetupResult.ResultOutput.Add(new MatInfo(Display_CharDomain, "", "Real Contour " + i.ToString()));

                    /*   if (foundFormat < CharFormat.Count())
                       {

                           if (CharFormat[foundFormat] == 'A')
                           {
                               OutputString = OutputString + CompareCharMoment(orderedContours2[i]);
                               //OutputString = OutputString + CompareCharResults(orderedContours2[i]);
                               // var results = CompareCharResults(orderedContours2[i]);
                           }
                           else
                           {
                               OutputString = OutputString + CompareNumMoment(orderedContours2[i]);
                               //OutputString = OutputString + CompareNumResults(orderedContours2[i]);
                               // var resilts = CompareNumResults(orderedContours2[i]);
                           }
                           foundFormat++;
                       }*/
                    //OutputString = OutputString + OutputChar;
                }
            }
            Mat Tester = Cropped_Char.Clone();
            for (int j = 0; j < 5; j++)
            {
                Cv2.DrawContours(Tester, orderedContours2, (int)j, new Scalar(0, 0, 255));
                Cv2.DrawContours(Tester, hull, (int)j, new Scalar(0, 255, 0));
                SetupResult.ResultOutput.Add(new MatInfo(Tester, "", "Hull " + j.ToString()));
            }

            SetupResult.ResultTuple = OutputString;

            inspectionData = SetupResult;
            return true;     
        }

        public List<double> ContoursArea = new List<double>();
        public List<double> Contourslength = new List<double>();
        public List<double> CharDistance = new List<double>();
        public List<int> HullValue = new List<int>();
        public List<double> MomentsValue = new List<double>();
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
                    NumContours[num_checker] = orderedContours[i];
             
                    var Display_NumDomain = image.Clone();
                    //var biggestContourRect = Cv2.BoundingRect(orderedContours[i]);

                    Mat CroppedChar = new Mat(image, biggestContourRect);
                    Cv2.Resize(CroppedChar, CroppedChar, ResizeFormat, 0, 0, InterpolationFlags.Area);

                    Point[][] CharacterContours;
                    HierarchyIndex[] hierarchyIndices;
                    Cv2.FindContours(CroppedChar, out CharacterContours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    double[] huMoments = new double[7];
                    if (CharacterContours.Length != 1)
                    {
                        for (int j = 0; j < CharacterContours.Length; j++)
                        {
                            if (CharacterContours[j].Length > length)
                            {
                                Cv2.DrawContours(CroppedChar, CharacterContours, j, new Scalar(255, 0, 0, 255), 1);
                                NumContours[num_checker] = CharacterContours[j];
                                huMoments = Cv2.Moments(CharacterContours[j]).HuMoments();
                            }

                        }
                    }
                    else
                    {
                        Cv2.DrawContours(CroppedChar, CharacterContours, 0, new Scalar(255, 0, 0, 255), 1);
                        NumContours[num_checker] = CharacterContours[0];
                        huMoments = Cv2.Moments(CharacterContours[0]).HuMoments();
                    }

                    SetupResult.ResultOutput.Add(new MatInfo(CroppedChar, "", "Contour " + i.ToString()));

                    // Get HuMoments

                    for (int j = 0; j < 7; j++)
                    {
                        huMoments[j] = -1 * Math.Sign(huMoments[j]) * Math.Log10(Math.Abs(huMoments[j]));
                    }

                    NumMoments[num_checker] = huMoments;
                    num_checker++;

                }
            }
        }
    }
}
