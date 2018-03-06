using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace train
{
    class LedShow
    {
        public string configString = ConfigurationManager.AppSettings["IPstr"].ToString(); 

        /// <summary>
        /// LED显示屏显示数据函数
        /// </summary>
        /// <param name="Words"></param>
        /// <param name="Train"></param>
        /// <param name="Cnumber"></param>
        public void ShowLEDMessage(string Words, string Train, string Cnumber)
        {
           
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯

            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = configString;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = 1;//LED屏号为1

            int hProgram;//节目句柄
            hProgram = LedDll.LV_CreateProgram(192, 64, 1);//根据传的参数创建节目句柄，192是屏宽点数，64是屏高点数，1是屏的颜色(单色)
            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

            nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                MessageBox.Show(ErrStr);
                return;
            }
            LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 32;
            AreaRect.width = 192;
            AreaRect.height = 32;

            LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;

            LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 2;
            PlayProp.Speed = 4;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, Words, ref FontProp, 4);//通过字符串添加一个多行文本到图文区，参数说明见声明注示



            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = 42;
            AreaRect.height = 32;

            LedDll.LV_AddImageTextArea(hProgram, 1, 2, ref AreaRect, 0);


            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;


            PlayProp.InStyle = 0;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 2, LedDll.ADDTYPE_STRING, Train, ref FontProp, ref PlayProp, 2, 1);//通过字符串添加一个多行文本到图文区，参数说明见声明注示

            AreaRect.left = 58;
            AreaRect.top = 0;
            AreaRect.width = 66;
            AreaRect.height = 32;

            LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
            DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;

            DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
            DigitalClockAreaInfo.ShowStrFont.FontSize = 12;
            DigitalClockAreaInfo.IsShowHour = 1;
            DigitalClockAreaInfo.IsShowMinute = 1;
            DigitalClockAreaInfo.IsShowSecond = 1;
            DigitalClockAreaInfo.TimeFormat = 2;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddDigitalClockArea(hProgram, 1, 3, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示

            AreaRect.left = 140;
            AreaRect.top = 5;
            AreaRect.width = 28;
            AreaRect.height = 22;

            LedDll.LV_AddImageTextArea(hProgram, 1, 4, ref AreaRect, 0);
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddFileToImageTextArea(hProgram, 1, 4, "test.png", ref PlayProp);

            AreaRect.left = 169;
            AreaRect.top = 0;
            AreaRect.width = 23;
            AreaRect.height = 32;

            LedDll.LV_AddImageTextArea(hProgram, 1, 5, ref AreaRect, 0);


            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;


            PlayProp.InStyle = 0;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 5, LedDll.ADDTYPE_STRING, Cnumber, ref FontProp, ref PlayProp, 2, 1);//通过字符串添加一个多行文本到图文区，参数说明见声明注示


            nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                MessageBox.Show(ErrStr);
            }
            else
            {
                // MessageBox.Show("发送成功");
            }
        }

        /// <summary>
        /// LED显示屏显示首末站
        /// </summary>
        /// <param name="Words"></param>
        /// <param name="Train"></param>
        /// <param name="Cnumber"></param>
        public void ShowLedFLStation(string Words, string Train, string Cnumber)
        {
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯

            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = configString;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = 1;//LED屏号为1

            int hProgram;//节目句柄
            hProgram = LedDll.LV_CreateProgram(192, 64, 1);//根据传的参数创建节目句柄，192是屏宽点数，64是屏高点数，1是屏的颜色(单色)
            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

            nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                MessageBox.Show(ErrStr);
                return;
            }
            LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 40;
            AreaRect.width = 192;
            AreaRect.height = 16;

            LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

            LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;

            LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 2;
            PlayProp.Speed = 4;
            PlayProp.DelayTime = 2;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_STRING, Words, ref FontProp, ref PlayProp,2,1);//通过字符串添加一个多行文本到图文区，参数说明见声明注示



            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = 42;
            AreaRect.height = 32;

            LedDll.LV_AddImageTextArea(hProgram, 1, 2, ref AreaRect, 0);


            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;


            PlayProp.InStyle = 0;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 2, LedDll.ADDTYPE_STRING, Train, ref FontProp, ref PlayProp, 2, 1);//通过字符串添加一个多行文本到图文区，参数说明见声明注示

            AreaRect.left = 58;
            AreaRect.top = 0;
            AreaRect.width = 66;
            AreaRect.height = 32;

            LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
            DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;

            DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
            DigitalClockAreaInfo.ShowStrFont.FontSize = 12;
            DigitalClockAreaInfo.IsShowHour = 1;
            DigitalClockAreaInfo.IsShowMinute = 1;
            DigitalClockAreaInfo.IsShowSecond = 1;
            DigitalClockAreaInfo.TimeFormat = 2;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddDigitalClockArea(hProgram, 1, 3, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示

            AreaRect.left = 140;
            AreaRect.top = 5;
            AreaRect.width = 28;
            AreaRect.height = 22;

            LedDll.LV_AddImageTextArea(hProgram, 1, 4, ref AreaRect, 0);
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddFileToImageTextArea(hProgram, 1, 4, "test.png", ref PlayProp);

            AreaRect.left = 169;
            AreaRect.top = 0;
            AreaRect.width = 23;
            AreaRect.height = 32;

            LedDll.LV_AddImageTextArea(hProgram, 1, 5, ref AreaRect, 0);


            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;


            PlayProp.InStyle = 0;
            //可以添加多个子项到图文区，如下添加可以选一个或多个添加
            nResult = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 5, LedDll.ADDTYPE_STRING, Cnumber, ref FontProp, ref PlayProp, 2, 1);//通过字符串添加一个多行文本到图文区，参数说明见声明注示


            nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                MessageBox.Show(ErrStr);
            }
            else
            {
                // MessageBox.Show("发送成功");
            }
        }


        public void ShowLedNULL()
        {
            int nResult;
            LedDll.COMMUNICATIONINFO CommunicationInfo = new LedDll.COMMUNICATIONINFO();//定义一通讯参数结构体变量用于对设定的LED通讯，具体对此结构体元素赋值说明见COMMUNICATIONINFO结构体定义部份注示
            
            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = configString;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = 1;//LED屏号为1，注意socket通讯和232通讯不识别屏号，默认赋1就行了，485必需根据屏的实际屏号进行赋值
            int hProgram;//节目句柄
            hProgram = LedDll.LV_CreateProgram(192, 64, 1);//根据传的参数创建节目句柄，64是屏宽点数，32是屏高点数，2是屏的颜色，注意此处屏宽高及颜色参数必需与设置屏参的屏宽高及颜色一致，否则发送时会提示错误
            //此处可自行判断有未创建成功，hProgram返回NULL失败，非NULL成功,一般不会失败

            nResult = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目，参数说明见函数声明注示
            if (nResult != 0)
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                MessageBox.Show(ErrStr);
                return;
            }
            LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = 192;
            AreaRect.height = 64;

            LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;
            //int nsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LedDll.FONTPROP));

            nResult = LedDll.LV_QuickAddSingleLineTextArea(hProgram, 1, 1, ref AreaRect, LedDll.ADDTYPE_STRING, " ", ref FontProp, 4);//快速通过字符添加一个单行文本区域，

            nResult = LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
            if (nResult != 0)//如果失败则可以调用LV_GetError获取中文错误信息
            {
                string ErrStr;
                ErrStr = LedDll.LS_GetError(nResult);
                MessageBox.Show(ErrStr);
            }
            else
            {
                //MessageBox.Show("发送成功");
            }
        }
    }
}
