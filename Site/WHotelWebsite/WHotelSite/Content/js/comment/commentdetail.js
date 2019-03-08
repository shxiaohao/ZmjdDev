$(function ()
{
    //============小图轮播=============
    var isdrag = false;
    var tx, x;
    var window_width = parseFloat($(window).width() * 0.92);
    var move_width = parseFloat($("#moveid").width());

    if (move_width > window_width) {
        document.getElementById("moveid").addEventListener('touchend', moveend);
        document.getElementById("moveid").addEventListener('touchstart', selectmouse);
        document.getElementById("moveid").addEventListener('touchmove', movemouse);
    }

    function movemouse(e) {
        if (isdrag) {
            var n = tx + e.touches[0].pageX - x;
            $("#moveid").css("left", n);
            return false;
        }
    }

    function selectmouse(e) {
        isdrag = true;
        tx = parseInt(document.getElementById("moveid").style.left + 0);
        x = e.touches[0].pageX;
        return false;
    }
    function moveend(e) {
        sdrag = false;

        var window_width = parseFloat($(window).width() * 0.92);
        var move_width = parseFloat($("#moveid").width());
        var left = parseInt($("#moveid").css("left").replace("px", "").replace("PX", ""));

        var num1 = parseInt(0 - left);
        var num2 = parseFloat(move_width - window_width);

        //如果小图已经左/右拖到头了，则初始左/右位置
        if (move_width > window_width) {
            if (left > 0) {
                $("#moveid").animate({ left: "0px" }, 300);//.css("left", 0);
            }
            else if (left < 0) {
                if (num1 - num2 > 0) {
                    $("#moveid").animate({ left: (0 - num2 + 5) }, 300);//.css("left", 0 - num2 + 5);
                }
            }
        }
    }

    function jumpSlidePic() {
        var index = $(this).data("index");
        slider.slide(index, 300);
    }

    //小图的click event
    $(".pic-item").each(function () {
        $(this).click(jumpSlidePic);
    });

    //=========================

    //==============大图轮播===============
    var slider =
    Swipe(document.getElementById('slider'), {
        auto: 0,
        continuous: true,
        callback: function (pos) {

            var window_width = parseFloat($(window).width() * 0.92);
            var move_width = parseFloat($("#moveid").width());
            var left = parseInt($("#moveid").css("left").replace("px", "").replace("PX", ""));

            //大图切换后，动态选中小图状态
            $(".pic-item").each(function () { $(this).addClass("pic-item-not"); });
            var thisPic = $("#pic-item-" + pos);
            thisPic.removeClass("pic-item-not");

            //如果小图被遮盖了，则调整小图模块left让其可见
            var offset = thisPic.offset();
            var thisWith = parseInt(thisPic.css("width").replace("px", "").replace("PX", ""));
            var addLeft = 0;
            if (offset.left - 15 < 0) {
                addLeft = 0 - offset.left + 15;
                $("#moveid").animate({ left: left + addLeft }, 200);//.css("left", left + addLeft);
            }
            else if (offset.left + thisWith > window_width) {
                addLeft = left + (window_width - (offset.left + thisWith));
                $("#moveid").animate({ left: addLeft }, 200);//.css("left", addLeft);
            }
        }
    });

    //==============================
});