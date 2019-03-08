
var _Config = new Config();
var userid = $("#userid").val();
var categoryId = $("#categoryId").val();
//var albumId = $("#albumId").val();
var isload = true;
var isApp = $("#isApp").val();


//_Config.APIUrl = "http://api.dev.jiudian.corp";
//_Config.APIUrl = "http://api.zmjd100.com";

var loadFunction = function () {


    var $win = $(window);
    var winTop = $win.scrollTop();
    var winWidth = $win.width();
    var winHeight = $win.height();
    var tabsHeight = $("#tabs").height();

    var tabsActiveTop = 0;


    $win.on('scroll', function () {
        var tagTop = $(".more-packages-foot").offset().top;
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        if (winTop >= tagTop - winHeight - 100) {
            loadMorePackageList(tabDetailList.albumId, false, false);
        }
    });


    var tabDetailList = {};

    var _tabDetailItem = {
        categoryId: categoryId,
        albumId: 0,
        name: "",
        start: 0,
        count: 10,
        districtID: $("#districtID").val(),
        skuList: [],
        AdData: {},
        categoryList: []
    }
    switch (parseInt(categoryId)) {
        case 14: { _tabDetailItem.name = "美食精选"; _tabDetailItem.albumId = 2; break; }
        case 20: { _tabDetailItem.name = "玩乐精选"; _tabDetailItem.albumId = 3; break; }
    }

    tabDetailList = _tabDetailItem;

    var tabDataInfo = new Vue({
        el: "#tabinfo",
        data: {
            "tabDetailList": tabDetailList
        }
    })


    var loadMorePackageList = function (id, isfirst, goScrollTop) {

        if (isfirst) {
            isload = true;
        }
        if (isload) {

            isload = false;
            //默认显示loadding
            $(".scrollpageloading").show();

            //第一次绑定完后，做一些初始操作
            if (isfirst) {
                $(".scrollpageloading").html('<img class="img-first" src="http://whfront.b0.upaiyun.com/app/img/loading-changes.gif" alt="" />');
            }
            else {
                $(".scrollpageloading").find("img").removeClass("img-first");
            }
            loadDataList(id);

        }
    }

    var loadDataList = function (id, isfirst) {
        if (isfirst)
        {
            tabDataInfo.tabDetailList.skuList = [];
        }
        var _listDic = { albumid: id, count: tabDetailList.count, start: tabDetailList.start, userid: userid, districtID: tabDetailList.districtID };
        $.get(_Config.APIUrl + '/api/coupon/GetProductAlbumSKUCouponActivityListByAlbumID', _listDic, function (data) {
            loadProductListCallback(data);
        });
        //加载指定专辑/分类的产品list
        var loadProductListCallback = function (data) {
            if (data) {

                if (data.SKUCouponList && data.SKUCouponList.length) {

                    //第一次绑定完后，做一些初始操作
                    if (data.SKUCouponList.length >= tabDetailList.count) {
                        if (isfirst) {
                            $(".scrollpageloading").show();
                        }
                        else {
                            $(".scrollpageloading").hide();
                        }
                    }
                    else {
                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (!isfirst) {
                            $(".scrollpageloading").hide();
                        }
                        else {
                            $(".scrollpageloading").hide();
                        }
                    }
                    $(".scrollpageloading").hide();

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
                        if (isApp == "1") {
                            _litem.url = "/coupon/product/" + _litem.SKUID + "?userid={userid}&_newpage=1";
                        }
                        else {
                            _litem.url = "/coupon/product/" + _litem.SKUID + "?userid=" + userid + "&_newpage=1&_newtitle=1";
                        }

                        for (var _tnum = 0; _tnum < tabDataInfo.tabDetailList.length; _tnum++) {
                            var _tabDetailItem = tabDataInfo.tabDetailList;
                            _tabDetailItem.skuList.push(_litem);
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


                        tabDataInfo.tabDetailList.skuList.push(_litem);

                    }

                    //更新页码
                    var _tabDetailItem = tabDataInfo.tabDetailList;
                    _tabDetailItem.start = (tabDetailList.start + tabDetailList.count)
                    //start += count;
                    console.log(tabDataInfo.tabDetailList);
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
                        $(".scrollpageloading").hide();
                    }
                }
            }
            else {
                //如果是第一页，则不显示“没有更多了”，不然很奇怪
                if (!isfirst) {
                    $(".scrollpageloading").hide();
                }
            }
        }
    }


    loadMorePackageList(tabDetailList.albumId, true, false);
    //遍历所有待加载图片，并动态加载
    var loadImgsEvent = function () {
        $(".load-img").each(function () {
            var _load = $(this).data("load");
            if (_load === 0) {
                setImgOriSrc($(this));
            }
        })
    }

    //加载子分类
    var loadProductCategory = function (_parentcatergoryid) {
        $.get(_Config.APIUrl + "/api/Product/GetProductCategoryList", { typeID: 600, dicKey: 1, districtID: tabDetailList.districtID }, function (data) {
            console.log(data);
            tabDataInfo.tabDetailList.categoryList = [];
            for (var i = 0; i < data.length; i++)
            {
                var geoScopeType = 1;
                if (tabDetailList.districtID == 0) {
                    geoScopeType = 3;
                }
                var _litem = data[i];
                _litem["ActionUrl"] = "/Coupon/MoreList/1/0/0?_newpage=1&category=" + _litem.ID + "&ischild=true&districtId=" + tabDetailList.districtID + "&districtName=" + $("#districtName").val() + "&geoScopeType=" + geoScopeType;
                if (_litem.ICON == null || _litem.ICON == "") {
                    console.log(_litem.ICON);
                    _litem.ICON = "http://whfront.b0.upaiyun.com/app/img/me/icon-play.png";
                };
                tabDataInfo.tabDetailList.categoryList.push(_litem);
            }
            //if (data.length < 4) {
            //    Vue.nextTick(function () {
            //        var _itemWidth = 100 / data.length
            //        $(".catogryitem").each(function () {
            //            $(this).css("width", _itemWidth + "%")
            //        });
            //    })
            //}
        })
    }
    loadProductCategory();

    //加载广告banner
    var loadTopAdList = function (_tabId) {
        if ($("#banner-section")) {
            //7美食头banner  8玩乐头banner
            var _adTypeId = 0;
            switch (parseInt(_tabId)) {
                case 14: { _adTypeId = 7; break; }
                case 20: { _adTypeId = 8; break; }
            }
            if (_adTypeId) {
                var _paramData = { "type": _adTypeId, "curuserid": userid, districtId: tabDetailList.districtID };
                $.get(_Config.APIUrl + "/api/hotel/GetHomeOnlineBannersByTypeAndDistrictId", _paramData, function (_data) {
                    tabDataInfo.tabDetailList.AdData = _data;
                    Vue.nextTick(function () {
                        $("#banner-section .home-hlist-panel").swiper({
                            slidesPerView: 'auto',
                            speed: 200,
                            //offsetPxBefore: _HomeSliderLiMarLeft,
                            //offsetPxAfter: _HomeSliderLiMarLeft,
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
    loadTopAdList(categoryId);

    //加载筛选条件与排序选项
    var filterCityData = null;
    //var sortData = null;
    var loadFilterAndSortInfo = function () {

        var _paramDic = { albumId: tabDetailList.albumId };
        $.get(_Config.APIUrl + '/api/coupon/GetDistinctSPUDistrictIDByAlbum', _paramDic, function (_data) {

            console.log(_data);
            _data["districtId"] = parseInt(tabDetailList.districtID);
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
            Vue.nextTick(function () {
                $(".filter-section .city").html($("#districtName").val());

                console.log(filterCityData.cityData);

                //城市选择器的选择
                $("#city-selector ._item").each(function () {

                    $(this).click(function () {

                        var _did = $(this).data("did");// alert(_did);
                        var _dname = $(this).data("dname");
                        districtId = _did;
                        tabDetailList.start = 0;
                        tabDetailList.districtID = _did;
                        filterCityData.cityData.districtId = _did;

                        $("#districtID").val(_did);

                        loadDataList(tabDetailList.albumId, true)

                        //加载广告banner
                        loadTopAdList(categoryId);

                        //加载子分类
                        loadProductCategory();

                        //change city filter
                        $(".filter-section .city").html(_dname);
                        $("#districtName").val(_dname);

                        hideSelector();
                    });
                });

                //城市选择器的清空
                $("#city-selector ._clear").click(function () {

                    tabDetailList.start = 0;
                    tabDetailList.districtID = 0;
                    filterCityData.cityData.districtId = 0;
                    $("#districtID").val(0);
                    $("#districtName").val("全部城市");

                    loadDataList(tabDetailList.albumId, true);

                    //加载广告banner
                    loadTopAdList(categoryId);

                    //加载子分类
                    loadProductCategory();

                    //change city filter
                    $(".filter-section .city").html("全部城市");

                    //filterCityData.cityData["districtId"] = parseInt(districtId);

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


    //打开全部城市
    var filterCityShow = function () {

        $("._selector-model").show();
        $("#city-selector").slideDown(200);
    }
    $(".filter-section .city").click(filterCityShow);
}
$(function () {
    loadFunction();
})


//function goUrl(url) {
//    location.href = url;
//}

//function goproducturl(url) {
//    location.href = url;
//}