using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common
{
    public class MsgHelper
    {
        /// <summary>
        /// 情報メッセージ
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="title">タイトル</param>
        public static void InfoMsg(string txt, string title)
        {
            MessageBox.Show(txt, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="title">タイトル</param>
        public static void ErrorMsg(string txt, string title)
        {
            MessageBox.Show(txt, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Warningメッセージ
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="title">タイトル</param>
        public static void WarningMsg(string txt, string title)
        {
            MessageBox.Show(txt, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 確認メッセージ
        /// </summary>
        /// <param name="txt">内容</param>
        /// <param name="title">タイトル</param>
        public static bool QuestionMsg(string txt, string title)
        {
            if (MessageBox.Show(txt, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
