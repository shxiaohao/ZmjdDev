var districtId = $P["districtId"];
var districtName = $P["districtName"];
var lat = $P["lat"];
var lng = $P["lng"];
var geoScopeType = $P["geoScopeType"];
var productSort = 0;
var _sourcekey = $P["_sourcekey"];

var tagId = $("#tagId").val();
var tagName = $("#tagName").val();

var shareTitle = $("#shareTitle").val();
var shareDesc = $("#shareDesc").val();
var shareLink = $("#shareLink").val();
var shareImgUrl = $("#shareImgUrl").val();

//当前列表展示方式（列表list或者矩阵grid）
var listStyle = "grid";

var _Config = new Config();
var userid = $("#userid").val();
var albumIds = $("#albumId").val();
var topId = $("#topId").val();
var categoryId = parseInt($("#categoryId").val());
var isApp = $("#isApp").val() == "1";
var isInWeixin = $("#isInWeixin").val() == "1";
var showBanner = $("#showBanner").val() == "1"; //是否显示banner，会影响列表切换和筛选显示
var useSubCityFilter = $("#useSubCityFilter").val() == "1"; //是否使用省份级联的城市筛选
var showHotCity = $("#showHotCity").val() == "1"; //是否显示“热门度假地”模块（并切换城市的排列为矩阵排列）
var isgridnum = $("#isgrid").val();
var isgrid = isgridnum == "1";
listStyle = $("#listStyle").val();
var loadMorePackages = {};
var isload = true;

var _cityFilterLastOption = null;
var _cityFilterLastSubitems = null;

var loadItem = {};

var _HomeSliderItemWidth = 363;
var _HomeSliderLiMarLeft = 0;

//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

