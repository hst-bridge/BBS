using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace BudFileListen
{
    public static class GlobalVariable
    {
        // バックアップ用の全体（バックアップThread List）
        public static Hashtable listThread = new Hashtable();
        // バックアップ用の全体（Threadの下にバックアップフォルダーの对象 List）
        public static Hashtable listListenThread = new Hashtable();
        // 被停止的バックアップ对象 List
        public static Hashtable stopListenThread = new Hashtable();
    }
}
