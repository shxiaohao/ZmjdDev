using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class AttractionQueryParam1
    {
        public int districtid { get; set; }//目的地id
        public double lat { get; set; }//用户坐标维度
        public double lng { get; set; }//用户坐标经度
        public int distanceFrom { get; set; }//范围起 (m)
        public int distanceTo { get; set; }//范围至 (m)
        public int sort { get; set; }//排序：缺省：0   距离：1 
        public int order { get; set; }//排序 正序:0 倒序：1
        public int start { get; set; }//开始
        public int count { get; set; }//每次返回数量
        public string themes { get; set; }//景区主题
    }
}
