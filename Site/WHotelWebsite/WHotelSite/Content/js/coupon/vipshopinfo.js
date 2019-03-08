var loadMorePackages = {};
var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";

var aid = 100929;
var userid = $("#userid").val();
var albumId = $("#albumId").val();
var t = $("#t").val();
var userlat = $("#userlat").val();
var userlng = $("#userlng").val();
var geoScopeType = 0;
var districtId = 0;
var isApp = $("#isApp").val() == "1";
var magicallUrl = $("#magicallUrl").val();

//会员切换
var ctrlBottom = $(".ctrl-bottom");
var option199 = $(".option-199");
var table199 = $(".t-199");
var option599 = $(".option-599");
var table599 = $(".t-599");
var optionBg = $(".op-bg");
var goBuy199 = $(".go-buy-199");
var goBuy599 = $(".go-buy-599");

$(function () {

    option199.click(function () {
        var _sel = option199.data("sel");
        if (_sel == "0") {
            option199.addClass("sel");
            option199.data("sel", "1");
            option599.removeClass("sel");
            option599.data("sel", "0");
            optionBg.animate({ left: "-2px" }, 0);

            table199.fadeIn(500);
            table599.hide();

            goBuy199.show();
            goBuy599.hide();

            aid = $("#aid199").val();
        }
    });
    option599.click(function () {
        var _sel = option599.data("sel");
        if (_sel == "0") {
            option599.addClass("sel");
            option599.data("sel", "1");
            option199.removeClass("sel");
            option199.data("sel", "0");
            optionBg.animate({ left: "50%" }, 0);

            table599.fadeIn(500);
            table199.hide();

            goBuy599.show();
            goBuy199.hide();

            aid = $("#aid599").val();
        }
    });

    goBuy199.click(function () {

        if (userid == "0") {

            if (isApp) {
                location.href = loginapphref;
                return;
            }
        }

        showModal();
        //var _aid = $("#aid199").val();
        //goBuy(_aid);
    });
    goBuy599.click(function () {

        if (userid == "0") {

            if (isApp) {
                location.href = loginapphref;
                return;
            }
        }

        showModal();
        //var _aid = $("#aid599").val();
        //goBuy(_aid);
    });

    //手机验证窗口中的购买
    _modalBuy.click(function () {

        _hideModalErr();

        var realname = $("#userName").val().replace(" ", "");
        if (realname.length == 0) {
            _showModalErr("请如实填写您的姓名");
            //alert("请如实填写您的姓名");
            return;
        }

        if (userid == "0") {

            //手机号码输入验证
            var userPhone = "";
            var vCode = "";
            if (userid == "0") {
                userPhone = $("#userPhone").val();
                if (!isMobile(userPhone)) {
                    _showModalErr("请输入有效的手机号");
                    //alert("请输入有效的手机号");
                    $("#userPhone").focus();
                    return;
                }

                vCode = $("#vCode").val();
                if (vCode == "") {
                    _showModalErr("请输入短信验证码");
                    //alert("请输入短信验证码");
                    $("#vCode").focus();
                    return;
                }
            }

            $.post(verifyNewUserUrl, {
                action: 'check',
                number: userPhone,
                code: vCode,
                CID: $("#hidCurUserCID").val()
            }, 'json').then(function (r) {
                if (r.ok == "1") {
                    goBuy(r.userid, realname);
                }
                else {
                    _showModalErr("短信验证码有误");
                    //alert("短信验证码有误");
                }
            });
        }
        else {
            goBuy(userid, realname);
        }

    });

    //验证码操作
    $(".vCodeBtn").click(function () {

        _hideModalErr();

        var userPhone = $("#userPhone").val();
        if (!isMobile(userPhone)) {
            _showModalErr("请输入有效的手机号");
            //alert('请输入有效的手机号');
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
                    _showModalErr("系统错误");
                    //alert('系统错误');
                }
            }).fail(function () {
                self._stopTimer(lock);
                _showModalErr("网络请求失败");
                //alert('网络请求失败');
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

    //加载更多专享套餐
    var loadUrl = "/App/More_AlbumPackages";

    //VIP专享套餐默认当前周边
    geoScopeType = 3;

    var $win = $(window);
    var isload = true;

    $win.on('scroll', function () {
        var tagTop = $(".more-packages-foot").offset().top;
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        if (winTop >= tagTop - winHeight - 100) {
            loadMorePackages(false);
        }
    });

    var start = 0;
    var count = 6;

    //load
    loadMorePackages = function (isfirst) {
        //return;
        if (isload) {

            isload = false;

            //下一页
            start += count;
            if (isfirst) { start = 0; }

            $.get(loadUrl, { userid: userid, s: start, c: count, albumId: albumId, t: t, userlat: userlat, userlng: userlng, geoScopeType: geoScopeType, districtid: districtId }, function (htmls) {
                if (htmls) {

                    if (htmls != "" && htmls.indexOf("ul") >= 0) {

                        if (isfirst) {
                            $(".scrollpageloading").show();
                            $(".more-seat-slider").hide();
                            $(".more-packages").html("");
                            $("html,body").animate({ scrollTop: 0 }, 300);
                        }

                        $(".more-packages").html($(".more-packages").html() + htmls);

                        $("img").lazyload({
                            threshold: 20,
                            placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-16x9.png",
                            effect: "show"
                        });
                    }
                    else {
                        //如果是第一页，则不显示“没有更多了”，不然很奇怪
                        if (!isfirst) {
                            $(".scrollpageloading").html("<div>没有更多了</div>");
                        }
                    }

                    isload = true;
                }
            });
        }
    }
    setTimeout(function () { loadMorePackages(true); }, 100);
});

var openMagiCall = function () {

    if (isApp) {
        gourl("whotelapp://www.zmjiudian.com/MagiCall");
    }
    else {
        gourl("http://www.zmjiudian.com/Account/WxMenuTransfer?menu=4");
    }

}

var goVipHotelList = function () {
    $("html,body").animate({ scrollTop: $(".more-packages-tit").offset().top }, 300);
}

//弹出手机号/真实姓名验证窗口
var _modalBg = $("._modal-bg");
var _modalPanel = $("._modal-panel");
var _modalPanelClose = $("._modal-panel ._close");
var _modalErr = $("._modal-panel ._body ._err");
var _modalBuy = $("._modal-panel ._btns ._buy");

//show
var showModal = function () {
    ctrlBottom.hide();
    _modalBg.show();
    _modalPanel.show();
}

//hide
var hideModal = function () {
    ctrlBottom.show();
    _hideModalErr();
    _modalBg.hide();
    _modalPanel.hide();
}

//close modal
_modalPanelClose.click(hideModal);
_modalBg.click(hideModal);

var _showModalErr = function (msg) {
    _modalErr.html(msg);
    _modalErr.fadeIn();
}
var _hideModalErr = function () {
    _modalErr.html("");
    _modalErr.hide();
}

//购买支付
var goBuy = function (userid, realname) {

    var subdic = {};
    subdic["aid"] = aid;
    subdic["atype"] = $("#atype").val();
    subdic["pid"] = $("#pid").val();
    subdic["pricetype"] = $("#pricetype").val();
    var sellnum = $("#sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
    subdic["paynum"] = sellnum;
    subdic["userid"] = userid;
    subdic["realname"] = realname;

    if (!subdic["userid"] || subdic["userid"] == "0") {
        alert("非法请求");
        return;
    }

    $.get('/Coupon/SubmitVipConpon', subdic, function (content) {
        var msg = content.Message;
        var suc = content.Success;
        var url = content.Url;

        switch (suc) {
            case "0":
                {
                    hideModal();
                    location = url;
                    break;
                }
            default:
                {
                    alert(msg);
                    break;
                }
        }
    });
}

function loginCallback(_userid) {

    //处理输入姓名和手机号的弹窗模式
    if (_userid > 0) {

        userid = _userid;

        $.get('/Coupon/GetUserPhone', { userid: _userid }, function (content) {
            if (content) {
                $(".vcode-line").hide();
                $(".phone-line").addClass("onlyone-line");
                $(".vcode-line").hide();

                $("#userPhone").val(content);
                $("#userPhone").addClass("userPhone2").removeClass("userPhone");
                $("#userPhone").attr("disabled", "disabled");

                showModal();
            }
            else {
                alert("登录错误，请退出该页面后重试")
            }
        });
    }

    //gosubmit(userid);
    //location.reload();
    //location.replace(location.pathname + "?userid=" + userid);
}

function goto(param) {
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (!isApp) {
        url = "http://www.zmjiudian.com/" + param;
    }

    this.location = url;
}

function gotopage(param) {
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    if (!isApp) {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

var _showCity = false;
var showCity = function () {
    if (!_showCity) {
        showCityFun();
    }
    else {
        hideCityFun();
    }
}