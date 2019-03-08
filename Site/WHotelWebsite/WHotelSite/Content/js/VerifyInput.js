/// <reference path="verifyInput.js" />
/// <reference path="verifyInput.js" />
var phoneNumReg = /^((100)|(13[0-9])|(15[0-9])|(19[0-9])|(18[0-9])|(17[0-9])|(16[0-9])|(14[0-9]))\d{8}$/;
var mailReg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((.[a-zA-Z0-9_-]{2,3}){1,2})$/;
var pwdReg = /^\S{6,}$/;

/*验证手机号码格式*/
(function () {
    $("input.phoneNum").change(function () {
        var $that = $(this);
        if ($that.parents("#myModal2").length > 0) {
            verifyPhoneNum($that, true);
        }
        else {
            verifyPhoneNum($that, false);
        }
    });
})();

/*非注册界面还要验证手机账号是否存在*/
function verifyPhoneNum($target,isRegister) {
    var isPass = false;
    var $phoneNum = $target;
    var phoneNumVal = $.trim($phoneNum.val());//自动清除两侧空格
    $phoneNum.siblings().remove(".infoerror");
    $phoneNum.siblings().remove(".infosuccess");

    var $phoneNumberGroup = $phoneNum.parents('.form-group').eq(0);

    var $phoneNumberinfosuccess = $phoneNumberGroup.find('.imgsuccess');
    var $phoneNumberinfoerror = $phoneNumberGroup.find('.imgerror');

    $phoneNum.siblings('.infoerror').remove();
    $phoneNum.siblings('.infosuccess').remove();

    $phoneNumberGroup.removeClass("has-success");
    $phoneNumberGroup.removeClass("has-error");

    if (phoneNumReg.test(phoneNumVal)) {
        if(!isRegister){
            if (checkMobileAccountExistance(phoneNumVal, false)) {                
                $phoneNumberGroup.addClass("has-success");
                isPass = true;
            }
            else {
                //clearLoginAndRegisterDialogInput();
                isPass = false;
                if (!$phoneNumberGroup.hasClass("has-error")) {
                    $phoneNumberGroup.addClass("has-error");
                }
                $phoneNum.after('<div class="infoerror">该手机账号尚未注册</div>');
            }
        }
        else {
            if (checkMobileAccountExistance(phoneNumVal, false)) {
                //clearLoginAndRegisterDialogInput();
                isPass = false;
                if (!$phoneNumberGroup.hasClass("has-error")) {
                    $phoneNumberGroup.addClass("has-error");
                }
                $phoneNum.after('<div class="infoerror">该手机账号已存在</div>');
            }
            else{
                $phoneNumberGroup.addClass("has-success");
                $("#verifyCode4Register").parents(".form-group").eq(0).removeClass("hidden");//注册手机号合规后 显示验证码这一行
                isPass = true;
            }
        }
    }
    else {
        $phoneNum.after('<div class="infoerror">无效手机号码，请重新输入！</div>');
        $phoneNumberGroup.addClass("has-error");
        if(isRegister){
            $("#verifyCode4Register").parents(".form-group").eq(0).addClass("hidden");//注册手机号不合规后 隐藏验证码这一行
            $("#verifyCode4Register").val("");
        }
    }
    $phoneNumberinfosuccess.removeClass("hidden");
    $phoneNumberinfoerror.removeClass("hidden");
    return isPass;
}

/*验证密码格式*/
$("input.password").change(function () {
    var $pwd = $(this);
    var pwd = $.trim($pwd.val());
    $pwd.siblings().remove(".infoerror");
    $pwd.siblings().remove(".infosuccess");

    var $pwdGroup = $pwd.parents('.form-group').eq(0);

    var $pwdinfosuccess = $pwdGroup.find('.imgsuccess');
    var $pwdinfoerror = $pwdGroup.find('.imgerror');

    $pwd.siblings('.infoerror').remove();
    $pwd.siblings('.infosuccess').remove();

    $pwdGroup.removeClass("has-success");
    $pwdGroup.removeClass("has-error");    

    if (pwd === "") {
        if (!$pwdGroup.hasClass("has-error")) {
            $pwdGroup.addClass("has-error");
        }
        $pwd.after('<div class="infoerror">密码不能为空</div>');
    }
    else if (pwdReg.test(pwd)) {
        $pwdGroup.addClass("has-success");
    }
    else {
        $pwdGroup.addClass("has-error");
        $pwd.after('<div class="infoerror">密码长度必须不小于6位</div>');
    }
    $pwdinfosuccess.removeClass("hidden");
    $pwdinfoerror.removeClass("hidden");
});

