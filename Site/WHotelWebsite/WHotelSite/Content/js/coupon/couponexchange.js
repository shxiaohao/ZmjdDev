var pubuserid = $("#userid").val();
var isapp = $("#isapp").val() == "1";
var isSingleSelectDate = $("#isSingleSelectDate").val() == "1";
var exchangeTagId = parseInt($("#exchangeTagId").val());
var hotelId = $("#hotelId").val();
var pid = $("#packageId").val();
var activityType = parseInt($("#activityType").val());
var oriCouponPrice = $("#oriCouponPrice").val();
var defCheckIn = $("#checkIn").val();
var defCheckOut = $("#checkOut").val();

var maxPersonNum = parseInt($("#maxPersonNum").val());
var minPersonNum = parseInt($("#minPersonNum").val());
var cartTypeListStr = $("#cartTypeList").val();
var cartTypeList = cartTypeListStr.split(',');
var travelPersonDesc = $("#travelPersonDesc").val();
var dateSelectType = parseInt($("#dateSelectType").val());
var dateTypeName = $("#dateTypeName").val();
var userLabel = $("#userLabel").val();
var userPlaceholder = $("#userPlaceholder").val();

var Options = $("#hidOptions").val();
var Notes = $("#hidNotes").val();
var divOptionsID = "divOptions";
var displayTipTag = $("#displayTipTag").val();
var tipTag = "";//提示语
 

//是否已经选择日期
var selectedDate = false;

