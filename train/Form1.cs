using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace train
{
    public partial class Form1 : Form
    {
        //定义全局变量
        public int currentCount = 0;//计时器timer1时间变量
        public int m = 1;//dataGridView行索引变量
        public int Index = 0;//dataGridView选中行索引变量
        public int flagYu = 0;//预到站标识
        public int flagDao = 0;//到站标识
        public string staName = null;//前方到站站名
        public string staLng = null;//前方到站经度
        public string staLat = null;//前方到站纬度
        public string colIndex = null;//dataGridView中选中行的站名
        public double v = 0.0;//速度
        public double L = 7.0;//到下一站距离
        public string lng = null;//上一站经度
        public string lat = null;//上一站纬度
        public int LedSw = 0;//车速温度控制变量
        public string OutstrWen = null;//温度
        public int ThreadTime = 0;//计时器时间变量
        public bool timer6_Enable = false;//计时器timer6是否开启变量
        public bool timer1_First = true;//计时器timer1是否第一次开始
        public static string[] PortNames = SerialPort.GetPortNames();
         
        public static string configComString = PortNames[0];//获取COM端口号

        List<DateView> listDV = new List<DateView>();

        SerialPort serialPort1 = new SerialPort(configComString, 115200, Parity.None, 8, StopBits.One);      //初始化串口设置
        //定义委托
        public delegate void Displaydelegate(byte[] InputBuf);
        public Displaydelegate disp_delegate;

        AutoSizeFormClass asc = new AutoSizeFormClass();//响应式布局
        public Form1()
        {
            //接收温湿度数据
            disp_delegate = new Displaydelegate(DispUI);
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(Comm_DataReceived);
            Console.WriteLine(configComString);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Console.WriteLine(configComString);
            asc.controllInitializeSize(this);
            DateViewDetails();//加载运行线路信息

            this.timer1.Enabled = false;
            this.timer1.Interval = 1000;
            this.textBox15.Text = "360";//默认最高车速
            this.textBox16.Text = "0.5";//默认加速度
            this.textBox1.Text = "G59";//默认车次
            this.textBox17.Text = "11";//默认车厢
            this.textBox4.Text = "0";
            this.textBox5.Text = this.dataGridView1.Rows[m - 1].Cells[3].Value.ToString();
            this.textBox6.Text = this.dataGridView1.Rows[m - 1].Cells[4].Value.ToString();
            this.textBox7.Text = "0";
            this.textBox8.Text = "0";
            this.textBox9.Text = "0";
            this.textBox10.Text = "0";
            this.textBox11.Text = "0";
            this.textBox12.Text = this.dataGridView1.Rows[m - 1].Cells[4].Value.ToString();
            this.textBox13.Text = this.dataGridView1.Rows[m - 1].Cells[3].Value.ToString();
            this.textBox14.Text = this.dataGridView1.Rows[m - 1].Cells[0].Value.ToString();

            dataGridView1.Rows[0].Selected = false;//dataGridView默认未选中
            showStation();//显示首末站
            ComboBoxDetails();//comboBox1绑定数据源
            LedShow led = new LedShow();
            led.ShowLedNULL();
            
        }

        
        /// <summary>
        /// 列车运行信息展示（计时器处理函数）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            // Console.WriteLine(listDV[i].StationName);
            Calculate cal = new Calculate();
            Double D = double.Parse(this.dataGridView1.Rows[m].Cells[5].Value.ToString());//两站之间的距离
            dataGridView1.Rows[m].Selected = true;
            currentCount += 1;
            staName = this.dataGridView1.Rows[m].Cells[0].Value.ToString();//前方到站
            staLng = this.dataGridView1.Rows[m].Cells[3].Value.ToString();//前方到站经度
            staLat = this.dataGridView1.Rows[m].Cells[4].Value.ToString();//前方到站纬度
            double acceleration = double.Parse(textBox16.Text);//加速度
            double speed = double.Parse(textBox15.Text);//最高运行速度 
            v = cal.getSpeed(currentCount, speed, acceleration, D);//当前运行速度速度
            L = cal.getDistance(currentCount, speed, acceleration, D);//当前位置距下一站距离
            lng = this.dataGridView1.Rows[m - 1].Cells[3].Value.ToString();//上一到站经度
            lat = this.dataGridView1.Rows[m - 1].Cells[4].Value.ToString();//上一到站纬度
            string result = cal.getCoordinate(double.Parse(lng), double.Parse(lat), double.Parse(staLng), double.Parse(staLat), L);
            string[] str = result.Split(',');
            this.textBox5.Text = Math.Round(double.Parse(str[0]), 4).ToString();
            this.textBox6.Text = Math.Round(double.Parse(str[1]), 4).ToString();
            this.textBox4.Text = v.ToString();
            this.textBox7.Text = L.ToString();
            this.textBox14.Text = staName;
            this.textBox13.Text = staLng;
            this.textBox12.Text = staLat;
            if (L <= 0) {
                //Console.WriteLine(111);
                this.timer1.Stop(); //计时器timer1停止计时
                this.timer4.Stop(); //计时器timer4停止计时
                currentCount = 0;
                this.textBox5.Text = this.dataGridView1.Rows[m].Cells[3].Value.ToString();
                this.textBox6.Text = this.dataGridView1.Rows[m].Cells[4].Value.ToString();
                this.textBox4.Text = "0";
                this.textBox7.Text = "0";
                dataGridView1.Rows[m].Selected = false;//dataGridView默认未选中
                this.label21.Text = "列车停靠";
                m++;
                this.textBox14.Text = this.dataGridView1.Rows[m - 1].Cells[0].Value.ToString();
                this.textBox13.Text = this.dataGridView1.Rows[m - 1].Cells[3].Value.ToString();
                this.textBox12.Text = this.dataGridView1.Rows[m - 1].Cells[4].Value.ToString();
                
                serialPort1.Close();
                this.textBox8.Text = "0";
                this.textBox9.Text = "0";
                this.textBox10.Text = "0";
                this.textBox11.Text = "0";
            }

        }
        /// <summary>
        /// 预到站和到站语音播报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e) 
        {
            if (L < 6.5)
            {
                if (this.dataGridView1.Rows[m].Cells[0].Value.ToString() == this.textBox3.Text)
                {//终点站
                    flagYu++;
                    if (flagYu == 1)
                    {

                        timer1_First = false;
                        Thread thread = new Thread(speechYuZDZ);
                        thread.Start();
                    }
                }
                else 
                {//途径站
                    flagYu++;
                    if (flagYu == 1)
                    {
                        timer1_First = false;
                        Thread thread = new Thread(speechYu);
                        thread.Start();
                    }
                }
                
                if (L < 0.5) {
                    if (this.dataGridView1.Rows[m].Cells[0].Value.ToString() == this.textBox3.Text)
                    {//终点站
                        flagDao++;
                        if (flagDao == 1)
                        {
                            Thread thread = new Thread(speechDaoZDZ);
                            thread.Start();
                        }
                    }
                    else
                    {//途径站
                        flagDao++;
                        if (flagDao == 1)
                        {
                            Thread thread = new Thread(speechDao);
                            thread.Start();
                        }
                    }
                    
                }
            }
        }


        /// <summary>
        /// 列车到站停靠
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer3_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine(L);
            if (L <= 0) 
            {
                flagYu = 0;
                flagDao = 0;
                L = 7.0;
            }
        }
        /// <summary>
        /// 预到站语音播报
        /// </summary>

        public void speechYu()
        {
            LedShow led = new LedShow();
            Speech sp = new Speech();
            SpeechOperat spo = new SpeechOperat();
            string text1 = null;
            string text2 = null;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            List<SpeechModel> listYu = new List<SpeechModel>();
            listYu = spo.SelectSp("预到站提示语");
            for (int i = 0; i < listYu.Count; i++)
            {
                text1 = listYu[i].speechContent;
                text2 = listYu[i].speechEnContent;
            }
            text1 = "惠而浦冰箱洗衣机提醒您，列车即将到达 " + textBox14.Text + "," + text1;
            text2 = "Whirlpool refrigerator washing machine remind you,The train is about to arrive " + this.dataGridView1.Rows[m].Cells[1].Value.ToString() + " Station," + text2;
            led.ShowLEDMessage(text1 + "  " + text2 + "                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            Thread.Sleep(4000);  // 停顿4秒
            sp.SpeechTest(text2, ent2, vcn2);
            Thread.Sleep(48000);  // 停顿48秒
            led.ShowLedFLStation(" ", this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 临近终点站语音播报
        /// </summary>

        public void speechYuZDZ()
        {
            LedShow led = new LedShow();
            Speech sp = new Speech();
            SpeechOperat spo = new SpeechOperat();
            string text1 = null;
            string text2 = null;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            List<SpeechModel> listYu = new List<SpeechModel>();
            listYu = spo.SelectSp("终点站提示语");
            for (int i = 0; i < listYu.Count; i++)
            {
                text1 = listYu[i].speechContent;
                text2 = listYu[i].speechEnContent;
            }
            text1 = "惠而浦冰箱洗衣机提醒您，列车即将到达我们的终点站 " + textBox14.Text + "," + text1;
            text2 = "Whirlpool refrigerator washing machine remind you,The train is about to arrive at our terminus " + this.dataGridView1.Rows[m].Cells[1].Value.ToString() + " Station," + text2;
            led.ShowLEDMessage(text1 + "  " + text2 + "                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            Thread.Sleep(2000);  // 停顿2秒
            sp.SpeechTest(text2, ent2, vcn2);
            Thread.Sleep(58000);  // 停顿58秒
            led.ShowLedFLStation(" ", this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 途径站发车语音播报
        /// </summary>

        public void speechTu()
        {
            LedShow led = new LedShow();
            Speech sp = new Speech();
            string text1 = null;
            string text2 = null;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            text1 = "惠而浦冰箱洗衣机提醒您，列车已经由 " + this.dataGridView1.Rows[m - 1].Cells[0].Value.ToString() + " 开出，前方到站 " +
                this.dataGridView1.Rows[m].Cells[0].Value.ToString();
            text2 = "Whirlpool refrigerator washing machine remind you,The train has been started from " + this.dataGridView1.Rows[m - 1].Cells[1].Value.ToString() + " Station " +
                "and next station is " + this.dataGridView1.Rows[m].Cells[1].Value.ToString()+" Station.";
            led.ShowLEDMessage(text1 + "。 " + text2 + "                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            Thread.Sleep(2000);  // 停顿2秒
            sp.SpeechTest(text2, ent2, vcn2);
            Thread.Sleep(20000);  // 停顿24秒
            led.ShowLEDMessage(" ", this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 途径站下一站为终点站的发车语音播报
        /// </summary>

        public void speechTuZDZ()
        {
            LedShow led = new LedShow();
            Speech sp = new Speech();
            string text1 = null;
            string text2 = null;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            text1 = "惠而浦冰箱洗衣机提醒您，列车已经由 " + this.dataGridView1.Rows[m - 1].Cells[0].Value.ToString() + " 开出，前方到站是我们的终点站 " +
                this.dataGridView1.Rows[m].Cells[0].Value.ToString();
            text2 = "Whirlpool refrigerator washing machine remind you,The train has been started from " + this.dataGridView1.Rows[m - 1].Cells[1].Value.ToString() + " Station," +
                "The nest station is our terminus station " + this.dataGridView1.Rows[m].Cells[1].Value.ToString() + " Station.";
            led.ShowLEDMessage(text1 + "。 " + text2 + "                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            Thread.Sleep(2000);  // 停顿2秒
            sp.SpeechTest(text2, ent2, vcn2);
            Thread.Sleep(27000);  // 停顿27秒
            led.ShowLEDMessage(" ", this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 到站语音播报
        /// </summary>

        public void speechDao()
        {
            LedShow led = new LedShow();
            Speech sp = new Speech();
            SpeechOperat spo = new SpeechOperat();
            string text1 = null;
            string text2 = null;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            List<SpeechModel> listYu = new List<SpeechModel>();
            listYu = spo.SelectSp("到站提示语");
            for (int i = 0; i < listYu.Count; i++)
            {
                text1 = listYu[i].speechContent;
                text2 = listYu[i].speechEnContent;
            }
            text1 = "惠而浦冰箱洗衣机提醒您，列车已经到达 " + textBox14.Text + "," + text1;
            text2 = "Whirlpool refrigerator washing machine remind you,The train has arrived at " + this.dataGridView1.Rows[m].Cells[1].Value.ToString() + " Station," + text2;
            led.ShowLEDMessage(text1 + "  " + text2 + "                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            Thread.Sleep(2000);  // 停顿2秒
            sp.SpeechTest(text2, ent2, vcn2);
            Thread.Sleep(30000);  // 停顿30秒
            led.ShowLedFLStation(this.textBox2.Text + "——" + this.textBox3.Text, this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 终点站语音播报
        /// </summary>

        public void speechDaoZDZ()
        {
            LedShow led = new LedShow();
            Speech sp = new Speech();
            SpeechOperat spo = new SpeechOperat();
            string text1 = null;
            string text2 = null;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            List<SpeechModel> listYu = new List<SpeechModel>();
            listYu = spo.SelectSp("终点站提示语");
            for (int i = 0; i < listYu.Count; i++)
            {
                text1 = listYu[i].speechContent;
                text2 = listYu[i].speechEnContent;
            }
            text1 = "惠而浦冰箱洗衣机提醒您，列车已经安全抵达我们的终点站 " + textBox14.Text + "," + text1;
            text2 = "Whirlpool refrigerator washing machine remind you,The train has arrived safely at our terminal " + this.dataGridView1.Rows[m].Cells[1].Value.ToString() + " station," + text2;
            led.ShowLEDMessage(text1 + "  " + text2 +"                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            Thread.Sleep(2000);  // 停顿2秒
            sp.SpeechTest(text2, ent2, vcn2);
            Thread.Sleep(35000);  // 停顿35秒
            led.ShowLedFLStation(this.textBox2.Text + "——" + this.textBox3.Text, this.textBox1.Text, this.textBox17.Text);
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
        /// <summary>
        /// 增加停靠点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 fm = new Form2(this);

            this.Hide();//隐藏现在这个窗口
            fm.Show() ;//新窗口显现
        }
        /// <summary>
        /// 程序退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult r = MessageBox.Show("确定要退出程序?", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r != DialogResult.OK)
                {
                    e.Cancel = true;
                }
                else {
                    try
                    {
                        serialPort1.Close();
                        LedShow ledS = new LedShow();
                        ledS.ShowLedNULL();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                }
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        /// <summary>
        /// 广播信息编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            
            Form3 form3 = new Form3(this);

            this.Hide();//隐藏现在这个窗口
            form3.Show();//新窗口显现
        }

        //串口读取数据处理函数
        public void Comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            Byte[] InputBuf = new Byte[8];//接收缓冲区

            try
            {
                //Console.WriteLine(serialPort1.BytesToRead);
                serialPort1.Read(InputBuf, 0, 6);  //读取接收缓冲区内6个字节数据                             
                System.Threading.Thread.Sleep(100);
                this.BeginInvoke(disp_delegate, InputBuf);
            }
            catch (TimeoutException ex)         //超时处理
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //将温湿度数据拉入对应的界面位置
        public void DispUI(byte[] InputBuf)
        {

            string str = System.Text.Encoding.Default.GetString(InputBuf);
           // Console.WriteLine(str);
            string strW = str.Substring(0, 2);//截取str的子串，从index=0开始截取长度为2的字符串
            int OutStrW = int.Parse(strW);
            string strS = str.Substring(2, 2);//截取str的子串，从index=2开始截取长度为2的字符串
            int OutStrS = int.Parse(strS);
            OutstrWen = (OutStrW - 4).ToString();
            textBox8.Text = strW;
            textBox9.Text = (OutStrW - 4).ToString();
            textBox10.Text = strS;
            textBox11.Text = (OutStrS - 10).ToString();
        }
        //发车
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                this.timer1.Start(); //计时器timer1开始计时
                this.timer4.Start(); //计时器timer4开始计时
                serialPort1.Open();//打开串口
                this.button4.Enabled=false;
                this.label21.Text = "前方到站";
                textBoxEnable(false);
                Thread thread = new Thread(SpeechFa);
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// 发车语音播报
        /// </summary>

        public void SpeechFa() {
            LedShow led = new LedShow();
            string text1 = null;
            string text2 = null;
            string[] str1,str2;
            string ent1 = "intp65";
            string vcn1 = "xiaoyan";
            string ent2 = "intp65";
            string vcn2 = "henry";
            SelectStation sel = new SelectStation();
            SpeechOperat spo = new SpeechOperat();
            List<Common> listCom = new List<Common>();
            List<SpeechModel> listSpFa = new List<SpeechModel>();
            List<SpeechModel> listSpJin = new List<SpeechModel>();
            Speech sp = new Speech();
            str1 = sel.SelectFirstStation().Split(',');
            str2 = sel.SelectLastStation().Split(',');
            listCom = sel.SelectComboBoxStation();
            text1 = "，惠而浦冰箱洗衣机提醒您，列车已经由 " + this.dataGridView1.Rows[m - 1].Cells[0].Value.ToString() + " 开出，前方到站 " +
                this.dataGridView1.Rows[m].Cells[0].Value.ToString();
            text2 = "Whirlpool refrigerator and washing machine remind you，The train has been started from " + this.dataGridView1.Rows[m - 1].Cells[1].Value.ToString() + " Station " +
                "and next station is " + this.dataGridView1.Rows[m].Cells[1].Value.ToString()+" Station";
            text1 = text1 + ",本次列车为 " + this.textBox1.Text + " 次列车，由 " + this.textBox2.Text + " 发往 " + this.textBox3.Text + ",途径";
            text2 = text2 + ",This train is " + this.textBox1.Text + " train,and is sent from " + str1[1] + " Station to " + str2[1] + " Station,channel ";
            for (int i =1 ; i < listCom.Count - 1; i++)
            {

                text1 = text1 + listCom[i].StationName + "、";
                text2 = text2 + listCom[i].StationEnName + " Station,";
            }
            listSpFa = spo.SelectSp("欢迎提示语");
            listSpJin = spo.SelectSp("禁烟提示语");
            for (int i = 0; i < listSpFa.Count; i++)
            {
                text1 = listSpFa[i].speechContent + text1 ;
                text2 = listSpFa[i].speechEnContent + text2 ;
            }
            for (int i = 0; i < listSpJin.Count; i++)
            {
                text1 = text1 + listSpJin[i].speechContent;
                text2 = text2 + listSpJin[i].speechEnContent;
            }
            led.ShowLEDMessage(text1 + "  " + text2 + "                     ", this.textBox1.Text, this.textBox17.Text);
            sp.SpeechTest(text1, ent1, vcn1);
            sp.SpeechTest(text2, ent2, vcn2);
            int RowNum = this.dataGridView1.Rows.Count;
            Thread.Sleep(102000 + (RowNum - 13) * 2 * 1000);  // 停顿秒数
            LedSWShow();

        }

       /// <summary>
       /// 获取选中行的占名称
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            colIndex = this.dataGridView1.CurrentRow.Cells["Column1"].Value.ToString();
            Index = this.dataGridView1.CurrentRow.Index;
            //Console.WriteLine(Index);
        }
        /// <summary>
        /// 删除停靠点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (colIndex == null)
            {//未选中
                MessageBox.Show("请选择您要删除的站点！");
            }
            else {
                DialogResult r = MessageBox.Show("确定要删除该停靠站点?", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r != DialogResult.OK)
                {
                  // Cancel = true;
                    dataGridView1.Rows[Index].Selected = false;
                }
                else
                {
                    try
                    {
                        string commLastStaName = null;
                        string commStaName = null;
                        UpdateStation ups = new UpdateStation();
                        DeleteStation delSta = new DeleteStation();
                        SelectStation sel = new SelectStation();
                        Calculate cal = new Calculate();
                        int sub = sel.SelectBelong(colIndex);
                        List<Common> list = new List<Common>();
                        list = sel.SelectSubStation(sub);
                        for (int i = 0; i < list.Count; i++)
                        {
                            ups.UpdateSubject(list[i].StationName, list[i].SubjectStation - 1);
                        }
                        //Console.WriteLine(colIndex);
                        commLastStaName = sel.SelectBeforeStation(colIndex).LastStationName;//查询删除站点的上一站
                        if (commLastStaName == "首站")
                        {//只有首站
                            if (sel.SelectFirstStation(colIndex) == null)
                            {
                                delSta.deleteSta(colIndex);
                            }
                            else
                            {
                                ups.UpdateLastStation(commStaName, "首站");
                                delSta.deleteSta(colIndex);
                            }
                        }
                        else
                        {
                            if (sel.SelectFirstStation(colIndex) == null)
                            {//删除末站
                                delSta.deleteSta(colIndex);
                            }
                            else
                            {//删除中间某一站
                                commStaName = sel.SelectFirstStation(colIndex).StationName;//查询以删除站点为上一站的站点
                                double lastLng = sel.SelectLngLat(commLastStaName).lng;
                                double lastLat = sel.SelectLngLat(commLastStaName).lat;
                                double afterLng = sel.SelectLngLat(commStaName).lng;
                                double afterLat = sel.SelectLngLat(commStaName).lat;
                                ups.UpdateDistance(commStaName, cal.Distance(afterLng, afterLat, lastLng, lastLat));
                                ups.UpdateLastStation(commStaName, commLastStaName);
                                delSta.deleteSta(colIndex);
                            }
                        }
                        DateViewDetails();//加载运行线路信息
                        showStation();//首末站数据加载
                        ComboBoxDetails();//掉电恢复区comboBox数据加载
                        showStation();//显示首末站
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                } 
            } 
        }
        /// <summary>
        /// 显示首末站
        /// </summary>
        public void showStation() {
            string[] str1, str2;
            SelectStation sel = new SelectStation();
            str1 = sel.SelectFirstStation().Split(',');
            str2 = sel.SelectLastStation().Split(',');
            this.textBox2.Text = str1[0];    //首站
            this.textBox3.Text = str2[0];       //末站
        }
        /// <summary>
        ///加载运行线路信息
        /// </summary>
        public void DateViewDetails() {
            SelectStation selSta = new SelectStation();
            listDV = selSta.SelectGridViewStation();
            dataGridView1.DataSource = listDV;
            dataGridView1.Rows[0].Selected = false;//dataGridView默认未选中
        }

        /// <summary>
        ///加载运行线路各个站名称（comboBox数据）
        /// </summary>
        public void ComboBoxDetails()
        {
            SelectStation sel = new SelectStation();
            List<Common> listCom = new List<Common>();
            listCom = sel.SelectComboBoxStation();
            ArrayList list = new ArrayList();
            for (int i = 0; i < listCom.Count; i++) {
                list.Add(listCom[i].StationName);
            }
                comboBox1.DataSource = list;
        }

        /// <summary>
        ///设置textBox和部分按钮的Enable属性值
        /// </summary>
        public void textBoxEnable(bool num)
        {
            this.button1.Enabled = num;
            this.button2.Enabled = num;
            this.button3.Enabled = num;
            this.button6.Enabled = num;
            this.button7.Enabled = num;
            this.textBox1.Enabled = num;
            this.textBox15.Enabled = num;
            this.textBox16.Enabled = num;
            this.textBox17.Enabled = num;
            this.dataGridView1.Enabled = num;
            this.comboBox1.Enabled = num;
        }
        /// <summary>
        /// 掉电恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string staName = comboBox1.Text;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == staName){
                    dataGridView1.Rows[m].Selected = false;//设置dataGridView所有行未选中
                    m = i;//返回当前单元格行的index
                    break;
                }
            }
            this.timer1.Stop();
            currentCount = 0;
            this.timer1.Start(); ;//计时器timer1开始计时
            this.timer4.Start(); //计时器timer4开始计时
            serialPort1.Open();//打开串口
            Thread thread = new Thread(speechTu);
            thread.Start();
            this.button4.Enabled = false;
            this.textBoxEnable(false);
            this.textBox14.Text = "前方到站";
        }
        /// <summary>
        /// Led 车速温度、爱护环境提示语和爱护设施提示语显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer4_Tick(object sender, EventArgs e)
        {
            LedSw++;
            LedShow show = new LedShow();
           
            if (L > 6.5)
            {
                if (this.dataGridView1.Rows[m - 1].Cells[0].Value.ToString() == this.textBox2.Text)
                {//首站
                    if (LedSw % 2 == 0 && LedSw >= 4 && LedSw != 6)
                    {//车速温度
                        Thread tread = new Thread(LedSWShow);
                        tread.Start();
                    }
                    if (LedSw == 5)
                    {//爱护环境
                        Thread tread = new Thread(LedEnviShow);
                        tread.Start();
                    }
                    if (LedSw == 6)
                    {//爱护设施
                        Thread tread = new Thread(LedEqumShow);
                        tread.Start();
                    }
                }
                else 
                {//途径站
                    if (LedSw % 2 == 0 && LedSw != 6)
                    {//车速温度
                        Thread tread = new Thread(LedSWShow);
                        tread.Start();
                    }
                    if (LedSw == 5)
                    {//爱护环境
                        Thread tread = new Thread(LedEnviShow);
                        tread.Start();
                    }
                    if (LedSw == 6)
                    {//爱护设施
                        Thread tread = new Thread(LedEqumShow);
                        tread.Start();
                    }
                }
                
            }
            else {
                LedSw = 0;
            }
        }
       

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            textBoxEnable(false);
            LedShow led = new LedShow();
            led.ShowLedFLStation(this.textBox2.Text + "——" + this.textBox3.Text, this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 车速温度显示
        /// </summary>
        public void LedSWShow() {
            LedShow show = new LedShow();
            string text1 = "当前车速：" + v.ToString() + " km/h   外温：" + OutstrWen + " ℃";
            string text2 = "Current speed: " + v.ToString() + " km/h   Outside temperature: " + OutstrWen + " ℃";
            show.ShowLEDMessage(text1 + "     " + text2 + "                ", this.textBox1.Text, this.textBox17.Text);
            Thread.Sleep(20000);//暂停20秒
            show.ShowLEDMessage(" ", this.textBox1.Text, this.textBox17.Text);
        }

        /// <summary>
        /// 爱护环境提示语显示
        /// </summary>
        public void LedEnviShow()
        {
            LedShow show = new LedShow();
            SpeechOperat spo = new SpeechOperat();
            List<SpeechModel> ListEnvi = new List<SpeechModel>();
            List<SpeechModel> ListEqum = new List<SpeechModel>();
            string text = null;
            ListEnvi = spo.SelectSp("爱护环境提示语");
            for (int i = 0; i < ListEnvi.Count; i++)
            {
                text = ListEnvi[i].speechContent;
                text = text + "     " + ListEnvi[i].speechEnContent;
            }
            show.ShowLEDMessage(text + "                   ", this.textBox1.Text, this.textBox17.Text);
            Thread.Sleep(50000);//暂停50秒
            show.ShowLEDMessage(" ", this.textBox1.Text, this.textBox17.Text);
        }


        /// <summary>
        /// 爱护设施提示语显示
        /// </summary>
        public void LedEqumShow()
        {
            LedShow show = new LedShow();
            SpeechOperat spo = new SpeechOperat();
            List<SpeechModel> ListEnvi = new List<SpeechModel>();
            List<SpeechModel> ListEqum = new List<SpeechModel>();
            string text = null;
            ListEqum = spo.SelectSp("爱护设施提示语");
            for (int i = 0; i < ListEqum.Count; i++)
            {
                text = ListEqum[i].speechContent;
                text = text + "     " + ListEqum[i].speechEnContent;
            }
            show.ShowLEDMessage(text + "                   ", this.textBox1.Text, this.textBox17.Text);
            Thread.Sleep(35000);//暂停35秒
            show.ShowLEDMessage(" ", this.textBox1.Text, this.textBox17.Text);
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            if (this.textBox4.Text == "0") 
            {
                if (timer1_First == false) 
                {
                    if (m < listDV.Count)
                    {
                        if (timer6_Enable == false)
                        {
                            timer6.Start();
                        }
                        while (ThreadTime >= 60)// 停顿60秒
                        {
                            ThreadTime = 0;
                            timer6_Enable = false;
                            timer1_First = true;
                            this.timer6.Stop();  //计时器timer6停止计时
                            this.timer1.Start(); //计时器timer1开始计时
                            this.timer4.Start(); //计时器timer4开始计时
                            serialPort1.Open();
                            this.label21.Text = "前方到站";
                            if (this.dataGridView1.Rows[m].Cells[0].Value.ToString() == this.textBox3.Text)
                            {//终点站
                                Thread thread = new Thread(speechTuZDZ);
                                thread.Start();
                            }
                            else
                            {//途径站
                                Thread thread = new Thread(speechTu);
                                thread.Start();
                            }
                        }
                    }
                    else
                    {
                        m = 1;//从首站开始
                        this.textBox14.Text = this.dataGridView1.Rows[m].Cells[0].Value.ToString();//前方到站
                        this.textBox13.Text = this.dataGridView1.Rows[m].Cells[3].Value.ToString();//前方到站经度
                        this.textBox12.Text = this.dataGridView1.Rows[m].Cells[4].Value.ToString();//前方到站纬度
                        this.textBoxEnable(true);
                        this.button4.Enabled = true;
                        this.textBox4.Text = "0";
                        this.textBox5.Text = this.dataGridView1.Rows[m].Cells[3].Value.ToString();//列车当前经度
                        this.textBox6.Text = this.dataGridView1.Rows[m].Cells[4].Value.ToString();//列车当前纬度
                        this.textBox7.Text = "0";
                        this.label21.Text = "列车停靠";
                    } 
                } 
            }
        }

        /// <summary>
        /// 到站停靠时间计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer6_Tick(object sender, EventArgs e)
        {
            ThreadTime++;
        }


    }
}