/*注册对话框验证码是否正确*/
$("#verifyCode4Register").change(function () {
    var phoneNum = $.trim($("#phoneNum4Register").val());
    var verifyCode = $.trim($(this).val());
    var $verifyCode4RegisterGroup = $(this).parents('.form-group').eq(0);

    $verifyCode4RegisterGroup.removeClass("has-success");
    $verifyCode4RegisterGroup.removeClass("has-error");

    $verifyCode4RegisterGroup.find(".infosuccess").remove();
    $verifyCode4RegisterGroup.find(".infoerror").remove();

    if (verifyCode === "") {
        $verifyCode4RegisterGroup.addClass("has-error");
        $(this).after("<div class='infoerror'>验证码不能为空</div>");
    }
    else if (checkVerifyCode(phoneNum, verifyCode, false)) {
        $verifyCode4RegisterGroup.addClass("has-success");
    }
    else {
        $verifyCode4RegisterGroup.addClass("has-error");
        $(this).after("<div class='infoerror'>验证码不正确</div>");
    }

    $verifyCode4RegisterGroup.find(".imgsuccess").removeClass("hidden");
    $verifyCode4RegisterGroup.find(".imgerror").removeClass("hidden");
});

/*找回密码对话框验证码是否正确*/
$("#verifyCode4Password").change(function () {
    var phoneNum = $.trim($("#phoneNum4Password").val());
    var verifyCode = $.trim($(this).val());
    var $verifyCode4PasswordGroup = $(this).parents('.form-group').eq(0);

    $verifyCode4PasswordGroup.removeClass("has-success");
    $verifyCode4PasswordGroup.removeClass("has-error");

    $verifyCode4PasswordGroup.find(".infosuccess").remove();
    $verifyCode4PasswordGroup.find(".infoerror").remove();    

    if (verifyCode === "") {
        $verifyCode4PasswordGroup.addClass("has-error");
        $(this).after("<div class='infoerror'>验证码不能为空</div>");
    }
    else if (checkVerifyCode(phoneNum, verifyCode, false)) {
        $verifyCode4PasswordGroup.addClass("has-success");
    }
    else {
        $verifyCode4PasswordGroup.addClass("has-error");
        $(this).after("<div class='infoerror'>验证码不正确</div>");
    }

    $verifyCode4PasswordGroup.find(".imgsuccess").removeClass("hidden");
    $verifyCode4PasswordGroup.find(".imgerror").removeClass("hidden");
});

// 验证密码前后输入是否一致 交由again框来完成
$("#newPasswordAgain").change(function () {
    var $newPasswordAgain = $(this);
    var $newPassword = $("#newPassword");
    var $newPasswordDiv = $newPassword.parents(".form-group").eq(0);
    var $newPasswordAgainDiv = $newPasswordAgain.parents(".form-group").eq(0);

    var passwordAgain = $.trim($newPasswordAgain.val());
    var password = $.trim($newPassword.val());

    
    if(passwordAgain===""){
        return false;
    }
    if (!pwdReg.test(passwordAgain)){
        return false;
    }

    $newPasswordAgainDiv.removeClass("has-success");
    $newPasswordAgainDiv.removeClass("has-error");

    $newPasswordAgainDiv.find(".infosuccess").remove();
    $newPasswordAgainDiv.find(".infoerror").remove();

    if (password === "") {
        if (!$newPasswordDiv.hasClass("has-error")){
            $newPasswordDiv.addClass("has-error");
        }
        $newPasswordDiv.removeClass("has-success");
        $newPasswordDiv.find(".infoerror").remove();
        $newPasswordDiv.find(".infosuccess").remove();
        $newPassword.after('<div class="infoerror">密码不能为空</div>');

        if (!$newPasswordAgainDiv.hasClass("has-error")) {
            $newPasswordAgainDiv.addClass("has-error");
        }
        $newPasswordAgain.after('<div class="infoerror">前后密码不一致</div>');
    }
    else if (password !== passwordAgain) {
        if (!$newPasswordAgainDiv.hasClass("has-error")) {
            $newPasswordAgainDiv.addClass("has-error");
        }
        $newPasswordAgain.after('<div class="infoerror">前后密码不一致</div>');
    }
    else {
        if (!$newPasswordAgainDiv.hasClass("has-success")) {
            $newPasswordAgainDiv.addClass("has-success");
        }
    }
});

