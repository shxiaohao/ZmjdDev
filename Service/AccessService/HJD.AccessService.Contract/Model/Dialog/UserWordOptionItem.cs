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
    public class UserWordOptionItem : DialogItemBase
    {
        public UserWordOptionItem() { ItemType = MagiCallDialogItemType.UserWordOption; }

        [DataMemberAttribute()]
        public long RelItemID;

        [DataMemberAttribute()]
        public MagiCallUserWordOptionType OptionType;

        [DataMemberAttribute()]
        public String ActionParam;
    }
}
