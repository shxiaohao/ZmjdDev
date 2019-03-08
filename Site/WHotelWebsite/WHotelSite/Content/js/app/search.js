var IsApp = $("#_isApp").val();
var AppType = $("#_appType").val();
var UserId = parseInt($("#_userid").val());

$(function () {

    var _Config = new Config();

    //搜索事件
    var searchResultData = {};
    searchResultData = new Vue({
        el: '#search-section',
        data: { "searchResult": searchResultData }
    })

    var searchInpTimer = null;
    var cpLock = false;
    var sInpObj = $("._input_search");
    var sInp = sInpObj[0];
    var searchEvent = function () {
        if (!cpLock) {
            if (sInpObj.val() == "") {
                clearTimeout(searchInpTimer);
                searchResultData.searchResult = {};
                $("#search-section").hide();
                $("#search-section-null").hide();
            }

            searchInpTimer = setTimeout(function () {
                var keyword = sInpObj.val();
                if (keyword) {

                    $(".clear-inp").show();

                    var _searchDic = { citycount: 3, hotelcount: 3, playcount: 3, foodcount: 3, keyword: keyword, needHighlight: true };
                    //$.get(_Config.APIUrl + '/api/hotel/SuggestCityAndHotel', _searchDic, function (data) {
                    $.get(_Config.APIUrl + '/api/search/search', _searchDic, function (data) {

                        console.log(data);

                        if (data && data.length > 0) {

                            //for (var i = 0; i < data.length; i++) {
                            //    var _d = data[i];
                            //    _d.ShowName = _d.Name.replace(keyword, "<span>{0}</span>".format(keyword))
                            //}

                            searchResultData.searchResult = data;
                            $("#search-section-null").hide();
                            $("#search-section").show();
                        }
                        else {
                            $("#search-section").hide();
                            $("#search-section-null").show();
                        }

                    });
                }
                else {
                    $(".clear-inp").hide();
                }
            },
            300);
        }
    }

    //2(中文触发)
    sInp.addEventListener('compositionstart', function () { cpLock = true; })

    //4(中文触发)
    sInp.addEventListener('compositionend', function () {
        cpLock = false;
        clearTimeout(searchInpTimer);
        searchEvent();
    })

    //3
    sInp.addEventListener('input', function () {
        clearTimeout(searchInpTimer);
        searchEvent();
    });

    //sInp.onkeyup = function () {
    //    searchEvent();
    //};

    //1
    sInp.onkeydown = function () {
        clearTimeout(searchInpTimer);
    }

    //clear search input
    $(".clear-inp").click(function () {
        sInpObj.val("");
        searchEvent();
        sInpObj.focus();
        $(".clear-inp").hide();
    });

    //获取最近浏览记录
    loadDistrictHistory();
});

//跳转到酒店列表
var openHotelList = function (thisObj) {
    var _this = $(thisObj);
    var _did = _this.data("did");
    var _dname = _this.data("dname");
    var _gtype = _this.data("gtype");
    var _url = "http://www.zmjiudian.com/city{0}".format(_did);
    if (_gtype == 3 || _gtype == "3" || _dname.indexOf("及周边") >= 0) {
        _gtype = "3";
        _url = "http://www.zmjiudian.com/cityaround{0}".format(_did);
    }
    else {
        //console.log(_did)
        //console.log(_dname)
        //console.log(_gtype)

        //记录目的地搜索记录
        recDistrictHistory(_did, _dname, _gtype);

        //刷新最近浏览
        loadDistrictHistory();
    }

    //console.log(_url)
    gourl(_url);
}

//跳转到酒店详情
var openHotelDetail = function (thisObj) {
    var _this = $(thisObj);
    var _hid = _this.data("hid");
    var _url = "http://www.zmjiudian.com/hotel/{0}?userid={1}".format(_hid, UserId);
    //console.log(_url)
    gourl(_url);
}

