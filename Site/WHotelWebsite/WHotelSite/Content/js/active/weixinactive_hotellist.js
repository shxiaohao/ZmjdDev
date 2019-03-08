$(function () {

    var _menuleft = $(".menu .left");
    var _menuright = $(".menu .right");
    _menuleft.click(function () {
        var s = $(this).data("s");
        if (s == "0") {
            $(this).addClass("selected");
            $("#list0").show();
            $(this).data("s", "1");

            _menuright.removeClass("selected");
            $("#list1").hide();
            _menuright.data("s", "0");

            $("html,body").animate({ scrollTop: $("#list0").offset().top - 5 }, 300);
        }
    });

    _menuright.click(function () {
        var s = $(this).data("s");
        if (s == "0") {
            $(this).addClass("selected");
            $("#list1").show();
            $(this).data("s", "1");

            _menuleft.removeClass("selected");
            $("#list0").hide();
            _menuleft.data("s", "0");

            $("html,body").animate({ scrollTop: $("#list1").offset().top - 5 }, 300);
        }
    });

    $("img").lazyload({
        threshold: 20,
        placeholder: "http://whfront.b0.upaiyun.com/app/img/home/home-load2-16x9.png",
        effect: "show"
    });

});