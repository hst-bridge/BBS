using DevExpress.Web.ASPxTreeView;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Model;
using log4net;

namespace budbackup.Models
{
    public class TreeViewVirtualModeHelper
    {
        const string FileImageUrl = "~/Content/TreeView/FileSystem/file.png";
        //const string DirImageUrl = "~/Content/TreeView/FileSystem/directory.png";
        const string DirImageUrl = "~/Content/Image/iconFolder.gif";
        const string ServerImageUrl = "~/Content/Image/iconServer.gif";
        static HttpRequest Request { get { return HttpContext.Current.Request; } }
        public static string strStartPath = string.Empty;
        //public static List<budbackup.Models.MonitFolder> initFolderList= new List<budbackup.Models.MonitFolder>();
        public static bool bInit = true;
        public static string msID = string.Empty;
        private static readonly ILog logger = LogManager.GetLogger(typeof(TreeViewVirtualModeHelper));

        public static void CreateChildren(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            //string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? Request.MapPath("~/") : e.NodeName;
            string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? strStartPath : e.NodeName;
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            //if (strStartPath == "")
            //{

            //}
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
            //else if (Directory.Exists(parentNodePath))
            //{
            //    foreach (string childPath in Directory.GetDirectories(parentNodePath))
            //    {
            //        string childDirName = Path.GetFileName(childPath);
            //        //if(IsSystemName(childDirName))
            //        //    continue;
            //        TreeViewVirtualNode childNode = new TreeViewVirtualNode(childPath, childDirName);
            //        childNode.Image.Url = DirImageUrl;
            //        childNode.Target = "99";
            //        children.Add(childNode);
            //    }
            //    // ファイルを表示する必要がない。
            //    foreach (string childPath in Directory.GetFiles(parentNodePath))
            //    {
            //        string childFileName = Path.GetFileName(childPath);
            //        //if(IsSystemName(childFileName))
            //        //    continue;
            //        TreeViewVirtualNode childNode = new TreeViewVirtualNode(childPath, childFileName);
            //        childNode.IsLeaf = false;
            //        childNode.Image.Url = FileImageUrl;
            //        childNode.ClientVisible = false;
            //        children.Add(childNode);
            //    }
            //}
            else if (Directory.Exists(parentNodePath))
            {
                IBLL.IFileTypeSetService setService = BLLFactory.ServiceAccess.CreateFileTypeSetService();
                FileTypeSet fset = setService.GetFileTypeSetByMonitorServerIdAndFolderName(msID, parentNodePath);
                DirectoryInfo dirInfo = new DirectoryInfo(parentNodePath);
                try
                {
                    foreach (DirectoryInfo folder in dirInfo.GetDirectories())
                    {
                        if (fset.id != null)
                        {
                            if (fset.hiddenFileFlg == "1")
                            {//隠しファイル
                                if ((folder.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }
                            if (fset.systemFileFlg == "1")
                            {//システムファイル
                                if ((folder.Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                        }
                        TreeViewVirtualNode childNode = new TreeViewVirtualNode(folder.FullName.ToString(), folder.Name.ToString());
                        childNode.Image.Url = DirImageUrl;
                        childNode.Target = "99";
                        children.Add(childNode);
                    }

                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        if (fset.id != null)
                        {
                            //string strExceptAttribute1 = string.Empty;
                            //string strExceptAttribute2 = string.Empty;
                            //string strExceptAttribute3 = string.Empty;
                            //if (fset.exceptAttributeFlg1 == "1")
                            //{
                            //    strExceptAttribute1 = fset.exceptAttribute1;
                            //}
                            //if (fset.exceptAttributeFlg2 == "1")
                            //{
                            //    strExceptAttribute2 = fset.exceptAttribute2;
                            //}
                            //if (fset.exceptAttributeFlg3 == "1")
                            //{
                            //    strExceptAttribute3 = fset.exceptAttribute3;
                            //}
                            //if (CommonWeb.CommonUtil.IsExceptFile(file.Name, strExceptAttribute1, strExceptAttribute2, strExceptAttribute3))
                            //{
                            //    continue;
                            //}
                            if (fset.hiddenFileFlg == "1")
                            {//隠しファイル
                                if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                {
                                    continue;
                                }
                            }
                            if (fset.systemFileFlg == "1")
                            {//システムファイル
                                if ((file.Attributes & FileAttributes.System) == FileAttributes.System)
                                {
                                    continue;
                                }
                            }
                        }
                        TreeViewVirtualNode childNode = new TreeViewVirtualNode(file.FullName.ToString(), file.Name.ToString());

                        //show files——2014-06-02 wjd modified

                        //childNode.IsLeaf = true;
                        childNode.Image.Url = FileImageUrl;
                        childNode.Image.AlternateText = file.Extension;//ファイル拡張子
                        //childNode.Target = file.Extension;
                        childNode.Expanded = true;
                        //childNode.ClientVisible = false;
                        children.Add(childNode);
                    }
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
            e.Children = children;
        }
    }
}
