using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Dialog.EasemobPushEvents
{


    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Agent
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string nickname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<string> roles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string business_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long created_at { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long  updated_at { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class From
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string type { get; set; }

    }



    //    [System.SerializableAttribute()]   [System.Runtime.Serialization.DataContractAttribute()]   public  class Channel
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Runtime.Serialization.DataMemberAttribute()] public int id { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Runtime.Serialization.DataMemberAttribute()] public string name { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Runtime.Serialization.DataMemberAttribute()] public string type { get; set; }

    //}



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Size
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int height { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Body
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string secret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Size size { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Message
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string service_session_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public From from { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string origin_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Channel channel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Body body { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Timestamp
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long create { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long stop { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Channel
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int id { get; set; }

        /// <summary>
        /// 体验关联
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string type { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Channel_user
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string app_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string channel_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string im_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string org_name { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Visitor
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Channel_user channel_user { get; set; }

    }



    //    [System.SerializableAttribute()]   [System.Runtime.Serialization.DataContractAttribute()]   public  class Agent
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Runtime.Serialization.DataMemberAttribute()] public string id { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Runtime.Serialization.DataMemberAttribute()] public string name { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Runtime.Serialization.DataMemberAttribute()] public string type { get; set; }

    //}



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Agent_queue
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int id { get; set; }

        /// <summary>
        /// 集体组
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Summary_categoryItem
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int id { get; set; }

        /// <summary>
        /// 测试4-4
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Enquriy
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string comment { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class ServiceSession
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string state { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string origin_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Timestamp timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Channel channel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Visitor visitor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Agent agent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Agent_queue agent_queue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<Summary_categoryItem> summary_category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Enquriy enquriy { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class Payload
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Agent agent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long logoutTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long  stateChangeTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceSession serviceSession { get; set; }

    }



    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    public class EasemobPushEventsEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string eventId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Payload payload { get; set; }

    }
}
