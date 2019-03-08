using HJD.CouponService.Contracts.Entity;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class SKUCouponActivityDetailModel
    {
        //[DataMember]
        //public StepGroupEntity StepGroup { get; set; }

        [DataMember]
        public CouponActivityEntity activity { get; set; }


        [DataMember]
        public SKUInfoEntity SKUInfo { get; set; }

        [DataMember]
        public List<CommentTag> RelTagList { get; set; }

        /// <summary>
        /// 当前产品可使用的券
        /// </summary>
        [DataMember]
        public PackageAndProductCouponDefineEntity CouponInfo { get; set; }


        /// <summary>
        /// 0 已结束；1 进行中
        /// </summary>
        [DataMember]
        public int activityOpenState { get; set; }

        /// <summary>
        /// 房券关联酒店ID
        /// </summary>
        [DataMember]
        public int relHotelID { get; set; }
        /// <summary>
        /// 已购买数量
        /// </summary>
        [DataMember]
        public int BoughtCount { get; set; }

        /// <summary>
        /// 活动数据
        /// </summary>
         [DataMember]
        public string ActiviyInfo { get; set; }

    }
}
