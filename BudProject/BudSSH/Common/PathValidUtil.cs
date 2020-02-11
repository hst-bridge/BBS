using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.IO;
using BudSSH.Common.Util;

namespace BudSSH.Common
{
    class PathValidUtil
    {
        public static string GetValidPath(string path)
        {
            //特殊字符替换
            path = InvalidFileChange(path);
            // 濁点/半濁点処理
            return BudSSH.Common.Japanese.NormalizeSoundSymbol(path);
        }

        /// <summary>
        /// ファイルパスの変換
        /// </summary>
        /// <param name="fileName"></param>
        private static string InvalidFileChange(string filePath)
        {
            string resultFilePath = "";
            string[] filePathList = filePath.Split(new string[1] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            char[] invalidPathChars = Path.GetInvalidFileNameChars();
            try
            {
                if (filePathList.Count() > 0)
                {
                    for (int i = 1; i < filePathList.Count(); i++)
                    {
                        // 無効な文字のチェック
                        foreach (char someChar in invalidPathChars)
                        {
                            if (filePathList[i].IndexOf(someChar) > -1)
                            {
                                // 全角変換
                                string zenkakuInvalidCharacter = Strings.StrConv(filePathList[i][filePathList[i].IndexOf(someChar)].ToString(), VbStrConv.Wide, 0);
                                filePathList[i] = filePathList[i].Replace(filePathList[i][filePathList[i].IndexOf(someChar)].ToString(), "〓" + zenkakuInvalidCharacter + "〓");
                            }
                            if (filePathList[i].IndexOf(@"'") > -1)
                            {
                                // 全角変換
                                string zenkakuInvalidCharacter = Strings.StrConv(filePathList[i][filePathList[i].IndexOf(@"'")].ToString(), VbStrConv.Wide, 0);
                                filePathList[i] = filePathList[i].Replace(filePathList[i][filePathList[i].IndexOf(@"'")].ToString(), "〓" + zenkakuInvalidCharacter + "〓");
                            }
                        }
                    }
                }
                foreach (string pathSec in filePathList)
                {
                    if (String.IsNullOrEmpty(resultFilePath))
                    {
                        resultFilePath = pathSec;
                    }
                    else
                    {
                        resultFilePath = resultFilePath + @"\" + pathSec;
                    }
                }
                if (String.IsNullOrEmpty(resultFilePath))
                {
                    resultFilePath = filePath;
                }
            }
            catch (System.Exception ex)
            {
                BudSSH.Common.Util.LogManager.WriteLog(BudSSH.Common.Util.LogFile.Error, MessageUtil.GetExceptionMsg(ex, ""));
                resultFilePath = filePath;
            }
            return resultFilePath;
        }
    }
}
