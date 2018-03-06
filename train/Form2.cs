using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace train
{
    [ComVisible(true)] //1、必须设置且为true,否则设置webBrowser1.ObjectForScripting对象时会报错 

    public partial class Form2 : Form
    {
        private Form1 returnForm1 = null;
        AutoSizeFormClass asc = new AutoSizeFormClass();
        public Form2(Form1 F1)
        {
            InitializeComponent();
            // 接受Form1对象
            this.returnForm1 = F1;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            asc.controllInitializeSize(this);
            try
            {
               //引入HTML页面，显示地图
                webBrowser1.Url = new Uri(Path.Combine(Application.StartupPath, "ff.html"));
                webBrowser1.ObjectForScripting = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
                //恢复Form1
            this.returnForm1.Visible = true;
            returnForm1.DateViewDetails();//加载运行线路信息
            returnForm1.showStation();//首末站数据加载
            returnForm1.ComboBoxDetails();//掉电恢复区comboBox数据加载
            returnForm1.showStation();//显示首末站
        }

        public void ShowMessage(String stationName, String stationEnName, double stationLng, double stationLat, int stopTime,String lastStation)
        {//插入站点
           
           // Console.WriteLine(stationName + stationEnName + stationLng + stationLat + stopTime + lastStation);
            SelectStation sel = new SelectStation();//实例化查询对象
            UpdateStation updateSta = new UpdateStation();//实例化更新对象
            Common commStaName = new Common();//实例化实体类对象
            IncreaseStation increaseSta = new IncreaseStation();//实例化插入对象
            Calculate cal = new Calculate();//实例化计算距离对象
            Common com= sel.SelectFirstStation(lastStation);
            //Console.WriteLine(com.StationName);
            try
            {
                if (lastStation == "首站")//在运行线路开始插入一站（首站）
                {
                    List<Common> listSub1 = sel.SelectSubStation(0);
                    if (listSub1.Count > 0)
                    {//数据库中还有站点
                        for (int i = 0; i < listSub1.Count; i++)
                        {
                            String StaName = listSub1[i].StationName;
                            int StaSubject = listSub1[i].SubjectStation + 1;
                            updateSta.UpdateSubject(StaName, StaSubject);
                        }
                        String firstStationName = null;
                        if (com.StationName == null)
                        {
                            firstStationName = sel.SelectFirstStation("首站").StationName;
                        }
                        else
                        {
                            firstStationName = com.StationName;
                        }
                        Console.WriteLine(firstStationName);
                        double lng = sel.SelectLngLat(firstStationName).lng;
                        double lat = sel.SelectLngLat(firstStationName).lat;
                        double distance = cal.Distance(stationLng, stationLat, lng, lat);//计算相邻两站之间的距离
                        updateSta.UpdateDistance(firstStationName, distance);//修改当前首站的距离
                        updateSta.UpdateLastStation(stationName, firstStationName);//修改当前的首站的上一站名称为要插入的站的站名
                        increaseSta.insertStation(stationName, stationEnName, stationLng, stationLat, stopTime, 0, lastStation, 1);
                    }
                    else
                    {//刚开始插入，数据库中还没有插入站点（首站）
                        increaseSta.insertStation(stationName, stationEnName, stationLng, stationLat, stopTime, 0, lastStation, 1);
                    }

                }
                else
                {
                    int sub = sel.SelectBelong(lastStation);
                    //Console.WriteLine(sub);
                    List<Common> listSub2 = sel.SelectSubStation(sub);
                    if (listSub2.Count > 0)
                    {//在运行线路中间插入一站
                        for (int i = 0; i < listSub2.Count; i++)
                        {
                            String StaName = listSub2[i].StationName;
                            int StaSubject = listSub2[i].SubjectStation + 1;
                            updateSta.UpdateSubject(StaName, StaSubject);
                        }
                        String staname = null;
                        if (com.StationName == null)
                        {
                            staname = sel.SelectFirstStation("首站").StationName;
                        }
                        else
                        {
                            staname = com.StationName;
                        }
                        Console.WriteLine(staname);
                        double lng = sel.SelectLngLat(staname).lng;
                        double lat = sel.SelectLngLat(staname).lat;
                        double lngL = sel.SelectLngLat(lastStation).lng;
                        double latL = sel.SelectLngLat(lastStation).lat;
                        double distanceL = cal.Distance(stationLng, stationLat, lngL, latL);//计算插入站与前一站两站之间的距离
                        double distance = cal.Distance(lng, lat, stationLng, stationLat);//计算插入站与后一站两站之间的距离
                        updateSta.UpdateDistance(staname, distance);//修改插入站点与后一站的的距离
                        updateSta.UpdateLastStation(stationName, staname);//修改后一站的上一站名称为要插入的站的站名
                        increaseSta.insertStation(stationName, stationEnName, stationLng, stationLat, stopTime, distanceL, lastStation, sub + 1);
                    }
                    else
                    {//在运行线路的末尾插入一站
                        double lng = sel.SelectLngLat(lastStation).lng;
                        double lat = sel.SelectLngLat(lastStation).lat;
                        double distance = cal.Distance(lng, lat, stationLng, stationLat);//计算插入站与后一站两站之间的距离
                        increaseSta.insertStation(stationName, stationEnName, stationLng, stationLat, stopTime, distance, lastStation, sub + 1);
                    }

                }
                MessageBox.Show("添加成功！"); 
            }
            catch {
                MessageBox.Show("添加失败，请检查网络连接！"); 
            }
        }
    } 
}
