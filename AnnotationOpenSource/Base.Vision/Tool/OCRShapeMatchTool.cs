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
using Keras;
//using Tensorflow;
using Shape = Base.Vision.Shapes.Base.Shape;
using Keras.Datasets;
using Keras.Models;
using Numpy;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.IO.Ports;
using Base.Common;

namespace Base.Vision.Tool
{
    public enum ThresType
    {
        Binary,
        BinaryInv,
        AdaptiveGaussian,
        AdaptiveGaussianInv,
        AdaptiveMean,
        AdaptiveMeanInv
    }
    public enum PixelFormat
    {
        Mono,
        Color,
    }
    public enum RectColor
    {
        White,
        Black
    }
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AnnotationToolConfig: BaseConfig
    {
        private int _ksize;
        private int _morphIteration;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [PropertyOrder(1)]
        public Shape Search_ROI
        {
            get => _search_ROI;
            set
            {
                SetProperty(ref _search_ROI, value, "Search_ROI");
            }
        }
        [DisplayName("Pixel Format")]
        [PropertyOrder(2)]
        public PixelFormat PixelFormat { get; set; } = PixelFormat.Mono;
        [DisplayName("Threshold Type")]
        [PropertyOrder(2)]
        public ThresType ThresType { get; set; } = ThresType.Binary;
        [DisplayName("Min. Threshold")]
        [PropertyOrder(3)]
        public int MinThreshold { get; set; } = 0;
        [DisplayName("Adaptive Block")]
        [PropertyOrder(4)]
        public int AdaptiveBlock { get; set; } = 3;
        [DisplayName("Morphology")]
        [PropertyOrder(4)]
        public MorphTypes Morphology { get; set; } = 0;

        [DisplayName("Kernel Size")]
        [PropertyOrder(5)]
        public int KSize
        {
            get { return this._ksize.Between(1,99); }
            set { SetProperty(ref this._ksize, value); }
        }
        [DisplayName("Morph Iteration")]
        [PropertyOrder(6)]
        public int MorphIteration
        {
            get { return this._morphIteration.Between(1, 99); }
            set { SetProperty(ref this._morphIteration, value); }
        }
        [DisplayName("Rect Color")]
        [PropertyOrder(7)]
        public RectColor RectColor { get; set; } = 0;
        [Browsable(false)]
        public string ModelLocation { get; set; } = @"C:\\Users\\jason.yap\\Desktop\\ODOCR\\saved_model_2";

        public AnnotationToolConfig()
        {
            _search_ROI = new Rectangle1() { Column1= 492, Row1 = 353, Column2= 1604,Row2= 494};
         
        }
        [XmlIgnore]
        public IDictionary<string, Shape> TeachRegions = new Dictionary<string, Shape>();
        private bool use_Search_ROI;

     
    
        private Shape _search_ROI;
       
        public override void UpdateTeachRegion()
        {
            TeachRegions.Clear();
            TeachRegions.Add("Search_ROI", Search_ROI);
     
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
        private AnnotationToolConfig config;
        public AnnotationToolConfig Config
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
  
        public string OcrDeepChar= new string(("0123456789ABCDEFGHKLMNQUZ").ToArray());
        public string OutputString = "";
        public OpenCvSharp.Size ResizeFormat = new OpenCvSharp.Size(150, 150);
        public Keras.Models.BaseModel BaseModel = new Keras.Models.BaseModel();
        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReadyToInspect = false;
        }
        public OCRShapeMatchTool(AnnotationToolConfig config)
        {
            this.Config = config;
            config.PropertyChanged += Config_PropertyChanged;
        }
       
       /* public string CompareCharResults(int hull, double moments)
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

        }*/
        public bool Setup(out InspectionData inspectionData)
        {
            InspectionData SetupResult = new InspectionData();
            #region Deep Learning
            BaseModel = Keras.Models.Model.LoadModel(config.ModelLocation);
            ReadyToInspect = true;
            /* int batch_size = 1000;   //Size of the batches per epoch
             int num_classes = 10;    //We got 10 outputs since 
                                      //we can predict 10 different labels seen on the 
                                      //dataset: https://github.com/zalandoresearch/fashion-mnist#labels
             int epochs = 30;         //Amount on trainingperiods, 
                                      //I figure it out that the maximum is something about 
                                      //700 epochs, after this it won't increase the 
                                      //accuracy siginificantly

             // input image dimensions
             int img_rows = 28, img_cols = 28;

             // the data, split between train and test sets
             var ((x_train, y_train), (x_test, y_test)) =
                                       FashionMNIST.LoadData();*/

            #endregion
            inspectionData = SetupResult;
            return true;     
        }
        public bool Run(Mat source, out InspectionData inspectionData)
        {
            InspectionData RunResult = new InspectionData();

            Mat image = source.Clone();

            if ((image.Channels() > 1) && (config.PixelFormat == PixelFormat.Mono))
            {
                Cv2.CvtColor(image, image, ColorConversionCodes.BGRA2GRAY);
            }

            Mat Display_RefImage = image.Clone();

            Rect RealSearchROI = Rect.FromLTRB((int)config.Search_ROI.Parameters[1], (int)config.Search_ROI.Parameters[0], (int)config.Search_ROI.Parameters[3], (int)config.Search_ROI.Parameters[2]);
            Cv2.Rectangle(Display_RefImage, RealSearchROI, new Scalar(0, 255, 0, 255), 2);
            RunResult.ResultOutput.Add(new MatInfo(Display_RefImage, "", "Search ROI"));
            Mat Cropped_Char = new Mat(image, RealSearchROI);

            Mat ThresObj = new Mat();

            if (config.ThresType == ThresType.Binary) { ThresObj = Cropped_Char.Threshold((double)config.MinThreshold, 255.0, ThresholdTypes.Binary); }
            else if (config.ThresType == ThresType.BinaryInv) { ThresObj = Cropped_Char.Threshold((double)config.MinThreshold, 255.0, ThresholdTypes.BinaryInv); }
            else if (config.ThresType == ThresType.AdaptiveGaussian) { ThresObj = Cropped_Char.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, config.AdaptiveBlock, 2); }
            else if (config.ThresType == ThresType.AdaptiveGaussianInv) { ThresObj = Cropped_Char.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, config.AdaptiveBlock, 2); }
            else if (config.ThresType == ThresType.AdaptiveMean) { ThresObj = Cropped_Char.AdaptiveThreshold(255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, config.AdaptiveBlock, 2); }
            else { ThresObj = Cropped_Char.AdaptiveThreshold(255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv, config.AdaptiveBlock, 2); }

