$(function () {

    //// main calendar
    //var checkIn = Calendar.parse($('#checkIn').val());
    //var checkOut = Calendar.parse($('#checkOut').val());
    //var onSelect = function (newCheckIn, newCheckOut) {
    //    if (newCheckIn - checkIn || newCheckOut - checkOut) {
    //        location.search = '?' + $.param({
    //            checkIn: Calendar.format(newCheckIn),
    //            checkOut: Calendar.format(newCheckOut)
    //        });
    //        $('<div class="spinner dark"><a></a></div>').appendTo(document.body).find('a').css('opacity', 0).animate({ opacity: 1 }, 400);
    //    }
    //};
    ////入住日期控件
    //var calendar = null;
    //$('#dates').on('click', function () {
    //    calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
    //    calendar.show();
    //    calendar.selectRange(checkIn, checkOut);
    //});
    //确认提交入住
    $("#subbookin").click(function () {
        var dic = {};
        dic["id"] = $(this).data("id");
        dic["hotelid"] = $(this).data("hotelid");
        dic["userid"] = $(this).data("user");
        dic["checkin"] = $("#checkIn").val();
        dic["checkout"] = $("#checkOut").val();
        $.get('/Inspector/SubmitInspectorHotel', dic, function (content) {
            var msg = content.Message;
            var suc = content.Success;

            if (suc == 0) {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>提交成功<br />我们的客服会尽快联系您。</div>",
                    buttons: {
                        ok: {
                            label: '确定',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {
                        location.href = "/inspector/hotellist";
                    },
                    closeButton: false
                });
            }
            else {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>提交失败<br />请您联系客服。</div>",
                    buttons: {
                        ok: {
                            label: '确定',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {
                        //location.href = "/inspector/hotellist";
                    },
                    closeButton: false
                });
            }
        });
    });
});