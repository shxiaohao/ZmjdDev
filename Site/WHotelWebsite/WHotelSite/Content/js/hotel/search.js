var searchTimers;
var lastSearchKey;
$("#topSearchInput").keydown(function () {
    clearTimeout(searchTimers);
});
$("#topSearchInput").keyup(function () {
    searchTimers = setTimeout(quickSerach, 500);
});

function quickSerach() {
    var keyword = $.trim($("#topSearchInput").val());
    if (keyword == lastSearchKey) return;
    lastSearchKey = $.trim($("#topSearchInput").val());
    var offset = $("#topSearchDiv").offset();
    var leftPX = offset.left + "px";
    var topPX = (offset.top + $("#topSearchDiv")[0].offsetHeight) + "px";
    $(".pcsearchhotel").css({ position: "absolute", left: leftPX, top: topPX });

    //test ...
    //$('.pcsearchhotel .quicklist2 ul').remove();
    //$("<div>test test</div>").appendTo('.pcsearchhotel .quicklist2');
    //$(".pcsearchhotel").removeClass("hidden");

    if (keyword) {
        $.get('/Portal/SearchList', { keyword: keyword, page: $("#pagetag").val() }, function (html) {
            if ($(html).length > 0) {
                $('.pcsearchhotel .quicklist2 ul').remove();
                $(html).appendTo('.pcsearchhotel .quicklist2');
                $(".pcsearchhotel").removeClass("hidden");
            }
            else {
                $('.pcsearchhotel .quicklist2 ul').remove();
                $('.pcsearchhotel').addClass("hidden");
            }
        });
    }
    else {
        $('.pcsearchhotel .quicklist2 ul').remove();
        $('.pcsearchhotel').addClass("hidden");
    }
}

$('#topSearch').on('click', function () {
    var keyword = $('#topSearchInput').val();
    $(".pcsearchhotel").removeClass("hidden");

    var offset = $("#topSearchDiv").offset();
    var leftPX = offset.left + "px";
    $(".pcsearchhotel").css({ position: "absolute", left: leftPX });

    $('.pcsearchhotel .quicklist2 ul').remove();
    if (keyword) {
        $.get('/Portal/SearchList', { keyword: keyword, page: $("#pagetag").val() }, function (html) {
            if ($(html).length > 0) {
                $(html).appendTo('.pcsearchhotel .quicklist2');
            }
            else {
                $('.pcsearchhotel').addClass("hidden");
            }
        });
    }
    else {
        $('.pcsearchhotel').addClass("hidden");
    }
});

$(".pcsearchhotel .close").click(function () {
    var $searchResultDiv = $(this).parents(".pcsearchhotel");
    $searchResultDiv.addClass("hidden");
});