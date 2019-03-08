using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.EMC
{

    public class EMCTextMessageMsg
    {
        public string type { get; set; }
        public string msg { get; set; }
    }
    public class EMCTextMessageEXT
    {
        public string type { get; set; }
        public string msg { get; set; }
    }
    public class EMCTxtMessageEntity
    {


        public string target_type { get; set; }
        public string target { get; set; }
        public string from { get; set; }
        public string msg { get; set; }


        //StringBuilder _build = new StringBuilder();
        //_build.Append("{");
        //_build.AppendFormat("\"target_type\":\"{0}\",\"target\":[{1}],", target_type, string.Join(",", targetList.Select(t => "\"" + t + "\"")));
        //_build.Append("\"msg\":{");
        //_build.AppendFormat("\"type\":\"txt\",\"msg\":\"{0}\"", msg);
        //_build.Append("}");
        //if (from.Length > 0)
        //{
        //    _build.AppendFormat(",\"from\":\"{0}\"", from);
        //}
        //if (dicExt.Count > 0)
        //{
        //    _build.Append(",\"ext\":{");
        //    _build.AppendFormat("{0}", string.Join(",", dicExt.Select(d => "\"" + d.Key + "\":" + (d.Value.StartsWith("{") ? d.Value : "\"" + d.Value + "\""))));
        //    _build.Append("}");
        //}
        //_build.Append("}");


    }
}
