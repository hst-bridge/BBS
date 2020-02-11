using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace Common
{
    public enum ERROR_ID
    {
        ERROR_SUCCESS = 0,  // Success
        ERROR_BUSY = 170,
        ERROR_MORE_DATA = 234,
        ERROR_NO_BROWSER_SERVERS_FOUND = 6118,
        ERROR_INVALID_LEVEL = 124,
        ERROR_ACCESS_DENIED = 5,
        ERROR_INVALID_PASSWORD = 86,
        ERROR_INVALID_PARAMETER = 87,
        ERROR_BAD_DEV_TYPE = 66,
        ERROR_NOT_ENOUGH_MEMORY = 8,
        ERROR_NETWORK_BUSY = 54,
        ERROR_BAD_NETPATH = 53,
        ERROR_NO_NETWORK = 1222,
        ERROR_INVALID_HANDLE_STATE = 1609,
        ERROR_EXTENDED_ERROR = 1208,
        ERROR_DEVICE_ALREADY_REMEMBERED = 1202,
        ERROR_NO_NET_OR_BAD_PATH = 1203
    }

    public enum RESOURCE_SCOPE
    {
        RESOURCE_CONNECTED = 1,
        RESOURCE_GLOBALNET = 2,
        RESOURCE_REMEMBERED = 3,
        RESOURCE_RECENT = 4,
        RESOURCE_CONTEXT = 5
    }

    public enum RESOURCE_TYPE
    {
        RESOURCETYPE_ANY = 0,
        RESOURCETYPE_DISK = 1,
        RESOURCETYPE_PRINT = 2,
        RESOURCETYPE_RESERVED = 8,
    }

    public enum RESOURCE_USAGE
    {
        RESOURCEUSAGE_CONNECTABLE = 1,
        RESOURCEUSAGE_CONTAINER = 2,
        RESOURCEUSAGE_NOLOCALDEVICE = 4,
        RESOURCEUSAGE_SIBLING = 8,
        RESOURCEUSAGE_ATTACHED = 16,
        RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
    }

    public enum RESOURCE_DISPLAYTYPE
    {
        RESOURCEDISPLAYTYPE_GENERIC = 0,
        RESOURCEDISPLAYTYPE_DOMAIN = 1,
        RESOURCEDISPLAYTYPE_SERVER = 2,
        RESOURCEDISPLAYTYPE_SHARE = 3,
        RESOURCEDISPLAYTYPE_FILE = 4,
        RESOURCEDISPLAYTYPE_GROUP = 5,
        RESOURCEDISPLAYTYPE_NETWORK = 6,
        RESOURCEDISPLAYTYPE_ROOT = 7,
        RESOURCEDISPLAYTYPE_SHAREADMIN = 8,
        RESOURCEDISPLAYTYPE_DIRECTORY = 9,
        RESOURCEDISPLAYTYPE_TREE = 10,
        RESOURCEDISPLAYTYPE_NDSCONTAINER = 11
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NETRESOURCE
    {
        public RESOURCE_SCOPE dwScope;
        public RESOURCE_TYPE dwType;
        public RESOURCE_DISPLAYTYPE dwDisplayType;
        public RESOURCE_USAGE dwUsage;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpLocalName;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpRemoteName;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpComment;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpProvider;
    }

    public class NetworkConnection
    {

        [DllImport("mpr.dll")]
        public static extern int WNetAddConnection2A(NETRESOURCE[] lpNetResource, string lpPassword, string lpUserName, int dwFlags);

        [DllImport("mpr.dll")]
        public static extern int WNetCancelConnection2A(string sharename, int dwFlags, int fForce);

        public static int Connect(string remotePath, string localPath, string username, string password)
        {
            NETRESOURCE[] share_driver = new NETRESOURCE[1];
            share_driver[0].dwScope = RESOURCE_SCOPE.RESOURCE_GLOBALNET;
            share_driver[0].dwType = RESOURCE_TYPE.RESOURCETYPE_DISK;
            share_driver[0].dwDisplayType = RESOURCE_DISPLAYTYPE.RESOURCEDISPLAYTYPE_SHARE;
            share_driver[0].dwUsage = RESOURCE_USAGE.RESOURCEUSAGE_CONNECTABLE;
            share_driver[0].lpLocalName = localPath;
            share_driver[0].lpRemoteName = remotePath;

            Disconnect(localPath);
            int ret = WNetAddConnection2A(share_driver, password, username, 1);
            ret = ret == 85 ? 0 : ret;
            return ret;
        }

        public static int Disconnect(string localpath)
        {
            return WNetCancelConnection2A(localpath, 1, 1);
        }
    }
    public class NetWorkFileShare1
    {
        public NetWorkFileShare1() { }

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
                    Exception ex = new Exception(errormsg);
                    throw ex;
                }
            }
            catch (Exception e)
            {
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
            }
        }
    }
}
