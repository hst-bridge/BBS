using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudCopyListen.Common
{
    public class RandomCode
    {
        /// <summary> 
        /// Random Code
        /// </summary> 
        /// <param   name= "num ">桁数</param> 
        public static string GetCode(int num)
        {
            string[] source ={"0","1","2","3","4","5","6","7","8","9",
                         "A","B","C","D","E","F","G","H","I","J","K","L","M","N",
                       "O","P","Q","R","S","T","U","V","W","X","Y","Z"};
            string code = "";
            Random rd = new Random();
            for (int i = 0; i < num; i++)
            {
                code += source[rd.Next(0, source.Length)];
            }
            return code;
        }
    }
}
