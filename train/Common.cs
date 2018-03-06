using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    public class Common
    {
        //站名
        public String StationName { get; set; }


        //站英文名
        public String StationEnName { get; set; }

        //隶属站次
        public int SubjectStation { get; set; }

        //经度
        public double lng { get; set; }

        //纬度
        public double lat { get; set; }
        
        //上一站名
        public String LastStationName { get; set; }


    }
}
