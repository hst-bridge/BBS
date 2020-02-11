using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudFileListen.Entities;

namespace BudFileListen
{
    public class DirectoryNameComparer : IEqualityComparer<monitorServerFolder>
    {
        public bool Equals(monitorServerFolder x, monitorServerFolder y)
        {
            if (x == null)
                return y == null;
            return (x.monitorFilePath == y.monitorFilePath);
        }

        public int GetHashCode(monitorServerFolder obj)
        {
            if (obj == null)
                return 0;
            return obj.monitorFilePath.GetHashCode();
        }
    }
}
