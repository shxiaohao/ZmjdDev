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
   public  class DialogItemBase
    {
        public DialogItemBase() { CreatTime = DateTime.Now; Text = ""; }

        [DataMemberAttribute()]
        public long ItemID;

        [DataMemberAttribute()]
        public int DialogID;

        [DataMemberAttribute()]
        public MagiCallDialogItemType ItemType;

        [DataMemberAttribute()]
        public string Text;

        [DataMemberAttribute()]
        public DateTime CreatTime;

    }
}
