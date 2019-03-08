/*验证常数开始*/
var verifyPasswordAgreeType = {FIND:"find",REGISTER:"register"};
/*验证常数结束*/

/*后台数据交互开始*/
//1.用户登录按钮
$("#myModal .btn-login").click(function () {
    var $phoneNumber = $('#phoneNum4Login');
    var $password = $('#password4Login');    
    
    var phoneNumber = $.trim($phoneNumber.val());
    var password = $.trim($password.val());

    if (phoneNumber === "" || password === "") {
        alert("信息不完整,请填写后再登录。");
        return false;
    }

    var $phoneNumberGroup = $phoneNumber.parents('.form-group').eq(0);
    var $passwordGroup = $password.parents('.form-group').eq(0);
    
    if ($phoneNumberGroup.hasClass("has-error") || $passwordGroup.hasClass("has-error")) {
        alert("有错误信息,请修改后再登录。");
        return false;
    }

    $phoneNumber.siblings('.infoerror').remove();
    $password.siblings('.infoerror').remove();
    $phoneNumber.siblings('.infosuccess').remove();
    $password.siblings('.infosuccess').remove();

    //if (phoneNumber === "") {
    //    $phoneNumberGroup.addClass('has-error');
    //    $phoneNumber.after('<div class="infoerror">手机号码不能为空</div>');
    //}
    //if (password === "") {
    //    $passwordGroup.addClass('has-error');
    //    $password.after('<div class="infoerror">密码不能为空</div>');
    //}
    
    var $isSaveCookie = $("#isAutoLogin").find("input").eq(0);
    var isSaveCookie = $isSaveCookie.val() === "True" ? true : false;
    var data2 = { number: phoneNumber, password: password, isSaveCookie: isSaveCookie };
    $.ajax({
        type: 'POST',
        url: '/Account/MobileLogin',
        data: data2,
        datatype: 'json',
        async: false,
        success: function (result) {
            if (result === '') {
                //window.location.href = "/";
                location.reload(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
            }
            else {

                $phoneNumberGroup.removeClass("has-success");
                $passwordGroup.removeClass("has-success");
                $phoneNumberGroup.removeClass("has-error");
                $passwordGroup.removeClass("has-error");

                if (result === '1') {
                    $phoneNumberGroup.addClass('has-success');
                    $passwordGroup.addClass('has-error');
                    $password.after('<div class="infoerror">密码有误，请重新输入</div>');
                }
                else if (result === '0') {
                    //clearLoginAndRegisterDialogInput();
                    $phoneNumberGroup.addClass('has-error');
                    $passwordGroup.addClass('has-success');
                    $phoneNumber.after('<div class="infoerror">该手机账号不存在，请重新输入</div>');
                    //alert("该手机账号不存在，请先注册！");
                }
                else {
                    clearLoginAndRegisterDialogInput();
                    $("#myModal .close").click();
                    return;
                }
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });

    var $phoneNumberinfosuccess = $phoneNumberGroup.find('.imgsuccess');
    var $phoneNumberinfoerror = $phoneNumberGroup.find('.imgerror');
    var $passwordinfosuccess = $passwordGroup.find('.imgsuccess');
    var $passwordinfoerror = $passwordGroup.find('.imgerror');

    $phoneNumberinfosuccess.removeClass("hidden");
    $phoneNumberinfoerror.removeClass("hidden");
    $passwordinfosuccess.removeClass("hidden");
    $passwordinfoerror.removeClass("hidden");
});

//2.用户注册按钮
$("#myModal2 .btn-login").click(function () {
    var $phoneNum4Register = $('#phoneNum4Register');
    var $verifyCode4Register = $('#verifyCode4Register');
    var $pwd = $("#password4Register");
    var $pwdAgain = $("#password4RegisterAgain");
    
    var phoneNum = $.trim($phoneNum4Register.val());
    var verifyCode = $.trim($verifyCode4Register.val());
    var pwd = $.trim($pwd.val());
    var pwdAgain = $.trim($pwdAgain.val());

    var $phoneNumberGroup = $phoneNum4Register.parents('.form-group').eq(0);
    var $verifyCode4RegisterGroup = $verifyCode4Register.parents('.form-group').eq(0);
    var $pwdGroup = $pwd.parents('.form-group').eq(0);
    var $pwdAgainGroup = $pwdAgain.parents('.form-group').eq(0);
    
    if ($phoneNumberGroup.hasClass("has-error") || $verifyCode4RegisterGroup.hasClass("has-error")
        || $pwdGroup.hasClass("has-error") || $pwdAgainGroup.hasClass("has-error")) {
        alert("您还有没修改的错误信息,请修改后再注册。");
        return false;
    }

    var $phoneNumberinfosuccess = $phoneNumberGroup.find('.imgsuccess');
    var $phoneNumberinfoerror = $phoneNumberGroup.find('.imgerror');
    var $verifyCode4Registerinfosuccess = $verifyCode4RegisterGroup.find('.imgsuccess');
    var $verifyCode4Registerinfoerror = $verifyCode4RegisterGroup.find('.imgerror');
    var $pwdinfosuccess = $pwdGroup.find('.imgsuccess');
    var $pwdinfoerror = $pwdGroup.find('.imgerror');
    var $pwdAgaininfosuccess = $pwdAgainGroup.find('.imgsuccess');
    var $pwdAgaininfoerror = $pwdAgainGroup.find('.imgerror');

    $("#myModal2").find(".infoerror").remove();
    $("#myModal2").find(".infosuccess").remove();

    $("#myModal2").find(".form-group").each(function () {
        var $that = $(this);
        //$that.removeClass("has-success");
        //$that.removeClass("has-error");

        $that.find(".imgsuccess").removeClass("hidden");
        $that.find(".imgerror").removeClass("hidden");
    });
    
    if (phoneNum === "") {
        $phoneNumberGroup.removeClass("has-success");
        $phoneNumberGroup.addClass('has-error');
        $phoneNum4Register.after('<div class="infoerror">手机号码不能为空</div>');
    }
    if (verifyCode === "") {
        $verifyCode4RegisterGroup.removeClass("has-success");
        $verifyCode4RegisterGroup.addClass('has-error');
        $verifyCode4Register.after('<div class="infoerror">验证码不能为空</div>');
    }
    if (pwd === "") {
        $pwdGroup.removeClass("has-success");
        $pwdGroup.addClass('has-error');
        $pwd.after('<div class="infoerror">密码不能为空</div>');
    }
    if (pwdAgain === "") {
        $pwdAgainGroup.removeClass("has-success");
        $pwdAgainGroup.addClass('has-error');
        $pwdAgain.after('<div class="infoerror">密码不能为空</div>');
    }
    if (phoneNum === "" || pwd === "" || verifyCode === "" || pwdAgain === "") {
        return false;
    }

    register(phoneNum, pwd, verifyCode, true);
    //if (verifyPasswordAgree(verifyPasswordAgreeType.REGISTER)) {
    //    register(phoneNum, pwd, verifyCode, true);
    //}
});

//3.发送短信验证码按钮
$("#myModal2 .btn-yzm1").click(function () {
    var phoneNum = $.trim($('#phoneNum4Register').val());
    if (phoneNum) {
        var jQueryTimerBtn = $("#myModal2 .btn-yzm2");
        jQueryTimerBtn.removeClass("hidden");
        $(this).addClass("hidden");

        var sizzelStr = "#verifyCode4Register";
        var sizzelStr2 = "#myModal2 .btn-yzm2";
        var $verifyCode = $(sizzelStr);

        $verifyCodeGroup = $verifyCode.parents(".form-group").eq(0);
        $verifyCodeGroup.find(".infosuccess").remove();
        $verifyCodeGroup.find(".infoerror").remove();
        $verifyCode.val("");

        sendVerifyCode(phoneNum, true);

        var intervalId = window.setInterval("changeBtnText('" + sizzelStr2 + "')", 1000);
        if (intervalId && !$("#intervalIdHidden4Register").val()) {
            $("#intervalIdHidden4Register").val(intervalId);
        }
    }
});

$("#myModal4 .btn-yzm1").click(function () {
    var phoneNum = $.trim($('#phoneNum4Password').val());
    if (phoneNum) {
        var jQueryTimerBtn = $("#myModal4 .btn-yzm2");
        var sizzelStr2 = "#myModal4 .btn-yzm2";
        jQueryTimerBtn.removeClass("hidden");
        $(this).addClass("hidden");

        var sizzelStr = "#verifyCode4Password";
        var $verifyCode = $(sizzelStr);
        var $verifyCodeGroup = $verifyCode.parents(".form-group").eq(0);
        $verifyCodeGroup.find(".infosuccess").remove();
        $verifyCodeGroup.find(".infoerror").remove();
        $verifyCode.val("");

        sendVerifyCode(phoneNum, true);

        var intervalId = window.setInterval("changeBtnText('" + sizzelStr2 + "')", 1000);
        if (intervalId && !$("#intervalIdHidden4Password").val()) {
            $("#intervalIdHidden4Password").val(intervalId);
        }
    }
});

function changeBtnText(sizzleStr) {
    var currentVal = parseInt($.trim($(sizzleStr).text()));
    currentVal--;
    if (currentVal) {
        $(sizzleStr).text(currentVal + "秒后重发");
    }
    else {
        var pwd = $("#intervalIdHidden4Password").val();
        var reg = $("#intervalIdHidden4Register").val()
        if (pwd) {
            $("#intervalIdHidden4Password").val("");
            window.clearInterval(parseInt(pwd));
        }
        if (reg) {
            $("#intervalIdHidden4Register").val("");
            window.clearInterval(parseInt(reg));
        }
        unLockSend();
    }
}

function unLockSend() {

    $("#myModal4 .btn-yzm1").removeClass("hidden");
    $("#myModal4 .btn-yzm2").addClass("hidden");
    $("#myModal4 .btn-yzm2").text("30秒后重发");

    $("#myModal2 .btn-yzm1").removeClass("hidden");
    $("#myModal2 .btn-yzm2").addClass("hidden");
    $("#myModal2 .btn-yzm2").text("30秒后重发");

    var sizzelStr = "#verifyCode4Password";
    var $verifyCode = $(sizzelStr);
    //$verifyCode.val("");

    var $verifyCodeGroup = $verifyCode.parents(".form-group").eq(0);
    $verifyCodeGroup.find(".infosuccess").remove();
    $verifyCodeGroup.find(".infoerror").remove();

    sizzelStr = "#verifyCode4Register";
    $verifyCode = $(sizzelStr);
    //$verifyCode.val("");

    $verifyCodeGroup = $verifyCode.parents(".form-group").eq(0);
    $verifyCodeGroup.find(".infosuccess").remove();
    $verifyCodeGroup.find(".infoerror").remove();
}

function sendVerifyCode(phoneNum, isAsync) {
    var data2 = { number: phoneNum, action: 'send' };
    $.ajax({
        type: 'POST',
        url: '/Account/Verify',
        data: data2,
        datatype: 'json',
        async: isAsync,
        success: function (result) {
            if (result.ok) {
                $("#verifyCode4Password").after("<div class='infosuccess'>验证码已发送，请注意查收</div>");
                $("#verifyCode4Register").after("<div class='infosuccess'>验证码已发送，请注意查收</div>");
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

//4.验证短信验证码
function checkVerifyCode(phoneNum, confirmCode, isAsync) {
    var isRight = false;
    var data2 = { number: phoneNum, code: confirmCode, action: 'check' };
    $.ajax({
        type: 'POST',
        url: '/Account/Verify',
        data: data2,
        datatype: 'json',
        async: isAsync,
        cache: false,
        success: function (result) {
            isRight = result.ok ? true : false;
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
    return isRight;
}

//5.找回密码1 验证手机号
$("#myModal3 .btn-login").click(function () {
    var $phoneNum = $("#myModal3 .phoneNum");
    var phoneNum = $.trim($phoneNum.val());
    if (!phoneNum) {
        $phoneNum.after('<div class="infoerror">手机号码不能为空！</div>');
        return false;
    }
    else if ($("#myModal3").find("div.has-error").length > 0) {
        alert("您还有未修改的错误信息");
        return false;
    }
    else {
        if (verifyPhoneNum($phoneNum)) {
            $("#myModal3").find(".infoerror").remove();
        }
    }
});

//6.找回密码2 
$("#myModal4 .btn-login").click(function () {
    var $phoneNum = $("#phoneNum4Password");
    var $newPassword = $("#newPassword");
    var $verifyCode = $("#verifyCode4Password");
    var $newPasswordAgain = $("#newPasswordAgain");

    var phoneNum = $.trim($phoneNum.val());
    var newPassword = $.trim($newPassword.val());
    var verifyCode = $.trim($verifyCode.val());
    var newPasswordAgain = $.trim($newPasswordAgain.val());

    var $phoneNumberGroup = $phoneNum.parents('.form-group').eq(0);
    var $verifyCode4ResetGroup = $verifyCode.parents('.form-group').eq(0);
    var $pwdGroup = $newPassword.parents('.form-group').eq(0);
    var $pwdAgainGroup = $newPasswordAgain.parents('.form-group').eq(0);

    if ($phoneNumberGroup.hasClass("has-error") || $verifyCode4ResetGroup.hasClass("has-error")
        || $pwdGroup.hasClass("has-error") || $pwdAgainGroup.hasClass("has-error")) {
        alert("您还有没修改的错误信息,请先修改。");
        return false;
    }
    
    if (verifyCode === "") {
        $verifyCode4ResetGroup.removeClass("has-success");
        $verifyCode4ResetGroup.addClass('has-error');
        $verifyCode.after('<div class="infoerror">验证码不能为空</div>');
    }
    if (newPassword === "") {
        $pwdGroup.removeClass("has-success");
        $pwdGroup.addClass('has-error');
        $newPassword.after('<div class="infoerror">密码不能为空</div>');
    }
    if (newPasswordAgain === "") {
        $pwdAgainGroup.removeClass("has-success");
        $pwdAgainGroup.addClass('has-error');
        $newPasswordAgain.after('<div class="infoerror">密码不能为空</div>');
    }
    if (newPassword === "" || verifyCode === "" || newPasswordAgain === "") {
        alert("信息不完整，请填写。");
        return false;
    }

    if (phoneNum) {
        var isSuccess = resetPasswordWithPhone(phoneNum, newPassword, verifyCode, false);
        if (!isSuccess) {
            alert("重置密码失败。");
            return false;
        }
    }
    else {
        alert("没有找到手机号。");
        return false;
    }
});

//8.验证手机号是否已存在
function checkMobileAccountExistance(phoneNum, isAsync) {
    var isExistence = false;
    var data2 = { number: phoneNum};
    $.ajax({
        type: 'POST',
        url: '/Account/ExistsMobileAccount',
        data: data2,
        datatype: 'json',
        async: isAsync,
        success: function (result) {
            isExistence = result == "True" ? true : false;
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
    //$.post("/Account/ExistsMobileAccount", {number:phoneNum},function(result){
    //    isExistence = result;
    //});
    return isExistence;
}

//9.手机注册执行函数
function register(phoneNum,pwd,verifyCode,isAsync){
    var data2 = { number: phoneNum, password: pwd, code: verifyCode };
    $.ajax({
        type: 'POST',
        url: '/Account/Register',
        data: data2,
        datatype: 'json',
        async: isAsync,
        success: function (result) {
            if (result === "") {
                var ss = confirm('您已注册成功！现在登录吗？');
                if (ss) {
                    $("#myModal2 .close").click();
                    $("#anchorLogin").click();
                }
                else {
                    $("#myModal2 .close").click();
                }
            }
            else {
                alert(result);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

function resetPasswordWithPhone(phoneNum, newpwd, confirmCode, isAsync) {
    var isSuccess = false;
    var data2 = {number:phoneNum,password:newpwd,code:confirmCode};
    $.ajax({
        type: 'POST',
        url: '/Account/ResetPasswordWithPhone',
        data: data2,
        datatype: 'json',
        async: isAsync,
        success: function (result) {
            if (result==="True") {
                isSuccess = true;
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
    return isSuccess;
}
/*后台数据交互结束*/

/*初始化清空表单内容开始*/

$("#anchorLogin").click(function () {
    clearLoginAndRegisterDialogInput();
});

$("#anchorRegister").click(function () {
    clearLoginAndRegisterDialogInput();
});

$("#myModal .ftblue").click(function () {
    clearLoginAndRegisterDialogInput();
});

//6.找回密码2 
$("#myModal5 .btn-login").click(function () {
    clearLoginAndRegisterDialogInput();
});

$("#myModal6 .btn-login").click(function () {

    var userid = $.trim($("#hidden4userID").val());

    var $oldpwd = $("#oldpwd4modify");
    var $newpwd = $("#newpwd4modify");
    var $newpwdagain = $("#newpwd4modifyagain");

    var oldpwd = $.trim($oldpwd.val());
    var newpwd = $.trim($newpwd.val());
    var newpwdagain = $.trim($newpwdagain.val());

    var isPass = true;
    if (newpwd === "" || oldpwd === "" || newpwdagain === "") {
        alert("信息不完整，请先填写");
        isPass = false;
    }

    var $oldpwdGroup = $oldpwd.parents('.form-group').eq(0);
    var $newpwdGroup = $newpwd.parents('.form-group').eq(0);
    var $newpwdagainGroup = $newpwdagain.parents('.form-group').eq(0);


    if ($oldpwdGroup.hasClass("has-error") || $newpwdGroup.hasClass("has-error") || $newpwdagainGroup.hasClass("has-error")) {
        alert("信息有错误，请先修改");
        isPass = false;
    }    

    $oldpwd.siblings('.infoerror').remove();
    $oldpwd.siblings('.infosuccess').remove();
    $newpwd.siblings('.infoerror').remove();
    $newpwd.siblings('.infosuccess').remove();
    $newpwdagain.siblings('.infoerror').remove();
    $newpwdagain.siblings('.infosuccess').remove();

    $oldpwdGroup.removeClass("has-success");
    $oldpwdGroup.removeClass("has-error");
    $newpwdGroup.removeClass("has-success");
    $newpwdGroup.removeClass("has-error");
    $newpwdagainGroup.removeClass("has-success");
    $newpwdagainGroup.removeClass("has-error");

    if (isPass) {
        var data = { userid: userid, oldpassword: oldpwd, newpassword: newpwd };
        isPass = modifyPassword(data);
        if(!isPass)
        {
            $oldpwdGroup.addClass('has-error');
            $newpwdGroup.addClass('has-success');
            $newpwdagainGroup.addClass('has-success');
            $oldpwd.after('<div class="infoerror">密码错误，请重新输入</div>');
        }
        var $oldpwdGroupinfosuccess = $oldpwdGroup.find('.imgsuccess');
        var $oldpwdGroupinfoerror = $oldpwdGroup.find('.imgerror');
        var $newpwdGroupinfosuccess = $newpwdGroup.find('.imgsuccess');
        var $newpwdGroupinfoerror = $newpwdGroup.find('.imgerror');
        var $newpwdagainGroupinfosuccess = $newpwdagainGroup.find('.imgsuccess');
        var $newpwdagainGroupinfoerror = $newpwdagainGroup.find('.imgerror');

        $oldpwdGroupinfosuccess.removeClass("hidden");
        $oldpwdGroupinfoerror.removeClass("hidden");
        $newpwdGroupinfosuccess.removeClass("hidden");
        $newpwdGroupinfoerror.removeClass("hidden");
        $newpwdagainGroupinfosuccess.removeClass("hidden");
        $newpwdagainGroupinfoerror.removeClass("hidden");
    }
    return isPass;
});

function modifyPassword(data) {
    var isSuccess = false;
    $.ajax({
        type: 'POST',
        url: '/Account/ModifyPassword',
        data: data,
        datatype: 'json',
        async: false,
        success: function (result) {
            if (result.Success) {
                //$("#btnJump2Login").click();//跳到登录界面
                clearLoginAndRegisterDialogInput();
                isSuccess = true;
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
    return isSuccess;
}

//8.修改密码 忘记密码
$("#myModal6 .ftblue").click(function () {
    clearLoginAndRegisterDialogInput();
});

//9.点击页面修改密码
$("#btnEditPassword").click(function () {
    clearLoginAndRegisterDialogInput();
});

//统一清空登录注册输入框内容
function clearLoginAndRegisterDialogInput() {
    $("#myModal,#myModal1,#myModal2,#myModal3,#myModal4,#myModal6").each(function () {
        var $that = $(this);
        $that.find("input.form-control").val("");

        var $group = $that.find("div.form-group");

        $group.removeClass("has-success");
        $group.removeClass("has-error");

        $group.find(".imgsuccess").addClass("hidden");
        $group.find(".imgerror").addClass("hidden");

        $group.find(".infoerror").remove();
        $group.find(".infosuccess").remove();

        $("#confirmCode").addClass("hidden");

        $("#autoLogin").attr("checked", true);
        $("#autoLogin").prev().val("True");
    });
}

$(document).delegate("#myModal,#myModal1,#myModal2,#myModal3,#myModal4,#myModal6", "focus", function () {
    $(this).blur();//聚焦立即失焦
});
// form reset之后可以试试
/*初始化清空表单内容结束*/

$("li.usermenuhead").click(function () {
    var $Menu = $("div.usermenu");
    var offset = $(this).offset();
    var leftPX = (offset.left + 20) + "px";
    //var topPX = (offset.top + $(this).height()) + "px";
    $Menu.css({ position: "absolute", left: leftPX });
    if ($Menu.hasClass("hidden")) {
        $Menu.removeClass("hidden");
    }
    else{
        $Menu.addClass("hidden");
    }
});

$("#isAutoLogin").click(function () {
    var $that = $(this);
    var $hidden = $that.find("input").eq(0);
    var $check = $("#autoLogin");
    var isCheck = $check.attr("checked");

    if ($hidden.val() === "True") {
        $hidden.val("False");
        $check.removeAttr("checked");
    }
    else {
        $hidden.val("True");
        $check.attr("checked", true);
    }
});

$("#collectList div.hotellist").hover(mouseCome, mouseLeave);

function mouseCome(){
    $(this).find("a.collectUrl").removeClass("hidden");
    $(this).find("div.tehui").hide();
}

function mouseLeave(){
    $(this).find("a.collectUrl").addClass("hidden");
    $(this).find("div.tehui").show();
}

$("a.collectUrl").click(function () {
    var $rowDiv = $(this).parents("div.hotellist").eq(0);
    var photoUrl = $.trim($rowDiv.find("a.collectRef").eq(0).attr("href"));
    $rowDiv.remove();//直接移除掉
    if (photoUrl) {
        var hotelId = parseInt(photoUrl.split("/")[2]);
        removeColelct(hotelId);
    }
});

$("#addCollect").click(function () {
    if ($("a.nickname").length === 0)
    {
        alert("请先登录保存您的喜爱");
        return false;
    }

    var photoUrl = $.trim($(this).parents("div.hotelphoto").eq(0).find("a").eq(0).attr("href"));
    if(photoUrl){
        var hotelId = parseInt(photoUrl.split("/")[2],10);
        var interestId = parseInt($.trim($("#interestId").val()),10);
        addCollect(hotelId, interestId);
    }
});

$("#removeCollect").click(function () {
    var photoUrl = $.trim($(this).parents("div.hotelphoto").eq(0).find("a").eq(0).attr("href"));
    if(photoUrl){
        var hotelId= parseInt(photoUrl.split("/")[2]);
        removeColelct(hotelId);
    }
});

function addCollect(hotelId, interestId) {
    var data2 = {hotel:hotelId,interest:interestId};
    $.ajax({
        type: 'POST',
        url: '/Collection/Add',
        data: data2,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success === 0) {
                $("#removeCollectDiv").show();
                $("#addCollectDiv").hide();
            }
            else {
                alert(result.Message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

function removeColelct(hotelId) {
    var data2 = { hotel: hotelId };
    $.ajax({
        type: 'POST',
        url: '/Collection/Remove',
        data: data2,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success === 0) {
                $("#addCollectDiv").show();
                $("#removeCollectDiv").hide();
            }
            else {
                alert(result.Message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

$("li.usermenuhead").hover(function () {
    var $Menu = $("div.usermenu");
    //var offset = $(this).offset();
    var offset = $("#menuArrow").offset();
    //var leftPX = (offset.left + 20) + "px";
    var leftPX = offset.left - 85;
    //var topPX = (offset.top + $(this).height()) + "px";
    //$Menu.css({ position: "absolute", left: leftPX });
    $Menu.css({ position: "absolute", left: leftPX });
    $Menu.removeClass("hidden");
}, function () {
    if (!$("div.usermenu").hasClass("hidden")) {
        $("div.usermenu").addClass("hidden");
    }
});

$("div.usermenu").hover(function () {
    var $Menu = $(this);
    var offset = $("#menuArrow").offset();
    //var leftPX = (offset.left + 20) + "px";
    var leftPX = offset.left - 85;
    //var topPX = (offset.top + $(this).height()) + "px";
    //$Menu.css({ position: "absolute", left: leftPX });
    $Menu.css({ position: "absolute", left: leftPX });
    $Menu.removeClass("hidden");
}, function () {
    if (!$(this).hasClass("hidden")) {
        $(this).addClass("hidden");
    }
});

$("#btnEditInfo").click(function () {
    $("#dlEditBasicInfo").show();
    $("#basicInfo").hide();
    $("#basicInfoTool").hide();
});

$("#nickName4Modify").change(function () {
    var userID = $.trim($("#hidden4userID").val());
    var nickName = $.trim($(this).val());
    if (CheckNickName(nickName, userID)) {
        $("#phoneNumber4Modify").attr("readonly", true);
    }
});

function CheckNickName(nickName,userID) {
    var isTrue = false;
    var data2 = {nickName:nickName,userID:userID};
    $.ajax({
        type: 'POST',
        url: '/Account/CheckNickName',
        data: data2,
        datatype: 'json',
        async: false,
        success: function (result) {
            if (result.Success === 0) {
                isTrue = true;
            }
            else {
                alert(result.Message);
            }
        },
        error: function (result) {
            alert("网络异常，请重试！");
        }
    });
    return isTrue;
}

$("#phoneNumber4Modify").change(function () {  
    var phoneNum = $.trim($(this).val());
    var pwd = $.trim($("#password4Modify").val());
    if (phoneNumReg.test(phoneNum)) {
        if (checkMobileAccountExistance(phoneNum, false)) {
            $("#nickName4Modify").removeAttr("readonly");
            $("#phoneNumber4Modify").removeAttr("readonly");
            alert("该手机号已注册");
        }
        else if(!pwd)
        {
            alert("输入密码后发送验证码");
        }
        else if (pwdReg.test(pwd)) {
            $("#btnSend").show();
            $("#confirmCodeDiv").show();
            $("#nickName4Modify").attr("readonly", true);
        }
        else {
            alert("密码至少是6位");
        }
    }
    else {
        alert("手机号码格式错误");
    }
});

$("#password4Modify").change(function () {
    var pwd = $.trim($(this).val());
    var phoneNum = $.trim($("#phoneNumber4Modify").val());

    if (pwdReg.test(pwd))
    {
        var oldNickName = $.trim($("#ddNickName").html());
        var nickName = $.trim($("#nickName4Modify").val());
        var oldPhone = $.trim($("#ddPhoneNumber").html());
        if (oldNickName === nickName && oldPhone !== phoneNum)
        {
            if (phoneNumReg.test(phoneNum)) {
                if (!checkMobileAccountExistance(phoneNum, false)) {
                    $("#btnSend").show();
                    $("#confirmCodeDiv").show();
                    $("#nickName4Modify").attr("readonly", true);
                }
                else {
                    alert("该手机号已注册");
                }
            }
            else {
                alert("手机号码格式错误！");
            }
        }
    }
    else {
        alert("密码至少是6位");
    }
});

$("#btnSend").click(function () {
    var phoneNum = $.trim($("#phoneNumber4Modify").val());
    sendVerifyCode2(phoneNum, false);
    $("#btnReSend").show();
    var intervalId = window.setInterval("changeBtnValue()", 1000);
    if (intervalId && !$("#intervalID4ModifyPhoneNum").val()) {
        $("#intervalID4ModifyPhoneNum").val(intervalId);
    }
    $(this).hide();
});

function changeBtnValue() {
    var currentVal = parseInt($.trim($("#btnReSend").text()), 10);
    currentVal--;
    if (currentVal) {
        $("#btnReSend").text(currentVal + "秒后重发");
    }
    else {
        var intervalId = $("#intervalID4ModifyPhoneNum").val();
        if (intervalId) {
            $("#intervalID4ModifyPhoneNum").val("");
            window.clearInterval(parseInt(intervalId,10));
        }
        unLockSend2();
    }
}

function unLockSend2(){
    $("#btnSend").show();
    $("#btnReSend").text("30秒后重发");
    $("#btnReSend").hide();
}

$("#btnSaveBasicInfo").click(function () {
    var userID = $.trim($("#hidden4userID").val());
    var oldNickName = $.trim($("#ddNickName").html());
    var oldPhone = $.trim($("#ddPhoneNumber").html());
    var password = $.trim($("#password4Modify").val());
    var newPhone = $.trim($("#phoneNumber4Modify").val());
    var confirmCode = $.trim($("#confirmCode4Modify").val());
    var nickName = $.trim($("#nickName4Modify").val());
    if (!nickName) {
        alert('昵称不能为空');
        return false;
    }
    if (!newPhone) {
        alert('手机号不能为空');
        return false;
    }
    if (!password)
    {
        alert('密码不能为空');
        return false;
    }
    if (oldNickName !== nickName) {
        var data1 = { userID: userID, password: password, nickName: nickName };
        $.ajax({
            type: 'POST',
            url: '/Account/UpdateNickName',
            data: data1,
            datatype: 'json',
            async: false,
            success: function (result) {
                if (result.Success === 0) {
                    $("#ddNickName").html(nickName);
                    $("li a.nickname").html(nickName);
                    alert("昵称修改成功！");
                    InitEditBasicInfo();
                }
                else {
                    alert(result.Message);
                }
            },
            error: function (result) {
                alert("网络异常，请重试！");
            }
        });
    }
    else if (newPhone !== oldPhone) {
        if (!confirmCode) {
            alert('验证码不能为空');
            return false;
        }
        var data2 = { userid: userID, password: password, newphone: newPhone, confirmCode: confirmCode };
        $.ajax({
            type: 'POST',
            url: '/Account/ModifyUserPhone',
            data: data2,
            datatype: 'json',
            async: false,
            success: function (result) {
                if (result.Success) {
                    $("#ddPhoneNumber").html(newPhone);
                    InitEditBasicInfo();
                    clearLoginAndRegisterDialogInput();
                    //弹出对应的的提醒框 指定点击按钮
                    $("#myModal5").show();
                }
                else {
                    alert(result.Message);
                }
            },
            error: function (result) {
                alert("网络异常，请重试！");
            }
        });
    }
    else {
        alert("您没有修改任何内容");
    }
    $("#phoneNumber4Modify").removeAttr("readonly", true);
    $("#nickName4Modify").removeAttr("readonly", true);
});

$("#btnCancelBasicInfo").click(InitEditBasicInfo);

function InitEditBasicInfo() {
    $("#dlEditBasicInfo").hide();
    $("#phoneNumber4Modify").removeAttr("readonly");
    $("#nickName4Modify").removeAttr("readonly");
    $("#phoneNumber4Modify").val(getCookie("PhoneNumber"));
    $("#nickName4Modify").val(getCookie("NickName"));
    $("#confirmCode4Modify").val("");
    $("#password4Modify").val("");
    $("#confirmCodeDiv").hide();
    $("#btnSend").hide();

    $("#basicInfo").show();
    $("#basicInfoTool").show();
}

function sendVerifyCode2(){
    var userID = $.trim($("#hidden4userID").val());
    var password = $.trim($("#password4Modify").val());
    var newPhone = $.trim($("#phoneNumber4Modify").val());
    var data2 = { userid: userID, password: password, newphone: newPhone};
    $.ajax({
        type: 'POST',
        url: '/Account/SendModifyUserPhoneConfirmCode',
        data: data2,
        datatype: 'json',
        async: false,
        success: function (result) {
            if (result.Success) {
                alert("验证码已发送到您的新手机上,请注意查收。");
            }
            else
            {
                alert(result.Message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

function getCookie(name)
{
    var arr,reg=new RegExp("(^| )"+name+"=([^;]*)(;|$)"); 
    if(arr=document.cookie.match(reg))
    {
        return unescape(arr[2]);
    }
    else
    {
        return "";
    }
}

function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

$("#commInfoDiv .btnAdd").click(function () {
    var $preDiv = $(this).prev("div.fp");
    $preDiv.find("input.form-control").val("");
    $preDiv.show();
});


$("#commInfoDiv .btnEdit").click(function () {
    var $parentDiv = $(this).parents("div.fp");
    var $nextDiv = $parentDiv.next();
    var originValue = $.trim($parentDiv.find("div.originValueDiv").html());
    $nextDiv.find("input.form-control").val(originValue);
    $nextDiv.show();
});

function checkInfoExists(object, newTitle) {
    var isTrue = true;
    var $array = $(object).parents("dd").find(".originValueDiv");
    var minNum = $(object).hasClass("btnSaveEdit") ? 1 : 0;

    if ($array.length > minNum) {
        $array.each(function () {
            var $exist = $(this);
            var existValue = $.trim($exist.html());
            if (existValue === newTitle) {
                isTrue = false;
                return false;
            }
        });
    }
    return isTrue;
}

$("#commInfoDiv .btnSaveEdit").click(function () {
    var $parentDiv = $(this).parents("div.fp");
    var $preDiv = $parentDiv.prev("div.fp");
    var newTitle = $.trim($parentDiv.find(".form-control").val());
    if (!newTitle)
    {
        alert("信息不能为空！");
        return false;
    }
    var oldTitle = $.trim($preDiv.find(".originValueDiv").html());
    if (oldTitle === newTitle) {
        $parentDiv.hide();
        return false;
    }
    
    if (!checkInfoExists(this, newTitle)) {
        alert("信息已存在！");
        return false;
    }
    //发票个人信息验重

    var infotype = judgeCurrentInfoType($parentDiv);
    var state = parseInt($(this).siblings("input.stateHidden").val(),10);
    var idx = parseInt($(this).siblings("input.idxHidden").val(), 10);

    var data = { info: newTitle, infotype: infotype, state: state, idx: idx };
    $.ajax({
        type: 'POST',
        url: '/Account/AddUserCommInfo',
        data: data,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success == 0) {
                $preDiv.find("div.originValueDiv").html(newTitle);
                $parentDiv.hide();
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("#commInfoDiv .btnSaveAdd").click(function () {
    var $parentDiv = $(this).parents("div.fp");
    var newTitle = $.trim($parentDiv.find(".form-control").val());
    if (!newTitle) {
        alert("内容不能为空！");
        return false;
    }
    var infotype = judgeCurrentInfoType($parentDiv);
    if(infotype===0)
    {
        alert("缺少信息类型！");
        return false;
    }
    if (!checkInfoExists(this, newTitle)) {
        alert("信息已存在！");
        return false;
    }

    var data = { info: newTitle, infotype: infotype, state:1};
    $.ajax({
        type: 'POST',
        url: '/Account/AddUserCommInfo',
        data: data,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success == 0) {
                //ToDo 新建div层
                //$parentDiv.before('<div class="row fp"><div class="col-sm-1 radio1"><input type="radio" name="optionsRadios" value="option1"></div><div class="col-sm-7 originValueDiv">' + newTitle + '</div><div class="col-sm-4 edit"><button type="button" class="btn btn-06 btnEdit">编辑</button></div></div>');
                //$parentDiv.hide();
                window.location.href = "/personal/info";//新增的接口返回值暂时没有对应的IDX 必须刷新页面获得该信息
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("#commInfoDiv .btnDel").click(function () {
    var $parentDiv = $(this).parents("div.fp");
    //var $preDiv = $parentDiv.prev();
    var currentInfo = $.trim($parentDiv.find("div.originValueDiv").html());
    var idx = parseInt($(this).siblings("input.idxHidden").val(), 10);

    var data = { info: currentInfo, state: -1, idx: idx };
    $.ajax({
        type: 'POST',
        url: '/Account/AddUserCommInfo',
        data: data,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success == 0) {
                $parentDiv.remove();
                //$preDiv.remove();
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("#commInfoDiv .btnCancel").click(function () {
    var $parentDiv = $(this).parents("div.fp");
    $parentDiv.find(".form-control").val("");
    $parentDiv.hide();
});

function judgeCurrentInfoType($this)
{
    var $dl = $($this).parents("dl").eq(0);
    var id = $dl.attr("id");
    if(!id)
    {
        return 0;
    }
    if(id.indexOf("1") === 0){
        return 1;
    }
    else if (id.indexOf("2") === 0)
    {
        return 2;
    }
    else if (id.indexOf("3") === 0)
    {
        return 3;
    }
}

$("#commInfoDiv input[type='radio']").click(function(){
    //$(this).parents("div.fp").eq(0).find(".btnEdit").toggle();
    var $parents = $(this).parents("div.fp").eq(0);
    $parents.parents("dd").find(".btnEdit").hide();
    $parents.parents("dd").find(".btnDel").hide();
    var $btn = $parents.find(".btnEdit");
    var $btn2 = $parents.find(".btnDel");
    $btn.show();
    $btn2.show();
});

//退出则是
$("#btnQuit").click(function () {
    $.ajax({
        type: 'POST',
        url: '/Account/QuitLogin',
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success) {
                window.location.href = "/";
                //window.open('/', '_self');
            }
        },
        error: function (result) {
            alert("网络异常，请重试！");
        }
    });
    return false;
});