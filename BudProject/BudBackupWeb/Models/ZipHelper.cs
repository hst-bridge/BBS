using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace budbackup.Models
{
    public class ZipHelper
    {
        ///  summary 
        ///  compress multiple files
        ///  /summary 
        ///  param name= files
        ///  param name= ZipedFileName
        ///  param name= Password
        ///  returns  /returns 
        public static void Zip(string[] files, string ZipedFileName, string tempPath, string Password)
        {
            ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFileName));
            s.SetLevel(6);
            ZipFileDictory(files, ZipedFileName, s, tempPath);
            s.Finish();
            s.Close();
        } 
        ///  summary 
        ///  compress multiple files
        ///  /summary 
        ///  param name= files
        ///  param name= ZipedFileName
        ///  returns  /returns 
        public static void Zip(string[] files, string ZipedFileName, string tempPath)
        {

            Zip(files, ZipedFileName, tempPath, string.Empty);
        }
        public static string Convert(string src)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(src));
        }


        private static void ZipFileDictory(string[] files, string ZipedFileName, ZipOutputStream s, string tempPath)
        {
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                //create current folder
                entry = new ZipEntry("/");  //add "/" then will create a new folder
                s.PutNextEntry(entry);
                s.UseZip64 = UseZip64.Off;
                s.Flush();
                foreach (string file in files)
                {
                    string str = Convert(file);
                    if (Directory.Exists(file)) 
                    {
                        CreateFilePath(file);
                        CopyFolder(file, "/" + GetFileName(file));
                        ZipFileDictory(file, s, "");
                    }
                    if(File.Exists(file))
                    {
                        //open the file
                        fs = File.OpenRead(file);

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = File.GetLastWriteTime(file);
                        entry.Size = fs.Length;
                        fs.Close();
                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);
                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                    entry = null;
                GC.Collect();
            }
        }
        public static string CopyFolder(string sPath, string dPath)
        {
            string flag = "success";
            try
            {
                // create the destination path               
                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }
                // copy file                  
                DirectoryInfo sDir = new DirectoryInfo(sPath);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    file.CopyTo(dPath + "\\" + file.Name, true);
                }
                // foreach the child folder                  
                DirectoryInfo dDir = new DirectoryInfo(dPath);
                DirectoryInfo[] subDirArray = sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    CopyFolder(subDir.FullName, dPath + "//" + subDir.Name);
                }
            }
            catch (Exception ex)
            {
                flag = ex.ToString();
            }
            return flag;
        }
        public static string GetFileName(string path)
        {
            return path.Substring(path.LastIndexOf('\\'));
        }
        public static void CreateFilePath(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                // path not exist, then create a new one
                System.IO.Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// compress the files
        /// </summary>
        /// <param name="FolderToZip"></param>
        /// <param name="s"></param>
        /// <param name="ParentFolderName"></param>
        private static bool ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName)
        {
            bool res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            try
            {
                //create current folder
                entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/"));  // "/" is needed
                entry.DateTime = File.GetLastWriteTime(FolderToZip);
                s.PutNextEntry(entry);
                s.Flush();


                //first compress the file, then the folder 
                filenames = Directory.GetFiles(FolderToZip);
                foreach (string file in filenames)
                {
                    //open the file
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/" + Path.GetFileName(file)));
                    entry.DateTime = File.GetLastWriteTime(file);
                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
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
                if (!ZipFileDictory(folder, s, Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip))))
                {
                    return false;
                }
            }

            return res;
        }

        /// <summary>
        /// compress the folder
        /// </summary>
        /// <param name="FolderToZip"></param>
        /// <param name="ZipedFile"></param>
        /// <returns></returns>
        private static bool ZipFileDictory(string FolderToZip, string ZipedFile, String Password)
        {
            bool res;
            if (!Directory.Exists(FolderToZip))
            {
                return false;
            }

            ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFile));
            s.SetLevel(6);
            s.Password = Password;

            res = ZipFileDictory(FolderToZip, s, "");

            s.Finish();
            s.Close();

            return res;
        }

        /// <summary>
        /// compress the file
        /// </summary>
        /// <param name="FileToZip"></param>
        /// <param name="ZipedFile"></param>
        /// <returns></returns>
        private static bool ZipFile(string FileToZip, string ZipedFile, String Password)
        {
            //if not found, throw the error
            if (!File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException(FileToZip + " not exist!");
            }
            //FileStream fs = null;
            FileStream ZipFile = null;
            ZipOutputStream ZipStream = null;
            ZipEntry ZipEntry = null;

            bool res = true;
            try
            {
                ZipFile = File.OpenRead(FileToZip);
                byte[] buffer = new byte[ZipFile.Length];
                ZipFile.Read(buffer, 0, buffer.Length);
                ZipFile.Close();

                ZipFile = File.Create(ZipedFile);
                ZipStream = new ZipOutputStream(ZipFile);
                ZipStream.Password = Password;
                ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                ZipStream.PutNextEntry(ZipEntry);
                ZipStream.SetLevel(6);

                ZipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (ZipEntry != null)
                {
                    ZipEntry = null;
                }
                if (ZipStream != null)
                {
                    ZipStream.Finish();
                    ZipStream.Close();
                }
                if (ZipFile != null)
                {
                    ZipFile.Close();
                    ZipFile = null;
                }
                GC.Collect();
                GC.Collect(1);
            }

            return res;
        }

        /// <summary>
        /// compress the file and folder
        /// </summary>
        /// <param name="FileToZip"></param>
        /// <param name="ZipedFile"></param>
        /// <returns></returns>
        public static bool Zip(String FileToZip, String ZipedFile, String Password)
        {
            if (Directory.Exists(FileToZip))
            {
                return ZipFileDictory(FileToZip, ZipedFile, Password);
            }
            else if (File.Exists(FileToZip))
            {
                return ZipFile(FileToZip, ZipedFile, Password);
            }
            else
            {
                return false;
            }
        }
    }
}