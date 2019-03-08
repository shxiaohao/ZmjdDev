using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 快搜结果
    /// </summary>
    [DataContract]
    public class QuickSearchResult
    {
        /// <summary>
        /// 城市信息. 关键字匹配目的地时此项有效, 否则为null
        /// </summary>
        [DataMember]
        public CityInfo City { get; set; }

        /// <summary>
        /// 快搜结果列表
        /// </summary>
        [DataMember]
        public List<QuickSearchItem> Results { get; set; }
    }

	/// <summary>
    /// 快搜结果项
    /// </summary>
    [DataContract]
    public class QuickSearchItem
    {
		/// <summary>
		/// ID
		/// </summary>
		[DataMember]
		public long Id { get; set; }

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
        /// 评论总数
        /// </summary>
        [DataMember]
        public int ReviewCount { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        [DataMember]
        public decimal Score { get; set; }

        /// <summary>
        /// 回复总数
        /// </summary>
        [DataMember]
        public int ReplyCount { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [DataMember]
        public string DistrictName { get; set; }
    }
}
