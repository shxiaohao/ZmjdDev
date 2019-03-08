$(function () {

    function regXmas()
    {
        var activeid = $("#activeid").val();
        var openid = $("#openid").val();
        var username = $("#regname").val();
        var phone = $("#regtell").val();

        if (username == "") {
            alert("请输入您的姓名");
            return;
        }

        if (phone == "") {
            alert("请输入您的手机号");
            return;
        }

        if (phone.length != 11) {
            alert("手机号码有误");
            return;
        }

        var dic = {};
        dic["openid"] = openid;
        dic["username"] = username;
        dic["phone"] = phone;
        dic["activeid"] = activeid;

        $.get('/Active/SignUpWeixinActive', dic, function (content) {
            var add = content;
            if (add == "1") {
                //成功
                location.href = "/Active/Weixin_SignupActive_RegDone/" + activeid + "?openid=" + openid;
            }
            else {
                //失败
                alert("报名失败，请重试！");
            }
        });
    }
    $(".submit").click(regXmas);
});