﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Dialog
{
    [Serializable]
    [DataContract]
    public class UserActionItem: DialogItemBase
    {
        public UserActionItem() { ItemType = DialogItemType.UserAction; }

        [DataMemberAttribute()]
        public UserActionType ActionType;
    }
}