// 验证密码前后输入是否一致 交由again框来完成
$("#password4RegisterAgain").change(function () {
    var $newPasswordAgain = $(this);
    var $newPassword = $("#password4Register");
    var $newPasswordDiv = $newPassword.parents(".form-group").eq(0);
    var $newPasswordAgainDiv = $newPasswordAgain.parents(".form-group").eq(0);

    var passwordAgain = $.trim($newPasswordAgain.val());
    var password = $.trim($newPassword.val());

    if (passwordAgain === "") {
        return false;
    }
    if (!pwdReg.test(passwordAgain)) {
        return false;
    }

    $newPasswordAgainDiv.removeClass("has-success");
    $newPasswordAgainDiv.removeClass("has-error");

    $newPasswordAgainDiv.find(".infosuccess").remove();
    $newPasswordAgainDiv.find(".infoerror").remove();

    if (password === "") {
        if (!$newPasswordDiv.hasClass("has-error")) {
            $newPasswordDiv.addClass("has-error");
        }
        $newPasswordDiv.removeClass("has-success");
        $newPasswordDiv.find(".infoerror").remove();
        $newPasswordDiv.find(".infosuccess").remove();

        $newPassword.after('<div class="infoerror">密码不能为空</div>');

        if (!$newPasswordAgainDiv.hasClass("has-error")) {
            $newPasswordAgainDiv.addClass("has-error");
        }
        $newPasswordAgain.after('<div class="infoerror">前后密码不一致</div>');
    }
    else if (password !== passwordAgain) {
        if (!$newPasswordAgainDiv.hasClass("has-error")) {
            $newPasswordAgainDiv.addClass("has-error");
        }
        $newPasswordAgain.after('<div class="infoerror">前后密码不一致</div>');
    }
    else {
        if (!$newPasswordAgainDiv.hasClass("has-success")) {
            $newPasswordAgainDiv.addClass("has-success");
        }
    }
});

//修改密码 验证前后密码是否一致 交由again完成
$("input.passwordagain").change(function () {
    var $newPasswordAgain = $(this);
    var $newPasswordAgainDiv = $newPasswordAgain.parents("div.form-group");
    var $newPasswordDiv = $newPasswordAgainDiv.prev();
    var $newPassword = $newPasswordDiv.find("input.password");

    var passwordAgain = $.trim($newPasswordAgain.val());
    var password = $.trim($newPassword.val());

    //if (!passwordAgain) {
    //    return false;
    //}
    //if (!pwdReg.test(passwordAgain)) {
    //    return false;
    //}

    $newPasswordAgainDiv.removeClass("has-success");
    $newPasswordAgainDiv.removeClass("has-error");

    $newPasswordAgainDiv.find(".infosuccess").remove();
    $newPasswordAgainDiv.find(".infoerror").remove();

    if (password === "") {
        if (!$newPasswordAgainDiv.hasClass("has-error")) {
            $newPasswordAgainDiv.addClass("has-error");
        }
        $newPasswordAgain.after('<div class="infoerror">密码不能为空</div>');        
    }
    else if (password !== passwordAgain) {
        if (!$newPasswordAgainDiv.hasClass("has-error")) {
            $newPasswordAgainDiv.addClass("has-error");
        }
        $newPasswordAgain.after('<div class="infoerror">前后密码不一致</div>');
    }
    else {
        if (!$newPasswordAgainDiv.hasClass("has-success")) {
            $newPasswordAgainDiv.addClass("has-success");
        }
    }
});


