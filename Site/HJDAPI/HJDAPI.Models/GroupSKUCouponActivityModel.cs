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
    public class GroupSKUCouponActivityModel
    {
        [DataMember]
        public CouponActivityEntity activity { get; set; }


        [DataMember]
        public SKUInfoEntity SKUInfo { get; set; }

        /// <summary>
        /// 团购信息
        /// </summary>
        [DataMember]
        public GroupPurchaseEntity GroupPurchase { get; set; }

        /// <summary>
        /// 当前产品可使用的券
        /// </summary>
        [DataMember]
        public PackageAndProductCouponDefineEntity CouponInfo { get; set; }

        /// <summary>
        /// 团购活动状态
        /// </summary>
        [DataMember]
        public int ActivityState { get; set; }
        
        /// <summary>
        /// 是否参与过团购
        /// </summary>
        [DataMember]
        public bool IsJoinGroup { get; set; }


        /// <summary>
        /// 是否参与过SKU
        /// </summary>
        [DataMember]
        public bool IsJoinSKU { get; set; }
        /// <summary>
        /// 是否发起者
        /// </summary>
        [DataMember]
        public bool IsCreator { get; set; }

    }
}