$(document).ready(function () {

    //初始mobile login
    var loginCheckFun = function () {
        reloadPage(true);//刷新当前页 F5，true从服务器端重启，false从浏览器缓存取，不适合页面method='post'，
    }

    var loginCancelFun = function () {
        return true;
    }

    _loginModular.init(loginCheckFun, loginCancelFun);

    //init calendar
    var calendar = null;

    $(".packagereduce").click(function () {
        var canexchangenum = $("#exchangenum").val();
        if (canexchangenum <= 1) return;
        $("#exchangenum").val(canexchangenum - 1);

        $(".appendhtml:last").remove();
    })
    $(".packageplus").click(function () {
        var canexchangenum = $("#exchangenum").val();
        //if (canexchangenum <= 1) return;
        $("#exchangenum").val(++canexchangenum);
        var userLabel = $("#userLabel").val();
        var userPlaceholder = $("#userPlaceholder").val();

        var html = "<div class='basic-panel top-users appendhtml'><div class='left-panel'><span>" + userLabel + "</span></div><div class='right-panel'><input type='text' name='ex-checkinUser' class='top-users-input' placeholder='" + userPlaceholder + "' value='' /></div><div style='clear:none;'></div></div>";

        $(".appendhtml:last").after(html);
    })

    // main calendar
    var onSelect = function (newCheckIn, newCheckOut)
    {
        if (checkInfo(newCheckIn, newCheckOut))
        {
            if (!selectedDate) {
                $("#def-datetxt").hide();
                $("#dates").show();
                selectedDate = true;
            }

            $("#checkIn").val(Calendar.format(newCheckIn));
            $("#checkOut").val(Calendar.format(newCheckOut));
                    
            var cin = formatDate(newCheckIn, "yyyy/MM/dd");
            var cout = formatDate(newCheckOut, "yyyy/MM/dd");

            if (activityType == 500) {
                changeDiffPrice(cin, cout);
            }

            if (isSingleSelectDate) {
                $(".top-checktime-item").html(cin);
            }
            else {
                $(".top-checktime-item").html(cin + " - " + cout);
            }
             
            updateOptions(cin, cout);


        }
    };

    var updateOptions = function(cin , cout)
    {
        var pars = {hotelid:hotelId, pid : pid, checkIn : cin, checkOut : cout};

        $.get("/Coupon/GetOnePackageRoomOptions", pars, function (_data) {
            Options = _data;
            fillOptions(divOptionsID, Options, Notes);
        }); 
    }

    var fillOptions = function(divOptionsID,  Options, notes)
    {

        var divOption = $("#" + divOptionsID);
        var strOptions ="";
        var gid = 1;
        $.each(Options.split("|"), function (index, str) {
            $.each(str.split(","), function (index, item) {
                if (item) {
                    var on = notes.indexOf(item) >= 0;
                    if (on) {
                        notes.remove(item);
                    }
                    if (item.indexOf("满房") == -1)
                    {
                        strOptions += "<div class=\"item-div\"><input name=\"o" + gid + "\" type=\"radio\" value=\"" + item + "\" ><label fn=\"o" + gid + "\">" + item + "</label></div>"
                    }
                    
                }
            });
            gid += 1;         
        });

        divOption.html(strOptions);
    } 

    //更新差价
    var changeDiffPrice = function (cin, cout) {

        var diffDic = { hotelid: hotelId, pid: pid, cin: cin, cout: cout, oriCouponPrice: oriCouponPrice, };
        $.get("/Coupon/GetExchangeDiffPrice", diffDic, function (_data) {
            //console.log(_data)
            if (_data) {
                
                var diffPrice = parseInt(_data.Price);
                if (diffPrice > 0) {
                    $(".top-diffprice").find(".pri").html(_data.Price);
                    $(".top-diffprice").show();
                }
                else {
                    $(".top-diffprice").find(".pri").html("0");
                    $(".top-diffprice").hide();
                }
            }
        });

    }

    //show calendar
    calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
    
    //日期单选
    calendar.isSingleSelect = isSingleSelectDate;

    //显示入住日期事件
    var showCalendar = function () {

        calendar.show();

        //根据不同的
        if (isSingleSelectDate) {
            $(".calendar .ctrls .tip").html("*灰色日期为不可订");
        }

        if (selectedDate) {
            //def calendar selectRange
            var cin = $("#checkIn").val();
            var cout = $("#checkOut").val();
            calendar.selectRange(new Date(cin), new Date(cout));
        }
        else {
            calendar.selectRange();
        }
    }
    //select calendar
    $("#dates").click(showCalendar);
    $("#def-datetxt").click(showCalendar);

    if (activityType == 500) {

        if (exchangeTagId <= 0) {

            //更新差价
            changeDiffPrice(defCheckIn, defCheckOut);

            //默认显示
            calendar.show();

            //def calendar selectRange
            var cin = $("#checkIn").val();
            var cout = $("#checkOut").val();
            calendar.selectRange(new Date(cin), new Date(cout));
        }
    }

    showSpinner.prefetch();

    //申请兑换
    $(".ex-exchange-btn").click(function ()
    {
        //套餐是否下线
        var isValid = $("#isValid").val();

        if (!isValid) {          
            _Modal.show({
                title: "",
                content: "遇到问题啦，请联系客服",
                textAlign: "center",
                confirm: function () {
                    _Modal.hide();
                }
            });
            return;
        }


        if (!selectedDate) {
            //alert(dateTypeName + "尚未选择");
            _Modal.show({
                title: "",
                content: dateTypeName + '尚未选择',
                textAlign: "center",
                confirm: function () {
                    _Modal.hide();
                }
            });
            return;
        }

        //联系人
        var userElementList = $(".top-users-input");
        var contact = "";
        for (var i = 0; i < userElementList.length; i++)
        {
            var userItem = $(userElementList[i]);
            if (userItem.val().length > 0) {
                if (contact != "") contact += ",";
                contact = contact + userItem.val();
            }
            else {
                //alert("入住人姓名不完整");
                _Modal.show({
                    title:"",
                    content: userLabel + '不完整',
                    textAlign: "center",
                    confirm: function () {
                        _Modal.hide();
                    }
                });
                return;
            }
        }

        //travel person
        if (minPersonNum > 0) {
            if (addPersonSelecteds.length < minPersonNum) {
                return alert('此套餐需至少添加' + minPersonNum + "名出行人");
            }
        }

        //手机号
        var contactPhone = $("#ex-phone").val();

        //券码
        var exchangeNo = $("#exchangeNo").val();

        //入住日期
        var checkIn = $("#checkIn").val();
        //离店日期
        var checkOut = $("#checkOut").val();

        //几晚
        var checkInDate = Calendar.parse($('#checkIn').val());
        var checkOutDate = Calendar.parse($('#checkOut').val());
        var nightCount = parseInt(formatToDays(checkOutDate - checkInDate));
        if (!nightCount) nightCount = 1;

        //几间
        var roomCount = userElementList.length;

        //套餐ID
        var packageID = $("#packageId").val();

        //套餐ID
        var packageType = $("#packageType").val();


        //特殊要求与备注
        var note = "";
        var remark = $("#ex-otherInfo").val();

        //解析特殊要求项
        var noteOptions = "";
        var icheckList = $(".icheck-panel").find("input");
        icheckList.each(function ()
        {
            var ckItem = $(this)[0];
            if (ckItem && ckItem.checked) {
                if (noteOptions != "") noteOptions += " ";
                noteOptions += $(ckItem).val();
            }
        });

        note = (noteOptions == "" ? "" : noteOptions + " ");
        note = note + (remark == "" ? "" : " " + remark);


        if (Options.indexOf("床") > 0 && noteOptions.indexOf("床") < 0) {
            return alert("请选择床型。");
        }

        //用户ID
        var userId = $("#userid").val();

        //酒店ID
        var hotelId = $("#hotelId").val();

        var dic = {};
        dic["contact"] = contact;
        dic["contactPhone"] = contactPhone;
        dic["exchangeNo"] = exchangeNo;
        dic["checkIn"] = checkIn;
        dic["checkOut"] = checkOut;//离店日期
        dic["nightCount"] = nightCount;
        dic["roomCount"] = roomCount;
        dic["packageID"] = packageID;
        dic["packageType"] = packageType;
        dic["note"] = note;
        dic["userId"] = userId;
        dic["hotelId"] = hotelId;
        dic["travelPersons"] = getTravelPersonsStr();
        dic["useCouponNum"] = 1;
        //console.log(dic)
        //return;
        var useCount;
        showSpinner(true);



        //check
        $.get('/Coupon/CheckExChange', dic, function (content)
        {
            //var msg = content.Message;
            //var suc = content.Success; //suc = "1";
            //var useCount = content.useCouponCount;


            var msg = content.Message;
            var suc = content.Success; //suc = "1";
            dic["useCouponNum"] = content.useCouponNum;//一共使用的房券数量
            //test code
            //showSpinner(false);
            //if (confirm("您购买的是平日券，选择了周末入住。因此需要补差价100元。点击确认则认为您已同意补差价并将继续提交兑换申请，点击取消则放弃本次兑换，房券可以继续使用。")) {
            //    showSpinner(true);
            //    //subFun(dic);
            //}
            //return;


            if (suc == "0")
            {
                subFun(dic);
            }
            else if (suc == "1")
            {
                showSpinner(false);//是否显示弹出层

                if (msg != "") {//如果msg为空串，表示满足兑换情况，以前会提示使用几张房券提示语，但和下面的重复，故而，在这里就直接跳到下面，统一显示提示语了。
                    if (confirm(msg)) {//此步骤包含，补汇款确认提示。
                        showSpinner(true);
                        subFun(dic);
                    }

                } else {//再api返回的结果中，若满足兑换情况，不需要confirm提示信息了，直接执行下面subFun即可。
                    subFun(dic);

                }
            }
            else
            {
                //特殊处理...将“整数倍”相关字样临时过滤 2018.07.20 haoy
                msg = msg.replace("单张房券", "本房券");
                msg = msg.replace("可兑换", "需要兑换");
                msg = msg.replace("，不是整数倍", "");

                showSpinner(false);
                alert(msg);
            }
        });

    });

    function subFun(dic)
    {// roomCount == defaultRoomCount 


        //对于两间夜或者一间夜说明提示

        if (displayTipTag == 1) {//if (!couponPackage.isAllowMultiRoom)此处不允许多间夜时tipTag=""，值=0。默认允许多间,值=1

            if (dic["roomCount"] != dic["nightCount"]) {
                tipTag = "（如果想兑换" + dic["roomCount"] + "晚" + dic["nightCount"] + "间，请重新选择" + dateTypeName + "，房间数改为" + dic["nightCount"] + "。）";
            }
        }


        _Modal.show({
            title: "",
            //content: "请检查兑换" + dateTypeName + "，订单一经确认不可取消或更改。 ",
            content: "本次兑换:" + dateTypeName + dic["checkIn"] + "，房间数量为 " + dic["roomCount"] + ",需要使用" + dic["useCouponNum"] + "张房券。请检查所选日期，订单一经确认不可取消更改。" + tipTag + "",
            textAlign: "center",
            showClose: false,
            showCancel: true,
            confirmText: '确认兑换',
            confirm: function () {
                
                _Modal.hide();

                $.get('/Coupon/SubmitExChange', dic, function (content) {

                    showSpinner(false);

                    var msg = content.Message;
                    var suc = content.Success; //suc = "1";

                    if (suc == "0") {
                        location.reload();
                    }
                    else {

                        //特殊处理...将“整数倍”相关字样临时过滤 2018.07.20 haoy
                        msg = msg.replace("单张房券", "本房券");
                        msg = msg.replace("可兑换", "需要兑换");
                        msg = msg.replace("，不是整数倍", "");

                        //alert(msg);
                        _Modal.show({
                            title: "",
                            content: msg,
                            textAlign: "center",
                            confirm: function () {
                                _Modal.hide();
                            }
                        });
                    }
                });

            },
            cancel: function () {
                _Modal.hide();
                showSpinner(false);
                return;
            }
        });

        //if (!confirm("请核实兑换" + dateTypeName + "，已经确认不可更改与取消")) {
        //    showSpinner(false);
        //    return;
        //}
    }
});

