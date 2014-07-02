using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMoverBLL
{
    public static class FileCopyHelper
    {
        public static void MoveFiles(Dictionary<string, DateTime> files, string targetDir, string logFile, bool deleteFiles = true)
        {
            System.IO.StreamWriter tmpLogFile = null;
            string sTmpLogFile = Path.GetTempFileName();

            try
            {
                // open the tmp log file
                tmpLogFile = new System.IO.StreamWriter(sTmpLogFile);

                // open the final log file this creats it if it does not allready exist
                System.IO.StreamWriter fLogFile = new System.IO.StreamWriter(logFile, true);
                fLogFile.Close();

                // write to the log
                if (deleteFiles)
                    tmpLogFile.WriteLine(string.Format("{0} Moving Files ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(115, '>'));
                else
                    tmpLogFile.WriteLine(string.Format("{0} Copying Files ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(115, '>'));

                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                // itearate files
                foreach (var file in files.OrderByDescending(x => x.Value))
                {
                    // file name
                    var SourceFilePath = file.Key;
                    // get the date the picture was taken from teh file
                    var dt = file.Value;

                    // the name of the directory to copy teh file to
                    var target = String.Format(@"{0}\{1}\{2}\{3}", targetDir, dt.ToString("yyyy"), dt.ToString("yyyy-MM"), dt.ToString("yyyy-MM-dd"));

                    if (!Directory.Exists(target))
                        Directory.CreateDirectory(target);

                    // the full name of the destination file 
                    target = Path.Combine(@"{0}\{1}", target, Path.GetFileName(SourceFilePath));

                    try
                    {
                        // Test to see if a file exists of teh same name and if so come up with a new file name
                        CreateFileName(ref target, true);

                        // copy source file to teh destination
                        File.Copy(SourceFilePath, target);


                        // write to the log
                        tmpLogFile.WriteLine(string.Format("{0} '{1}' copied to '{2}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), file, target));

                        if (deleteFiles)
                        {
                            // write to the log
                            tmpLogFile.WriteLine(string.Format("{0} '{1}' deleted", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), file));
                            // delete file
                            File.Delete(SourceFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        tmpLogFile.WriteLine(ex.Message);
                        //throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                tmpLogFile.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                // write to the log
                if (deleteFiles)
                    tmpLogFile.WriteLine(string.Format("{0} Moving Files complete ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(115, '>'));
                else
                    tmpLogFile.WriteLine(string.Format("{0} Copying Files complete ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(115, '>'));

                tmpLogFile.WriteLine();

                // get the content from the log file
                string content = File.ReadAllText(logFile);
                // write to the tmp log file
                tmpLogFile.Write(content);
                // close the tmp log file
                tmpLogFile.Close();
                // copy the tmp log file to the log file
                content = File.ReadAllText(sTmpLogFile);
                File.WriteAllText(logFile, content);
                // delete the tmp log file
                File.Delete(sTmpLogFile);
            }
        }

        private static void CreateFileName(ref string filePath, bool firstIteration)
        {
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                var file = fi.Name;
                var dir = fi.DirectoryName;

                if (firstIteration)
                {
                    var args = file.Split(new string[] { "." }, StringSplitOptions.None);
                    filePath = string.Format(@"{0}\{1}_{2}.{3}", dir, args[0], 1, args[1]);
                }
                else
                {
                    var args1 = file.Split(new string[] { "." }, StringSplitOptions.None);
                    var args2 = args1[0].Split(new string[] { "_" }, StringSplitOptions.None);

                    var i = int.Parse(args2[args2.Length - 1]) + 1;

                    var args3 = args2.Take(args2.Length - 1);

                    var f = string.Join("", args3);

                    f = string.Format("{0}_{1}", f, i);

                    f = string.Format("{0}.{1}", f, args1[1]);

                    filePath = string.Format(@"{0}\{1}", dir, f);
                }

                CreateFileName(ref filePath, false);
            }
        }
    }
}
