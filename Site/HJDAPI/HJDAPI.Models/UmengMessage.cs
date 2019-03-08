using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class UmengMessagePostBaseParam
    {
        public UmengMessagePostBaseParam()
        {
            appkey = "";
            timestamp = "";
            type = "";
            device_tokens = "";
            alias_type = "";
            alias = "";
            file_id = "";
            production_mode = "";
            description = "";
            thirdparty_id = "";
            policy = new UmengMessagePolicy();
        }

        /// <summary>
        /// 必填 应用唯一标识
        /// </summary>
        [DataMember]
        public string appkey { get; set; }

        //必填 时间戳，
        //10位或者13位均可，时间戳有效期为10分钟
        [DataMember]
        public string timestamp { get; set; }

        //必填 消息发送类型,其值可以为:
        //unicast-单播
        //listcast-列播(要求不超过500个device_token)
        //filecast-文件播
        //(多个device_token可通过文件形式批量发送）
        //broadcast-广播
        //groupcast-组播
        //(按照filter条件筛选特定用户群, 具体请参照filter参数)
        //customizedcast(通过开发者自有的alias进行推送), 
        //包括以下两种case:
        //- alias: 对单个或者多个alias进行推送
        //- file_id: 将alias存放到文件后，根据file_id来推送              
        [DataMember]
        public string type { get; set; }

        // 可选 设备唯一表示
        //当type=unicast时,必填, 表示指定的单个设备
        //当type=listcast时,必填,要求不超过500个,
        //多个device_token以英文逗号间隔
        [DataMember]
        public string device_tokens { get; set; }

        //可选 当type=customizedcast时，必填，alias的类型, 
        //alias_type可由开发者自定义,开发者在SDK中
        //调用setAlias(alias, alias_type)时所设置的alias_type
        [DataMember]
        public string alias_type { get; set; }

        //可选 当type=customizedcast时, 开发者填写自己的alias。
        //要求不超过50个alias,多个alias以英文逗号间隔。
        //在SDK中调用setAlias(alias, alias_type)时所设置的alias
        [DataMember]
        public string alias { get; set; }

        // 可选 当type=filecast时，file内容为多条device_token, 
        //device_token以回车符分隔
        //当type=customizedcast时，file内容为多条alias，
        //alias以回车符分隔，注意同一个文件内的alias所对应
        //的alias_type必须和接口参数alias_type一致。
        //注意，使用文件播前需要先调用文件上传接口获取file_id, 
        //具体请参照"2.4文件上传接口"
        [DataMember]
        public string file_id { get; set; }

        //可选 "true/false" 正式/测试模式。
        //测试模式下，广播/组播只会将消息发给测试设备。
        //测试设备需要到web上添加。Android: 测试设备属于正式设备的一个子集。
        [DataMember]
        public string production_mode { get; set; }

        /// <summary>
        /// 可选 发送消息的描述，建议填写。
        /// </summary>
        [DataMember]
        public string description { get; set; }

        //可选 开发者自定义消息标识ID, 开发者可以为同一批发送的多条消息。
        //提供同一个thirdparty_id, 便于友盟后台后期合并统计数据。 
        [DataMember]
        public string thirdparty_id { get; set; }

        // 可选 发送策略
        [DataMember]
        public UmengMessagePolicy policy { get; set; }

        // 可选 发送策略
        [DataMember]
        public UmengMessageFilter filter { get; set; }
    }

    [DataContract]
    public class UmengAndroidPostParam : UmengMessagePostBaseParam
    {
        [DataMember]
        public UmengMessageAndroidPayLoad payload { get; set; }
    }

    [DataContract]
    public class UmengiOSPostParam : UmengMessagePostBaseParam
    {
        [DataMember]
        public UmengMessageiOSPayLoad payload { get; set; }
    }

    //必填 消息内容(iOS最大为2012B), 包含参数说明如下(JSON格式):
    [DataContract]
    public class UmengMessageiOSPayLoad
    {
        [DataMember]
        public UmengMessageiOSBody aps { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string action { get; set; }

        [DataMember]
        public bool isShowInApp { get; set; }

        [DataMember]
        public string bgColor { get; set; }

        /// <summary>
        /// 控制显示的秒数
        /// </summary>
        [DataMember]
        public float timeElapse { get; set; }

        [DataMember]
        public string btnName { get; set; }

        /// <summary>
        /// 接收消息的userId
        /// </summary>
        [DataMember]
        public long receiverId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [DataMember]
        public string msgType { get; set; }
    }

    // 必填 严格按照APNs定义来填写
    [DataContract]
    public class UmengMessageiOSBody
    {
        public UmengMessageiOSBody()
        {
            alert = "";
            sound = "";
            category = "";
        }

        //必填
        [DataMember]
        public string alert { get; set; }
        //可选
        [DataMember]
        public int badge { get; set; }
        //可选
        [DataMember]
        public string sound { get; set; }
        ////可选
        //[DataMember]
        //public int content-available { get; set; }
        //可选 ios8才支持该字段
        [DataMember]
        public string category { get; set; }
    }

    //必填 消息内容(Android最大为1840B), 包含参数说明如下(JSON格式):
    [DataContract]
    public class UmengMessageAndroidPayLoad
    {
        public UmengMessageAndroidPayLoad()
        {
            display_type = "";
            body = new UmengMessageAndroidBody();
            extra = new UmengMessageExtra();
        }
        // 必填 消息类型，值可以为: notification-通知，message-消息
        [DataMember]
        public string display_type { get; set; }
        [DataMember]
        public UmengMessageAndroidBody body { get; set; }
        /// <summary>
        /// 补充信息
        /// </summary>
        [DataMember]
        public UmengMessageExtra extra { get; set; }
    }

    // 必填 消息体。
    //display_type=message时,body的内容只需填写custom字段。
    //display_type=notification时, body包含如下参数:
    [DataContract]
    public class UmengMessageAndroidBody
    {
        public UmengMessageAndroidBody()
        {
            ticker = "";
            title = "";
            text = "";

            icon = "";
            largeIcon = "";
            img = "";

            sound = "";

            builder_id = "";

            play_vibrate = "";
            play_lights = "";
            play_sound = "";

            after_open = "";
            url = "";
            activity = "";
            //custom = "";
        }

        #region 通知展现内容:
        // 必填 通知栏提示文字
        [DataMember]
        public string ticker { get; set; }
        // 必填 通知标题
        [DataMember]
        public string title { get; set; }
        // 必填 通知文字描述 
        [DataMember]
        public string text { get; set; }
        #endregion

        #region 自定义通知图标:
        //可选 状态栏图标ID, R.drawable.[smallIcon],
        //如果没有, 默认使用应用图标。
        //图片要求为24*24dp的图标,或24*24px放在drawable-mdpi下。
        //注意四周各留1个dp的空白像素
        [DataMember]
        public string icon { get; set; }

        //可选 通知栏拉开后左侧图标ID, R.drawable.[largeIcon].
        //图片要求为64*64dp的图标,
        //可设计一张64*64px放在drawable-mdpi下,
        //注意图片四周留空，不至于显示太拥挤
        [DataMember]
        public string largeIcon { get; set; }
        
        //可选 通知栏大图标的URL链接。
        //该字段的优先级大于largeIcon。该字段要求以http或者https开头。
        [DataMember]
        public string img { get; set; }
        #endregion

        #region 自定义通知声音
        //可选 通知声音，R.raw.[sound]. 
        //如果该字段为空，采用SDK默认的声音, 即res/raw/下的 umeng_push_notification_default_sound声音文件
        //如果SDK默认声音文件不存在， 则使用系统默认的Notification提示音。
        [DataMember]
        public string sound { get; set; }
        #endregion

        #region 自定义通知样式:
        //可选 默认为0，用于标识该通知采用的样式。使用该参数时, 开发者必须在SDK里面实现自定义通知栏样式。
        [DataMember]
        public string builder_id { get; set; }
        #endregion

        #region 通知到达设备后的提醒方式:
        // 可选 收到通知是否震动,默认为"true". 注意，"true/false"为字符串
        [DataMember]
        public string play_vibrate { get; set; }

        // 可选 收到通知是否闪灯,默认为"true"
        [DataMember]
        public string play_lights { get; set; }

        // 可选 收到通知是否发出声音,默认为"true"
        [DataMember]
        public string play_sound { get; set; }
        #endregion

        #region 点击"通知"的后续行为，默认为打开app:
        //必填 值可以为:
        //"go_app": 打开应用
        //"go_url": 跳转到URL
        //"go_activity": 打开特定的activity
        //"go_custom": 用户自定义内容。
        [DataMember]
        public string after_open { get; set; }
        //可选 当"after_open"为"go_url"时，必填。通知栏点击后跳转的URL，要求以http或者https开头  
        [DataMember]
        public string url { get; set; }
        //可选 当"after_open"为"go_activity"时，必填。通知栏点击后打开的Activity
        [DataMember]
        public string activity { get; set; }
        //可选 display_type=message, 或者
        //display_type=notification且
        //"after_open"为"go_custom"时，
        //该字段必填。用户自定义内容, 可以为字符串或者JSON格式。
        [DataMember]
        public UmengMessageExtra custom { get; set; }
        #endregion
    }

    //可选 发送策略
    [DataContract]
    public class UmengMessagePolicy
    {
        public UmengMessagePolicy()
        {
            out_biz_no = "";
            start_time = "";
            expire_time = "";
        }

        //可选 定时发送时间，若不填写表示立即发送。
        //定时发送时间不能小于当前时间。
        //格式: "YYYY-MM-DD HH:mm:ss"。
        //注意, start_time只对任务生效。
        [DataMember]
        public string start_time { get; set; }

        //可选 消息过期时间,其值不可小于发送时间或者
        //start_time(如果填写了的话), 
        //如果不填写此参数，默认为3天后过期。格式同start_time
        [DataMember]
        public string expire_time { get; set; }

        //可选 发送限速，每秒发送的最大条数。
        //开发者发送的消息如果有请求自己服务器的资源，可以考虑此参数。
        [DataMember]
        public int max_send_num { get; set; }

        //可选 开发者对消息的唯一标识，服务器会根据这个标识避免重复发送。
        //有些情况下（例如网络异常）开发者可能会重复调用API导致。
        //消息多次下发到客户端。如果需要处理这种情况，可以考虑此参数。
        //注意, out_biz_no只对任务生效。
        [DataMember]
        public string out_biz_no { get; set; }
    }
    
    /// <summary>
    /// 用户自定义的key-value 只对通知(display_type=notification)生效。
    /// 可以配合通知到达后,打开App,打开URL,打开Activity使用。
    /// </summary>
    public class UmengMessageExtra
    {
        public UmengMessageExtra()
        {
            type = "";
            action = "";
            isShowInApp = false;
            bgColor = "";
            btnName = "";
            timeElapse = 10f;
            msgType = "";
        }

        /// <summary>
        /// actionType字段 区分action是不是有效的
        /// </summary>
        [DataMember]
        public string type { get; set; }

        /// <summary>
        /// 消息类型字段 区分是不是有效的
        /// magicall指收到的是Magicalll消息
        /// </summary>
        [DataMember]
        public string msgType { get; set; }

        [DataMember]
        public string action { get; set; }

        [DataMember]
        public bool isShowInApp { get; set; }

        [DataMember]
        public string bgColor { get; set; }

        /// <summary>
        /// 单位秒 持续显示的秒数
        /// </summary>
        [DataMember]
        public float timeElapse { get; set; }

        [DataMember]
        public string btnName { get; set; }

        /// <summary>
        /// 接收人userId
        /// </summary>
        [DataMember]
        public long receiverId { get; set; }
    }
    
    [DataContract]
    public class UmengMessageResult
    {
        // 返回结果，"SUCCESS"或者"FAIL"
        [DataMember]
        public string ret { get; set; }

        [DataMember]
        public UmengMessageResultData data { get; set; }
    }

    [DataContract]
    public class UmengMessageResultData
    {
        //当"ret"为"SUCCESS"时,包含如下参数:
        //当type为unicast、listcast或者customizedcast且alias不为空时:
        [DataMember]
        public string msg_id { get; set; }

        //当type为于broadcast、groupcast、filecast、customizedcast 且file_id不为空的情况(任务)
        [DataMember]
        public string task_id { get; set; }

        //当"ret"为"FAIL"时,包含如下参数: 
        // 错误码详见附录I。
        [DataMember]
        public string error_code { get; set; }

        //如果开发者填写了thirdparty_id, 接口也会返回该值。
        [DataMember]
        public string thirdparty_id { get; set; }
    }

    [DataContract]
    public class UmengMessageFilter
    {
        [DataMember]
        public AndCondition where { get; set; }
    }

    [DataContract]
    public class AndCondition
    {
        [DataMember]
        /// <summary>
        /// andDic查询数据
        /// </summary>
        public List<Dictionary<string, object>> and { get; set; }
    }

    [DataContract]
    public class OrCondition
    {
        [DataMember]
        public List<Tuple<string, object>> or { get; set; }
    }

    [DataContract]
    public class NotCondition
    {
        [DataMember]
        public Tuple<string, object> not { get; set; }
    }
}