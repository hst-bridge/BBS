using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudBackupSystem.util;
using Model;
using Common;
using IBLL;
using log4net;
using System.Reflection;

namespace BudBackupSystem
{
    public partial class FrmGroupDetail : Form
    {
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string id = "";
        //combobox changed flag
        private bool changed = false; 
        public FrmGroupDetail()
        {
            InitializeComponent();
        }
        public FrmGroupDetail(string id)
        {
            this.id = id;
            InitializeComponent();
        }
        private void FrmGroupDetail_Load(object sender, EventArgs e)
        {
            //init the backup group select tab
            IBackupServerGroupService backGroupList = BLLFactory.ServiceAccess.CreateBackupServerGroupService();
            IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
            IList<BackupServerGroup> bsgList = backGroupList.GetBackupServerGroupList();
            List<ComboBoxItem> cbiList = new List<ComboBoxItem>();
            int index = 0;
            int i = 0; 
            foreach(BackupServerGroup bgs in bsgList){
                if (bgs.id == id) 
                {
                    index = i;
                }
                cbiList.Add(new ComboBoxItem(bgs.id.ToString(),bgs.backupServerGroupName));
                i++;
            }
            this.cobBKServerGroup.DisplayMember = "Text";
            this.cobBKServerGroup.ValueMember = "Value";
            this.cobBKServerGroup.DataSource = cbiList;
            
            //set the combobox default value;
            this.cobBKServerGroup.SelectedIndex = index;
            
            //not allowed user add new row to datagridview
            this.dgrdMonitorServer.AllowUserToAddRows = false;
            
            IList<BackupServer> gbsList = bs.GetGroupBackupServerList(this.cobBKServerGroup.SelectedValue.ToString());
            foreach (BackupServer gbs in gbsList)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in dgrdMonitorServer.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = gbs.id;
                dgvr.Cells[1].Value = gbs.backupServerName;
                this.dgrdMonitorServer.Rows.Add(dgvr);
            }


            //not allowed user add new row to datagridview
            this.dgrdBackupServer.AllowUserToAddRows = false;

