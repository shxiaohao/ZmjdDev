var districtId = $P["districtId"];
var districtName = $P["districtName"];
var lat = $P["lat"];
var lng = $P["lng"];
var geoScopeType = $P["geoScopeType"];
var productSort = 0;

var _Config = new Config();
var userid = $("#userid").val();
var tempId = parseInt($("#tempId").val());
var albumIds = $("#albumId").val();
var categoryId = parseInt($("#categoryId").val());
var isApp = $("#isApp").val() == "1";
var shareNativeLink = $("#shareNativeLink").val();
var loadMorePackages = {};
var isload = true;
var nowLoadId = 0;

var ischildcateragory = $("#ischildcategory").val();

//模板配置
var tempData_Banner = {};
var tempData_Repack = {};
var tempData_Product = {};

//常规活动gift
var activeGiftData = null;

var loadItem = {};

var _HomeSliderItemWidth = 363;
var _HomeSliderLiMarLeft = 25;

//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

//加载模板定义
var loadTempSource = function () {

    var pcDic = { "id": tempId };
    $.get(_Config.APIUrl + "/api/HotelTheme/GetTempSource", pcDic, function (_templateData) {

        console.log("加载模板配置");
        console.log(_templateData);

        if (_templateData) {

            ////如果模板设置了 描述，则将描述设为页面title
            //if (_templateData.Description) {
            //    document.title = _templateData.Description;
            //}
            //else {
            //    document.title = "活动主页";
            //}

            //检查获取指定的模板配置信息
            if (_templateData.ContentList && _templateData.ContentList.length) {
                for (var i = 0; i < _templateData.ContentList.length; i++) {
                    var _item = _templateData.ContentList[i];

                    //初始各个模块的初始值（主要是配置，如数据请求接口、参数、UI控制等）
                    if (_item.Content) {
                        try {
                            _item.Content = _item.Content.replace("_USERID_", userid);
                            _item.ContentData = JSON.parse(_item.Content);
                        } catch (e) {
                            console.log(e)
                        }
                    }

                    switch (_item.Type) {
                        case "CouponCenterBanner": {

                            tempData_Banner = _item;

                            break;
                        }
                        case "CouponCenterRules": { break; }
                        case "CouponCenterRedpack": {

                            tempData_Repack = _item;

                            break;
                        }
                        case "CouponCenterList": {

                            tempData_Product = _item;

                            break;
                        }
                    }
                }
            }

            //加载具体内容
            loadDetailFunction();
        }

    });
}

