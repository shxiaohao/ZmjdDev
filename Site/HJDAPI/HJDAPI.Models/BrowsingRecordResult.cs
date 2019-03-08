using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class BrowsingRecordResult
    {
        public BrowsingRecordResult()
        {
            BorwsRecordList = new List<BrowsingRecordItem>();
        }

        public string BlockTitle { get; set; }

        public int TotalCount { get; set; }

        public List<BrowsingRecordItem> BorwsRecordList { get; set; }


    }

    public class BrowsingRecordItem
    {
        public int ID { get; set; }

        public string BrowRecordName { get; set; }

        public string BrowRecordPicUrl { get; set; }

        public decimal BrowRecordNotVipPrice { get; set; }

        public decimal BrowRecordVipPrice { get; set; }

        public string BrowRecordBrief { get; set; }

        public long BrowRecordBizID { get; set; }

        public int BrowRecordBizType { get; set; }

    }
}
