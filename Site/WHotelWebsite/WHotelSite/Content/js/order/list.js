$(".order-del-link").click(function () {

    if(!confirm('您确认删除该订单吗？'))
    {
        return;
    }

    var $that = $(this);
    var urlStr = $that.data("deleteurl");
    var index1 = urlStr.indexOf('?');
    var index2 = urlStr.indexOf('=');

    var url = urlStr.substring(0, index1);
    var paramName = urlStr.substring(index1 + 1, index2);
    var paramValue = parseInt(urlStr.substr(index2 + 1), 10);//链接后的订单ID
    var data = { order: paramValue };
    $.ajax({
        type: 'POST',
        url: url,
        data:data,
        datatype: 'json',
        async: true,
        success: function (r) {
            if(r.success === "true"){
                alert(r.message);
                window.location.href = r.url;
            }
            else {
                alert(r.message);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});