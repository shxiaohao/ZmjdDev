using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class InvoiceParamEntity
    {
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 描述连接
        /// </summary>
        [DataMember]
        public string DescriptionURL { get; set; }

        /// <summary>
        /// 开纸质发票 所需费用
        /// </summary>
        [DataMember]
        public decimal PaperInvoicePrice { get; set; }

        /// <summary>
        /// 开纸质发票 所需积分
        /// </summary>
        [DataMember]
        public decimal PaperInvoicePoints { get; set; }

        /// <summary>
        /// 开纸质发票 所需积分描述
        /// </summary>
        [DataMember]
        public string PaperInvoicePointsDesc { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        [DataMember]
        public List<InvoiceType> InvoiceType { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        [DataMember]
        public List<string> ShippingType { get; set; }

        /// <summary>
        /// 提示内容
        /// </summary>
        [DataMember]
        public string Tips { get; set; }


        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMember]
        public List<string> Title { get; set; }

        /// <summary>
        /// 是否可以用积分
        /// </summary>
        [DataMember]
        public bool IsCanPoint { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [DataMember]
        public string Contact { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string TelPhone { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }


        /// <summary>
        /// 发票打印类型
        /// </summary>
        [DataMember]
        public List<TextValueEntity> InvoicePrintType { get; set; }

    }
}
