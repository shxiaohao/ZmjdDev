var pubuserid = parseInt($("#userid").val());
var _name = $("#_name").val();
var _hotelSourceUrl = $("#_hotelSourceUrl").val();

// Change Dates
window.isMobile || $(function () {
	Calendar.ctripTwins('#checkIn', '#checkOut', window.calendarOptions, function (data) {
		if (Calendar.parse(data.checkOut) > Calendar.parse(data.checkIn)) {
			showSpinner(true);
			location.search = '?' + $.param(data);
		}
	});

	showSpinner.prefetch();
});



$(function () {

    var _starNum = parseFloat($("#hotelScore").val());
    var _starHtml = getScoreHtml(_starNum);
    $("#hotel-star-span").html(_starHtml);

    //购买须知
    var showShopRead = function () {
        $(".shopread-btn").hide();
        $(".shopread").fadeIn();
        }
    $(".shopread-btn").click(showShopRead);

    //pricebar top
    var _priceTopBar = $('.price-top-bar');
    var _priceTopBarOriTop = $('.price-top-bar').offset().top;

    //加载酒店图文详情
    var loadHotelSource = false;
    var loadHotelSourcePage = function () {

        if (loadHotelSource) return;
        if (_hotelSourceUrl) {

            loadHotelSource = true;

            //alert("b～");
            console.log("load source");

            //_hotelSourceUrl = "/active/activepage?midx=5886";
            _hotelSourceUrl = _hotelSourceUrl.replace("http://www.zmjiudian.com", "");

            $("#hotel-source-body").load(_hotelSourceUrl, function (response, status, xhr) {

                if (status === "success") {
                    $(".source-more-btn").show();
                    $(".source-more-btn").click(function () {

                        $(".source-more-btn").hide();
                        $("#hotel-source-body").css("max-height", "100%");
                    });
                }
                else {
                    $(".hotel-source").hide();
                }
            });
        }
        else {
            $(".hotel-source").hide();
        }
    }

    var $win = $(window);
    var hotelSourceTop = _hotelSourceUrl ? $(".hotel-source").offset().top : 0;

    $(window).scroll(function (){

            var winTop = $win.scrollTop();
            var winHeight = $win.height();

            //价格悬浮（体验不好...）
            //if (winTop < _priceTopBarOriTop) {
            //    _priceTopBar.removeClass("price-top-bar-fixed");
            //}
            //else {
            //    if (_priceTopBar.attr("class").indexOf("fixed") < 0) {
            //        _priceTopBar.addClass("price-top-bar-fixed");
            //    }
            //}

            //console.log(winTop)

            if (winTop > 0 && winTop > (hotelSourceTop - winHeight)) {
                loadHotelSourcePage();
            }

        });

});

//成为VIP
var goBuyVip = function () {

    //记录当前页，告知VIP购买成功后可以再跳回来
    Global.UrlReferrer.Set({ 'name': _name, 'url': location.href, 'imgsrc': '' });

    location.href = "/Account/VipRights?userid=" + pubuserid + "&_isoneoff=1&_newpage=1";
}