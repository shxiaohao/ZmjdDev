using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class InterestHotelsResult
    {
        public InterestHotelsResult() { filters = new HotelListMenu();}
        [DataMember]
        public int TotalCount { get; set; }
        [DataMember]
        public HotelListMenu filters { get; set; }
        [DataMember]
        public WapInterestHotelsResult2 hotels { get; set; }
    }

    [DataContract]
    public class HotelListMenu
    {
        public HotelListMenu()
        {
            ThemeInterestList = new List<InterestEntity>();
            SightInterestList = new List<InterestEntity>();
            DistanceFilters = new List<FilterItem>();
            PriceFilters = new List<FilterItem>();
            StarFilters = new List<FilterItem>();
        }

        [DataMember]
        public int TotalNum { get; set; }
        [DataMember]
        public List<InterestEntity> ThemeInterestList { get; set; }
        [DataMember]
        public List<InterestEntity> SightInterestList { get; set; }
        [DataMember]
        public List<FilterItem> DistanceFilters { get; set; }
        [DataMember]
        public List<FilterItem> PriceFilters { get; set; }
        [DataMember]
        public List<FilterItem> StarFilters { get; set; }

        public void InitMenu()
        {
            DistanceFilters = new List<FilterItem>(){
            new FilterItem(){ Name ="100公里以内", Value="0,100"},
            new FilterItem(){ Name ="100-200公里", Value="100,200"},
            new FilterItem(){ Name ="200-300公里", Value="200,300"}};

            PriceFilters = new List<FilterItem>(){
            new FilterItem(){ Name ="400元以下", Value="0,400"},
            new FilterItem(){ Name ="400-600元", Value="400,600"},
            new FilterItem(){ Name ="600-800元", Value="600,800"},
            new FilterItem(){ Name ="800-1000元", Value="800,1000"},
            new FilterItem(){ Name ="1000-1500元", Value="1000,1500"},
            new FilterItem(){ Name ="1500-2000元", Value="1500,2000"},
            new FilterItem(){ Name ="2000元以上", Value="2000,150000"}};

            StarFilters = new List<FilterItem>(){
                new FilterItem(){ Name ="五星", Value="5"},
                new FilterItem(){ Name ="四星", Value="4"},
                new FilterItem(){ Name ="三星及以下", Value="3"}
            };

            ThemeInterestList = new List<InterestEntity>();
            SightInterestList = new List<InterestEntity>();
            TotalNum = 0;
        }
    }

    [DataContract]
    public class FilterItem
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

    //[DataContract]
    //public class HotelListMenu20
    //{
    //    public HotelListMenu20()
    //    {
    //        DistanceFilters = new List<FilterItem>(){
    //            new FilterItem(){ Name ="100公里以内", Value="0,100"},
    //            new FilterItem(){ Name ="100-200公里", Value="100,200"},
    //            new FilterItem(){ Name ="200-300公里", Value="200,300"}
    //        };

    //        PriceFilters = new List<FilterItem>(){
    //            new FilterItem(){ Name ="500元以下", Value="0,500"},
    //            new FilterItem(){ Name ="500-1000元", Value="500,1000"},
    //            new FilterItem(){ Name ="1000-1500元", Value="1000,1500"},
    //            new FilterItem(){ Name ="1500-2000元", Value="1500,2000"},
    //            new FilterItem(){ Name ="2000元以上", Value="2000,1000000"}
    //        };

    //        StarFilters = new List<FilterItem>(){
    //            new FilterItem(){ Name ="五星", Value="5"},
    //            new FilterItem(){ Name ="四星", Value="4"},
    //            new FilterItem(){ Name ="三星及以下", Value="3"}
    //        };

    //        FacilityFilters = new List<FilterItem>(){                
    //            new FilterItem(){ Name ="室内游泳池", Value="5"},
    //            new FilterItem(){ Name ="可带宠物", Value="4"},
    //            new FilterItem(){ Name ="免费WiFi", Value="3"}
    //        };

    //        HotelTypeFilters = new List<FilterItem>(){
    //            new FilterItem(){ Name ="公寓式酒店", Value="10"},
    //            new FilterItem(){ Name ="别墅", Value="20"},
    //            new FilterItem(){ Name ="民宿", Value="30"}
    //        };

    //        InterestFilters = new List<FilterItem>();

    //        ZoneFilters = new List<FilterItem>();
    //    }

    //    [DataMember]
    //    public List<FilterItem> DistanceFilters { get; set; }

    //    [DataMember]
    //    public List<FilterItem> PriceFilters { get; set; }

    //    [DataMember]
    //    public List<FilterItem> StarFilters { get; set; }
    //}

    [DataContract]
    public class FilterBlockModel
    {
        public FilterBlockModel()
        {
            TagBlocks = new List<FilterTagBlock>();
            HotTagBlocks = new List<FilterTagBlock>();
        }

        /// <summary>
        /// 符合条件要求的所有的标签块
        /// </summary>
        [DataMember]
        public List<FilterTagBlock> TagBlocks { get; set; }

        /// <summary>
        /// 某个地区热门过滤块
        /// </summary>
        [DataMember]
        public List<FilterTagBlock> HotTagBlocks { get; set; }
    }

    [DataContract]
    public class FilterTagBlock
    {
        /// <summary>
        /// 过滤块的标题 1:靠近哪里 2:选择主题 3:酒店类型 4:房间设施
        /// </summary>
        [DataMember]
        public string BlockTitle { get; set; }
        /// <summary>
        /// 过滤块的大类ID  1:靠近哪里 2:选择主题 3:酒店类型 4:房间设施
        /// </summary>
        [DataMember]
        public int BlockCategoryID { get; set; }
        /// <summary>
        /// 标题最少选 默认0
        /// </summary>
        [DataMember]
        public int MinTags { get; set; }
        /// <summary>
        /// 标题最多选 默认100
        /// </summary>
        [DataMember]
        public int MaxTags { get; set; }
        [DataMember]
        public List<FilterTag> Tags { get; set; }
    }

    [DataContract]
    public class FilterTag{

        public FilterTag()
        {
            Name = "";
            PinyinName = "";
            Value = "0";
            IsMatch = false;
        }

        /// <summary>
        /// 过滤块的大类ID  1:靠近哪里 2:选择主题 3:酒店类型 4:房间设施
        /// </summary>
        [DataMember]
        public int BlockCategoryID { get; set; }
        /// <summary>
        /// 标签汉字名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 标签的拼音名字
        /// </summary>
        [DataMember]
        public string PinyinName { get; set; }
        /// <summary>
        /// 过滤块的大类ID  1:靠近哪里 2:选择主题 3:酒店类型 4:房间设施
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// 是否匹配搜索
        /// </summary>
        [DataMember]
        public bool IsMatch { get; set; }
        /// <summary>
        /// 关联的hotelCount
        /// </summary>
        [DataMember]
        public int HotelCount { get; set; }
    }
}