function verifyPasswordAgree(type) {
    var isAgree = false;
    if (type === verifyPasswordAgreeType.REGISTER) {
        var $password4Register = $("#password4Register");
        var $password4RegisterAgain = $("#password4RegisterAgain");
        var $password4RegisterDiv = $password4Register.parents(".form-group").eq(0);
        var $password4RegisterAgainDiv = $password4Register.parents(".form-group").eq(0);

        var password4Register = $.trim($password4Register.val());
        var password4RegisterAgain = $.trim($password4RegisterAgain.val());

        if (password4Register === password4RegisterAgain) {
            $password4RegisterDiv.addClass("has-success");
            $password4RegisterAgainDiv.addClass("has-success");
            $password4RegisterDiv.removeClass("has-error");
            $password4RegisterAgainDiv.removeClass("has-error");
            isAgree = true;
        }
        else {
            $password4RegisterDiv.addClass("has-error");
            $password4RegisterAgainDiv.addClass("has-error");
            $password4RegisterDiv.removeClass("has-success");
            $password4RegisterAgainDiv.removeClass("has-success");
        }
    }
    else if (type === verifyPasswordAgreeType.FIND) {
        var $newPassword = $("#newPassword");
        var $newPasswordAgain = $("#newPasswordAgain");
        var $newPasswordDiv = $newPassword.parents(".form-group").eq(0);
        var $newPasswordAgainDiv = $newPasswordAgain.parents(".form-group").eq(0);

        var newPassword = $.trim($newPassword.val());
        var newPasswordAgain = $.trim($newPasswordAgain.val());

        if (newPassword === newPasswordAgain) {
            $newPasswordDiv.addClass("has-success");
            $newPasswordAgainDiv.addClass("has-success");
            $newPasswordDiv.removeClass("has-error");
            $newPasswordAgainDiv.removeClass("has-error");
            isAgree = true;
        }
        else {
            $newPasswordDiv.addClass("has-error");
            $newPasswordAgainDiv.addClass("has-error");
            $newPasswordDiv.removeClass("has-success");
            $newPasswordAgainDiv.removeClass("has-success");
        }
    }
    return isAgree;
}

//function verifyDESEncode(sourceID, requestType, data, md5Key) {
//    var timeSeconds = new Date().getSeconds();
//    var data2 = { TimeStamp: timeSeconds, SourceID: sourceID, RequestType:,Sign:};
//    $.ajax({
//        type: 'GET',
//        url: '/Account/Verify',
//        data: data2,
//        datatype: 'json',
//        async: isAsync,
//        success: function (result) {
//            if (result === "True") {
//            }
//        },
//        error: function () {
//            alert('网络异常，请重试！');
//        }
//    });
//}

//$("input.pwd4modify").change(function () {
//    var pwd = $.trim($(this).val());
//    if (pwd === "") {
//        $(this).after('<div class="infoerror">密码不能为空</div>');
//    }
//    else if (pwdReg.test(pwd)) {
//        //$pwdGroup.addClass("has-success");
//        $(this).parents("div.maininput").eq(0).find("div.infoerror").remove();
//    }
//    else {
//        $(this).after('<div class="infoerror">密码长度必须不小于6位</div>');
//    }
//});


//验证接口签名
var hexcase = 0;  /* hex output format. 0 - lowercase; 1 - uppercase        */
var b64pad = "="; /* base-64 pad character. "=" for strict RFC compliance   */
var chrsz = 8;  /* bits per input character. 8 - ASCII; 16 - Unicode      */

var binl2b64 = function (binarray) {
    var tab = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    var str = "";

    for (var i = 0; i < binarray.length * 4; i += 3) {
        var triplet = (((binarray[i >> 2] >> 8 * (i % 4)) & 0xFF) << 16)
                   | (((binarray[i + 1 >> 2] >> 8 * ((i + 1) % 4)) & 0xFF) << 8)
                   | ((binarray[i + 2 >> 2] >> 8 * ((i + 2) % 4)) & 0xFF);

        for (var j = 0; j < 4; j++) {
            if (i * 8 + j * 6 > binarray.length * 32) str += b64pad;
            else str += tab.charAt((triplet >> 6 * (3 - j)) & 0x3F);
        }
    }
    return str;
};