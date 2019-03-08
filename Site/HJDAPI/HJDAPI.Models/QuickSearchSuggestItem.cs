using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 快搜的自动完成数据
    /// </summary>
    [DataContract]
    public class QuickSearchSuggestItem
    {
        public QuickSearchSuggestItem() { GeoScopeType = 1; }
        /// <summary>
        /// 条目ID
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 名称,加高亮
        /// </summary>
        [DataMember]
        public string ShowName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [DataMember]
        public string EName { get; set; }

        /// <summary>
        /// 英文名。Show
        /// </summary>
        [DataMember]
        public string ShowEName { get; set; }

        /// <summary>
        /// 父级名称
        /// </summary>
        [DataMember]
        public string ParentName { get; set; }

        /// <summary>
        /// 标签，逗号分割。 如特惠酒店
        /// </summary>
        [DataMember]
        public string Tag { get; set; }

        /// <summary>
        /// icon url
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        [DataMember]
        public string ActionUrl { get; set; }

       /// <summary>
       /// 实体坐标类型： 1实体坐标   2：指定城市ID的周边  3：坐标对应的城市周边
       /// </summary>
        [DataMember]
        public int GeoScopeType { get; set; }

        [DataMember]
        public double Lat { get; set; }

        [DataMember]
        public double Lon { get; set; }


        public double Score { get; set; }
    }
}
