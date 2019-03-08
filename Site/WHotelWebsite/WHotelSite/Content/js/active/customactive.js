$(function () {

    //验证码操作
    $(".vCodeBtn").click(function () {
        var userPhone = $("#userPhone").val();
        if (!isMobile(userPhone)) {
            alert('请输入有效的手机号');
            return;
        }

        verify.send(userPhone, function (lock) {
            $(".vCodeBtn").toggleClass('disabled', lock).prop('disabled', lock);
            if (!lock) {
                $(".vCodeBtn").text('重新获取');
            }
        }, function (seconds) {
            $(".vCodeBtn").text(seconds + '秒重发');
        });
    });

    //window.httpsWebUrl = "/";
    var verifyAnmanUrl = window.httpsWebUrl + 'Account/VerifyAnman';
    var verify = {
        send: function (number, lock, update) {
            var self = this;
            if (self._timer) {
                return;
            }
            self._startTimer(lock, update);
            $.post(verifyAnmanUrl, {
                action: 'send',
                number: number
            }, 'json').then(function (r) {
                if (!r.ok) {
                    self._stopTimer(lock);
                    alert('系统故障，请稍后重试');
                }
            }).fail(function () {
                self._stopTimer(lock);
                alert('网络请求失败');
            })
        },
        check: function (number, code) {
            return $.post(verifyAnmanUrl, {
                action: 'check',
                number: number,
                code: code
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
    
    //reg
    $(".submit").click(function () {

        var userName = $("#userName").val();
        if (!userName || userName == "") {
            alert('请输入姓名');
            return;
        }

        var userPhone = $("#userPhone").val();
        if (!isMobile(userPhone)) {
            alert('请输入有效的手机号');
            return;
        }

        var vCode = $("#vCode").val();
        if (vCode == "") {
            alert("请输入短信验证码");
            $("#vCode").focus();
            return;
        }

        //gosubmit(userName, userPhone);
        //return;

        $.post(verifyAnmanUrl, {
            action: 'check',
            number: userPhone,
            code: vCode
        }, 'json').then(function (r) {
            if (r.ok == "1") {
                gosubmit(userName, userPhone);
            }
            else {
                alert("短信验证码输入有误");
            }
        });
    });

    var gosubmit = function (userName, userPhone) {

        var aid = $("#aid").val();

        var subdic = {};
        subdic["aid"] = aid;
        subdic["userName"] = userName;
        subdic["userPhone"] = userPhone;

        $.get('/Active/RegCustomActiveUser', subdic, function (content) {
            var msg = content.Message;
            var suc = content.Success;
            var url = content.Url;

            switch (suc) {
                case "1":
                    {
                        location.href = "/custom/active/" + aid + "/" + userPhone + "/0";
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
});