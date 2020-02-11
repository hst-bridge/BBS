using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using log4net;
using System.Reflection;

namespace BudFileListen.Common
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
                string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
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
                    throw new Exception(errormsg);
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
