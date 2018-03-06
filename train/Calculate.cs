using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    public class Calculate

    {
        private const double EARTH_RADIUS = 6378137.0;//地球半径，单位M
        private const double PI =  Math.PI;//圆周率
        /// <summary>
        /// 已知两点的经纬度计算两点间的距离
        /// </summary>
        /// <param name="lng1"></param>
        /// <param name="lat1"></param>
        /// <param name="lng2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public double Distance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = getRad(lat1);
            double radLng1 = getRad(lng1);
            double radLat2 = getRad(lat2);
            double radLng2 = getRad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS/1000;
            return Math.Round(result, 4);//单位KM
        }


        /// <summary>
        /// 度化弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public double getRad(double d)
        {
            return d*PI/180.0;
        }

        /// <summary>
        /// 实时计算速度
        /// </summary>
        /// <param name="t"></param>时间（计时器）
        /// <param name="mv"></param>最大速度
        /// <param name="a"></param>加速度
        /// <param name="D"></param>距离
        /// <returns></returns>
        public double getSpeed(double t, double mv, double a, double D)
        {
            double v = 0;
            mv = mv / 3.6;
            D = D * 1000;
            if(D < ( mv * mv / a ))
            {//两站之间的距离过于短，列车加速不到最大速度就开始减速(距离小于两倍的加速阶段的距离)
                if (t > 0 &&  t <= (System.Math.Sqrt(D / a))) { //加速阶段
                    v = a * t;
                }
                else if (t > (System.Math.Sqrt(D / a)) && t <= 2*System.Math.Sqrt(D / a))
                { //减速阶段
                    v = System.Math.Sqrt(a * D) - a * ( t - System.Math.Sqrt(D / a));
                }
                else { //其他
                
                }
            }
            else if (D == (mv * mv / a))
            { //两站之间的距离刚刚好，列车加速到最大速度后就马上开始减速(距离等于两倍的加速阶段的距离)
                if (t > 0 && t <= mv / a)
                { //加速阶段
                    v = a * t;
                }
                else if (t > mv / a && t <= 2 * mv / a)
                { //减速阶段
                    v = mv - a * ( t - mv / a);
                }
                else { //其他
                
                }
            }
            else if (D > (mv * mv / a))
            {//两站之间的距离足够大，列车能够完成加速、匀速、减速的过程(距离大于两倍的加速阶段的距离)
                if (t > 0 && (t <= mv / a))
                {//加速阶段
                    v = a * t;
                }
                else if (t > mv / a && (t <= (D / mv)))
                {//匀速阶段
                    v = mv;
                }
                else if (t > (D / mv) && t <= (D / mv + mv / a))
                {//减速阶段
                    v = mv - a *( t - (D / mv));
                }
                else
                {//其他

                }
            }
            else
            { //其他
            
            }
            return Math.Round(v * 3.6,1);
        }

        /// <summary>
        /// 计算到站距离
        /// </summary>
        /// <param name="t"></param>时间（计时器）
        /// <param name="mv"></param>最大速度
        /// <param name="a"></param>加速度
        /// <param name="D"></param>距离
        /// <returns></returns>
        public double getDistance(double t, double mv, double a, double D)
        {
            double l = 0;
            mv = mv / 3.6;
            D = D * 1000;
            if (D < (mv * mv / a))
            {//两站之间的距离过于短，列车加速不到最大速度就开始减速(距离小于两倍的加速阶段的距离)
                if (t > 0 && t <= (System.Math.Sqrt(D / a)))
                { //加速阶段
                    l = D - a * t * t / 2;
                }
                else if (t > (System.Math.Sqrt(D / a)) && t <= 2 * System.Math.Sqrt(D / a))
                { //减速阶段
                    l = D / 2 - a * (t - System.Math.Sqrt(D / a)) * (t - System.Math.Sqrt(D / a)) / 2;
                }
                else
                { //其他

                }
            }
            else if (D == (mv * mv / a))
            { //两站之间的距离刚刚好，列车加速到最大速度后就马上开始减速(距离等于两倍的加速阶段的距离)
                if (t > 0 && t <= mv / a)
                { //加速阶段
                    l = a * t * t / 2;
                }
                else if (t > mv / a && t <= 2 * mv / a)
                { //减速阶段
                    l = D / 2 - a * (t - mv / a) * (t - mv / a) / 2;
                }
                else
                { //其他

                }
            }
            else if (D > (mv * mv / a))
            {//两站之间的距离足够大，列车能够完成加速、匀速、减速的过程(距离大于两倍的加速阶段的距离)
                if (t > 0 && (t <= mv / a))
                {
                    //Console.WriteLine("起步加速")
                    l = D - a * t * t * 0.5;
                }
                else if (t > (mv / a) && (t <= (D / mv)))
                {
                    //Console.WriteLine("匀速")
                    l = D - mv * t + mv * mv / a * 0.5;
                }
                else if (t > (D / mv) && t <= (D / mv + mv / a))
                {
                    //Console.WriteLine("停车减速")
                    l = a * (D / mv + mv / a - t) * (D / mv + mv / a - t) * 0.5;
                }
                else
                {
                    //Console.WriteLine("其他")
                    //l=D;
                };
            }
            else 
            {//其他
            
            }
            return Math.Round(l / 1000,4);
        }

        public string getCoordinate(double x1, double y1, double x2, double y2, double D)
        {
            string result = null;
            double x, y, k,b;
            D = D / 111.31955;
            k = (y2 - y1) / (x2 - x1);
            b = (x2 * y1 - x1 * y2) / (x2 - x1);
            x = x2 - D / Math.Sqrt(1 + k * k);
            y = k * x + b;
            result = x + "," + y;
            return result;
        }
    }
}
