using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Data;
using log4net;
using System.Reflection;

namespace BudFileCheckListen.Common
{
    public class SendMail
    {
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 发送eamil
        /// </summary>
        /// <param name="strAllInfo">mail body</param>
        /// <param name="mailattachments">file name</param>
        /// <param name="toFlag">0: send to receiver set in &lt;appSettings&gt;; other: send to me.</param>
        public void SendEmail(string strAllInfo, string mailattachments = null, int toFlag = 0)
        {
            SmtpClient client = new SmtpClient();
            MailMessage msg = new MailMessage();
            client.UseDefaultCredentials = true;

            DataSet ds = new DataSet();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;

            AppConfig ac = new AppConfig();

            try
            {
                logger.Info("Begin to get the head info of mail...");
                string username = ac.getEmailUser();
                string cc = ac.getEmailDataSendCC();
                string password = ac.getEmailPwd();
                string smtp = ac.getSMTP();
                string to = ac.getEmailDataSendTo();
                string subject = ac.getMailDataSubject();
                logger.Info("Success to get the head info of mail.");

                logger.Info("Begin to get or set the SMTP info...");
                client.Credentials = new NetworkCredential(username, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = smtp;//smtp
                client.Port = 25;

                client.Timeout = 20000;
                logger.Info("Success to get or set the SMTP info.");

                string[] toArr = to.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] toArrCC = cc.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //根据toFlag，判断是否群发
                logger.Info("Begin to load the infos of receivers and cc...");
                if (toFlag == 0)
                {
                    for (int j = 0; j < toArr.Length; j++)
                    {
                        msg.To.Add(toArr[j]);//设置发送给谁
                    }
                    for (int k = 0; k < toArrCC.Length; k++)
                    {
                        msg.CC.Add(toArrCC[k]);//设置CC给谁
                    }
                }
                else
                {
                    msg.To.Add("wangjingdong@hstech-china.com");
                }
                logger.Info("Success to load the infos of receivers and cc.");

                MailAddress address = new MailAddress(username);//from
                msg.From = address;
                msg.Subject = subject;//主题设置
                if (!String.IsNullOrWhiteSpace(mailattachments) && System.IO.File.Exists(mailattachments))
                {
                    logger.Info("Begin to load attachments info...");
                    msg.Attachments.Add(new Attachment(mailattachments));
                    logger.Info("Success to load attachments info.");
                }
                else
                {
                    logger.Info("there is not a attachment to load.");
                }
                msg.Body = strAllInfo;
                logger.Info("Success to load the mail's body.");
                msg.SubjectEncoding = encoding;
                msg.BodyEncoding = encoding;
                msg.IsBodyHtml = false;
                logger.Info("Success to set the mail's subject and encoding format of body.");
                msg.Priority = MailPriority.High;
                logger.Info("Success to set the mail's priority.");
                object userState = msg;
                try
                {
                    logger.Info("Begin to send mail...");
                    client.Send(msg);
                    logger.Info("Success to send mail.");
                }
                catch (Exception ex)
                {
                    logger.Error("Fail to send mail. " + ex.Message);
                }
            }
            catch (Exception ex1)
            {
                logger.Error("Error occurs where sending a mail. " + ex1.Message);
            }
            finally
            {
                msg.Dispose();
            }
        }



    }
}
