var wwidth = $(window).width();
var isapp = $("#isapp").val() == "1";
var pubuserid = parseInt($("#userid").val());

var _name = $("#_name").val();

$(document).ready(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //检测登录并自动登录
    if (!isapp && pub_userid == "0") {
        _loginModular.verify.autoLogin(loginCheckFun);
    }

    function checkLogin() {
        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
        if (pub_userid == "0")
        {
            if ($("#loginput").val() == "1")
            {
                location.href = "whotelapp://www.zmjiudian.com/";
                return;
            }
            $("#loginput").val("1");
            location.href = loginapphref;
        }
    }

    //if (isapp) checkLogin();

    //图片轮播
    if ($('#product-img-list-0')) {
        $(".product-img-item").css("width", wwidth);
        $('#product-img-list-0').swiper({
            slidesPerView: 'auto',
            pagination: '.pagination-0',
            paginationHide: false,
            loop: true,
            offsetPxBefore: 0,
            offsetPxAfter: 0,
            onTouchEnd: function (slider) {
                if (slider.activeIndex + 1 < slider.slides.length) {
                    var li = $(slider.slides[slider.activeIndex + 1]); //alert(slider.activeIndex)

                    $(".show-img").each(function () {
                        var _load = $(this).data("load");
                        if (_load === 0) {
                            //alert(_load)
                            setImgOriSrc($(this));
                            $(this).data("load", 1);
                        }
                    });

                    //setImgOriSrc(imgObj);
                }
            }
        })

        setTimeout(function () {

            $(".p-photo").show();
            $(".def-photo").hide();

        }, 200);
    }

    //购买须知展开折叠处理
    $(".shopread .tit").click(function () {
        var open = $(this).data("open");
        if (open == "0") {
            $(this).data("open", "1");
            $(".shopread .openimg").hide();
            $(".shopread .closeimg").show();
            $(".shopread .info").fadeIn(500);
        }
        else {
            $(this).data("open", "0");
            $(".shopread .openimg").show();
            $(".shopread .closeimg").hide();
            $(".shopread .info").hide();
        }
    });

    //加减
    $(".btn0").click(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = $(".sellnum").val();
        if (num == "" || isNaN(num)) {
            num = 1;
        }
        num = parseInt(num);
        if (num > 1) num--;
        $(".sellnum").val(num);

        //得出小计
        setxiaoji();

        //验证购买数量
        checkBuyNum();
    });

    $(".btn1").click(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = $(".sellnum").val();
        if (num == "" || isNaN(num)) {
            num = 0;
        }
        num = parseInt(num);
        num++;
        $(".sellnum").val(num);

        //得出小计
        setxiaoji();

        //验证购买数量
        checkBuyNum();
    });

    $(".sellnum").change(function () {

        var cansell = $("#cansell").val() == "1";
        if (!cansell) return;

        var num = $(this).val();
        if (num == "" || isNaN(num) || parseInt(num) < 1) {
            $(this).val(1);
        }

        //得出小计
        setxiaoji();

        //验证购买数量
        checkBuyNum();
    });

    //得出小计
    function setxiaoji()
    {
        var price = parseInt($("#pingriPrice").val());
        var num = parseInt($(".sellnum").val());
        var sum = price * num;
        $(".xiaoji").data("sum", sum);
        $(".xiaoji .right .price").text(sum);
    }

    //验证购买数量
    function checkBuyNum() {
        var dic = {};
        dic["id"] = $("#aid").val();
        var num = $(".sellnum").val(); if (num == "" || isNaN(num) || parseInt(num) < 1) num = 1;
        dic["buynum"] = num;
        dic["userid"] = $("#userid").val();

        //首先验证最小购买数量
        var minbuy = parseInt($(".sellnum").data("minbuy"));
        var minbuyMsg = $(".sellnum").data("minbuymsg");
        if (parseInt(num) < minbuy) {
            showTip(minbuyMsg);
            $(".sellnum").val(minbuy);
            setxiaoji();
            return;
        }

        showSpinner(true);
        
        $.get('/Coupon/CheckBuyNumber', dic, function (content) {
            var msg = content.Message;
            var suc = content.Success;
            var cansell = content.CanSell;

            showSpinner(false);

            switch (suc) {
                //当前券已经售完
                case "0":
                    {
                        alert(msg);
                        location.reload();
                        break;
                    }
                    //个人超过限额，则禁止购买
                case "1":
                    {
                        showTip(msg);
                        $(".sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
                case "2":
                    {
                        showTip(msg);
                        $(".sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
                case "3":
                    {
                        showTip(msg);
                        $(".sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
            }
        });
    }

    //验证码操作
    $(".vCodeBtn").click(function () {
        var userPhone = $("#userPhone").val();
        if (userPhone.length != 11) {
            alert('请输入有效的手机号');
            return;
        }

        verify.send(userPhone, function (lock) {
            $(".vCodeBtn").toggleClass('disabled', lock).prop('disabled', lock);
            if (!lock) {
                $(".vCodeBtn").text('重发验证码');
            }
        }, function (seconds) {
            $(".vCodeBtn").text(seconds + '秒后可重发');
        });
    });

    //window.httpsWebUrl = "/";
    var verifyUrl = window.httpsWebUrl + 'Account/Verify';
    var verifyNewUserUrl = window.httpsWebUrl + 'Account/VerifyNewUser';
    var verify = {
        send: function (number, lock, update) {
            var self = this;
            if (self._timer) {
                return;
            }
            self._startTimer(lock, update);
            $.post(verifyUrl, {
                action: 'send',
                number: number
            }, 'json').then(function (r) {
                if (!r.ok) {
                    self._stopTimer(lock);
                    alert('系统故障，请稍后重试或联系技术支持');
                }
            }).fail(function () {
                self._stopTimer(lock);
                alert('网络请求失败');
            })
        },
        check: function (number, code) {
            return $.post(verifyNewUserUrl, {
                action: 'check',
                number: number,
                code: code,
                CID: $("#hidCurUserCID").val()
            }, 'json').then(function (r) {
                return r.ok || true ? contacts._store(number) : $.Deferred().reject(new Error('短信校验码有误'));
            });
        },
        _startTimer: function (lock, update) {
            var self = this;
            self._dest = new Date().getTime() + 60 * 1000;
            self._timer = setInterval(function () {
                var seconds = Math.max(0, ((self._dest - new Date) / 1000) | 0);
                seconds > 0 ? update(seconds) : self._stopTimer(lock);
            }, 1000);
            lock(true);
        },
        _stopTimer: function (lock) {
            clearInterval(this._timer);
            this._timer = null;
            lock(false);
        }
    };

    var isMobile = function (val) {
        return phoneNumReg.test(val);
    };

    var localStoreContact = function (contactPhone) {
        if (window.localStorage) {
            localStorage.contactPhone = contactPhone;
        } else {
            var now = new Date();
            var expires = new Date(now.setYear(now.getFullYear() + 1));
            document.cookie = 'contactPhone=' + contactPhone + '; expires=' + expires.toGMTString();
        }
    };
    var getStorePhone = function () {
        if (window.localStorage) {
            return localStorage.contactPhone;
        }
        return (document.cookie.match(/(^|; )contactPhone=(\d+)/) || ['', ''])[1];
    };

    //购买
    $(".submit").click(function ()
    {
        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";
        if (pub_userid == "0") {
            //app环境下，如果没有登录则弹出登录
            if (isapp) {
                location.href = loginapphref;
                return;
            }
            else {
                _loginModular.show();
            }
        }
        else {

            gosubmit($("#userid").val());
        }

        //var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
        //if (pub_userid == "0")
        //{
        //    //手机号码输入验证
        //    var userPhone = "";
        //    var vCode = "";
        //    if (pub_userid == "0") {
        //        userPhone = $("#userPhone").val();
        //        if (userPhone.length != 11) {
        //            alert("请输入有效的手机号");
        //            $("#userPhone").focus();
        //            return;
        //        }

        //        vCode = $("#vCode").val();
        //        if (vCode == "") {
        //            alert("请输入短信验证码");
        //            $("#vCode").focus();
        //            return;
        //        }
        //    }

        //    $.post(verifyNewUserUrl, {
        //        action: 'check',
        //        number: userPhone,
        //        code: vCode,
        //        CID: $("#hidCurUserCID").val()
        //    }, 'json').then(function (r) {
        //        if (r.ok == "1") {
        //            if (r.userid == "0") {
        //                location.href = loginapphref;
        //                return;
        //            }
        //            gosubmit(r.userid);

        //            //本地缓存手机号
        //            localStorecontact(userPhone);
        //        }
        //        else {
        //            alert("短信验证码输入有误");
        //        }
        //    });
        //}
        //else {
        //    if (pub_userid == "0") {
        //        location.href = loginapphref;
        //        return;
        //    }

        //    gosubmit($("#userid").val());
        //}
    });

    setxiaoji();
});

//var setImgOriSrc = function (imgObj) {
//    var orisrc = imgObj.data("orisrc");
//    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
//        var defsrc = imgObj.attr("src");
//        imgObj.attr("src", orisrc);
//        imgObj.data("orisrc", "");
//        imgObj.error(function () {
//            imgObj.attr("src", defsrc);
//        });
//    }
//};

function loginCallback(userid) {
    gosubmit(userid);
    //location.reload();
    //location.replace(location.pathname + "?userid=" + userid);
}

var gosubmit = function (userid) {
    var dic = {};
    dic["id"] = $("#aid").val();
    var num = $(".sellnum").val(); if (num == "" || isNaN(num) || parseInt(num) < 1) num = 1;
    dic["buynum"] = num;
    dic["userid"] = userid;

    $.get('/Coupon/CheckBuyNumber', dic, function (result) {
        var message = result.Message;
        var success = result.Success;
        var cansell = result.CanSell;

        switch (success) {
            //其他错误
            case "-1":
                {
                    alert(message);
                    break;
                }
                //当前券已经售完
            case "0":
                {
                    alert(message);
                    location.reload();
                    break;
                }
                //个人超过限额，则禁止购买
            case "1":
                {
                    showTip2(message);
                    $(".sellnum").val(cansell);
                    setxiaoji();
                    break;
                }
            case "2":
                {
                    showTip2(message);
                    $(".sellnum").val(cansell);
                    setxiaoji();
                    break;
                }
            case "3":
                {
                    showTip2(message);
                    $(".sellnum").val(cansell);
                    setxiaoji();
                    break;
                }
            default:
                {
                    var subdic = {};
                    subdic["aid"] = $("#aid").val();
                    subdic["atype"] = $("#atype").val();
                    subdic["pid"] = $("#pid").val();
                    subdic["pricetype"] = $("#pricetype").val();
                    var sellnum = $(".sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
                    subdic["paynum"] = sellnum;
                    subdic["userid"] = userid;

                    $.get('/Coupon/SubmitConpon', subdic, function (content) {
                        var msg = content.Message;
                        var suc = content.Success;
                        var url = content.Url;

                        switch (suc) {
                            case "0":
                                {
                                    location = url;
                                    break;
                                }
                            case "1":
                                {
                                    showTip2(msg);
                                    break;
                                }
                        }
                    });
                    break;
                }
        }

    });
};

function showTip(mes) {
    $(".priceAlertTip .tipinfo").html(mes);
    $(".priceAlertTip").fadeIn(300);
    setTimeout(function () {
        $(".priceAlertTip").fadeOut(300);
    }, 3000);
}

function showTip2(mes) {
    $(".pubAlertTip .tipinfo").html(mes);
    $(".pubAlertTip").fadeIn(500);
    setTimeout(function () {
        $(".pubAlertTip").fadeOut(300);
    }, 3000);
}

function goto(param) {
    var isapp = $("#isapp").val() == "1";
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (!isapp) {
        url = "http://www.zmjiudian.com/" + param;
    }

    this.location = url;
}

function gourl(url) {
    location.href = url;
}

function loginCallback(userid) {
    location.replace(location.pathname + "?userid=" + userid);
}

var timerTags = $(".timer-tag");
var timeDic = [];
if (timerTags) {
    for (var i = 0; i < timerTags.length; i++) {

        timeDic[i] = {
            timerEntity: null,
            nowTime: null,
            endDate: null,
            closeDate: null,
            endTimerState: true,
            closeTimerState: false,
            initNowtime: function () {
                this.nowTime = new Date(
                parseInt(this.timerEntity.data("year0"))
                , parseInt(this.timerEntity.data("month0"))
                , parseInt(this.timerEntity.data("day0"))
                , parseInt(this.timerEntity.data("hour0"))
                , parseInt(this.timerEntity.data("minute0"))
                , parseInt(this.timerEntity.data("second0"))
                    ).getTime();
            },
            initEndtime: function () {
                this.endDate = new Date(
                parseInt(this.timerEntity.data("year1"))
                , parseInt(this.timerEntity.data("month1"))
                , parseInt(this.timerEntity.data("day1"))
                , parseInt(this.timerEntity.data("hour1"))
                , parseInt(this.timerEntity.data("minute1"))
                , parseInt(this.timerEntity.data("second1"))
                    ).getTime();
            },
            initClosetime: function () {
                this.closeDate = new Date(
                parseInt(this.timerEntity.data("year2"))
                , parseInt(this.timerEntity.data("month2"))
                , parseInt(this.timerEntity.data("day2"))
                , parseInt(this.timerEntity.data("hour2"))
                , parseInt(this.timerEntity.data("minute2"))
                , parseInt(this.timerEntity.data("second2"))
                    ).getTime();
            },
            init: function () {
                this.initNowtime();
                this.initEndtime();
                this.initClosetime();
            },
            timerAction: function () {
                if (this.endTimerState) {
                    var t = this.endDate - this.nowTime;
                    var d = Math.floor(t / (1000 * 60 * 60 * 24));
                    var h = Math.floor(t / 1000 / 60 / 60 % 24);// + (d * 24);
                    var m = Math.floor(t / 1000 / 60 % 60);
                    var s = Math.floor(t / 1000 % 60);

                    var timehtml =
                        d <= 0 ? (
                        h <= 0
                        ? "距开抢 <time>00</time>:<time>" + (m < 10 ? "0" + m : m) + "</time>:<time>" + (s < 10 ? "0" + s : s) + "</time>"
                        : "距开抢 <time>" + (h < 10 ? "0" + h : h) + "</time>:<time>" + (m < 10 ? "0" + m : m) + "</time>:<time>" + (s < 10 ? "0" + s : s) + "</time>"
                        ) : "距开抢" + d + "天";


                    this.timerEntity.html(timehtml);
                    //$("#timer-tag-0").html(timehtml);

                    try {

                        if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                            this.stopEndAction();
                        }

                    } catch (e) { }

                    this.nowTime = this.nowTime + 1000;
                }
            },
            timerCloseAction: function () {
                if (this.closeTimerState) {
                    var t = this.closeDate - this.nowTime;
                    var d = Math.floor(t / (1000 * 60 * 60 * 24));
                    var h = Math.floor(t / 1000 / 60 / 60 % 24);// + (d * 24);
                    var m = Math.floor(t / 1000 / 60 % 60);
                    var s = Math.floor(t / 1000 % 60);

                    var timehtml = d <= 0 ?
                        (h <= 0 ? "剩余<time>" + (m < 10 ? "0" + m : m) + "</time>分钟<time>" + (s < 10 ? "0" + s : s) + "</time>秒"
                        : "剩余<time>" + (h < 10 ? "0" + h : h) + "</time>小时<time>" + (m < 10 ? "0" + m : m) + "</time>分钟")
                        : "剩余<time>" + (d < 10 ? "0" + d : d) + "</time>天<time>" + (h < 10 ? "0" + h : h) + "</time>小时<time>" + (m < 10 ? "0" + m : m) + "</time>分钟";

                    this.timerEntity.html(timehtml);
                    //$("#timer-tag-0").html(timehtml);

                    try {

                        if (d < 0 || (d <= 0 && h <= 0 && m <= 0 && s <= 0)) {
                            this.stopCloseAction();
                        }

                    } catch (e) { }

                    this.nowTime = this.nowTime + 1000;
                }
            },
            stopEndAction: function () {
                this.endTimerState = false;
                this.closeTimerState = true;
                this.timerEntity.html("进行中");

                $("#cansell").val("1");
                try {
                    $(".submit").css("display", "block");
                    $(".submit0").css("display", "none");
                } catch (e) {

                }
            },
            stopCloseAction: function () {
                this.closeTimerState = false;
                this.timerEntity.html("已结束");

                $("#cansell").val("0");
                try {
                    $(".submit").css("display", "none");
                    $(".submit0").html("活动已结束");
                    $(".submit0").css("display", "block");
                } catch (e) {

                }
            }
        };

        //build
        timeDic[i].timerEntity = $(timerTags[i]);

        //init
        timeDic[i].init();

        //start
        timeDic[i].timerAction();
        setInterval("gotime(timeDic[" + i + "])", 1000);
    }
}

function gotime(timeObj) {
    timeObj.timerAction();
    timeObj.timerCloseAction();
}

$(".wxsignalert .iknow").click(function () {
    $(".wxsignalert").fadeOut();
});

$(".wxSign").click(function () {
    $(".wxsignalert").fadeIn();
    var _apptype = $("#apptype").val();
    if (_apptype != "android") window.open("weixin://dl/");
});

if ($(".wxSignOk")) {
    setInterval(function () {
        var _obj = $(".wxSignOk .goshop-timer");
        var _count = parseInt(_obj.data("count"));
        if (_count == 1) {
            $(".wxSignOk").fadeOut();
        }
        else {
            _count--;
            _obj.data("count", _count)
            _obj.html(_count + "s后将自动跳转");
        }
    }, 1000);
    $(".wxSignOk .goshop").click(function () {
        $(".wxSignOk").fadeOut();
    });
}

if ($(".buyvip-md")) {

    $(".buyvip-md").click(function () {
        $(".buyvip-alert").hide();
        $(".buyvip-md").hide();
    });

    $(".buyvip-alert").click(function () {
        $(".buyvip-alert").hide();
        $(".buyvip-md").hide();
    });
}

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': _name, 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}