using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web.ASPxTreeView;
using System.IO;
using Model;

namespace budbackup.Models
{
    public class TreeViewPathSelector
    {
        const string FileImageUrl = "~/Content/TreeView/FileSystem/file.png";
        const string DirImageUrl = "~/Content/Image/iconFolder.gif";
        const string ServerImageUrl = "~/Content/Image/iconServer.gif";
        static HttpRequest Request { get { return HttpContext.Current.Request; } }
        public static string strStartPath = string.Empty;
        public static bool bInit = true;
        public static string msID = string.Empty;
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
                IBLL.IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
                FileTypeSet fset = setService.GetFileTypeSetByMonitorServerIdAndFolderName(msID, parentNodePath);
                DirectoryInfo dirInfo = new DirectoryInfo(parentNodePath);
                foreach (DirectoryInfo folder in dirInfo.GetDirectories())
                {
                    TreeViewVirtualNode childNode = new TreeViewVirtualNode(folder.FullName.ToString(), folder.Name.ToString());
                    childNode.Image.Url = DirImageUrl;
                    childNode.Target = "99";
                    children.Add(childNode);
                }
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    TreeViewVirtualNode childNode = new TreeViewVirtualNode(file.FullName.ToString(), file.Name.ToString());
                    childNode.IsLeaf = true;
                    childNode.Target = file.Extension;//ファイル拡張子
                    children.Add(childNode);
                }
            }
            e.Children = children;
        }
    }
}