$(document).ready(function () {
    

    var $win = $(window);
    var winHeight = $win.height();

    var lastTop;
    $win.on('scroll', function () {
        //var tagTop = $(".more-packages-foot").offset().top;
        var tabsHeight = $("#listpanel").offset().top;
        var winTop = $win.scrollTop();
        var winHeight = $win.height();

        if (winTop - tabsHeight > 0 && winTop - lastTop < 0) {
            $(".selectstartdistrict").addClass("filter-section-fixed");
        }
        else {
            $(".selectstartdistrict").removeClass("filter-section-fixed");
        }

        lastTop = winTop;
    });

    var isapp = $("#isapp").val() == "1";
    var pubuserid = $("#userid").val();

    $("._selector-model, ._close").click(function () {
        $("._selector-model").hide();
        $("._selector").hide();
    })



});

function showSelect() {
    $("._selector-model").show();
    $("._selector").show();
}

function selectedCity(cid, startDistrictId, showType) {
    $("._selector-model").hide();
    $("._selector").hide();
    location.href = "/package/collection/" + cid + "?showType=" + showType + "&startDistrictId=" + startDistrictId
}