function checkInfo(checkIn, checkOut)
{
    //总的间晚数
    var nightRoomCount = parseInt($("#nightRoomCount").val());

    //当前选择的入住天数
    var days = parseInt(formatToDays(checkOut - checkIn));

    //入住天数不能大于/等于总的间晚数
    //if (days > nightRoomCount)
    //{
    //    alert("您最多可以选择入住" + nightRoomCount + "晚");
    //    //bootbox.alert({
    //    //    message: "<div class='alert-rulesmsg'>您最多可以选择入住" + nightRoomCount + "晚</div>",
    //    //    buttons: {
    //    //        ok: {
    //    //            label: '确认',
    //    //            className: 'btn-default'
    //    //        }
    //    //    },
    //    //    callback: function (result) {

    //    //    },
    //    //    closeButton: false
    //    //});
    //    return false;
    //}

    ////入住天数必须可以被除尽
    //if (!isSingleSelectDate && !zhengchu(nightRoomCount, days))
    //{
    //    alert("该套餐为" + nightRoomCount + "间夜套餐，不能选择" + days + "晚入住");
    //    //bootbox.alert({
    //    //    message: "<div class='alert-rulesmsg'>该套餐为" + nightRoomCount + "间夜套餐，不能选择" + days + "晚入住</div>",
    //    //    buttons: {
    //    //        ok: {
    //    //            label: '确认',
    //    //            className: 'btn-default'
    //    //        }
    //    //    },
    //    //    callback: function (result) {

    //    //    },
    //    //    closeButton: false
    //    //});
    //    return false;
    //}

    ////根据已选择的入住天数，来得出还可以入住几个人
    //var roomCount = (nightRoomCount == days ? 1 : nightRoomCount);
    
    ////获取当前入住姓名控件
    //var userElementList1 = $(".top-users-input");

    ////动态生成指定数量的控件
    //var userItemsHtml = "";
    //for (var i = 0; i < roomCount; i++)
    //{
    //    userItemsHtml += $("#ex-user-item-template").html();
    //}
    //$("#ex-user-array").html(userItemsHtml);

    ////将之前已经输入的姓名重新赋值
    //var userElementList2 = $(".top-users-input");
    //for (var i = 0; i < userElementList2.length; i++)
    //{
    //    var userItem = $(userElementList2[i]);
    //    if (userItem && userElementList1 && userElementList1.length > i && $(userElementList1[i]).val().length > 0)
    //    {
    //        userItem.val($(userElementList1[i]).val());
    //    }
    //}

    return true;
}