            IList<BackupServer> bsList = bs.GetPartBackupServerList(this.cobBKServerGroup.SelectedValue.ToString());
            foreach (BackupServer bserver in bsList)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in dgrdBackupServer.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                }
                dgvr.Cells[0].Value = bserver.id;
                dgvr.Cells[1].Value = bserver.backupServerName;
                this.dgrdBackupServer.Rows.Add(dgvr);
            }
            changed = true;
        }

        private void cobBKServerGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changed)
            {
                //get the combobox selected value
                string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();
                
                IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
                //remove the monitor current row
                //RowNumber = this.dgrdMonitorServer.CurrentCell.RowIndex;
                //dgrdMonitorServer.Rows.RemoveAt(RowNumber);

                //empty the monitor rows
                this.dgrdMonitorServer.Rows.Clear();
                this.dgrdMonitorServer.AllowUserToAddRows = false;

                IList<BackupServer> gbsList = bs.GetGroupBackupServerList(selectGroupId);
                foreach (BackupServer gbs in gbsList)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    foreach (DataGridViewColumn c in dgrdMonitorServer.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                    }
                    dgvr.Cells[0].Value = gbs.id;
                    dgvr.Cells[1].Value = gbs.backupServerName;
                    this.dgrdMonitorServer.Rows.Add(dgvr);
                }


                //empty the monitor rows
                this.dgrdBackupServer.Rows.Clear();
                this.dgrdBackupServer.AllowUserToAddRows = false;

                IList<BackupServer> gbpList = bs.GetPartBackupServerList(selectGroupId);
                foreach (BackupServer gbs in gbpList)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    foreach (DataGridViewColumn c in dgrdBackupServer.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                    }
                    dgvr.Cells[0].Value = gbs.id;
                    dgvr.Cells[1].Value = gbs.backupServerName;
                    this.dgrdBackupServer.Rows.Add(dgvr);
                }
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            //save or delete group detail data
            //get the selected row data from dgrdMonitorServer
            //int RowNumber;
            if (null == dgrdMonitorServer.CurrentCell)
            {
                return;
            }
            string monitorServerId = this.dgrdMonitorServer.CurrentRow.Cells[0].Value.ToString();
            string monitorServerName = this.dgrdMonitorServer.CurrentRow.Cells[1].Value.ToString();
            //get the combobox selected value
            string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();

            //empty the monitor rows
            this.dgrdMonitorServer.AllowUserToAddRows = false;
            this.dgrdMonitorServer.Rows.RemoveAt(this.dgrdMonitorServer.CurrentCell.RowIndex);

            //empty the monitor rows
            this.dgrdBackupServer.AllowUserToAddRows = false;
            this.dgrdBackupServer.Rows.Add(monitorServerId, monitorServerName);
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            //save or delete group detail data
            //get the selected row data from dgrdBackupServer
            if (null == this.dgrdBackupServer.CurrentCell)
            {
                return;
            }
            BackupServerGroupDetail bsgd = new BackupServerGroupDetail();
            string backupServerId = dgrdBackupServer.CurrentRow.Cells[0].Value.ToString();
            string backupServerName = dgrdBackupServer.CurrentRow.Cells[1].Value.ToString();
            //get the combobox selected value
            string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();
            IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();

            //empty the monitor rows
            this.dgrdMonitorServer.AllowUserToAddRows = false;
            this.dgrdMonitorServer.Rows.Add(backupServerId, backupServerName);
            

            //empty the monitor rows
            this.dgrdBackupServer.AllowUserToAddRows = false;
            this.dgrdBackupServer.Rows.RemoveAt(this.dgrdBackupServer.CurrentCell.RowIndex);


        }
        

        private void FrmGroupDetail_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void btnGroupDetailSave_Click(object sender, EventArgs e)
        {
            IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
            IBackupServerGroupDetailService backGroupDetail = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
            //get the combobox selected value
            string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();
            if (MsgHelper.QuestionMsg(ValidationRegex.Q002, ValidationRegex.publicTitle))
            {
                int delFlg = backGroupDetail.DeleteBackupServerGroupDetailByGroupId(Convert.ToInt32(selectGroupId), FrmMain.userinfo.loginID);
                if (delFlg > -1)
                {
                    for (int i = 0; i < this.dgrdMonitorServer.Rows.Count; i++)
                    {
                        BackupServerGroupDetail bsgd = new BackupServerGroupDetail();
                        string backupServerId = this.dgrdMonitorServer.Rows[i].Cells[0].Value.ToString();
                        bsgd.backupServerGroupId = Convert.ToInt32(selectGroupId);
                        bsgd.backupServerId = Convert.ToInt32(backupServerId);
                        bsgd.creater = FrmMain.userinfo.loginID;
                        bsgd.createDate = DateTime.Now.ToString();
                        bsgd.updater = FrmMain.userinfo.loginID;
                        bsgd.updateDate = DateTime.Now.ToString();
                        int insertFlg = backGroupDetail.InsertBackupServerGroupDetail(bsgd);
                        if (insertFlg > -1)
                        {
                            MsgHelper.InfoMsg(ValidationRegex.I001, ValidationRegex.publicTitle);
                            this.Dispose();
                        }
                        else
                        {
                            MsgHelper.InfoMsg(ValidationRegex.I002, ValidationRegex.publicTitle);
                        }
                    }
                }
            }
        }

        private void btnGroupDetailList_Click(object sender, EventArgs e)
        {

        }

        private void dgrdMonitorServer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnRight_Click(sender, e);
        }
        private void dgrdBackupServer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnLeft_Click(sender, e);
        }
        private void btnRight_Click_Copy(object sender, EventArgs e)
        {
            //save or delete group detail data
            //get the selected row data from dgrdMonitorServer
            //int RowNumber;
            if (null == dgrdMonitorServer.CurrentCell)
            {
                return;
            }

            IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
            IBackupServerGroupDetailService backGroupDetail = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
            BackupServerGroupDetail bsgd = new BackupServerGroupDetail();
            string monitorServerId = dgrdMonitorServer.CurrentRow.Cells[0].Value.ToString();
            //get the combobox selected value
            string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();

            bsgd.backupServerGroupId = Convert.ToInt32(selectGroupId);
            bsgd.backupServerId = Convert.ToInt32(monitorServerId);
            int flag = backGroupDetail.DeleteBackupServerGroupDetail(bsgd.backupServerId, bsgd.backupServerGroupId, FrmMain.userinfo.loginID);
            if (flag > -1)
            {
                //remove the monitor current row
                //RowNumber = this.dgrdMonitorServer.CurrentCell.RowIndex;
                //dgrdMonitorServer.Rows.RemoveAt(RowNumber);

                //empty the monitor rows
                this.dgrdMonitorServer.Rows.Clear();
                this.dgrdMonitorServer.AllowUserToAddRows = false;

                IList<BackupServer> gbsList = bs.GetGroupBackupServerList(selectGroupId);
                foreach (BackupServer gbs in gbsList)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    foreach (DataGridViewColumn c in dgrdMonitorServer.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                    }
                    dgvr.Cells[0].Value = gbs.id;
                    dgvr.Cells[1].Value = gbs.backupServerName;
                    this.dgrdMonitorServer.Rows.Add(dgvr);
                }


                //empty the monitor rows
                this.dgrdBackupServer.Rows.Clear();
                this.dgrdBackupServer.AllowUserToAddRows = false;

                IList<BackupServer> gbpList = bs.GetPartBackupServerList(selectGroupId);
                foreach (BackupServer gbs in gbpList)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    foreach (DataGridViewColumn c in dgrdBackupServer.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                    }
                    dgvr.Cells[0].Value = gbs.id;
                    dgvr.Cells[1].Value = gbs.backupServerName;
                    this.dgrdBackupServer.Rows.Add(dgvr);
                }
            }
        }
        private void btnLeft_Click_Copy(object sender, EventArgs e)
        {
            //save or delete group detail data
            //get the selected row data from dgrdBackupServer
            if (null == dgrdBackupServer.CurrentCell)
            {
                return;
            }
            BackupServerGroupDetail bsgd = new BackupServerGroupDetail();
            string backupServerId = dgrdBackupServer.CurrentRow.Cells[0].Value.ToString();
            //get the combobox selected value
            string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();
            bsgd.backupServerGroupId = Convert.ToInt32(selectGroupId);
            bsgd.backupServerId = Convert.ToInt32(backupServerId);
            bsgd.creater = FrmMain.userinfo.loginID;
            bsgd.createDate = DateTime.Now.ToString();
            bsgd.updater = FrmMain.userinfo.loginID;
            bsgd.updateDate = DateTime.Now.ToString();
            IBackupServerService bs = BLLFactory.ServiceAccess.CreateBackupServer();
            IBackupServerGroupDetailService backGroupDetail = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
            int flag = backGroupDetail.InsertBackupServerGroupDetail(bsgd);
            if (flag > -1)
            {

                //empty the monitor rows
                this.dgrdMonitorServer.Rows.Clear();
                this.dgrdMonitorServer.AllowUserToAddRows = false;

                IList<BackupServer> gbsList = bs.GetGroupBackupServerList(selectGroupId);
                foreach (BackupServer gbs in gbsList)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    foreach (DataGridViewColumn c in dgrdMonitorServer.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                    }
                    dgvr.Cells[0].Value = gbs.id;
                    dgvr.Cells[1].Value = gbs.backupServerName;
                    this.dgrdMonitorServer.Rows.Add(dgvr);
                }

                //empty the monitor rows
                this.dgrdBackupServer.Rows.Clear();
                this.dgrdBackupServer.AllowUserToAddRows = false;

                IList<BackupServer> gbpList = bs.GetPartBackupServerList(selectGroupId);
                foreach (BackupServer gbs in gbpList)
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    foreach (DataGridViewColumn c in dgrdBackupServer.Columns)
                    {
                        dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);//add row cells
                    }
                    dgvr.Cells[0].Value = gbs.id;
                    dgvr.Cells[1].Value = gbs.backupServerName;
                    this.dgrdBackupServer.Rows.Add(dgvr);
                }
            }
        }

        private void FrmGroupDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            string selectGroupId = this.cobBKServerGroup.SelectedValue.ToString();
            IBackupServerGroupDetailService backGroupDetail = BLLFactory.ServiceAccess.CreateBackupServerGroupDetailService();
            IList<BackupServerGroupDetail> bsgdLists = backGroupDetail.GetBackupServerGroupDetailByGroupId(selectGroupId);
            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();
            foreach (BackupServerGroupDetail bsgd in bsgdLists) 
            {
                list1.Add(bsgd.backupServerId.ToString());
            }
            for (int i = 0; i < this.dgrdMonitorServer.Rows.Count; i++) 
            {
                list2.Add(this.dgrdMonitorServer.Rows[i].Cells[0].Value.ToString());
            }
            if (list1.Count != list2.Count) 
            {
                //if (MsgHelper.QuestionMsg(ValidationRegex.Q003, ValidationRegex.publicTitle))
                //{
                //    e.Cancel = false;
                //}
                //else 
                //{
                //    e.Cancel = true;
                //}
            }
            else
            {
                bool flag = false;
                foreach (string i in list1) 
                {
                    if (!list2.Contains(i)) {
                        flag = true;
                        break;
                    }
                }
                if (flag) 
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

        private void btnGroupDetailClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void dgrdMonitorServer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            this.pictureBox1.Image = Properties.Resources.addBtnOn;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox1.Image = Properties.Resources.addBtn;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            this.pictureBox2.Image = Properties.Resources.removeBtnOn;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox2.Image = Properties.Resources.removeBtn;
        }
    }
}
