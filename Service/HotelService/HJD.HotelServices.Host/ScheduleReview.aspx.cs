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
using System.IO;

namespace HJD.HotelServices.Host
{
    public partial class ScheduleReview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Go();
            //return;

            //if (service.Begin(this.TaskName, System.Environment.MachineName) == HJD.Entity.JobAssistantEntity.WELCOME)
            {
                Thread thread = new Thread(this.Go);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        string TaskName = "HotelReview_MongoDB";
        string constLastTimeStamp = "HotelReviewLastTimeStamp";

        IJobAssistantService service = ServiceProxyFactory.Create<IJobAssistantService>("JobAssistantService");

        private void Go()
        {
            try
            {
                HotelService ps = new HotelService();

           //     ps.CheckAllHotelReview();
                //ps.HotelReviewByTimeStamp(this.TaskName, constLastTimeStamp);

               // service.Done(this.TaskName, System.Environment.MachineName);
            }
            catch (Exception e)
            {
                File.AppendAllText("D:\\HJDApp\\log\\hotelreviewlog.txt", e.ToString());
                //e.WriteLog();

                //service.Error(this.TaskName, string.Format("同步数据发生异常：{0}", e.Message));
            }
        }
    }
}