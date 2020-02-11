using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BudBackupCopy.DBInterface;
using BudBackupCopy.DBService;
using BudBackupCopy.Entities;

namespace BudBackupCopy.Models
{
    public class FileTypeSetManager
    {
        private IFileTypeSetService fileTypeSetService = null;

        public FileTypeSetManager() : this(new FileTypeSetService()) { }

        public FileTypeSetManager(IFileTypeSetService fileTypeSetService)
        {
            this.fileTypeSetService = fileTypeSetService;
        }

        /// <summary>
        /// 除外条件の抽出
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <param name="monitorServerID"></param>
        /// <param name="deleteFlg"></param>
        /// <returns></returns>
        public FileSetInfo CheckFileTypeSet(string fileFullName, int monitorServerID, int deleteFlg)
        {
            FileSetInfo fileSetInfo = new FileSetInfo();

            List<Entities.fileTypeSet> fileTypeSetList = this.fileTypeSetService.GetByFileTypeSet(monitorServerID, deleteFlg);
            if (fileTypeSetList.Count > 0)
            {
                for (int i = 0; i < fileTypeSetList.Count; i++)
                {
                    if (fileTypeSetList[i].monitorServerFolderName.IndexOf(fileFullName) > -1)
                    {
                        List<string> fileExtensionList = new List<string>();
                        // 判断
                        if (fileTypeSetList[i].exceptAttributeFlg1.Equals("1"))
                        {
                            if (!String.IsNullOrEmpty(fileTypeSetList[i].exceptAttribute1))
                            {
                                fileExtensionList.Add(fileTypeSetList[i].exceptAttribute1);
                            }
                        }
                        else if (fileTypeSetList[i].exceptAttributeFlg2.Equals("1"))
                        {
                            if (!String.IsNullOrEmpty(fileTypeSetList[i].exceptAttribute2))
                            {
                                fileExtensionList.Add(fileTypeSetList[i].exceptAttribute2);
                            }
                        }
                        else if (fileTypeSetList[i].exceptAttributeFlg3.Equals("1"))
                        {
                            if (!String.IsNullOrEmpty(fileTypeSetList[i].exceptAttribute3))
                            {
                                fileExtensionList.Add(fileTypeSetList[i].exceptAttribute3);
                            }
                        }
                        if (fileExtensionList.Count() > 0)
                        {
                            fileSetInfo.DirectoryPathList.Add(fileTypeSetList[i].monitorServerFolderName);
                            fileSetInfo.DirectoryFileExtensionList.Add(fileTypeSetList[i].monitorServerFolderName, fileExtensionList);
                        }
                    }
                }
            }
            return fileSetInfo;

        }
    }
}
