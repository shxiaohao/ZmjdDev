using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices
{
    [DataContract]
    public class HotelSearchParas
    {
        #region DistrictID
        private int intDistrictID = -1;
        /// <summary>
        /// Gets or sets DistrictID
        /// </summary>
        [DataMember]
        public int DistrictID
        {
            get { return this.intDistrictID; }
            set { this.intDistrictID = value; }
        }
        #endregion

        #region AroundCityID
        private int intAroundCityID = -1;
        /// <summary>
        /// Gets or sets DistrictID
        /// </summary>
        [DataMember]
        public int AroundCityID
        {
            get { return this.intAroundCityID; }
            set { this.intAroundCityID = value; }
        }
        #endregion

        #region Zone
        private int[] intZone;
        /// <summary>
        /// Gets or sets Zone
        /// </summary>
        [DataMember]
        public int[] Zone
        {
            get { return this.intZone; }
            set { this.intZone = value; }
        }
        #endregion

        #region Location
        private int[] intLocation;
        /// <summary>
        /// Gets or sets Location
        /// </summary>
        [DataMember]
        public int[] Location
        {
            get { return this.intLocation; }
            set { this.intLocation = value; }
        }
        #endregion

        #region Brands
        private int[] aryBrand;
        /// <summary>
        /// Gets or sets Brand
        /// </summary>
        [DataMember]
        public int[] Brands
        {
            get { return this.aryBrand; }
            set { this.aryBrand = value; }
        }
        #endregion

        #region Facilitys
        private int[] aryFacility;
        /// <summary>
        /// Gets or sets Facilitys
        /// </summary>
        [DataMember]
        public int[] Facilitys
        {
            get { return this.aryFacility; }
            set { this.aryFacility = value; }
        }
        #endregion

        #region Sort
        private int intSortType = 1;
        /// <summary>
        /// Gets or sets SortType（1--口碑 2--价格）
        /// </summary>
        [DataMember]
        public int SortType
        {
            get { return this.intSortType; }
            set { this.intSortType = value; }
        }

        private int intSortDirection = 1;
        /// <summary>
        /// Gets or sets SortDirection（1--升序 2--降序）
        /// </summary>
        [DataMember]
        public int SortDirection
        {
            get { return this.intSortDirection; }
            set { this.intSortDirection = value; }
        }
        #endregion

        /// <summary>
        /// 酒店分类 (包括一到多个酒店明细分类）
        /// </summary>
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// 酒店明细分类
        /// </summary>
        [DataMember]
        public int[] ClassID { get; set; }

        [DataMember]
        public int[] Class2ID { get; set; }

        [DataMember]
        public int StartIndex { get; set; }
        [DataMember]
        public int ReturnCount { get; set; }
        [DataMember]
        public Double Lat { get; set; }
        [DataMember]
        public Double Lng { get; set; }
        [DataMember]
        public int Distance { get; set; }
        /// <summary>
        /// 左上纬度
        /// </summary>
        [DataMember]
        public Double nLat { get; set; }
        /// <summary>
        /// 左上经度
        /// </summary>
        [DataMember]
        public Double nLng { get; set; }
        /// <summary>
        /// 左下纬度
        /// </summary>
        [DataMember]
        public Double sLat { get; set; }
        /// <summary>
        /// 左下经度
        /// </summary>
        [DataMember]
        public Double sLng { get; set; }
        /// <summary>
        /// 右上纬度
        /// </summary>
        [DataMember]
        public Double n1Lat { get; set; }
        /// <summary>
        /// 右上经度
        /// </summary>
        [DataMember]
        public Double n1Lng { get; set; }
        /// <summary>
        /// 右下纬度
        /// </summary>
        [DataMember]
        public Double s1Lat { get; set; }
        /// <summary>
        /// 右下经度
        /// </summary>
        [DataMember]
        public Double s1Lng { get; set; }
       
        [DataMember]
        public int NearbyGroupID { get; set; }

        [DataMember]
        public int NearbyPOIID { get; set; }

       //初始酒店集 ，如此刻附近酒店集
        [DataMember]
        public string InitHotelList { get; set; }

        /// <summary>
        /// 超值酒店
        /// </summary>
        [DataMember]
        public int Valued { get; set; }

        /// <summary>
        /// 频道页显示酒店
        /// </summary>
        [DataMember]
        public int ChannelHotel { get; set; }

        #region 酒店标签  //高性价比等。。。
        [DataMember]
        public int[] TagIDs { get; set; }
        #endregion

        #region 入住&离店日期

        [DataMember]
        public DateTime CheckInDate { get; set; }
        
        [DataMember]
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// 标识当前查询条件是否指定了入住日期（true指定，则使用参数中的CheckInDate | false不指定，则忽略参数中的CheckInDate）
        /// </summary>
        [DataMember]
        public bool FixDate { get; set; }
        
        #endregion

        #region 酒店价格区间
        [DataMember]
        public decimal MinPrice { get; set; }

        [DataMember]
        public decimal MaxPrice { get; set; }
        #endregion

        #region 星级
        [DataMember]
        public int[] Star { get; set; }
        #endregion

        #region 出游类型
        [DataMember]
        public int[] TripType { get; set; }
        #endregion

        private bool needFilterCol = true;
        /// <summary>
        /// 是否需要酒店属性统计值
        /// </summary>
        [DataMember]
        public bool NeedFilterCol
        {
            get { return this.needFilterCol; }
            set { this.needFilterCol = value; }
        }

        private bool needHotelID = true;
        /// <summary>
        /// 是否需要返回酒店ID列表 
        /// </summary>
        [DataMember]
        public bool NeedHotelID
        {
            get { return this.needHotelID; }
            set { this.needHotelID = value; }
        }

        [DataMember]
        public int Attraction
        {
            get;
            set;
        }

        [DataMember]
        public int HotelTheme
        {
            get;
            set;
        }
        
        /// <summary>
        /// 酒店特色
        /// </summary>
        [DataMember]
        public int[] Featured
        {
            get;
            set;
        }

        /// <summary>
        /// 酒店玩点ID
        /// </summary>
        [DataMember]
        public int Interest
        {
            get;
            set;
        }

        /// <summary>
        /// 酒店玩点属地ID
        /// </summary>
        [DataMember]
        public int[] InterestPlace
        {
            get;
            set;
        }

        /// <summary>
        /// 同一个district内定义的zone（包括商圈景区）
        /// </summary>
        [DataMember]
        public int[] ZonePlaces
        {
            get;
            set;
        }

        /// <summary>
        /// ZonePlaceID 限定酒店出现在某个区域内
        /// </summary>
        [DataMember]
        public int ZonePlaceID
        {
            get;
            set;
        }

        /// <summary>
        /// 酒店设施列表（维护在hotelContacts里面）
        /// </summary>
        [DataMember]
        public int[] HotelFacilitys
        {
            get;
            set;
        }

        /// <summary>
        /// 主题ID数组
        /// </summary>
        [DataMember]
        public int[] InterestArray
        {
            get;
            set;
        }

        /// <summary>
        /// 出游类型数组
        /// </summary>
        [DataMember]
        public int[] TripTypeArray { get; set; }

        /// <summary>
        /// 特色标签树
        /// </summary>
        [DataMember]
        public int[] FeaturedTreeArray { get; set; }

        /// <summary>
        /// 酒店状态
        /// </summary>
        [DataMember]
        public int[] HotelState
        {
            get;
            set;
        }
    }
}