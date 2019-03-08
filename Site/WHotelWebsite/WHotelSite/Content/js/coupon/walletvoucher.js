var isapp = $("#isapp").val() == "1";
var isvip = $("#isvip").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var isSelectForVoucher = $("#isSelect").val() == "1";
var isSection = $("#isSection").val() == "1";
var orderId = $("#orderId").val();
var totalPrice = parseInt($("#totalPrice").val());
var skuVoucherPrice = parseInt($("#skuVoucherPrice").val());

var _Config = new Config();
//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

$(function () {
   
    if (isSelectForVoucher) {

        if (skuVoucherPrice <= 0 && totalPrice  > 0) {
            skuVoucherPrice = totalPrice;
        }

        //tab切换事件
        var _couponList = $("#voucher-select .coupon-list");
        var _tabItems = $("#voucher-select .tab-list .t-item-s");
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
                    $("#voucher-select .{0}".format(_thisListCss)).fadeIn(200);
                }

            });

        });

        //绑定可用券的选择事件
        var _canSelectVoucherItems = $("#voucher-select .list-s-0 .item");
        var bindVoucherSelectEvent = function () {

            _canSelectVoucherItems.each(function () {

                $(this).click(function () {

                    var _discount = parseInt($(this).data("discount"));
                    var _id = $(this).data("id");
                    var _op = $(this).data("op");
                    if (_op === 1) {

                        $(this).removeClass("sel");
                        $(this).data("op", 0);
                    }
                    else {

                        if (totalPrice <= 0) {
                            alert("当前订单金额无需使用代金券");
                            return;
                        }

                        //获取当前所选总金额
                        var _selectedTotalDiscount = getSelectedTotalDiscount();

                        //如果当前所选金额已经>=当前SKU的代金券限额了，就不能再选了
                        if (skuVoucherPrice > 0 && _selectedTotalDiscount >= skuVoucherPrice) {

                            alert("当前产品只能抵用¥" + skuVoucherPrice + "哦～");
                            return;
                        }

                        $(this).addClass("sel");
                        $(this).data("op", 1);
                    }

                    //清除“不使用现金券”
                    $("#voucher-select .not-use").data("op", 0);
                });
            });
        }
        bindVoucherSelectEvent();

        //获取当前所选总金额
        var getSelectedTotalDiscount = function () {

            var _totalDiscount = 0;

            //遍历所有可用券的选择信息
            _canSelectVoucherItems.each(function () {

                var _discount = parseInt($(this).data("discount"));
                var _op = $(this).data("op");
                if (_op === 1) {

                    _totalDiscount += _discount;
                }
            });

            return _totalDiscount;
        }

        //不使用代金券的选择操作
        $("#voucher-select .not-use").click(function () {

            var _op = $(this).data("op");
            if (_op === 1) {
                //$(this).removeClass("nosel-ok");
                $(this).data("op", 0);
            }
            else {

                //设置选中状态
                _canSelectVoucherItems.each(function () {
                    $(this).removeClass("sel");
                    $(this).data("op", 0);
                });

                //$(this).addClass("nosel-ok");
                $(this).data("op", 1);

                history.back();

                window.selectVoucherFun([]);
            }

        });

        //确定事件
        $("#voucher-select .confirm").click(function () {

            var _selectCouponIds = new Array();

            //遍历所有可用券的选择信息
            _canSelectVoucherItems.each(function () {

                var _id = parseInt($(this).data("id"));
                var _op = $(this).data("op");
                if (_op === 1) {

                    _selectCouponIds.push(_id);
                }
            });

            console.log("已选择代金券：");
            console.log(_selectCouponIds);

            history.back();

            window.selectVoucherFun(_selectCouponIds);
        });
    }
    else {

        //tab切换事件
        var _couponList = $("#voucher .coupon-list");
        var _tabItems = $("#voucher .tab-list .t-item");
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
                    $("#voucher .{0}".format(_thisListCss)).fadeIn(200);
                }

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