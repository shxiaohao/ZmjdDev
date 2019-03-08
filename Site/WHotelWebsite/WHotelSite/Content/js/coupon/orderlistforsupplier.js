var _Config = new Config();
var wwidth = $(window).width();
var isApp = $("#isApp").val() == "1";
var isInWeixin = $("#isInWeixin").val() == "1";
var userid = $("#userId").val();
var unionId = $("#unionId").val();
var supplierId = $("#supplierId").val();

//当前登录用户信息初始对象
var userData;

$(function () {

    //if (!isInWeixin && !isApp) {
    //    alert("请使用微信扫码哦：）")
    //    $("#user-def-panel .phone").html("请使用微信扫码");
    //    return;
    //}

    _Loading.show();

    if (isApp) {

        //未登录，弹出登录
        if (userid == "0") {

            var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
            location.href = loginapphref;

            return;
        }
    }
    else {

        //初始mobile login
        var loginCheckFun = function () {
            reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
        }

        var loginCancelFun = function () {
            return true;
        }

        _loginModular.init(loginCheckFun, loginCancelFun, true);

        //检测登录并自动登录
        if (userid == "0") {
            if (!_loginModular.verify.autoLogin(loginCheckFun)) {
                _loginModular.show();
            }

            //避免自动登录失败，2秒中后检测是否登录，没有登录弹出登录提示
            setTimeout(function () {
                if (userid == "0") {
                    _loginModular.show();
                }
            }, 2000);

            return;
        }

        load();
    }
});

//app登录回调
var loginCallback = function (_userid) {
    userid = _userid;
    load();
}

