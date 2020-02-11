using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BudLogManage.Common.Util
{
    public class FolderDialog : FolderNameEditor
    {
        FolderNameEditor.FolderBrowser fDialog = new
        System.Windows.Forms.Design.FolderNameEditor.FolderBrowser();
        public FolderDialog()
        {
        }
        public DialogResult DisplayDialog()
        {
            return DisplayDialog("選択フォルダ");
        }

        public DialogResult DisplayDialog(string description)
        {
            fDialog.Description = description;
            return fDialog.ShowDialog();
        }
        public string Path
        {
            get
            {
                return fDialog.DirectoryPath;
            }
        }
        ~FolderDialog()
        {
            fDialog.Dispose();
        }
    }
}
