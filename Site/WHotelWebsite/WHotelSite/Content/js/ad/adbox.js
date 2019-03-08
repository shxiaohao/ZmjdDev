$(function () {
    
});
      
mui.init();

function goto(param) {
    var url = "whotelapp://www.zmjiudian.com/" + param;
    this.location = url;
}

function gotopage(param) {
    var url = "whotelapp://www.zmjiudian.com/gotopage?url=http://www.zmjiudian.com/" + param;
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

function linkClick(gotopageinfo)
{
    var jumpUrl = $("#jumpUrl").val();

    //app type
    var appType = $("#appType").val(); 
    if (appType == "android")
    {
        //跳转
        gourl(jumpUrl);

        //android关闭
        try { whotel.appClosePage(); } catch (e) { }
    }
    else
    {
        //ios关闭
        try { location.href = "whotelapp://loadJS?url=javascript:adCloseAndJump()"; } catch (e) { }
    }
}

function adCloseAndJump()
{
    var jumpUrl = $("#jumpUrl").val();

    //跳转
    gourl(jumpUrl);
}

