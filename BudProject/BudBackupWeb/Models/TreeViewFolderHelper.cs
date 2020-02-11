using DevExpress.Web.ASPxTreeView;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Model;

namespace budbackup.Models
{
    public class TreeViewFolderHelper
    {
        //const string DirImageUrl = "~/Content/TreeView/FileSystem/directory.png";
        const string DirImageUrl = "~/Content/Image/iconFolder.gif";
        const string ServerImageUrl = "~/Content/Image/iconServer.gif";
        public static string strStartPath = string.Empty;
        public static bool bInit = true;

        public static void CreateChildren(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? strStartPath : e.NodeName;
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            if (!bInit)
            {
                return;
            }
            else
            {
                bInit = false;
            }
            if (string.IsNullOrEmpty(e.NodeName))
            {
                TreeViewVirtualNode childNode = new TreeViewVirtualNode(strStartPath, strStartPath);
                childNode.Image.Url = ServerImageUrl;
                children.Add(childNode);
            }
            else if (Directory.Exists(parentNodePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(parentNodePath);
                foreach (DirectoryInfo folder in dirInfo.GetDirectories())
                {
                    //隠しファイル||システムファイル
                    if ((folder.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden || (folder.Attributes & FileAttributes.System) == FileAttributes.System)
                    {
                        continue;
                    }
                    TreeViewVirtualNode childNode = new TreeViewVirtualNode(folder.FullName.ToString(), folder.Name.ToString());
                    childNode.Image.Url = DirImageUrl;
                    childNode.Target = "99";
                    children.Add(childNode);
                }
            }
            e.Children = children;
        }
    }
}
