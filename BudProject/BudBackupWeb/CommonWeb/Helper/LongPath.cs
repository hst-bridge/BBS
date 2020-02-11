using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class LongPath
{
    static class Win32Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public class SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr pSecurityDescriptor;
            public int bInheritHandle;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreateDirectory(string lpPathName, SECURITY_ATTRIBUTES lpSecurityAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, FileShare dwShareMode, SECURITY_ATTRIBUTES securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);


    }

    public static bool CreateDirectory(string path)
    {
        return Win32Native.CreateDirectory(String.Concat(@"\\?\", path), null);
    }

    public static FileStream Open(string path, FileMode mode, FileAccess access)
    {
        SafeFileHandle handle = Win32Native.CreateFile(String.Concat(@"\\?\", path), (uint)0x10000000, FileShare.Read, null, mode, (int)0x00000080, IntPtr.Zero);
        if (handle.IsInvalid)
        {
            throw new System.ComponentModel.Win32Exception();
        }
        return new FileStream(handle, access);
    }

    public static FileStream OpenForOnlyRead(string path)
    {
        path = GetUNCPath(path);
        SafeFileHandle handle = Win32Native.CreateFile(path, (uint)0x80000000, FileShare.Read, null, FileMode.Open, (int)0x00000080, IntPtr.Zero);
        if (handle.IsInvalid)
        {
            throw new System.ComponentModel.Win32Exception();
        }
        return new FileStream(handle, FileAccess.Read);
    }

    public static bool IsFileExists(string path)
    {
        path = GetUNCPath(path);
        SafeFileHandle handle = Win32Native.CreateFile(path, (int)0x00000000, FileShare.None, null, FileMode.Open, (int)0x00000080, IntPtr.Zero);
        handle.Close();
        if (handle.IsInvalid)
        {
            return false;
        }

        return true;
    }



    public static bool CreateFile(string path)
    {
        path = GetUNCPath(path);
        SafeFileHandle handle = Win32Native.CreateFile(path, (int)0x10000000, FileShare.None, null, FileMode.Create, (int)0x00000080, IntPtr.Zero);
        if (handle.IsInvalid)
        {
            return false;
        }

        return true;
    }

    public static string GetUNCPath(string path)
    {
        if (path.Substring(0, 2).Equals(@"\\"))
        {
            path = string.Concat(@"\\?\UNC\", path.Substring(2));
        }
        else
        {
            path = String.Concat(@"\\?\", path);
        }

        return path;
    }
}
