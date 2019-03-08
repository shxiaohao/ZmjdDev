var phoneNumReg2 = /^((100)|(13[0-9])|(15[0-9])|(19[0-9])|(18[0-9])|(17[0-9])|(16[0-9])|(14[0-9]))\d{8}$/;
var userId = $("#curUserID").val();
var key = $("#key").val();
var openid = $("#openid").val();
var redRecordId = parseInt($("#redRecordId").val());
var redPhoneKey = "red_phone_key";

var _Config = new Config();

$(function () {

    //如果指定了领取记录ID，则更新其状态
    if (redRecordId) {

        $.get(_Config.APIUrl + "/api/coupon/ReturnRedResult", { redRecordId: redRecordId }, function (_returnData) {
            console.log(_returnData);
        });
    }

    var _getId = 0;
    var _giftCount = 10;
    var _giftStart = 0;

    //刷新红包领取list
    var redRedRecordListData = null;
    var redRedRecordListLock = true;
    var redRedRecordList = function () {

        var _redCouponListDic = { "key": key, "count": _giftCount, "start": _giftStart };
        $.get(_Config.APIUrl + "/api/coupon/GetRedRecordBykey", _redCouponListDic, function (_data) {

            console.log(_data);
            if (_data) {

                //参与人时间格式化
                if (_data.RedRecordList) {
                    _data.RedRecordList.map(function (item, index) {
                        var _joinTime = item.CreateTime;
                        dtArr = (_joinTime).split("-");
                        dayArr = dtArr[2].split("T");
                        timeArr = dayArr[1].split(":");
                        item.CreateTime = dtArr[1] + "-" + dayArr[0] + " " + timeArr[0] + ":" + timeArr[1];
                    });
                }

                if (redRedRecordListData) {

                    var _oldListData = redRedRecordListData.listData;
                    for (var _num = 0; _num < _data.RedRecordList.length; _num++) {
                        var _newData = _data.RedRecordList[_num];
                        _oldListData.RedRecordList.push(_newData);
                    }
                    redRedRecordListData.listData = _oldListData;
                }
                else {
                    redRedRecordListData = new Vue({
                        el: '#gift-list',
                        data: { "listData": _data }
                    })
                }

                //如果当前页已经没有数据了，则隐藏 查看更多
                if (!_data.RedRecordList || !_data.RedRecordList.length || _data.RedRecordList.length < _giftCount) {
                    $(".more-gift-list").css("color", "#919191");
                    $(".more-gift-list").html("没有更多了");
                    setTimeout(function () { $(".more-gift-list").fadeOut(); }, 1000);
                }

                if (_data.RedState == 0 || _getId) {

                    $("#red-info").show();
                    $("#red-null").hide();
                    $("#red-late").hide();
                }
                else {

                    switch (_data.RedState) {

                        //红包可用
                        case 0: {
                            $("#red-info").show();
                            $("#red-null").hide();
                            $("#red-late").hide();
                            break;
                        }
                            //红包领完
                        case 1: {
                            $("#red-info").hide();
                            $("#red-null").show();
                            $("#red-late").hide();
                            break;
                        }
                            //红包过期
                        case 2: {
                            $("#red-info").hide();
                            $("#red-null").hide();
                            $("#red-late").show();
                            break;
                        }

                    }
                }

            }
        });
    }

    //查看更多手气
    $(".more-gift-list").click(function () {

        _giftStart += _giftCount;
        redRedRecordList();
    });

    //刷新红包结果
    var refRedCouponResult = function (_phone) {

        //领取红包
        var _getRedDic = { "key": key, phoneNum: _phone, openid: openid };
        $.get(_Config.APIUrl + "/api/coupon/GetRedRecordByGuidAndPhone", _getRedDic, function (_getData) {

            console.log(_getData);
            if (_getData && _getData.ID) {

                _getId = _getData.ID;

                //领取过缓存
                Store.Set(redPhoneKey, _phone);

                var getRedData = new Vue({
                    el: '#get-red-info',
                    data: { "getData": _getData }
                })
            }
            else {

                //alert("领取失败，红包已过期或已领完");
                //领取过缓存
                Store.Set(redPhoneKey, _phone);

                var getRedData = new Vue({
                    el: '#get-red-info',
                    data: { "getData": {} }
                })
            }

            //控制是否显示 大家的手气
            if (_getData && !_getData.IsShowResult) {

                $("#gift-list").hide();
            }

            $('body').append("<style>html, body {background:url();}</style>");

            $("#get-step").hide();
            $("#result-step").show();

            //刷新list
            redRedRecordList();
        });
    }

    //如果已经缓存，则直接显示结果
    var _thisKeyPhoneCache = Store.Get(redPhoneKey);
    if (_thisKeyPhoneCache) {

        console.log("有缓存手机号");
        console.log(_thisKeyPhoneCache);

        refRedCouponResult(_thisKeyPhoneCache);
    }
    else {

        $("#get-step").show();
        $("#result-step").hide();
    }

    //var cookieguid = Store.Get("key");
    //var cookiecid = Store.Get("phone");
    //var guid = $("#redguid").data("guid");
    //var rednoneguid = $("#RedNoneguid").data("noneguid");
    //if (cookieguid == guid) {
    //    window.location.href = "/Coupon/RedCashCouponResult?key=" + guid + "&cid=" + cookiecid;
    //}
    //else if (rednoneguid.trim().length > 0) {
    //    window.location.href = "/Coupon/RedCashCouponResult?key=" + rednoneguid
    //}

    //领取红包事件
    $(".get-coupon").click(function () {

        var phoneNum = $.trim($("#phone").val());
        if (!phoneNum) {
            alert("请先填写手机号");
            return false;
        }
        else if (!phoneNumReg2.test(phoneNum)) {
            alert("手机号格式错误");
            return false;
        }

        refRedCouponResult(phoneNum);

    });

    //查看我的奖品
    $(".open-coupon").click(function () {

        var _html = $("#get-coupon-info-template").html();

        _Modal.show({
            title: '',
            content: _html,
            confirmText: '我知道了',
            confirm: function () {
                _Modal.hide();
            },
            showCancel: false
        });

        $("._modal-section").css("top", "15%");
    });

})

function getRedCoupon() {
    var phoneNumReg2 = /^((100)|(13[0-9])|(15[0-9])|(19[0-9])|(18[0-9])|(17[0-9])|(16[0-9])|(14[0-9]))\d{8}$/;
    var guid = $("#redguid").data("guid");
    var url = "/Coupon/RedCashCouponChecked?key=" + guid;
    var phoneNum = $.trim($("#phoneNum").val());
    if (!phoneNum) {
        alert("请先填写手机号");
        return false;
    }
    else if (!phoneNumReg2.test(phoneNum)) {
        alert("手机号格式错误");
        return false;
    }
    $.post(url, { phoneNum: phoneNum }, function (res) {
        if (res.url) {
            window.location.href = res.url;
        }
        else {
            alert(res.Message);
            return false;
        }
    }, "json");//post方式访问结果页
}