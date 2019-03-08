using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class ExchangeNoBookEntity
    {
        /// <summary>
        /// USERID
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// 产品标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// sku名称
        /// </summary>
        public string SKUName { get; set; }

        /// <summary>
        /// skuID
        /// </summary>
        public int SKUID { get; set; }
        /// <summary>
        /// sku名称
        /// </summary>
        public int CouponID { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public string ExpireTime { get; set; }

        /// <summary>
        /// 预约ID
        /// </summary>
        public int BookID { get; set; }
        /// <summary>
        /// 预约状态 0已预约 1未预约
        /// </summary>
        public int BookState { get; set; }

        /// <summary>
        /// 预约日期
        /// </summary>
        public string BookTime { get; set; }

        /// <summary>
        /// 预约场次
        /// </summary>
        public string BookPlayNum { get; set; }

    }
}
