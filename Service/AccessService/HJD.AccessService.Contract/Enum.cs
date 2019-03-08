using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 快搜智能提示项的类型
    /// </summary>
    public enum SearchType
    {
        [EnumMemberAttribute()]
        /// <summary>
        /// Demo
        /// </summary>
        Demo,

        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店
        /// </summary>
        Hotel,

        [EnumMemberAttribute()]
        /// <summary>
        /// 景点
        /// </summary>
        Spot,

        [EnumMemberAttribute()]
        /// <summary>
        /// 城市
        /// </summary>
        City,

        [EnumMemberAttribute()]
        /// <summary>
        /// 目的地
        /// </summary>
        District,

        [EnumMemberAttribute()]
        /// <summary>
        /// 主题
        /// </summary>
        Theme,

        [EnumMemberAttribute()]
        /// <summary>
        /// 点评
        /// </summary>
        Comment,

        [EnumMemberAttribute()]
        /// <summary>
        /// 品牌
        /// </summary>
        Brand,

        [EnumMemberAttribute()]
        /// <summary>
        /// 检索系统：酒店
        /// </summary>
        QaHotel
    }

    /// <summary>
    /// 索引操作类的操作方式（添加 or 删除）
    /// </summary>
    public enum IndexJobType 
    {
        Add, 
        Remove 
    }

    [Serializable]
    [DataContract]
    public enum QaWordType
    {
        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店名称
        /// </summary>
        HotelName,

        [EnumMemberAttribute()]
        /// <summary>
        /// 目的地
        /// </summary>
        DistrictName,

        [EnumMemberAttribute()]
        /// <summary>
        /// 区域
        /// </summary>
        DistrictZone,

        [EnumMemberAttribute()]
        /// <summary>
        /// 主题
        /// </summary>
        Themes,

        [EnumMemberAttribute()]
        /// <summary>
        /// 标签
        /// </summary>
        Featured,

        [EnumMemberAttribute()]
        /// <summary>
        /// 设施
        /// </summary>
        Facility,

        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店类型
        /// </summary>
        Class,

        [EnumMemberAttribute()]
        /// <summary>
        /// 品牌
        /// </summary>
        Brand,

        [EnumMemberAttribute()]
        /// <summary>
        /// 其它
        /// </summary>
        Other,
        /// <summary>
        /// 
        /// </summary>
        [EnumMemberAttribute]
        Money,
        /// <summary>
        /// 
        /// </summary>
        [EnumMemberAttribute]
        Date,
        [EnumMemberAttribute()]
        /// <summary>
        /// 用户数
        /// </summary>
        UserNum,
        [EnumMemberAttribute()]
        /// <summary>
        /// 
        /// </summary>
        POI,

        [EnumMemberAttribute()]
        /// <summary>
        /// 城市周边
        /// </summary>
        CityAround
    }

    [Serializable]
    [DataContract]
    /// <summary>
    /// 快搜智能提示项的类型
    /// </summary>
    public enum AnswerOptionType
    {
        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店名称
        /// </summary>
        HotelName,

        [EnumMemberAttribute()]
        /// <summary>
        /// 目的地
        /// </summary>
        DistrictName,

        [EnumMemberAttribute()]
        /// <summary>
        /// 区域
        /// </summary>
        DistrictZone,

        [EnumMemberAttribute()]
        /// <summary>
        /// 主题
        /// </summary>
        Themes,

        [EnumMemberAttribute()]
        /// <summary>
        /// 标签
        /// </summary>
        Featured,

        [EnumMemberAttribute()]
        /// <summary>
        /// 设施
        /// </summary>
        Facility,

        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店类型
        /// </summary>
        Class,

        [EnumMemberAttribute()]
        /// <summary>
        /// 品牌
        /// </summary>
        Brand,

        [EnumMemberAttribute()]
        /// <summary>
        /// 其它
        /// </summary>
        Other,
        [EnumMemberAttribute()]
        /// <summary>
        /// 
        /// </summary>
        POI

    }

    [Serializable]
    [DataContract]
    /// <summary>
    /// 快搜智能提示项的类型
    /// </summary>
    public enum QuestionType
    {
        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店查询
        /// </summary>
        HotelSearch,

        [EnumMemberAttribute()]
        /// <summary>
        /// 酒店查询
        /// </summary>
        OrderSearch,

       

        [EnumMemberAttribute()]
        /// <summary>
        /// 其它
        /// </summary>
        Other

    }
}
