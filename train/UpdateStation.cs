using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    public class UpdateStation
    {
        /// <summary>
        /// 修改某一站距上一站的距离
        /// </summary>
        /// <param name="stationname"></param>站名
        /// <param name="distance"></param>距离
        public void UpdateDistance(String stationname,double distance) {
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String updateDisStr = "update WorkingLine set StationDistance =@DISTANCE where StationName =@STATIONNAME";
                SqlCommand commmand = con.CreateCommand();// 绑定SqlConnection对象
                commmand.CommandText = updateDisStr;
                commmand.Parameters.AddWithValue("@DISTANCE", distance);
                commmand.Parameters.AddWithValue("@STATIONNAME", stationname);
                con.Open();
                commmand.ExecuteNonQuery();//执行命令 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("修改距离失败");
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// 修改某一站的隶属站次
        /// </summary>
        /// <param name="stationname"></param>站名
        /// <param name="subject"></param>隶属站次
        public void UpdateSubject(String stationname,int subject) {
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String updateSubStr = "update WorkingLine set SubjectStation =@SUBSTATION where StationName =@STATIONNAME";
                SqlCommand commmand = con.CreateCommand();// 绑定SqlConnection对象
                commmand.CommandText = updateSubStr;
                commmand.Parameters.AddWithValue("@SUBSTATION", subject);
                commmand.Parameters.AddWithValue("@STATIONNAME", stationname);
                con.Open();
                commmand.ExecuteNonQuery();//执行命令 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("修改隶属站次失败");
            }
            finally
            {
                con.Close();
            }
        }
        /// <summary>
        /// 修改某一站的上一站的名称
        /// </summary>
        /// <param name="laststationname"></param>上一站的名称
        /// <param name="stationname"></param>站名
        public void UpdateLastStation(String laststationname,String stationname)
        {
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String updateLastStaNameStr = "update WorkingLine set LastStationName =@LASTSTATIONNAME where StationName =@STATIONNAME";
                SqlCommand commmand = con.CreateCommand();// 绑定SqlConnection对象
                commmand.CommandText = updateLastStaNameStr;
                commmand.Parameters.AddWithValue("@LASTSTATIONNAME", laststationname);
                commmand.Parameters.AddWithValue("@STATIONNAME", stationname);
                con.Open();
                commmand.ExecuteNonQuery();//执行命令 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("修改上一站名称失败");
            }
            finally
            {
                con.Close();
            }
        }
    }
}
