using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    /// <summary>
    /// 插入站点
    /// </summary>
    public class IncreaseStation
    {
        /// <summary>
        /// 插入站点
        /// </summary>
        /// <param name="stationName"></param>站点名称
        /// <param name="stationEnName"></param>站点英文名
        /// <param name="stationLng"></param>站点经度
        /// <param name="stationLat"></param>站点纬度
        /// <param name="stopTime"></param>停留时间
        /// <param name="distance"></param>距离
        /// <param name="lastStation"></param>上一站点名称
        /// <param name="belongStation"></param>在本线路中的隶属站次
        public void insertStation(String stationName, String stationEnName, double stationLng, double stationLat, int stopTime, double distance, String lastStation,int belongStation)
        {
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String InsertStr = "insert into WorkingLine (StationName,StationEnName,StationLng,StationLat,StationStopTime,StationDistance,LastStationName,SubjectStation) values (@STATIONNAME,@STATIONENNAME,@STATIONLNG,@STATIONLAT,@STATIONSTOPTIME,@STATIONDISTANCE,@LASTSTATIONNAME,@SUBJECTSTATION)";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = InsertStr;
                con.Open();
                command.Parameters.AddWithValue("@STATIONNAME", stationName);
                command.Parameters.AddWithValue("@STATIONENNAME", stationEnName); ;
                command.Parameters.AddWithValue("@STATIONLNG", stationLng);
                command.Parameters.AddWithValue("@STATIONLAT", stationLat);
                command.Parameters.AddWithValue("@STATIONSTOPTIME", stopTime);
                command.Parameters.AddWithValue("@STATIONDISTANCE", distance);
                command.Parameters.AddWithValue("@LASTSTATIONNAME", lastStation);
                command.Parameters.AddWithValue("@SUBJECTSTATION", belongStation);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("添加站点失败");
            }
            finally
            {
                con.Close();
            }
        }
    }
}
