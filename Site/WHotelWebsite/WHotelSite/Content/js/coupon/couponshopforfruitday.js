$(document).ready(function () {
    
    $("img").lazyload({
        threshold: 100,
        placeholder: "http://whfront.b0.upaiyun.com/app/img/coupon/fruitday/img_loading.png",
        effect: "show"
    });

    //细则图片动态加载
    var ruleImg = $(".product-desc img");
    setImgOriSrc(ruleImg);

    var isapp = $("#isapp").val() == "1";
    var pubaid = $("#aid").val();

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

    //验证码操作
    $(".vCodeBtn").click(function ()
    {
        var userPhone = $("#userPhone").val();
        if (!isMobile(userPhone)) {
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

    //购买
    $(".gobuy").click(function ()
    {
        var realname = $("#userName").val().replace(" ", "");
        if (realname.length == 0) {
            alert("请如实填写您的姓名");
            return;
        }

        //userid 相关验证
        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
        if (pub_userid == "0")
        {
            //手机号码输入验证
            var userPhone = "";
            var vCode = "";
            if (pub_userid == "0") {
                userPhone = $("#userPhone").val();
                if (!isMobile(userPhone)) {
                    alert("请输入有效的手机号");
                    $("#userPhone").focus();
                    return;
                }

                vCode = $("#vCode").val();
                if (vCode == "") {
                    alert("请输入短信验证码");
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
                    if (r.userid == "0") {
                        location.href = loginapphref;
                        return;
                    }
                    gosubmit(r.userid, realname);
                }
                else {
                    alert("短信验证码输入有误");
                }
            });
        }
        else
        {
            if (pub_userid == "0")
            {
                location.href = loginapphref;
                return;
            }

            gosubmit($("#userid").val(), realname);
        }
    });
    var gosubmit = function (userid, realname)
    {
        var subdic = {};
        subdic["aid"] = $("#aid").val();
        subdic["atype"] = $("#atype").val();
        subdic["pid"] = $("#pid").val();
        subdic["pricetype"] = $("#pricetype").val();
        var sellnum = $("#sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
        subdic["paynum"] = sellnum;
        subdic["userid"] = userid;
        subdic["realname"] = realname;

        $.get('/Coupon/SubmitFruitDayConpon', subdic, function (content) {
            var msg = content.Message;
            var suc = content.Success;
            var url = content.Url;

            switch (suc) {
                case "0":
                    {
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
    };

    //打开细则
    $(".rules").click(function () {
        $(".product-desc-close").show();
        $(".product-desc").show();
    });

    //关闭细则
    $(".product-desc-close").click(function () {
        $(".product-desc-close").hide();
        $(".product-desc").hide();
    });

    //滚动到购买区域
    $(".jumpgobuy").click(function () {
        $("html,body").animate({ scrollTop: $(".vip-phone").offset().top - 20 }, 500);
    });

    //配置分享
    var shareBtn = $(".share-btn");
    if (shareBtn) {
        shareBtn.click(function () {
            var share_title = "果粉福利，“果”断出行";
            var share_text = "花398省1500，价值900元的五星级高端酒店任选免费住。";
            var share_url = "http://www.zmjiudian.com/custom/shop/ftd/100944#CID*160";  //?替换为# =替换为* (ftd相当low的处理方式)
            var share_image = "http://whphoto.b0.upaiyun.com/117QRbt0_small";
            var gourl = "fruitday://Share?shareUrl=" + share_url + "&shareText=" + share_text + "&shareTitle=" + share_title + "&iconUrl=" + share_image;
            //console.log(gourl);
            location.href = gourl;
        });
    }
    
});

var setImgOriSrc = function (imgObj) {
    var orisrc = imgObj.data("orisrc");
    if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
        var defsrc = imgObj.attr("src");
        imgObj.attr("src", orisrc);
        imgObj.data("orisrc", "");
        imgObj.error(function () {
            imgObj.attr("src", defsrc);
        });
    }
};