$(document).ready(function () {

    //验证码操作
    $("#vCodeBtn").click(function ()
    {
        var userPhone = $("#userPhone").val();
        if (!isMobile(userPhone)) {
            alert('请输入有效的手机号');
            return;
        }

        $("#regcode_li").show();

        verify.send(userPhone, function (lock) {
            $("#vCodeBtn").toggleClass('disabled', lock).prop('disabled', lock);
            if (!lock) {
                $("#vCodeBtn").text('重发验证码');
            }
        }, function (seconds) {
            $("#vCodeBtn").text(seconds + '秒后重发');
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
                    alert('系统故障，请稍后重试');
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

        $.post(verifyNewUserUrl, {
            action: 'check',
            number: userPhone,
            code: vCode,
            CID: $("#hidCurUserCID").val()
        }, 'json').then(function (r) {
            if (r.ok == "1") {
                gosubmit(userPhone);
            }
            else {
                alert("短信验证码输入有误");
            }
        });
    });

    var gosubmit = function (userPhone)
    {
        var userCode = $("#userCode").val();
        var channel = $("#channel").val();
        var tag = $("#tag").val();

        var subdic = {};
        subdic["userCode"] = userCode;
        subdic["phone"] = userPhone;
        subdic["channel"] = channel;
        subdic["tag"] = tag;

        $.get('/MagiCall/SubmitBind', subdic, function (content) {
            var msg = content.Message;
            var suc = content.Success;
            var url = content.Url;

            switch (suc) {
                case "1":
                    {
                        alert(msg);
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