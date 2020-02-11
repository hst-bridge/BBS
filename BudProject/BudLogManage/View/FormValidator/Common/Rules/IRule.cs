﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.View.FormValidator.Common.Rules
{
    /// <summary>
    /// Specification
    /// </summary>
    interface IRule
    {
        string Message { get; set; }
        bool Validate(string input);

    }
}