function zhengchu(x, y)
{
    var z = parseInt( x / y);
    if (z * y == x) {
        return true;
    }
    return false;
}

function formatDate(date, format)
{
    if (!date) return;
    if (!format) format = "yyyy-MM-dd";
    switch (typeof date) {
        case "string":
            date = new Date(date.replace(/-/, "/"));
            break;
        case "number":
            date = new Date(date);
            break;
    }
    if (!date instanceof Date) return;
    var dict = {
        "yyyy": date.getFullYear(),
        "M": date.getMonth() + 1,
        "d": date.getDate(),
        "H": date.getHours(),
        "m": date.getMinutes(),
        "s": date.getSeconds(),
        "MM": ("" + (date.getMonth() + 101)).substr(1),
        "dd": ("" + (date.getDate() + 100)).substr(1),
        "HH": ("" + (date.getHours() + 100)).substr(1),
        "mm": ("" + (date.getMinutes() + 100)).substr(1),
        "ss": ("" + (date.getSeconds() + 100)).substr(1)
    };
    return format.replace(/(yyyy|MM?|dd?|HH?|ss?|mm?)/g, function () {
        return dict[arguments[0]];
    });
}

function formatToDays(sm)
{
    return sm / 1000 / 60 / 60 / 24;
}


/*出行人 相关处理*/

