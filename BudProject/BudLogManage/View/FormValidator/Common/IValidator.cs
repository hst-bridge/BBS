using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.View.FormValidator.Common
{
    interface IValidator
    {
        bool Validate(out string message);
    }
}
