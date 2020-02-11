using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BudDBSync.FormValidator.Common;
using BudDBSync.FormValidator.Common.Rules;

namespace BudDBSync.FormValidator
{
    /// <summary>
    /// valid the textbox input
    /// </summary>
    class DBEDITValidator : IValidator
    {
        private Control parentControl = null;
        private List<TextBoxSetting> settings = null;
        public DBEDITValidator(Control parent)
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
                    settings = new List<TextBoxSetting>()
                    {
                        new TextBoxSetting(){Name="textBox1", Rules=new List<IRule>(){
                            new Required("サーバー名を入力してください"),

                        }},
                        new TextBoxSetting(){Name="textBox2", Rules=new List<IRule>(){
                            new Required("ログイン名を入力してください"),

                        }},
                        new TextBoxSetting(){Name="textBox3", Rules=new List<IRule>(){
                            new Required("パスワードを入力してください"),

                        }},
                         new TextBoxSetting(){Name="textBox4", Rules=new List<IRule>(){
                            new Required("データベース名を入力してください"),

                        }},
                    };
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
