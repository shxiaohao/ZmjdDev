var userId = $("#userid").val();
var isapp = $("#isapp").val() == "1";
var _Config = new Config();


$(function () {
    var couponinfo = new Vue({
        el: "#couponinfo",
        data: {
            couponinfo: { State:-1}
        },
        methods: {
            vueSearchExchangeNO: function () {
                searchExchangeNO();
            },
            vueGoBook: function () {
                goBook();
            },
            vueCanceBook: function () {
                cancelBook();
            }
        }
    })

    var searchExchangeNO = function () {

        var exchangeno = $("#exchangeno").val().trim();
        if (exchangeno != "") {
            var param = { "exchangeNO": exchangeno };
            $.get(_Config.APIUrl + "/api/coupon/GetExchangeNoBookExchangeInfoByExchangeNo", param, function (_result) {

                console.log(_result);
                Vue.nextTick(function () {
                    couponinfo.couponinfo = _result;
                })

                console.log(couponinfo.couponinfo);
            })
        }
        else {
            alert("请输入券码！");
        }
    }
    
    if ($("#exchangeno").val().trim() != "") {
        searchExchangeNO();
    }

    var goBook = function () {
        gourl("/coupon/CouponReserve?skuid=" + couponinfo.couponinfo.SKUID + "&exid=" + couponinfo.couponinfo.CouponID + "&userid=" + userId);
    }

    var cancelBook = function () {
        _Modal.show({
            title: "取消预约",
            content: "是否确定取消预约？",
            confirmText: "确定取消",
            cancelText:"暂不取消",
            confirm: function () {
                var param = { "id": couponinfo.couponinfo.BookID };
                $.get(_Config.APIUrl + "/api/coupon/CancelBookInfo", param, function (_result) {
                    searchExchangeNO();
                    _Modal.hide();
                })
            },
            showCancel: true,
            cancel: function () {
                _Modal.hide();
            }

        })
        
    }
})