//加载具体页面体
var loadDetailFunction = function () {

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
                case 33: { var _n = "上海玩乐"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 34: { var _n = "上海美食"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 35: { var _n = "苏州玩乐"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
                case 36: { var _n = "苏州美食"; _tabDetailItem.name = _n; if (_pageTitle) { _pageTitle += "&"; } _pageTitle += _n; break; }
            }

            tabDetailList.push(_tabDetailItem);
        }
    }

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
            "tabDetailList": tabDetailList,
            "tempDataBanner": tempData_Banner,
            "tempDataRepack": tempData_Repack,
            "tempDataProduct": tempData_Product,
        }
    })

    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();
    var tabsHeight = $("#tabs").height();
    var tabsActiveTop = 0;
    $("#top-banner-img").load(function () {
        tabsActiveTop = $("#tabs-active").offset().top;
    });

    if (tabDetailList.length < 2) {
        $("#tabs").hide();
        $("#tabs-active").hide();
        tabsHeight = 0;
    }

    //加载 常规节日活动礼
    var loadHolidayGift = function () {

        var getCouponListApi = _Config.APIUrl + "/api/coupon/GetVIPCouponGiftUserCouponList";
        $.get(getCouponListApi, {}, function (_data) {

            console.log(_data);

            if (_data) {

                if (activeGiftData) {
                    activeGiftData.AlbumsInfo = _data;
                }
                else {
                    activeGiftData = new Vue({
                        el: '#gift-coupon-list',
                        data: {
                            "AlbumsInfo": _data,
                            "tempDataBanner": tempData_Banner,
                            "tempDataRepack": tempData_Repack,
                            "tempDataProduct": tempData_Product,
                        }
                    })
                }

                $(".gift-coupon-list .close").click(function () {
                    _hideGiftCouponList();
                });
                $(".gift-coupon-list-model").click(function () {
                    _hideGiftCouponList();
                });

                Vue.nextTick(function () {

                    console.log(tempData_Repack)

                    //根据红包的领取类型，设置不同的领取操作
                    if (tempData_Repack.ContentData.type == "get-all") {

                        //检查当前用户是否领取活动券（之前是只有VIP才会验证，现在是放开所有用户都可以领取了 2018.07.16 haoy）
                        var _checkUserGetGiftState = function () {
                            $.get(_Config.APIUrl + "/api/coupon/IsUserHasGetNewVIPGift", { userid: userid }, function (_checkData) {

                                if (_checkData) {

                                    //隐藏活动红包
                                    _hideGiftCouponList();

                                    //显示已领取，隐藏领取
                                    $(".go-use-coupon").show();
                                    $(".holiday-gift-geted").show();
                                    $(".get-gift-coupon").hide();

                                    $(".active-coupon-section .look-img").show();
                                    $(".active-coupon-section .get-img").hide();
                                }
                                else {

                                    //弹出活动红包
                                    _showGiftCouponList();

                                    //隐藏已领取，显示领取
                                    $(".go-use-coupon").hide();
                                    $(".holiday-gift-geted").hide();
                                    $(".get-gift-coupon").show();

                                    $(".active-coupon-section .get-img").show();
                                    $(".active-coupon-section .look-img").hide();
                                }
                            });
                        }
                        _checkUserGetGiftState();

                        //分享领券事件
                        var _getGiftCouponEvent = function () {

                            //需登录
                            var _loginapphref_holidaygift = "whotelapp://loadJS?url=javascript:loginCallbackForHolidayGift('{userid}')&realuserid=1";
                            if (!parseInt(userid)) {

                                if (isApp) {
                                    location.href = _loginapphref_holidaygift;
                                    return;
                                }
                                else {
                                    _loginModular.show();
                                }
                                return;
                            }

                            goShareAndGetGift(userid);
                        }
                        $(".get-gift-coupon").click(_getGiftCouponEvent);

                        //去使用券
                        var goUseCoupon = function () {

                            _hideGiftCouponList();
                            $("#scroll_" + nowLoadId).animate({ scrollTop: tabsActiveTop }, 300);
                        }
                        $('.go-use-coupon').click(goUseCoupon);

                    }
                    else if (tempData_Repack.ContentData.type == "get-from-product") {

                        //隐藏活动红包
                        _hideGiftCouponList();

                        //检查当前用户对于指定产品的领取/购买状态
                        $.get(_Config.APIUrl + "/api/Coupon/CheckSkuGetStateByUserId", { skuId: tempData_Repack.ContentData.getskuid, userId: userid }, function (_checkData) {

                            console.log(_checkData);

                            if (_checkData) {

                                $(".active-coupon-section .look-img").show();
                                $(".active-coupon-section .get-img").hide();
                            }
                            else {

                                $(".active-coupon-section .get-img").show();
                                $(".active-coupon-section .look-img").hide();
                            }
                        });
                    }
                });
            }
        });
    }
    loadHolidayGift();

    var oneDivBeforeScrollTop = 0;
    $(".scroll-div").each(function () {

        var _scrollObj = $(this);

        _scrollObj.css("height", winHeight);

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

            //当tab的数量大于1的时候，才会显示
            if (tabDetailList.length > 1) {
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
                                _litem.url = "/coupon/product/" + _litem.SKUID + "?userid={userid}&_newpage=1&_newtitle=1";
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
            var _listDic = { albumid: id, count: _tabDetailObj.count, start: _tabDetailObj.start, userid: userid };
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
                
                $("#tabs").hide();
                $("#scroll_" + _relid).animate({ scrollTop: tabsActiveTop }, 300);
            });

        });

        //tabs item click event
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
                
                $("#tabs").hide();
                $("#scroll_" + _relid).animate({ scrollTop: tabsActiveTop }, 300);
            });

        });

        //分享提示图片动态加载
        var shareTipImg = $(".weixin-share-tip img");
        setImgOriSrc(shareTipImg);

        //右上角分享提示点击事件
        $(".weixin-share-tip").click(function () {
            $(this).hide();
        });
    }
    bindEvent();

    //选中当前菜单
    var selectMenu = function (id, isfirst) {

        nowLoadId = id;

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

        //根据红包的领取类型，设置不同的领取操作
        if (tempData_Repack.ContentData.type == "get-all") {

            //领取欢迎礼相关事件
            $(".open-active-coupon").each(function () {

                $(this).unbind("click");
                $(this).click(function () {
                    _showGiftCouponList();
                });
            });

            //查看欢迎礼相关事件
            $(".look-active-coupon").each(function () {

                $(this).unbind("click");
                $(this).click(function () {
                    _showGiftCouponList();
                });
            });
        }
        else if (tempData_Repack.ContentData.type == "get-from-product") {

            //领取欢迎礼相关事件
            $(".open-active-coupon").each(function () {

                $(this).unbind("click");
                $(this).click(function () {
                    gourl(tempData_Repack.ContentData.geturl);
                });
            });
        }
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

        loadTempSource();
    }

});

