$(function () {

    var bs = $("#bs").val();
    var houseCount = parseInt($("#houseCount").val());
    var nightCount = parseInt($("#nightCount").val());

    //init calendar
    var calendar = null;

    //确认提交入住
    $("#subbookin").click(function () {
        var dic = {};
        dic["id"] = $(this).data("id");
        dic["hotelid"] = $(this).data("hotelid");
        dic["userid"] = $(this).data("user");
        dic["checkin"] = $("#checkIn").val();
        dic["checkout"] = $("#checkOut").val();
        dic["nightCount"] = nightCount;
        dic["bs"] = bs;

        $.get('/Inspector/SubmitInspectorHotel', dic, function (content) {
            var msg = content.Message;
            var suc = content.Success; //suc = 0;

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
                        //show calendar
                        calendar.show();

                        location.href = "/inspector/hotellist?userid=" + dic["userid"];
                    },
                    closeButton: false
                });
            }
            else if(suc == -1) {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>" + msg + "</div>",
                    buttons: {
                        ok: {
                            label: '确定',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {
                        //show calendar
                        calendar.show();
                    },
                    closeButton: false
                });
            }
            else {
                bootbox.alert({
                    message: "<div class='alert-rulesmsg'>" + msg + "</div>",
                    buttons: {
                        ok: {
                            label: '确定',
                            className: 'btn-default'
                        }
                    },
                    callback: function (result) {
                        //show calendar
                        calendar.show();

                        location.href = "/inspector/hotellist?userid=" + dic["userid"];
                    },
                    closeButton: false
                });

                //bootbox.alert({
                //    message: "<div class='alert-rulesmsg'>提交失败<br />请您联系客服。</div>",
                //    buttons: {
                //        ok: {
                //            label: '确定',
                //            className: 'btn-default'
                //        }
                //    },
                //    callback: function (result) {
                //        //show calendar
                //        calendar.show();
                //    },
                //    closeButton: false
                //});
            }
        });
    });

    // main calendar
    var checkIn = Calendar.parse($('#checkIn').val());
    var checkOut = Calendar.parse($('#checkOut').val());
    var onSelect = function (newCheckIn, newCheckOut) {
        if (newCheckIn - checkIn || newCheckOut - checkOut) {

            $("#checkIn").val(Calendar.format(newCheckIn));
            $("#checkOut").val(Calendar.format(newCheckOut));
            $("#subbookin").click();

            //location.search = '?' + $.param({
            //    checkIn: Calendar.format(newCheckIn),
            //    checkOut: Calendar.format(newCheckOut)
            //});
            //$('<div class="spinner dark"><a></a></div>').appendTo(document.body).find('a').css('opacity', 0).animate({ opacity: 1 }, 400);
        }
    };

    //show calendar
    calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
    calendar.show();//.selectRange(checkIn, checkOut);

    $("#confirm").html("确定");
});