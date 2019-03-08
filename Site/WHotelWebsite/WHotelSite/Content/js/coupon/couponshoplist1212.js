var loginapphref = "whotelapp://loadJS?url=javascript:loginCallback('{userid}')";
var pubShopId = "";

$(document).ready(function () {
    
    //$(".main .list .item").each(function ()
    //{
    //    $(this).click(function ()
    //    {
    //        var shopid = $(this).data("shopid");
    //        pubShopId = shopid;

    //        var userid = $("#userid").val();
    //        if (userid != "" && userid != "0") {
    //            checkUserPartActiveState(userid);
    //        }
    //        else {
    //            location.href = loginapphref;
    //        }
    //    });
    //});
});

function checkUserPartActiveState(userid) {
    showSpinner(true);

    var dic = {};
    dic["userid"] = userid;
    $.get('/Coupon/CheckUserPartActiveStateFor1212', dic, function (content) {
        var msg = content.Message;
        var suc = content.Success;

        showSpinner(false);

        switch (suc) {
            //没有通过验证，不能参与抢购
            case "0":
                {
                    bootbox.alert({
                        message: "<div class='alert-rulesmsg'>" + msg + "</div>",
                        buttons: {
                            ok: {
                                label: '知道了',
                                className: 'btn-default'
                            }
                        },
                        callback: function (result) {

                        },
                        closeButton: false
                    });
                    break;
                }
                //通过验证，可以进入抢购
            case "1":
                {
                    location.href = '/coupon/shop/' + pubShopId;
                    break;
                }
        }
    });
}