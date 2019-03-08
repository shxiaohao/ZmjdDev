$(function () {

    
});

var isload = true;
var loadTimer;

mui.init({
    pullRefresh: {
        container: ".rec-comments-panel",//待刷新区域标识，querySelector能定位的css选择器均可，比如：id、.class等
        up: {
            contentrefresh: "正在加载中",//可选，正在加载状态时，上拉加载控件上显示的标题内容
            contentnomore: '没有更多数据了',//可选，请求完毕若没有更多数据时显示的提醒内容；
            callback: pullfresh //必选，刷新函数，根据具体业务来编写，比如通过ajax从服务器获取新数据；
        }
    }
});

function pullfresh() {

    clearTimeout(loadTimer);
    loadTimer = setTimeout(function () {
        //业务逻辑代码，比如通过ajax从服务器获取新数据；
        loadComments();
    }
    , 500);
}

var start = 0;
var count = 8;

function loadComments() {
    if (isload) {

        isload = false;

        //下一页
        start += count;

        var dic = {};
        dic["userid"] = name;
        dic["s"] = start;
        dic["c"] = count;

        $.get('/App/LoadRecComments', dic, function (content) {
            var comments = content;
            var comTemplate = $("#comobj-template").html();
            var htmls = "";

            try {
                for (var i = 0; i < comments.length; i++) {
                    var comEntity = comments[i];
                    var setHtml = comTemplate;
                    setHtml = setHtml.replace("{CommentID}", comEntity.CommentID);
                    setHtml = setHtml.replace("{PhotoUrl}", comEntity.PhotoUrl);
                    setHtml = setHtml.replace("{HotelName}", comEntity.HotelName);
                    setHtml = setHtml.replace("{Title}", comEntity.Title);
                    setHtml = setHtml.replace("{AvatarUrl}", comEntity.AvatarUrl);
                    setHtml = setHtml.replace("{NickName}", comEntity.NickName);

                    setHtml = setHtml.replace("{Clear}", (i % 2 == 0 ? "clear:both;" : ""));

                    htmls += setHtml;
                }
            } catch (e) {

            }

            $(".item-ul").html($(".item-ul").html() + htmls);

            //注意：
            //1、加载完新数据后，必须执行如下代码，true表示没有更多数据了：
            //2、若为ajax请求，则需将如下代码放置在处理完ajax响应数据之后
            mui('.rec-comments-panel').pullRefresh().endPullupToRefresh((htmls == ""));

            isload = true;

            //重新绑定事件
            bindItemLiEvent();
        });
    }
}

function goto(param) {
    var isApp = $("#isApp").val();
    var url = "whotelapp://www.zmjiudian.com/" + param;
    if (isApp == "0") {
        url = "http://www.zmjiudian.com/" + param;
    }
    this.location = url;
}

function gourl(url) {
    location.href = url;
}

function bindItemLiEvent() {
    $(".item-li").each(function () {
        //debugger;
        $(this)[0].addEventListener("tap", function () {
            var cid = $(this).data("cid");
            goto("personal/comments/" + cid);
        });
    });
}
bindItemLiEvent();