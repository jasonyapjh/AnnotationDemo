using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Base.Vision
{
    public static class OpenCV
    {
        public static bool AttachImage(ImageSource window, BitmapImage image)
        {
            window = image;
            return true;
        }
        public static void AttachImage(ImageSource window, Mat image)
        {
            window = MatToBitmapImage(image);
            //return true;
        }
        public static void AttachImage(ImageSource window, Bitmap bitmap)
        {
            window = ConvertBitmapToBitmapSource(bitmap);
            //return true;
        }
        #region Convert Mat to Bitmap and BitmapImage
        public static Bitmap MatToBitmap(Mat image)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        }
 
        public static BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            //MemoryStream stream = new MemoryStream();
            MemoryStream stream = new MemoryStream();

            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); 

            stream.Position = 0;
            BitmapImage result = new BitmapImage();
            result.BeginInit();
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();
            return result;



        }
        public static BitmapImage MatToBitmapImage(Mat image)
        {
            Bitmap bitmap = MatToBitmap(image);
           // BitmapImage result = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;

            }
         
        }
        #endregion
    }
}
