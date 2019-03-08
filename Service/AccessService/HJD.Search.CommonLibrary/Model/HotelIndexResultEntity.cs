using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Model
{
    /// <summary>
    /// 酒店索引库模型
    /// </summary>
    public class HotelIndexResultEntity
    {
        /* 
         * h1.HotelId Id,
         * h1.HotelName,
         * h1.Ename,
         * 
         * h2.HotelDesc,
         * h2.Address,
         * h2.TelePhone,
         * h2.OpenDate,
         * 
         * hc.ContactName,
         * hc.Tel,
         * hc.Fax,
         * hc.Fax2,
         * hc.Financial,
         * hc.FinancialTel,
         * hc.Sales,
         * hc.BankName,
         * hc.BankAccountName,
         * hc.BankAccountNo,
         * hc.SelfBreakfastPrice,
         * hc.Note 
         
         * Themes
         */

        public int Id;

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName;

        /// <summary>
        /// 英文名称
        /// </summary>
        public string Ename;

        /// <summary>
        /// 拼音名称
        /// </summary>
        public string PinyinName;

        /// <summary>
        /// 酒店地址
        /// </summary>
        public string Address;

        /// <summary>
        /// 酒店主题信息
        /// </summary>
        public string Themes;
        
        /// <summary>
        /// 酒店标签信息
        /// </summary>
        public string Featured;

        /// <summary>
        /// 酒店星级
        /// </summary>
        public string Star;

        /// <summary>
        /// 酒店所在目的地
        /// </summary>
        public string DistrictName;

        /// <summary>
        /// 酒店所在区域
        /// </summary>
        public string DistrictZone;

        /// <summary>
        /// 酒店相关POI
        /// </summary>
        public string POI;

        /// <summary>
        /// 酒店点评评分
        /// </summary>
        public double ReviewScore;

        /// <summary>
        /// 酒店设施
        /// </summary>
        public string Facility;

        /// <summary>
        /// 酒店类型
        /// </summary>
        public string Class;

        /// <summary>
        /// 酒店状态
        /// </summary>
        public int HotelState;

        /// <summary>
        /// 酒店直采套餐的相关HID
        /// </summary>
        public int HotelPkHid;

    }
}
