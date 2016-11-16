using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

// Use the same namespace as Photo so it will be included when that namespace is included.
namespace PaD.Models
{
    public static class PhotoExtensionMethods
    {
        #region Day.GenerateThumbnail
        /// <summary>
        /// Generates the Photo.Thumbnail for this Photo from its Photo.Photo.
        /// </summary>
        /// <param name="photo">The Photo for which you ant to geenerate a Thumbnail</param>
        /// <param name="width">Width of the generated thumbnail image</param>
        /// <param name="height">Height of the generated thumbnail image</param>
        public static void GenerateThumbnail(this Photo photo, int width = 100, int height = 100)
        {
            if (photo.PhotoImage == null || photo.PhotoImage.Bytes == null)
            {
                photo.ThumbnailImage = null;
                return;
            }

            Image image = photo.PhotoImage.Bytes.ToImage();
            // Resize image to the passed width/height while preserving aspect ratio.
            // Some of the image will be cropped, but it will not be distorted.
            Image thumbnail = ResizeImage(image, ref width, ref height);

            // Convert it to JPEG to save space.
            using (MemoryStream ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Jpeg);

                // Create new thumbnail or update existing one.
                if (photo.ThumbnailImage == null)
                {
                    photo.ThumbnailImage = new ThumbnailImage();
                }

                photo.ThumbnailImage.ContentType = ImageFormat.Jpeg.GetMimeType();
                photo.ThumbnailImage.Bytes = ms.ToArray();
                photo.ThumbnailImage.PhotoId = photo.PhotoId;
            }
        }
        #endregion

        #region Image.ResizeImage
        /// <summary>
        /// Resizes the passed image to the passed width/height preserving aspect ratio.
        /// So it is cropped to match the passed width/height after resizing.
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="width">Width of the resized image</param>
        /// <param name="height">Height of the resized image</param>
        private static Image ResizeImage(Image image, ref int width, ref int height)
        {
            Image tnail = null;

            // Save the passed dimensions for the cropped image.
            int croppedWidth = width;
            int croppedHeight = height;

            // Get a number by which to multiply the dimensions to scale it down.
            double scale = 1;
            if ((image.Width > width) || (image.Height > height))
            {
                double widthScale = (double)width / image.Width;
                double heightScale = (double)height / image.Height;

                // Use the larger of the 2. ** This makes the image the correct size in one dimension and too long in the other.
                // We will crop the dimension that is too long.
                scale = widthScale > heightScale ? widthScale : heightScale;

            }

            // Calculate the new height/width
            width = (int)(scale * image.Width);
            height = (int)(scale * image.Height);

            // Create a thumbnail from the image with this new height/width
            try
            {
                tnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            // Now crop
            Bitmap cropped = new Bitmap(croppedWidth, croppedHeight);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                //set the resize quality modes to high quality
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(tnail, new Rectangle(0, 0, croppedWidth, croppedHeight),
                    new Rectangle((width - croppedWidth) / 2, (height - croppedHeight) / 2, croppedWidth, croppedHeight),
                    GraphicsUnit.Pixel);
            }

            tnail.Dispose();

            // Remember to set these to the cropped width/height.
            width = croppedWidth;
            height = croppedHeight;

            return cropped;
        }
        #endregion

        #region Image.ToByteArray
        public static byte[] ToByteArray(this Image image)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] bytes = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));

            return bytes;
        }
        #endregion

        #region byte[].ToImage
        public static Image ToImage(this byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Image image = Image.FromStream(ms);

            return image;
        }
        #endregion

        #region ImageFormat.GetMimeType
        /// <summary>
        /// Returns the mime type for the ImageFormat as a string.
        /// </summary>
        /// <returns> A string representation of the ImageFormat. For example, ImageFormat.Jpeg returns "image/jpeg".</returns>
        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }
        #endregion
    }
}