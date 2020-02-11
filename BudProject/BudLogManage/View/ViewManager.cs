using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BudLogManage.View
{
    /// <summary>
    /// ビューを切り替え
    /// </summary>
    class ViewManager
    {
        //Parent Control
        private Control parent;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parent"></param>
        public ViewManager(Control parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// load the user control
        /// </summary>
        /// <param name="usercontrol"></param>
        public void Load(ViewControl usercontrol)
        {
            try
            {
                this.parent.Controls.Clear();
                usercontrol.ViewLocation = this;
                this.parent.Controls.Add(usercontrol);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// switch the user control
        /// </summary>
        /// <param name="usercontrol"></param>
        public void SwitchTo(ViewControl usercontrol)
        {
            try
            {
                this.parent.Controls.Clear();
                usercontrol.ViewLocation = this;
                this.parent.Controls.Add(usercontrol);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
