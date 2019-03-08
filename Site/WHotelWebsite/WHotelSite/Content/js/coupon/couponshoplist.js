$(function () {
    
    $(".list img").lazyload({
        threshold: 20,
        placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-3x2.png",
        effect: "show"
    });

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

    var cansell = $("#cansell").val() == "1";

    if (cansell) {
        //加减
        $(".btn0").click(function () {
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
            var num = $(this).val();
            if (num == "" || isNaN(num) || parseInt(num) < 1) {
                $(this).val(1);
            }

            //得出小计
            setxiaoji();

            //验证购买数量
            checkBuyNum();
        });
    }
    else
    {

    }

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

    //购买
    $(".submit").click(function ()
    {
        var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
        if (pub_userid == "0")
        {
            location.href = loginapphref;
            return;
        }

        var dic = {};
        dic["id"] = $("#aid").val();
        var num = $(".sellnum").val(); if (num == "" || isNaN(num) || parseInt(num) < 1) num = 1;
        dic["buynum"] = num;
        dic["userid"] = $("#userid").val();

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
                        showTip(message);
                        $(".sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
                case "2":
                    {
                        showTip(message);
                        $(".sellnum").val(cansell);
                        setxiaoji();
                        break;
                    }
                case "3":
                    {
                        showTip(message);
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
                        subdic["userid"] = $("#userid").val();

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
                                        showTip(msg);
                                        break;
                                    }
                            }
                        });
                        break;
                    }
            }

        });
    });
});

function gourl(url) {
    location.href = url;
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
                    var h = Math.floor(t / 1000 / 60 / 60 % 24);
                    var m = Math.floor(t / 1000 / 60 % 60);
                    var s = Math.floor(t / 1000 % 60);

                    var timehtml =
                        d <= 0 ? (
                        h <= 0
                        ? "距离开抢还有" + (m > 0 ? (m < 10 ? "0" + m : m) + "分" : "") + (s < 10 ? "0" + s : s) + "秒"
                        : "距离开抢还有" + (h < 10 ? "0" + h : h) + "小时" + (m < 10 ? "0" + m : m) + "分"
                        ) : "距离开抢还有" + d + "天";

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
                        (h <= 0 ? "还有" + (m < 10 ? "0" + m : m) + "分钟闪购结束"
                        : "还有" + (h < 10 ? "0" + h : h) + "小时" + (m < 10 ? "0" + m : m) + "分钟闪购结束")
                        : "还有" + (d < 10 ? "0" + d : d) + "天" + "闪购结束";

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
            },
            stopCloseAction: function () {
                this.closeTimerState = false;
                this.timerEntity.html("已结束");
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