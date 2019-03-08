using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Params
{

    /*
     Index/搜索：Keywords/HotelID
     */

    /// <summary>
    /// 行为参数基础类
    /// </summary>
    public class BehaviorParams
    {
        public Guid ID = Guid.NewGuid();

        public string AppKey;
        public string ClientType;

        public DateTime RecordTime;
    }

    #region 页面加载



    #endregion

    #region 搜索

    /// <summary>
    /// 首页搜索酒店
    /// </summary>
    public class BehaviorIndexSearchHotel : BehaviorParams
    {
        public string Keywords;
        public string SearchHotelID;
    }

    /// <summary>
    /// 酒店列表页搜索酒店
    /// </summary>
    public class BehaviorZoneSearchHotel : BehaviorParams
    {
        public string Zone;
        public string Theme;
        public string Sight;
        public string Star;
        public string Price;

        public string Keywords;
        public string SearchHotelID;
    }

    /// <summary>
    /// 酒店页搜索酒店
    /// </summary>
    public class BehaviorHotelSearchHotel : BehaviorParams
    {
        public string HotelID;

        public string Keywords;
        public string SearchHotelID;
    }

    /// <summary>
    /// 套餐页搜索酒店
    /// </summary>
    public class BehaviorPackageSearchHotel : BehaviorParams
    {
        public string HotelID;
        public string CheckIn;
        public string CheckOut;

        public string Keywords;
        public string SearchHotelID;
    }

    /// <summary>
    /// 结算页搜索酒店
    /// </summary>
    public class BehaviorBookSearchHotel : BehaviorParams
    {
        public string HotelID;
        public string Package;
        public string CheckIn;
        public string CheckOut;

        public string Keywords;
        public string SearchHotelID;
    }

    #endregion

    #region 特惠专享



    #endregion

    #region 套餐购买



    #endregion
}
