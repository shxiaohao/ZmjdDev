using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class ChannelMineModel
    {
        public string NickName;
        public Decimal CanRequireAmount;
        public Decimal NotRequireAmount;
        public string CurBankAccount;
        /// <summary>
        /// 待入账
        /// </summary>
        public Decimal WaitAmount { get; set; }
        /// <summary>
        /// 待提现
        /// </summary>
        public Decimal NotPutAmount { get; set; }
        /// <summary>
        /// 提现中
        /// </summary>
        public Decimal PutingAmount { get; set; }
        /// <summary>
        /// 已提现
        /// </summary>
        public Decimal AlreadyAmounted { get; set; }

        /// <summary>
        /// 当日订单金额
        /// </summary>
        public Decimal NowDayOrderPrice { get; set; }

        /// <summary>
        /// 当日佣金
        /// </summary>
        public Decimal NowDayReward { get; set; }

    }
}
