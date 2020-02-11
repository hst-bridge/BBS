using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Common
{
    public class TransferConfFile
    {
        /// <summary> 
        /// ファイルのパス
        /// </summary> 
        private string _filepath;

        /// <summary> 
        /// 書き込むのIO
        /// </summary> 
        private StreamWriter fWriter = null;

        /// <summary> 
        /// ファイルのパス
        /// </summary> 
        public string FilePath
        {
            get { return _filepath; }
            set { _filepath = value; }
        }

        /// <summary> 
        /// ファイルを呼んでArrayListに保存して、行番号によって並べる
        /// </summary> 
        /// <param   name= "FilePath "> </param> 
        public TransferConfFile(string FilePath)
        {
            _filepath = FilePath;

            if (File.Exists(_filepath))
            {
                File.Delete(_filepath);
            }
            // ファイル新規
            FileStream fs = File.Create(_filepath);
            fWriter = new StreamWriter(fs, Encoding.GetEncoding("utf-8"));
            //fWriter = new StreamWriter(fs, Encoding.GetEncoding("shift_jis"));
        }
        /// <summary> 
        /// 配置ファイル作成
        /// </summary> 
        /// <param name= "バックアップ元ファイル">monitordir</param> 
        /// <param name= "バックアップ先対象api_user">api_user</param> 
        /// <param name= "バックアップ先対象api_password">api_password</param>
        /// <param name= "バックアップ先対象remotenode">remotenode</param>
        /// <param name= "バックアップ先対象destdir">destdir</param>
        /// <returns></returns> 
        public void Write(string monitordir, string api_user, string api_password, string remotenode, string destdir)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[local_server]");
            sb.Append("\r\n");
            sb.Append("# REST API base URI.");
            sb.Append("\r\n");
            sb.Append("base_url=http://127.0.0.1:9090/silver-bullet/admin/");
            sb.Append("\r\n");
            sb.Append("# REST API authorization user.");
            sb.Append("\r\n");
            sb.Append("api_user="+api_user);
            sb.Append("\r\n");
            sb.Append("# REST API user password.");
            sb.Append("\r\n");
            sb.Append("api_password="+api_password);
            sb.Append("\r\n");
            sb.Append("[params]");
            sb.Append("\r\n");
            sb.Append("# Remote node id.");
            sb.Append("\r\n");
            sb.Append("# can be configured in \"Settings > Settings for accessing this node > Destination node settings\"");
            sb.Append("\r\n");
            sb.Append("remotenode="+remotenode);
            sb.Append("\r\n");
            sb.Append("# File path of transfer target (full path notation).");
            sb.Append("\r\n");            
            sb.Append("srcfile=" + monitordir);
            sb.Append("\r\n");
            sb.Append("# Directory of file transfer destination (full path notation).");
            sb.Append("\r\n");
            sb.Append("# Exclude file name.");
            sb.Append("\r\n");
            sb.Append("destdir=" + destdir);
            sb.Append("\r\n");
            sb.Append("# Minimum transfer rate (bps).");
            sb.Append("\r\n");
            sb.Append("minbps=1048576");
            sb.Append("\r\n");
            sb.Append("# Maximum transfer rate (bps).");
            sb.Append("\r\n");
            sb.Append("maxbps=104857600");
            sb.Append("\r\n");
            sb.Append("# Initial transfer rate (bps).");
            sb.Append("\r\n");
            sb.Append("initbps=10485760");
            sb.Append("\r\n");
            sb.Append("# Initial transfer rate (bps).");
            sb.Append("\r\n");
            sb.Append("initbps=10485760");
            sb.Append("\r\n");
            sb.Append("# The maximum rate control policy to be applied to this file transfer task.");
            sb.Append("\r\n");
            sb.Append("# 0 = Reserved; 1 = Fair; 2 = Aggressive; 3 = Fixed (not recommended).");
            sb.Append("\r\n");
            sb.Append("policy=1");
            sb.Append("\r\n");
            sb.Append("# Whether the transfer file entity is encrypted	or not in this file transfer task.");
            sb.Append("\r\n");
            sb.Append("# True = encrypt; False = not to encrypt.");
            sb.Append("\r\n");
            sb.Append("encrypt=False");
            sb.Append("\r\n");
            sb.Append("# Transfer only modified file.");
            sb.Append("\r\n");
            sb.Append("# True = transfer only modified files; False = transfer all files");
            sb.Append("\r\n");
            sb.Append("transferonlyupdated=False");
            sb.Append("\r\n");
            sb.Append("[return_code]");
            sb.Append("\r\n");
            sb.Append("# Normal end.");
            sb.Append("\r\n");
            sb.Append("NORMAL_END=0");
            sb.Append("\r\n");
            sb.Append("# Destination directory creation error.");
            sb.Append("\r\n");
            sb.Append("DEST_DIR_CREATION_ERROR=11");
            sb.Append("\r\n");
            sb.Append("# Local server access error.");
            sb.Append("\r\n");
            sb.Append("LOCAL_SERVER_ACCESS_ERROR=12");
            sb.Append("\r\n");
            sb.Append("# Local server authorization error.");
            sb.Append("\r\n");
            sb.Append("LOCAL_SERVER_AUTH_ERROR=13");
            sb.Append("\r\n");
            sb.Append("# Session establishment error.");
            sb.Append("\r\n");
            sb.Append("SESSION_ESTABLISHMENT_ERROR=14");
            sb.Append("\r\n");
            sb.Append("# Transfer initiation error.");
            sb.Append("\r\n");
            sb.Append("TRANSFER_INITIATION_ERROR=15");
            sb.Append("\r\n");
            sb.Append("# Aborted by transfer failure.");
            sb.Append("\r\n");
            sb.Append("TRANSFER_ABORTED=16");
            sb.Append("\r\n");
            sb.Append("# Aborted by transfer canceled.");
            sb.Append("\r\n");
            sb.Append("TRANSFER_CANCELLED=17");
            sb.Append("\r\n");
            sb.Append("# Session close error.");
            sb.Append("\r\n");
            sb.Append("SESSION_CLOSE_ERROR=18");
            sb.Append("\r\n");
            sb.Append("# Unknown error.");
            sb.Append("\r\n");
            sb.Append("UNKNOWN_ERROR=99");
            sb.Append("\r\n");
            sb.Append("[log]");
            sb.Append("\r\n");
            sb.Append("# Enable or disable log file output.");
            sb.Append("\r\n");
            sb.Append("# True: enable; False = disable");
            sb.Append("\r\n");
            sb.Append("writeLogFile=False");
            sb.Append("\r\n");
            sb.Append("# Log output level.");
            sb.Append("\r\n");
            sb.Append("# all=0, debug=10, info=20, warn=30, error=40, critical=50");
            sb.Append("\r\n");
            sb.Append("logLevel=10");
            sb.Append("\r\n");
            sb.Append("# Log file output path.");
            sb.Append("\r\n");
            sb.Append("logFilePath=dirsync.log");
            sb.Append("\r\n");
            sb.Append("# Max size of log file (in bytes).");
            sb.Append("\r\n");
            sb.Append("logFileMaxBytes=10485760");
            sb.Append("\r\n");
            sb.Append("# Number of rotate log file backup.");
            sb.Append("\r\n");
            sb.Append("logFileBackupCount=5");
            sb.Append("\r\n");

            fWriter.Write(sb.ToString() + "\r\n");
            // I/O close
            fWriter.Flush();
            fWriter.Dispose();
            fWriter.Close();
        }

    }
}
