using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.Model
{
    /// <summary>
    /// Database server model
    /// </summary>
    class DBServer
    {
        public string ServerName { get; set; }
        public string LoginName { get; set; }
        /// <summary>
        /// Base64 Encryption and decryption
        /// </summary>
        public string Password { get; set; }

        public string DatabaseName { get; set; }

        /// <summary>
        /// get the connection string 
        /// </summary>
        public string ConnString {
            get
            {
                StringBuilder sb = new StringBuilder("server=@ServerName;uid=@LoginName;pwd=@Password;database=@DatabaseName;");
                sb.Replace("@ServerName", ServerName).Replace("@LoginName", LoginName).Replace("@Password", Password).Replace("@DatabaseName", DatabaseName);
                return sb.ToString();
            }
        }
    }
}
