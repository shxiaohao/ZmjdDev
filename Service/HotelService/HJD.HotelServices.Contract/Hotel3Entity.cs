
namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class Hotel3Entity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int ID { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelID { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Type { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string Description { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string items { get; set; }
    }
}