var loadFunction = function () {

    var wwidth = $(window).width();
    if (wwidth >= 430) { }							//meizu mx3
    if (wwidth < 410) { _HomeSliderItemWidth = 328; _HomeSliderLiMarLeft = 0; }	//ip6
    if (wwidth == 360) { _HomeSliderItemWidth = 317; _HomeSliderLiMarLeft = 0; }
    if (wwidth < 350) { _HomeSliderItemWidth = 280; _HomeSliderLiMarLeft = 0; }	//ip4/ip5

    $('body').append("<style>.home-hlist-panel{ width: " + wwidth + "px !important; }.home-hlist-panel .item{ width: " + wwidth + "px !important; }</style>");

    if (geoScopeType == "" || geoScopeType == undefined) {
        geoScopeType = "3";
        if (lat == undefined) lat = 31.236237;
        if (lng == undefined) lng = 121.389139;
    }
    else {
        if (lat == undefined) lat = 0;
        if (lng == undefined) lng = 0;
    }

    //districtId默认值
    if (districtId == undefined) districtId = "0";
    if (districtName == undefined) districtName = "";
    districtName = decodeURIComponent(districtName);

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //检测登录并自动登录
    if (!isApp && userid == "0") {
        _loginModular.verify.autoLogin(loginCheckFun);
    }

    var _pageTitle = "";
    var tabDetailList = [];

    if (albumIds) {
        var _ids = albumIds.split(',');
        for (var _idnum = 0; _idnum < _ids.length; _idnum++) {
            var _id = parseInt(_ids[_idnum]);
            var _tabDetailItem = {
                id: _id,
                type: "album",
                name: "其它",
                start: 0,
                count: 12,
                sort: 1,    //0默认专辑排序 1售卖结束时间排序（最先结束的在前面）
                skuList: [],
                AdData: {},
                listStyle: listStyle    //"grid" //"list"
            }

            switch (_id) {
                case 1: { var _n = "专辑"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 2: { var _n = "美食精选"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 3: { var _n = "玩乐精选"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 4: { var _n = "今日特价"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 5: { var _n = "超值团"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 6: { var _n = "苏州乐园现场活动"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 7: { var _n = "其它"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }

                case 13: { var _n = "酒店"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 14: { var _n = "美食"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 15: { var _n = "玩乐"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }

                case 16: { var _n = "酒店"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 18: { var _n = "玩乐"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }

                case 19: { var _n = "酒店"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 20: { var _n = "玩乐"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }

                case 32: { var _n = "酒店"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 33: { var _n = "玩乐"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 34: { var _n = "美食"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
            }

            tabDetailList.push(_tabDetailItem);
        }
    }

    //if (_pageTitle) {
    //    document.title = _pageTitle;
    //}

    //tabDetailList = [
    //    {
    //        id: 2,
    //        name: "美食",
    //        start: 0,
    //        count: 6,
    //        skuList: []
    //    },
    //    {
    //        id: 3,
    //        name: "玩乐",
    //        start: 0,
    //        count: 6,
    //        skuList: []
    //    }
    //];

    //初始绑定tabs
    var productTabData = new Vue({
        el: "#tabs",
        data: {
            "tabDetailList": tabDetailList
        }
    })

    //双11特殊处理
    var productTabData2 = new Vue({
        el: "#tabs-active",
        data: {
            "tabDetailList": tabDetailList
        }
    })

    //默认第一个
    loadItem = tabDetailList[0];

    //初始绑定list
    var productListData = new Vue({
        el: "#more-vue-obj",
        data: {
            "topInfo": {},
            "tabDetailList": tabDetailList
        }
    })

    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();
    var tabsHeight = $("#tabs").height();
    var tabsActiveTop = 0;

    if (tabDetailList.length < 2) {
        $("#tabs").hide();
        tabsHeight = 0;
    }

    var oneDivBeforeScrollTop = 0;
    var upTopNum = 0;
    var downTopNum = 0;
    $(".scroll-div").each(function () {

        var _scrollObj = $(this);

        _scrollObj.css("height", winHeight - tabsHeight);

        //single div scroll top
        oneDivBeforeScrollTop = _scrollObj[0].scrollTop;

        _scrollObj.scroll(function () {
            
            var _relid = _scrollObj.data("relid");
            var _reltype = _scrollObj.data("reltype");

            var _tagTop = $(".more-packages-foot").offset().top;
            var _divTop = _scrollObj[0].scrollTop;
            var _divHeight = _scrollObj.height() + 10;  //多减去10，很奇怪，android上计算有问题... haoy 20170808
            var _divScrollHeight = _scrollObj[0].scrollHeight;

            if (_divTop >= (_divScrollHeight - _divHeight)) {

                //console.log("div到底了")
                loadMorePackageList(_relid, _reltype, false, false);
            }

            var afterScrollTop = _divTop,
                delta = afterScrollTop - oneDivBeforeScrollTop; //console.log(delta)
            if (delta === 0) return false;
            scrollTopDownFunc(delta > 0 ? "down" : "up", afterScrollTop, delta);
            oneDivBeforeScrollTop = afterScrollTop;

        });
    });

    var scrollTopDownFunc = function (delta, afterScrollTop, diffTop) {

        var _checkShowTop = 260;
        if (!showBanner) {
            _checkShowTop = 55;
        }

        //改为到位置直接显示
        if (afterScrollTop > _checkShowTop) {

            $("#filter-section").addClass("filter-section-fixed");

            //显示筛选
            $("#filter-section .right-item").show();

            //显示列表排版文字
            $(".filter-section .showchange .list-style-txt").show();
        }
        else {

            $("#filter-section").removeClass("filter-section-fixed");

            if ($(".filter-section .city-filter-txt").html().indexOf("度假地") >= 0) {

                if (showBanner) {

                    //隐藏筛选
                    $("#filter-section .right-item").hide();
                }
            }

            if (showBanner) {

                //隐藏列表排版文字
                $(".filter-section .showchange .list-style-txt").hide();
            }
        }
        return;

        //console.log(diffTop)

        if ($("#filter-section")) {

            if (delta == "up") {

                downTopNum = 0;
                upTopNum += (0 - diffTop);
                //console.log(upTopNum)

                if (afterScrollTop > 260) {

                    if (upTopNum > 40) {

                        $("#filter-section").addClass("filter-section-fixed");

                        //显示筛选
                        $("#filter-section .right-item").show();

                        //显示列表排版文字
                        $(".filter-section .showchange .list-style-txt").show();
                    }
                }
                else {

                    $("#filter-section").removeClass("filter-section-fixed");

                    if ($(".filter-section .city-filter-txt").html().indexOf("度假地") >= 0) {

                        //隐藏筛选
                        $("#filter-section .right-item").hide();
                    }

                    //隐藏列表排版文字
                    $(".filter-section .showchange .list-style-txt").hide();
                }
            }
            else {

                upTopNum = 0;
                downTopNum += diffTop;
                //console.log(downTopNum)

                if (downTopNum >= 80) {

                    $("#filter-section").removeClass("filter-section-fixed");

                    if ($(".filter-section .city-filter-txt").html().indexOf("度假地") >= 0) {

                        //隐藏筛选
                        $("#filter-section .right-item").hide();
                    }

                    //隐藏列表排版文字
                    $(".filter-section .showchange .list-style-txt").hide();
                }
            }
        }
    }

    //var start = 0;
    //var count = 6;

    var loadMorePackageList = function (id, type, isfirst, goScrollTop) {
        
        if (isfirst) {
            isload = true;
        }

        if (isload) {
            
            isload = false; 

            //获取start，count等
            var _tabDetailObj = {};
            for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                var _tabDetailItem = productListData.tabDetailList[_tnum];
                if (_tabDetailItem.id === id) {
                    _tabDetailObj = _tabDetailItem;
                    break;
                }
            }

            //如果是第一页，则初始start
            if (isfirst) {
                _tabDetailObj.start = 0;
                _tabDetailObj.skuList = [];
            }

            //第一次绑定完后，做一些初始操作
            //$(".scrollpageloading").find("img").removeClass("img-first");

            //默认显示loadding
            $("#scrollpageloading-" + id).show();

            //隐藏nothing
            $("#nothing-section").hide();

            //加载指定专辑/分类的产品list
            var loadProductListCallback = function (data) {

                //console.log(data);

                if (data) {

                    //动态设置页面标题
                    if (type == "category") {
                        document.title = _tabDetailObj.name;
                    }
                    else {
                        document.title = data.AlbumName;
                    }

                    if (data.SKUCouponList && data.SKUCouponList.length) {

                        //第一次绑定完后，做一些初始操作
                        if (data.SKUCouponList.length >= _tabDetailObj.count) {
                            if (isfirst) {
                                $("#scrollpageloading-" + id).show();
                            }
                            else {
                                $("#scrollpageloading-" + id).hide();
                            }
                        }
                        else {
                            //如果是第一页，则不显示“没有更多了”，不然很奇怪
                            if (!isfirst) {
                                $("#scrollpageloading-" + id).hide();
                                //$(".scrollpageloading").html("<div>没有更多了</div>");
                            }
                            else {
                                $("#scrollpageloading-" + id).hide();
                            }
                        }

                        for (var _lnum = 0; _lnum < data.SKUCouponList.length; _lnum++) {
                            var _litem = data.SKUCouponList[_lnum];

                            //item图片
                            _litem.PicUrl = "";
                            if (_litem.PicList && _litem.PicList.length > 0 && _litem.PicPath.length > 2) {
                                //_litem.PicUrl = _litem.PicList[0].replace("_theme", "_350X350");
                                _litem.PicUrl = _litem.PicList[0].replace("_theme", "_w320h230");
                            }
                            if (!_litem.PicUrl) {
                                //_litem.PicUrl = "/content/images/seat/home-hotel-load-1x1.png";
                                _litem.PicUrl = "/content/images/seat/img-viparea-item-3x2.png";
                            }

                            //不同环境下跳转url
                            if (isApp) {
                                _litem.url = "/coupon/product/" + _litem.SKUID + "?userid={userid}&_newpage=1&_sourcekey=sku_couponsales_products";
                            }
                            else {
                                _litem.url = "/coupon/product/" + _litem.SKUID + "?userid=" + userid + "&_newpage=1&_newtitle=1&_sourcekey=sku_couponsales_products";
                            }

                            for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                                var _tabDetailItem = productListData.tabDetailList[_tnum];
                                if (_tabDetailItem.id === id) {
                                    _tabDetailItem.skuList.push(_litem);
                                    break;
                                }
                            }

                            var dtArr = {};
                            var dayArr = {};
                            var timeArr = {};

                            //now
                            var _now = new Date();
                            _litem["y0"] = _now.getFullYear();
                            _litem["mo0"] = _now.getMonth() + 1;
                            _litem["d0"] = _now.getDate();
                            _litem["h0"] = _now.getHours();
                            _litem["mi0"] = _now.getMinutes();
                            _litem["s0"] = _now.getSeconds();

                            try {
                                if (_litem.EffectiveTime) {
                                    dtArr = (_litem.EffectiveTime).split("-");
                                    dayArr = dtArr[2].split("T");
                                    timeArr = dayArr[1].split(":");
                                    _litem["y1"] = dtArr[0];
                                    _litem["mo1"] = dtArr[1];
                                    _litem["d1"] = dayArr[0];
                                    _litem["h1"] = timeArr[0];
                                    _litem["mi1"] = timeArr[1];
                                    _litem["s1"] = timeArr[2];
                                }

                                if (_litem.SaleEndDate) {
                                    dtArr = (_litem.SaleEndDate).split("-");
                                    dayArr = dtArr[2].split("T");
                                    timeArr = dayArr[1].split(":");
                                    _litem["y2"] = dtArr[0];
                                    _litem["mo2"] = dtArr[1];
                                    _litem["d2"] = dayArr[0];
                                    _litem["h2"] = timeArr[0];
                                    _litem["mi2"] = timeArr[1];
                                    _litem["s2"] = timeArr[2];
                                }
                            } catch (e) {

                            }
                        }

                        //更新页码
                        for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                            var _tabDetailItem = productListData.tabDetailList[_tnum];
                            if (_tabDetailItem.id === id) {
                                _tabDetailItem.start = (_tabDetailObj.start + _tabDetailObj.count)
                                break;
                            }
                        }
                        //start += count;

                        Vue.nextTick(function () {

                            if (topId && topId != "0") {
                                loadTopList(topId);
                            }
                            else {
                                $("#banner-section").hide();
                            }

                            //动态加载图
                            loadImgsEvent();

                            //run
                            runTimer_Flash();

                            //$("#more-packages img").lazyload({
                            //    threshold: 20,
                            //    placeholder: "~/Content/images/seat/img-viparea-item-3x2.png",
                            //    effect: "fadeIn"
                            //});

                            try {

                                var _pageIndexNow = (_tabDetailObj.start / _tabDetailObj.count);
                                if (_pageIndexNow > 1) {

                                    //【数据统计】统计列表上拉翻页(目前第1页不统计，第2页开始统计)
                                    _statistic.push("今日特价", "上拉翻页", "第{0}页".format(_pageIndexNow), _sourcekey, "");
                                }

                            } catch (e) {

                            }

                            //更新分享文案
                            refShareInfo();
                        })

                        isload = true;
                    }
                    else {

                        //第一页时的操作
                        if (isfirst) {
                            $("#nothing-section").show();
                            $("#scrollpageloading-" + id).hide();
                        }
                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        else {
                            $("#scrollpageloading-" + id).hide();
                        }

                        //更新分享文案
                        refShareInfo();
                    }
                }
                else {

                    //第一页时的操作
                    if (isfirst) {
                        $("#nothing-section").show();
                        $("#scrollpageloading-" + id).hide();
                    }
                    //如果是第一页，则不显示“没有更多了”，不然很奇怪
                    else {
                        $("#scrollpageloading-" + id).hide();
                    }

                    //更新分享文案
                    refShareInfo();
                }
            }

            //get request
            var _listDic = { albumid: id, count: _tabDetailObj.count, start: _tabDetailObj.start, userid: userid, districtID: districtId, commTagID: tagId, sort: _tabDetailObj.sort };
            $.get(_Config.APIUrl + '/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID', _listDic, function (data) {
                loadProductListCallback(data);
            });

        }
    }

    //页面初始加载
    setTimeout(function () {
        selectMenu(loadItem.id, true);
        loadMorePackageList(loadItem.id, loadItem.type, true, false);
    }, 0);

    var bindEvent = function () {

        $("#tabs .t-item").each(function () {

            var _mitem = $(this);

            //设置宽度
            if (tabDetailList.length > 1) {
                var _chuNum = (tabDetailList.length > 4 ? 4.4 : tabDetailList.length);
                var _itemWidth = winWidth / _chuNum;
                //console.log(_itemWidth)
                _mitem.css("width", _itemWidth);

                //让头菜单支持横向滑动
                var tabsScroll = new IScroll('#tabs', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
            }

            //event
            _mitem.click(function () {

                var _relid = _mitem.data("relid");

                //设置选中
                selectMenu(_relid);

                var _tabDetailObj = {};
                for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                    var _tabDetailItem = productListData.tabDetailList[_tnum];
                    if (_tabDetailItem.id === _relid) {
                        _tabDetailObj = _tabDetailItem;
                        break;
                    }
                }
                
                //console.log(_tabDetailObj);

                if (!_tabDetailObj.skuList || !_tabDetailObj.skuList.length) {
                    loadMorePackageList(_tabDetailObj.id, _tabDetailObj.type, false, true);
                }
                else {
                    //if (isDouble11) {
                    //    $("#tabs").hide();
                    //    $("#scroll_" + _relid).animate({ scrollTop: tabsActiveTop }, 300);
                    //}
                }
            });

        });

        //双11特殊处理
        $("#tabs-active .t-item").each(function () {

            var _mitem = $(this);

            //设置宽度
            if (tabDetailList.length > 1) {
                var _chuNum = (tabDetailList.length > 4 ? 4.4 : tabDetailList.length);
                var _itemWidth = winWidth / _chuNum;
                //console.log(_itemWidth)
                _mitem.css("width", _itemWidth);

                //让头菜单支持横向滑动
                var tabsScroll = new IScroll('#tabs-active', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
            }

            //event
            _mitem.click(function () {

                var _relid = _mitem.data("relid");

                //设置选中
                selectMenu(_relid);

                var _tabDetailObj = {};
                for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                    var _tabDetailItem = productListData.tabDetailList[_tnum];
                    if (_tabDetailItem.id === _relid) {
                        _tabDetailObj = _tabDetailItem;
                        break;
                    }
                }

                if (!_tabDetailObj.skuList || !_tabDetailObj.skuList.length) {
                    loadMorePackageList(_tabDetailObj.id, _tabDetailObj.type, false, true);
                }
                else {
                    //if (isDouble11) {
                    //    $("#tabs").hide();
                    //    $("#scroll_" + _relid).animate({ scrollTop: tabsActiveTop }, 300);
                    //}
                }
            });

        });
    }
    bindEvent();

    //选中当前菜单
    var selectMenu = function (id, isfirst) {

        $("#tabs .t-item").each(function () {

            var _mitem = $(this);
            var _relid = _mitem.data("relid");

            _mitem.removeClass("sel");
            if (_relid === id) {

                _mitem.addClass("sel");

                $(".scroll-div").hide();
                $("#scroll_" + id).show();

            }

        });

        //双11特殊处理
        $("#tabs-active .t-item").each(function () {

            var _mitem = $(this);
            var _relid = _mitem.data("relid");

            _mitem.removeClass("sel");
            if (_relid === id) {

                _mitem.addClass("sel");

                $(".scroll-div").hide();
                $("#scroll_" + id).show();

            }

        });
    }

    //遍历所有待加载图片，并动态加载
    var loadImgsEvent = function () {
        $(".load-img").each(function () {
            var _load = $(this).data("load");
            if (_load === 0) {
                setImgOriSrc($(this));
            }
        })
    }


    //加载特色标签
    var tagFilterData = new Vue({
        el: '#tag-selector',
        data: {
            "tagData": {}
        }
    })
    var loadTagFilterInfo = function () {

        var _paramDic = { albumid: albumIds };
        var urlSearch = "/api/coupon/GetTagListByAlbum";

        $.get(_Config.APIUrl + urlSearch, _paramDic, function (_data) {

            //console.log(_data);
            //console.log("模拟加载特色标签完成");

            var _tagData = {};
            _tagData["tagId"] = 0;
            _tagData["tags"] = _data;

            //城市vue
            tagFilterData.tagData = _tagData;

            Vue.nextTick(function () {

                //是否参数默认了指定特色标签
                if (tagId > 0) {

                    if (tagName) {
                        $(".tag-filter-txt").html(tagName);
                    }
                    else {

                        //只指定了标签ID的时候，需要反查出标签名称显示
                        for (var _tnum = 0; _tnum < _data.length; _tnum++) {

                            var _tag = _data[_tnum];
                            if (_tag.ID == tagId) {
                                tagName = _tag.Name;
                                $(".tag-filter-txt").html(tagName);
                                break;
                            }
                        }
                    }

                    tagFilterData.tagData["tagId"] = parseInt(tagId);
                }

                //特色切换事件
                $("#tag-selector ._list .sub-item-col").each(function () {

                    $(this).unbind("click");
                    $(this).click(function () {

                        //console.log(888)

                        var _tid = $(this).data("tid");// alert(_tid);
                        var _tname = $(this).data("tname");
                        tagId = _tid;
                        tagName = _tname;

                        try {

                            //【数据统计】统计城市筛选的点击
                            var _category = "今日特价";
                            var _action = "筛选特色";
                            var _label = _tname;
                            var _value = 1;
                            var _nodeid = "";
                            _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

                            _statistic.push("今日特价", "筛选特色", _tname, _sourcekey, "");

                        } catch (e) {

                        }

                        loadMorePackageList(loadItem.id, loadItem.type, true, false);

                        //change tag filter
                        $(".tag-filter-txt").html(_tname);

                        tagFilterData.tagData["tagId"] = parseInt(_tid);

                        hideSelector();
                    });
                });

                //特色选择器的清空
                $("#tag-selector ._clear").unbind("click");
                $("#tag-selector ._clear").click(function () {

                    tagId = 0;
                    tagName = "";
                    loadMorePackageList(loadItem.id, loadItem.type, true, false);

                    //change city filter
                    $(".tag-filter-txt").html("特色");

                    tagFilterData.tagData["tagId"] = parseInt(tagId);

                    hideSelector();
                });
            });
        });
    }
    loadTagFilterInfo();

    //打开特色标签筛选
    var filterTagShow = function () {

        $("._selector-model").show();
        $("#tag-selector").slideDown(200);
    }
    $(".filter-section .tag-filter").click(filterTagShow);


    //加载筛选条件
    var filterCityData = new Vue({
        el: '#city-selector',
        data: {
            "hotCityData": null,
            "cityData": {}
        }
    })
    var loadFilterInfo = function () {

        var _paramDic = { albumid: albumIds };
        var urlSearch = "/api/coupon/GetDistinctSPUDistrictIDByAlbum";

        //使用省份级联接口
        if (useSubCityFilter) {
            urlSearch = "/api/coupon/GetGroupDistinctSPUDistrictIDByAlbum";
        }

        $.get(_Config.APIUrl + urlSearch, _paramDic, function (_data) {
            
            //console.log(_data);

            var _cityData = {};

            //使用省份级联
            if (useSubCityFilter) {
                _cityData["districtId"] = parseInt(districtId);
                _cityData["productSort"] = productSort;
                _cityData["DestinationList"] = _data;
            }
            else {
                _cityData = _data;
                _cityData["districtId"] = parseInt(districtId);
                _cityData["productSort"] = productSort;
            }

            //城市vue
            filterCityData.cityData = _cityData;

            Vue.nextTick(function () {

                if (districtName) {

                    $(".filter-section .city-filter-txt").html("{0}".format(districtName));
                    if (districtId > 0) {
                        //$(".filter-section .city").addClass("sel");
                    }
                }

                //省份的切换
                if ($("#city-selector .option")) {

                    $("#city-selector .option").each(function () {

                        $(this).unbind("click");
                        $(this).click(function () {

                            var _thisOption = $(this);

                            //this sub items
                            var _subid = _thisOption.data("subid");
                            if (_subid > -1) {

                                var _subItemsObj = $("#city-selector .sub-items-" + _subid);
                                if (_subItemsObj) {

                                    var _open = _subItemsObj.data("open");
                                    if (_open == 1) {

                                        //当前省的右箭头下箭头切换
                                        _thisOption.find("._right_icon").show();
                                        _thisOption.find("._up_icon").hide();

                                        //close mine
                                        _subItemsObj.slideUp(150);
                                        _subItemsObj.data("open", 0);

                                    }
                                    else {

                                        //当前option的右侧箭头 change right and down
                                        if (_cityFilterLastOption) {

                                            _cityFilterLastOption.find("._right_icon").show();
                                            _cityFilterLastOption.find("._up_icon").hide();
                                        }
                                        //$("#city-selector .option").each(function () {
                                        //    $(this).find("._right_icon").show();
                                        //    $(this).find("._up_icon").hide();
                                        //});

                                        //hide all subitems
                                        if (_cityFilterLastSubitems) {
                                            _cityFilterLastSubitems.hide();
                                            _cityFilterLastSubitems.data("open", 0);
                                        }
                                        //$("#city-selector .sub-items").each(function () {

                                        //    $(this).hide();
                                        //    $(this).data("open", 0);
                                        //});

                                        //当前省的右箭头下箭头切换
                                        _thisOption.find("._right_icon").hide();
                                        _thisOption.find("._up_icon").show();

                                        //open mine
                                        _subItemsObj.slideDown(200);
                                        _subItemsObj.data("open", 1);

                                        setTimeout(function () {

                                            //this click ele top
                                            //console.log(_thisOption)
                                            //console.log(_thisOption.offset().top)
                                            //console.log($win.height())
                                            //console.log($("#city-selector ._list").height())
                                            //console.log($("#city-selector ._list").scrollTop())

                                            var _thisOptionTop = _thisOption.offset().top - ($win.height() - $("#city-selector ._list").height());
                                            //console.log(_thisOptionTop)

                                            var _thisListScrollTop = $("#city-selector ._list").scrollTop();
                                            if (_thisOptionTop < 0) {

                                                $("#city-selector ._list").animate({ scrollTop: 0 }, 200);
                                            }
                                            else {

                                                //console.log(_thisOptionTop + _thisListScrollTop)
                                                $("#city-selector ._list").animate({ scrollTop: _thisOptionTop + _thisListScrollTop + 5 }, 200);
                                            }

                                        }, 220);

                                        _cityFilterLastOption = _thisOption;
                                        _cityFilterLastSubitems = _subItemsObj;

                                    }

                                }
                            }

                        });
                    });
                }

                //城市选择器的清空
                $("#city-selector ._clear").unbind("click");
                $("#city-selector ._clear").click(function () {

                    districtId = 0;
                    districtName = "";
                    loadMorePackageList(loadItem.id, loadItem.type, true, false);

                    //change city filter
                    $(".filter-section .city-filter-txt").html("{0}".format("度假地"));
                    $(".filter-section .city-filter-txt").removeClass("sel");

                    filterCityData.cityData["districtId"] = parseInt(districtId);

                    hideSelector();

                    if (showBanner) {

                        //隐藏筛选
                        $("#filter-section .right-item").hide();
                    }

                    //是否省份级联筛选
                    if (useSubCityFilter) {

                        //change right and down
                        $("#city-selector .option").each(function () {

                            if (!$(this).data("hot")) {
                                $(this).find("._right_icon").show();
                                $(this).find("._up_icon").hide();                            }
                        });

                        //hide all subitems
                        $("#city-selector .sub-items").each(function () {

                            if (!$(this).data("hot")) {

                                $(this).hide();
                                $(this).data("open", 0);
                            }
                        });

                        $("#city-selector ._list").animate({ scrollTop: 0 }, 0);
                    }
                });

                //城市点击事件绑定
                bindCityEvent();
            });
        });
    }
    loadFilterInfo();

    //加载热门度假地
    var loadHotFilterInfo = function () {

        //加载热门度假地
        $.get(_Config.APIUrl + "/api/HotelTheme/GetTempSource", { "id": 3861 }, function (_hotdata) {

            console.log(_hotdata);

            filterCityData.hotCityData = _hotdata;

            Vue.nextTick(function () {

                //城市点击事件绑定
                bindCityEvent();
            });

        });
    }
    if (showHotCity) {
        loadHotFilterInfo();
    }

    //城市点击事件绑定
    var bindCityEvent = function () {

        //城市选择器的选择
        var _clickItemList = $("#city-selector ._item");
        if (useSubCityFilter) {
            _clickItemList = $("#city-selector .sub-item");
        }
        if (showHotCity) {
            _clickItemList = $("#city-selector .sub-item-col");
        }
        _clickItemList.each(function () {

            $(this).unbind("click");
            $(this).click(function () {

                console.log(999)

                var _did = $(this).data("did");// alert(_did);
                var _dname = $(this).data("dname");
                districtId = _did;
                districtName = _dname;

                try {

                    //【数据统计】统计城市筛选的点击
                    var _category = "今日特价";
                    var _action = "筛选度假地";
                    var _label = _dname;
                    var _value = 1;
                    var _nodeid = "";
                    _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

                    _statistic.push("今日特价", "筛选度假地", _dname, _sourcekey, "");

                } catch (e) {

                }

                loadMorePackageList(loadItem.id, loadItem.type, true, false);

                //change city filter
                $(".filter-section .city-filter-txt").html("{0}".format(_dname));
                //$(".filter-section .city").addClass("sel");

                filterCityData.cityData["districtId"] = parseInt(districtId);

                hideSelector();
            });
        });
    }

    //选择器的背景点击事件（关闭所有的筛选）
    var hideSelector = function () {

        $("._selector-model").hide();
        $("._selector").slideUp(100);
    }
    $("._selector-model").click(hideSelector);

    //选择器的关闭事件
    $("._selector ._close").click(hideSelector);

    //打开全部城市
    var filterCityShow = function () {

        $("._selector-model").show();
        $("#city-selector").slideDown(200);
    }
    $(".filter-section .city").click(filterCityShow);

    //列表和矩阵展示切换
    var listSstyleChange = function () {

        var _thisStyleName = "列表";

        if (loadItem.listStyle == "list") {

            loadItem.listStyle = "grid";
            $(".filter-section .showchange ._icon").html("&#xe68c;");
            $(".filter-section .showchange .list-style-txt").html(" 列表浏览");

            _thisStyleName = "阵列";
        }
        else {

            loadItem.listStyle = "list";
            $(".filter-section .showchange ._icon").html("&#xe68b;");
            $(".filter-section .showchange .list-style-txt").html(" 阵列浏览");

            _thisStyleName = "列表";
        }

        try {

            //【数据统计】统计列表视图切换
            var _category = "今日特价";
            var _action = "视图切换";
            var _label = _thisStyleName;
            var _value = 0;
            var _nodeid = "";
            _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

            _statistic.push("今日特价", "视图切换", _thisStyleName, _sourcekey, "");

        } catch (e) {

        }

        loadMorePackageList(loadItem.id, loadItem.type, true, false);
    }
    $(".filter-section .showchange").click(listSstyleChange);

    //头banner
    var _loadTopOk = false;
    var loadTopList = function (_topId) {

        if (_loadTopOk) {
            return;
        }

        //get request
        var _listDic = { albumid: _topId, count: 1, start: 0, userid: userid };
        $.get(_Config.APIUrl + '/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID', _listDic, function (data) {

            //console.log(data);

            if (data) {

                _loadTopOk = true;

                var _topList = [];

                for (var _lnum = 0; _lnum < data.SKUCouponList.length; _lnum++) {
                    var _litem = data.SKUCouponList[_lnum];

                    //item图片
                    _litem.PicUrl = "";
                    if (_litem.PicList && _litem.PicList.length > 0 && _litem.PicPath.length > 2) {
                        _litem.PicUrl = _litem.PicList[0].replace("_theme", "_640x360");
                    }
                    if (!_litem.PicUrl) {
                        _litem.PicUrl = "/content/images/seat/img-viparea-item-3x2.png";
                    }

                    //不同环境下跳转url
                    if (isApp) {
                        _litem.url = "/coupon/product/" + _litem.SKUID + "?userid={userid}&_newpage=1&_sourcekey=sku_couponsales_banner";
                    }
                    else {
                        _litem.url = "/coupon/product/" + _litem.SKUID + "?userid=" + userid + "&_newpage=1&_newtitle=1&_sourcekey=sku_couponsales_banner";
                    }

                    var dtArr = {};
                    var dayArr = {};
                    var timeArr = {};

                    //now
                    var _now = new Date();
                    _litem["y0"] = _now.getFullYear();
                    _litem["mo0"] = _now.getMonth() + 1;
                    _litem["d0"] = _now.getDate();
                    _litem["h0"] = _now.getHours();
                    _litem["mi0"] = _now.getMinutes();
                    _litem["s0"] = _now.getSeconds();

                    try {
                        if (_litem.EffectiveTime) {
                            dtArr = (_litem.EffectiveTime).split("-");
                            dayArr = dtArr[2].split("T");
                            timeArr = dayArr[1].split(":");
                            _litem["y1"] = dtArr[0];
                            _litem["mo1"] = dtArr[1];
                            _litem["d1"] = dayArr[0];
                            _litem["h1"] = timeArr[0];
                            _litem["mi1"] = timeArr[1];
                            _litem["s1"] = timeArr[2];
                        }

                        if (_litem.SaleEndDate) {
                            dtArr = (_litem.SaleEndDate).split("-");
                            dayArr = dtArr[2].split("T");
                            timeArr = dayArr[1].split(":");
                            _litem["y2"] = dtArr[0];
                            _litem["mo2"] = dtArr[1];
                            _litem["d2"] = dayArr[0];
                            _litem["h2"] = timeArr[0];
                            _litem["mi2"] = timeArr[1];
                            _litem["s2"] = timeArr[2];
                        }
                    } catch (e) {

                    }

                    _topList.push(_litem);
                }

                productListData.topInfo = data;

                Vue.nextTick(function () {

                    //倒计时
                    runTimer_Top();

                    //Banners
                    $("#banner-section .home-hlist-panel").swiper({
                        slidesPerView: 'auto',
                        //loop:true,
                        speed: 200,
                        offsetPxBefore: _HomeSliderLiMarLeft,
                        offsetPxAfter: _HomeSliderLiMarLeft,
                        onTouchEnd: function (slider) {
                            if (slider.activeIndex + 1 < slider.slides.length) {
                                var li = $(slider.slides[slider.activeIndex + 1]);
                                var imgObj = li.find(".show-img");
                                setImgOriSrc(imgObj);
                            }
                        }
                    })

                    $("#banner-section .seat").hide();
                })
            }

        });
    }

    //刷新页面分享配置
    var refShareInfo = function () {

        var _shareTitle = "今日特价";
        var _shareLink = "http://www.zmjiudian.com/Coupon/CouponSales?albumId={0}&topId={1}&isgrid={2}&tagId={3}&districtId={4}&districtName={5}".format(albumIds, topId, isgridnum, tagId, districtId, (districtName ? encodeURI(districtName) : ""));

        //根据筛选条件动态变更分享配置
        if ((parseInt(districtId) && districtName) || tagName) {

            _shareTitle += "-";

            if (parseInt(districtId) && districtName) {
                _shareTitle += districtName;
            }

            if (tagName) {
                _shareTitle += tagName;
            }
        }

        //console.log(_shareTitle)
        //console.log(_shareLink)
        //console.log(shareDesc)
        //console.log(shareImgUrl)

        if (isApp) {

            var _param = { title: _shareTitle, content: shareDesc, photoUrl: shareImgUrl, shareLink: _shareLink };
            zmjd.setShareConfig(_param);
        }
        else if (isInWeixin) {

            loadWechat(_shareTitle, shareDesc, _shareLink, shareImgUrl, function () { });
        }
    }

    //初始记录访问记录
    try {

        //【数据统计】统计访问记录
        var _category = "今日特价";
        var _action = "访问";
        var _label = (_sourcekey ? _sourcekey + "+" : "") + albumIds + "&" + topId;
        var _value = 1;
        var _nodeid = "";
        _czc.push(﻿["_trackEvent", _category, _action, _label, _value, _nodeid]);

        _statistic.push("今日特价", "访问", albumIds + "&" + topId, _sourcekey, "");

    } catch (e) {

    }
}

$(function () {

    if (!isApp) {

        loadFunction();
    }

});

//【头部banner产品】运行倒计时
var timeDic_Top = [];
var runTimer_Top = function () {

    //run timer
    var timerTags = $(".timer-tag-top");
    if (timerTags) {
        for (var i = 0; i < timerTags.length; i++) {

            timeDic_Top[i] = {
                timerEntity: null,
                nowTime: null,
                endDate: null,
                closeDate: null,
                endTimerState: true,
                closeTimerState: false,
                initNowtime: function () {
                    this.nowTime = new Date(
                        parseInt(this.timerEntity.data("year0"))
                        , parseInt(this.timerEntity.data("month0")) - 1
                        , parseInt(this.timerEntity.data("day0"))
                        , parseInt(this.timerEntity.data("hour0"))
                        , parseInt(this.timerEntity.data("minute0"))
                        , parseInt(this.timerEntity.data("second0"))
                    ).getTime();
                },
                initEndtime: function () {
                    this.endDate = new Date(
                        parseInt(this.timerEntity.data("year1"))
                        , parseInt(this.timerEntity.data("month1")) - 1
                        , parseInt(this.timerEntity.data("day1"))
                        , parseInt(this.timerEntity.data("hour1"))
                        , parseInt(this.timerEntity.data("minute1"))
                        , parseInt(this.timerEntity.data("second1"))
                    ).getTime();
                },
                initClosetime: function () {
                    this.closeDate = new Date(
                        parseInt(this.timerEntity.data("year2"))
                        , parseInt(this.timerEntity.data("month2")) - 1
                        , parseInt(this.timerEntity.data("day2"))
                        , parseInt(this.timerEntity.data("hour2"))
                        , parseInt(this.timerEntity.data("minute2"))
                        , parseInt(this.timerEntity.data("second2"))
                    ).getTime();
                },
                init: function () {
                    this.initNowtime();
                    this.initEndtime();
                    this.initClosetime();
                },
                timerAction: function () {
                    if (this.endTimerState) {
                        var t = this.endDate - this.nowTime;
                        var d = Math.floor(t / (1000 * 60 * 60 * 24));
                        var h = Math.floor(t / 1000 / 60 / 60 % 24);
                        var h2 = h + (d * 24);
                        var m = Math.floor(t / 1000 / 60 % 60);
                        var m2 = m + ((d * 24) * 60);
                        var s = Math.floor(t / 1000 % 60);
                        var s2 = s + (((d * 24) * 60) * 60);

                        var _timeHtml = "";
                        var _dVal = "", _hVal = "", _mVal = "", _sVal = "";
                        if (d > 0) {
                            _dVal = d < 0 ? "00" : (d < 10 ? "0" + d : "" + d);
                            _timeHtml = "还有{0}天...".format(_dVal);
                        }
                        else if (h2 > 0) {
                            _hVal = h2 < 0 ? "00" : (h2 < 10 ? "0" + h2 : "" + h);
                            _timeHtml = "还有{0}小时...".format(_hVal);
                        }
                        else if (m > 0) {
                            _mVal = m2 < 0 ? "00" : (m2 < 10 ? "0" + m2 : "" + m2);
                            _timeHtml = "还有{0}分钟...".format(_mVal);
                        }
                        else {
                            _sVal = s2 < 0 ? "00" : (s2 < 10 ? "0" + s2 : "" + s2);
                            _timeHtml = "还有{0}秒...".format(_sVal);
                        }

                        _timeHtml = "未开始";

                        this.timerEntity.html(_timeHtml);

                        try {

                            if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                                this.stopEndAction();
                            }

                        } catch (e) { }

                        this.nowTime = this.nowTime + 1000;
                    }
                },
                timerCloseAction: function () {
                    if (this.closeTimerState) {
                        var t = this.closeDate - this.nowTime;
                        var d = Math.floor(t / (1000 * 60 * 60 * 24));
                        var h = Math.floor(t / 1000 / 60 / 60 % 24);
                        var h2 = h + (d * 24);
                        var m = Math.floor(t / 1000 / 60 % 60);
                        var m2 = m + ((d * 24) * 60);
                        var s = Math.floor(t / 1000 % 60);
                        var s2 = s + (((d * 24) * 60) * 60);

                        var _timeHtml = "";
                        var _dVal = "", _hVal = "", _mVal = "", _sVal = "";
                        if (d > 0) {
                            _dVal = d;// < 0 ? "00" : (d < 10 ? "0" + d : "" + d);
                            _timeHtml = "还有{0}天...".format(_dVal);
                        }
                        else if (h2 > 0) {
                            _hVal = h2;// < 0 ? "00" : (h2 < 10 ? "0" + h2 : "" + h);
                            _timeHtml = "还有{0}小时...".format(_hVal);
                        }
                        else if (m2 > 0) {
                            _mVal = m2;// < 0 ? "00" : (m2 < 10 ? "0" + m2 : "" + m2);
                            _timeHtml = "还有{0}分钟...".format(_mVal);
                        }
                        else {
                            _sVal = s2;// < 0 ? "00" : (s2 < 10 ? "0" + s2 : "" + s2);
                            _timeHtml = "还有{0}秒...".format(_sVal);
                        }

                        this.timerEntity.html(_timeHtml);

                        try {

                            if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {

                                this.stopCloseAction();
                            }

                        } catch (e) { }

                        this.nowTime = this.nowTime + 1000;
                    }
                },
                stopEndAction: function () {
                    this.endTimerState = false;
                    this.closeTimerState = true;
                    this.timerEntity.html("进行中");
                },
                stopCloseAction: function () {
                    this.closeTimerState = false;
                    this.timerEntity.html("已结束");
                }
            };

            //build
            timeDic_Top[i].timerEntity = $(timerTags[i]);

            //init
            timeDic_Top[i].init();

            //start
            timeDic_Top[i].timerAction();
            setInterval("gotime_Top(timeDic_Top[" + i + "])", 1000);
        }
    }

}
var gotime_Top = function (timeObj) {
    timeObj.timerAction();
    timeObj.timerCloseAction();
}

//【闪购】运行倒计时
var timeDic_Flash = [];
var runTimer_Flash = function () {

    //run timer
    var timerTags = $(".timer-tag-flash");
    if (timerTags) {
        for (var i = 0; i < timerTags.length; i++) {

            timeDic_Flash[i] = {
                timerEntity: null,
                nowTime: null,
                endDate: null,
                closeDate: null,
                endTimerState: true,
                closeTimerState: false,
                timerWillSection: $("#{0}".format($(timerTags[i]).data("willtimerid"))),
                timerSection: $("#{0}".format($(timerTags[i]).data("timerid"))),
                initNowtime: function () {
                    this.nowTime = new Date(
                        parseInt(this.timerEntity.data("year0"))
                        , parseInt(this.timerEntity.data("month0")) - 1
                        , parseInt(this.timerEntity.data("day0"))
                        , parseInt(this.timerEntity.data("hour0"))
                        , parseInt(this.timerEntity.data("minute0"))
                        , parseInt(this.timerEntity.data("second0"))
                    ).getTime();
                },
                initEndtime: function () {
                    this.endDate = new Date(
                        parseInt(this.timerEntity.data("year1"))
                        , parseInt(this.timerEntity.data("month1")) - 1
                        , parseInt(this.timerEntity.data("day1"))
                        , parseInt(this.timerEntity.data("hour1"))
                        , parseInt(this.timerEntity.data("minute1"))
                        , parseInt(this.timerEntity.data("second1"))
                    ).getTime();
                },
                initClosetime: function () {
                    this.closeDate = new Date(
                        parseInt(this.timerEntity.data("year2"))
                        , parseInt(this.timerEntity.data("month2")) - 1
                        , parseInt(this.timerEntity.data("day2"))
                        , parseInt(this.timerEntity.data("hour2"))
                        , parseInt(this.timerEntity.data("minute2"))
                        , parseInt(this.timerEntity.data("second2"))
                    ).getTime();
                },
                init: function () {
                    this.initNowtime();
                    this.initEndtime();
                    this.initClosetime();
                },
                timerAction: function () {
                    if (this.endTimerState) {
                        var t = this.endDate - this.nowTime;
                        var d = Math.floor(t / (1000 * 60 * 60 * 24));
                        var h = Math.floor(t / 1000 / 60 / 60 % 24);
                        var h2 = h + (d * 24);
                        var m = Math.floor(t / 1000 / 60 % 60);
                        var m2 = m + ((d * 24) * 60);
                        var s = Math.floor(t / 1000 % 60);
                        var s2 = s + (((d * 24) * 60) * 60);

                        var _timeHtml = "";
                        var _dVal = "", _hVal = "", _mVal = "", _sVal = "";
                        if (d > 0) {
                            _dVal = d;// < 0 ? "00" : (d < 10 ? "0" + d : "" + d);
                            _timeHtml = "距开始还有{0}天...".format(_dVal);
                        }
                        else if (h2 > 0) {
                            _hVal = h2;// < 0 ? "00" : (h2 < 10 ? "0" + h2 : "" + h);
                            _timeHtml = "距开始还有{0}小时...".format(_hVal);
                        }
                        else if (m2 > 0) {
                            _mVal = m2;// < 0 ? "00" : (m2 < 10 ? "0" + m2 : "" + m2);
                            _timeHtml = "距开始还有{0}分钟...".format(_mVal);
                        }
                        else {
                            _sVal = s2;// < 0 ? "00" : (s2 < 10 ? "0" + s2 : "" + s2);
                            _timeHtml = "距开始还有{0}秒...".format(_sVal);
                        }

                        //_timeHtml = "未开始";

                        this.timerEntity.html(_timeHtml);

                        try {

                            if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                                this.stopEndAction();

                                this.timerWillSection.hide();
                                this.timerSection.show();
                            }
                            else {
                                
                                this.timerWillSection.show();
                                this.timerSection.hide();
                            }

                        } catch (e) { }

                        this.nowTime = this.nowTime + 1000;
                    }
                },
                timerCloseAction: function () {
                    if (this.closeTimerState) {
                        var t = this.closeDate - this.nowTime;
                        var d = Math.floor(t / (1000 * 60 * 60 * 24));
                        var h = Math.floor(t / 1000 / 60 / 60 % 24);
                        var h2 = h + (d * 24);
                        var m = Math.floor(t / 1000 / 60 % 60);
                        var m2 = m + ((d * 24) * 60);
                        var s = Math.floor(t / 1000 % 60);
                        var s2 = s + (((d * 24) * 60) * 60);

                        var _timeHtml = "";
                        var _dVal = "", _hVal = "", _mVal = "", _sVal = "";
                        if (d > 0) {
                            _dVal = d;// < 0 ? "00" : (d < 10 ? "0" + d : "" + d);
                            _timeHtml = "进行中 还有{0}天...".format(_dVal);
                        }
                        else if (h2 > 0) {
                            _hVal = h2;// < 0 ? "00" : (h2 < 10 ? "0" + h2 : "" + h);
                            _timeHtml = "进行中 还有{0}小时...".format(_hVal);
                        }
                        else if (m2 > 0) {
                            _mVal = m2;// < 0 ? "00" : (m2 < 10 ? "0" + m2 : "" + m2);
                            _timeHtml = "进行中 还有{0}分钟...".format(_mVal);
                        }
                        else {
                            _sVal = s2;// < 0 ? "00" : (s2 < 10 ? "0" + s2 : "" + s2);
                            _timeHtml = "进行中 还有{0}秒...".format(_sVal);
                        }

                        this.timerEntity.html(_timeHtml);

                        try {

                            if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {

                                this.stopCloseAction();

                                this.timerWillSection.show();
                                this.timerSection.hide();
                            }
                            else {
                                
                                this.timerWillSection.hide();
                                this.timerSection.show();
                            }

                        } catch (e) { }

                        this.nowTime = this.nowTime + 1000;
                    }
                },
                stopEndAction: function () {
                    this.endTimerState = false;
                    this.closeTimerState = true;
                    this.timerEntity.html("进行中");
                },
                stopCloseAction: function () {
                    this.closeTimerState = false;
                    this.timerEntity.html("已结束");
                }
            };

            //build
            timeDic_Flash[i].timerEntity = $(timerTags[i]);

            //init
            timeDic_Flash[i].init();

            //start
            timeDic_Flash[i].timerAction();
            setInterval("gotime_Flash(timeDic_Flash[" + i + "])", 1000);
        }
    }

}
var gotime_Flash = function (timeObj) {
    timeObj.timerAction();
    timeObj.timerCloseAction();
}


function loginCallback(userid, skuid) {

    //跳转到book页面继续购买
    gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&_isoneoff=1&_newpage=1".format(skuid, 1, userid));
}

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        var defsrc = imgObj.attr("src"); //console.log(orisrc)
        imgObj.attr("src", orisrc);
        //imgObj.data("orisrc", "");
        imgObj.error(function () {
            imgObj.attr("src", defsrc);
        });
        $(this).data("load", "1");
    }
};

function goto(param) {
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (!isApp) {
        url = "http://www.zmjiudian.com/" + param;
    }

    this.location = url;
}

function gotopage(param) {
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    if (!isApp) {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

function gourl(url) {
        location.href = url;
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    loadFunction();

}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno, localLat, localLng) {

    //init data
    _InitApp(userid, apptype, appvercode, appverno, localLat, localLng);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}