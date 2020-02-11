using DiskSpaceChecker.Common.Helper;
using DiskSpaceChecker.Common.Util;
using DiskSpaceChecker.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiskSpaceChecker.BLL
{
    /// <summary>
    /// 操作
    /// </summary>
    class OperationController
    {
        private static bool IsStoping = false;
        private static Thread MainThread = null;
        public void Stop()
        {
            IsStoping = true;
            Thread.Sleep(2000);
        }

        public void Check(Config config)
        {
            ConfigController configCtrl = new ConfigController();
            configCtrl.Save(config);

            config = configCtrl.Get();

            if (MainThread != null) MainThread.Start();
            //开始监控
            new Thread(() => {
                IsStoping = false;
                while (true)
                {
                    if (IsStoping) break;
                    string syncTime = "checkTime".AppSetting();
                    if (TimeCheckHelper.CheckTime("check", syncTime))
                    {
                        //判断
                        if (!CheckDisk(config))
                        {
                            string content = MessageUtil.LocalIPAddress() + ":" + config.Volume;
                            new SendMail().SendEmailWithSubject(content, "This disk space is insufficient");
                        }
                    }

                    Thread.Sleep(2000);
                }

            }).Start();
        }

        /// <summary>
        /// 检查target目录所在盘区空间是否充足
        /// </summary>
        /// <returns></returns>
        public static bool CheckDisk(Config config)
        {
            double baseline = Convert.ToDouble(config.Baseline);
            //是否大于设置线
            return DiskUtil.GetFreeSpace(config.Volume) > baseline;
        }
    }
}
