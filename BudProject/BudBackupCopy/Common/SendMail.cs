using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudBackupCopy.Common
{
    public class SendMail
    {
        /// <summary>
        /// メール送信
        /// </summary>
        /// <param name="serv">SMTPサーバー</param>
        /// <param name="port">SMTPポート番号</param>
        /// <param name="user">ユーザID</param>
        /// <param name="pass">パスワード</param>
        /// <param name="fadr">FROMアドレス</param>
        /// <param name="tadr">TOアドレス(カンマ区切り)</param>
        /// <param name="subj">タイトル</param>
        /// <param name="body">本文</param>
        /// <param name="fils">添付ファイル(TAB区切り)</param>
        public void SmtpSend(
            String serv, int port, String user, String pass,
            String fadr, String tadr, String subj, String body, String fils)
        {

            System.Net.Mail.MailMessage msg = null;
            try
            {
                // MailMessage生成
                msg = new System.Net.Mail.MailMessage();

                // エンコード指定
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding(50220);

                // 件名設定
                msg.Subject = subj;
                msg.SubjectEncoding = enc;

                // 本文設定
                msg.Body = body;
                msg.BodyEncoding = enc;

                // FROMアドレス設定
                msg.From = new System.Net.Mail.MailAddress(fadr);

                // TOアドレス設定(カンマ区切り)
                String[] tos = tadr.Split(',');
                for (int i = 0; i <= tos.Length - 1; i++)
                {
                    if (tos[i] != "")
                    {
                        msg.To.Add(new System.Net.Mail.MailAddress(tos[i]));
                    }
                }

                // 添付ファイルの設定(TAB区切り)
                String[] fls = fils.Split('\t');
                System.Net.Mail.Attachment attachment;
                for (int i = 0; i <= fls.Length - 1; i++)
                {
                    if (fls[i] != "")
                    {
                        if (System.IO.File.Exists(fls[i]))
                        {
                            attachment = new System.Net.Mail.Attachment(fls[i]);
                            attachment.NameEncoding = enc;
                            msg.Attachments.Add(attachment);
                        }
                    }
                }

                // SMTPサーバーとポート番号の設定
                System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
                sc.Host = serv;
                sc.Port = port;

                // ユーザIDとパスワードの設定
                if (user != "")
                {
                    sc.Credentials = new System.Net.NetworkCredential(user, pass);
                }

                // このサーバーはセキュリティで保護された接続(SSL)が必要
                //sc.EnableSsl = true;

                // メール送信
                sc.Send(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (msg != null)
                {
                    msg.Dispose();
                }
            }
        }

        /// <summary>
        /// Pop Before Smtp認証のためPOPサーバに接続
        /// </summary>
        /// <param name="serv">POPサーバー</param>
        /// <param name="port">POPポート番号</param>
        /// <param name="user">ユーザID</param>
        /// <param name="pass">パスワード</param>
        public void PopBeforeSmtp(
            String serv, int port, String user, String pass)
        {

            System.Net.Sockets.NetworkStream stream = null;
            System.Net.Sockets.TcpClient client = null;
            try
            {
                String rstr;
                client = new System.Net.Sockets.TcpClient();

                // POPサーバーに接続
                client.Connect(serv, port);
                stream = client.GetStream();

                // POPサーバー接続時のレスポンス受信
                rstr = WriteAndRead(stream, "");
                if (rstr.IndexOf("+OK") != 0)
                {
                    throw new Exception("POPサーバー接続エラー");
                }

                // ユーザIDの送信
                rstr = WriteAndRead(stream, "USER " + user + "\r\n");
                if (rstr.IndexOf("+OK") != 0)
                {
                    throw new Exception("ユーザIDエラー");
                }

                // パスワードの送信
                rstr = WriteAndRead(stream, "PASS " + pass + "\r\n");
                if (rstr.IndexOf("+OK") != 0)
                {
                    throw new Exception("パスワードエラー");
                }

                //// APOPの場合は[ユーザID送信]と[パスワード送信]の処理を以下のように変更します
                //// POPサーバー接続時のレスポンスからAPOP用のキー(<>で囲まれた部分)を取得して
                //// パスワードと連結(例:"<999.999@mxg999.xxx.com>PASS")してMD5(HEX)変換して
                //// "APOP user MD5(HEX)"形式で送信します
                //Byte[] byt = System.Text.Encoding.ASCII.GetBytes("<999.999@mxg999.xxx.com>" + pass);
                //System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                //    new System.Security.Cryptography.MD5CryptoServiceProvider();
                //Byte[] res = md5.ComputeHash(byt);
                //String aps = BitConverter.ToString(res).Replace("-", "").ToLower();
                //rstr = WriteAndRead(stream, "APOP " + user + " " + aps + "\r\n");
                //if (rstr.IndexOf("+OK") != 0)
                //{
                //    throw new Exception("ユーザIDまたはパスワードエラー");
                //}

                // ステータスの送信
                rstr = WriteAndRead(stream, "STAT" + "\r\n");
                if (rstr.IndexOf("+OK") != 0)
                {
                    throw new Exception("STATエラー");
                }

                // 終了の送信
                rstr = WriteAndRead(stream, "QUIT" + "\r\n");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        /// <summary>
        /// POPサーバ送受信
        /// </summary>
        /// <param name="stm">ストリーム</param>
        /// <param name="req">リクエスト</param>
        /// <returns>レスポンス</returns>
        private String WriteAndRead(
            System.Net.Sockets.NetworkStream stm, String req)
        {

            // POPサーバへリクエスト送信
            if (req != "")
            {
                Byte[] sdata;
                sdata = System.Text.Encoding.ASCII.GetBytes(req);
                stm.Write(sdata, 0, sdata.Length);
            }
            for (int i = 1; i < 300; i++)
            {
                if (stm.DataAvailable) break;
                System.Threading.Thread.Sleep(10);
            }

            // POPサーバからのレスポンス受信
            String rtn = "";
            Byte[] rdata = new Byte[1024];
            while (stm.DataAvailable)
            {
                int l = stm.Read(rdata, 0, rdata.Length);
                if (l > 0)
                {
                    Array.Resize<Byte>(ref rdata, l);
                    rtn = rtn + System.Text.Encoding.ASCII.GetString(rdata);
                }
            }

            // レスポンス返信
            return rtn;
        }
    }
}
