using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Common
{
    /// <summary>
    /// メール関連 のヘルパークラス.
    /// </summary>
    class DotNetSendMail
    {
        static string strHost = string.Empty;
        static string strAccount = string.Empty;
        static string strPwd = string.Empty;
        static string strFrom = string.Empty;
        public DotNetSendMail()
        {
            strHost = System.Configuration.ConfigurationManager.AppSettings["strHost"];
            strAccount = System.Configuration.ConfigurationManager.AppSettings["strAccount"];
            strPwd = System.Configuration.ConfigurationManager.AppSettings["strPwd"];
            strFrom = System.Configuration.ConfigurationManager.AppSettings["strFrom"];
        }
        /// <summary>
        /// メール送信
        /// </summary>
        /// <param name="to">送信先</param>
        /// <param name="title">題名</param>
        /// <param name="content">本文</param>
        /// <returns></returns>
        /// <author>cwm</author>
        /// <date>2013-11-22</date>
        public bool sendMail(string to, string title, string content)
        {
            SmtpClient _smtpClient = new SmtpClient();
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.Host = strHost; ;
            _smtpClient.Credentials = new System.Net.NetworkCredential(strAccount, strPwd);

            MailMessage _mailMessage = new MailMessage(strFrom, to);
            _mailMessage.Subject = title;
            _mailMessage.Body = content;
            _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            //テキスト/HTMLの設定
            _mailMessage.IsBodyHtml = true;
            _mailMessage.Priority = MailPriority.Normal;

            try
            {
                _smtpClient.Send(_mailMessage);
                return true;
            }
            catch (Exception e)
            {
                LogManager.WriteLog(LogFile.ERROR, e.Message);
                return false;
            }
        }
    }
}
