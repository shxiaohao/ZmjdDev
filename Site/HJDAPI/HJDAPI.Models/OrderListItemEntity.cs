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
    public class OrderListItemEntity
    {
        /// <summary>
        /// 订单显示图标
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public long OrderId { get; set; }
        /// <summary>
        /// 订单标题  大标题
        /// </summary>
        [DataMember]
        public string OrderProductName { get; set; }
        /// <summary>
        /// 订单描述 小标题
        /// </summary>
        [DataMember]
        public string OrderProductDesc { get; set; }
        /// <summary>
        /// 订单数量
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }
        /// <summary>
        /// 订单状态名称
        /// </summary>
        [DataMember]
        public string OrderStateName { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime SubmitOrderDate { get; set; }
        /// <summary>
        /// 入住日期或出发日期
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 券过期日期
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 预定日期
        /// </summary>
        [DataMember]
        public DateTime BookDate { get; set; }
        /// <summary>
        /// 入住天数
        /// </summary>
        [DataMember]
        public int NightCount { get; set; }
        /// <summary>
        /// 房间数
        /// </summary>
        [DataMember]
        public int RoomCount { get; set; }
        /// <summary>
        /// 订单类型，对应EnumHelper.OrderType
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }
        /// <summary>
        /// 团状态
        /// </summary>
        [DataMember]
        public int GroupState { get; set; }
        /// <summary>
        /// 团状态名称
        /// </summary>
        [DataMember]
        public string GroupStateName { get; set; }
        /// <summary>
        /// 支付id，用来处理券订单折叠问题
        /// </summary>
        [DataMember]
        public int PayID { get; set; }

        /// <summary>
        /// SKUID
        /// </summary>
        [DataMember]
        public int SKUID { get; set; }



        /// <summary>
        /// 券orderid，大团购产品，根据CouponOrderId分组折叠
        /// </summary>
        [DataMember]
        public long CouponOrderId { get; set; }
        /// <summary>
        /// 是否壳产品 
        /// </summary>
        [DataMember]
        public bool IsPackage { get; set; }


        /// <summary>
        /// 产品类型 0：普通产品，1：大团购产品，2: 打包产品
        /// </summary>
        [DataMember]
        public int ProductType { get; set; }

        /// <summary>
        /// 产品类型 0：普通产品，1：大团购产品.订金
        /// </summary>
        [DataMember]
        public int SubSkuType { get; set; }



        /// <summary>
        /// 补汇款支付连接 ， 如果有，那么是需要补汇款，如果有多个，这个只出一个
        /// </summary>
        [DataMember]
        public string OrderAddPayURL { get; set; }


        /// <summary>
        /// 机酒或游轮上传证件提示，请尽快提交出行人信息，以免耽误你的行程
        /// </summary>
        [DataMember]
        public string OrderDetailTip { get; set; }

        /// <summary>
        /// 是否为赠送产品或子产品
        /// </summary>
        [DataMember]
        public bool IsPromotion { get; set; }


        /// <summary>
        /// 折叠券订单列表
        /// </summary>
        [DataMember]
        public List<DetailOrderListEntity> DetailOrderList { get; set; }

        /// <summary>
        /// 使用积分数
        /// </summary>
        [DataMember]
        public decimal TotalPoints { get; set; }

        /// <summary>
        /// 定金SKU对应的大团购信息
        /// </summary>
        [DataMember]
        public StepGroupEntity StepGroup { get; set; }


        /// <summary>
        /// 券一级分类
        /// </summary>
        [DataMember]
        public int CategoryParentId { get; set; }

        /// <summary>
        /// 兑换类型
        /// </summary>
        [DataMember]
        public int ExchangeMethod { get; set; }

        /// <summary>
        /// 消费券类型 200房券 400vip 500专辑房券 600消费券
        /// </summary>
        [DataMember]
        public int ActivityType { get; set; }

        /// <summary>
        /// 关联套餐专辑
        /// </summary>
        [DataMember]
        public int RelPackageAlbumsID { get; set; }

        /// <summary>
        /// 预约信息
        /// </summary>
        [DataMember]
        public List<BookUserDateInfoEntity> BookUserDateList { get; set; }


        /// <summary>
        /// 是否需要预约
        /// </summary>
        [DataMember]
        public bool IsBook { get; set; }
    }

    [Serializable]
    [DataContract]
    public class DetailOrderListEntity
    {
        /// <summary>
        /// 订单显示图标
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public long OrderId { get; set; }
        /// <summary>
        /// 订单标题 大标题
        /// </summary>
        [DataMember]
        public string OrderProductName { get; set; }
        /// <summary>
        /// 订单描述 小标题
        /// </summary>
        [DataMember]
        public string OrderProductDesc { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }
        /// <summary>
        /// 订单状态名称
        /// </summary>
        [DataMember]
        public string OrderStateName { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        [DataMember]
        public DateTime SubmitOrderDate { get; set; }
        /// <summary>
        /// 入住日期或出发日期
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 券过期日期
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 预定日期
        /// </summary>
        [DataMember]
        public DateTime BookDate { get; set; }
        /// <summary>
        /// 订单类型，对应EnumHelper.OrderType
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }

        /// <summary>
        /// 产品类型 0：普通产品，1：大团购产品.订金
        /// </summary>
        [DataMember]
        public int SubSkuType { get; set; }

        /// <summary>
        /// 团状态
        /// </summary>
        [DataMember]
        public int GroupState { get; set; }
        /// <summary>
        /// 团状态名称
        /// </summary>
        [DataMember]
        public string GroupStateName { get; set; }
        /// <summary>
        /// 支付id，用来处理券订单折叠问题
        /// </summary>
        [DataMember]
        public int PayID { get; set; }
        /// <summary>
        /// 券orderid，大团购产品，根据CouponOrderId分组折叠
        /// </summary>
        [DataMember]
        public long CouponOrderId { get; set; }
        /// <summary>
        /// 是否壳产品 
        /// </summary>
        [DataMember]
        public bool IsPackage { get; set; }
        /// <summary>
        /// 补款连接
        /// </summary>
        [DataMember]
        public string OrderAddPayURL { get; set; }
        /// <summary>
        /// 订单ID,用来处理折叠订单
        /// </summary>
        [DataMember]
        public int ParentOrderID { get; set; }

        /// <summary>
        /// 是否为赠送产品或子产品
        /// </summary>
        [DataMember]
        public bool IsPromotion { get; set; }


        /// <summary>
        /// 使用积分数
        /// </summary>
        [DataMember]
        public decimal TotalPoints { get; set; }

        /// <summary>
        /// 兑换类型
        /// </summary>
        [DataMember]
        public int ExchangeMethod { get; set; }

        /// <summary>
        /// 兑换类型
        /// </summary>
        [DataMember]
        public int ActivityType { get; set; }

        /// <summary>
        /// 关联套餐专辑
        /// </summary>
        [DataMember]
        public int RelPackageAlbumsID { get; set; }

        /// <summary>
        /// 预约信息
        /// </summary>
        [DataMember]
        public List<BookUserDateInfoEntity> BookUserDateList { get; set; }


        /// <summary>
        /// 是否需要预约
        /// </summary>
        [DataMember]
        public bool IsBook { get; set; }
    }
}
