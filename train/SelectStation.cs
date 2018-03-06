using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    /// <summary>
    /// 查询相关站点内容
    /// </summary>
    public class SelectStation
    {
        /// <summary>
        /// 查询某一站的隶属站次
        /// </summary>
        /// <param name="lastStationName"></param>站名
        /// <returns></returns>

        public int SelectBelong(String lastStationName) {
            int sub = 0;//隶属站次
            SqlDataReader reader = null;
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectSubStr = "select SubjectStation from WorkingLine where StationName = @LASTNAME";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectSubStr;
                con.Open();
                command.Parameters.AddWithValue("@LASTNAME", lastStationName);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sub = reader.GetInt32(0);
                    //Console.WriteLine(sub);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询隶属站次失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return sub;
        }

        /// <summary>
        /// 查询某一站后面所有站点的隶属站次
        /// </summary>
        /// <param name="substation"></param>某一站的隶属站次
        /// <returns></returns>
        public List<Common> SelectSubStation(int substation) {
            SqlDataReader reader = null;
            Common comm = null;
            List<Common> list = new List<Common>();
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectAllSubStr = "select StationName,SubjectStation from WorkingLine where SubjectStation > @SUBSTATION";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectAllSubStr;
                con.Open();
                command.Parameters.AddWithValue("@SUBSTATION", substation);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comm = new Common()
                    {
                        StationName = reader.GetString(0),
                        SubjectStation = reader.GetInt32(1)
                    };

                    list.Add(comm);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询隶属站次数组失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return list;
        }

        /// <summary>
        /// 查询某一站的经纬度
        /// </summary>
        /// <param name="StationName"></param>站名
        /// <returns></returns>
        public Common SelectLngLat(String StationName) {
            SqlDataReader reader = null;
            Common comm = null;
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectLngLatStr = "select StationLng,StationLat from WorkingLine where StationName = @STATIONNAME";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectLngLatStr;
                con.Open();
                command.Parameters.AddWithValue("@STATIONNAME", StationName);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comm = new Common()
                    {
                        lng = reader.GetDouble(0),
                        lat = reader.GetDouble(1)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询经纬度失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return comm;
        }

        /// <summary>
        /// 根据上一站的站名查该站站名
        /// </summary>
        /// <param name="name"></param>站名
        /// <returns></returns>
        public Common SelectFirstStation(String name)
        {
            SqlDataReader reader = null;
            Common comm = null;
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectStaNameStr = "select StationName from WorkingLine where LastStationName =@NAME";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectStaNameStr;
                con.Open();
                command.Parameters.AddWithValue("@NAME", name);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comm = new Common()
                    {
                        StationName=reader.GetString(0)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询站名失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return comm;
        }

        /// <summary>
        /// 根据站名的站名查该上一站站名
        /// </summary>
        /// <param name="name"></param>站名
        /// <returns></returns>
        public Common SelectBeforeStation(String name)
        {
            SqlDataReader reader = null;
            Common comm = null;
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectStaNameStr = "select LastStationName from WorkingLine where StationName =@NAME";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectStaNameStr;
                con.Open();
                command.Parameters.AddWithValue("@NAME", name);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comm = new Common()
                    {
                        LastStationName = reader.GetString(0)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询站名失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return comm;
        }

        /// <summary>
        /// 查询列车运行线路信息，为DateGridView绑定数据源
        /// </summary>
        /// <returns></returns>
        public List<DateView> SelectGridViewStation()
        {
            SqlDataReader reader = null;
            DateView view = null;
            List<DateView> list = new List<DateView>();
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectGVStr = "select StationName,StationEnName,StationStopTime,StationLng,StationLat,StationDistance from WorkingLine order by SubjectStation asc";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectGVStr;
                con.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    view = new DateView()
                    {
                        StationName = reader.GetString(0),
                        StationEnName = reader.GetString(1),
                        stopTime=reader.GetInt32(2),
                        lng=reader.GetDouble(3),
                        lat = reader.GetDouble(4),
                        distance=reader.GetDouble(5)
                    };
                    list.Add(view);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询线路信息失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return list;
        }
        /// <summary>
        /// 查询首站
        /// </summary>
        /// <returns></returns>
        public string  SelectFirstStation()
        {
            SqlDataReader reader = null;
            string firstName = null;
            string firstEnName = null;
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectGVStr = "select top 1 StationName,StationEnName from WorkingLine order by SubjectStation asc";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectGVStr;
                con.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    firstName = reader.GetString(0);
                    firstEnName = reader.GetString(1);
                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询首站名称失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            firstName = firstName + "," + firstEnName;
            return firstName;
        }

        /// <summary>
        /// 查询末站
        /// </summary>
        /// <returns></returns>
        public string SelectLastStation()
        {
            SqlDataReader reader = null;
            string lastName = null;
            string lastEnName = null;
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectGVStr = "select top 1 StationName,StationEnName from WorkingLine order by SubjectStation desc";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectGVStr;
                con.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lastName = reader.GetString(0);
                    lastEnName = reader.GetString(1);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询末站名称失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            lastName = lastName + "," + lastEnName;
            return lastName;
        }

        /// <summary>
        /// 查询列车运行线路站名称信息，为ComboBox绑定数据源
        /// </summary>
        /// <returns></returns>
        public List<Common> SelectComboBoxStation()
        {
            SqlDataReader reader = null;
            Common Comm=null;
            List<Common> list = new List<Common>();
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectGVStr = "select StationName,StationEnName from WorkingLine order by SubjectStation asc";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectGVStr;
                con.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Comm = new Common
                    {
                        StationName = reader.GetString(0),
                        StationEnName = reader.GetString(1)
                    };
                       
                    list.Add(Comm);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询站名称信息失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return list;
        }
    }
}
