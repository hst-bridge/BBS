using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudLogManage.Common.Helper
{
    /// <summary>
    /// 用于表达文件夹或文件大小
    /// 
    /// 根据日志信息 判断MAC机的文件系统单位为4KB
    /// </summary>
    public class Size
    {
        /// <summary>
        /// 数值
        /// </summary>
        public double Real { get; set; }
        /// <summary>
        /// 单位 m g 如果为empty单位默认为1KB
        /// 不设置4kb为默认的原因：
        /// 1.这样显示的时候，还得转换回来，因为用户只对 b kb m g有直接感觉
        /// 2.这样可以规避风险：有的机器文件系统单位为1KB
        /// 3.相对单位b来说，能减少计算量
        /// </summary>
        public Unit Unit { get; set; }

        public Size() {
            this.Real = 0;
            this.Unit = Helper.Unit.B;
        }

        public Size(string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.Real = 0;
                    this.Unit = Helper.Unit.B;
                }
                else
                {
                    value = value.Trim().ToLower().Replace("g", " G").Replace("m", " M").Replace("b", " B").Replace("k", " K");
                    string[] paras = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (paras.Length == 2)
                    {
                        this.Real = Convert.ToDouble(paras[0]);
                        this.Unit = (Unit)Enum.Parse(typeof(Unit), paras[1]);
                    }
                    else
                    {
                        this.Real = Convert.ToDouble(paras[0]);
                        this.Unit = Helper.Unit.B;
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public Size(double real, Unit dw)
        {
            this.Real = real;
            this.Unit = dw;
        }
        /// <summary>
        /// 重载+
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Size operator +(Size s1, Size s2)
        {
            Size size = new Size();
            //算出总和 单位为b
            double sum = s1.Real * (double)s1.Unit + s2.Real*(double)s2.Unit;
            //取单位
            double mdw = (double)(s1.Unit>s2.Unit?s1.Unit:s2.Unit);
            double temp = sum/mdw;
            if(temp>1024){
                if ((Unit)mdw != Unit.G)
                {
                    size.Real = temp / 1024;
                    size.Unit = (Unit)(1024 * mdw);
                }
            }else{
                size.Real = temp;
                size.Unit = (Unit)mdw;
            }

            return size;
        }

        public override string ToString()
        {
            string real = string.Empty;
            switch (Unit)
            {
                case Unit.G: real = String.Format("{0:N3}", Real); break;
                case Unit.M: real = String.Format("{0:N2}", Real); break;
                case Unit.K: real = String.Format("{0:N1}", Real); break;
                default: real = "0"; break;
            }
            
            return real + Unit.ToString();
        }
    }
}
