using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Collections;
namespace DBUtility
{
    public class Database
    {   //标志更新model是否选取null字段
        public bool updateAll = false;
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConn()
        {
            try
            {
                //从配置文件中读取数据库连接字符串
                string servername = System.Configuration.ConfigurationManager.AppSettings["servername"]; 
                string dbname = System.Configuration.ConfigurationManager.AppSettings["dbname"];
                string userinfo = System.Configuration.ConfigurationManager.AppSettings["dbuser"];
                string pwdinfo = System.Configuration.ConfigurationManager.AppSettings["dbpwd"];
                //string driver = System.Configuration.ConfigurationManager.AppSettings["driver"];
                string connstr = "Server=" + servername + ";Database=" + dbname + "; User Id=" + userinfo + "; Password=" + pwdinfo;

                //实例化一个连接
                SqlConnection conn = new SqlConnection(connstr);
                return conn;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->GetConn");
                throw e;
            }
        }

        static public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;

            return obj;
        }

        /// <summary>
        /// 判断数据库连接是否打开
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <returns>
        /// flag:
        ///     true:打开
        ///     false:未打开
        /// </returns>
        public bool CheckConn(SqlConnection conn)
        {
            bool flag = false;
            if (conn.State == ConnectionState.Open)
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="sconn"></param>
        public void CloseConnection(SqlConnection sconn)
        {
            if (sconn != null)
            {
                sconn.Close();
                sconn.Dispose();
            }
        }
        /// <summary>
        /// 执行检索语句
        /// </summary>
        /// <param name="sconn">数据库连接</param>
        /// <param name="sql">要执行的sql语句</param>
        /// <returns></returns>
        public DataSet Squery(string name, SqlParameter[] parameters, SqlConnection sconn)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sconn;
                cmd.CommandText = name;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                SqlDataAdapter dp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dp.Fill(ds);
                return ds;

            }
            catch (Exception e)
            {
                return null;
                throw e;
            }
            
        }

        /// <summary>
        /// 执行检索语句
        /// </summary>
        /// <param name="sconn">数据库连接</param>
        /// <param name="sql">要执行的sql语句</param>
        /// <returns></returns>
        public DataTable Squery(string sql, SqlConnection sconn)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(sql, sconn);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Squery");
                throw e;
            }
        }
        /// <summary>
        /// 执行检索语句(加事务)
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        /// <param name="stran">事务</param>
        /// <returns>查询结果datatable</returns>
        public DataTable Squery(string sql, SqlConnection sconn, SqlTransaction stran)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(sql, sconn);
                sda.SelectCommand.Transaction = stran;
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Squery");
                throw e;
            }
        }
        /// <summary>
        /// 执行检索语句(加事务)
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        /// <param name="stran">事务</param>
        /// <returns>查询结果datatable</returns>
        public DataTable Squery(string sql, SqlConnection sconn, SqlTransaction stran, params SqlParameter[] values)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand scom = new SqlCommand(sql, sconn);
                scom.Parameters.AddRange(values);
                scom.Transaction = stran;
                SqlDataAdapter sda = new SqlDataAdapter(scom);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Squery");
                throw e;
            }
        }
        /// <summary>
        /// 执行检索语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="sconn">数据库连接</param>
        /// <param name="values">需要传入的参数</param>
        /// <returns>datatable</returns>
        public DataTable Squery(string sql, SqlConnection sconn, params SqlParameter[] values)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand scmd = new SqlCommand(sql, sconn);
                scmd.Parameters.AddRange(values);
                SqlDataAdapter sda = new SqlDataAdapter(scmd);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Squery");
                throw e;
            }
        }
        /// <summary>
        /// 执行更新，删除，插入语句
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        public int Udaquery(string sql, SqlConnection sconn)
        {
            try
            {
                SqlCommand scom = new SqlCommand(sql, sconn);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(scom);
                sda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                return 0;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Udaquery");
                throw e;
            }

        }
        /// <summary>
        /// 执行更新，删除，插入语句
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        public int Udaquery(string sql, SqlConnection sconn, params SqlParameter[] values)
        {
            try
            {
                SqlCommand scom = new SqlCommand(sql, sconn);
                scom.Parameters.AddRange(values);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(scom);
                sda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0] == DBNull.Value ? -1 : dt.Rows[0][0]);
                }
                return 0;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Udaquery");
                throw e;
            }

        }
        /// <summary>
        /// 执行更新，删除，插入语句(事务)
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        /// <param name="strans">事务</param>
        public int Udaquery(string sql, SqlConnection sconn, SqlTransaction strans)
        {
            try
            {
                SqlCommand scom = new SqlCommand(sql, sconn);
                scom.Transaction = strans;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(scom);
                sda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                return 0;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Udaquery");
                throw e;
            }
        }
        /// <summary>
        /// 执行更新，删除，插入语句(事务)
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        /// <param name="strans">事务</param>
        public int Udaquery(string sql, SqlConnection sconn, SqlTransaction strans, params SqlParameter[] values)
        {
            try
            {
                SqlCommand scom = new SqlCommand(sql, sconn);
                scom.Parameters.AddRange(values);
                scom.Transaction = strans;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(scom);

                sda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                return 0;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Udaquery");
                throw e;
            }
        }
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">要执行的sql语句</param>
        /// <param name="sconn">数据库连接</param>
        public int GetInfol(string sql, SqlConnection sconn)
        {
            int num = 0;
            try
            {
                SqlCommand scom = new SqlCommand(sql, sconn);
                return num;
            }
            catch (Exception e)
            {
                //this.lw.error(e.Message, "database->Udaquery");
                throw e;
            }

        }

        /// <summary>
        /// 获取一共有多少行内容
        /// </summary>
        /// <param name="sconn">数据库连接</param>
        /// <param name="tblName">数据表名</param>
        /// <param name="strWhere">查询条件</param>
        /// <returns>总行数</returns>
        public int GetCount(SqlConnection sconn, string tblName, string strWhere = "")
        {
            string sqlStr = "";

            try
            {
                if (tblName.Length > 0)
                {
                    sqlStr = "SELECT COUNT(*) AS num FROM " + tblName;

                    if (strWhere.Length > 0)
                    {
                        sqlStr += " WHERE " + strWhere;
                    }
                    DataTable dt = this.Squery(sqlStr, sconn);
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取特定行数的内容
        /// </summary>
        /// <param name="pageNum">当前页数</param>
        /// <param name="psize">每页显示条目数</param>
        /// <param name="sconn">数据库连接conn</param>
        /// <param name="ary_fields">传入要查询的字段</param>
        /// <param name="strWhere">Where条件</param>
        /// <param name="tblName">要查询的表名</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(int pageNum, int psize, SqlConnection sconn, string tblName, string[] ary_fields, string strWhere = "", string orders = "")
        {
            try
            {
                int seltop = 0;
                int noseltop = 0;
                if (pageNum <= 1)
                    seltop = psize;
                else
                {
                    seltop = pageNum * psize;
                    noseltop = (pageNum - 1) * psize;
                }
                string sqlpart = "";
                if (ary_fields.Length > 0)
                {
                    if (orders != "")
                    {
                        sqlpart = "SELECT row_number() OVER (ORDER BY " + orders + ") AS row_num";
                    }
                    else
                    {
                        sqlpart = "SELECT row_number() OVER (ORDER BY " + ary_fields[0] + ") AS row_num";
                    }
                    for (int i = 0; i < ary_fields.Length; i++)
                    {
                        sqlpart += ",ISNULL(" + ary_fields[i] + ",'') AS " + ary_fields[i];
                    }
                }
                else
                {
                    sqlpart = "SELECT row_number() OVER (ORDER BY id) AS row_num";
                    sqlpart += ",*";
                }
                sqlpart += " FROM " + tblName;
                if (strWhere.Length > 0)
                {
                    sqlpart += " WHERE " + strWhere;
                }
                string sqlstr = "SELECT * FROM (" + sqlpart + ") AS table_a WHERE row_num > " + noseltop + " AND row_num <= " + seltop;
                DataTable dt = this.Squery(sqlstr, sconn);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 获取特定行数的内容
        /// </summary>
        /// <param name="pageNum">当前页数</param>
        /// <param name="psize">每页显示条目数</param>
        /// <param name="sconn">数据库连接conn</param>
        /// <param name="ary_fields">传入要查询的字段</param>
        /// <param name="strWhere">Where条件</param>
        /// <param name="tblName">要查询的表名</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataJoinTable(int pageNum, int psize, SqlConnection sconn, string tblName1, string[] ary_fields, string tblName2, string orders = "")
        {
            try
            {
                int seltop = 0;
                int noseltop = 0;
                if (pageNum <= 1)
                    seltop = psize;
                else
                {
                    seltop = pageNum * psize;
                    noseltop = (pageNum - 1) * psize;
                }
                string sqlpart = "";
                if (ary_fields.Length > 0)
                {
                    if (orders != "")
                    {
                        sqlpart = "SELECT row_number() OVER (ORDER BY " + orders + ") AS row_num";
                    }
                    else
                    {
                        sqlpart = "SELECT row_number() OVER (ORDER BY " + ary_fields[0] + ") AS row_num";
                    }
                    for (int i = 0; i < ary_fields.Length; i++)
                    {
                        sqlpart += ",ISNULL(" + ary_fields[i] + ",'') AS " + ary_fields[i];
                    }
                }
                else
                {
                    sqlpart = "SELECT row_number() OVER (ORDER BY id) AS row_num";
                    sqlpart += ",*";
                }
                sqlpart += " FROM " + tblName1;
                if (tblName2.Length > 0)
                {
                    sqlpart += " , " + tblName2;
                }
                string sqlstr = "SELECT * FROM (" + sqlpart + ") AS table_a WHERE row_num > " + noseltop + " AND row_num <= " + seltop;
                DataTable dt = this.Squery(sqlstr, sconn);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public SqlTransaction stran;//事物处理

        /// <summary>
        /// 通过model查询count
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="o"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int getCountByModel(SqlConnection conn, Object o, string tableName)
        {
            Hashtable ht = getModelValue(o);//获取有效数据
            int paramCount = ht.Count;
            SqlParameter[] sp = new SqlParameter[paramCount];//创建SqlParameter参数数组
            string sql = getQueryCountSql(ht, tableName, sp);//生成sql 装载SqlParameter
            DataTable dt = null;
            if (ht.Count != 0)
            {
                dt = this.Squery(sql, conn, sp);
            }
            else
            {
                dt = this.Squery(sql, conn);
            }

            if (dt.Rows.Count != 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return -1;
        }


        /// <summary>
        /// 通过sql查询单列内容
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="o">参数对象</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int getCountBySql(SqlConnection conn, Object o, string sql)
        {
            Hashtable ht = getModelValue(o);//有效数据
            int paramCount = ht.Count;
            SqlParameter[] sp = new SqlParameter[paramCount];
            getQuerySql(ht, "", sp);
            DataTable dt = null;
            if (ht.Count != 0)
            {
                dt = this.Squery(sql, conn, sp);
            }
            else
            {
                dt = this.Squery(sql, conn);
            }
            if (dt.Rows.Count != 0)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return -1;
        }
        /// <summary>
        /// 删除model中对应数据库中的字段
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="o">model 对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="conn">连接</param>
        /// <param name="stran">事务</param>
        /// <returns></returns>
        public int delete(Object o, string tableName, SqlConnection conn, SqlTransaction stran = null)
        {
            Hashtable ht = getModelValue(o);//有效数据
            int paramCount = ht.Count;
            SqlParameter[] sp = new SqlParameter[paramCount];
            string sql = getDeleteSql(ht, tableName, sp);
            if (stran == null)
                return this.Udaquery(sql, conn, sp);
            else
                return this.Udaquery(sql, conn, stran, sp);
        }


        /// <summary>
        /// 将model中的内容添加到指定的表中
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="o">model 对虾</param>
        /// <param name="tableName">表名</param>
        /// <param name="conn">连接</param>
        /// <param name="stran">事务</param>
        /// <returns></returns>
        public int insert(Object o, string tableName, SqlConnection conn, SqlTransaction stran = null)
        {
            Hashtable ht = getModelValue(o);//有效数据
            int paramCount = ht.Count;
            SqlParameter[] sp = new SqlParameter[paramCount];
            string sql = getInsertSql(ht, tableName, sp);
            sql += " SELECT scope_identity()";
            if (stran != null)//不需要事务
            {
                return this.Udaquery(sql, conn, stran, sp);
            }
            else
            {
                return this.Udaquery(sql, conn, sp);
            }
        }


        /// <summary>
        /// 更新model中的内容到数据库
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="o">model对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="wheres">作为条件的model中的字段</param>
        /// <param name="conn"></param>
        /// <param name="stran"></param>
        /// <returns>-1失败0成功</returns>
        public int update(SqlConnection conn, Object o, string tableName, String[] wheres, SqlTransaction stran = null)
        {
            Hashtable ht = getModelValue(o, updateAll);//有效数据
            int paramCount = ht.Count;
            SqlParameter[] sp = new SqlParameter[paramCount];
            string sql = getUpdateSql(ht, tableName, sp, wheres);
            updateAll = false;
            if (stran != null)//不需要事务
            {
                return this.Udaquery(sql, conn, stran, sp);
            }
            else
            {
                return this.Udaquery(sql, conn, sp);
            }
            
        }
        /// <summary>
        /// 更新model中的内容到数据库
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="o">model对象</param>
        /// <param name="conn"></param>
        /// <param name="stran"></param>
        /// <returns>-1失败0成功</returns>
        public int update(SqlConnection conn, Object o, string sql, SqlTransaction stran = null)
        {   //有效数据
            Hashtable ht = getModelValue(o,updateAll);
            SqlParameter[] sp = new SqlParameter[ht.Count];
            getUpdateSql(ht, "", sp, new string[0]);
            updateAll = false;
            //不需要事务
            if (stran != null)
            {
                return this.Udaquery(sql, conn, stran, sp);
            }
            else
            {
                return this.Udaquery(sql, conn, sp);
            }
        }

        /// <summary>
        /// 通过model对象查询数据库
        /// 该方法返回一个model实例
        /// model中无有效参数返回空的model(这里不能返回List)
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="o">对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="para">查询指定字段</param>
        /// <returns>model</returns>
        public Object getByModel(SqlConnection conn, Object o, string tableName)
        {   //有效数据
            Hashtable ht = getModelValue(o);
            if (ht.Count == 0)
            {
                return Activator.CreateInstance(o.GetType());
            }
            SqlParameter[] sp = new SqlParameter[ht.Count];
            //生成sql & 装载参数列表
            string sql = getQuerySql(ht, tableName, sp);
            //执行查询
            DataTable dt = this.Squery(sql, conn, sp);
            //装载DataRow到Model
            return loadModelData(o, dt);

        }
        /// <summary>
        /// 通过制定sql语句和参数model查询
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="o">model对象 可以为null</param>
        /// <param name="sql">sql语句</param>
        /// <returns>查询不到任何数据会返回空model</returns>
        public Object getBySQL(SqlConnection conn, Object o, string sql)
        {
            Hashtable ht = getModelValue(o);//有效数据
            int paramCount = ht.Count;
            //if (ht.Count == 0)
            //{
            //    return Activator.CreateInstance(o.GetType());
            //}
            SqlParameter[] sp = new SqlParameter[paramCount];
            getQuerySql(ht, "", sp);//生成sql & 装载参数列表
            DataTable dt = this.Squery(sql, conn, sp);//执行查询
            return loadModelData(o, dt);//装载DataRow到Model

        }
        /// <summary>
        /// 通过sql查询数据库获取List
        /// sql和参数对象o
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="o">sql中的参数对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="p_sql">sql语句</param>
        /// <returns>返回指定泛型的list</returns>
        public Object getListBySQL(SqlConnection conn, Object o, string p_sql)
        {   //取得有效数据
            Hashtable ht = getModelValue(o);
            //创建sql参数列表
            SqlParameter[] sp = new SqlParameter[ht.Count];
            //装载参数列表 这里不需要生成sql所以表名为空
            getQuerySql(ht, "", sp);
            //执行查询
            DataTable dt = this.Squery(p_sql, conn, sp);
            //装载DataRow到Model
            return loadModelListData(o, dt);

        }

        /// <summary>
        /// 通过model对象查询数据库
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="o">对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="para">查询指定字段</param>
        /// <returns>返回指定类型的泛型</returns>
        public Object getListByModel(SqlConnection conn, Object o, string tableName)
        {   //获取model中的有效数据
            Hashtable ht = getModelValue(o);
            //创建参数列表
            SqlParameter[] sp = new SqlParameter[ht.Count];
            //生成sql并加载sp参数
            string sql = getQuerySql(ht, tableName, sp);
            //执行查询
            DataTable dt = this.Squery(sql, conn, sp);
            //装载DataRow到Model
            return loadModelListData(o, dt);

        }
        /// <summary>
        /// 传入model对象,将datatable中的内容封装到model中
        /// ArgumentOutOfRangeException 当查询结果有多条记录时触发
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="obj">model 对象</param>
        /// <param name="dt">数据表</param>
        /// <returns>DataTable中不存在有效行返回空的model</returns>
        public Object loadModelData(Object obj, DataTable dt)
        {
            try
            {
                Object newModel = Activator.CreateInstance(obj.GetType());
                if (dt.Rows.Count == 1)
                {
                    DataRow dr = dt.Rows[0];
                    setFieldValue(dr, newModel);
                }
                else if (dt.Rows.Count > 1)
                {
                    //throw new ArgumentOutOfRangeException("查询结果不唯一 by:wangjianxu");
                    throw new Exception("ERR");
                }
                return newModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 通过传入类型返回DataTable中的所有行集合
        /// BindingFlags.Instance    指定实例成员包含在搜索中
        /// BindingFlags.GetProperty 指定可以返回属性的值
        /// BindingFlags.NonPublic  搜索包含非public成员
        /// BindingFlags.Public     搜索包含public成员
        /// field.FieldType.IsPrimitive 判断是否为基本数据类型
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="o">集合类型</param>
        /// <param name="dt">数据表</param>
        /// <returns>
        /// 如果DataTable中的行数为0返回空list
        /// </returns>
        public Object loadModelListData(Object o, DataTable dt)
        {   //创指定类型的泛型的类型
            Type t = Type.GetType("System.Collections.Generic.List`1[" + o.GetType().FullName + "]");
            //通过类型创建实例
            object list = t.InvokeMember(null,
                                        BindingFlags.DeclaredOnly |
                                        BindingFlags.Public |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.CreateInstance, null, null, new object[] { });
            //获取泛型对象中的Add方法
            MethodInfo add = t.GetMethod("Add");
            if (dt.Rows.Count == 0)
            {
                return list;
            }
            foreach (DataRow dr in dt.Rows)
            {   //创建单个model对象
                Object obj = Activator.CreateInstance(o.GetType());
                //DataRow-->Model
                setFieldValue(dr, obj);
                //将装满数据的model添加进list
                add.Invoke(list, new Object[] { obj });
            }
            return list;

        }
        /// <summary>
        /// 把datatRow中的内容加载到model中
        /// BindingFlags.Instance    指定实例成员包含在搜索中
        /// BindingFlags.GetProperty 指定可以返回属性的值
        /// BindingFlags.NonPublic  搜索包含非public成员
        /// BindingFlags.Public     搜索包含public成员
        /// field.FieldType.IsPrimitive 判断是否为基本数据类型
        /// 如果该字段在数据库中对应的是空 基本数据类型赋默认值 -1 对象类型默认值null
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="obj">对象</param>
        /// <returns>
        /// 无返回值obj是引用类型该方法执行完成会直接影响obj
        /// </returns>
        public void setFieldValue(DataRow dr, Object obj)
        {
            foreach (FieldInfo fd in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public))
            {
                FieldInfo field = obj.GetType().GetField(fd.Name, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                //如果这个字段存在
                if (dr.Table.Columns[fd.Name.ToUpper()] != null)
                {
                    if (field.FieldType.IsPrimitive)
                    {   //字段赋值
                        field.SetValue(obj, dr[fd.Name.ToUpper()] != DBNull.Value ? Convert.ChangeType(dr[fd.Name.ToUpper()], field.FieldType) : 0);

                    }
                    else
                    {   //字段赋值
                        field.SetValue(obj, dr[fd.Name.ToUpper()] != DBNull.Value ? Convert.ChangeType(dr[fd.Name.ToUpper()], field.FieldType) : null);
                    }

                }
                else
                {   // 字段不属于基本数据类型 并且不是string类型(string没有空参构造)
                    if (!field.FieldType.IsPrimitive && field.FieldType != typeof(string))
                    {
                        field.SetValue(obj, Activator.CreateInstance(field.FieldType));
                    }
                }
            }
        }

        /// <summary>
        /// 获取对象中有效数据封装成Hashtable
        /// 获取任意model中的有效数据
        /// 有效数据: int      !=-1;
        ///           string   !="";
        ///           T        !=null;
        ///           DateTime !=DateTime.MinValue;
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="o">model对象</param>
        /// <returns>
        /// o为null或者不存在有效数据返回空的Hashtable
        /// </returns>
        private Hashtable getModelValue(Object o,bool all=false)
        {   //key:model字段 value:model字段中的值
            Hashtable ht = new Hashtable();
            if (o != null)
            {
                //获取model中的所有字段 包括public private
                PropertyInfo[] FieldInfos = o.GetType().GetProperties();
                foreach (PropertyInfo item in FieldInfos)
                {
                    string name = item.Name;
                    /**
                     * FieldInfo 获取model中的字段
                     * name      要获取的字段名(区分大小写)
                     * */
                    PropertyInfo item2 = o.GetType().GetProperty(name);
                    /**
                     * 取得字段类型如果是int,string,DateTime
                     * 并且内容不为空添加到ht中
                     **/
                    switch (item2.PropertyType.ToString())
                    {
                        case "System.String":
                            if (item2.GetValue(o,null) != null || all)
                            {
                                ht.Add(name, item2.GetValue(o, null));
                            }
                            break;
                        case "System.Int32":
                            if ((item2.GetValue(o, null) != null && Convert.ToInt32(item2.GetValue(o, null)) != -1) || all)
                            {
                                ht.Add(name, item2.GetValue(o, null));
                            }
                            break;
                        case "System.DateTime":
                            if ((item2.GetValue(o, null) != null && item2.GetValue(o, null).ToString() != DateTime.MinValue.ToString()) || all)
                            {
                                ht.Add(name, item2.GetValue(o, null));
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
            return ht;
        }

        /// <summary>
        /// 根据ht参数拼接sql并装载sp
        /// By:wangjianxu 2012-05-22
        /// </summary>
        private string getQuerySql(Hashtable ht, string tableName, SqlParameter[] sp)
        {
            string sql = "select * from {0} where 1=1 {1}";
            string sonsql = "";
            if (ht.Count != 0)
            {
                IDictionaryEnumerator ide = ht.GetEnumerator();
                int nownumer = 0;
                while (ide.MoveNext())
                {
                    sonsql += " and " + ide.Key.ToString().ToUpper() + "=@" + ide.Key.ToString().ToUpper();
                    sp[nownumer] = new SqlParameter("@" + ide.Key.ToString().ToUpper(), ide.Value.ToString());
                    nownumer++;

                }
            }
            sql = string.Format(sql, tableName, sonsql);
            return sql;
        }
        /// <summary>
        /// 拼接更新sql
        /// By:wangjianxu 2012-05-22
        /// </summary>
        /// <param name="ht">有效的字段</param>
        /// <param name="tableName">表名</param>
        /// <param name="sp">事务</param>
        /// <param name="wheres">条件列表</param>
        /// <returns></returns>
        private string getUpdateSql(Hashtable ht, string tableName, SqlParameter[] sp, String[] wheres)
        {
            //if (ht.Count == 0)
            //{
            //    throw new ArgumentOutOfRangeException("参数异常，model中没有任何需要更新的字段 by:wangjianxu");
            //}
            string sql = "update {0} set {1} WHERE 1=1 AND {2}";//这是注释
            string setSql = "";//set
            string whereSsql = "";//where
            if (wheres.Length != 0)
            {
                for (int i = 0; i < wheres.Length; i++)
                {
                    whereSsql += wheres[i].ToUpper() + "=@" + wheres[i];

                }

            }
            IDictionaryEnumerator ide = ht.GetEnumerator();
            int nownumer = 0;
            while (ide.MoveNext())
            {   //排除条件字段
                if (Array.BinarySearch(wheres, ide.Key.ToString().ToUpper()) < 0)
                {
                    setSql += ", " + ide.Key.ToString().ToUpper() + "=@" + ide.Key.ToString().ToUpper();

                }
                sp[nownumer] = new SqlParameter("@" + ide.Key.ToString().ToUpper(), ide.Value == null ? "" : ide.Value.ToString());
                nownumer++;

            }
            setSql = setSql.Substring(1, setSql.Length - 1);
            sql = string.Format(sql, tableName, setSql, whereSsql);
            return sql;
        }
        private string getInsertSql(Hashtable ht, string tableName, SqlParameter[] sp)
        {
            //if (ht.Count == 0)
            //{
            //    throw new ArgumentOutOfRangeException("参数异常，model中没有任何有价值的内容 by:wangjianxu");
            //}
            string sql = "INSERT INTO {0} ({1}) values ({2})";//这是注释
            string keySql = "";
            string valueSql = "";
            int nownumer = 0;
            IDictionaryEnumerator ide = ht.GetEnumerator();
            while (ide.MoveNext())
            {
                keySql += ", " + ide.Key.ToString().ToUpper();
                valueSql += ", @" + ide.Key.ToString().ToUpper();
                sp[nownumer] = new SqlParameter("@" + ide.Key.ToString().ToUpper(), ide.Value.ToString());
                nownumer++;

            }
            keySql = keySql.Substring(1, keySql.Length - 1);
            valueSql = valueSql.Substring(1, valueSql.Length - 1);
            sql = string.Format(sql, tableName, keySql, valueSql);
            return sql;
        }
        private string getDeleteSql(Hashtable ht, string tableName, SqlParameter[] sp)
        {
            if (ht.Count == 0)
            {
                throw new ArgumentOutOfRangeException("参数异常，model中所有字段值为默认，操作无效 by:wangjianxu");
            }
            string sql = "delete from {0} where 1=1 {1}";
            string sonsql = "";
            IDictionaryEnumerator ide = ht.GetEnumerator();
            int nownumer = 0;
            while (ide.MoveNext())
            {
                sonsql += " and " + ide.Key.ToString().ToUpper() + "=@" + ide.Key.ToString().ToUpper();
                sp[nownumer] = new SqlParameter("@" + ide.Key.ToString().ToUpper(), ide.Value.ToString());
                nownumer++;

            }
            sql = string.Format(sql, tableName, sonsql);
            return sql;
        }
        private string getQueryCountSql(Hashtable ht, string tableName, SqlParameter[] sp)
        {
            string sql = "select count(*) from {0} where 1=1 {1}";
            string sonsql = "";
            if (ht.Count != 0)
            {
                IDictionaryEnumerator ide = ht.GetEnumerator();
                int nownumer = 0;
                while (ide.MoveNext())
                {
                    sonsql += " and " + ide.Key.ToString().ToUpper() + "=@" + ide.Key.ToString().ToUpper();
                    sp[nownumer] = new SqlParameter("@" + ide.Key.ToString().ToUpper(), ide.Value.ToString());
                    nownumer++;

                }

            }
            return string.Format(sql, tableName, sonsql);
        }
    }

}