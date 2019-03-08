$(function () {

    var isLogin = $("#isLogin").val() == "1";
    var isErrPage = $("#isErrPage").val() == "1";
    var isClearCache = $("#isClearCache").val() == "1";
    var isRedUrl = $("#isRedUrl").val() == "1";
    var redirectUrl = $("#redirectUrl").val();
    console.log(isRedUrl)
    //初始mobile login
    var loginCheckFun = function () {
        if (isRedUrl && redirectUrl) {
            location.href = redirectUrl;
        }
        else {
            location.href = location.href + "&v=1";
            //location.reload(false);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
        }
    }

    var loginCancelFun = function () {
        alert("请您先登录");
        return false;
    }

    _loginModular.init(loginCheckFun, loginCancelFun, true);

    if (!isErrPage && !isLogin && !isClearCache) {

        //如果需要强制登录，那么判断当前用户是否已经登录过，则自动登录
        if (!_loginModular.verify.autoLogin(loginCheckFun)) {
            _loginModular.show();
        }
    }
    else {

        if (isLogin && isRedUrl && redirectUrl) {
            location.href = redirectUrl;
        }
    }

});