            RunResult.ResultOutput.Add(new MatInfo(ThresObj, Const.Color_Red, "ThresObj"));

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(config.KSize, config.KSize));
            Mat Morphed_Char = ThresObj.MorphologyEx(config.Morphology, kernel, null, config.MorphIteration);
            RunResult.ResultOutput.Add(new MatInfo(Morphed_Char, "", "Morphed"));
            // Mat Canny = new Mat();
            // Cv2.Canny(Morphed_Char, Canny, 10, 30);

            string DeepCharFormat = new string(OcrDeepChar.ToArray());
            Point[][] contours2;
            HierarchyIndex[] hierarchyIndexes3;
            Cv2.FindContours(Morphed_Char, out contours2, out hierarchyIndexes3, mode: RetrievalModes.External,
                    method: ContourApproximationModes.ApproxSimple);
            var orderedContours2 = contours2.OrderBy(c => Cv2.BoundingRect(c).Y).OrderBy(a => Cv2.BoundingRect(a).X).ToArray();

            Mat DisplayImage = image.Clone();
            OutputString = "";
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

                    if (config.RectColor == RectColor.Black)
                    {
                        Cv2.Rectangle(DisplayImage, biggestContourRect, new Scalar(0, 0, 255, 255), 3);
                    }
                    else
                        Cv2.Rectangle(DisplayImage, biggestContourRect, new Scalar(255, 255, 255, 255), 3);
                   
                    string outputchar = "Character";
                    if (ReadyToInspect)
                    {


                        Mat croppedChar = new Mat(image, biggestContourRect);
                        Mat inverse = new Mat();
                        Cv2.BitwiseNot(croppedChar, inverse);
                        RunResult.ResultOutput.Add(new MatInfo(inverse, "", "inverse"));
                        croppedChar = inverse;
                        croppedChar = croppedChar.Resize(new OpenCvSharp.Size(100, 100));
                        //Mat reshaped = croppedChar.Reshape(0, new int[] { 1, 100, 100, 1 });
                        //var graymat = croppedChar.CvtColor(ColorConversionCodes.BGR2GRAY);
                        croppedChar.GetArray(out byte[] plainArray); //there it is, c# array for nparray constructor
                        NDarray nDarray = np.array(plainArray, dtype: np.uint8); //party party
                        nDarray.resize(new Numpy.Models.Shape(100, 100));
                        nDarray = nDarray / 255;
                        nDarray = np.expand_dims(nDarray, 0);

                        var result = BaseModel.Predict(nDarray);
                        var output = np.argmax(result).asscalar<int>();
                        outputchar = DeepCharFormat[output].ToString();
                        OutputString = OutputString + outputchar;
                    }
                 
                    RunResult.ResultOutputRect.Add(new RectInfo(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height, outputchar));

                }
            }
            if (ReadyToInspect)
            {
                if (OutputString.Count() > 10)
                {
                    string test = OutputString.Insert(9, "-");
                    if (CheckSum.SEMI_CheckSum(test))
                        RunResult.Result = Result.Pass;
                    else
                        RunResult.Result = Result.Fail;
                }
                else
                    RunResult.Result = Result.Fail;
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
    
    }
}
