using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HJD.HotelServices.Host
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HotelService hs = new HotelService();

            //var calList = hs.GetHotelPackageCalendar30(297562, DateTime.Parse("2015-05-20"), 1005251);

            //var clist = hs.GenCtripHotelPackageCalendar(297562, DateTime.Now.Date);

            var list = hs.GetCtripHotelPackages(527784, DateTime.Parse("2016-01-21"), DateTime.Parse("2016-01-22"), false);
            return;

            //var list = hs.GetCtripHotelPackagesV42(297562, DateTime.Parse("2016-01-17"), DateTime.Parse("2016-01-18"), false);
            //return;

            //var list = hs.GetCtripHotelPackages(297562, DateTime.Parse("2015-11-28"), DateTime.Parse("2015-11-29"), false);

        }
    }
}