using HJD.AccessService.Contract;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Helper;
using HJD.Search.CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HJD.AccessService.Host
{
    public partial class IndexSearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            var get1 = NLPEngine.KeyTokenizerPangu("明天上海哪些酒店有房?");
            return;

            var keywords = searchTxt.Value;

            //创建搜索引擎对象
            var searchEngine = SearchEngine.GetInstance(SearchType.Hotel);

            //搜索
            searchEngine.SearchStartTime = DateTime.Now;

            var list = searchEngine.Search(keywords, 40, SearcherType.Single);

            //将搜索的id转换为对象
            var datas = HotelEngine.GetHotelSearchResult(list);
            //.Select(x => new HotelSearchResult()
            //{
            //    HotelId = x.HotelId,
            //    HotelName = x.HotelName,//SearchEngine.GetHightLighter(keywords, SearchHelper.NoHTML(x.HotelName), 200),
            //    Ename = x.Ename,//SearchEngine.GetHightLighter(keywords, SearchHelper.NoHTML(x.Ename), 200),
            //    Address = x.Address,//SearchEngine.GetHightLighter(keywords, SearchHelper.NoHTML(x.Address), 200),
            //    HotelDesc = x.HotelDesc,//SearchEngine.GetHightLighter(keywords, SearchHelper.NoHTML(x.HotelDesc), 200)
            //});

            //呈现
            resultDiv.InnerHtml = "";
            foreach (var hotel in datas)
            {
                resultDiv.InnerHtml += "<div style='margin:10px 0 20px 0;'>";
                resultDiv.InnerHtml += string.Format("<div><a href='http://www.zmjiudian.com/hotel/{0}'>{1}</a><span style='color:#666;margin-left:20px;'>{2}</span><span style='color:#666;margin-left:20px;'>星级：{3}</span><span style='color:#666;margin-left:20px;'>点评分：{4}</span></div>", hotel.HotelId, hotel.HotelName, hotel.Themes, hotel.Star, hotel.ReviewScore);
                resultDiv.InnerHtml += string.Format("<div>{0}</div>", hotel.Address);
                //resultDiv.InnerHtml += string.Format("<div>{0}</div>", hotel.HotelDesc);
                resultDiv.InnerHtml += string.Format("<div>【{0}】</div>", hotel.Boost);
                resultDiv.InnerHtml += string.Format("<details>【{0}】</details>", hotel.Explain);
                resultDiv.InnerHtml += "</div>";    
            }

            //统计
            searchEngine.SearchEndTime = DateTime.Now;
            resultTitDiv.InnerHtml = string.Format("共查到{0}条相关记录，共耗时{1}毫秒", list.Count, searchEngine.SearchTotalMilliseconds);
        }
    }
}