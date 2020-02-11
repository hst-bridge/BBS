using BudSSH.BLL;
using BudSSH.Common.Util;
using BudSSH.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler;

namespace BudSSH.Controller
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    public class TaskController
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        const string TaskName = "BudSSHBootTask";

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 启动任务
        /// </summary>
        public void BootTask()
        {
            try
            {
                var importCts = new CancellationTokenSource();
                var threadTask = new Task(new Action(() =>
                {
                    //任务不存在，则创建
                    if (!IsTaskExist())
                    {
                        CreateTask();
                    }

                    //任务被关闭时，再启动
                    StartTask();

                    //执行完取消任务
                    importCts.Cancel();

                }), importCts.Token);

                threadTask.Start();
            }
            catch (System.Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        void CreateTask()
        {
            try
            {
                string creator = "HST";
                string taskName = TaskName;
                string path = Process.GetCurrentProcess().MainModule.FileName;
                string interval = "PT24H0M";

                //new scheduler
                TaskSchedulerClass scheduler = new TaskSchedulerClass();
                //pc-name/ip,username,domain,password
                scheduler.Connect(null, null, null, null);
                //get scheduler folder
                ITaskFolder folder = scheduler.GetFolder("\\");


                //set base attr 
                ITaskDefinition task = scheduler.NewTask(0);
                task.RegistrationInfo.Author = creator;//creator
                task.RegistrationInfo.Description = "Boot BudSSH";//description
                task.RegistrationInfo.Date = DateTimeUtil.GetTaskFormatTime(DateTime.Now);

                //set trigger  (IDailyTrigger ITimeTrigger)
                ITimeTrigger tt = (ITimeTrigger)task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
                tt.Repetition.Interval = interval;// format PT1H1M==1小时1分钟 设置的值最终都会转成分钟加入到触发器

                Config config = ConfigManager.GetCurrentConfig();
                var date = DateTime.Parse(config.SSHBootTime);

                tt.StartBoundary = DateTimeUtil.GetTaskFormatTime(date);// "2015-04-09T14:27:25";//start time

                //set action
                IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                action.Path = path;

                task.Settings.ExecutionTimeLimit = "PT0S"; //运行任务时间超时停止任务吗? PTOS 不开启超时
                task.Settings.DisallowStartIfOnBatteries = false;//只有在交流电源下才执行
                task.Settings.RunOnlyIfIdle = false;//仅当计算机空闲下才执行

                IRegisteredTask regTask = folder.RegisterTaskDefinition(taskName, task,
                                                                    (int)_TASK_CREATION.TASK_CREATE, null, //user
                                                                    null, // password
                                                                    _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,
                                                                    "");
                if (regTask.State != _TASK_STATE.TASK_STATE_READY && regTask.State != _TASK_STATE.TASK_STATE_RUNNING)
                {
                    IRunningTask runTask = regTask.Run(null);
                }

                logger.Info("regTask.State: " + regTask.State);

            }
            catch (System.Exception ex)
            {
                logger.Error("Create Task Error");
                throw ex;
            }
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        void StartTask()
        {
            try
            {
                var task = GetTask();
                if (task != null)
                {
                    if (task.State == _TASK_STATE.TASK_STATE_DISABLED)
                    {
                        task.Enabled = true;
                        Thread.Sleep(1000);
                    }
                    if (task.State != _TASK_STATE.TASK_STATE_READY && task.State != _TASK_STATE.TASK_STATE_RUNNING)
                    {
                        IRunningTask runTask = task.Run(null);
                    }
                    logger.Info("task.State: " + task.State);
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex);
            }
        }

        IRegisteredTask GetTask()
        {
            IRegisteredTask task = null;

            TaskSchedulerClass ts = new TaskSchedulerClass();

            ts.Connect(null, null, null, null);

            ITaskFolder folder = ts.GetFolder("\\");

            IRegisteredTaskCollection tasks_exists = folder.GetTasks(1);

            for (int i = 1; i <= tasks_exists.Count; i++)
            {
                IRegisteredTask t = tasks_exists[i];
                if (t.Name == TaskName)
                {
                    task = t;
                    break;
                }
            }

            return task;
        }

        bool IsTaskExist()
        {

            return GetTask() != null;
        }

        /// <summary>
        /// delete task
        /// </summary>
        void DeleteTask()
        {
            TaskSchedulerClass ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);
            ITaskFolder folder = ts.GetFolder("\\");
            folder.DeleteTask(TaskName, 0);
        }
    }
}