var _showGiftCouponList = function () {
    //$(".welcome-list").css("top", $(window).scrollTop() + 15);
    $(".gift-coupon-list").fadeIn(200);
    $(".gift-coupon-list-model").show();
}
var _hideGiftCouponList = function () {
    $(".gift-coupon-list").hide();
    $(".gift-coupon-list-model").hide();
}

//领取节日活动券 登录回调
function loginCallbackForHolidayGift(_userid) {
    goShareAndGetGift(_userid);
}

//1 去分享并领取节日活动券
var goShareAndGetGift = function (_userid) {

    if (isApp) {

        //app内默认6秒钟后自动领取
        setTimeout(function () {

            //领取并弹出
            genHolidayGift(_userid);

        }, 6000);

        //app内弹出分享
        gourl(shareNativeLink);
    }
    else if (1) {

        //微信内默认6秒钟后自动领取
        setTimeout(function () {

            //领取并弹出
            genHolidayGift(_userid);

        }, 6000);

        //弹出微信分享提示
        $(".weixin-share-tip").show();

        ////隐藏gift券弹窗
        //_hideGiftCouponList();

        ////弹出活动浮窗
        //$(".gift-coupon-ball").fadeIn();
    }
    else {
        alert("请至周末酒店APP或微信中领取哦~");
        return;
    }

    /************* 现在是放开所有用户都可以领取了 2018.07.16 haoy ************/
    ////VIP可领取
    //if (isVip) {

    //    //去领取的代码....
    //}
    //else {

    //    _hideGiftCouponList();

    //    //弹出活动浮窗
    //    $(".gift-coupon-ball").fadeIn();

    //    if (userid == "0") {

    //        if (isApp) {
    //            location.href = loginapphref;
    //            return;
    //        }
    //    }

    //    showModal(true);
    //}
    /**************************************************************************/
}

//2 领取节日活动券
var genHolidayGift = function (_userid) {

    $.get(_Config.APIUrl + "/api/coupon/SendVIPCouponActitity", { userid: _userid }, function (_getData) {

        if (_getData) {

            if (isApp) {
                alert("恭喜你已成功领取红包，礼券已发放至你的钱包，稍后可以到“钱包->现金券”中查看");
            }
            else {
                alert("恭喜你已成功领取红包，礼券已发放至你的钱包，稍后可以到“我的->我的券->现金券”中查看");
            }

            //显示已领取，隐藏领取
            $(".go-use-coupon").show();
            $(".holiday-gift-geted").show();
            $(".get-gift-coupon").hide();

            $(".active-coupon-section .look-img").show();
            $(".active-coupon-section .get-img").hide();
        }
        else {
            //隐藏已领取，显示领取
            $(".go-use-coupon").hide();
            $(".holiday-gift-geted").hide();
            $(".get-gift-coupon").show();

            $(".active-coupon-section .get-img").show();
            $(".active-coupon-section .look-img").hide();
        }
    });
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

    loadTempSource();

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