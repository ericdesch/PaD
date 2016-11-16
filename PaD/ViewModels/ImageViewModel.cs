using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace PaD.ViewModels
{
    public class ImageViewModel
    {
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }

        /// <summary>
        /// Returns a string with the image converted to a base64 string.
        /// Suitable for using in, ie. <image href="@Photo.ImgSrc" ... />
        /// </summary>
        public string ImgSrc
        {
            get
            {
                string base64 = Convert.ToBase64String(Bytes);
                string imgSrc = String.Format("data:" + ContentType + ";base64,{0}", base64);

                return imgSrc;
            }
        }

        public ImageViewModel() { }

    }
}