
$(function () {
    if (Store.Get("rememberPassword") == true)
    {
        $("#OperatorName").val(Store.Get("OperatorName"));
        $("#PassWord").val(Store.Get("PWD"));
        document.getElementById("rememberPassWord").checked = true;
        //$("#rememberPassWord").checked = true;
    }
    if (Store.Get("SignIn") == true && auto == "true") {
        $("#SignIn").attr("checked", true)
        //$("#SignIn").checked = true;
        //document.getElementById("SignIn").checked = true;
        if (Store.Get("OperatorName") != null && Store.Get("PWD") != null) {
            var name = Store.Get("OperatorName");
            var pwd = Store.Get("PWD");
            $.ajax({
                type: "POST",
                url: "/Shop/Login",
                data: { OperatorName: name, PassWord: pwd },
                success: function (result) {
                    if (result.Success == 1) {
                        window.location.href = "/Shop/Coupon";
                    }
                    else if (result.Success == 0) {
                        $("#pwdError").attr("style", "float:left;font-size: 10px;color: red;display:block;");
                        $("#nameError").attr("style", "display:none;");
                        $("#OperatorName").removeAttr("style");
                        $("#PassWord").attr("style", "border: 1px solid red;");
                    }
                    else if (result.Success == -1)
                    {
                        $("#nameError").attr("style", "float:left;font-size: 10px;color: red;display:block;");
                        $("#OperatorName").attr("style", "border: 1px solid red;");
                        $("#pwdError").attr("style", "display:none;");
                        $("#PassWord").removeAttr("style");
                    }
                },
                error: function () {
                    alert("请联系管理员");
                }
            })
        }
    }
})
function Login()
{
    var name = $("#OperatorName").val();
    var pwd = $("#PassWord").val();
    if (name == "" || name == null)
    {
        alert("账号不能为空！");
        return false;
    }
    if (pwd == "" || pwd == null) {
        alert("密码不能为空！");
        return false;
    }
    var rememberPassword = $("#rememberPassWord").is(':checked');
    var SignIn = $("#SignIn").is(':checked');
    if (rememberPassword == true)
    {
        Store.Set("rememberPassword", true);
    }
    if (SignIn == true) {
        Store.Set("SignIn", true);
    }
    $.ajax({
        type: "POST",
        url: "/Shop/Login",
        data: { OperatorName: name, PassWord: pwd },
        success: function (result) {
            if (result.Success == 1)
            {
                if (SignIn == true || rememberPassword == true) {
                    Store.Set("OperatorName", result.OperatorName);
                    Store.Set("PWD", result.PassWord);
                }
                window.location.href = "/Shop/Coupon";
            }
            else if (result.Success == 0) {
                $("#pwdError").attr("style", "float:left;font-size: 10px;color: red;display:block;");
                $("#nameError").attr("style", "display:none;");
                $("#OperatorName").removeAttr("style");
                $("#PassWord").attr("style", "border: 1px solid red !important;");
            }
            else if (result.Success == -1) {
                $("#nameError").attr("style", "float:left;font-size: 10px;color: red;display:block;");
                $("#OperatorName").attr("style", "border: 1px solid red !important;");
                $("#pwdError").attr("style", "display:none;");
                $("#PassWord").removeAttr("style");
            }
        },
        error: function (XMLHttpRequest, textStatus) {
            alert(XMLHttpRequest.responseText);
        }
    })
}
