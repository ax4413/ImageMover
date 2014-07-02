using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileMoverBLL;
using System.Configuration;

namespace FileMoverTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceDir = ConfigurationManager.AppSettings["SourceFiles"];
            string targetPhotoDir = ConfigurationManager.AppSettings["PhotoDestination"];
            string targetVideoDir = ConfigurationManager.AppSettings["VideoDestination"];
            string logFile = Path.Combine(sourceDir, "Transfer.log");


            var images = ImageHelper.IdentifyImages(sourceDir);
            FileCopyHelper.MoveFiles(images, targetPhotoDir, logFile, true);

            var videos = VideoHelper.IdentifyVideos(sourceDir);
            FileCopyHelper.MoveFiles(videos, targetVideoDir, logFile, true);
        }
        
    }
}
