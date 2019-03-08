var isapp = $("#isapp").val() == "1";
var isvip = $("#isvip").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var isSelect = $("#isSelect").val() == "1";
var isSection = $("#isSection").val() == "1";
var orderId = $("#orderId").val();

var _Config = new Config();
//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

var cashCouponUseLogVue = null;

$(function () {
   
    //tab切换
    if (isSelect) {

        var _couponList = $("#cash-coupon-select .coupon-list");
        var _tabItems = $("#cash-coupon-select .tab-list .t-item-s");
        _tabItems.each(function () {

            $(this).click(function () {

                var _op = $(this).data("op");
                var _id = $(this).data("id");

                if (_op === 0) {

                    //关闭所有菜单状态
                    _tabItems.each(function () { $(this).removeClass("sel"); $(this).data("op", 0); });
                    $(this).addClass("sel");
                    $(this).data("op", 1);

                    //隐藏所有list
                    _couponList.each(function () { $(this).hide(); });

                    var _thisListCss = "list-s-{0}".format(_id);
                    $("#cash-coupon-select .{0}".format(_thisListCss)).fadeIn(200);
                }

            });

        });

        //绑定可用券的选择事件
        var _canSelectCouponItems = $("#cash-coupon-select .list-s-0 .item");
        var bindCouponSelectEvent = function () {

            _canSelectCouponItems.each(function () {

                $(this).click(function () {

                    var _id = $(this).data("id");

                    //alert(_id)

                    //清除“不使用现金券”
                    $("#cash-coupon-select .no-select").removeClass("nosel-ok");
                    $("#cash-coupon-select .no-select").data("op", 0);

                    //设置选中状态
                    _canSelectCouponItems.each(function () { $(this).removeClass("sel"); });
                    $(this).addClass("sel");

                    //如果是app环境，则使用app的back
                    if (isapp && location.pathname.toLowerCase().indexOf('walletcashcoupon') >= 0) {

                        //history.back();

                        var param = {
                            "refresh": 1,               //是否刷新:1
                            "data": { "couponid": _id },  //当是现金券类型操作时，会传现金券ID回去
                        }
                        zmjd.pageBack(param);

                    }
                    else {

                        history.back();

                        window.selectCashCouponFun(_id);
                    }
                });
            });
        }
        bindCouponSelectEvent();

        //不使用现金券的选择操作
        $("#cash-coupon-select .no-select").click(function () {

            var _op = $(this).data("op");
            if (_op === 1) {
                $(this).removeClass("nosel-ok");
                $(this).data("op", 0);
            }
            else {

                //设置选中状态
                _canSelectCouponItems.each(function () { $(this).removeClass("sel"); });

                $(this).addClass("nosel-ok");
                $(this).data("op", 1);

                //如果是app环境，则使用app的back
                if (isapp && location.pathname.toLowerCase().indexOf('walletcashcoupon') >= 0) {

                    //history.back();

                    var param = {
                        "refresh": 1,               //是否刷新:1
                        "data": { "couponid": -1 },  //当是现金券类型操作时，会传现金券ID回去
                    }
                    zmjd.pageBack(param);

                }
                else {

                    history.back();

                    window.selectCashCouponFun(-1);
                }
            }

        });
    }
    else {

        var _couponList = $("#cash-coupon .coupon-list");
        var _tabItems = $("#cash-coupon .tab-list .t-item");
        _tabItems.each(function () {

            $(this).click(function () {

                var _op = $(this).data("op");
                var _id = $(this).data("id");

                if (_op === 0) {

                    //关闭所有菜单状态
                    _tabItems.each(function () { $(this).removeClass("sel"); $(this).data("op", 0); });
                    $(this).addClass("sel");
                    $(this).data("op", 1);

                    //隐藏所有list
                    _couponList.each(function () { $(this).hide(); });

                    var _thisListCss = "list-{0}".format(_id);
                    $("#cash-coupon .{0}".format(_thisListCss)).fadeIn(200);
                }

            });

        });

        //使用记录 事件
        $("#cash-coupon .detail-link").each(function () {

            $(this).click(function () {

                var _idx = $(this).data("idx");
                //_idx = 15;

                $.get(_Config.APIUrl + '/api/coupon/GetUserCouponLogByCouponItemID', { idx: _idx }, function (_listData) {

                    console.log(_listData);

                    _listData.map(function (item, index) {
                        if (item && item.CreateTime) {
                            item.CreateTime = item.CreateTime.split("T")[0];
                        }
                    });

                    if (cashCouponUseLogVue) {
                        cashCouponUseLogVue.listData = _listData;
                    }
                    else {
                        cashCouponUseLogVue = new Vue({
                            el: '#cashcoupon-detaillist-template',
                            data: { "listData": _listData }
                        })
                    }

                    Vue.nextTick(function () {
                        
                        var _html = $("#cashcoupon-detaillist-template").html();

                        _Modal.show({
                            title: '使用记录',
                            content: _html,
                            confirmText: '关闭',
                            confirm: function () {
                                _Modal.hide();
                            },
                            showCancel: false
                        });

                        $("._modal-section").css("top", "10%");
                    })
                    
                });
            });
        });
    }
    

});

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //alert(apptype)

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}