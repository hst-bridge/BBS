using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudSSH.Model;

namespace BudSSH.DAL.LogFile
{
    public interface ILogReader : IDisposable
    {
        ErrorPathInfo GetErrorList();
    }
}
