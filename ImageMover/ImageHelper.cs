using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;


namespace FileMoverBLL
{
    public static class ImageHelper
    {
        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetCreatedDate(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        public static Dictionary<string, DateTime> IdentifyImages(string dir)
        {
            var imageExts = new string[] { "jpg", "jpeg" };

            // list of files that are images
            var images = new Dictionary<string, DateTime>();

            imageExts.ToList()
                .ForEach(ext => Directory.GetFiles(dir)
                    .Where(x => x.ToLower()
                        .EndsWith(ext))
                        .ToList()
                        .ForEach(f => images.Add(f, GetCreatedDate(f))));

            return images;               
        }
    }
}
