using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Factory;
using BudLogManage.DAL.IBLL;
using System.IO;

namespace BudLogManage.BLL
{
    /// <summary>
    /// 用于任务调度
    /// 
    /// 根据情况调度程序
    /// </summary>
    class TaskScheduler
    {
        private TaskScheduler() { }
        public static readonly TaskScheduler Instance = new TaskScheduler();

        public ILogAnalyser CurrentLogAnalyser
        {
            get;
            set;
        }
        private bool isStoping = false;
        /// <summary>
        /// 启动
        /// </summary>
        public void Execute()
        {
            //根据配置，选择工厂
            IAnalyseFactory IAF = AnalyseFactoryFactory.GetAnalyseFactory(ConfigManager.GetCurrentConfig());

            //获取分析器
            ILogAnalyser ila = IAF.GetLogAnalyser();
            CurrentLogAnalyser = ila;
            new System.Threading.Thread(() =>
            ila.Analyse(ConfigManager.GetCurrentConfig())).Start();

           

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            isStoping = true;
        }

        
    }
}
