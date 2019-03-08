$(function () {

    //go reg page
    $(".explain-btn-reg").click(function () {
        location.href = "/inspector/register?userid=" + $(this).data("userid");
    });

    //to register
    $(".submit").click(function () {
        var userid = $(this).data("userid");
        var name = $("#regname").val();
        var tell = $("#regtell").val();
        var mail = $("#regmail").val();

        var dic = {};
        dic["name"] = name;
        dic["tell"] = tell;
        dic["mail"] = mail;

        if (!checkRegInsInput(dic)) {
            return;
        }

        $.get('/Inspector/RegisterInspector', dic, function (content) {
            var msg = content.Message;
            var suc = content.Success;

            //alert(suc);
            //alert(msg);
            var ruleshtml = "<a href='/inspector/rules'><div class='alert-ruleslink'>了解品鉴师</div></a>";

            switch (suc) {
                case 0: {
                    bootbox.confirm({
                        buttons: {
                            confirm: { label: '去挑选免费品鉴的酒店', className: 'btn-default box-big-btn' },
                            cancel: { label: '分享点评 ', className: 'btn-default hide' }
                        },
                        message: "<div class='alert-rulesmsg'>您好，" + dic["name"] + "<p class='h'>恭喜您荣获品鉴师资格</p></div>",
                        callback: function (result) {
                            if (result) {
                                location.href = "/inspector/hotellist?userid=" + userid;
                                //gotopage("inspector/hotellist?userid=" + userid);
                            } else {
                                goto("personal/comment?isuncomment=False");
                            }
                        },
                        closeButton: false,
                        onEscape: function () { }
                    });
                    break;
                }
                case 1: {
                    bootbox.confirm({
                        buttons: {
                            confirm: { label: '去订酒店', className: 'btn-default' },
                            cancel: { label: '取消', className: 'btn-default' }
                        },
                        message: "<div class='alert-rulesmsg'>您好，" + dic["name"] + "<br>您尚未入住并点评过酒店，先订酒店试试看哦！</div>" + ruleshtml,
                        callback: function (result) {
                            if (result) {
                                goto("");
                            } else {
                                //location.href = "/inspector/rules";
                            }
                        },
                        closeButton: false,
                        onEscape: function () { }
                    });
                    break;
                }
                case 2: {
                    bootbox.confirm({
                        buttons: {
                            confirm: { label: '去订酒店', className: 'btn-default' },
                            cancel: { label: '取消', className: 'btn-default' }
                        },
                        message: "<div class='alert-rulesmsg'>您好，" + dic["name"] + "<br>" + msg + "</div>" + ruleshtml,
                        callback: function (result) {
                            if (result) {
                                goto("");
                            } else {
                                //location.href = "/inspector/rules";
                            }
                        },
                        closeButton: false,
                        onEscape: function () { }
                    });
                    break;
                }
                case 3: {
                    bootbox.confirm({
                        buttons: {
                            confirm: { label: '写点评', className: 'btn-default' },
                            cancel: { label: '取消', className: 'btn-default' }
                        },
                        message: "<div class='alert-rulesmsg'>您好，" + dic["name"] + "<br>" + msg + "</div>" + ruleshtml,
                        callback: function (result) {
                            if (result) {
                                goto("personal/comment?isuncomment=True");
                            } else {
                                //location.href = "/inspector/rules";
                            }
                        },
                        closeButton: false,
                        onEscape: function () { }
                    });
                    break;
                }
                case 4: {
                    bootbox.confirm({
                        buttons: {
                            confirm: { label: '去挑选免费品鉴的酒店', className: 'btn-default box-big-btn' },
                            cancel: { label: '分享点评 ', className: 'btn-default hide' }
                        },
                        message: "<div class='alert-rulesmsg'>您好，" + dic["name"] + "<br>您已经是品鉴师了。</div>" + ruleshtml,
                        callback: function (result) {
                            if (result) {
                                location.href = "/inspector/hotellist?userid=" + userid;
                                //gotopage("inspector/hotellist");
                            } else {
                                goto("personal/comment?isuncomment=False");
                            }
                        },
                        closeButton: false,
                        onEscape: function () { }
                    });
                    break;
                }
            }
        });
    });
    function checkRegInsInput(dic)
    {
        var ruleshtml = "<a href='/inspector/rules'><div class='alert-ruleslink'>了解品鉴师</div></a>";
        //bootbox.confirm({
        //    buttons: {
        //        confirm: { label: '挑选品鉴酒店', className: 'btn-default' },
        //        cancel: { label: '分享点评 ', className: 'btn-default' }
        //    },
        //    message: "<div class='alert-rulesmsg'>您好，" + dic["name"] + "<br>您已经是品鉴师了。</div>" + ruleshtml,
        //    callback: function (result) {
        //        if (result) {
        //            location.href = "/inspector/hotellist?userid=" + userid;
        //            //gotopage("inspector/hotellist");
        //        } else {
        //            goto("personal/comment?isuncomment=False");
        //        }
        //    },
        //    closeButton: false,
        //    onEscape: function () { }
        //});
        //return;

        if (dic["name"] == "") {
            //alert("姓名不能为空");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>姓名不能为空</div>", closeButton: false });
            return false;
        }

        if (dic["tell"] == "") {
            //alert("电话号码不能为空");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>电话号码不能为空</div>", closeButton: false });
            return false;
        }
        else if (!phoneNumReg.test(dic["tell"])) {
            //alert("无效手机号码，请重新输入！");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>无效手机号码，请重新输入！</div>", closeButton: false });
            return false;
        }

        if (dic["mail"] == "") {
            //alert("邮箱地址不能为空");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>邮箱地址不能为空</div>", closeButton: false });
            return false;
        }
        else if (!mailReg.test(dic["mail"])) {
            //alert("无效邮箱地址，请重新输入！");
            bootbox.alert({ message: "<div class='alert-rulesmsg'>无效邮箱地址，请重新输入！</div>", closeButton: false });
            return false;
        }

        return true;
    }
});

