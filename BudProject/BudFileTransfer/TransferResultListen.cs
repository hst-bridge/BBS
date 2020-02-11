using System;
using System.IO;
using System.Collections;
using System.Threading;
using log4net;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using IBLL;
using Model;

namespace BudFileTransfer
{
    public class TransferResultListen
    {
        /// <summary>
        /// monitorid
        /// </summary>
        private string _monitorid;
        /// <summary>
        /// monitorid
        /// </summary>
        private string _backupgroupid;
        /// <summary>
        /// 時間コンポーネント
        /// </summary>
        System.Timers.Timer m_timer = null;
        /// <summary>
        /// コンポーネント起動判断
        /// </summary>
        public bool EnableRaisingEvents { get; set; }
        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// ログ
        /// </summary>
        private IMonitorServerFileService MonitorServerFileService = BLLFactory.ServiceAccess.CreateMonitorServerFileService();
        /// <summary>
        /// TransferNum
        /// </summary>
        private int transferNum = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["TransferNum"]);

        /// <summary>
        /// 初期化
        /// </summary>
        public TransferResultListen(string monitorid, string backupgroupid, int timecount)
        {
            //time処理の設定、ここで起動していない状態です。
            _monitorid = monitorid;
            _backupgroupid = backupgroupid;
            m_timer = new System.Timers.Timer(timecount);
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(WatchStart);
            m_timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WatchStart(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (EnableRaisingEvents)
                {
                    m_timer.Enabled = false;
                    TransferResultCheck();
                    m_timer.Enabled = true;
                }
            }
            catch (Exception exb)
            {
                logger.Error(exb.Message);
                m_timer.Enabled = true;
            }
        }

        /// <summary>
        /// 転送ファイルのチェック
        /// </summary>
        /// <param name="dirs"></param>
        private void TransferResultCheck()
        {
            // 転送失敗のファイルリスト
            IList<MonitorServerFile> MonitorServerFile = MonitorServerFileService.GetTransferNGFileList(_monitorid);
            // リストのループ処理
            foreach (MonitorServerFile fileInfo in MonitorServerFile)
            {
                try
                {
                    if (fileInfo.transferFlg == 2)
                    {
                        if (fileInfo.transferNum > 0)
                        {
                            int transferUpdateNum = fileInfo.transferNum - 1;
                            MonitorServerFileService.UpdateNGTransferFlg(fileInfo.id, 0, transferUpdateNum);
                        }
                        else
                        {
                            // メール送信
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    continue;
                }
            }
        }
    }
}