//所有出行人信息
var allPersonList = {};

//初始加载当前用户的出行人信息
var allPersonList = [];
var allPersonVue = {};

//添加出行人初始list
var addPersonAdds = [];
var addPersonSelecteds = [];
var addPersonVue = {};

//编辑出行人Vue相关
//0 add     1 edit
var personEditType = 0;
var personEditVue = {};

var getPerson = function (num) {
    return allPersonList[num];
}

//show win
var showSelPerson = function () {

    //如果需要添加出行人，但没有userid，则需要首先验证手机号获取userid
    if (pubuserid == "0") {
        _loginModular.show();
        return;
    }

    $(".b-d-win-panel").show();
    $(".b-d-win-model").show();
}

//hide win
var hideSelPerson = function () {
    $(".b-d-win-panel").hide();
    $(".b-d-win-model").hide();
}

var getTravelPersonsStr = function () {
    var _str = "";
    if (addPersonSelecteds) {
        addPersonSelecteds.map(function (item, index) {
            if (_str != "") _str += ",";
            _str = _str + item.ID;
        });
    }

    return _str;
}

//back selperson
$("#back-selperson").click(function () {

    //返回操作，统计当前选择的出行人信息，以及是否合法
    var cklist = $("#ad-person input:checkbox[name='person-ck']:checked");
    if (cklist.length > maxPersonNum) {
        alert("最多选择" + maxPersonNum + "位出行人");
        return;
    }

    var _repeatckstate = true;
    var _repeatcks = [];
    var _addsles = [];
    cklist.map(function (index, item) {
        var _num = parseInt($(item).data("psnum"));
        var _person = getPerson(_num);

        //check repeat
        var _ckkey = _person.IDType + _person.IDNumber;
        if ($.inArray(_ckkey, _repeatcks) > -1) {
            _repeatckstate = false;
            return;
        }
        _repeatcks.push(_ckkey);

        _addsles.push(_person);
    });

    if (!_repeatckstate) {
        alert("请勿选择证件号码重复的出行人，请核对");
        return;
    }

    addPersonSelecteds = _addsles;

    refAddPersonOptions();

    hideSelPerson();
});

//show edit
var showEditPerson = function (num) {
    $("#ad-person").hide();
    $("#edit-person").show();

    //add
    if (personEditType == 0) {

        personEditVue.person = {
            "ID": 0,
            "UserID": pubuserid,
            "TravelPersonName": "",
            "IDType": 1,
            "IDNumber": "",
            "Birthday": "1990-01-01"
        };
    }
        //edit
    else if (personEditType == 1) {

        var _person = getPerson(num);
        _person.Birthday = formatDate(parseInt(_person.Birthday.slice(6)), "yyyy-MM-dd");

        personEditVue.person = _person;
    }
}

var delSelPerson = function (index) {
    addPersonSelecteds.baoremove(index);
    bindPersonList();
}

