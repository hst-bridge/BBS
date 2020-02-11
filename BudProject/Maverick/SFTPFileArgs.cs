using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maverick.SFTP;

namespace Maverick
{
    /// <summary>
    /// use this class  to cover the sftpfile
    /// </summary>
    public class SFTPFileArgs
    {
        /// <summary>
        /// when SftpFile = null,use this property
        /// </summary>
        public string FailPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SFTPFile SftpFile{get;set;}
    }
}
