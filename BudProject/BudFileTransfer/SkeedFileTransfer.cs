using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeplex.Data;

namespace BudFileTransfer
{
    public class SkeedFileTransfer
    {
        private string requestUrl;
        private string account;
        private string password;
        private string strAuth;

        public SkeedFileTransfer(string _requestUrl, string _account, string _password)
        {
            //ローカルサーバーのREST API受け付けURLを生成する。
            requestUrl = _requestUrl;
            account = _account;
            password = _password;
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //Basic認証用のリクエストヘッダを生成する。
            strAuth = "Basic " + Convert.ToBase64String(enc.GetBytes(account + ":" + password));
        }
        /// <summary>
        /// SSBセッションを確立する。
        /// </summary>
        /// <param name="strRemoteNodeId"></param>
        /// <returns></returns>
        public dynamic connect(string strRemoteNodeId)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/session";
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcUploadString(enc, strAuth, strAdress, "remoteNodeId=" + strRemoteNodeId);

            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            if (strResponseBody != string.Empty)
            {
                var jsonSession = DynamicJson.Parse(strResponseBody);
                return jsonSession;
            }
            else
            {
                return strResponseBody;
            }
        }
        /// <summary>
        /// SSBセッションを切断する。
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <returns></returns>
        public dynamic disconnect(string strSessionId)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/session/" + strSessionId;
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcUploadString(enc, strAuth, strAdress, "DELETE", "");
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            var jsonSession = DynamicJson.Parse(strResponseBody);
            return jsonSession;
        }
        /// <summary>
        /// This API obtains information of one session specified by the URI.
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <returns></returns>
        public dynamic getSessionInfo(string strSessionId)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/session/" + strSessionId;
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcDownloadData(enc, strAuth, strAdress);
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            var jsonSession = DynamicJson.Parse(strResponseBody);
            return jsonSession;
        }
        /// <summary>
        /// API请求提交（不带Method）
        /// </summary>
        /// <param name="enc"></param>
        /// <param name="strAuth"></param>
        /// <param name="strAddress"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string wcUploadString(Encoding enc, string strAuth, string strAddress, string strData)
        {
            //データを送受信する
            System.Net.WebClient wc = new System.Net.WebClient();
            //文字コードを指定する
            wc.Encoding = enc;

            //ヘッダ
            wc.Headers.Add("Authorization", strAuth);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string strResponseBody = "";
            try
            {
                strResponseBody = wc.UploadString(strAddress, strData);
            }
            catch (System.Net.WebException ex)
            {
                //HTTPプロトコルエラーかどうか調べる
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    //HttpWebResponseを取得
                    System.Net.HttpWebResponse errres =
                        (System.Net.HttpWebResponse)ex.Response;
                    //応答したURIを表示する
                    Console.WriteLine(errres.ResponseUri);
                    //応答ステータスコードを表示する
                    Console.WriteLine("{0}:{1}",
                        errres.StatusCode, errres.StatusDescription);
                }
            }
            finally
            {
                wc.Dispose();
            }
            return strResponseBody;
        }
        /// <summary>
        /// API请求提交（带Method）
        /// </summary>
        /// <param name="enc"></param>
        /// <param name="strAuth"></param>
        /// <param name="strAddress"></param>
        /// <param name="strMethod"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string wcUploadString(Encoding enc, string strAuth, string strAddress, string strMethod, string strData)
        {
            //データを送受信する
            System.Net.WebClient wc = new System.Net.WebClient();
            //文字コードを指定する
            wc.Encoding = enc;

            //ヘッダ
            wc.Headers.Add("Authorization", strAuth);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string strResponseBody = "";
            try
            {
                strResponseBody = wc.UploadString(strAddress, strMethod, strData);
            }
            catch (System.Net.WebException ex)
            {
                //HTTPプロトコルエラーかどうか調べる
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    //HttpWebResponseを取得
                    System.Net.HttpWebResponse errres =
                        (System.Net.HttpWebResponse)ex.Response;
                    //応答したURIを表示する
                    Console.WriteLine(errres.ResponseUri);
                    //応答ステータスコードを表示する
                    Console.WriteLine("{0}:{1}",
                        errres.StatusCode, errres.StatusDescription);
                }
            }
            finally
            {
                wc.Dispose();
            }
            return strResponseBody;
        }
        /// <summary>
        /// API请求提交（GET）
        /// </summary>
        /// <param name="enc"></param>
        /// <param name="strAuth"></param>
        /// <param name="strAddress"></param>
        /// <returns></returns>
        private string wcDownloadData(Encoding enc, string strAuth, string strAddress)
        {
            //データを送受信する
            System.Net.WebClient wc = new System.Net.WebClient();
            //文字コードを指定する
            wc.Encoding = enc;

            //ヘッダ
            wc.Headers.Add("Authorization", strAuth);
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string strResponseBody = "";
            try
            {
                //データを送受信する
                byte[] bytResponseBody = wc.DownloadData(strAddress);
                strResponseBody = Encoding.UTF8.GetString(bytResponseBody);
            }
            catch (System.Net.WebException ex)
            {
                //HTTPプロトコルエラーかどうか調べる
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    //HttpWebResponseを取得
                    System.Net.HttpWebResponse errres =
                        (System.Net.HttpWebResponse)ex.Response;
                    //応答したURIを表示する
                    Console.WriteLine(errres.ResponseUri);
                    //応答ステータスコードを表示する
                    Console.WriteLine("{0}:{1}",
                        errres.StatusCode, errres.StatusDescription);
                }
            }
            finally
            {
                wc.Dispose();
            }
            return strResponseBody;
        }
        /// <summary>
        /// 多数ファイル転送タスク開始
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name="strFileName"></param>
        public dynamic transferFiles(string sessionId, string strSenderDir, string strFileName)
        {
            //●多数ファイル転送タスク開始(P.133)
            var jsonTask = startUploading(sessionId, strSenderDir, strFileName);
            //実行中のファイル転送タスクの終了を待つ。                                                                        
            if (jsonTask.ToString() != string.Empty)
            {
                var strResult = awaitTerminationOfTask(sessionId, jsonTask.taskId);
                return strResult;
            }
            else
            {
                return jsonTask;
            }
        }
        /// <summary>
        /// ディレクトリ差分転送
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        public dynamic dirrsynctransferFiles(string sessionId, string strSenderDir, string strReceiverDir)
        {
            //●多数ファイル転送タスク開始(P.147)
            var jsonTask = startdirrsynUploading(sessionId, strSenderDir, strReceiverDir);
            return jsonTask;
        }
        /// <summary>
        /// ディレクトリ差分転送バッチ開始
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name="strFileNames"></param>
        /// <returns></returns>
        private dynamic startdirrsynUploading(string strSessionId, string strSenderDir, string strReceiverDir)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/session/" + strSessionId + "/dirrsync";
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcUploadString(enc, strAuth, strAdress, "direction=0&localSyncTargetDir=" + strSenderDir + "&remoteSyncTargetDir=" + strReceiverDir + "&syncDelete=true&forceCreateSyncTargetDir=true");
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            if (strResponseBody != string.Empty)
            {
                var jsonTask = DynamicJson.Parse(strResponseBody);
                return jsonTask;
            }
            else
            {
                return strResponseBody;
            }
        }

        /// <summary>
        /// 特定のセッションに属する実行中のディレクトリ差分転送バッチ一覧
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        public dynamic dirrsyncDoingList(string sessionId, string strSenderDir, string strReceiverDir)
        {
            //●多数ファイル転送タスク開始(P.147)
            var jsonTask = startdirrsynDoingList(sessionId, strSenderDir, strReceiverDir);
            return jsonTask;
        }

        /// <summary>
        /// 特定のセッションに属する実行中のディレクトリ差分転送バッチ一覧
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name="strFileNames"></param>
        /// <returns></returns>
        private dynamic startdirrsynDoingList(string strSessionId, string strSenderDir, string strReceiverDir)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/session/" + strSessionId + "/dirrsync";
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcDownloadData(enc, strAuth, strAdress);
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            if (strResponseBody != string.Empty)
            {
                var jsonTask = DynamicJson.Parse(strResponseBody);
                return jsonTask;
            }
            else
            {
                return strResponseBody;
            }
        }

        /// <summary>
        /// List of all File Transfer Batches currently executed
        /// <param name="strSessionId"></param>
        /// </summary>
        /// <returns></returns>
        public dynamic doingTransferBatchesList()
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/transfer";
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcDownloadData(enc, strAuth, strAdress);
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            if (strResponseBody != string.Empty)
            {
                var jsonTask = DynamicJson.Parse(strResponseBody);
                return jsonTask;
            }
            else
            {
                return strResponseBody;
            }
        }

        /// <summary>
        /// ファイルのアップロードを開始する。
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name="strFileNames"></param>
        /// <returns></returns>
        private dynamic startUploading(string strSessionId, string strSenderDir, string strFileNames)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/session/" + strSessionId + "/bulktransfer";
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcUploadString(enc, strAuth, strAdress, "direction=0&transferonlyupdated=true&senderDir=" + strSenderDir + "&receiverDir=" + strFileNames);
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            if (strResponseBody != string.Empty)
            {
                var jsonTask = DynamicJson.Parse(strResponseBody);
                return jsonTask;
            }
            else
            {
                return strResponseBody;
            }
        }
        /// <summary>
        /// 実行中のファイル転送タスクの終了を待つ。
        /// 等待多文件传输完成
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <param name="strTaskId"></param>
        private dynamic awaitTerminationOfTask(string strSessionId, string strTaskId)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string urlTask = requestUrl + "/session/" + strSessionId + "/bulktransfer/" + strTaskId;
            //URI によって指定された多数ファイル転送タスクの情報を取得します。
            string strTask = wcDownloadData(enc, strAuth, urlTask);
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            var jsonTask = DynamicJson.Parse(strTask);

            bool blnTerminated = false;
            bool blnCompleted = false;
            bool blnCancelled = false;
            bool blnAborted = false;

            while (!blnTerminated)
            {
                //URI によって指定された多数ファイル転送タスクの情報を取得します。
                strTask = wcDownloadData(enc, strAuth, urlTask);

                //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
                jsonTask = DynamicJson.Parse(strTask);

                if (jsonTask.result == 0)    //成功の場合：0 。失敗の場合：0 以外。
                {
                    blnTerminated = jsonTask.fileTransferTask.isTerminated;
                    blnCompleted = jsonTask.fileTransferTask.isCompleted;
                    blnCancelled = jsonTask.fileTransferTask.isCancelled;
                    blnAborted = jsonTask.fileTransferTask.isAborted;
                }
                else
                {
                    blnTerminated = true;
                }

                if (!blnTerminated)
                {
                    //URI によって指定された１セッションの情報を取得します。
                    var jsonSession = getSessionInfo(strSessionId);
                    if (jsonSession.result == 1 || jsonSession.session.isClosed == true)    //成功の場合：0 。失敗の場合：0 以外。
                    {
                        blnTerminated = true;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            return jsonTask;
        }
        /// <summary>
        /// 多数ファイル転送消除開始
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name="strFileName"></param>
        public dynamic deleteFiles(string sessionId, string strSenderDir, string strReceiverDir, string strFileName)
        {
            //●多数ファイル転送タスク開始(P.133)
            var jsonTask = startDeleteing(sessionId, strSenderDir, strReceiverDir, strFileName);
            return jsonTask;
        }
        /// <summary>
        /// ファイルの消除を開始する。
        /// </summary>
        /// <param name="strSessionId"></param>
        /// <param name="strSenderDir"></param>
        /// <param name="strReceiverDir"></param>
        /// <param name="strFileNames"></param>
        /// <returns></returns>
        private dynamic startDeleteing(string strSessionId, string strSenderDir, string strReceiverDir, string strFileNames)
        {
            //文字コードを指定する 
            Encoding enc = Encoding.GetEncoding("UTF-8");
            //ローカルサーバーのREST API受け付けURLを生成する。
            string strAdress = requestUrl + "/fs/remote/" + strSessionId + "/delete";
            //URI によって指定された１セッションの情報を取得します。
            string strResponseBody = wcUploadString(enc, strAuth, strAdress, strFileNames);
            //DynamicJsonクラスのParseメソッドでJSONデータを解析してdynamic型として返す
            if (strResponseBody != string.Empty)
            {
                var jsonTask = DynamicJson.Parse(strResponseBody);
                return jsonTask;
            }
            else
            {
                return strResponseBody;
            }
        }
    }
}
