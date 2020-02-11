using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudDBSync.BLL.SyncTable
{
    /// <summary>
    /// 用于辅助拼接sql
    /// </summary>
    class TableHelper
    {
        /// <summary>
        /// 根据id字段值 产生相应的where 
        /// 暂时只有log表这样 因为log表不需要考虑删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetWhereCond(int ID)
        {
            string where = string.Empty;
                if (ID == -1)//第一次
                {
                    where = "order by id desc";
                }
                else if (ID > 0)
                {
                    int min = ID - Properties.Settings.Default.TopNumber;
                    if(min<0)min=0;

                    where = " where id between " + min + " and " + ID + " and  synchronismFlg=0 order by id desc";
                }

                return where;
        }
        
        /// <summary>
        /// log 表以外
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static string GetWhereCondCommon(int ID)
        {
            if (ID == -1) ID = 0;

            return " where id > " + ID + " and synchronismFlg=0 order by id asc ";
        }
    }
}
