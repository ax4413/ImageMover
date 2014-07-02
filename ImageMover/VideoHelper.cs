using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMoverBLL
{
    public static class VideoHelper
    {
        public static Dictionary<string, DateTime> IdentifyVideos(string dir)
        {
            var videoExts = new string[] { "mov" };

            // list of files that are images
            var images = new Dictionary<string, DateTime>();

            videoExts.ToList()
                .ForEach(ext => Directory.GetFiles(dir)
                    .Where(x => x.ToLower()
                        .EndsWith(ext))
                        .ToList()
                        .ForEach(f => images.Add(f, GetCreatedDate(f))));

            return images;
        }

        public static DateTime GetCreatedDate(string path)
        {
            var fi = new FileInfo(path);

            return fi.LastWriteTime;
        }
    }
}
