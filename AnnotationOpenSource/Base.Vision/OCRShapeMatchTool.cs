using Base.Common;
using Base.Vision.Framework;
using Base.Vision.Shapes;
using Base.Vision.Shapes.Base;
using Base.Vision.Tool.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Base.Vision.Inspection_Tool
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OCRShapeMatchConfig : BaseConfig
    {
        public string OCRTrainedImage { get; set; } = @"C:\Users\jason.yap\source\repos\AnnotationsCreator\AnnotationsCreator\bin\x64\Vision Setting\System Setting\bcssemi.png";
        public string OCRBrightFieldRefImage { get; set; } = @"C:\Users\jason.yap\source\repos\AnnotationsCreator\AnnotationsCreator\bin\x64\Vision Setting\System Setting\BrightFieldRef.jpg";
        public string OCRDarkFieldRefImage { get; set; } = @"C:\Users\jason.yap\source\repos\AnnotationsCreator\AnnotationsCreator\bin\x64\Vision Setting\System Setting\DarkFieldRef.jpg";
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
    public class OcrShapeResult
    {
        public string ReadStrings;
        public List<double> Confidence;
        public bool PassCheckSum;
    }
  /*  public class OCRShapeMatchTool : BaseTool
    {
        //public OcrShapeResult OcrResult = new OcrShapeResult();
        public LightingMode LightingMode = LightingMode.DarkField;
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
        public string OcrAvailAlphabet = new string(("ABCDEFGHIJKLMNOPQRSTUVWXYZ").ToArray());
        public string OcrAvailNumber = new string(("1234567890").ToArray());

        public double ScaleMin { get; set; }
        public double ScaleMax { get; set; }
        public double AngleStart { get; set; }
        public double AngleExtent { get; set; }

        public string OutputStrings { get; set; }
        public List<double> OutputScores = new List<double>();
        public InspectionData AllResult { get; set; }
        HTuple CharModelIDs = new HTuple();
        HTuple DarkFieldCharModelIDs = new HTuple();
        HTuple BCharModelID = new HTuple();
        HTuple BDarkFieldCharModelID = new HTuple();
        HTuple UCharModelID = new HTuple();
        HTuple UDarkFieldCharModelID = new HTuple();

        HTuple NumModelIDs = new HTuple();
        HTuple DarkFieldNumModelIDs = new HTuple();

        HTuple ChecksumCharModelIDs = new HTuple();
        HTuple DarkFieldChecksumCharModelIDs = new HTuple();

        HTuple BrightNccModelID = new HTuple();
        HTuple DarkNccModelID = new HTuple();

        HTuple Bright_ref_row = new HTuple();
        HTuple Bright_ref_col = new HTuple();
        HTuple Bright_ref_ang = new HTuple();
        HTuple Bright_score = new HTuple();
        HTuple Dark_ref_row = new HTuple();
        HTuple Dark_ref_col = new HTuple();
        HTuple Dark_ref_ang = new HTuple();
        HTuple Dark_score = new HTuple();
        HTuple HomMat2DIdentity = new HTuple();
        HObject CharContours, NumContours, AllCharacterRegion;

        private OCRShapeMatchConfig config;

        public OCRShapeMatchTool(OCRShapeMatchConfig config)
        {
            this.Config = config;
            config.PropertyChanged += Config_PropertyChanged;
        }
        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReadyToInspect = false;
        }

        public override bool Run(HObject image, out InspectionData data)
        {
            if (!ReadyToInspect)
                Setup(out InspectionData inspectionData);

          
            bool InspectionSuccess = false;
            if (LightingMode == LightingMode.BrightField)
                InspectionSuccess = RunInspection(image, BrightNccModelID, CharModelIDs, NumModelIDs, ChecksumCharModelIDs, BCharModelID, UCharModelID, Bright_ref_row, Bright_ref_col, Bright_ref_ang, out InspectionData AllResult);
            else
                InspectionSuccess = RunInspection(image, DarkNccModelID, DarkFieldCharModelIDs, DarkFieldNumModelIDs, DarkFieldChecksumCharModelIDs, BDarkFieldCharModelID, UDarkFieldCharModelID, Dark_ref_row, Dark_ref_col, Dark_ref_ang, out InspectionData AllResult);

            data = AllResult;
            return true;
        }


   
        public bool RunInspection(HObject InputImage, HTuple NccModelID, HTuple CharacterModelID, HTuple NumberModelID, HTuple CheckSumCharacterModelID, HTuple BCharModelID, HTuple UCharModelID, HTuple ref_row, HTuple ref_col, HTuple ref_ang, out InspectionData data)
        {
            try
            {
                InspectionData RunResult = new InspectionData();
                RunResult.Result = Result.Pass;

                bool IsSuccess = true;

                RunResult.ResultTuple[0]= "";

                RunResult.ResultTuple[1] = 0;

                HTuple HomMat2DTrans, HomMat2DRotate;
                HObject AllCharacterRegionTrans, CharImageReduced, ConnectedRegions, OcrRegion, ObjectSelected;
                HObject StringContours;

                OutputStrings = "";
                OutputScores.Clear();

                HOperatorSet.GenEmptyObj(out StringContours);

                string CharFormat = new string(config.OcrFormat.ToArray());

                HOperatorSet.FindNccModel(InputImage, NccModelID, AngleStart, AngleExtent, 0.3, 1, 0.5, "true", 0, out HTuple Row, out HTuple Col, out HTuple Ang, out HTuple Score);

                if (Score.TupleLength() > 0)
                {
                    HOperatorSet.HomMat2dTranslate(HomMat2DIdentity, Row - ref_row, Col - ref_col, out HomMat2DTrans);
                    HOperatorSet.HomMat2dRotate(HomMat2DTrans, Ang - ref_ang, Row, Col, out HomMat2DRotate);
                    HOperatorSet.AffineTransRegion(AllCharacterRegion, out AllCharacterRegionTrans, HomMat2DRotate, "nearest_neighbor");

                    // HOperatorSet.ReduceDomain(InputImage, AllCharacterRegionTrans, out CharImageReduced);
                    HOperatorSet.Connection(AllCharacterRegionTrans, out ConnectedRegions);
                    HOperatorSet.SortRegion(ConnectedRegions, out OcrRegion, "first_point", "true", "row");
                    HOperatorSet.CountObj(OcrRegion, out HTuple Number);
                    for (int i = 0; i < (int)Number; i++)
                    {
                        HOperatorSet.SelectObj(OcrRegion, out ObjectSelected, i + 1);

                        HOperatorSet.DilationCircle(ObjectSelected, out ObjectSelected, 5);

                        HOperatorSet.ReduceDomain(InputImage, ObjectSelected, out CharImageReduced);


                        string OutputChar = "";
                        HTuple CRow = new HTuple(), CCol = new HTuple(), CAng = new HTuple(), CScore, CScale = new HTuple(), CModel = new HTuple();
                        HTuple BRow = new HTuple(), BCol = new HTuple(), BAng = new HTuple(), BScore, BScale = new HTuple(), BModel = new HTuple();
                        HTuple URow = new HTuple(), UCol = new HTuple(), UAng = new HTuple(), UScore, UScale = new HTuple(), UModel = new HTuple();
                        HOperatorSet.TupleGenConst(1, 0, out CScore);

                        if (CharFormat[i] == 'A')
                        {
                            HOperatorSet.FindScaledShapeModels(CharImageReduced, CharacterModelID, AngleStart, AngleExtent, ScaleMin, ScaleMax, 0.5, 1, 0.1, "least_squares_high", 0, 0.9,
                                out CRow, out CCol, out CAng, out CScale, out CScore, out CModel);
                            HOperatorSet.TupleLength(CModel, out HTuple Found);
                            if (Found > 0) OutputChar = CharacterConverter((int)CModel);
                            HOperatorSet.FindScaledShapeModels(CharImageReduced, BCharModelID, AngleStart, AngleExtent, ScaleMin, ScaleMax, 0.6, 1, 0.1, "least_squares_high", 0, 0.9,
                                out BRow, out BCol, out BAng, out BScale, out BScore, out BModel);
                            HOperatorSet.FindScaledShapeModels(CharImageReduced, UCharModelID, AngleStart, AngleExtent, ScaleMin, ScaleMax, 0.5, 1, 0.1, "least_squares_high", 0, 0.9,
                              out URow, out UCol, out UAng, out UScale, out UScore, out UModel);
                            HOperatorSet.TupleLength(BModel, out HTuple BFound);
                            HOperatorSet.TupleLength(UModel, out HTuple UFound);
                            if (BFound > 0)
                            {
                                if ((double)BScore > 0.72)
                                {
                                    OutputChar = CharacterConverter((int)1);
                                    CRow = BRow;
                                    CCol = BCol;
                                    CAng = BAng;
                                    CScore = BScore;
                                    CScale = BScale;
                                    CModel = 1;
                                }
                            }
                            if (UFound > 0)
                            {
                                if ((double)UScore > 0.75)
                                {
                                    OutputChar = CharacterConverter((int)20);
                                    CRow = URow;
                                    CCol = UCol;
                                    CAng = UAng;
                                    CScore = UScore;
                                    CScale = UScale;
                                    CModel = 20;
                                }
                            }

                        }
                        else if (CharFormat[i] == 'N')
                        {
                            HOperatorSet.FindScaledShapeModels(CharImageReduced, NumberModelID, AngleStart, AngleExtent, ScaleMin, ScaleMax, 0.5, 1, 0.5, "least_squares", 0, 0.9,
                                out CRow, out CCol, out CAng, out CScale, out CScore, out CModel);
                            HOperatorSet.TupleLength(CModel, out HTuple Found);
                            if (Found > 0) OutputChar = NumberConverter((int)CModel);

                        }
                        else if (CharFormat[i] == 'a')
                        {
                            HOperatorSet.FindScaledShapeModels(CharImageReduced, CheckSumCharacterModelID, AngleStart, AngleExtent, ScaleMin, ScaleMax, 0.5, 1, 0.5, "least_squares", 0, 0.9,
                                out CRow, out CCol, out CAng, out CScale, out CScore, out CModel);
                            HOperatorSet.TupleLength(CModel, out HTuple Found);
                            if (Found > 0) OutputChar = CharacterConverter((int)CModel);
                            //OutputChar = CharacterConverter((int)CModel);
                        }
                        else if (CharFormat[i] == '-')
                        {
                            OutputChar = "-";

                        }
                        else
                        {
                            OutputChar = "X";
                        }

                        if (OutputChar != "" && OutputChar != "-")
                        {
                            OutputStrings = OutputStrings + OutputChar;
                            //HOperatorSet.TupleString(CScore, ".2f", out CScore);
                            OutputScores.Add(CScore);

                            HOperatorSet.HomMat2dTranslate(HomMat2DIdentity, CRow, CCol, out HomMat2DTrans);
                            HOperatorSet.HomMat2dRotate(HomMat2DTrans, CAng, CRow, CCol, out HomMat2DRotate);
                            HOperatorSet.HomMat2dScale(HomMat2DRotate, CScale, CScale, CRow, CCol, out HTuple HomMat2DScale);

                            if (CharFormat[i] == 'A' || CharFormat[i] == 'a')
                            {
                                HOperatorSet.SelectObj(CharContours, out HObject FoundContour, ((int)CModel + 1));
                                HOperatorSet.AffineTransContourXld(FoundContour, out FoundContour, HomMat2DScale);
                                HOperatorSet.ConcatObj(StringContours, FoundContour, out StringContours);
                            }
                            else if (CharFormat[i] == 'N')
                            {
                                HOperatorSet.SelectObj(NumContours, out HObject FoundContour, ((int)CModel + 1));
                                HOperatorSet.AffineTransContourXld(FoundContour, out FoundContour, HomMat2DScale);
                                HOperatorSet.ConcatObj(StringContours, FoundContour, out StringContours);
                            }
                        }
                        else if (OutputChar == "-")
                        {
                            OutputStrings = OutputStrings + OutputChar;
                            OutputScores.Add(0.99);
                        }
                        else
                        {
                            OutputStrings = OutputStrings + "X";
                            OutputScores.Add(0.00);
                            //IsSuccess = true;
                            IsSuccess = false;
                        }

                    }
                    RunResult.ResultTuple[0] = OutputStrings;
                    RunResult.ResultTuple[1] = CheckSum.SEMI_CheckSum(OutputStrings) ? 1 : 0;
                    HOperatorSet.GenContourRegionXld(AllCharacterRegionTrans, out HObject AllCharacterContour, "border");
                    RunResult.ResultOutput.Add(new HObjectInfo(AllCharacterContour, Const.Color_Green, "OCR Results"));
                    RunResult.ResultOutput.Add(new HObjectInfo(StringContours, Const.Color_Green, "OCR String"));

                }
                else
                    IsSuccess = false;

                data = RunResult;
                return IsSuccess;
            }
            catch (Exception exp)
            {
                ErrorMessage = (exp.GetDetailMessage());
                data = null;
                return false;
            }
        }
        public string CharacterConverter(int i)
        {
            return (string)OcrAvailAlphabet[i].ToString();
        }
        public string NumberConverter(int i)
        {
            return (string)OcrAvailNumber[i].ToString();
        }
        public override bool Setup(out InspectionData data)
        {

            HOperatorSet.GenEmptyObj(out CharContours);
            HOperatorSet.GenEmptyObj(out NumContours);
            HOperatorSet.GenEmptyRegion(out AllCharacterRegion);
            HOperatorSet.HomMat2dIdentity(out HomMat2DIdentity);

            //
            CharModelIDs = new HTuple();
            DarkFieldCharModelIDs = new HTuple();
            BCharModelID = new HTuple();
            BDarkFieldCharModelID = new HTuple();
            UCharModelID = new HTuple();
            UDarkFieldCharModelID = new HTuple();

            NumModelIDs = new HTuple();
            DarkFieldNumModelIDs = new HTuple();

            ChecksumCharModelIDs = new HTuple();
            DarkFieldChecksumCharModelIDs = new HTuple();

            BrightNccModelID = new HTuple();
            DarkNccModelID = new HTuple();

            Bright_ref_row = new HTuple();
            Bright_ref_col = new HTuple();
            Bright_ref_ang = new HTuple();
            Bright_score = new HTuple();
            Dark_ref_row = new HTuple();
            Dark_ref_col = new HTuple();
            Dark_ref_ang = new HTuple();
            Dark_score = new HTuple();


            //

            HObject TrainContour = new HObject();
            HTuple HomMatScale = new HTuple();
            HTuple HomMat2D = new HTuple();
            InspectionData SetupResult = new InspectionData();

            HOperatorSet.ReadImage(out HObject TrainImage, config.OCRTrainedImage);

            HOperatorSet.ReadImage(out HObject BrightFieldRef, config.OCRBrightFieldRefImage);

            HOperatorSet.ReadImage(out HObject DarkFieldRef, config.OCRDarkFieldRefImage);

            SetupResult.ResultOutput.Add(new HObjectInfo(BrightFieldRef, Const.Color_Green, "Bright Ref Image"));
            SetupResult.ResultOutput.Add(new HObjectInfo(DarkFieldRef, Const.Color_Green, "Dark Ref Image"));

            HOperatorSet.GenRectangle1(out HObject ROI_Char, 2, 10, 129, 430);

            HOperatorSet.GenRectangle1(out HObject ROI_Number, 125, 6.9, 190, 330);

            HOperatorSet.ReduceDomain(TrainImage, ROI_Char, out HObject CharDomain);

            HOperatorSet.ReduceDomain(TrainImage, ROI_Number, out HObject NumberDomain);

            HOperatorSet.Threshold(CharDomain, out HObject CharRegion, 0, 60);

            HOperatorSet.Threshold(NumberDomain, out HObject NumberRegion, 0, 60);

            HOperatorSet.Connection(CharRegion, out HObject CharConnectedRegions);

            HOperatorSet.Connection(NumberRegion, out HObject NumberConnectedRegions);

            HOperatorSet.SortRegion(CharConnectedRegions, out HObject CharSortedRegions, "character", "true", "row");

            HOperatorSet.SortRegion(NumberConnectedRegions, out HObject NumberSortedRegions, "character", "true", "row");

            HOperatorSet.DilationCircle(CharSortedRegions, out CharSortedRegions, 3);

            HOperatorSet.DilationCircle(NumberSortedRegions, out NumberSortedRegions, 3.5);

            HOperatorSet.CountObj(CharSortedRegions, out HTuple CharLength);

            HOperatorSet.CountObj(NumberSortedRegions, out HTuple NumberLength);

            HOperatorSet.SelectObj(CharSortedRegions, out HObject FirstCharSelected, 1);

            HOperatorSet.RegionFeatures(FirstCharSelected, "height", out HTuple RefCharHeight);
            HOperatorSet.RegionFeatures(FirstCharSelected, "width", out HTuple RefCharWidth);

            HObject RealChar_ROI = Halcon.CreateRegion(config.Character_ROI);

            HOperatorSet.RegionFeatures(RealChar_ROI, "height", out HTuple RealCharHeight);
            HOperatorSet.RegionFeatures(RealChar_ROI, "width", out HTuple RealCharWidth);

            double ScaleHeight = RealCharHeight / RefCharHeight;
            double ScaleWidth = RealCharWidth / RefCharWidth;

            ScaleMin = 0.98;
            ScaleMax = 1.05;
            double ValueScale = 1.48;

            AngleStart = (-10).ConvertDegreesToRadians();
            AngleExtent = (10).ConvertDegreesToRadians();

            HOperatorSet.ZoomImageFactor(TrainImage, out HObject TrainImageZoomed, ScaleWidth, ScaleHeight, "constant");
            HOperatorSet.InvertImage(TrainImageZoomed, out HObject TrainImageZoomedInvert);

            for (int i = 1; i < CharLength + 1; i++)
            {
                HOperatorSet.GenEmptyObj(out TrainContour);
                HOperatorSet.SelectObj(CharSortedRegions, out HObject ObjectSelected, i);

                HOperatorSet.ReduceDomain(TrainImage, ObjectSelected, out HObject ImageReduced);

                HOperatorSet.Threshold(ImageReduced, out HObject TrainRegion, 0, 180);

                HOperatorSet.GenContourRegionXld(TrainRegion, out HObject Contours, "border_holes");

                HOperatorSet.CountObj(Contours, out HTuple ContourSize);

                if ((int)ContourSize > 0)
                {
                    HOperatorSet.SelectShapeXld(Contours, out TrainContour, "area", "and", 300, 1000);
                    if (i == 2)
                    {
                        HOperatorSet.SelectShapeXld(Contours, out TrainContour, "area", "and", 100, 300);
                    }
                    else if (i == 12)
                    {
                        HOperatorSet.SelectShapeXld(Contours, out TrainContour, "area", "and", 150, 350);
                    }
                }


                HOperatorSet.HomMat2dScaleLocal(HomMat2DIdentity, ScaleWidth, ScaleHeight, out HomMatScale);

                HOperatorSet.AffineTransContourXld(TrainContour, out TrainContour, HomMatScale);

                HOperatorSet.CreateScaledShapeModelXld(TrainContour, "auto", AngleStart, AngleExtent, "auto", ScaleMin, ScaleMax, "auto", "auto", "ignore_local_polarity", 10, out HTuple ModelID);

                HOperatorSet.FindShapeModel(TrainImageZoomed, ModelID, AngleStart, AngleExtent, 0.5, 1, 0.5, "least_squares", 0, 0.9, out HTuple TempRow, out HTuple TempCol, out HTuple TempAng, out HTuple TempScore);

                HOperatorSet.VectorAngleToRigid(0, 0, 0, TempRow, TempCol, TempAng, out HomMat2D);

                HOperatorSet.GetShapeModelContours(out HObject ModelContours, ModelID, 1);

                HOperatorSet.SetShapeModelMetric(TrainImageZoomed, ModelID, HomMat2D, "use_polarity");

                HOperatorSet.CountObj(ModelContours, out HTuple TwoContours);


                if (TwoContours > 1)
                {
                    HOperatorSet.AreaCenterXld(ModelContours, out HTuple TwoArea, out HTuple TwoRow, out HTuple TwoCol, out HTuple tupless);
                    HOperatorSet.SelectShapeXld(ModelContours, out ModelContours, "area", "and", TwoArea[0] + 20, TwoArea[1] + 100);
                    //HOperatorSet.SelectShapeXld(ModelContours, out ModelContours, "area", "and", 420*ValueScale, 500*ValueScale);
                }
                if (i == 2)
                {
                    BCharModelID = ModelID;
                }
                if (i == 21)
                {
                    UCharModelID = ModelID;
                }

                HOperatorSet.TupleConcat(CharModelIDs, ModelID, out CharModelIDs);



                HOperatorSet.ConcatObj(CharContours, ModelContours, out CharContours);

                HOperatorSet.CreateScaledShapeModelXld(TrainContour, "auto", AngleStart, AngleExtent, "auto", ScaleMin, ScaleMax, "auto", "auto", "ignore_local_polarity", 5, out HTuple DarkModelID);

                HOperatorSet.FindShapeModel(TrainImageZoomedInvert, DarkModelID, AngleStart, AngleExtent, 0.5, 1, 0.5, "least_squares", 0, 0.9, out TempRow, out TempCol, out TempAng, out TempScore);

                HOperatorSet.VectorAngleToRigid(0, 0, 0, TempRow, TempCol, TempAng, out HomMat2D);

                HOperatorSet.GetShapeModelContours(out ModelContours, DarkModelID, 1);

                HOperatorSet.SetShapeModelMetric(TrainImageZoomedInvert, DarkModelID, HomMat2D, "use_polarity");

                HOperatorSet.TupleConcat(DarkFieldCharModelIDs, DarkModelID, out DarkFieldCharModelIDs);

                if (i == 2)
                {
                    BDarkFieldCharModelID = DarkModelID;
                }
                if (i == 21)
                {

                    UDarkFieldCharModelID = DarkModelID;
                }
                if (i < 9)
                {
                    HOperatorSet.TupleConcat(ChecksumCharModelIDs, ModelID, out ChecksumCharModelIDs);
                    HOperatorSet.TupleConcat(DarkFieldChecksumCharModelIDs, DarkModelID, out DarkFieldChecksumCharModelIDs);
                }
            }


            for (int j = 1; j < NumberLength + 1; j++)
            {
                HOperatorSet.SelectObj(NumberSortedRegions, out HObject ObjectSelected, j);

                HOperatorSet.ReduceDomain(TrainImage, ObjectSelected, out HObject ImageReduced);

                HOperatorSet.ZoomImageFactor(ImageReduced, out ImageReduced, ScaleWidth, ScaleHeight, "constant");

                HOperatorSet.CreateScaledShapeModel(ImageReduced, "auto", AngleStart, AngleExtent, "auto", ScaleMin, ScaleMax, "auto", "auto", "use_polarity", 40, 5, out HTuple ModelID);

                HOperatorSet.InspectShapeModel(ImageReduced, out HObject ModelImages, out HObject ModelRegions, 1, 30);

                HOperatorSet.GetShapeModelContours(out HObject ModelContours, ModelID, 1);

                HOperatorSet.TupleConcat(NumModelIDs, ModelID, out NumModelIDs);

                HOperatorSet.CountObj(ModelContours, out HTuple ContourSize);

                if ((int)ContourSize > 1)
                {
                    HOperatorSet.AreaCenterXld(ModelContours, out HTuple TwoArea, out HTuple TwoRow, out HTuple TwoCol, out HTuple tupless);
                    HOperatorSet.SelectShapeXld(ModelContours, out ModelContours, "area", "and", 800 * ValueScale, 3000 * ValueScale);
                }
                HOperatorSet.ConcatObj(NumContours, ModelContours, out NumContours);

                HOperatorSet.InvertImage(ImageReduced, out HObject ImageInvert);

                HOperatorSet.CreateScaledShapeModel(ImageInvert, "auto", AngleStart, AngleExtent, "auto", ScaleMin, ScaleMax, "auto", "auto", "use_polarity", 40, 5, out HTuple DarkModelID);

                HOperatorSet.InspectShapeModel(ImageInvert, out ModelImages, out ModelRegions, 1, 30);

                HOperatorSet.GetShapeModelContours(out ModelContours, DarkModelID, 1);

                HOperatorSet.TupleConcat(DarkFieldNumModelIDs, DarkModelID, out DarkFieldNumModelIDs);

                //if (j < 8)
               // {
               //     HOperatorSet.TupleConcat(DarkFieldChecksumCharModelIDs, DarkModelID, out DarkFieldChecksumCharModelIDs);
               // }

            }
            HOperatorSet.TupleLength(DarkFieldNumModelIDs, out HTuple Test);

            HObject SearchRegion = Halcon.CreateRegion(config.Search_ROI);

            HOperatorSet.ReduceDomain(BrightFieldRef, SearchRegion, out HObject NccImageReduced);

            HOperatorSet.CreateNccModel(NccImageReduced, "auto", AngleStart, AngleExtent, "auto", "use_polarity", out BrightNccModelID);

            HOperatorSet.FindNccModel(BrightFieldRef, BrightNccModelID, AngleStart, AngleExtent, 0.7, 1, 0.5, "true", 0, out Bright_ref_row, out Bright_ref_col, out Bright_ref_ang, out Bright_score);

            HOperatorSet.ReduceDomain(DarkFieldRef, SearchRegion, out HObject DarkNccImageReduced);

            HOperatorSet.CreateNccModel(DarkNccImageReduced, "auto", AngleStart, AngleExtent, "auto", "use_polarity", out DarkNccModelID);

            HOperatorSet.FindNccModel(DarkFieldRef, DarkNccModelID, AngleStart, AngleExtent, 0.7, 1, 0.5, "true", 0, out Dark_ref_row, out Dark_ref_col, out Dark_ref_ang, out Dark_score);

            HOperatorSet.Threshold(DarkNccImageReduced, out HObject Regions, config.OCRDarkFieldThreshold, 255);

            SetupResult.ResultOutput.Add(new HObjectInfo(Regions, Const.Color_Blue, "Dark Field Threshold"));

            HOperatorSet.DilationCircle(Regions, out Regions, 3);

            HOperatorSet.Connection(Regions, out HObject RegionsConnected);

            HOperatorSet.SelectShape(RegionsConnected, out HObject SelectedRegions, "area", "and", 400, 4000);
            // 3000 t0 4000

            HOperatorSet.SortRegion(SelectedRegions, out HObject Characters, "character", "true", "row");

            HOperatorSet.AreaCenter(Characters, out HTuple Area, out HTuple CharRow, out HTuple CharCol);

            HOperatorSet.CountObj(Characters, out HTuple Number);

            //HOperatorSet.TupleMedian(CharRow, out HTuple Median);
            double Median = CharRow.TupleMedian();
            string CharFormat = new string(config.OcrFormat.ToArray());
            for (int i = 0; i < Number; i++)
            {
                if (i < 14 && CharFormat[i] == '-')
                {
                    //  HOperatorSet.GenRectangle2(out HObject Rectangle, Median, CharCol[i], Bright_ref_ang, 3, 3);
                    HOperatorSet.GenRectangle2(out HObject Rectangle, Median, CharCol[i], Bright_ref_ang, (RealCharWidth / 2) + 2, (RealCharHeight / 2) + 3);
                    HOperatorSet.Union2(AllCharacterRegion, Rectangle, out AllCharacterRegion);
                }
                else
                {
                    HOperatorSet.GenRectangle2(out HObject Rectangle, Median, CharCol[i], Bright_ref_ang, (RealCharWidth / 2) + 2, (RealCharHeight / 2) + 3);
                    HOperatorSet.Union2(AllCharacterRegion, Rectangle, out AllCharacterRegion);
                }

            }
            SetupResult.ResultOutput.Add(new HObjectInfo(AllCharacterRegion, Const.Color_Blue, "Character Regions"));


            ReadyToInspect = true;


            HomMat2D.Dispose();
            HomMatScale.Dispose();
            data = SetupResult;
            return true;

        }
    }*/

}
