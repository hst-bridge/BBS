using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Ionic.Zip;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;

namespace budbackup.CommonWeb.Helper
{
    public class ZipIonicHelper
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// zip when has special file
        /// </summary>
        /// <param name="fileToZips"></param>
        /// <param name="zipedFile"></param>
        /// <param name="basePath"></param>
        public static void ZipAdvanced(string[] fileToZips, string zipedFile, string basePath)
        {

            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipedFile, Encoding.GetEncoding(932)))
            {
                zip.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Always;
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
                zip.BufferSize = 65536 * 8; // Set the buffersize to 512k for better efficiency with large files
               
                foreach (string fileToZip in fileToZips)
                {
                    if (LongPath.IsFileExists(fileToZip))
                    {
                        FileStream fs = LongPath.OpenForOnlyRead(fileToZip);
                        zip.AddEntry(fileToZip.Substring(basePath.Length), fs);

                        continue;
                    }

                    else //文件夹
                    {
                        if (Directory.Exists(fileToZip))
                        {
                            foreach (var e in Directory.EnumerateFiles(fileToZip, "*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    //判断文件名是否以空格结尾
                                    bool spaceEnd = false;
                                    string name = e.Substring(e.LastIndexOf('\\') + 1);
                                    if (name.TrimEnd().Length < name.Length)
                                    {
                                        spaceEnd = true;
                                    }
                                    if (!spaceEnd && File.Exists(e))
                                    {
                                        zip.AddFile(e, e.Substring(basePath.Length, e.LastIndexOf('\\') - basePath.Length));
                                    }
                                    else
                                    {
                                        string entryName = e.Substring(basePath.Length);
                                        /*
                                         * 末尾添加全角空格,(ps:如果同一目录下有个 文件a 又有一个文件 a空格 则解压时会同名冲突，故添加全角空格以区分
                                         */
                                        if (spaceEnd)
                                        {
                                            entryName += "　";
                                        }
                                        FileStream fs = LongPath.OpenForOnlyRead(e);
                                        zip.AddEntry(entryName, fs);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("path:"+e+Environment.NewLine+ex.Message);
                                }
                            }
                        }
                        else
                        {
                            foreach (String path in FilesystemHelper.EnumFilesYield(fileToZip))
                            {
                                try
                                {
                                    FileStream fs = LongPath.OpenForOnlyRead(path);
                                    zip.AddEntry(path.Substring(basePath.Length), fs);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("path:" + path + Environment.NewLine + ex.Message);
                                    
                                }
                            }
                        }

                    }
                }
                zip.Save();
            }
        }
        /// <summary>
        /// 压缩zip
        /// </summary>
        /// <param name="fileToZips">文件路径集合</param>
        /// <param name="zipedFile">想要压成zip的文件名</param>
        public static void Zip(string[] fileToZips, string zipedFile,string basePath)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipedFile, Encoding.UTF8))
            {
                foreach (string fileToZip in fileToZips)
                {
                    if (System.IO.File.Exists(fileToZip))
                    {
                        zip.AddFile(fileToZip, fileToZip.Substring(basePath.Length, fileToZip.LastIndexOf('\\') - basePath.Length));
                        continue;
                    }

                    if (System.IO.Directory.Exists(fileToZip)) 
                    {
                        zip.AddDirectory(fileToZip, fileToZip.Substring(basePath.Length));
                    }
                }
                zip.Save();
            }
        }

        /// <summary>
        /// 压缩zip
        /// </summary>
        /// <param name="fileToZips">文件路径集合</param>
        /// <param name="zipedFile">想要压成zip的文件名</param>
        public static void ZipOld(string[] fileToZips, string zipedFile)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipedFile, Encoding.UTF8))
            {
                List<string> pathlist = new List<string>();
                foreach (string fileToZip in fileToZips)
                {
                    if (System.IO.File.Exists(fileToZip))
                    {
                        using (FileStream fs = new FileStream(fileToZip, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                            string filePath = fileToZip.Substring(0, fileToZip.LastIndexOf("\\"));

                            if (!pathlist.Exists(x => x.Equals(filePath)))
                            {
                                pathlist.Add(filePath);
                                zip.AddDirectoryByName(filePath);
                            }
                            zip.AddFile(fileToZip, filePath);
                        }
                    }
                    if (System.IO.Directory.Exists(fileToZip))
                    {
                        string filePath = fileToZip.Substring(fileToZip.IndexOf("\\") + 1);
                        zip.AddDirectoryByName(filePath);
                        zip.AddDirectory(fileToZip, filePath);
                        //ZipFileDictory(fileToZip, "", zip);
                    }
                }
                zip.Save();
            }
        }
        public static string GetFileName(string path)
        {
            return path.Substring(path.LastIndexOf('\\'));
        }
        /// <summary>
        /// compress the files
        /// </summary>
        /// <param name="FolderToZip"></param>
        /// <param name="s"></param>
        /// <param name="ParentFolderName"></param>
        private static bool ZipFileDictory(string FolderToZip, string ParentFolderName,Ionic.Zip.ZipFile zip)
        {
            bool res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;

            try
            {
                //create current folder
                using (Ionic.Zip.ZipFile izip = new Ionic.Zip.ZipFile(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip)), Encoding.UTF8))
                {

                    //first compress the file, then the folder 
                    filenames = Directory.GetFiles(FolderToZip);
                    foreach (string file in filenames)
                    {
                        if (File.Exists(file))
                        {
                            using (FileStream innerfs = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                byte[] buffer = new byte[innerfs.Length];
                                innerfs.Read(buffer, 0, buffer.Length);
                                string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                                izip.AddEntry(fileName, buffer);
                            }
                        }
                    }
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            folders = Directory.GetDirectories(FolderToZip);
            foreach (string folder in folders)
            {
                if (!ZipFileDictory(folder, Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip)),zip))
                {
                    return false;
                }
            }

            return res;
        }
    }
}