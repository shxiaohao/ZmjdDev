$(function () {

    //go reg page
    $(".explain-btn-reg").click(function () {
        location.href = "/inspector/register?userid=" + $(this).data("userid");
    });

    //to register
    $(".submit").click(function () {

        showSpinner(true);

        var isapp = $(this).data("isapp");
        var userid = $(this).data("userid");
        var name = $("#regname").val();
        var tell = $("#regtell").val();
        var mail = "";

        var dic = {};
        dic["userid"] = "";
        dic["name"] = name;
        dic["tell"] = tell;
        dic["mail"] = mail;
        
        if (!checkRegInsInput(dic)) {
            showSpinner(false);
            return;
        }

        //在app里不需要验证手机号了
        if (isapp == "True" || isapp == "true") {
            realyGoSub(dic, userid);
        }
        else {
            if (contacts.getVCode() == "") {
                showSpinner(false);
                bootbox.alert({ message: "<div class='alert-rulesmsg'>请输入手机验证码</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
                return false;
            }

            //检查验证码
            $.post(verifyUrl, {
                action: 'check',
                number: tell,
                code: contacts.getVCode()
            }, 'json').then(function (r)
            {
                if (r.ok)
                {
                    //根据当前会员之前的报名进展，处理下一步操作
                    realyGoSub(dic, userid);
                }
                else
                {
                    showSpinner(false);
                    bootbox.alert({ message: "<div class='alert-rulesmsg'>手机验证码错误</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
                    return;
                }
            });
        }
    });
    function checkRegInsInput(dic)
    {
        if ($(".check-ckbox-link").data("checked") != "1") {
            bootbox.alert({ message: "<div class='alert-rulesmsg'>请先同意品鉴师条款</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
            return false;
        }

        if (dic["name"] == "") {
            //alert("姓名不能为空");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>请提供您的真实姓名</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
            return false;
        }

        if (dic["tell"] == "") {
            //alert("电话号码不能为空");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>电话号码不能为空</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
            return false;
        }
        else if (!phoneNumReg.test(dic["tell"])) {
            //alert("无效手机号码，请重新输入！");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>无效手机号码，请重新输入</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
            return false;
        }

        return true;
    }
    //验证会员的报名进度，并做相关后续处理
    function realyGoSub(dic, userid)
    {
        $.get('/Inspector/CheckInspector', dic, function (content) {
            showSpinner(false);

            var msg = content.Message;
            var suc = content.Success;

            //1.没有提交过申请 2.提交过但是没有写过简历 3.提交过页写过简历
            switch (suc) {
                //如果当前用户是新用户或者是只填写过基础信息的用户，则跳转打开自我介绍的界面
                case 1:
                case 2:
                    {
                        //goUserTagPage(userid);
                        showMineGroup()
                        break;
                    }
                    //如果该用户基础信息和自我介绍都已经填写，则直接跳转至报名成功
                case 3:
                    {
                        goRegisterCompleted(userid);
                        break;
                    }
            }
        });
    }
    //跳转至用户个人签名/Tag页面
    function goUserTagPage(userid)
    {
        var name = $("#regname").val();
        var tell = $("#regtell").val();
        location.href = "/Account/UserTag?userId=" + userid + "&regname=" + name + "&regtell=" + tell;
    }
    //切换显示 自我介绍 输入界面
    function showMineGroup()
    {
        $(".main-element").hide();
        $(".main-element-mine").show();
    }
    //跳转至报名成功页面
    function goRegisterCompleted(userid)
    {
        location.href = "/inspector/RegisterCompleted?userid=" + userid;
    }

    //提交完整的注册信息
    function sub()
    {
        var isapp = $(this).data("isapp");
        var userid = $(this).data("userid");
        var name = $("#regname").val();
        var tell = $("#regtell").val();
        var mail = "";

        var mineJob = $("#reg-mine-job").val();

        //自我介绍不能为空
        if (mineJob.replace(/\s+/g, "").replace(/\s+/g, " ").length < 1) {

            showSpinner(false);

            bootbox.alert({
                message: "<div class='alert-rulesmsg'>您还没有自我介绍哦</div>",
                buttons: {
                    ok: {
                        label: '知道了',
                        className: 'btn-default'
                    }
                },
                callback: function (result) {

                },
                closeButton: false
            });
            return;
        }

        var dic = {};
        dic["name"] = name;
        dic["tell"] = tell;
        dic["mail"] = mail;

        dic["mineJob"] = mineJob;

        $.get('/Inspector/RegisterInspector', dic, function (content) {

            showSpinner(false);

            var msg = content.Message;
            var suc = content.Success;

            switch (suc) {
                case 0:
                {
                    goRegisterCompleted(userid);
                    break;
                }
                default:
                    {
                        bootbox.alert({
                            message: "<div class='alert-rulesmsg'>抱歉，提交失败</div>",
                            buttons: {
                                ok: {
                                    label: '确认',
                                    className: 'btn-default'
                                }
                            },
                            callback: function (result) {

                            },
                            closeButton: false
                        });
                        break;
                    }
            }
        });
    }
    $(".mine-submit").click(sub);

    var contacts = {
        init: function () {
            var self = this;
            this._phone = $("#regtell");
            this._vcode = $("#regccode");
            this._vbutton = $(".checkbtn");
            this._vbutton.on('click', function () {
                var number = self._phone.val();
                if (!phoneNumReg.test(number)) {
                    bootbox.alert({ message: "<div class='alert-rulesmsg'>无效手机号码，请重新输入</div>", buttons: { ok: { label: '确认', className: 'btn-default box-big-btn' } }, closeButton: false });
                    return;
                }
                verify.send(number, function (lock) {
                    self._vbutton.toggleClass('disabled', lock).prop('disabled', lock);
                    if (!lock) {
                        self._vbutton.text('重发验证码');
                    }
                }, function (seconds) {
                    self._vbutton.text(seconds + '秒后重发');
                });
            });
        },
        getVCode: function () {
            return $.trim(this._vcode.val());
        }
    };
    var verifyUrl = window.httpsWebUrl + 'Account/Verify';
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
        _startTimer: function (lock, update) {
            //debugger;
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
    contacts.init();

    //show rules more
    var rulemoreinfo = "";
    $("#rule-more-link").click(function () {

        if (rulemoreinfo == "") {
            rulemoreinfo = $("#rule-moreinfo-text-template").html();
        }

        if ($("#rule-moreinfo-text").html() == "") {
            $("#rule-moreinfo-text").html(rulemoreinfo);
        }

        var opens = $(this).data("opens");
        if (opens == "0") {
            $("#rule-moreinfo-text").show();
            $(this).data("opens", "1");
            $(".rule-moreinfo").addClass("rules-moreinfo-borders");

            $("html,body").animate({ scrollTop: $(".rule-moreinfo").offset().top - 5 }, 300);
        }
        else {
            $("#rule-moreinfo-text").hide();
            $(this).data("opens", "0");
            $(".rule-moreinfo").removeClass("rules-moreinfo-borders");
        }
    });

    //“注册页”-“我已阅读 品鉴师说明 ...” 勾选
    $(".check-ckbox-link").click(function () {
        var cked = $(this).data("checked");
        if (cked == "1")
        {
            $(this).data("checked", "0");
            $(this).find("img").attr("src", "/Content/images/inspector/201512/rule-ck0.png");
            $(".check-rule-panel").addClass("unckcss");
        }
        else
        {
            $(this).data("checked", "1");
            $(this).find("img").attr("src", "/Content/images/inspector/201512/rule-ck1.png");
            $(".check-rule-panel").removeClass("unckcss");
        }
    });

    /**************报名成功页-写点评**************/
    $(".regcompleted-writecom-btn").click(function ()
    {
        var uid = parseInt($("#useridtxt").val());
        goWriteComment(uid);
    });

    /**************招募品鉴师-写点评**************/
    $(".recins-writecom-btn").click(function()
    {
        var uid = parseInt($("#useridtxt").val());
        goWriteComment(uid);
    });
});

//apply inspector
var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
var lastapply;
$(".hotellist-current .btn-apply-normal").each(function () {

    var userid = $("#userid").val();

    $(this).click(function () {
        lastapply = $(this);

        goApplyInspector(userid);

        ////当不是在app中打开的时候，则强制在app中打开（没有去下载）
        //var isapp = $("#isapp").val() == "1";
        //if (!isapp) {
        //    gotopage("inspector/hotellist?userid={userid}");
        //    return;
        //}

        //location.href = loginapphref;
    });
});

function goApplyInspector(userid) {
    showSpinner(true);

    //userid = 4512064;
    if (!lastapply) { showSpinner(false); return; }
    var insid = lastapply.data("id");
    var bs = lastapply.data("bs");
    var dic = {};
    dic["hotelid"] = insid;
    dic["userid"] = userid;

    $.get('/Inspector/ApplyInsHotel', dic, function (content) {
        showSpinner(false);

        var msg = content.Message;
        var suc = content.Success;

        switch (suc) {
            case "0": {
                location.href = "/inspector/hotel/" + insid + "/" + userid + "?_newpage=1";
                break;
            }
            case "1": {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>" + msg + "</div>",
                    buttons: {
                        ok: {
                            label: '确定',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {
                        
                    },
                    closeButton: false
                });
                break;
            }
            case "2": {
                bootbox.confirm({
                    message: "<div class='alert-rulesmsg'>抱歉，您还不是品鉴师</div>",
                    buttons: {
                        confirm: { label: '知道了', className: 'btn-default box-big-btn' },
                        cancel: { label: '如何成为品鉴师', className: 'btn-default box-big-btn box-btn-bottomborder' }
                    },
                    callback: function (result) {
                        if (result) {

                        } else {
                            location.href = "/inspector/rules?userid=" + userid + "?_newpage=1";
                        }
                    },
                    closeButton: false,
                    onEscape: function () { }
                });
                break;
            }
            case "3": {
                bootbox.confirm({
                    message: "<div class='alert-rulesmsg'>您尚未申请品鉴师</div>",
                    buttons: {
                        confirm: { label: '取消', className: 'btn-default box-big-btn' },
                        cancel: { label: '去报名', className: 'btn-default box-big-btn box-btn-bottomborder' }
                    },
                    callback: function (result) {
                        if (result) {

                        } else {
                            location.href = "/inspector/explain?userid=" + userid + "?_newpage=1";
                        }
                    },
                    closeButton: false,
                    onEscape: function () { }
                });
                break;
            }
            case "4": {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>您的积分不足，请重新选择酒店</div>",
                    buttons: {
                        ok: {
                            label: '确定',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {

                    },
                    closeButton: false
                });
                break;
            }
        }
    });
}

//招募品鉴师&报名成功页-写点评
function goWriteComment(uid)
{
    if (uid <= 0)
    {
        //必须先登录
        location.href = loginapphref;
    }
    else
    {
        //验证当前app是否为最新版（大于等于4.0）
        var verIsOk = $("#verIsOk").val();
        if (verIsOk != "1")
        {
            var apptype = $("#apptype").val();

            //这里要判断当前是ios还是android，分别给出不同的提示
            if (apptype == "iosapp") {
                bootbox.confirm({
                    buttons: {
                        confirm: { label: '以后再说', className: 'btn-default btn-default0 box-big-btn' },
                        cancel: { label: '现在更新', className: 'btn-default box-big-btn box-btn-bottomborder' }
                    },
                    message: "<div class='alert-rulesmsg'>升级新版 参与活动</div>",
                    callback: function (result) {
                        if (result) {

                        } else {
                            location.href = "http://app.zmjiudian.com";
                        }
                    },
                    closeButton: false,
                    onEscape: function () { }
                });
            }
            else if (apptype == "android") {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>升级到新版APP写点评<br />复制 http://app.zmjiudian.com<br />到浏览器打开</div>",
                    buttons: {
                        ok: {
                            label: '确认',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {
                        
                    },
                    closeButton: false
                });
            }
            return;
        }

        showSpinner(true);

        //验证当前用户是否使用的最新版、是否已经报名品鉴师
        var dic = {};
        dic["userid"] = uid;
        dic["name"] = "";
        dic["tell"] = "";
        dic["mail"] = "";

        $.get('/Inspector/CheckInspector', dic, function (content) {

            showSpinner(false);

            var msg = content.Message;
            var suc = content.Success;

            //1.没有提交过申请 2.提交过但是没有写过简历 3.提交过页写过简历
            switch (suc) {
                //如果当前用户是新用户或者是只填写过基础信息的用户，则跳转打开自我介绍的界面
                case 1:
                    {
                        bootbox.confirm({
                            buttons: {
                                confirm: { label: '知道了', className: 'btn-default btn-default0 box-big-btn' },
                                cancel: { label: '去报名', className: 'btn-default box-big-btn box-btn-bottomborder' }
                            },
                            message: "<div class='alert-rulesmsg'>尚未报名提交品鉴师身份</div>",
                            callback: function (result) {
                                if (result) {

                                } else {
                                    location.href = "/inspector/register?userid=" + uid;
                                }
                            },
                            closeButton: false,
                            onEscape: function () { }
                        });
                        break;
                    }
                default:
                    {
                        //去写点评
                        goto('personal/comments');
                        break;
                    }
            }
        });
    }
}