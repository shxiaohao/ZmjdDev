$(document).ready(function () {

    var UserId = parseInt($("#userid").val());
    var IsLogin = $("#islogin").val() == "1";
    var IsApp = $("#isapp").val() == "1";

    //检测购买成功后是否有自动跳转操作
    var checkUrlReferrer = function () {

        //购买完成后，提示用户是否返回购买前的页面
        Global.UrlReferrer.Check(
        function (_data) {

            Global.UrlReferrer.Clear();

            if (IsApp) {

                
            }
            else {

                _Modal.show({
                    title: '回到刚才页面',
                    content: _data.name,
                    textAlign: 'center',
                    showClose: true,
                    confirmText: '前往',
                    confirm: function () {
                        location.href = _data.url;
                        //_Modal.hide();
                    }
                });

                //if (confirm("回到刚才页面：" + _data.name)) {
                //    location.href = _data.url;
                //}
            }

        },
        function (_data) {
            console.log("不存在跳转")
        });

    }

    //初始mobile login
    var loginCheckFun = function () {
        setTimeout(checkUrlReferrer, 1000);
    }
    var loginCancelFun = function () { }

    _loginModular.init(loginCheckFun, loginCancelFun);

    if (!IsLogin && UserId) {
        //console.log(UserId + "自动登录");

        //检测登录并自动登录
        _loginModular.verify.loginByUid(UserId, loginCheckFun);
    }
    else {
        setTimeout(checkUrlReferrer, 1800);
    }

});

//app相关参数初始化以后，回调处理
var _appInitCallback = function () {

    //如果是app环境，在VIP购买成功后向app标记用户信息产品了变更
    zmjd.userinfoChanged();

}

//该方法为app主动调用（目前为页面加载完成后调用）
var _getAppData = function (userid, apptype, appvercode, appverno) {

    //init data
    _InitApp(userid, apptype, appvercode, appverno);

    //call back
    try {
        _appInitCallback();
    } catch (e) {

    }
}