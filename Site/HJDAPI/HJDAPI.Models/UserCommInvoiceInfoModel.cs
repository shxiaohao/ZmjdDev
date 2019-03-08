using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class UserCommInvoiceInfoModel
    {
        public UserCommInvoiceInfoModel()
        {
            invoiceType = new List<InvoiceType>();
            ShippingType = new List<string>();
            Title = new List<string>();
            ContactPeople = new List<string>();
            TelPhone = new List<string>();
            Address = new List<string>();
            TaxNumber = new List<string>();
        }
        /// <summary>
        /// 发票类型
        /// </summary>
        [DataMember]
        public List<InvoiceType> invoiceType{ get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        [DataMember]
        public List<string> ShippingType { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMember]
        public List<string> Title { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [DataMember]
        public List<string> ContactPeople { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [DataMember]
        public List<string> TelPhone { get; set; }

        /// <summary>
        /// 收件地址
        /// </summary>
        [DataMember]
        public List<string> Address { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Describe { get; set; }

        /// <summary>
        /// 纳税号
        /// </summary>
        [DataMember]
        public List<string> TaxNumber { get; set; }

        /// <summary>
        /// 开发票日期如：入住完成后，1-5个工作日开具；
        ///（规则适用于入住完成前发起开票申请，若入住完成后申请开具，则计提交申请后1-5个工作日开具。）
        /// </summary>
        [DataMember]
        public string InvoiceDateDescribe { get; set; }

    }

    [DataContract]
    public class InvoiceType
    {
        public InvoiceType()
        {
            TypeName = "";
        }
    
        /// <summary>
        /// 类型id
        /// </summary>
        [DataMember]
        public int TypeId
        {
            get;
            set;
        }

        /// <summary>
        /// 发票类型名称
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }

        /// <summary>
        /// 发票类型描述
        /// </summary>
        [DataMember]
        public string TypeDescribe { get; set; }
    }

}
