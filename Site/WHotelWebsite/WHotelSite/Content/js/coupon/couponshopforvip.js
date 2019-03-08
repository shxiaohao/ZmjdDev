$(document).ready(function () {
    
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
    $(".submit").click(function ()
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

        $.get('/Coupon/SubmitVipConpon', subdic, function (content) {
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

        //会员购买暂不需要check，直接submit
        return;

        var dic = {};
        dic["id"] = $("#aid").val();
        var num = $("#sellnum").val(); if (num == "" || isNaN(num) || parseInt(num) < 1) num = 1;
        dic["buynum"] = num;
        dic["userid"] = userid;
        $.get('/Coupon/CheckBuyNumberForVip', dic, function (result) {
            var message = result.Message;
            var success = result.Success;
            var cansell = result.CanSell;

            switch (success) {
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
                        alert(message);
                        $("#sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
                case "2":
                    {
                        alert(message);
                        $("#sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
                case "3":
                    {
                        alert(message);
                        $("#sellnum").val(cansell);
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
                        var sellnum = $("#sellnum").val(); if (sellnum == "" || isNaN(sellnum) || parseInt(sellnum) < 1) sellnum = 1;
                        subdic["paynum"] = sellnum;
                        subdic["userid"] = userid;

                        $.get('/Coupon/SubmitVipConpon', subdic, function (content) {
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
                                        alert(msg);
                                        break;
                                    }
                            }
                        });
                        break;
                    }
            }

        });
    };
});