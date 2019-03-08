var _Config = new Config();
var userid = $("#userid").val();
var albumId = $("#albumId").val();
var t = $("#t").val();
var userlat = $("#userlat").val();
var userlng = $("#userlng").val();
var areas = $("#areas").val();
var airhoteldate = $("#airhoteldate").val();
var startdid = parseInt($("#startdid").val());
var stopdid = parseInt($("#stopdid").val());
var grid = $("#grid").val();
var geoScopeType = 0;
var districtId = 0;
var loadMorePackages = {};

var isload = true;
var loadId = 0;

$(function () {

    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();

    if (areas) {

        //指定了area区域划分
        var _ids = areas.split(',');
        var tabDetailList = [];
        for (var _idnum = 0; _idnum < _ids.length; _idnum++) {
            var _id = parseInt(_ids[_idnum]);
            var _tabDetailItem = {
                id: _id,
                name: "其它",
                start: 0,
                count: 6,
                pkList: []
            }

            switch (_id) {
                case 1: { var _n = "国内"; _tabDetailItem.name = _n; break; }
                case 2: { var _n = "港澳"; _tabDetailItem.name = _n; break; }
                case 3: { var _n = "日本"; _tabDetailItem.name = _n; break; }
                case 4: { var _n = "东南亚"; _tabDetailItem.name = _n; break; }
                case 5: { var _n = "欧美澳"; _tabDetailItem.name = _n; break; }
            }

            tabDetailList.push(_tabDetailItem);
        }

        //初始绑定tabs
        var productListData = new Vue({
            el: "#tabs",
            data: {
                "tabDetailList": tabDetailList
            }
        })

        //默认第一个
        loadId = tabDetailList[0].id;

        //初始绑定list
        var productListData = new Vue({
            el: "#more-vue-obj",
            data: {
                "tabDetailList": tabDetailList
            }
        })

        var tabsHeight = $("#tabs").height();
        if (tabDetailList.length < 2) {
            $("#tabs").hide();
            tabsHeight = 0;
        }

        $(".scroll-div").each(function () {

            var _scrollObj = $(this);
            _scrollObj.css("height", winHeight - tabsHeight);
            _scrollObj.scroll(function () {

                var _relid = _scrollObj.data("relid");

                var _tagTop = $(".more-packages-foot").offset().top;
                var _divTop = _scrollObj[0].scrollTop;
                var _divHeight = _scrollObj.height();
                var _divScrollHeight = _scrollObj[0].scrollHeight;

                if (_divTop >= (_divScrollHeight - _divHeight)) {
                    //console.log("div到底了")
                    loadMorePackageList(_relid, false);
                }
            });
        });

        //var start = 0;
        //var count = 6;

        var loadMorePackageList = function (id, isfirst) {

            if (isload) {

                isload = false; 
                //_Config.APIUrl = "http://192.168.1.114:8000";
                //_Config.APIUrl = "http://api.zmjd100.com";
              
                //获取start，count等
                var _tabDetailObj = {};
                for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                    var _tabDetailItem = productListData.tabDetailList[_tnum];
                    if (_tabDetailItem.id === id) {
                        _tabDetailObj = _tabDetailItem;
                        break;
                    }
                }

                //console.log(_tabDetailObj)

                var _listDic = { albumId: albumId, count: _tabDetailObj.count, start: _tabDetailObj.start, curUserID: userid, ckvip: 0, area: id, grid: grid };
                $.get(_Config.APIUrl + '/api/hotel/GetRecommendHotelResultByAlbumId', _listDic, function (data) {

                    //console.log(data);

                    if (data) {

                        if (data.HotelList && data.HotelList.length) {

                            //第一次绑定完后，做一些初始操作
                            if (isfirst) {
                                if (data.HotelList.length >= _tabDetailObj.count) {
                                    $("#scrollpageloading-" + id).show();
                                }
                                $(".more-seat-slider").hide();
                            }

                            for (var _lnum = 0; _lnum < data.HotelList.length; _lnum++) {
                                var _litem = data.HotelList[_lnum];

                                //item图片
                                if (!_litem.HotelPicUrl) {
                                    _litem.HotelPicUrl = "http://whfront.b0.upaiyun.com/app/img/home/home-load2-3x2.png";
                                }

                                _litem.HotelPicUrl = _litem.HotelPicUrl.replace("_theme", "_640x426");

                                //不同环境下跳转url
                                if (isApp) {
                                    _litem.url = "/Hotel/Package/" + _litem.PID + "?userid={userid}&_newpage=1&_newtitle=1";
                                }
                                else {
                                    _litem.url = "/Hotel/Package/" + _litem.PID + "?userid=" + userid;
                                }

                                for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                                    var _tabDetailItem = productListData.tabDetailList[_tnum];
                                    if (_tabDetailItem.id === id) {
                                        _tabDetailItem.pkList.push(_litem);
                                        break;
                                    }
                                }
                                //productListData.tabDetailList.pkList.push(_litem);
                            }

                            for (var _tnum = 0; _tnum < productListData.tabDetailList.length; _tnum++) {
                                var _tabDetailItem = productListData.tabDetailList[_tnum];
                                console.log(_tabDetailItem)
                                console.log(id)
                                if (_tabDetailItem.id === id) {
                                    console.log(_tabDetailObj)
                                    _tabDetailItem.start = (_tabDetailObj.start + _tabDetailObj.count)
                                    break;
                                }
                            }
                            //start += count;

                            Vue.nextTick(function () {
                                loadImgsEvent();
                            })
                        }
                        else {
                            //如果是第一页，则不显示“没有更多了”，不然很奇怪
                            if (!isfirst) {
                                $("#scrollpageloading-" + id).html("<div>没有更多了</div>");
                            }
                        }
                    }
                    else {
                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (!isfirst) {
                            $("#scrollpageloading-" + id).html("<div>没有更多了</div>");
                        }
                    }

                    isload = true;

                });
            }

        }

        //页面初始加载
        setTimeout(function () {
            selectMenu(loadId);
            loadMorePackageList(loadId, true);
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

                    if (!_tabDetailObj.pkList || !_tabDetailObj.pkList.length) {
                        loadMorePackageList(_relid, true);
                    }

                });

            });

        }
        bindEvent();

        //选中当前菜单
        var selectMenu = function (id) {

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

    }
    else {

        var loadUrl = "/App/More_AlbumPackages";

        //其它特惠套餐
        if (albumId == "1") {
            loadUrl = "/App/More_AlbumPackages";
        }
            //VIP专享
        else if (albumId == "10") {
            loadUrl = "/App/More_AlbumPackages";

            //VIP专享套餐默认当前周边
            geoScopeType = 3;
        }
            //酒+机套餐
        else if (albumId == "12") {
            loadUrl = "/App/More_AlbumPackages";
        }
            //元旦VIP专享
        else if (albumId == "21") {
            loadUrl = "/App/More_AlbumPackages";
        }
            //最新推荐
        else if (albumId == "37") {
            loadUrl = "/App/More_AlbumPackages";
        }
            //闪购
        else if (t == 1) {
            loadUrl = "/App/More_Flash";
        }
            //酒机
        else if (t == 2) {
            loadUrl = "/App/More_Group";
        }
            //最近浏览的酒店
        else if (t == 3) {
            loadUrl = "/App/More_RecentSee";
        }
            //朋友推荐的酒店
        else if (t == 4) {
            loadUrl = "/App/More_FriendRec";
        }
            //本周特惠
        else if (t == 5) {
            loadUrl = "/App/More_SaleHotelPackages";
        }
            //周末好去处
        else if (t == 6) {
            loadUrl = "/App/More_RecentSee";
        }

        $win.on('scroll', function () {
            var tagTop = $(".more-packages-foot").offset().top;
            var winTop = $win.scrollTop();
            var winHeight = $win.height();

            if (winTop >= tagTop - winHeight - 100) {
                loadMorePackages(false);
            }
        });

        var start = 0;
        var count = 10;

        //load
        loadMorePackages = function (isfirst) {
            if (isload) {

                isload = false;

                //下一页
                start += count;
                if (isfirst) { start = 0; }

                //默认显示loadding
                $(".scrollpageloading").show();

                //第一次绑定完后，做一些初始操作
                if (isfirst) {
                    $(".more-packages").html("");
                    $(".scrollpageloading").html('<img class="img-first" src="http://whfront.b0.upaiyun.com/app/img/loading-changes.gif" alt="" />');
                }
                else {
                    $(".scrollpageloading").find("img").removeClass("img-first");
                }

                var _paramDic = {
                    userid: userid,
                    s: start,
                    c: count,
                    albumId: albumId,
                    t: t,
                    userlat: userlat,
                    userlng: userlng,
                    geoScopeType: geoScopeType,
                    districtid: districtId,
                    airhoteldate: airhoteldate,
                    startdid: startdid,
                    stopdid: stopdid,
                    grid: grid
                };
                //console.log(_paramDic)

                $.get(loadUrl, _paramDic, function (htmls) {

                    if (htmls) {

                        if (htmls != "" && (htmls.indexOf("ul") >= 0 || htmls.indexOf("b-list") >= 0)) {

                            if (isfirst) {
                                $(".scrollpageloading").show();
                                $(".more-seat-slider").hide();
                                $(".more-packages").html("");
                                $("html,body").animate({ scrollTop: 0 }, 300);
                            }

                            $(".more-packages").html($(".more-packages").html() + htmls);

                            $("img").lazyload({
                                threshold: 20,
                                placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-16x9.png",
                                effect: "show"
                            });
                        }
                        else {
                            //如果是第一页，则不显示“没有更多了”，不然很奇怪
                            if (!isfirst) {
                                $(".scrollpageloading").html("<div>没有更多了</div>");
                            }
                        }

                        isload = true;
                    }
                });
            }
        }
        setTimeout(function () { loadMorePackages(true); }, 100);

        //城市周边点击
        var clickAroundCity = function (_lat, _lng, _dname) {

            userlat = _lat;
            userlng = _lng;
            geoScopeType = 3;

            $(".more-top .districtName").html("&#xe629;" + _dname);
            $(".more-seat-slider").show();
            $(".more-packages").html("");
            $(".scrollpageloading").hide();
            $(".scrollpageloading").html("<img src='http://whfront.b0.upaiyun.com/app/img/loading.gif' alt='' />");
            loadMorePackages(true);

            hideCityFun();
        }

        //指定城市点击
        var clickCity = function (_did, _dname) {

            districtId = _did;
            userlat = 0;
            userlng = 0;
            geoScopeType = 0;
            start = 0;

            $(".more-top .districtName").html("&#xe629;" + _dname);
            $(".more-seat-slider").show();
            $(".more-packages").html("");
            $(".scrollpageloading").hide();
            $(".scrollpageloading").html("<img src='http://whfront.b0.upaiyun.com/app/img/loading.gif' alt='' />");
            loadMorePackages(true);

            hideCityFun();
        }

        console.log(albumId)

        switch (albumId) {

            //VIP专享
            case "10": {

                //加载地址筛选
                if ($(".city-section").html()) {

                    var _cityDic = { "albumsID": albumId, "lat": 0, "lng": 0, "v": 1 };
                    $.get("http://api.zmjiudian.com//api/hotel/GetDistrictInfoForAlbum", _cityDic, function (_data) {

                        //XX及周边的点击事件
                        $(".city-section .single-item .item").click(function () {
                            clickAroundCity($(this).data("lat"), $(this).data("lng"), $(this).data("dname"));
                        });


                        if (_data && _data.dicHotelDestInfoList && _data.dicHotelDestInfoList.length > 0) {
                            new Vue({
                                el: '#city-section0',
                                data: { "CityInfo": _data.dicHotelDestInfoList[0] }
                            })

                            $("#city-section0 .list .item").each(function () {
                                $(this).click(function () {
                                    clickCity($(this).data("did"), $(this).data("dname"));
                                });
                            });
                        }

                        if (_data && _data.dicHotelDestInfoList && _data.dicHotelDestInfoList.length > 1) {
                            new Vue({
                                el: '#city-section1',
                                data: { "CityInfo": _data.dicHotelDestInfoList[1] }
                            })

                            $("#city-section1 .list .item").each(function () {
                                $(this).click(function () {
                                    clickCity($(this).data("did"), $(this).data("dname"));
                                });
                            });
                        }

                        if (_data && _data.dicHotelDestInfoList && _data.dicHotelDestInfoList.length > 2) {
                            new Vue({
                                el: '#city-section2',
                                data: { "CityInfo": _data.dicHotelDestInfoList[2] }
                            })

                            $("#city-section2 .list .item").each(function () {
                                $(this).click(function () {
                                    clickCity($(this).data("did"), $(this).data("dname"));
                                });
                            });
                        }
                    });
                }
                break;
            }
            //机加酒
            case "12": {

                //加载筛选条件与排序选项
                var filterStartCityData = null;
                var filterStopCityData = null;
                var loadFilterInfo = function () {

                    var _paramDic = { albumId: albumId};
                    $.get(_Config.APIUrl + '/api/hotel/GetAlbumFilter', _paramDic, function (_data) {

                        _data["year"] = 2018;
                        _data["month"] = 1;
                        _data["startdid"] = startdid;
                        _data["stopdid"] = stopdid;

                        //start vue
                        if (filterStartCityData) {
                            filterStartCityData.cityData = _data;
                        }
                        else {
                            filterStartCityData = new Vue({
                                el: '#startcity-selector',
                                data: { "cityData": _data }
                            })
                        }

                        //stop vue
                        if (filterStopCityData) {
                            filterStopCityData.cityData = _data;
                        }
                        else {
                            filterStopCityData = new Vue({
                                el: '#stopcity-selector',
                                data: { "cityData": _data }
                            })
                        }

                        Vue.nextTick(function () {

                            //城市选择器的选择
                            $("#startcity-selector ._item").each(function () {

                                $(this).click(function () {

                                    var _did = $(this).data("did");// alert(_did);
                                    var _dname = $(this).data("dname");
                                    startdid = _did;

                                    loadMorePackages(true);

                                    //change start city filter
                                    $(".filter-section .start-city").html(_dname);
                                    $(".filter-section .start-city").addClass("sel");

                                    filterStartCityData.cityData["startdid"] = parseInt(startdid);

                                    hideSelector();
                                });
                            });

                            //城市选择器的清空
                            $("#startcity-selector ._clear").click(function () {

                                startdid = 0;
                                loadMorePackages(true);

                                //change start city filter
                                $(".filter-section .start-city").html("出发地");
                                $(".filter-section .start-city").removeClass("sel");

                                filterStartCityData.cityData["startdid"] = startdid;

                                hideSelector();
                            });

                            $("#stopcity-selector ._item").each(function () {

                                $(this).click(function () {

                                    var _did = $(this).data("did");
                                    var _dname = $(this).data("dname");
                                    stopdid = parseInt(_did);
                                    loadMorePackages(true);

                                    //change stop filter
                                    $(".filter-section .stop-city").html(_dname);
                                    $(".filter-section .stop-city").addClass("sel");

                                    filterStopCityData.cityData["stopdid"] = stopdid;

                                    hideSelector();
                                });
                            });

                            //排序选择器的清空
                            $("#stopcity-selector ._clear").click(function () {

                                stopdid = 0;
                                loadMorePackages(true);

                                //change stop filter
                                $(".filter-section .stop-city").html("目的地");
                                $(".filter-section .stop-city").removeClass("sel");

                                filterStopCityData.cityData["stopdid"] = stopdid;

                                hideSelector();
                            });
                        });
                    });
                }
                loadFilterInfo();

                //选择器的背景点击事件（关闭所有的筛选/排序）
                var hideSelector = function () {

                    $("._selector-model").hide();
                    $("._selector").slideUp(100);
                }
                $("._selector-model").click(hideSelector);

                //选择器的关闭事件
                $("._selector ._close").click(hideSelector);

                //打开出发地
                var filtersStartCityShow = function () {

                    $("._selector-model").show();
                    $("#startcity-selector").slideDown(200);
                }
                $(".filter-section .start-city").click(filtersStartCityShow);

                //打开目的地
                var filterStopCityShow = function () {

                    $("._selector-model").show();
                    $("#stopcity-selector").slideDown(200);
                }
                $(".filter-section .stop-city").click(filterStopCityShow);
                break;
            }

        }

    }
});

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
    var isApp = $("#isApp").val();
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (isApp == "0") {
        url = "http://www.zmjiudian.com/" + param;
    }

    this.location = url;
}

function gotopage(param) {
    var isApp = $("#isApp").val();
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    if (isApp == "0") {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

var _showCity = false;
var showCity = function () {
    if (!_showCity) {
        showCityFun();
    }
    else {
        hideCityFun();
    }
}

$(".city-section-bg").click(function () {
    hideCityFun();
});

var _lastScrollTop = 0;
var showCityFun = function () {

    $(".city-section-bg").show();
    //$(".city-section").css("top", "500%");
    $(".city-section").show();
    //$(".city-section").animate({ top: '25%' }, 300);
    //$(".city-section").fadeIn(300);

    _lastScrollTop = $(document).scrollTop()
    $("html,body").animate({ scrollTop: 0 }, 0);

    $(".more-packages").hide();
    _showCity = true;
}

var hideCityFun = function () {
    $("html,body").animate({ scrollTop: _lastScrollTop }, 0);
    $(".city-section-bg").hide();
    $(".city-section").hide();
    //$(".city-section").animate({ top: '500%' }, 200);
    //setTimeout(function () { $(".city-section").hide(); }, 250);
    //$(".city-section").fadeOut(200);
    $(".more-packages").show();
    _showCity = false;
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

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