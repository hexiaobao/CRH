using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace train
{
    public class DateView
    {
        //站名
        public String StationName { get; set; }

        //站英文名
        public String StationEnName { get; set; }

        //停留时间
        public double stopTime { get; set; }

        //经度
        public double lng { get; set; }

        //纬度
        public double lat { get; set; }

        //距离
        public double distance { get; set; }
    }
}