$("#add-person-btn").click(function () {
    personEditType = 0;
    showEditPerson();
});

//save editperson
$("#save-editperson").click(function () {

    var _dic = {};
    _dic["id"] = personEditVue.person.ID;
    _dic["userid"] = pubuserid;
    _dic["travelPersonName"] = personEditVue.person.TravelPersonName;
    _dic["idType"] = personEditVue.person.IDType;
    _dic["idnumber"] = personEditVue.person.IDNumber;
    _dic["birthday"] = personEditVue.person.Birthday;

    //add
    if (personEditType == 0) {
        _dic["saveType"] = 0;
    }
        //edit
    else if (personEditType == 1) {
        _dic["saveType"] = 1;
    }

    if (_dic["travelPersonName"].length <= 0) {
        alert("请如实填写姓名");
        return;
    }
    else if (_dic["idnumber"].length <= 0) {
        alert("请如实填写证件号码");
        return;
    }
    else if (_dic["idType"] != 1 && _dic["birthday"].length <= 0) {
        alert("请如实填写出生日期");
        return;
    }

    $.get('/Hotel/SaveTravelPerson', _dic, function (back) {

        if (back.Success) {

            bindPersonList();

            $("#edit-person").hide();
            $("#ad-person").show();
        }
        else {
            alert("出新人信息保存错误");
        }
    });
});

//cancel edit person
$("#back-editperson").click(function () {
    $("#edit-person").hide();
    $("#ad-person").show();
});

var bindPersonList = function () {
    $.get('/Hotel/GetTravelPersonByUserId', { userid: pubuserid }, function (back) {

        //person list
        allPersonList = back;
        allPersonList.map(function (item, index) {

            //身份证加星
            item["IDNumber2"] = plusXing(item.IDNumber, 4, 4);

            //checkbox state
            item["ck"] = false;
            addPersonSelecteds.map(function (item2, index2) {
                if (item.ID == item2.ID) {
                    item["ck"] = true;
                    return;
                }
            });

            //select state
            item["select"] = true;
            if (("," + cartTypeListStr + ",").indexOf("," + item.IDType + ",") < 0) {
                item["select"] = false;
            }

        });
        allPersonVue.list = allPersonList;
    });

    //生成“添加出行人”项
    refAddPersonOptions();
};

var refAddPersonOptions = function () {

    addPersonAdds = [];

    for (var _i = 0; _i < maxPersonNum - addPersonSelecteds.length; _i++) {

        addPersonAdds.push({});
    }

    //add options
    addPersonVue.adds = addPersonAdds;

    //selected options
    addPersonVue.sels = addPersonSelecteds;
};

var loginCheckFun = function (userid) {
    //alert(userid);
    location.href = location.href + "&userid=" + userid;
}

var loginCancelFun = function () {

    alert(travelPersonDesc + "\r\n" + "添加出行人需先登录");
    return true;
}

var initPersonInfo = function () {

    //绑定全部出行人信息
    //allPersonList = [
    //    { "ID": 1, "TravelPersonName": "小豪", "IDNumber": "372922198902046219", "Birthday": "1999-01-01" },
    //    { "ID": 2, "TravelPersonName": "小强", "IDNumber": "388888199002028888", "Birthday": "1999-02-02" }
    //];

    //初始添加出行人
    if (maxPersonNum > 0) {

        _loginModular.init(loginCheckFun, loginCancelFun);

        //init add person options
        addPersonVue = new Vue({
            el: "#person-panel",
            data: {
                "adds": addPersonAdds,
                "sels": addPersonSelecteds
            }
        })

        //init person list
        allPersonVue = new Vue({
            el: '#ad-person',
            data: { "list": allPersonList }
        })
        bindPersonList();

        //init edit person
        var _newperson = {
            "ID": 0,
            "UserID": pubuserid,
            "TravelPersonName": "",
            "IDType": 1,
            "IDNumber": "",
            "Birthday": "1990-01-01"
        };
        personEditVue = new Vue({
            el: '#edit-person',
            data: {
                "person": _newperson
            }
        })

        $("#person-panel").show();
    }
}
initPersonInfo();







