using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BudLogManage.Common.Helper;

namespace BudLogManage.Model
{
    /// <summary>
    /// 用于合计
    /// </summary>
    public class Total
    {
        public int FilesCopied { get; set; }
        private Size _bytesCopied = null;
        public Size BytesCopied
        {
            get
            {
                if (_bytesCopied == null) _bytesCopied = new Size();
                return _bytesCopied;
            }
            set
            {
                _bytesCopied = value;
            }
        }

        public int FilesDeleted { get; set; }

        private Size _bytesDeleted = null;
        public Size BytesDeleted
        {
            get
            {
                if (_bytesDeleted == null) _bytesDeleted = new Size();
                return _bytesDeleted;
            }
            set
            {
                _bytesDeleted = value;
            }
        }

        public int FileTransfered { get; set; }

        private Size _bytesTransfered = null;
        public Size BytesTransfered
        {
            get
            {
                if (_bytesTransfered == null) _bytesTransfered = new Size();
                return _bytesTransfered;
            }

            set
            {
                _bytesTransfered = value;
            }
        }

        public int FileTransDeleted { get; set; }

        private Size _bytesTransDeleted { get; set; }
        public Size BytesTransDeleted
        {
            get
            {
                if (_bytesTransDeleted == null) _bytesTransDeleted = new Size();

                return _bytesTransDeleted;
            }
            set
            {
                _bytesTransDeleted = value;
            }
        }

        public TimeInterval TimeInterval { get; set; }
    }
}
