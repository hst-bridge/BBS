using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BudFileTransfer
{
    public static class GlobalVariable
    {
        // バックアップ用の全体（Thread List）
        public static Hashtable listThread = new Hashtable();
        // バックアップ用の全体（Threadの下にTransferのThread List）
        public static Hashtable listTransferThread = new Hashtable();
        // 被停止的バックアップ对象转送 List
        public static Hashtable stopTransferThread = new Hashtable();
    }
}
