using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.View.FormValidator.Common.Rules;

namespace BudLogManage.View.FormValidator.Common
{
    /// <summary>
    /// setting
    /// </summary>
    class TextBoxSetting
    {
        public string Name { get; set; }
        public List<IRule> Rules { get; set; }
    }
}
