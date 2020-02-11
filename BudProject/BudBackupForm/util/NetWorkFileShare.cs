using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using log4net;
using System.Reflection;

namespace BudBackupSystem.util
{
    public class NetWorkFileShare
    {
        /// <summary>
        /// log4net
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public NetWorkFileShare() { }

        public bool ConnectState(string path)
        {
            return ConnectState(path, "", "");
        }

        public bool ConnectState(string path, string userName, string passWord)
        {
            bool Flag = false;

            //路径不含反斜杠和盘符时，加上
            if (!path.StartsWith(@"\\") && path.IndexOf(":/") < 0 && path.IndexOf(@":\") < 0)
            {
                path = @"\\" + path;
            }
            RemoveConnectState(path);
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = "net use " + path + " \"" + passWord + "\" /User:\"" + userName + "\" /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    logger.Info(errormsg);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }
        /// <summary>
        /// 用net use delete命令移除网络共享连接
        /// </summary>
        /// <param name="Server">目标ip</param>
        public void RemoveConnectState(string path)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = "net.exe";
                process.StartInfo.Arguments = @"use " + path + " /delete";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            finally
            {
                process.Close();
                process.Dispose();
            }
        }
        //read file  
        public void ReadFiles(string path)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.  
                // The using statement also closes the StreamReader.  
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;
                    // Read and display lines from the file until the end of   
                    // the file is reached.  
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        //write file  
        public void WriteFiles(string path)
        {
            try
            {
                // Create an instance of StreamWriter to write text to a file.  
                // The using statement also closes the StreamWriter.  
                using (StreamWriter sw = new StreamWriter(path))
                {
                    // Add some text to the file.  
                    sw.Write("This is the ");
                    sw.WriteLine("header for the file.");
                    sw.WriteLine("-------------------");
                    // Arbitrary objects can also be written to the file.  
                    sw.Write("The date is: ");
                    sw.WriteLine(DateTime.Now);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
    }
}
