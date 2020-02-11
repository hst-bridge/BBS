using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using System.Reflection;

namespace BudBackupSystem.util
{
    public class OpaqueCommand
    {
        private MyOpaqueLayer m_OpaqueLayer = null;//半透明蒙板层
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="alpha">透明度</param>
        /// <param name="isShowLoadingImage">是否显示图标</param>
        public void ShowOpaqueLayer(Control control, int alpha, bool isShowLoadingImage)
        {
            try
            {
                if (this.m_OpaqueLayer == null)
                {
                    this.m_OpaqueLayer = new MyOpaqueLayer(alpha, isShowLoadingImage);
                    control.Controls.Add(this.m_OpaqueLayer);
                    this.m_OpaqueLayer.Dock = DockStyle.Fill;
                    this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }
            catch(Exception ex) 
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        public void HideOpaqueLayer()
        {
            try
            {
                if (this.m_OpaqueLayer != null)
                {
                    this.m_OpaqueLayer.Visible = false;
                    this.m_OpaqueLayer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

    }
}
