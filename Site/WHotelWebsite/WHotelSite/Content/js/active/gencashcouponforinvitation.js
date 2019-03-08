$(function () {

    var userId = $("#userId").val();
    var nickName = $("#nickName").val();
    var invitationCode = $("#invitationCode").val();

    //初始mobile login
    var loginCheckFun = function () {
        location.reload(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        alert("请您先登录");
        return false;
    }
    
    $(".go-getcash-btn").click(function () {

        _loginModular._followUserId = parseInt(userId);
        _loginModular._followNickName = nickName;
        _loginModular._invitationCode = invitationCode;
        _loginModular.init(loginCheckFun, loginCancelFun, true);

        _loginModular.show();

        $("#page2").hide();
    });

    //加载地步的优惠套餐列表
    setTimeout(function () {
        var flashDic = { "albumId": 10, "start": 0, "count": 10, "curUserID": 4514792, "ckvip": 1 };
        $.get("http://api.zmjiudian.com/api/hotel/GetRecommendHotelResultByAlbumId", flashDic, function (_data) {
            console.log(_data)
            if (_data && _data.HotelList && _data.HotelList.length > 0) {
                //debugger;
                _data.HotelList.map(function (item, index) {
                    if (!item.HotelPicUrl) {
                        item.HotelPicUrl = "http://whfront.b0.upaiyun.com/app/img/home/home-load-3x2.png";
                    }
                });

                _data["albumTitle"] = "专享优惠套餐";
                _data["albumId"] = 0;

                new Vue({
                    el: '#flashDealsSection',
                    data: { "AlbumsInfo": _data }
                })

                $("#flashDealsSection").show();

                //Banners
                $('#flashDealsSection .home-hlist-panel').swiper({
                    slidesPerView: 'auto',
                    offsetPxBefore: 25,
                    offsetPxAfter: 25,
                    onTouchEnd: function (slider) {
                        if (slider.activeIndex + 1 < slider.slides.length) {
                            var li = $(slider.slides[slider.activeIndex + 1]);
                            var imgObj = li.find("img");
                            setImgOriSrc(imgObj);
                        }
                    }
                })
            }
        });
    },
	0);

    var setImgOriSrc = function (imgObj) {
        var orisrc = imgObj.data("orisrc");
        if (orisrc && orisrc != null && orisrc != "" && orisrc != undefined && orisrc != "undefined") {
            var defsrc = imgObj.attr("src");
            imgObj.attr("src", orisrc);
            imgObj.data("orisrc", "");
            imgObj.error(function () {
                imgObj.attr("src", defsrc);
            });
        }
    };

});

