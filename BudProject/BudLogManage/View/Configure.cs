using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudLogManage.View.FormValidator.Common;
using BudLogManage.View.FormValidator;
using BudLogManage.Common.Util;
using BudLogManage.Controller;
using BudLogManage.Model;
using System.Reflection;
using log4net;

namespace BudLogManage.View
{
    internal partial class Configure : ViewControl
    {

        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Configure()
        {
            InitializeComponent();
        }

        private void Configure_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigController cc = new ConfigController();
                Config config = cc.LoadConfig();
                if (!string.IsNullOrWhiteSpace(config.LogPath))
                {
                    
                    this.listBox1.Items.AddRange(config.LogPath.Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries));

                }

                this.cbBoxFolder.SelectedIndex = 0;

                if (!string.IsNullOrWhiteSpace(config.Folder))
                {
                    this.lblFolder.Text = config.Folder;
                }
            }
            catch (System.Exception ex)
            {
                logger.Error(ex.Message);
                MessageBox.Show(MessageUtil.GetMessage("LoadFailed"));
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string prompt = null;
            if (!Validator.Validate(out prompt))
            {
                MessageBox.Show(prompt);
                return;
            }

            ConfigController cc = new ConfigController();
            StringBuilder sb = new StringBuilder();
            if (this.listBox1.Items.Count > 0)
            {
                foreach (var item in this.listBox1.Items)
                {
                    sb.Append(item as string).Append(";");
                }
            }
            if (cc.SaveConfig(new Config() { LogPath = sb.ToString(), Folder = this.lblFolder.Text }))
            {
                MessageBox.Show(MessageUtil.GetMessage("SaveSuccess"));

            }


        }

        private IValidator validator = null;
        internal IValidator Validator
        {
            get
            {
                if (validator == null)
                {
                    validator = new ConfigureValidator(this);
                }
                return validator;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderDialog folderDialog = new FolderDialog();
            if (DialogResult.OK == folderDialog.DisplayDialog())
            {
                //判断是否已经存在
                if (this.listBox1.Items.Count > 5)
                {
                    
                    for (int i=0;i<this.listBox1.Items.Count;i++)
                    {
                        if (folderDialog.Path.Equals(this.listBox1.Items[i] as string))
                        {
                            this.listBox1.SelectedIndex = i;
                            MessageBox.Show(MessageUtil.GetMessage("existed"));
                            return;
                        }
                    }
                }
                
                this.listBox1.Items.Add(folderDialog.Path);
            }
        }

        /// <summary>
        /// 删除path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if(this.listBox1.SelectedIndex>=0)
              this.listBox1.Items.RemoveAt(this.listBox1.SelectedIndex);

        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBoxFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbBoxFolder.SelectedIndex == 1)
            {
                FolderDialog folderDialog = new FolderDialog();
                if (folderDialog.DisplayDialog() == DialogResult.OK)
                {
                    this.lblFolder.Text = folderDialog.Path;
                }
            }
            else if (this.cbBoxFolder.SelectedItem != null)
            {
                this.lblFolder.Text = this.cbBoxFolder.SelectedItem.ToString();
            }
            else
            {
                this.lblFolder.Text = "すべて";
            }
        }
    }
}
