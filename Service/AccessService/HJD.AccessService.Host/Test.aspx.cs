using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccessService.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HJD.AccessService.Host
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var hotelid = 2321;
            new AccessServices().AddPriceSlot(hotelid);

            return;


            //AccessServices ass = new AccessServices();

            //ass.AddIndexDocument("226258", SearchType.Hotel);
            //ass.AddIndexDocument("216387", SearchType.Hotel);
            //ass.AddIndexDocument("324571", SearchType.Hotel);
            //ass.AddIndexDocument("401148", SearchType.Hotel);

            //ass.ReaderIndexManagerQueue();



            //List<Behavior> blist = new List<Behavior>();
            //blist.Add(new Behavior{ 
            //    Code = "Search",
            //    Value = "苏州 金鸡湖",
            //    Page = ass.GetBehaviorProfile("Search").Page,
            //    Event = ass.GetBehaviorProfile("Search").Event,
            //    AppKey = Guid.NewGuid().ToString(),
            //    ClientType = "WWW"
            //});



            //blist.Add(new Behavior
            //{
            //    Code = "Search",
            //    Value = "苏州 凯宾斯基",
            //    Page = ass.GetBehaviorProfile("Search").Page,
            //    Event = ass.GetBehaviorProfile("Search").Event,
            //    AppKey = Guid.NewGuid().ToString(),
            //    ClientType = "WWW"
            //});

            //ass.RecordBehaviorQueue(blist);

            ////
            ////ass.ReaderBehaviorQueue();
        }
    }
}