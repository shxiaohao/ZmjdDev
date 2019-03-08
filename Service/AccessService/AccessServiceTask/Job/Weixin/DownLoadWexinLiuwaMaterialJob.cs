using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
using HJD.PhotoServices.Contracts;
using HJD.WeixinServices.Contracts;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job
{
    /// <summary>
    /// 下载指定微信号(遛娃指南)的永久素材数据
    /// </summary>
    public class DownLoadWexinLiuwaMaterialJob : BaseJob
    {
        static string weixinAcount = "liuwa";
        static int categoryId = 4;
        static int weixinAcountId = 12;

        public DownLoadWexinLiuwaMaterialJob()
            : base("DownLoadWexinLiuwaMaterialJob")
        {
            Log(string.Format("Start Job [DownLoadWexinLiuwaMaterialJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [DownLoadWexinLiuwaMaterialJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [DownLoadWexinLiuwaMaterialJob]"));
        }

        private void RunJob()
        {
            var weixinApi = new Weixin();

            //下载周末酒店zmjiudian的所有关注用户信息
            var count = 10;
            var lastOffset = GetLastOffset();

            Console.WriteLine(string.Format("起始：Offset【{0}】 Count【{1}】", lastOffset, count));
            Log(string.Format("起始：Offset【{0}】 Count【{1}】", lastOffset, count));

            var weixinMaterialObj = WeixinApiHelper.GetAllWeixinMaterialList(weixinApi.GetWeixinToken(weixinAcount), lastOffset, count);

            while (weixinMaterialObj != null && weixinMaterialObj.Item != null && weixinMaterialObj.Item.Length > 0)
            {
                Console.WriteLine(string.Format("GetAllWeixinMaterialList Count:{0}", weixinMaterialObj.Item.Length));
                Log(string.Format("GetAllWeixinMaterialList Count:{0}", weixinMaterialObj.Item.Length));

                for (int i = 0; i < weixinMaterialObj.Item.Length; i++)
			    {
                    var materialObj = weixinMaterialObj.Item[i];
                    if (materialObj != null && materialObj.Content != null && materialObj.Content.NewsItem != null && materialObj.Content.NewsItem.Length > 0)
                    {
                        for (int itemNum = 0; itemNum < materialObj.Content.NewsItem.Length; itemNum++)
                        {
                            Console.WriteLine(string.Format("处理【{0}】", itemNum));

                            //内容对象
                            var contentObj = materialObj.Content.NewsItem[itemNum];

                            //文章修改时间
                            var sourceUpdateTime = WeixinApiHelper.UnixTimestampToDateTime(materialObj.UpdateTime);

                            try
                            {
                                //检查当前WeixinMediaId是否已经存在，存在则不再执行解析插入等操作
                                var _getCount = HJD.AccessServiceTask.Job.Helper.WeixinHelper.GetWeixinMaterialCountByTitle(contentObj.Title, categoryId);
                                if (_getCount <= 0)
                                {
                                    Console.WriteLine(string.Format("Save Item：{0} ：{1}", contentObj.Title, materialObj.MediaId));

                                    var _materialModel = new WeixinMaterialModel();
                                    _materialModel.Type = 1;
                                    _materialModel.WeiXinMediaID = materialObj.MediaId;
                                    _materialModel.Title = contentObj.Title;
                                    _materialModel.Content = contentObj.Content;
                                    _materialModel.Digest = contentObj.Digest;
                                    _materialModel.Author = contentObj.Author;
                                    _materialModel.Url = contentObj.Url;
                                    _materialModel.ContentSourceUrl = contentObj.ContentSourceUrl;
                                    _materialModel.ThumbMediaId = contentObj.ThumbMediaId;
                                    //_materialModel.SourceUpdateTime = DateTime.Now;
                                    _materialModel.SourceUpdateTime = sourceUpdateTime;
                                    _materialModel.Creator = 0;
                                    _materialModel.CreateTime = DateTime.Now;
                                    _materialModel.Editor = 0;
                                    _materialModel.UpdateTime = DateTime.Now;
                                    _materialModel.WeixinAcountId = weixinAcountId;
                                    _materialModel.CategoryId = categoryId;
                                    _materialModel.State = 1;

                                    //数据再处理
                                    _materialModel.Content = FormatContent(_materialModel.Content, weixinApi);

                                    var _insert = HJD.AccessServiceTask.Job.Helper.WeixinHelper.InsertWeixinMaterial(_materialModel);

                                    Console.WriteLine("Insert Item:" + _insert);
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("已经存在的MediaId：{0}  {1}", materialObj.MediaId, contentObj.Title));
                                    Log(string.Format("已经存在的MediaId：{0}  {1}", materialObj.MediaId, contentObj.Title));
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("【Err】【{0}】：{1}", materialObj.MediaId, ex.Message));
                                Log(string.Format("【Err】【{0}】：{1}", materialObj.MediaId, ex.Message));
                            }   
                        }
                    }
			    }

                Console.WriteLine(string.Format("转下一页~"));
                Log(string.Format("转下一页~"));

                SetLastOffset((lastOffset + count).ToString());
                lastOffset = GetLastOffset();

                Console.WriteLine(string.Format("范围：Offset【{0}】 Count【{1}】", lastOffset, count));
                Log(string.Format("范围：Offset【{0}】 Count【{1}】", lastOffset, count));

                //break;

                weixinMaterialObj = WeixinApiHelper.GetAllWeixinMaterialList(weixinApi.GetWeixinToken(weixinAcount), lastOffset, count);
            }

            Console.WriteLine("GetAllWeixinMaterialList is null:lastOffset:" + lastOffset);
            Log("GetAllWeixinMaterialList is null:lastOffset:" + lastOffset);

            Console.WriteLine("～下载完了～");
            Log("～下载完了～");

            try
            {
                SetLastOffset("1");

                Console.WriteLine("初始页码为 1");
                Log("初始页码为 1");
            }
            catch (Exception ex)
            {
                
            }
            
        }

        private string FormatContent(string content, Weixin weixinApi)
        {
            var _content = content;

            //替换img的data-src
            _content = _content.Replace("data-src=", "src=");

            //提取所有img的src
            var _imgSrcArray = GetHtmlImageUrlList(_content);

            //上传并生成ids
            List<int> _ids = weixinApi.PhotoUploadWithUrls(_imgSrcArray);

            //Thread.Sleep(3000);

            //根据id获取photo对象
            List<PHSPhotoInfoEntity> _newImagePaths = weixinApi.GenPhotoPaths(_ids);

            for (int i = 0; i < _imgSrcArray.Length && i < _newImagePaths.Count; i++)
            {
                var _oldUrl = _imgSrcArray[i];

                var _newImgObj = _newImagePaths[i];
                var _newUrl = _newImgObj.PhotoUrl[PhotoServices.Entity.PhotoSizeType.jupiter];
                if (!string.IsNullOrEmpty(_newUrl) && !_newUrl.Contains("p1.zmjiudian.com"))
                {
                    //Thread.Sleep(500);

                    //如果第一次没有生成正确的图片path，则再生成一次
                    List<int> _s = new List<int> { _newImgObj.PHSID };
                    _newUrl = weixinApi.GenPhotoPaths(_s)[0].PhotoUrl[PhotoServices.Entity.PhotoSizeType.jupiter];
                }

                //replace img src
                if (_newUrl.Contains("p1.zmjiudian.com"))
                {
                    _content = _content.Replace(_oldUrl, _newUrl);
                }
            }

            return _content;
        }

        /// <summary> 
        /// 正则提取HTML中所有图片的URL 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表 
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }

        static string lastOffsetFile = @"D:\Log\Config\Weixin\WeixinMaterial_Liuwa_LastOffset_{0}.txt";

        private int GetLastOffset()
        {
            var lastoffset = 1;

            var now = DateTime.Now.Date;
            lastOffsetFile = string.Format(@"D:\Log\Config\Weixin\WeixinMaterial_Liuwa_LastOffset_{0}.txt", now.ToString("yyyyMMdd"));

            if (File.Exists(lastOffsetFile))
            {
                try
                {
                    lastoffset = Convert.ToInt32(File.ReadAllText(lastOffsetFile));
                    Console.WriteLine("【GetLastOffset】:" + lastoffset);
                    Log("【GetLastOffset】:" + lastoffset);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("读取LastOffset错误：" + ex.Message);
                    Log("读取LastOffset错误：" + ex.Message);
                }
            }
            else
            { 
                //如果不存在今天的记录页数的文件，则初始
                SetLastOffset(lastoffset.ToString());
            }
            return lastoffset;
        }

        private void SetLastOffset(string lastOffset)
        {
            File.WriteAllText(lastOffsetFile, lastOffset);
            Console.WriteLine("LastOffset已经存储至本地！" + lastOffset);
            Log("LastOffset已经存储至本地！" + lastOffset);
        }
    }
}
