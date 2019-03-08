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
    public class FeaturedCommentEntity
    {
        public FeaturedCommentEntity()
        {
            PicUrl = new List<string>();
        }
        /// <summary>
        /// 酒店编号
        /// </summary>
        [DataMemberAttribute()]
        public int RelInterestID { get; set; }

        /// <summary>
        /// 酒店编号
        /// </summary>
        [DataMemberAttribute()]
        public int TagType { get; set; }

        /// <summary>
        /// 酒店编号
        /// </summary>
        [DataMemberAttribute()]
        public int HotelID { get; set; }

        /// <summary>
        /// 特色标签编号
        /// </summary>
        [DataMemberAttribute()]
        public int FeaturedID { get; set; }

        /// <summary>
        /// 特色标签名称
        /// </summary>
        [DataMemberAttribute()]
        public string FeaturedName { get; set; }
        
        /// <summary>
        /// 点评内容
        /// </summary>
        [DataMemberAttribute()]
        public string Comment { get; set; }

        /// <summary>
        /// 相关点评数，用于前端排序
        /// </summary>
        [DataMemberAttribute()]
        public int CommentCount { get; set; } 
                
        /// <summary>
        /// 特色分类
        /// </summary>
        [DataMemberAttribute()]
        public int CategoryID { get; set; }

        /// <summary>
        /// 特色配图长链接 可能多张
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public List<String> PicUrl { get; set; }

        /// <summary>
        /// 点评(是否推荐)
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsRecommend { get; set; }
    }
}