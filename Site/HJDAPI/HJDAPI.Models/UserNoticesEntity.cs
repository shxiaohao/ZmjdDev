using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class UserNoticesEntity
    {    
        public long NoticeID { get; set; }

        public string Message { get; set; }

        public string BackgroundColor { get; set; }

        public List<ActionInfo> Actions { get; set; }

    }

    public class ActionInfo
    {
        public int ActionType { get; set; }
        public string ActionURL { get; set; }
        public bool NeedNotifyService { get; set; }
    }
}
