using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudLogManage.View.FormValidator.Common;
using BudLogManage.View.FormValidator.Common.Rules;

namespace BudLogManage.View.FormValidator
{
    /// <summary>
    /// valid the textbox input
    /// </summary>
    class ConfigureValidator : IValidator
    {
        private Control parentControl = null;
        private List<TextBoxSetting> settings = null;
        public ConfigureValidator(Control parent)
        {
            this.parentControl = parent;
        }

        private List<TextBoxSetting> Settings 
        {
            get
            {
                if (settings == null)
                {
                    #region setting config
                    settings = new List<TextBoxSetting>();
                    //{
                    //    new TextBoxSetting(){Name="textBox1", Rules=new List<IRule>(){
                    //        new Required("RoboCopyログ·パスを入力してください"),

                    //    }},
                    //    new TextBoxSetting(){Name="textBox2", Rules=new List<IRule>(){
                    //        new Required("SSHログ·パスを入力してください"),

                    //    }},
                       
                    //};
                    #endregion
                }

                return settings;
            }
        }
        /// <summary>
        /// valid text inputs and return the message
        /// </summary>
        /// <returns></returns>
        public bool Validate(out string message)
        {
            bool status = true;
            message = string.Empty;
            
            foreach (var setting in Settings)
            {
                string input = ((TextBox)this.parentControl.Controls[setting.Name]).Text;
                foreach (var rule in setting.Rules)
                {
                    status = rule.Validate(input);
                    if (!status)
                    {
                        message = rule.Message;
                        break;
                    }
                }

                if (!status) break;
            }
            
            return status;
        }
    }
}
