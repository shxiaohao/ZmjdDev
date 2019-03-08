using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 特色标签For酒店列表查询参数 | For酒店详情页
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class FilterDicEntity
    {
        /// <summary>
        /// 特色标签编号
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long Key { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Type { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Num { get; set; }

        /// <summary>
        /// 特色标签名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Name { get; set; }        
    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class SearchHotelListFilterTagInfoParam
    {
        /// <summary>
        /// 区域ID集合
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<int> zoneids { get; set; }

        /// <summary>
        /// 主题特色ID集合
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<int> interestids { get; set; }

        /// <summary>
        /// 酒店类型ID集合
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<int> classids { get; set; }

        /// <summary>
        /// 设施ID集合
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<int> facilitys { get; set; }

        /// <summary>
        /// 出游类型ID集合
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<int> triptypeids { get; set; }

        /// <summary>
        /// 其他查询标签集合
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<int> featuredtreeids { get; set; }
    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelFilterColEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long FilterCol { get; set; }

        /// <summary>
        /// 酒店Id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 标签大类ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CategoryID { get; set; }

        /// <summary>
        /// 标签大类名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CategoryName { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Name { get; set; }

        /// <summary>
        /// 如果是特色内容需要代表性的描述
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Comment { get; set; }

        /// <summary>
        /// 涉及点评数目
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CommentCount { get; set; }

        /// <summary>
        /// 特征ID 对应featuredtree表ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }
    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class DistrictHotFilterTagEntity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DistrictId { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CategoryId { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Value { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelCount { get; set; }
    }

    [Serializable]
    [DataContract]
    public enum BindZoneHotelRelType
    {
        /// <summary>
        /// zmjd的酒店(由划分区域和酒店的地理位置计算出来)
        /// </summary>
        [EnumMember]
        OnlyZMJD,

        /// <summary>
        /// xc的酒店(直接和xc绑定的酒店关联)
        /// </summary>
        [EnumMember]
        OnlyCtrip,

        /// <summary>
        /// zmjd和xc合并
        /// </summary>
        [EnumMember]
        BothZMJDAndCtrip
    }

    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class HotelRoomTypeFilterTagEntity
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }
        /// <summary>
        /// 房型ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RoomID { get; set; }
        /// <summary>
        /// 房型名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RoomName { get; set; }
        /// <summary>
        /// 特征ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }
        /// <summary>
        /// 特征名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string Name { get; set; }
        /// <summary>
        /// 点评数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int CommentCount { get; set; }
    }
}