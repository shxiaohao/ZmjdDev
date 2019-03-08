var isapp = $("#isapp").val() == "1";
var inWeixin = $("#isInWeixin").val() == "1";
var canShareRedCoupon = $("#canShareRedCoupon").val() == "1";
var appShareLink = $("#appShareLink").val();

$(function () {

    //非团购 & 非积分 & 非免费领取 产品，购买完成后弹出的红包分享
    if (canShareRedCoupon) {

        //延时加载活动说明
        setTimeout(function () {

            //分享提示图片动态加载
            var shareTipImg = $(".group-share-tip img");
            setImgOriSrc(shareTipImg);

        }, 100);

        //右上角分享提示点击事件
        $(".group-share-tip").click(function () {
            $(this).hide();
        });

        //红包分享事件
        var shareRedCoupon = function () {

            if (inWeixin) {
                $(".group-share-tip").show();
            }
            else if (isapp) {
                gourl(appShareLink);
            }
        }

        //右上角发红包浮窗
        $(".send-red-coupon-float").click(shareRedCoupon);
        var showRedCouponFloat = function () {
            $(".send-red-coupon-float").fadeIn();
        }

        var _html = $("#send-red-coupon-template").html();

        _Modal.show({
            title: '',
            content: _html,
            showClose: true,
            confirmText: '分享领红包',
            confirm: function () {

                //分享
                shareRedCoupon();

                //显示浮窗
                showRedCouponFloat();

                _Modal.hide();
            },
            cancel: function () {

                //显示浮窗
                showRedCouponFloat();
            },
            showCancel: false
        });

        //$("._modal-section").css("top", "15%");
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