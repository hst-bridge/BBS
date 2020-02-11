using System.Windows.Forms;
using log4net;
using BudLogManage.View;
using System.Reflection;
using BudLogManage.Common.Util.FormStyle;

namespace BudLogManage
{
    public partial class FrmMain : Form
    {
        #region private
        private readonly ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ViewManager viewManager;
        internal ViewManager ViewManager
        {
            get
            {
                if (viewManager == null)
                {
                    viewManager = new ViewManager(this.panel3);
                }
                return viewManager;
            }
            set { viewManager = value; }

        }

        #endregion
        public FrmMain()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, System.EventArgs e)
        {
            ViewManager.SwitchTo(new Configure());
        }

        private void FrmMain_Load(object sender, System.EventArgs e)
        {
            //disabled the maximize
            FormStyleUtil.RemoveMaximizeMenu(this);
            ViewManager.Load(new Configure());
        }

        private void label3_Click(object sender, System.EventArgs e)
        {
            viewManager.SwitchTo(new Action());
        }

        private void label4_Click(object sender, System.EventArgs e)
        {
            viewManager.SwitchTo(new OverView());
        }
    }
}
