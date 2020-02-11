using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace BudFileCheckListen.Common
{
    public class AppConfig
    {
        //--磁盘空间不足警告线 或者 最小磁盘空间
        public double getSpaceWarnline()
        {
            string line = getAppSetting("SpaceWarnline");
            double threshold = 0;
            if (line.EndsWith("%"))
            {
                line = line.TrimEnd('%');
                threshold = Convert.ToDouble(line) / 100;
            }
            else
            {
                threshold = Convert.ToDouble(line);
            }
            return threshold;
        }

        //------------邮件设置信息-----------------------------------------------------------------------

        //--data邮件发送到
        public String getEmailDataSendTo()
        {
            return getAppSetting("DataSendTo");
        }

        //--data邮件CC到
        public String getEmailDataSendCC()
        {
            string cc = string.Empty;
            try
            {
                cc = getAppSetting("DataSendCC");
            }
            catch { }
            return cc;
        }

        //--data主题
        public String getMailDataSubject()
        {
            return getAppSetting("DataSubject");
        }

        //--用户名
        public String getEmailUser()
        {
            return getAppSetting("EmailUser");
        }
        public void setEmailUser(String emailuser)
        {
            setAppSetting("EmailUser", emailuser);
        }


        //--密码
        public String getEmailPwd()
        {
            return getAppSetting("EmailPwd");
        }
        public void setEmailPwd(String emailpwd)
        {
            setAppSetting("EmailPwd", emailpwd);
        }


        //--SMTP
        public String getSMTP()
        {
            return getAppSetting("smtp");
        }
        public void setSMTP(String smtp)
        {
            setAppSetting("smtp", smtp);
        }


        //-------共同调用部分-------------
        private String getAppSetting(String key)
        {
            try
            {
                String str = System.Configuration.ConfigurationManager.AppSettings[key];
                if (String.IsNullOrWhiteSpace(str))
                {
                    throw new Exception();
                }
                return str;
            }
            catch (Exception e)
            {
                throw new Exception("Fail to load the set:" + key + "; ", e);
            }

        }
        private void setAppSetting(String key, String value)
        {
            
        }
    }
}
