var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var userId = parseInt($("#userId").val());
var appShareLink = $("#appShareLink").val();

$(function () {

    //初始mobile login
    var loginCheckFun = function () {
        location.reload(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //检测登录并自动登录
    if (!isapp && userId <= 0) {
        _loginModular.verify.autoLogin(loginCheckFun);
    }
    
    var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')&realuserid=1";

    //邀请好友
    $(".share-btn").click(function () {

        if (inWeixin) {

            if (userId && userId > 0) {
                $(".genvip-share-tip").show();
            }
            else {
                //alert("H登录")
                _loginModular.show();
            }

        }
        else if (isapp) {

            if (userId && userId > 0) {
                gourl(appShareLink);
            }
            else {
                //alert("原生登录")
                location.href = loginapphref;
            }

        }
        else {
            alert("请到微信或周末酒店APP中进行分享哦～")
        }

    });

    //延时加载活动说明
    setTimeout(function () {

        //分享提示图片动态加载
        var shareTipImg = $(".genvip-share-tip img");
        setImgOriSrc(shareTipImg);

    }, 800);

    //右上角分享提示点击事件
    $(".genvip-share-tip").click(function () {
        $(this).hide();
    });
});

//原生登录回调
function loginCallback(userid) {
    location.href = "/Active/VipActiveForGift?userid={userid}";
    //alert("抱歉，请返回首页重新进入后操作");
    //history.back();
}

function gourl(url) {
    location.href = url;
}
