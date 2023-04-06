using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Vision.Framework
{
    public class ImageInfo : IDisposable
    {
        // Properties.
        public Mat Image
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        // Methods.
        public ImageInfo Clone()
        {
            return new ImageInfo()
            {
                Image = new Mat(Image),
                Name = Name,
            };
        }

        // ctor.
        public ImageInfo()
        {
        }

        #region IDisposable members.
        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }
        }
        #endregion
    }
}
