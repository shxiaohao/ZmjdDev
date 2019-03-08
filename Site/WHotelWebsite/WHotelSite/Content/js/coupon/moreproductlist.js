var districtId = $P["districtId"];
var districtName = $P["districtName"];
var lat = $P["lat"];
var lng = $P["lng"];
var geoScopeType = $P["geoScopeType"];
var productSort = 0;

var _Config = new Config();
var userid = $("#userid").val();
var albumIds = $("#albumId").val();
var categoryId = parseInt($("#categoryId").val());
var isApp = $("#isApp").val() == "1";
var isDouble11 = $("#isDouble11").val() == "1";
var loadMorePackages = {};
var isload = true;

var ischildcateragory = $("#ischildcategory").val();

var loadItem = {};

var _HomeSliderItemWidth = 363;
var _HomeSliderLiMarLeft = 25;

//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

var loadFunction = function () {

    var wwidth = $(window).width();
    if (wwidth >= 430) { }							//meizu mx3
    if (wwidth < 410) { _HomeSliderItemWidth = 328; _HomeSliderLiMarLeft = 23; }	//ip6
    if (wwidth == 360) { _HomeSliderItemWidth = 317; _HomeSliderLiMarLeft = 22; }
    if (wwidth < 350) { _HomeSliderItemWidth = 280; _HomeSliderLiMarLeft = 20; }	//ip4/ip5

    $('body').append("<style>.home-hlist-panel{ width: " + wwidth + "px !important; }.home-hlist-panel .item{ width: " + _HomeSliderItemWidth + "px !important; }</style>");
    $('body').append("<style>.home-hlist-panel .sml-item{ width: " + ((_HomeSliderItemWidth / 2) - 5) + "px !important; }</style>");

    if (geoScopeType == "" || geoScopeType == undefined) {
        geoScopeType = "3";
        if (lat == undefined) lat = 31.236237;
        if (lng == undefined) lng = 121.389139;
    }
    else {
        if (lat == undefined) lat = 0;
        if (lng == undefined) lng = 0;
    }

    if (districtId == "" || districtId == undefined) districtId = "2";
    if (districtName == "" || districtName == undefined) districtName = "上海";

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

    //var loadUrl = "/Coupon/MoreProduct_SKU";

    ////其它特惠套餐
    //if (albumId == "0") {
    //    loadUrl = "/Coupon/MoreProduct_SKU";
    //}

    var _pageTitle = "";
    var tabDetailList = [];

    if (albumIds && albumIds != "0") {
        var _ids = albumIds.split(',');
        for (var _idnum = 0; _idnum < _ids.length; _idnum++) {
            var _id = parseInt(_ids[_idnum]);
            var _tabDetailItem = {
                id: _id,
                type: "album",
                name: "其它",
                start: 0,
                count: 10,
                skuList: [],
                AdData: {}
            }

            switch (_id) {
                case 1: { var _n = "专辑"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 2: { var _n = "美食精选"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 3: { var _n = "玩乐精选"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 4: { var _n = "每日闪购"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
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
    else {

        var _tabDetailItem = {
            id: categoryId,
            type: "category",
            name: "",
            start: 0,
            count: 10,
            skuList: [],
            AdData: {}
        }
        if (ischildcateragory == "0") {
            switch (categoryId) {
                case 14: { _tabDetailItem.name = "美食"; break; }
                case 20: { _tabDetailItem.name = "玩乐"; break; }
                case 15: { _tabDetailItem.name = "酒店房券"; break; }
            }
        }
        else {
            switch (categoryId) {
                case 1: { _tabDetailItem.name = "乐园"; break; }
                case 2: { _tabDetailItem.name = "亲子剧";  break; }
                case 3: { _tabDetailItem.name = "亲子美食"; break; }
                case 4: { _tabDetailItem.name = "景区"; break; }
                case 5: { _tabDetailItem.name = "亲近自然"; break; }
                case 6: { _tabDetailItem.name = "寓教于乐"; break; }
                case 7: { _tabDetailItem.name = "玩具"; break; }
                case 8: { _tabDetailItem.name = "绘本"; break; }
            }
        }

        tabDetailList.push(_tabDetailItem);
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
            "tabDetailList": tabDetailList
        }
    })

    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();
    var tabsHeight = $("#tabs").height();
    var tabsActiveTop = 0;
    if (isDouble11) {
        $("#double11-banner1").load(function () {
            tabsActiveTop = $("#tabs-active").offset().top;
        });
    }

    if (tabDetailList.length < 2) {
        $("#tabs").hide();
        tabsHeight = 0;
    }

    var oneDivBeforeScrollTop = 0;
    $(".scroll-div").each(function () {

        var _scrollObj = $(this);

        if (isDouble11) {
            _scrollObj.css("height", winHeight);
        }
        else {
            _scrollObj.css("height", winHeight - tabsHeight);
        }

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

            if (isDouble11) {

                if (_divTop > tabsActiveTop) {
                    $("#tabs").show();
                    //_scrollObj.css("height", winHeight - tabsHeight);
                } else {
                    $("#tabs").hide();
                    //_scrollObj.css("height", winHeight);
                }
            }

            var afterScrollTop = _divTop,
                delta = afterScrollTop - oneDivBeforeScrollTop;
            if (delta === 0) return false;
            scrollTopDownFunc(delta > 0 ? "down" : "up", afterScrollTop);
            oneDivBeforeScrollTop = afterScrollTop;

        });
    });

    var scrollTopDownFunc = function (delta, afterScrollTop) {

        if ($("#filter-section")) {

            if (delta == "up") {

                if (afterScrollTop > 0) {

                    $("#filter-section").addClass("filter-section-fixed");
                }
                else {

                    $("#filter-section").removeClass("filter-section-fixed");
                }
            }
            else {

                $("#filter-section").removeClass("filter-section-fixed");
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

            //默认显示loadding
            $("#scrollpageloading-" + id).show();

            //第一次绑定完后，做一些初始操作
            if (isfirst) {
                $(".scrollpageloading").html('<img class="img-first" src="http://whfront.b0.upaiyun.com/app/img/loading-changes.gif" alt="" />');
            }
            else {
                $(".scrollpageloading").find("img").removeClass("img-first");
            }

            //加载指定专辑/分类的产品list
            var loadProductListCallback = function (data) {

                console.log(data);

                if (data) {

                    //动态设置页面标题
                    if (isDouble11) {
                        document.title = "双十一专场";
                    }
                    else {
                        if (type == "category") {
                            document.title = _tabDetailObj.name;
                        }
                        else {
                            document.title = data.AlbumName;
                        }
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

                        //$(".more-seat-slider").hide();
                        $("#dealsSection-seat-" + id).hide();
                        if ($("#dealsSection-seat2-" + id)) $("#dealsSection-seat2-" + id).hide();

                        for (var _lnum = 0; _lnum < data.SKUCouponList.length; _lnum++) {
                            var _litem = data.SKUCouponList[_lnum];

                            //item图片
                            _litem.PicUrl = "";
                            if (_litem.PicList && _litem.PicList.length > 0 && _litem.PicPath.length > 2) {
                                _litem.PicUrl = _litem.PicList[0].replace("_theme", "_350X350");
                            }
                            if (!_litem.PicUrl) {
                                _litem.PicUrl = "/content/images/seat/home-hotel-load-1x1.png";
                            }

                            //不同环境下跳转url
                            if (isApp) {
                                _litem.url = "/coupon/product/" + _litem.SKUID + "?userid={userid}&_newpage=1";
                            }
                            else {
                                _litem.url = "/coupon/product/" + _litem.SKUID + "?userid=" + userid + "&_newpage=1&_newtitle=1";
                            }

                            for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                                var _tabDetailItem = productListData.tabDetailList[_tnum];
                                if (_tabDetailItem.id === id) {
                                    _tabDetailItem.skuList.push(_litem);
                                    break;
                                }
                            }

                            //双11下的代金券信息
                            _litem["UserCouponDefine"] = {};
                            if (_litem.UserCouponDefineList && _litem.UserCouponDefineList.length) {

                                var _listCouponItem = _litem.UserCouponDefineList[0];
                                if (_listCouponItem) {
                                    _litem.UserCouponDefine = _listCouponItem;
                                }
                            }

                            //计算差价
                            var _diffPrice = 0;
                            if (_litem.SKUPrice > _litem.SKUVipPrice) {
                                _diffPrice = _litem.SKUPrice - _litem.SKUVipPrice;
                            }
                            else if (_litem.SKUMarketPrice > _litem.SKUVipPrice) {
                                _diffPrice = _litem.SKUMarketPrice - _litem.SKUVipPrice;
                            }
                            _litem["DiffPrice"] = returnFloat(_diffPrice);

                            //productListData.tabDetailList.skuList.push(_litem);
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

                            //动态加载图
                            loadImgsEvent();

                            //加载头banner
                            loadTopAdList(id);
                        })

                        isload = true;
                    }
                    else {
                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (!isfirst) {
                            $("#scrollpageloading-" + id).hide();
                        }
                    }
                }
                else {
                    //如果是第一页，则不显示“没有更多了”，不然很奇怪
                    if (!isfirst) {
                        $("#scrollpageloading-" + id).hide();
                    }
                }

                //if (isDouble11 && goScrollTop) {
                //    $("#tabs").hide();
                //    $("#scroll_" + id).animate({ scrollTop: tabsActiveTop }, 300);
                //}
            }

            //console.log(type);

            //get request
            if (type == "category") {

                //暂时美食玩乐等产品分类查询，不做周边查询
                if (id === 14 || id === 20) {
                    if (parseInt(districtId)) {

                        //默认城市选择
                        if (!$(".filter-section .city").html().indexOf("全部城市") && districtName) {

                            //change city filter
                            $(".filter-section .city").html(districtName);
                            $(".filter-section .city").addClass("sel");
                        }
                    }
                    else {

                        lat = 0;
                        lng = 0;
                        geoScopeType = 1;
                    }
                }

                if (ischildcateragory=="1") {
                    var _listDic = { ID: id, districtId: districtId, lat: lat, lng: lng, geoScopeType: geoScopeType, count: _tabDetailObj.count, start: _tabDetailObj.start, userid: userid, sort: productSort, loclat: LocalLat, loclng: LocalLng };
                    $.get(_Config.APIUrl + '/api/coupon/GetProductSKUCouponListByID', _listDic, function (data) {
                        loadProductListCallback(data);
                    });
                }
                else {
                    var _listDic = { category: id, districtId: districtId, lat: lat, lng: lng, geoScopeType: geoScopeType, count: _tabDetailObj.count, start: _tabDetailObj.start, userid: userid, sort: productSort, loclat: LocalLat, loclng: LocalLng };
                    $.get(_Config.APIUrl + '/api/coupon/GetProductSKUCouponList', _listDic, function (data) {
                        loadProductListCallback(data);
                    });
                }
            }
            else if (type == "album") {

                var _listDic = { albumid: id, count: _tabDetailObj.count, start: _tabDetailObj.start, userid: userid };
                $.get(_Config.APIUrl + '/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID', _listDic, function (data) {
                    loadProductListCallback(data);
                });
            }
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
                if (isDouble11) {
                    $("#tabs").hide();
                    $("#scroll_" + _relid).animate({ scrollTop: tabsActiveTop }, 300);
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
                if (isDouble11) {
                    $("#tabs").hide();
                    $("#scroll_" + _relid).animate({ scrollTop: tabsActiveTop }, 300);
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

    //双11特殊处理
    if (isDouble11) {

        if ($(".rule-link")) {
            $(".rule-link").click(function () {

                $(this).hide();
                $(this).data("op", 1);
                $(".rule-desc").show();
                tabsActiveTop = $("#tabs-active").offset().top;
            });
        }
    }

    //加载筛选条件与排序选项
    var filterCityData = null;
    var sortData = null;
    var loadFilterAndSortInfo = function () {

        var _paramDic = { catorgoryParentId: categoryId, paytype: 0 };
        var urlSearch = "/api/coupon/GetSearchItem";
        //加载子分类搜索城市
        if (ischildcateragory == "1") {
            _paramDic = { productTagID: categoryId, paytype: 0 };
            urlSearch = "/api/coupon/GetDistinctSPUDistrictIDByProductTagID";
        }

        $.get(_Config.APIUrl + urlSearch, _paramDic, function (_data) {
            
            console.log(_data);

            _data["districtId"] = parseInt(districtId);
            _data["productSort"] = productSort;

            //城市vue
            if (filterCityData) {
                filterCityData.cityData = _data;
            }
            else {
                filterCityData = new Vue({
                    el: '#city-selector',
                    data: { "cityData": _data }
                })
            }

            //排序vue
            if (sortData) {
                sortData.sortData = _data;
            }
            else {
                sortData = new Vue({
                    el: '#sort-selector',
                    data: { "sortData": _data }
                })
            }

            Vue.nextTick(function () {
                $(".filter-section .city").html(districtName);
                if (districtId > 0) {
                    $(".filter-section .city").addClass("sel");
                }
                

                //城市选择器的选择
                $("#city-selector ._item").each(function () {

                    $(this).click(function () {

                        var _did = $(this).data("did");// alert(_did);
                        var _dname = $(this).data("dname");
                        districtId = _did;

                        loadMorePackageList(loadItem.id, loadItem.type, true, false);

                        //change city filter
                        $(".filter-section .city").html(_dname);
                        $(".filter-section .city").addClass("sel");

                        filterCityData.cityData["districtId"] = parseInt(districtId);

                        hideSelector();
                    });
                });

                //城市选择器的清空
                $("#city-selector ._clear").click(function () {

                    districtId = 0;
                    loadMorePackageList(loadItem.id, loadItem.type, true, false);

                    //change city filter
                    $(".filter-section .city").html("全部城市");
                    $(".filter-section .city").removeClass("sel");

                    filterCityData.cityData["districtId"] = parseInt(districtId);

                    hideSelector();
                });

                $("#sort-selector ._item").each(function () {

                    $(this).click(function () {

                        var _sort = $(this).data("sort");
                        var _sortname = $(this).data("sortname");
                        productSort = parseInt(_sort);
                        loadMorePackageList(loadItem.id, loadItem.type, true, false);

                        //change sort filter
                        $(".filter-section .sort").html(_sortname);
                        $(".filter-section .sort").addClass("sel");

                        sortData.sortData["productSort"] = productSort;

                        hideSelector();
                    });
                });

                //排序选择器的清空
                $("#sort-selector ._clear").click(function () {

                    productSort = 0;
                    loadMorePackageList(loadItem.id, loadItem.type, true, false);

                    //change sort filter
                    $(".filter-section .sort").html("排序");
                    $(".filter-section .sort").removeClass("sel");

                    sortData.sortData["productSort"] = productSort;

                    hideSelector();
                });
            });
        });
    }
    loadFilterAndSortInfo();

    //选择器的背景点击事件（关闭所有的筛选/排序）
    var hideSelector = function () {

        $("._selector-model").hide();
        $("._selector").slideUp(100);
    }
    $("._selector-model").click(hideSelector);

    //选择器的关闭事件
    $("._selector ._close").click(hideSelector);

    //打开排序
    var filtersSortShow = function () {

        $("._selector-model").show();
        $("#sort-selector").slideDown(200);
    }
    $(".filter-section .sort").click(filtersSortShow);

    //打开全部城市
    var filterCityShow = function () {

        $("._selector-model").show();
        $("#city-selector").slideDown(200);
    }
    $(".filter-section .city").click(filterCityShow);

    //头banner
    var topAdListData = null;
    var loadTopAdList = function (_tabId) {

        if ($("#banner-section")) {

            //7美食头banner  8玩乐头banner
            var _adTypeId = 0;
            switch (_tabId) {
                case 14: { _adTypeId = 7; break; }
                case 20: { _adTypeId = 8; break; }
            }

            if (_adTypeId) {

                var _paramData = { "type": _adTypeId, "curuserid": userid };
                $.get(_Config.APIUrl + "/api/hotel/GetHomeOnlineBannersByType", _paramData, function (_data) {

                    console.log(_data);

                    for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                        var _tabDetailItem = productListData.tabDetailList[_tnum];
                        if (_tabDetailItem.id === _tabId) {

                            _tabDetailItem["AdData"] = _data

                            break;
                        }
                    }

                    //if (topAdListData) {
                    //    topAdListData.AdData = _data;
                    //}
                    //else {
                    //    topAdListData = new Vue({
                    //        el: '#banner-section',
                    //        data: { "AdData": _data }
                    //    })
                    //}

                    Vue.nextTick(function () {

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

                });
            }
        }
    }
}

$(function () {

    if (!isApp) {

        loadFunction();
    }

});

    //sku直接跳转到订单确认页面
var submitFun = function (skuid) {

    var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}','" + skuid + "')&realuserid=1";
    if (userid == "0") {

        //app环境下，如果没有登录则弹出登录
        if (isApp) {
            location.href = loginapphref;
            return;
        }
        else {
            _loginModular.show();
        }
    }
    else {

        //跳转到book页面继续购买
        var _sellnum = 1;
        gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&_isoneoff=1&_newpage=1".format(skuid, _sellnum, userid));
    }

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