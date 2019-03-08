using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Dialog
{
    [Serializable]
    [DataContract]
    public enum MagiCallDialogItemType
    {
        [EnumMemberAttribute]
        UserWord, //用户的话语
        [EnumMemberAttribute ]
        KeFuWord, //客服话语
        [EnumMemberAttribute ]
        UserWordOption, //对用户话语的回复建议
        [EnumMemberAttribute ]
        UserAction, //用户的动作：点击了某个连接
        [EnumMemberAttribute ]
        KeFuAction, //客服的动作：如选择了某个建议，发送了某个酒店给用户等
        [EnumMemberAttribute]
        SystemAction,  //如自动发提醒给用户
        [EnumMemberAttribute ]
        Other = 100//好吧，还不知道会发生什么事
    }

    [Serializable]
    [DataContract]
    public enum MagiCallUserWordOptionType
    {
        [EnumMemberAttribute ]
        City,
        [EnumMemberAttribute ]
        Zone,
        [EnumMemberAttribute ]
        Theme,
        [EnumMemberAttribute ]
        Featured,
        [EnumMemberAttribute ]
        Facility,
        [EnumMemberAttribute ]
        Class,
        [EnumMemberAttribute ]
        Brand,        
        [EnumMemberAttribute ]
        Other,
        [EnumMemberAttribute]
        Money,
        [EnumMemberAttribute]
        Date,
        [EnumMemberAttribute]
        UserNum,
        [EnumMemberAttribute]
        POI,
        [EnumMemberAttribute]
        CityAround
    }
     


}
