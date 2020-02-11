using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.FormValidator.Common.Rules
{
    class Required : IRule
    {
        public string Message { get; set; }
        public Required(string message)
        {
            this.Message = message;
        }

        public bool Validate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            return true;
        }
    }
}
