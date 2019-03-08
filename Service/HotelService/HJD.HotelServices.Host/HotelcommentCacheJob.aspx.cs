using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using HJD.Framework.WCF;
using HJD.Contracts;
using HJD.Framework.Log;
using HJD.HotelServices.Contracts;

namespace HJD.HotelServices.Host
{
    public partial class HotelcommentCacheJob : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Go();
            //return;

            if (null != Request.QueryString["run"] && Request.QueryString["run"] == "T")
            {
             //   Go();
            }
            else
            {

                //if (service.Begin(this.TaskName, System.Environment.MachineName) == HJD.Entity.JobAssistantEntity.WELCOME)
                //{
                //    Thread thread = new Thread(this.Go);
                //    thread.IsBackground = true;
                //    thread.Start();
                //}
            }
        }

        string TaskName = "HotelReviewCacheForApi";

        IJobAssistantService service = ServiceProxyFactory.Create<IJobAssistantService>("JobAssistantService");

        private void Go()
        {
            try
            {
                HotelService ps = new HotelService();

                //int o = Request.QueryString["o"] == null ? 10 : Convert.ToInt32(Request.QueryString["o"]);
                //int p = Request.QueryString["p"] == null ? 7 : Convert.ToInt32(Request.QueryString["p"]);
                //string q = Request.QueryString["q"] == null ? "L" : Request.QueryString["q"];

               ps.GenerateHJDHotelReviewCacheForApi(10,7,"L");

                service.Done(this.TaskName, System.Environment.MachineName);
            }
            catch (Exception e)
            {
                e.WriteLog();

                service.Error(this.TaskName, string.Format("生成数据发生异常：{0}", e.Message));
            }
        }
    }
}