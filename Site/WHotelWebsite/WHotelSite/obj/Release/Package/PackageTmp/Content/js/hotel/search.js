$("#topSearchInput").keyup(function () {
    //var timeOutId = setTimeout(null, 2000);
    //舍弃当前的settimeout执行队列
    //clearTimeout(timeOutId);
    quickSerach();
});

function quickSerach() {
    var keyword = $.trim($("#topSearchInput").val());
    var offset = $("#topSearchDiv").offset();
    var leftPX = offset.left + "px";
    $(".pcsearchhotel").css({ position: "absolute", left: leftPX });

    if (keyword) {
        $.get('/Portal/SearchList', { keyword: keyword }, function (html) {
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
        $.get('/Portal/SearchList', { keyword: keyword }, function (html) {
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