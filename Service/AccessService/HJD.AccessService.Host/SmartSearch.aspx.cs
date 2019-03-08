using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HJD.AccessService.Host
{
    public partial class SmartSearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {

            var testStr = "";
            if (!string.IsNullOrEmpty(testStr)) testStr += " "; testStr += "1";

            var limcount = 20;
            var keywords = searchTxt.Value;

            var result = new AccessServices().QaSearchHotel(keywords, limcount, "2015-11-11", "2015-11-18");

            var resultList = new AccessServices().StrictSearch(keywords, limcount);

            //resultDiv.InnerHtml = string.Format("<br />");
            //resultDiv.InnerHtml += string.Format("<br />我猜您可能在找：");
            //for (int i = 0; i < resultList.Count; i++)
            //{
            //    var hotel = resultList[i];
            //    resultDiv.InnerHtml += string.Format("<br /><b><a href=\"http://www.zmjiudian.com/hotel/{1}\">{0}</a></b>", hotel.HotelName, hotel.HotelId);
            //}

            //return;

            //var searchParams = new SearchParams();
            //searchParams.keywords = searchTxt.Value;

            //var searchResult = HmmHelper.Search(searchParams);

            //resultDiv.InnerHtml += string.Format("<br />");
            //resultDiv.InnerHtml += string.Format("<br />用时{0}秒", (searchResult.returnTime - searchResult.searchTime).TotalSeconds);
            //resultDiv.InnerHtml += string.Format("<br />{0}", searchResult.resultTitle);
            //resultDiv.InnerHtml += string.Format("<br /><b>{0}</b>", searchResult.resultValue);

            //if (searchResult.relationResult != null && searchResult.relationResult.Count > 0)
            //{
            //    resultDiv.InnerHtml += string.Format("<br />我猜您可能还想了解：");
            //    for (int i = 0; i < searchResult.relationResult.Count; i++)
            //    {
            //        var relation = searchResult.relationResult[i];
            //        resultDiv.InnerHtml += string.Format("<br /><b><a href=\"{1}\">{0}</a></b>", relation.title, relation.url);
            //    }   
            //}
        }
    }
}