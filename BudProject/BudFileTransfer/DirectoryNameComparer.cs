using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BudFileTransfer
{
    public class DirectoryNameComparer : IEqualityComparer<MonitorServerFile>
    {
        public bool Equals(MonitorServerFile x, MonitorServerFile y)
        {
            if (x == null)
                return y == null;
            return (x.monitorFileStatus == y.monitorFileStatus && x.monitorFileDirectory == y.monitorFileDirectory);
        }

        public int GetHashCode(MonitorServerFile obj)
        {
            if (obj == null)
                return 0;
            return obj.monitorFileDirectory.GetHashCode();
        }
    }
}