//跳转到消费券详情
var openProductDetail = function (thisObj) {
    var _this = $(thisObj);
    var _pid = _this.data("pid");
    var _url = "http://www.zmjiudian.com/coupon/product/{0}?userid={1}".format(_pid, UserId);
    //console.log(_url)
    gourl(_url);
}

//加载目的地搜索历史记录
var districtHistoryData = {};
districtHistoryData = new Vue({
    el: '#search-history',
    data: { "hisDistrict": {} }
})
var loadDistrictHistory = function () {
    var _newList = [];

    var _hisDistrict = getDistrictHistory();
    if (_hisDistrict && _hisDistrict.length > 0) {
        for (var i = (_hisDistrict.length - 1) ; i >= 0; i--) {
            if (_newList.length == 4) break;
            _newList.push(_hisDistrict[i]);
        }
        districtHistoryData.hisDistrict = _newList;
        $("#search-history").show();

        //that.setData({ hisDistrict: _newList });
    }
}

/* 记录搜索历史 */
var disHisKey = "zmjd_districtHistory";
var recDistrictHistory = function (did, dname, gtype) {
    var _delIndex = -1;
    var _have = false;
    var _history = Store.Get(disHisKey);
    if (_history && _history.length > 0) {
        for (var i = 0; i < _history.length; i++) {
            if (_history[i].DistrictId == did) {
                _delIndex = i;
                _have = true;
                break;
            }
        }
    }
    else {
        _history = [];
    }
    //if(_have) return;
    if (_delIndex >= 0) { removeIndex(_history, _delIndex); console.log('删除索引' + _delIndex); }

    _history.push({ DistrictId: did, DistrictName: dname, geoScopeType: gtype });
    Store.Set(disHisKey, _history)
}
var getDistrictHistory = function () {
    var _history = Store.Get(disHisKey);
    return _history;
}

var gourl = function (url) {
    location.href = url;
}



//cancel
var checkCancel = function () {
    
    if ($(".cityquicklist-bg").data("sw") == "1") {
        hideCityListElem();
        return false;
    }
    return true;
};

var hideCityListElem = function () {
    if (isApp == "1") {
        $(".city-topbar").removeClass("s-fixed");
        $(".s-inp").removeClass("s-fixed2");
    }
    else {
        $(".s-inp").removeClass("s-fixed");
    }

    $('#c-s-inp').val("");
    $(".cityquicklist").hide();
    $('.cityquicklist').html("");
    $(".cityquicklist-bg").fadeOut();
    $(".cityquicklist-bg").data("sw", "0");
};

var showCityListElem = function (html) {
    $('.cityquicklist').show();
    $('.cityquicklist').html(html);
    $(".cityquicklist-bg").data("sw", "1");
};

var initCitySearchHistory = function () {
    var historyVal = getCitySearchHistory();
    if (historyVal != "null" && historyVal != "") {
        var dic = {};
        dic["history"] = historyVal;
        $.get('/App/GenCitySearchHistory', dic, function (result) {
            try {
                if (result && result.length > 0) {
                    var html = "<ul>";
                    for (var i = 0; i < result.length; i++) {
                        var item = result[i];
                        html += "<li onclick=\"districtClick('" + item.url + "', " + item.id + ", '" + item.name + "')\">" + item.name + "</li>";
                    }
                    html += "<ul>";
                    $(".s-history .s-list").html(html);
                    $(".s-history").show();
                }
            } catch (e) {

            }
        });
    }
};
var setCitySearchHistory = function (id, name) {
    var historyVal = getCitySearchHistory();
    if (historyVal != "null") {
        var item = (id + "," + name); if (historyVal != "") item += ";";
        historyVal = item + historyVal;

        localStorage.CitySearchHistory = historyVal;
    }
};
var getCitySearchHistory = function () {
    if (window.localStorage) {
        if (localStorage.CitySearchHistory == undefined) localStorage.CitySearchHistory = "";
        return localStorage.CitySearchHistory;
    }
    return "null";
}
initCitySearchHistory();