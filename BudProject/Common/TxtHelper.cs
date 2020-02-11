using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Common
{
    class TxtHelper
    {
        // 全体の行数
        private int _LineNumber;
        // ファイルパス
        private string _FilePath;

        /// <summary> 
        /// ファイルの全体の行数
        /// </summary> 
        public int LineNumber
        {
            get { return this._LineNumber; }
        }

        /// <summary> 
        /// 文件路径 
        /// </summary> 
        public string FilePath
        {
            get { return this._FilePath; }
        }

        // データリストの保存
        private ArrayList fileLine;

        /// <summary> 
        /// ファイルを呼んでArrayListに保存して、行番号によって並べる
        /// </summary> 
        /// <param   name= "FilePath "> </param> 
        public TxtHelper(string FilePath)
        {
            try
            {
                this._FilePath = FilePath;
                StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding("utf-8"));
                fileLine = new ArrayList();
                int i = 0;
                while (sr.Peek() > -1)
                {
                    fileLine.Insert(i, sr.ReadLine());
                    i = i + 1;
                }

                this._LineNumber = i;
                sr.Dispose();
                sr.Close();
            }
            catch (Exception error)
            {
                LogManager.WriteLog(LogFile.ERROR, error.Message);
            }
        }


        /// <summary> 
        /// ある行の内容を戻る
        /// </summary> 
        /// <param   name= "LineIndex "> </param> 
        /// <returns> </returns> 
        public string ReadLine(int LineIndex)
        {
            return this.fileLine[LineIndex].ToString();
        }

        /// <summary> 
        /// 行が存在しない場合は、行の内容を交換し、最後に追加。 
        /// </summary> 
        /// <param   name= "LineIndex "> </param> 
        /// <param   name= "LineValue "> </param> 
        /// <returns> </returns> 
        public void WriteLine(int LineIndex, string LineValue)
        {
            if (LineIndex <= this._LineNumber)
            {
                this.fileLine[LineIndex] = LineValue;
            }
            else
            {
                this.fileLine.Insert(this._LineNumber, LineValue);
                this._LineNumber += 1;
            }
        }

        /// <summary> 
        /// ある行に挿入する
        /// </summary> 
        /// <param   name= "LineIndex "> </param> 
        /// <param   name= "LineValue "> </param> 
        public void InsertLine(int LineIndex, string LineValue)
        {
            if (LineIndex <= this._LineNumber)
            {
                this.fileLine.Insert(LineIndex, LineValue);
            }
            else
            {
                this.fileLine.Insert(this._LineNumber, LineValue);
                this._LineNumber += 1;
            }
        }

        /// <summary> 
        /// 元のファイルを上書きする 
        /// </summary> 
        public void Save()
        {
            StreamWriter sw = new StreamWriter(this._FilePath, false, Encoding.GetEncoding("utf-8"));
            try
            {
                for (int i = 0; i < this.fileLine.Count; i++)
                {
                    sw.WriteLine(this.fileLine[i]);
                }
                sw.Dispose();
                sw.Close();
            }
            catch (Exception error)
            {
                LogManager.WriteLog(LogFile.ERROR, error.Message);
            }
        }

        /// <summary> 
        /// 新しいファイルを生成する
        /// </summary> 
        /// <param   name= "FilePath "> </param> 
        public void Save(string FilePath)
        {
            StreamWriter sw = new StreamWriter(FilePath, true, Encoding.GetEncoding("utf-8"));
            try
            {
                for (int i = 0; i < this.fileLine.Count; i++)
                {
                    sw.WriteLine(this.fileLine[i]);
                }
                sw.Dispose();
                sw.Close();
            }
            catch (Exception error)
            {
                LogManager.WriteLog(LogFile.ERROR, error.Message);
            }
        }
    }
}
