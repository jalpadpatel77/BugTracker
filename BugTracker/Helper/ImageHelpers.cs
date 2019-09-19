using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace BugTracker.Helper
{
    public class ImageHelpers
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase filePath)
        {
            var file = filePath;
            if (file == null)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                           ImageFormat.Png.Equals(img.RawFormat) ||
                           ImageFormat.Icon.Equals(img.RawFormat) ||
                           ImageFormat.Bmp.Equals(img.RawFormat) ||
                           ImageFormat.Gif.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }

        }
        public static bool IsValidAttachment(HttpPostedFileBase file)
        {
            try
            {

                if (file == null)
                    return false;

                if (file.ContentLength > 5 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;
                            
                var extValid = false;
                foreach(var ext in WebConfigurationManager.AppSettings["AllowedAttachmentExtensions"].Split(','))
                {
                    if (Path.GetExtension(file.FileName)==ext)
                    {
                        extValid = true;
                        break;
                    }
                 }
                return IsWebFriendlyImage(file) || extValid;
            }
            catch
            {
                return false;
            }

        }
               
    }
}