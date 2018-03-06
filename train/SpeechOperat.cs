using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    /// <summary>
    /// 语音内容相关的操作
    /// </summary>
    public class SpeechOperat
    {
        /// <summary>
        /// 根据选怎的语音类型查询对应的语音内容
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<SpeechModel> SelectSp(string type) {
            SqlDataReader reader = null;
            SpeechModel SM = null;
            List<SpeechModel> list = new List<SpeechModel>();
            SqlConnection con = SqlConnect.getConn();
            try
            {
                String selectSpStr = "select SpeechContent,SpeechEnContent from Speech  where SpeechType =@TYPE";
                SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                command.CommandText = selectSpStr;
                con.Open();
                command.Parameters.AddWithValue("@TYPE", type);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SM = new SpeechModel()
                    {
                        speechContent = reader.GetString(0),
                        speechEnContent = reader.GetString(1)
                    };
                    list.Add(SM);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("查询失败");
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return list;
        }

        public void InsertSpeech(string type, string content, string Encontent)
        {
            SqlConnection con = SqlConnect.getConn();
            SpeechOperat spo = new SpeechOperat();
            List<SpeechModel> list = new List<SpeechModel>();
            list = spo.SelectSp(type);
            if (list.Count == 0)
            {
                try
                {
                    String selectSpStr = "insert into Speech (SpeechType,SpeechContent,SpeechEnContent) values (@TYPE,@CONTENT,@ENCONTENT)";
                    SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                    command.CommandText = selectSpStr;
                    con.Open();
                    command.Parameters.AddWithValue("@TYPE", type);
                    command.Parameters.AddWithValue("@CONTENT", content);
                    command.Parameters.AddWithValue("@ENCONTENT", Encontent);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("添加语音失败");
                }
                finally
                {
                    con.Close();
                }
            }
            else {
                try
                {
                    String selectSpStr = "update Speech set SpeechContent = @CONTENT,SpeechEnContent = @ENCONTENT where SpeechType = @TYPE";
                    SqlCommand command = con.CreateCommand();// 绑定SqlConnection对象
                    command.CommandText = selectSpStr;
                    con.Open();
                    command.Parameters.AddWithValue("@TYPE", type);
                    command.Parameters.AddWithValue("@CONTENT", content);
                    command.Parameters.AddWithValue("@ENCONTENT", Encontent);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("修改语音失败");
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
