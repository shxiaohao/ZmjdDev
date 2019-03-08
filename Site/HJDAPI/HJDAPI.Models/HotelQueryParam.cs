using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class HotelListQueryParam
    {
        public int districtid { get; set; }//目的地id
        public float lat { get; set; }//目的地维度
        public float lng { get; set; }//目的地经度
        public int fromDistance { get; set; } //范围 从 (m)
        public int distance { get; set; }//范围 到(m)
        public int geoScopeType { get; set; } //地理范围类型 1：目的地 2：目的地周边  3：坐标附近
        public int hotelid { get; set; } //
        public int type { get; set; }//酒店分类 全部:0{get;set;}度假:1001{get;set;}快捷:1002{get;set;} 客栈/农家乐:1003
        public int sort { get; set; }//排序：口碑：0  价格：1 距离：2 
        public int order { get; set; }//排序 正序:0{get;set;}倒序：1
        public int start { get; set; }//开始
        public int count { get; set; }//每次返回数量
        public string checkin { get; set; }//入店日期     如果是0则忽略
        public string checkout { get; set; }//离店日期
        public double nLat { get; set; }//范围：东北维度点  如果是0则忽略
        public double nLng { get; set; }//东北经度度点
        public double sLat { get; set; }//西南维度点
        public double sLng { get; set; }//西南经度点{get;set;} int distance
        public int valued { get; set; } //超值酒店  0:不过滤超值， 1：超值优先
        public string tag { get; set; } //特色标签
        public int minPrice { get; set; }//最低价 
        public int maxPrice { get; set; }//最高价。 0：不限止价格，-1:不需要价格（测试时不需要返回价格信息）
        public string location { get; set; }//行政区
        public string zone { get; set; }//商业区
        public string brand { get; set; }//品牌
        public int attraction { get; set; } //景区ID
        public int hotelTheme { get; set; } //酒店主题ID
        public string featured { get; set; }//酒店特色
        public int Interest { get; set; } //酒店玩点ID
        public string InterestPlace { get; set; } //酒店玩点属地ID
        public int star { get; set; } //星级
    }

    [Serializable]
    [DataContract]
    public class HotelListQueryParam20
    {
        [DataMember]
        public int aroundCityId { get; set; }
        [DataMember]
        public int districtid { get; set; }//目的地id
        [DataMember]
        public string districtName { get; set; }//目的地名称
        [DataMember]
        public float lat { get; set; }//用户定位纬度 
        [DataMember]
        public float lng { get; set; }//用户定位经度
        [DataMember]
        public float districtLat { get; set; }//目的地纬度
        [DataMember]
        public float districtLng { get; set; }//目的地经度
        [DataMember]
        public int distance { get; set; }//多少(m)范围内
        [DataMember]
        public int geoScopeType { get; set; } //地理范围类型 1：目的地  2：目的地周边  3：坐标附近
        [DataMember]
        public int sort { get; set; }//排序 口碑：0; 价格：1; 距离：2; 标签匹配度：20
        [DataMember]
        public int order { get; set; }//排序 正序：0 倒序：1 
        [DataMember]
        public int start { get; set; }//从第几条开始
        [DataMember]
        public int count { get; set; }//每次返回数量
        [DataMember]
        public string checkin { get; set; }//入店日期,如果是0则忽略
        [DataMember]
        public string checkout { get; set; }//离店日期,如果是0则忽略
        [DataMember]
        public int minPrice { get; set; }//最低价。
        [DataMember]
        public int maxPrice { get; set; }//最高价。 0：不限制价格 -1：不需要价格(测试时不需要返回价格信息 maxPrice输入-1)
        [DataMember]
        public string star { get; set; }//酒店星级 ""不限制 其他限制好了
        [DataMember]
        public int peopleCount { get; set; }//0不限制 其他1 2 3代表几个人
        [DataMember]
        public string tripType { get; set; }//出游类型codesn
        [DataMember]
        public string sType { get; set; }//来自客户端: mobile ios android
        [DataMember]
        public int interest { get; set; } //主题
        [DataMember]
        public int zoneId { get; set; } //(地图圈出的)区域ID
        [DataMember]
        public int hotelId { get; set; } //酒店ID
        [DataMember]
        public List<FilterTag> FilterTags { get; set; }
        /// <summary>
        /// 是否需要酒店列表
        /// </summary>
        [DataMember]
        public bool IsNeedHotelList { get; set; }
        /// <summary>
        /// 是否需要过滤条件列表
        /// </summary>
        [DataMember]
        public bool IsNeedFilterCol { get; set; }
        /// <summary>
        /// 只需要显示最低列表价（标准酒店列表如果是我们的套餐，会显示关联推荐套餐的列表价；如果为true,则不需要显示关联套餐列表价）
        /// </summary>
        [DataMember]
        public bool JustMinPricePlan { get; set; }
        /// <summary>
        /// 地图区域左上角纬度
        /// </summary>
        [DataMember]
        public float Top_Left_Lat { get; set; }
        /// <summary>
        /// 地图区域左上角经度
        /// </summary>
        [DataMember]
        public float Top_Left_Lng { get; set; }
        /// <summary>
        /// 地图区域右上角纬度
        /// </summary>
        [DataMember]
        public float Top_Right_Lat { get; set; }
        /// <summary>
        /// 地图区域右上角经度
        /// </summary>
        [DataMember]
        public float Top_Right_Lng { get; set; }
        /// <summary>
        /// 地图区域左下角纬度
        /// </summary>
        [DataMember]
        public float Bottom_Left_Lat { get; set; }
        /// <summary>
        /// 地图区域左下角经度
        /// </summary>
        [DataMember]
        public float Bottom_Left_Lng { get; set; }
        /// <summary>
        /// 地图区域右下角纬度
        /// </summary>
        [DataMember]
        public float Bottom_Right_Lat { get; set; }
        /// <summary>
        /// 地图区域右下角经度
        /// </summary>
        [DataMember]
        public float Bottom_Right_Lng { get; set; }
        /// <summary>
        /// 是否支持WebP
        /// </summary>
        [DataMember]
        public bool SupportWebP { get; set; }
    }
}