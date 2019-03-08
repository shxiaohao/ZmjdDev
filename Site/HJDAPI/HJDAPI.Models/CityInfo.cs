using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    [DataContract]
    public class CityInfo
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
		/// 城市图片
		/// </summary>
		[DataMember]
		public string Picture { get; set; }

        /// <summary>
        /// 城市酒店总数
        /// </summary>
        [DataMember]
        public int HotelCount { get; set; }

        /// <summary>
        /// 城市景点总数
        /// </summary>
        [DataMember]
        public int SightCount { get; set; }

        /// <summary>
        /// 城市问答总数
        /// </summary>
        [DataMember]
        public int QaCount { get; set; }
    }
}
