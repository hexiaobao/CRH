
using System.Configuration;
using System;
using System.Data.SqlClient;
namespace train
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    public class SqlConnect
    {
       /// <summary>
       /// 连接字符串获取
       /// </summary>
        private static string connectString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <returns></returns>
        public static SqlConnection getConn()
        {
            SqlConnection con = new SqlConnection(connectString);
            
            //Console.WriteLine("连接成功");
            return con;
        }
    }
}
