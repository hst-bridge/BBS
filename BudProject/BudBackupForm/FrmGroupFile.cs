using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;
using IBLL;
using Common;
using log4net;
using System.Reflection;

namespace BudBackupSystem
{
    public partial class FrmGroupFile : Form
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string msFolderName = "";
        string mServerID = "";
        private FileTypeSet fileTypeSetLists = null;
        private bool gFlag = false;
        public FrmGroupFile()
        {
            InitializeComponent();
        }
        public FrmGroupFile(string msFolderPath, string mServerID)
        {
            InitializeComponent();
            this.msFolderName = msFolderPath;
            this.mServerID = mServerID;
            IFileTypeSetService IFileTypeSetService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
            fileTypeSetLists = IFileTypeSetService.GetFileTypeSetByMonitorServerIdAndFolderName(mServerID, msFolderPath);
            this.lblTitle.Text = msFolderPath;
            if (fileTypeSetLists.id != null && fileTypeSetLists.id != "") 
            {
                if (fileTypeSetLists.exceptAttributeFlg1 == "1")
                {
                    this.chkExceptAttr1.Checked = true;
                }
                else 
                {
                    this.chkExceptAttr1.Checked = false;
                }
                if (fileTypeSetLists.exceptAttribute1 != null && fileTypeSetLists.exceptAttribute1 != "") 
                {
                    this.txtExceptAttr1.Text = fileTypeSetLists.exceptAttribute1;
                }
                if (fileTypeSetLists.exceptAttributeFlg2 == "1")
                {
                    this.chkExceptAttr2.Checked = true;
                }
                else
                {
                    this.chkExceptAttr2.Checked = false;
                }
                if (fileTypeSetLists.exceptAttribute2 != null && fileTypeSetLists.exceptAttribute2 != "")
                {
                    this.txtExceptAttr2.Text = fileTypeSetLists.exceptAttribute2;
                }
                if (fileTypeSetLists.exceptAttributeFlg3 == "1")
                {
                    this.chkExceptAttr3.Checked = true;
                }
                else
                {
                    this.chkExceptAttr3.Checked = false;
                }
                if (fileTypeSetLists.exceptAttribute3 != null && fileTypeSetLists.exceptAttribute3 != "")
                {
                    this.txtExceptAttr3.Text = fileTypeSetLists.exceptAttribute3;
                }
                //if (fileTypeSetLists.systemFileFlg == "1") 
                //{
                //    this.chkSystemFileFlg.Checked = true;
                //}
                //if (fileTypeSetLists.hiddenFileFlg == "1") 
                //{
                //    this.chkHiddenFileFlg.Checked = true;
                //}
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            IFileTypeSetService IFileTypeSetService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
            FileTypeSet fileTypeSet = new FileTypeSet();
            if (msFolderName != "") 
            {
                fileTypeSet.monitorServerFolderName = msFolderName;
            }
            fileTypeSet.monitorServerID = Convert.ToInt32(mServerID);
            fileTypeSet.exceptAttribute1 = "";
            fileTypeSet.exceptAttribute2 = "";
            fileTypeSet.exceptAttribute3 = "";
            fileTypeSet.exceptAttributeFlg1 = "0";
            fileTypeSet.exceptAttributeFlg2 = "0";
            fileTypeSet.exceptAttributeFlg3 = "0";
            fileTypeSet.systemFileFlg = "0";
            fileTypeSet.hiddenFileFlg = "0";
            if (this.chkExceptAttr1.Checked == true && this.txtExceptAttr1.Text.Trim() != "") 
            {
                fileTypeSet.exceptAttributeFlg1 = "1";
            }
            if (this.txtExceptAttr1.Text.Trim() != "")
            {
                fileTypeSet.exceptAttribute1 = this.txtExceptAttr1.Text.Trim();
            }
            if (this.chkExceptAttr2.Checked == true && this.txtExceptAttr2.Text.Trim() != "")
            {
                fileTypeSet.exceptAttributeFlg2 = "1";
            }
            if (this.txtExceptAttr2.Text.Trim() != "")
            {
                fileTypeSet.exceptAttribute2 = this.txtExceptAttr2.Text.Trim();
            }
            if (this.chkExceptAttr3.Checked == true && this.txtExceptAttr3.Text.Trim() != "")
            {
                fileTypeSet.exceptAttributeFlg3 = "1";
            }
            if (this.txtExceptAttr3.Text.Trim() != "")
            {
                fileTypeSet.exceptAttribute3 = this.txtExceptAttr3.Text.Trim();
            }
            //if (this.chkSystemFileFlg.Checked == true) 
            //{
            //    fileTypeSet.systemFileFlg = "1";
            //}
            //if (this.chkHiddenFileFlg.Checked == true)
            //{
            //    fileTypeSet.hiddenFileFlg = "1";
            //}
            fileTypeSet.deleteFlg = 0;
            fileTypeSet.creater = FrmMain.userinfo.loginID;
            fileTypeSet.createDate = CommonUtil.DateTimeNowToString();
            fileTypeSet.updater = FrmMain.userinfo.loginID;
            fileTypeSet.updateDate = CommonUtil.DateTimeNowToString();
            
            FileTypeSet fts = IFileTypeSetService.GetFileTypeSetByMonitorServerIdAndFolderName(mServerID, msFolderName);
            if (fts.id != "" && fts.id != null)
            {
                fileTypeSet.id = fts.id;
                int updateFlg = -1;
                if (MsgHelper.QuestionMsg(ValidationRegex.Q002, ValidationRegex.publicTitle))
                {
                    try
                    {
                        updateFlg = IFileTypeSetService.UpdateFileTypeSet(fileTypeSet);
                    }
                    catch (Exception ex) 
                    {
                        logger.Error(ex.Message);
                    }
                }
                if (updateFlg > -1)
                {
                    MsgHelper.InfoMsg(ValidationRegex.U001, ValidationRegex.publicTitle);
                    gFlag = true;
                    this.Dispose();
                }
                else
                {
                    MsgHelper.InfoMsg(ValidationRegex.U002, ValidationRegex.publicTitle);
                }
            }
            else
            {
                int insertFlg = -1;
                if (this.txtExceptAttr1.Text.Trim() != ""
                    || this.txtExceptAttr2.Text.Trim() != ""
                    || this.txtExceptAttr3.Text.Trim() != ""
                    || this.chkHiddenFileFlg.Checked == true
                    || this.chkSystemFileFlg.Checked == true) 
                {
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q002, ValidationRegex.publicTitle))
                    {
                        try
                        {
                            insertFlg = IFileTypeSetService.InsertFileTypeSet(fileTypeSet);
                        }
                        catch (Exception ex) 
                        {
                            logger.Error(ex.Message);
                        }
                    }
                }
                if (this.txtExceptAttr1.Text.Trim() == "" 
                    && this.txtExceptAttr2.Text.Trim() == "" 
                    && this.txtExceptAttr3.Text.Trim() == ""
                    || this.chkHiddenFileFlg.Checked == true
                    || this.chkSystemFileFlg.Checked == true) 
                {
                    insertFlg = 0;
                }
                if (insertFlg > -1)
                {
                    MsgHelper.InfoMsg(ValidationRegex.I001, ValidationRegex.publicTitle);
                    gFlag = true;
                    this.Dispose();
                }
                else
                {
                    MsgHelper.InfoMsg(ValidationRegex.I002, ValidationRegex.publicTitle);
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string sysChk = "0";
            string hidChk = "0";
            if (this.chkSystemFileFlg.Checked == true)
            {
                sysChk = "1";
            }
            if (this.chkSystemFileFlg.Checked == true)
            {
                hidChk = "1";
            }
            string chkExceptAttrFlg1 = this.chkExceptAttr1.Checked == true ? "1" : "0";
            string chkExceptAttrFlg2 = this.chkExceptAttr2.Checked == true ? "1" : "0";
            string chkExceptAttrFlg3 = this.chkExceptAttr3.Checked == true ? "1" : "0";
            if ((fileTypeSetLists.id == "" || fileTypeSetLists.id == null) 
                && ((this.chkExceptAttr1.Checked == true || this.txtExceptAttr1.Text.Trim() != "")
                || (this.chkExceptAttr2.Checked == true || this.txtExceptAttr2.Text.Trim() != "")
                || (this.chkExceptAttr3.Checked == true || this.txtExceptAttr3.Text.Trim() != "")
                || this.chkSystemFileFlg.Checked == true || this.chkHiddenFileFlg.Checked == true))
            {
                if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                {
                    gFlag = true;
                }
            }
            else if ((fileTypeSetLists.id != "" && fileTypeSetLists.id != null) && (fileTypeSetLists.exceptAttribute1 != this.txtExceptAttr1.Text.Trim()
                || fileTypeSetLists.exceptAttributeFlg1 != chkExceptAttrFlg1
                || fileTypeSetLists.exceptAttribute2 != this.txtExceptAttr2.Text.Trim()
                || fileTypeSetLists.exceptAttributeFlg2 != chkExceptAttrFlg2
                || fileTypeSetLists.exceptAttribute3 != this.txtExceptAttr3.Text.Trim()
                || fileTypeSetLists.exceptAttributeFlg3 != chkExceptAttrFlg3
                || fileTypeSetLists.systemFileFlg != sysChk || fileTypeSetLists.hiddenFileFlg != hidChk))
            {
                if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                {
                    gFlag = true;
                }
            }
            else 
            {
                gFlag = true;
            }
            if (gFlag) 
            {
                this.Dispose();
            }
        }

        private void FrmGroupFile_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FrmGroupFile_FormClosing(object sender, FormClosingEventArgs e)
        {
            string sysChk = "0";
            string hidChk = "0";
            if (this.chkSystemFileFlg.Checked == true) 
            {
                sysChk = "1";
            }
            if (this.chkSystemFileFlg.Checked == true)
            {
                hidChk = "1";
            }
            string chkExceptAttrFlg1 = this.chkExceptAttr1.Checked == true ? "1" : "0";
            string chkExceptAttrFlg2 = this.chkExceptAttr2.Checked == true ? "1" : "0";
            string chkExceptAttrFlg3 = this.chkExceptAttr3.Checked == true ? "1" : "0";
            if (gFlag == false)
            {
                if ((fileTypeSetLists.id == "" || fileTypeSetLists.id == null)
                && ((this.chkExceptAttr1.Checked == true || this.txtExceptAttr1.Text.Trim() != "")
                || (this.chkExceptAttr2.Checked == true || this.txtExceptAttr2.Text.Trim() != "")
                || (this.chkExceptAttr3.Checked == true || this.txtExceptAttr3.Text.Trim() != "")
                || this.chkSystemFileFlg.Checked == true || this.chkHiddenFileFlg.Checked == true))
                {
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else if ((fileTypeSetLists.id != "" && fileTypeSetLists.id != null) && (fileTypeSetLists.exceptAttribute1 != this.txtExceptAttr1.Text.Trim()
                || fileTypeSetLists.exceptAttributeFlg1 != chkExceptAttrFlg1
                || fileTypeSetLists.exceptAttribute2 != this.txtExceptAttr2.Text.Trim()
                || fileTypeSetLists.exceptAttributeFlg2 != chkExceptAttrFlg2
                || fileTypeSetLists.exceptAttribute3 != this.txtExceptAttr3.Text.Trim()
                || fileTypeSetLists.exceptAttributeFlg3 != chkExceptAttrFlg3
                || fileTypeSetLists.systemFileFlg != sysChk || fileTypeSetLists.hiddenFileFlg != hidChk))
                {
                    if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}
