using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace budbackup.Models
{
    /// <summary>
    /// the infomation to access server shared file
    /// autor:xiecongwen 20141202
    /// </summary>
    public class ServerShareInfo
    {
        /// <summary>
        /// base unc
        /// </summary>
        public string UNCBase { get; set; }

        public string Username { get; set; }

        public string Passwd { get; set; }

    }
}