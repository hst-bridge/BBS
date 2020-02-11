using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Model;

namespace BudLogManage.DAL.IDAL
{
   public interface ILogReader
    {
       Operation ReadBlock();
       bool EndOfStream { get; }
       string Path { get; set; }
       void Close();

    }
}
