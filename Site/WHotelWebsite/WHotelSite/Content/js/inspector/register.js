function loadUserTell(userid) {
    var dic = {};
    dic["user"] = userid;
    $.get('/Inspector/GetUserTel', dic, function (content) {
        var tel = content;
        var regtel_div = $("#regtel_div");
        var regsend_div = $("#regsend_div");
        var regcode_li = $("#regcode_li");

        //如果存在手机号返回，则直接显示手机号，然后灰掉
        if (tel != null && tel != "") {
            $("#regtell").val(tel).attr("readonly", "readonly");
            $("#regtel_div").css("width", "100%");
            $("#regsend_div").hide();
            $("#regcode_li").hide();
        }
    });
}

window.onload = function () {
    //var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
    //var isapp = $("#isapptxt").val() == "1";
    //var telIsNull = $("#regtell").val() == "";

    ////当是在app的里面，并且手机号又为空的时候，需要动态去获取用户手机号
    //if (isapp && telIsNull) {
    //    location.href = loginapphref;
    //}
};