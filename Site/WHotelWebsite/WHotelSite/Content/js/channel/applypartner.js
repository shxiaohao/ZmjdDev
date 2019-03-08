var isMobile = $("#isMobile").val() == "1";
var isInWeixin = $("#isInWeixin").val() == "1";
var isApp = $("#isApp").val() == "1";
var userid = parseInt($("#userId").val());
var phone = $("#phone").val();
var cid = parseInt($("#cid").val());
var _Config = new Config();

var partnerData = {};


$(function () {

    partnerData = new Vue({
        el: "#partner-info",
        data: {
            "partnerData": {
                "UserID": userid,
                "PhoneNum": phone,
                "RefereesUserId": cid,
                "Name": "",
                "OftenCityName": "点击选择",
                "DistrictId": 0
            }
        }
    })


    //提交申请
    $(".sub-apply").click(function () {

        if (!partnerData.partnerData.Name) {

            _Modal.show({
                title: '',
                content: '请填写你的真实姓名',
                confirmText: '好的',
                confirm: function () {
                    _Modal.hide();
                },
                showCancel: false
            });
            return;
        }

        if (!partnerData.partnerData.OftenCityName || partnerData.partnerData.OftenCityName =="点击选择") {

            _Modal.show({
                title: '',
                content: '请填写你的常住城市',
                confirmText: '好的',
                confirm: function () {
                    _Modal.hide();
                },
                showCancel: false
            });
            return;
        }
        console.log(partnerData.partnerData);

        var _addInfo = partnerData.partnerData;
        $.post(_Config.APIUrl + "/api/Partner/AddRetailerInvateinfo", _addInfo, function (_data) {
            
            console.log(_data)

            if (_data) {
                document.title = "提交成功";
                $("#partner-info").hide();
                $(".sub-complate").show();
            }
            else {
                _Modal.show({
                    title: '',
                    content: '提交失败，请重试',
                    confirmText: '好的',
                    confirm: function () {
                        _Modal.hide();
                    },
                    showCancel: false
                });
                return;
            }
        });

    });

    //复制微信号
    if (isApp) {
        $(".copy-wxno-link").click(function () {

            var _wxno = $(this).data("wxno");
            zmjd.copyTxt(_wxno);
        });
    }

    hotCities = new Vue({
        el: "#hotcities",
        data: {
            hotCityData: null
        },
        methods: {
            openSearch: function () {
                console.log(1);
                $(".selector_cover").hide();
                $(".selector_city").hide();
                $("#partner-info").hide();
                $(".searchbody").show();
                $(".input_city_search").focus();
            }
        }
    })
    $(".selector_cover").click(function () {
        $(".selector_cover").hide();
        $(".selector_city").hide();
    })

    searchCities = new Vue({
        el: "#searchCities",
        data: {
            searchCitiesData: null
        }
    })

    $(".select_city").click(function () {
        $(".selector_cover").show();
        $(".selector_city").show();
    })
    //var i = 0;
    //$(".input_city_search").bind("input propertychange", function (event) {
    //    i += 1;
    //    console.log(i)
    //});
    var flag = true;
    $(".input_city_search").on('compositionstart', function () {
        flag = false;
    })
    $(".input_city_search").on('compositionend', function () {
        flag = true;
    })
    $(".input_city_search").on('input', function () {
        setTimeout(function () {
            if (flag) {
                var k = $(".input_city_search").val();
                $.ajax({
                    url: _Config.APIUrl + "/api/Search/SearchCity",
                    data: { cityCount: 10, keyword: k, onlyInChina: true, needHighlight: false },
                    type: "GET",
                    success: function (_result) {
                        console.log(_result);
                        searchCities.searchCitiesData = _result
                        Vue.nextTick(function () {
                            bindCick();
                        })
                    }
                })
                //searchCities(k);
            }
        }, 0)
    })
    loadHotFilterInfo();

    $(".cancel").click(function () {
        $(".searchbody").hide();
        $(".selector_cover").hide();
        $(".selector_city").hide();
        $("#partner-info").show();
    })
});
//加载常用城市
var loadHotFilterInfo = function () {
    //4015 正式地址的id
    $.get(_Config.APIUrl + "/api/HotelTheme/GetTempSource", { "id": 4015 }, function (_hotdata) {

        console.log(_hotdata);

        hotCities.hotCityData = _hotdata;
        Vue.nextTick(function () {
            bindCick();
        })

    });
}

function bindCick() {
    var _subitemcol = $(".sub-item-col");
    _subitemcol.each(function () {
        $(this).unbind("click");
        $(this).click(function () {
            var _dname = $(this).data("dname");
            var _did = $(this).data("did");
            $(".select_city").html(_dname);
            partnerData.partnerData.OftenCityName = _dname;
            partnerData.partnerData.DistrictId = _did;
            $(".select_city").css({ "color": "#2c2c2c" });
            $(".selector_cover").hide();
            $(".selector_city").hide();

            $(".searchbody").hide();

            $("#partner-info").show();

            changeName();
            console.log(partnerData.partnerData);
        })
    })
}

function searchCities(keyword) {
    $.ajax({
        url: _Config.APIUrl + "/api/Search/Search",
        data: { cityCount: 10, keyword: keyword },
        type: "GET",
        success: function (_result) {
            console.log(_result);
        }
    })
}

var _canSubmit = false;
function changeName() {
    if (!_canSubmit) { 
    var _name = $("#username").val();

    var _city = $(".select_city").text();
        if (_name.length > 0 && (_city.length > 0 && _city != "点击选择")) {
            $(".ctrl .btn").css({ "background": "#FE8000" });
            $(".ctrl .btn").addClass("sub-apply");
            _canSubmit = true;
            //提交申请
            $(".sub-apply").click(function () {

                if (!partnerData.partnerData.Name) {

                    _Modal.show({
                        title: '',
                        content: '请填写你的真实姓名',
                        confirmText: '好的',
                        confirm: function () {
                            _Modal.hide();
                        },
                        showCancel: false
                    });
                    return;
                }

                if (!partnerData.partnerData.OftenCityName || partnerData.partnerData.OftenCityName == "点击选择") {

                    _Modal.show({
                        title: '',
                        content: '请填写你的常住城市',
                        confirmText: '好的',
                        confirm: function () {
                            _Modal.hide();
                        },
                        showCancel: false
                    });
                    return;
                }
                console.log(partnerData.partnerData);

                var _addInfo = partnerData.partnerData;
                $.post(_Config.APIUrl + "/api/Partner/AddRetailerInvateinfo", _addInfo, function (_data) {

                    console.log(_data)
                    isload = true;
                    if (_data) {
                        document.title = "提交成功";
                        $("#partner-info").hide();
                        $(".sub-complate").show();
                    }
                    else {
                        _Modal.show({
                            title: '',
                            content: '提交失败，请重试',
                            confirmText: '好的',
                            confirm: function () {
                                _Modal.hide();
                            },
                            showCancel: false
                        });
                        return;
                    }
                });
            });
        }
    }
}

//var i = 0;
//function keyup() {
//    i++;
//    var ss = $(".input_city_search").val()
//    console.log(i);
//    console.log(ss);
//}



