using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    public class DeleteStation
    {
        /// <summary>
        /// 删除站点
        /// </summary>
        /// <param name="name"></param>站点名称
        public void deleteSta(String name) {
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String deleteStr = "delete from WorkingLine where StationName=@STATIONNAME";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = deleteStr;
                con.Open();
                command.Parameters.AddWithValue("@STATIONNAME", name);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("删除站点失败");
            }
            finally
            {
                con.Close();
            }
        }
    }
}
