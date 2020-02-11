using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public class ValidationRegex
    {
        public static string publicTitle = "BUD Backup System";

        public static string C001 = "接続テストが成功しました。";
        public static string C002 = "接続テストが失敗しました。";
        public static string C003 = "バックアップ元の設定は保存されました。";
        public static string C004 = "MACの接続テストが失敗しました。";

        public static string D001 = "削除しました。";
        public static string D002 = "削除出来ませんでした。";
 
        public static string I001 = "保存しました。";
        public static string I002 = "保存出来ませんでした。";
        public static string I003 = "ログインIDまたはパスワードが間違っています。";
        public static string I004 = "データベースに接続出来ないため、ログインできませんでした。";
        public static string I005 = "エラーが出たため、ログインできませんでした。";

        public static string Q001 = "データを保存します。よろしいですか？";
        public static string Q002 = "データを変更します。よろしいですか？";
        public static string Q003 = "入力中のデータを破棄します。よろしいですか？";
        public static string Q004 = "データを削除します。よろしいですか？";
        public static string Q005 = "※ {1}が必須項目なので、入力してください。";

        public static string U001 = "更新しました。";
        public static string U002 = "更新出来ませんでした。";

        public static string W003 = "{1}の入力は不正です。";
        public static string W008 = "{1}が存在しています。";
        public static string W009 = "IPアドレスと開始フォルダ唯一。";
        public static string W010 = "この転送先は利用履歴があります。同じ転送先で再設定しますか？";

        public static string pattern;
        public static string errmsg;

        /// <summary>
        /// public function
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="validteString"></param>
        /// <returns>Bool</returns>
        private static bool PublicMethod(string pattern, string validteString)
        {
            Regex reg = new Regex(pattern);
            Match m = reg.Match(validteString);
            return m.Success;
        }
        /// <summary>
        /// 必須入力の判定
        /// </summary>
        /// <param name="validteParam">項目名</param>
        /// <param name="validteString">判定対象</param>
        /// <returns>受け取りがない場合エラーを返す</returns>
        public static string ValidteEmpty(string validteParam,string validteString)
        {
            errmsg = "";
            if (validteString.Trim() == "")
            {
                //errmsg = "'※ " + validteParam + "が必須項目なので、入力してください。";
                errmsg = "必須項目を入力してください。";  
            }
            return errmsg;
        }
        /// <summary>
        /// 数字の判定
        /// </summary>
        /// <param name="validteParam">項目名</param>
        /// <param name="validteString">判定対象</param>
        /// <returns>入力文字が数字以外ならエラーを返す</returns>
        public static string ValidteData(string validteParam, string validteString)
        {
            pattern = "^[+]?\\d+$";
            errmsg = "";
            if (!PublicMethod(pattern, validteString))
            {
                errmsg = "※ " + validteParam + "は数字で入力してください。";
            }
            return errmsg;
        }

        /// <summary>
        /// 英数字の判定
        /// </summary>
        /// <param name="validteParam">項目名</param>
        /// <param name="validteString">判定対象</param>
        /// <returns>入力文字が英数字以外ならエラーを返す</returns>
        public static string VadidateDataLetter(string validteParam, string validteString)
        {
            pattern = "^[a-zA-Z0-9]+$";
            errmsg = "";
            if (!PublicMethod(pattern, validteString))
            {
                errmsg = "※ " + validteParam + "は英数字で入力してください。";
            }
            return errmsg;
        }
        /// <summary>
        /// 中国字の判定
        /// </summary>
        /// <param name="validteParam">項目名</param>
        /// <param name="validteString">判定対象</param>
        /// <returns>入力文字が中国字以外ならエラーを返す</returns>
        public static string ValidateChineseChar(string validteParam, string validteString)
        {
            pattern = "^[\u4e00-\u9fa5]+$";
            errmsg = "";
            if (!PublicMethod(pattern, validteString))
            {
                errmsg = "※ " + validteParam + "は中国字で入力してください。";
            }
            return errmsg;
        }
        /// <summary>
        /// 時間形式の判定-【H:mm:ss】
        /// </summary>
        /// <param name="validteParam">項目名</param>
        /// <param name="validteString">判定対象</param>
        /// <returns>エラーを返す</returns>
        public static string ValidateTime(string validteParam, string validteString)
        {
            pattern = "^[0-9]{1,2}:[0-9]{2}(:[0-9]{2})?$";
            errmsg = "";
            if (!PublicMethod(pattern, validteString))
            {
                errmsg = "※ "+validteParam+"の形式が正しくありません。入力内容を確認してください。";
            }
            return errmsg;
        }
        /// <summary>
        /// ファイルが存在するかチェックする
        /// </summary>        
        /// <param name="validteFile">判定対象</param>
        /// <returns>エラーを返す</returns>
        public static string ValidateFile(string validteFile)
        {            
            errmsg = "";
            if (!System.IO.File.Exists(validteFile))
            {
                errmsg = "※ " + validteFile + "が見つかりません。";
            }
            return errmsg;
        }
        /// <summary>
        /// ディレクトリ存在チェック
        /// </summary>        
        /// <param name="validteDir">判定対象</param>
        /// <returns>エラーを返す</returns>
        public static string ValidateDir(string validteDir)
        {
            errmsg = "";
            if (!System.IO.Directory.Exists(validteDir))
            {
                errmsg = "※ 指定した" + validteDir + "は存在しません。";
            }
            return errmsg;
        }
        /// <summary>
        /// 同一性の判定
        /// </summary>        
        /// <param name="validteParam1">項目名1</param>
        /// <param name="validteString1">判定対象1</param>
        /// <param name="validteParam1">項目名2</param>
        /// <param name="validteString1">判定対象2</param>
        /// <returns>エラーを返す</returns>
        public static string ValidateEqual(string validteParam1, string validteString1, string validteParam2, string validteString2)
        {
            errmsg = "";
            if (validteString1 != validteString2)
            {
                errmsg = "※ " + validteParam1 +"と"+ validteParam2 + "が一致しません。";
            }
            return errmsg;
        }
    }
}
