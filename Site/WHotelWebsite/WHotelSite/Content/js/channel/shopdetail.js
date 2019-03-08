var isMobile = $("#isMobile").val() == "1";
var cid = parseInt($("#cid").val());
var _Config = new Config();
var pageSize = 10;
var start = 0;
var screenScroll;

var _cityFilterLastOption = null;
var _cityFilterLastSubitems = null;

//上次选择目的地id和名称
var searchDistrictIdKey = "searchDistrictId";
var searchDistrictNameKey = "searchDistrictName";
var winWidth = window.screen.width;

$(function () {

    //搜索筛选参数
    var _searchDic = {

        Cid: cid,
        Start: 0,
        Count: 10,
        SearchWord: "",
        Sort: 0,
        Type: 0,
        FilterName: "筛选",
        Screen: {
            ProductType: []
        },
        TotalCount: 0,
        DistrictId: 0,
        DistrictName: "城市" 
    };

    var productListData = new Vue({
        el: "#productList",
        data: {
            "listEntity": {
                type: 0,
                list: []
            }
        }
    })

    //filter bar
    var _filterBar = $('.filter');
    var _filterSpace = $(".filter-space");
    var _filterBarOriTop = $('.filter').offset().top;

    //try {
    //    screenScroll = new IScroll('#wrapper', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
    //}
    //catch(e){}

    //获取上次筛选目的地信息
    var cacheDistrictId = Store.Get(searchDistrictIdKey);
    var searchDistrictName = Store.Get(searchDistrictNameKey);
    if (cacheDistrictId) {
        _searchDic.DistrictId = cacheDistrictId;
        _searchDic.DistrictName = searchDistrictName;
        $(".f_r .f-item").css("color", "#FE8000");
    }
    else {
        var _partnerurl = "/api/Partner/GetRetailerInvate";
        $.get(_Config.APIUrl + _partnerurl, { userID: cid }, function (data) {
            _searchDic.DistrictId = data.DistrictId || 0;
            //常驻城市没有id时，不显示城市
            if (_searchDic.DistrictId) {
                _searchDic.DistrictName = data.OftenCityName;
                //加载产品详情
                loadProductList(true);
            }
        })
    }

    var $win = $(window);
    var isload = true;

    $win.on('scroll', function () {
        
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        //筛选条的置顶处理
        var m_st = Math.max(document.body.scrollTop || document.documentElement.scrollTop);
        if (m_st < _filterBarOriTop) {
            _filterBar.removeClass("filter-fixed");
            _filterSpace.hide();
            //hideFilterBg();
        } else {
            if (_filterBar.attr("class").indexOf("fixed") < 0) {
                _filterBar.addClass("filter-fixed");
                _filterSpace.show();
            }
        }


        //上拉加载
        var tagTop = $(".more-packages-foot").offset().top;
        if (winTop >= tagTop - winHeight - 100) {
            //console.log(start);
            loadProductList(false);
        }
    });

    //_Config.APIUrl = "http://api.zmjd100.com";

    //加载产品详情
    var loadProductList = function (isfirst) {

        //hideFilterBg();

        if (isload) {

            isload = false;

            if (isfirst) {
                _searchDic.Start = 0;
                $(".scrollpageloading").html('<img src="http://whfront.b0.upaiyun.com/app/img/loading.gif" alt="" />');
            } 
            //_Config.APIUrl = "http://192.168.1.114:8000";
            //_Config.APIUrl = "http://api.zmjd100.com";
         
            var _apiUrl = "/api/coupon/GetSearchRetailerProductListByCategory";

            //酒店搜索
            if (_searchDic.Type == 1) {
                _apiUrl = "/api/hotel/GetRetailHotelList";
            }

            //console.log(_searchDic)

            $.post(_Config.APIUrl + _apiUrl, _searchDic, function (data) {
                

                if (data) {

                    if (data.list && data.list.length) {

                        $(".p-null").hide();

                        if (data.list.length >= _searchDic.Count) {
                            if (isfirst) {
                                productListData.listEntity.list = [];
                                $(".scrollpageloading").show();
                            }
                        }
                        else {
                            //如果是第一页，则不显示“没有更多了”，不然很奇怪
                            if (!isfirst) {
                                $(".scrollpageloading").html("<div>没有更多了</div>");
                            }
                            else {
                                productListData.listEntity.list = [];
                                $(".scrollpageloading").hide();
                            }
                        }

                        for (var _lnum = 0; _lnum < data.list.length; _lnum++) {
                            var _litem = data.list[_lnum];
                            productListData.listEntity.list.push(_litem);
                        }

                        if (isfirst) {
                            $("#productList").fadeIn();
                        }
                        _searchDic.Start += _searchDic.Count;
                        start += pageSize;

                    }
                    else {
                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (!isfirst) {
                            $(".scrollpageloading").html("<div>没有更多了</div>");
                        }
                        else {
                            productListData.listEntity.list = [];
                            $(".scrollpageloading").hide();
                            $(".p-null").show();
                        }
                    }
                }
                else {
                    //如果是第一页，则不显示“没有更多了”，不然很奇怪
                    if (!isfirst) {
                        $(".scrollpageloading").html("<div>没有更多了</div>");
                    }
                    else {
                        productListData.listEntity.list = [];
                        $(".scrollpageloading").hide();
                        $(".p-null").show();
                    }
                }

                isload = true;

            });
        }
    }

    //------筛选功能------
    var _sortBtn = $(".f-sort");
    var _filterBtn = $(".f-filter");
    var _filterBtnName = $(".f-filter .name");
    var _filterSort = $(".f-sort-options");
    var _filterOptions = $(".f-filter-options");
    var _filterConfirm = $(".f-filter-options .confirm");
    var _filterClear = $(".f-filter-options .clear");

    var filterSortData = new Vue({
        el: "#filter",
        data: {
            "filterData": {
                "SearScreen": {},
                "SearchSort": {}
            },
            "searchDic": _searchDic
        }
    })

    //【items】加载筛选排序项
    var loadSortFilterItems = function () {

        var _dic = {};
        $.get(_Config.APIUrl + '/api/coupon/GetSearchRetailScreenList', _dic, function (data) {
            
            //console.log(data)

            filterSortData.filterData.SearScreen = data;

            Vue.nextTick(function () {

                //$(".scrollLine .t-item").css("width", (winWidth / data.length) + "px");
                if (data.length > 3) { 
                try {
                    screenScroll = new IScroll('#wrapper', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
                } catch (e) {
                    }
                }


                //绑定筛选项的事件
                $(".t-item").each(function () {
                    $(this).click(filterItemClick);
                    //var _selItem = $(".scrollLine").find(".scroll-seled");
                    //var _selNum = parseInt(_selItem.data("num1"));
                    //if (_selNum > 1) {
                    //    //如果选择的项靠后，则自动滚动到指定区域
                    //    //screenScroll.scrollToElement('scrollLine:nth-child(' + (_selNum + 1) + ')', 500);
                    //    _themeListScroll.scrollToElement('.scroll-seled', 500);
                    //}
                });


                ////绑定筛选项的事件
                //$(".filter-item").each(function () {
                //    $(this).click(filterItemClick);
                //});

                //默认筛选项选择
                bindFilterConfirmInfo();

                //默认加载产品列表
                loadProductList(true);

            })

        });

    }
    loadSortFilterItems();

    //【sort items】加载排序项
    var loadSortItems = function () {

        //1酒店产品 2消费券产品
        var _dic = { type: _searchDic.Type, isRetailShop: true };
        $.get(_Config.APIUrl + '/api/coupon/GetSearchRetailSortList', _dic, function (data) {

            //console.log(data)

            filterSortData.filterData.SearchSort = data;

            Vue.nextTick(function () {

                //绑定排序项的事件
                $(".sort-item").each(function () {
                    $(this).click(sortItemClick);
                });

            })

        });
    }
    loadSortItems();


    //Vue.nextTick(function () {
    //    try {
    //        screenScroll = new IScroll('#wrapper', { eventPassthrough: true, scrollX: true, scrollY: false, preventDefault: false });
    //    } catch (e) {
    //    }

    //    var _selItem = $(".scrollLine").find(".scroll-seled");
    //    var _selNum = parseInt(_selItem.data("num"));
    //    if (_selNum > 1) {
    //        //如果选择的项靠后，则自动滚动到指定区域
    //        //_themeListScroll.scrollToElement('scrollLine:nth-child(' + (_selNum + 1) + ')', 500);
    //        _themeListScroll.scrollToElement('.scroll-seled', 500);
    //    }
    //})

    //【排序】打开排序模块
    _sortBtn.click(function () {

        var _op = $(this).data("op");
        if (_op === 0) {

            $(this).find("._icon").html("&#xe65b;");
            $(this).data("op", 1);

            _filterSort.slideDown(200);

            //$("html,body").animate({ scrollTop: $(".filter").offset().top - 0 }, 300);

            hideFilterOptions();

            setTimeout(function () { showFilterBg(); }, 0);
        }
        else {
            hideSortOptions();
        }
    });

    //隐藏排序模块
    var hideSortOptions = function () {

        _sortBtn.find("._icon").html("&#xe65c;");
        _sortBtn.data("op", 0);

        _filterSort.hide();
        hideFilterBg();
    }

    //点击排序的具体项
    var sortItemClick = function () {

        var _num = $(this).data("num");
        var _name = $(this).data("name");
        var _sel = $(this).data("sel");
        if (_sel === 0) {

            //处理选中样式
            $(".sort-item").each(function () {
                $(this).data("sel", 0);
                $(this).removeClass("_item-seled");
            });
            $(this).addClass("_item-seled");
            $(this).data("sel", 1);
            _name = _name.replace("（从低到高）", "").replace("（从高到低）", "");
            _sortBtn.find(".name").html(_name);
        }

        //设置最新的排序
        _searchDic.Sort = _num;

        //初始list
        _searchDic.Start = 0;
        productListData.listEntity.list = [];

        loadProductList(true);

        //隐藏排序
        hideSortOptions();
    }

    //【筛选】打开筛选模块
    _filterBtn.click(function () {

        var _op = $(this).data("op");
        if (_op === 0) {

            $(this).addClass("f-filter-seled");
            $(this).data("op", 1);

            _filterOptions.slideDown(200);

            //$("html,body").animate({ scrollTop: $(".filter").offset().top - 0 }, 200);

            hideSortOptions();

            setTimeout(function () { showFilterBg(); }, 0);
        }
        else {
            hideFilterOptions();
        }
    });

    //隐藏筛选模块
    var hideFilterOptions = function () {

        _filterBtn.data("op", 0);

        _filterOptions.hide();
        hideFilterBg();
        //_filterBg.hide();

        //清除未确定筛选的项目（有些项目选中了，但是没有点 确定 按钮）
        $(".filter-item").each(function () {

            var _num = parseInt($(this).data("num"));
            
            if (_searchDic.Screen.ProductType.length) {
                var _have = false;
                for (var i = 0; i < _searchDic.Screen.ProductType.length; i++) {
                    var _objVal = _searchDic.Screen.ProductType[i];
                    if (_num === _objVal) {
                        _have = true; break;
                    }
                }
                
                if (!_have) {
                    $(this).removeClass("item-seled");
                    $(this).data("sel", 0);
                }
            }
            else {
                $(this).removeClass("item-seled");
                $(this).data("sel", 0);
            }

        });
    }

    //点击筛选的具体项
    var filterItemClick = function () {

        var _num = $(this).data("num");
        var _name = $(this).data("name");
        var _sel = $(this).data("sel");
        if (_sel === 0) {

            //单选
            $(".t-item").each(function () {
                $(this.children).removeClass("scroll-seled");
                $(this).data("sel", 0);
            });
            $(this.children).addClass("scroll-seled");
            $(this).data("sel", 1);

            ////选择新的筛选，排序 默认排序
            //_sortBtn.find(".name").html("默认排序");
            ////设置最新的排序
            //_searchDic.Sort = 0;

            ////单选
            //$(".filter-item").each(function () {
            //    $(this).removeClass("item-seled");
            //    $(this).data("sel", 0);
            //});

            //$(this).addClass("item-seled");
            //$(this).data("sel", 1);
        }
        //else {

        //    $(this).removeClass("item-seled");
        //    $(this).data("sel", 0);
        //}


        //绑定筛选项
        bindFilterConfirmInfo();

        //刷新产品列表
        loadProductList(true);

        //刷新排序项
        loadSortItems();

        //隐藏筛选
        hideFilterOptions();
    }

    //筛选模块的确定事件
    var bindFilterConfirmInfo = function () {

        //初始list
        _searchDic.Start = 0;
        _searchDic.Screen.ProductType = [];
        productListData.listEntity.list = [];

        //统计当前所有筛选项选择情况
        $(".t-item").each(function () {

            var _name = $(this).data("name");
            var _num = $(this).data("num"); //_num = 600;
            var _type = $(this).data("t");
            var _sel = $(this).data("sel");
            if (_sel === 1) {
                _searchDic.Type = _type;
                _searchDic.FilterName = _name;
                _searchDic.Screen.ProductType.push(_num);

                productListData.listEntity.type = _type;
            }
        });

        ////统计当前所有筛选项选择情况
        //$(".filter-item").each(function () {

        //    var _name = $(this).data("name");
        //    var _num = $(this).data("num"); //_num = 600;
        //    var _type = $(this).data("t");
        //    var _sel = $(this).data("sel");
        //    if (_sel === 1) {
        //        _searchDic.Type = _type;
        //        _searchDic.FilterName = _name;
        //        _searchDic.Screen.ProductType.push(_num);

        //        productListData.listEntity.type = _type;
        //    }
        //});

        //筛选默认必选一项
        _filterBtn.addClass("f-filter-seled");
        _filterBtnName.text(_searchDic.FilterName);
    }
    _filterConfirm.click(function () {

        //绑定筛选项
        bindFilterConfirmInfo();

        //刷新产品列表
        loadProductList(true);

        //刷新排序项
        loadSortItems();

        //隐藏筛选
        hideFilterOptions();
    });

    //筛选模块的清除事件
    _filterClear.click(function () {

        //初始list
        _searchDic.Start = 0;
        _searchDic.Screen.ProductType = [];
        productListData.listEntity.list = [];

        //清除当前所有筛选项选中样式
        $(".filter-item").each(function () {

            $(this).removeClass("item-seled");
            $(this).data("sel", 0);
        });

        loadProductList(true);

        //隐藏筛选
        hideFilterOptions();
    });

    //------搜索功能相关------
    var _shopHead = $(".shop-head");

    var _filter = $(".filter");
    var _searchInp = $("#searchText");
    var _searchGlass = $("#searchGlass");
    var _searchInpClear = $(".s-inp-clear");

    var _filterSearch = $(".filter-search");
    var _searchInp2 = $("#searchText2");
    var _searchBtn = $(".search-btn");
    var _filterBg = $(".filter-bg");

    _searchGlass.click(function () {
        location.href = "http://bg.zmjiudian.com/Channel/SearchProductList?isShop=true&CID=" + cid;
        //showFilterSearch();
    });

    //搜索筛选的遮罩层点击（隐藏所有筛选项）
    _filterBg.click(function () {

        hideFilterSearch();

    });

    //显示搜索筛选的遮罩层
    var showFilterBg = function () {

        //隐藏头
        _shopHead.hide();
        //_shopHead.slideUp(100);

        _filterBg.show();
        //_filterBg.fadeIn(200);

        //如果筛选模块bar没有悬浮，则需要滑动到顶部（不然会因为shopHead的隐藏，导致筛选模块bar跑到屏幕外面去）
        if (_filterBar.attr("class").indexOf("fixed") < 0) {
            $("html,body").animate({ scrollTop: 0 }, 200);
        }
    }

    //隐藏搜索筛选的遮罩层
    var hideFilterBg = function () {

        //显示头
        _shopHead.show();

        _filterBg.hide();
    }

    //点击“搜索”事件
    _searchBtn.click(function () {

        //设置搜索关键字
        var _keyword = $("#searchText2").val();
        _searchDic.SearchWord = _keyword;

        //将关键字设置到外部搜索框
        $("#searchText").val(_keyword);

        //初始list
        _searchDic.Start = 0;
        productListData.listEntity.list = [];

        //重新搜索
        loadProductList(true);

        hideFilterSearch();
    });

    //搜索清空事件
    _searchInpClear.click(function () {

        $("#searchText").val("");
        $("#searchText2").val("");

        //初始list
        _searchDic.SearchWord = "";
        _searchDic.Start = 0;
        productListData.listEntity.list = [];

        //重新搜索
        loadProductList(true);

        _searchInpClear.hide();

        //hideFilterSearch();
    });

    //显示搜索模块
    var showFilterSearch = function () {

        //隐藏头
        _shopHead.hide();
        //_filter.hide();

        //显示搜索模块
        _filterSearch.show();
        showFilterBg();

        //搜索框自动获焦
        _searchInp2[0].focus();

        //点击搜索1，要隐藏排序和筛选模块
        _filterSort.hide();
        _filterOptions.hide();
    }

    //隐藏搜索筛选模块
    var hideFilterSearch = function () {

        //显示头
        _shopHead.show();
        //_filter.show();

        //显示搜索模块
        _filterSearch.hide();
        _filterBg.hide();

        //点击搜索2，要隐藏排序和筛选模块
        hideSortOptions();
        hideFilterOptions();

        //判断是否需要显示 清除搜索记录 的按钮
        if (_searchInp.val()) {
            _searchInpClear.show();
        }
        else {
            _searchInpClear.hide();
        }
    }




    //加载城市筛选
    var filterCityData = new Vue({
        el: '#city-selector',
        data: {
            "hotCityData": null,
            "cityData": {}
        }
    })
    var loadFilterInfo = function () {

        //var _paramDic = { albumid: albumIds };
        var urlSearch = "/api/coupon/GetRetailGroupDistinct";

        $.get(_Config.APIUrl + urlSearch, "", function (_data) {

            var _cityData = {};

            _cityData["districtId"] = parseInt(_searchDic.DistrictId);
            //_cityData["productSort"] = productSort;
            _cityData["DestinationList"] = _data;

            //城市vue
            filterCityData.cityData = _cityData;

            Vue.nextTick(function () {


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

                                        //hide all subitems
                                        if (_cityFilterLastSubitems) {
                                            _cityFilterLastSubitems.hide();
                                            _cityFilterLastSubitems.data("open", 0);
                                        }

                                        //当前省的右箭头下箭头切换
                                        _thisOption.find("._right_icon").hide();
                                        _thisOption.find("._up_icon").show();

                                        //open mine
                                        _subItemsObj.slideDown(200);
                                        _subItemsObj.data("open", 1);

                                        setTimeout(function () {

                                            var _pageScrollTop = $win.scrollTop();
                                            var _thisOptionTop = _thisOption.offset().top - ($win.height() - $("#city-selector ._list").height());
                                            //console.log(_thisOptionTop)

                                            var _thisListScrollTop = $("#city-selector ._list").scrollTop();
                                            if (_thisOptionTop < 0) {

                                                $("#city-selector ._list").animate({ scrollTop: 0 }, 200);
                                            }
                                            else {

                                                //console.log(_thisOptionTop + _thisListScrollTop)
                                                $("#city-selector ._list").animate({ scrollTop: _thisOptionTop + _thisListScrollTop + 5 - _pageScrollTop }, 200);
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

                    //初始list
                    _searchDic.Start = 0;

                    filterCityData.cityData.districtId = 0;
                    _searchDic.DistrictId = 0;
                    _searchDic.DistrictName = "城市";
                    $(".f_r .f-item").css("color", "");
                    Store.Set(searchDistrictIdKey, 0);
                    Store.Set(searchDistrictNameKey, "城市");
                    loadProductList(true);

                    hideSelector();


                    $("#city-selector ._list").animate({ scrollTop: 0 }, 0);
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
    loadHotFilterInfo();

    //城市点击事件绑定
    var bindCityEvent = function () {

        //城市选择器的选择
        var _clickItemList = $("#city-selector .sub-item-col");
        _clickItemList.each(function () {

            $(this).unbind("click");
            $(this).click(function () {
                //初始list
                _searchDic.Start = 0;

                var _did = $(this).data("did");
                var _dname = $(this).data("dname");
                _searchDic.DistrictId = _did;
                _searchDic.DistrictName = _dname;
                //change city filter
                filterCityData.cityData.districtId = _did;
                Store.Set(searchDistrictIdKey, _did);
                Store.Set(searchDistrictNameKey, _dname);
                $(".f_r .f-item").css("color", "#FE8000");
                loadProductList(true);
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
    $(".f_r .f-city").click(filterCityShow);


    //筛选爆款产品
    var hotProduct = function () {
        if (_searchDic.IsHot == 1) {
            $(".f-hot .name").css("color", "#2c2c2c");
            _searchDic.IsHot = false;
        }
        else {
            $(".f-hot .name").css("color", "#FE8000");
            _searchDic.IsHot = true;

        }
        loadProductList(true);
    }

    $(".f-hot").click(hotProduct);

});