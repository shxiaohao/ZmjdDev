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

    if (isapp) checkLogin();

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

    //大人数量加减
    $(".daren-btn0").click(function () {

        var num = $(".darenNum").val();
        if (num == "" || isNaN(num)) {
            num = 1;
        }
        num = parseInt(num);
        if (num > 1) num--;
        $(".darenNum").val(num);
    });
    $(".daren-btn1").click(function () {

        var num = $(".darenNum").val();
        if (num == "" || isNaN(num)) {
            num = 0;
        }
        num = parseInt(num);
        num++;
        $(".darenNum").val(num);
    });
    $(".darenNum").change(function () {

        var num = $(this).val();
        if (num == "" || isNaN(num) || parseInt(num) < 1) {
            $(this).val(1);
        }
    });

    //小孩数量加减
    $(".child-btn0").click(function () {

        var num = $(".childNum").val();
        if (num == "" || isNaN(num)) {
            num = 1;
        }
        num = parseInt(num);
        if (num >= 1) num--;
        $(".childNum").val(num);

        changeChildOldInput();
    });
    $(".child-btn1").click(function () {

        var num = $(".childNum").val();
        if (num == "" || isNaN(num)) {
            num = 0;
        }
        num = parseInt(num);
        num++;
        $(".childNum").val(num);

        changeChildOldInput();
    });
    $(".childNum").change(function () {

        var num = $(this).val();
        if (num == "" || isNaN(num) || parseInt(num) < 0) {
            $(this).val(1);
        }

        changeChildOldInput();
    });
    //增减小孩年龄输入项
    var lastSetChildCounnt = 1;
    function changeChildOldInput()
    {
        var childCount = parseInt($(".childNum").val());
        if (childCount != lastSetChildCounnt) {
            
            //保留旧的入住人
            var childOldList1 = $(".childOldtxt");

            //生成新的
            lastSetChildCounnt = childCount;
            var temDef = $("#childold-def-template").html();
            var temNolabel = $("#childold-nolabel-template").html();
            var htmls = "";
            for (var i = 0; i < childCount; i++) {

                var nobottom = ""; if (i + 1 == childCount) { nobottom = "noBottomLine"; }
                var childNumber = i + 1;
                var temHtml = temDef; if (i > 0) { temHtml = temNolabel; }
                temHtml = temHtml.replace("{NOBOTTOMLINE}", nobottom).replace("{CHILDNUMBER}", childNumber);

                htmls += temHtml;
            }
            $("#childOldPanel").html(htmls);

            //将之前填的值设置回去
            var childOldList2 = $(".childOldtxt");
            for (var i = 0; i < childOldList2.length; i++) {
                var childOldItem = $(childOldList2[i]);
                if (childOldItem && childOldList1 && childOldList1.length > i && $(childOldList1[i]).val().length > 0) {
                    childOldItem.val($(childOldList1[i]).val());
                }
            }
        }
    }

    //套数加减
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

    function showTip(mes)
    {
        $(".priceAlertTip .tipinfo").html(mes);
        $(".priceAlertTip").fadeIn(500);
        setTimeout(function () {
            $(".priceAlertTip").fadeOut(500);
        }, 5000);
    }

    function showTip2(mes) {
        $(".pubAlertTip .tipinfo").html(mes);
        $(".pubAlertTip").fadeIn(500);
        setTimeout(function () {
            $(".pubAlertTip").fadeOut(500);
        }, 5000);
    }

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
        //基础验证
        var trueName = $("#tureName").val();
        if (trueName == "") {
            alert("请填写入住人姓名");
            $("#tureName").focus();
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
                    gosubmit(trueName, r.userid);

                    //本地缓存手机号
                    localStorecontact(userPhone);
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

            gosubmit(trueName, $("#userid").val());
        }
    });

    var gosubmit = function (trueName, userid)
    {
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

                        //入住人
                        subdic["trueName"] = trueName;

                        //人员结构
                        subdic["personnelStatus"] = $(".darenNum").val() + "," + $(".childNum").val();

                        //小孩年龄信息
                        var childOldInfo = "";
                        var childOldList = $(".childOldtxt");
                        for (var i = 0; i < childOldList.length; i++) {
                            var childOldItem = $(childOldList[i]);
                            if (childOldInfo != "") { childOldInfo += ","; }
                            var oldVal = childOldItem.val();
                            if (oldVal == "") {
                                alert("小孩年龄信息不完整");
                                return;
                            }
                            childOldInfo += oldVal;
                        }
                        subdic["childOldInfo"] = childOldInfo;

                        $.get('/Coupon/SubmitGroupConpon', subdic, function (content) {
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

    //查看更多已购买用户
    $(".openMoreBuyList").click(function ()
    {
        var thisObj = $(this);
        var opened = thisObj.data("opened");
        if (opened == "0") {
            $("#buyUserList").removeClass("hiddenList");
            $(this).html("收起<span class='arrow'>&and;</span>").data("opened", "1");
        }
        else {
            $("#buyUserList").addClass("hiddenList");
            $(this).html("查看更多<span class='arrow'>&or;</span>").data("opened", "0");
        }
    });
});