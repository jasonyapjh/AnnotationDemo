using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Vision
{
    public class Const
    {
        #region Types
        public const string String = "System.String";
        public const string Integer = "System.Int32";
        public const string Double = "System.Double";
        #endregion
        #region CameraServer
        public const int StationA = 0;
        public const int StationB = 1;
        public const int StationC = 2;
        public const int StationD = 3;
        public const string PixelFormat = "PixelFormat";
        public const string GainAuto = "GainAuto";
        public const string OffsetX = "OffsetX";
        public const string OffsetY = "OffsetY";
        public const string Width = "Width";
        public const string Height = "Height";
        public const string GainRaw = "GainRaw";
        public const string Gain = "Gain";
        public const string AcquisitionFrameCount = "AcquisitionFrameCount";
        public const string TriggerSelector = "TriggerSelector";
        public const string TriggerMode = "TriggerMode";
        public const string TriggerSource = "TriggerSource";
        public const string ExposureAuto = "ExposureAuto";
        public const string ExposureTimeRaw = "ExposureTimeRaw";
        public const string ExposureTime = "ExposureTime";
        public const string AcquisitionFrameRateAbs = "AcquisitionFrameRateAbs";


        #endregion
        #region Halcon
        public const string Byte = "byte";

        public const string Rectangle1 = "rectangle1";
        public const string Rectangle2 = "rectangle2";
        public const string Circle = "circle";
        public const string CircleSector = "circle_arc";
        public const string Ellipse = "ellipse";
        public const string EllipseSector = "ellipse_arc";
        public const string Line = "line";
        public const string XLD = "xld";
        public const string Text = "text";

        public const string Tiff = "tiff";
        public const string Tif = "tif";

        public const string Type = "type";
        public const string Row = "row";
        public const string Row1 = "row1";
        public const string Row2 = "row2";
        public const string Column = "column";
        public const string Column1 = "column1";
        public const string Column2 = "column2";
        public const string Radius = "radius";
        public const string Radius1 = "radius1";
        public const string Radius2 = "radius2";
        public const string StartAngle = "start_angle";
        public const string EndAngle = "end_angle";
        public const string Phi = "phi";
        public const string Length1 = "length1";
        public const string Length2 = "length2";
        public const string LineStyle = "line_style";
        public const string LineWidth = "line_width";

        #region Color
        public const string Color = "color";
        public const string Color_Black = "black";                                // #000000
        public const string Color_White = "white";                                // #FFFFFF
        public const string Color_Red = "red";                                    // #FF0000
        public const string Color_Green = "green";                                // #00FF00
        public const string Color_Blue = "blue";                                  // #0000FF
        public const string Color_DimGray = "dim gray";                           // #696969
        public const string Color_Gray = "gray";                                  // #BEBEBE
        public const string Color_LightGray = "light gray";                       // #D3D3D3
        public const string Color_Cyan = "cyan";                                  // #00FFFF
        public const string Color_Magneta = "magenta";                            // #FF00FF
        public const string Color_Yellow = "yellow";                              // #FFFF00
        public const string Color_MediumSlateBlue = "medium slate blue";          // #7B68EE
        public const string Color_Coral = "coral";                                // #FF7F50
        public const string Color_SlateBlue = "slate blue";                       // #6A5ACD
        public const string Color_SpringGreen = "spring green";                   // #00FF7F
        public const string Color_OrangeRed = "orange red";                       // #FF4500
        public const string Color_DarkOliveGreen = "dark olive green";            // #556B2F
        public const string Color_Pink = "pink";                                  // #FFC0CB
        public const string Color_CadetBlue = "cadet blue";                       // #5F9EA0
        public const string Color_Goldenrod = "goldenrod";                        // #DAA520
        public const string Color_Orange = "orange";                              // #FFA500
        public const string Color_Gold = "gold";                                  // #FFD700
        public const string Color_ForestGreen = "forest green";                   // #228B22
        public const string Color_CornflowerBlue = "cornflower blue";             // #6495ED
        public const string Color_Navy = "navy";                                  // #000080
        public const string Color_Turquoise = "turquoise";                        // #40E0D0
        public const string Color_DarkSlateBlue = "dark slate blue";              // #483D8B
        public const string Color_LightBlue = "light blue";                       // #ADD8E6
        public const string Color_IndianRed = "indian red";                       // #CD5C5C
        public const string Color_VioletRed = "violet red";                       // #D02090
        public const string Color_LightSteelBlue = "light steel blue";            // #B0C4DE
        public const string Color_MediumBlue = "medium blue";                     // #0000CD
        public const string Color_Khaki = "khaki";                                // #F0E68C
        public const string Color_Violet = "violet";                              // #EE82EE
        public const string Color_Firebrick = "firebrick";                        // #B22222
        public const string Color_MidnightBlue = "midnight blue";                 // #191970
        public const string Color_SeaGreen = "sea green";                         // #2E8B57
        public const string Color_DarkTurquoise = "dark turquoise";               // #00CED1
        public const string Color_Orchid = "orchid";                              // #DA70D6
        public const string Color_Sienna = "sienna";                              // #A0522D
        public const string Color_MediumOrchid = "medium orchid";                 // #BA55D3
        public const string Color_MediumForestGreen = "medium forest green";      // #6B8E23
        public const string Color_MediumTurquoise = "medium turquoise";           // #48D1CC
        public const string Color_MediumVioletRed = "medium violet red";          // #C71585
        public const string Color_Salmon = "salmon";                              // #FA8072
        public const string Color_BlueViolet = "blue violet";                     // #8A2BE2
        public const string Color_Tan = "tan";                                    // #D2B48C
        public const string Color_PaleGreen = "pale green";                       // #98FB98
        public const string Color_SkyBlue = "sky blue";                           // #87CEEB
        public const string Color_MediumGoldrod = "medium goldenrod";             // #EAEAAD
        public const string Color_Plum = "plum";                                  // #DDA0DD
        public const string Color_Thistle = "thistle";                            // #D8BFD8
        public const string Color_DarkOrchid = "dark orchid";                     // #9932CC
        public const string Color_Maroon = "maroon";                              // #B03060
        public const string Color_DarkGreen = "dark green";                       // #006400
        public const string Color_SteelBlue = "steel blue";                       // #4682B4
        public const string Color_MediumSpringGreen = "medium spring green";      // #00FA9A
        public const string Color_MediumSeaGreen = "medium sea green";            // #3CB371
        public const string Color_YellowGreen = "yellow green";                    // #9ACD32
        public const string Color_MediumAquamarine = "medium aquamarine";         // #66CDAA
        public const string Color_LimeGreen = "lime green";                       // #32CD32
        public const string Color_Aquamarine = "aquamarine";                      // #7FFFD4
        public const string Color_Wheat = "wheat";                                // #F5DEB3
        public const string Color_GreenYellow = "green yellow";


        #endregion



        #region Draw Mode
        public const string Fill = "fill";
        public const string Filled = "filled";
        public const string Margin = "margin";
        #endregion

        #endregion
    }
}
