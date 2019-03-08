function refreshVerifyCode() {
    $("#verifyCode").attr("src", "/Account/GetIdentifyCode?t=" + new Date().valueOf());
}

function submitCode() {
    var Code = $("#identifyCode").val();
    var acurl=$("#acUrl").val();
    $.ajax({
        type: "GET",
        data: { code: Code },
        url: "/Account/CheckIdentifyCode",
        success: function (data) {
            if (data == "True") {
                window.location.href = acurl;
            }
            else {
                alert("验证码错误！");
            }
        }
    });
}