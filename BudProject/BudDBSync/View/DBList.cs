using System;
using BudDBSync.BLL;
using System.Windows.Forms;
using System.Data;

namespace BudDBSync.View
{
    internal partial class DBList : ViewControl
    {
        public DBList()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
           
        }
        private DBServerManager dbsm = new DBServerManager();
        
        /// <summary>
        /// データベース元新規作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //switch to DBADD view
            ViewLocation.SwitchTo(new DBADD());
        }

        private void DBList_Load(object sender, EventArgs e)
        {
            try
            {
               
                DataTable dt = dbsm.GetAllSourceInfo();
                if (dt != null && dt.Rows.Count > 0)
                    this.dataGridView1.DataSource = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
           DataGridViewRow dgvr = this.dataGridView1.Rows[e.RowIndex];
           dgvr.Cells["no"].Value = e.RowIndex + 1;
           dgvr.Cells["edit"].Value = "編集";
           dgvr.Cells["delete"].Value = "消除";
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewColumn column = this.dataGridView1.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    if ("delete".Equals(column.Name))
                    {
                        //delete the dbserver
                        //DataRow dr = (this.dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
                        dbsm.DeleteSourceDBServerAt(e.RowIndex);
                        this.dataGridView1.Rows.RemoveAt(e.RowIndex);

                    }

                    if ("edit".Equals(column.Name))
                    {
                        //edit the dbserver
                        ViewLocation.SwitchTo(new DBEDIT(e.RowIndex));
                    }
                    
                }
            }
        }

       
    }
}