var load = function () {

    //获取当前用户信息
    var loadUserInfo = function () {

        var _userDic = { userid: userid };
        $.get(_Config.APIUrl + "/api/Accounts/GetUserInfoByUserID", _userDic, function (_data) {

            console.log(_data);
            if (_data) {

                if (userData) {
                    userData.userInfo = _data; 
                }
                else {
                    userData = new Vue({
                        el: '#user-panel',
                        data: { "userInfo": _data }
                    })
                }
    
                Vue.nextTick(function () {

                    $("#user-def-panel").hide();
                    $("#user-panel").show();
                });
            }
            else {

            }
        });
    }
    loadUserInfo();

    //查询订单列表
    var loadOrderList = function () {

        var _userDic = { supplierId: supplierId, userid: userid };
        $.get(_Config.APIUrl + "/api/Coupon/GetUserOrderListBySupplierId", _userDic, function (_data) {

            _Loading.hide();

            console.log(_data);
            if (_data && _data.length > 0) {

                for (var _lnum = 0; _lnum < _data.length; _lnum++) {
                    var _litem = _data[_lnum];

                    //有效期日期格式化
                    _litem.ExpireTime = _litem.ExpireTime.split('T')[0];

                    _litem.Others = "有效期至{0}".format(_litem.ExpireTime);

                    //显示 去使用 按钮
                    _litem.ShowUseBtn = false;

                    //显示 已使用 按钮
                    _litem.ShowUsedBtn = false;

                    //状态验证
                    if (_litem.OrderState == 2) {
                        _litem.ShowUseBtn = true;
                        _litem.ShowUsedBtn = false;
                    }
                    else if (_litem.OrderState == 3) {
                        _litem.ShowUseBtn = false;
                        _litem.ShowUsedBtn = true;
                    }
                }

                var orderListData = new Vue({
                    el: '#order-list',
                    data: { "orderList": _data }
                })

                Vue.nextTick(function () {

                    //去使用事件绑定
                    $(".go-use-btn").each(function () {

                        var _skuId = $(this).data("skuid");
                        var _exchangeCouponId = $(this).data("excid");
                        var _exchangeCouponNo = $(this).data("exno");

                        $(this).unbind("click");
                        $(this).click(function () {
                            openOrderDetail(_skuId, _exchangeCouponId, _exchangeCouponNo, false);
                        });
                    });

                    //已使用事件绑定
                    $(".used-btn").each(function () {

                        var _skuId = $(this).data("skuid");
                        var _exchangeCouponId = $(this).data("excid");
                        var _exchangeCouponNo = $(this).data("exno");

                        $(this).unbind("click");
                        $(this).click(function () {
                            openOrderDetail(_skuId, _exchangeCouponId, _exchangeCouponNo, true);
                        });
                    });
                })
            }
            else {
                $("#null-list").show();
            }
        });
    }
    loadOrderList();

    //弹出订单明细
    var orderDetailData = new Vue({
        el: "#order-detail-panel",
        data: {
            "detailInfo": {}
        }
    })
    var openOrderDetail = function (_skuId, _exchangeCouponId, _exchangeCouponNo, _isUsed) {

        _Loading.show();

        //hide old detail
        $("#order-detail-scroll .sku-info").hide();
        $("#order-detail-scroll .detail-info").hide();
        $("#detail-btns").hide();

        //show panel
        showOrderDetailFunc();

        //回到顶部
        $("#order-detail-scroll").animate({ scrollTop: 0 }, 0);

        var _detailDic = { id: _exchangeCouponId };
        $.get(_Config.APIUrl + '/api/coupon/GetExchangeCouponOrderDetail', _detailDic, function (data) {

            _Loading.hide();

            console.log(data);
            if (data) {

                //是否已使用
                data.IsUsed = _isUsed;

                //是否正在核销（点击核销按钮后，会显示“正在核销”，以避免用户重复点击）
                data.WriteOffIng = false;

                //追加倒计时
                data.UseBtnTimerNum = 5;

                //有效期格式化
                data.ExpireTime = data.ExpireTime.split('T')[0];

                //有效期模块显示（OthersTip可能会显示 大团购产品的付尾款时间 提示）
                data.OthersTip = "有效期至" + data.ExpireTime;

                //电话/地址显示
                if (data.DicProperties) {
                    data.telObj = [];
                    data.otherObjList = [];
                    for (var _key in data.DicProperties) {
                        var _val = data.DicProperties[_key];
                        var _lab = _key.replace(":", "").replace("：", "");
                        if (_lab.indexOf("电话") >= 0) {
                            var _tel = _val;
                            var _telex = "";
                            if (_val.indexOf("转") >= 0) {
                                _tel = _val.split('转')[0];
                                _telex = "转" + _val.split('转')[1];
                            }

                            data.otherObjList.push({
                                "lab": _lab,
                                "tel": _tel,
                                "telex": _telex,
                                "istel": 1,
                            });
                        }
                        else {
                            data.otherObjList.push({
                                "lab": _lab,
                                "val": _val,
                                "telex": "",
                                "istel": 0
                            })
                        }
                    }
                }

                //已预约信息
                data.FirstReserveInfo = null;

                //exchangelist处理
                if (data.ExchangeCouponList) {

                    for (var _enum = 0; _enum < data.ExchangeCouponList.length; _enum++) {
                        var _eitem = data.ExchangeCouponList[_enum];

                        //获取非定金的券码（如果是大团购获取尾款的券码，非大团购则肯定都是非定金）
                        if (!_eitem.IsDepositOrder) {

                            //需要预约
                            if (_eitem.IsBook) {

                                //预约配置
                                data.ReserveSKUID = _eitem.SKUID;
                                data.ReserveExId = _eitem.ID;

                                //已预约信息
                                if (_eitem.BookUserDateList && _eitem.BookUserDateList.length) {
                                    data.FirstReserveInfo = _eitem.BookUserDateList[0];

                                    //时间格式化
                                    data.FirstReserveInfo.BookDay = data.FirstReserveInfo.BookDay.split('T')[0];
                                }
                            }
                        }
                    }
                }

                //build order detail vue
                orderDetailData.detailInfo = data;

                Vue.nextTick(function () {

                    //show detail
                    $("#order-detail-scroll .sku-info").show();
                    $("#order-detail-scroll .detail-info").show();
                    $("#detail-btns").show();

                    //关闭事件
                    $("#order-detail-panel .close").unbind("click");
                    $("#order-detail-panel .close").click(hideOrderDetailFunc);

                    //【特殊处理】针对立秀宝被黄牛刷票的产品特殊处理
                    if ((_skuId == 10760 || _skuId == 10983)) {

                        //获取当前userid关联的unionid是否为当前登录的微信unionid，如果不是，则说明不是购买该产品的手机微信在使用该产品，不允许
                        var _channlelRelDic = { IDX: 0, Channel: "weixin", UserId: userid, Code: "", PhoneNum: "", Tag: "" };
                        $.post(_Config.APIUrl + '/api/Accounts/GetOneUserChannelRelByUID', _channlelRelDic, function (_channelReldata) {
                        //$.post('http://api.zmjd100.com/api/Accounts/GetOneUserChannelRelByUID', _channlelRelDic, function (_channelReldata) {

                            console.log(_channelReldata)

                            if (_channelReldata && _channelReldata.Code) {

                                if (_channelReldata.Code && _channelReldata.Code.AllTrim() != unionId.AllTrim()) {
                                    alert("抱歉，当前登录的微信不是购买该产品时使用的微信，暂不能使用。\r\n\r\n如有疑问请联系客服\r\n\r\n即将退出登录..");

                                    //关闭核销详情
                                    hideOrderDetailFunc();

                                    //退出登录
                                    gourl("/Account/WxMenuTransfer?menu=-1");
                                    return;
                                }
                                else {

                                    if (!_isUsed) {

                                        //开始倒计时
                                        startUseTimer();

                                        //核销事件
                                        $(".bottom-use-btn").unbind("click");
                                        $(".bottom-use-btn").click(function () {

                                            writeOffExchangeCoupon(_exchangeCouponNo);
                                        });
                                    }

                                }

                            }
                        });
                        return;
                    }

                    if (!_isUsed) {

                        //开始倒计时
                        startUseTimer();

                        //核销事件
                        $(".bottom-use-btn").unbind("click");
                        $(".bottom-use-btn").click(function () {

                            writeOffExchangeCoupon(_exchangeCouponNo);
                        });
                    }

                })
            }
        });
    }

    //核销订单
    var writeOffDetailData = new Vue({
        el: "#use-complated-panel",
        data: {
            "detailInfo": {}
        }
    })
    var writeOffExchangeCoupon = function (_exchangeNo) {

        _Loading.show();

        //显示“正在核销”
        orderDetailData.detailInfo.WriteOffIng = true;

        ////隐藏订单明细
        //hideOrderDetailFunc();

        ////显示核销完成
        //showUseComplatedFunc();

        //return;

        var _dic = { exchangeNo: _exchangeNo, supplierId: supplierId, curUserId: userid };
        $.get(_Config.APIUrl + '/api/coupon/WriteOffExchangeCoupon', _dic, function (data) {

            _Loading.hide();

            //隐藏“正在核销”
            orderDetailData.detailInfo.WriteOffIng = false;

            console.log(data);
            if (data) {

                switch (data.State) {
                    case 1: {

                        //核销日期格式化
                        data.WriteOffTime = data.WriteOffTime.split('T')[0];

                        //build data
                        writeOffDetailData.detailInfo = data;

                        //隐藏订单明细
                        hideOrderDetailFunc();

                        //显示核销完成
                        showUseComplatedFunc();

                        Vue.nextTick(function () {

                            //核销成功确定事件
                            $("#use-complated-panel .confirm-btn").unbind("click");
                            $("#use-complated-panel .confirm-btn").click(function () {

                                ////关闭核销成功弹窗
                                //hideUseComplatedFunc();

                                //刷新页面
                                location.reload();
                            });
                        })
                        break;
                    }
                    case -1: {

                        _Modal.show({
                            title: "",
                            content: "该订单不可用",
                            textAlign: "center",
                            confirmText: "关闭",
                            confirm: function () {
                                _Modal.hide();
                            }
                        });
                        break;
                    }
                    case 3: {

                        _Modal.show({
                            title: "",
                            content: "该订单已核销，请勿重复核销",
                            textAlign: "center",
                            confirmText: "关闭",
                            confirm: function () {
                                _Modal.hide();
                            }
                        });
                        break;
                    }
                    default: {

                        _Modal.show({
                            title: "",
                            content: "核销失败，请重试或联系我们",
                            textAlign: "center",
                            confirmText: "关闭",
                            confirm: function () {
                                _Modal.hide();
                            }
                        });
                        break;
                    }

                }
            }
        });
    }

    //5秒核销按钮倒计时
    var useTimer = null;
    var startUseTimer = function () {

        useTimer = setInterval(function () {

            console.log(orderDetailData.detailInfo.UseBtnTimerNum);

            if (orderDetailData.detailInfo.UseBtnTimerNum < 1) {

                stopUseTimer();
            }
            orderDetailData.detailInfo.UseBtnTimerNum = orderDetailData.detailInfo.UseBtnTimerNum - 1;
        }, 1000);
    }
    var stopUseTimer = function () {
        clearInterval(useTimer)
    }

    //显示订单详情
    var showOrderDetailFunc = function () {
        $("#order-detail-panel").show();
        $("#model-bg-panel").show();
    }
    var hideOrderDetailFunc = function () {
        $("#order-detail-panel").hide();
        $("#model-bg-panel").hide();

        //明细关闭后，需停止核销按钮倒计时
        stopUseTimer();
    }

    //显示核销完成
    var showUseComplatedFunc = function () {
        $("#use-complated-panel").show();
        $("#model-bg-panel").show();
    }
    var hideUseComplatedFunc = function () {
        $("#use-complated-panel").hide();
        $("#model-bg-panel").hide();
    }
}

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    load();
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