//apply inspector
var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
var lastapply;
$(".hotellist-current .btn-apply-normal").each(function () {

    var userid = $("#userid").val();

    $(this).click(function () {
        lastapply = $(this);

        //当不是在app中打开的时候，则强制在app中打开（没有去下载）
        var isapp = $("#isapp").val() == "1";
        if (!isapp) {
            gotopage("inspector/hotellist?userid=" + userid);
            return;
        }

        location.href = loginapphref;
        //loginCallback("4512064");
        //loginCallback("4512004");
    });
});

function goApplyInspector(userid) {
    if (!lastapply) return;
    var hotelid = lastapply.data("id");
    var dic = {};
    dic["hotelid"] = hotelid;
    dic["userid"] = userid;

    $.get('/Inspector/ApplyInsHotel', dic, function (content) {
        var msg = content.Message;
        var suc = content.Success;

        switch (suc) {
            case "0": {
                var checkIn = content.CheckIn;
                var checkOut = content.CheckOut;
                location.href = "/inspector/hotel/" + hotelid + "/" + userid + "?checkIn=" + checkIn + "&checkOut=" + checkOut;
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
                //location.href = loginapphref;
                //bootbox.alert({ message: msg, closeButton: false });
                break;
            }
            case "2": {
                bootbox.confirm({
                    buttons: {
                        confirm: { label: '去报名', className: 'btn-default' },
                        cancel: { label: '取消 ', className: 'btn-default' }
                    },
                    message: "<div class='alert-rulesmsg'>" + msg + "</div>",
                    callback: function (result) {
                        if (result) {
                            location.href = "/inspector/explain";
                        } else {
                        }
                    },
                    closeButton: false,
                    onEscape: function () { }
                });
                break;
            }
        }
    });
}