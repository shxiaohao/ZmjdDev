$(function () {
    var key = $("#guidcid").data("keyguid");
    var cid = $("#guidcid").data("cid");

    var guid = Store.Get("key");
    //if (guid == key)
    //{
    //    window.location.href = "/Coupon/RedCashCouponResult?key=" + key + "&cid=" + cid;
    //}

    Store.Set("key", key);
    Store.Set("cid", cid);

    //查看我的奖品
    $(".open-coupon").click(function () {

        var _html = $("#get-coupon-info-template").html();

        _Modal.show({
            title: '',
            content: _html,
            confirmText: '我知道了',
            confirm: function () {
                _Modal.hide();
            },
            showCancel: false
        });

        $("._modal-section").css("top", "15%");
    });
})
function downLoadApp() {
    window.open("http://app.zmjiudian.com/");
}
function LookMore() {
    var key = $("#guidcid").data("keyguid");
    var cid = $("#guidcid").data("cid");
    window.location.href = "/Coupon/RedCashCouponResult?key=" + key + "&cid=" + cid + "&all=all";

}