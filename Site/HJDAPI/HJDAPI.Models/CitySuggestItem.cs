using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 输入城市提示数据
    /// </summary>
    [DataContract]
    public class CitySuggestItem
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 城市英文名
        /// </summary>
        [DataMember]
        public string EName { get; set; }

        /// <summary>
        /// 父级名称
        /// </summary>
        [DataMember]
        public string ParentName { get; set; }

    }
}
