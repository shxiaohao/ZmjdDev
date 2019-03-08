var pubuserid = $("#userid").val();
var exchangeid = $("#exchangeid").val();
var skuid = $("#skuid").val();
var isapp = $("#isapp").val() == "1";
var isSingleSelectDate = $("#isSingleSelectDate").val() == "1";
var exchangeTagId = parseInt($("#exchangeTagId").val());
var hotelId = $("#hotelId").val();
var pid = $("#packageId").val();
var activityType = parseInt($("#activityType").val());
var oriCouponPrice = $("#oriCouponPrice").val();
var defCheckIn = $("#checkIn").val();
var defCheckOut = $("#checkOut").val();

var writeOtherPostion = parseInt($("#writeOtherPostion").val());
var maxPersonNum = parseInt($("#maxPersonNum").val());
var minPersonNum = parseInt($("#minPersonNum").val());
var cartTypeListStr = $("#cartTypeList").val();
var cartTypeList = cartTypeListStr.split(',');
var travelPersonDesc = $("#travelPersonDesc").val();
var dateSelectType = parseInt($("#dateSelectType").val());
var dateTypeName = $("#dateTypeName").val();
var userLabel = $("#userLabel").val();
var userPlaceholder = $("#userPlaceholder").val();
var preReserve = $("#preReserve").val() == "1";
var paynum = $("#paynum").val();
var fromwxuid = $("#fromwxuid").val();
var groupid = $("#groupid").val();
var openid = $("#openid").val();
var coid = $("#coid").val();

var Options = $("#hidOptions").val();
var Notes = $("#hidNotes").val();
var divOptionsID = "divOptions";
 
var _Config = new Config();

//_Config.APIUrl = "http://192.168.1.114:8000";
//_Config.APIUrl = "http://api.zmjd100.com";

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

    // main calendar
    var selectDateId = "0";
    var onSelect = function (newCheckIn, newCheckOut, selectId)
    {
        selectDateId = selectId;

        console.log(selectDateId);

        if (!selectedDate) {
            $("#def-datetxt").hide();
            $("#dates").show();
            selectedDate = true;
        }

        $("#checkIn").val(Calendar.format(newCheckIn));
        $("#checkOut").val(Calendar.format(newCheckOut));

        var cin = formatDate(newCheckIn, "yyyy/MM/dd");
        var cout = formatDate(newCheckOut, "yyyy/MM/dd");

        if (isSingleSelectDate) {
            $(".top-checktime-item").html(cin);
        }
        else {
            $(".top-checktime-item").html(cin + " - " + cout);
        }

        //更新场次信息
        updateShows();
    };

    //更新场次信息
    var showsOk = true;
    var dateShowData = null;
    var updateShows = function()
    {
        showsOk = false;

        var _dic = { skuid: skuid, bookDateId: selectDateId };

        $.get(_Config.APIUrl + "/api/Coupon/GetBookDetailByBookDateId", _dic, function (_data) {
            
            showsOk = true;

            console.log(_data);

            if (dateShowData) {
                dateShowData.listData = _data;
            }
            else {
                dateShowData = new Vue({
                    el: '#top-shows',
                    data: { "listData": _data }
                })
            }

            if (_data && _data.length) {
                $("#top-shows").show();
            }

        });

    }

    //show calendar
    calendar || (calendar = new Calendar(onSelect, window.calendarOptions));
    
    //日期单选
    calendar.isSingleSelect = isSingleSelectDate;

    //显示入住日期事件
    var showCalendar = function () {

        calendar.showValid();

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

    //加载模板表单
    var formSectionData = null;
    var loadFormSection = function () {

        var _dic = { skuid: skuid };

        $.get(_Config.APIUrl + "/api/Coupon/GetSKUTempSource", _dic, function (_data) {

            console.log(_data);

            if (_data.TemplateItem) {

                //将模板json字符串转换为对象
                var templateItemStr = _data.TemplateItem;
                var templateItemObjs = JSON.parse(templateItemStr);
                console.log(templateItemObjs);
                _data.TemplateItemObjs = templateItemObjs;

                if (formSectionData) {
                    formSectionData.formData = _data;
                }
                else {
                    formSectionData = new Vue({
                        el: '#form-section',
                        data: { "formData": _data }
                    })
                }

                $("#form-section").show();
            }
            else {
                formSectionData = {};
                formSectionData.formData = _data;
            }

        });
        
    }
    loadFormSection();

    showSpinner.prefetch();

    //申请兑换
    $(".ex-exchange-btn").click(function ()
    {
        if (!selectedDate) {
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

        if (!showsOk) {
            _Modal.show({
                title: "",
                content: '操作太快了，请稍后',
                textAlign: "center",
                confirm: function () {
                    _Modal.hide();
                }
            });
            return;
        }

        //场次ID
        var bookDetailId = "0";
        var rdShows = $('input:radio[name="rd-shows"]');
        if (dateShowData && rdShows && rdShows.length) {
            var rdShowChecked = $('input:radio[name="rd-shows"]:checked');
            bookDetailId = rdShowChecked.val();
            console.log(bookDetailId);
            if (!bookDetailId) {
                _Modal.show({
                    title: "",
                    content: "请选择预约场次",
                    textAlign: "center",
                    confirm: function () {
                        _Modal.hide();
                    }
                });
                return;
            }
        }

        //travel person
        if (writeOtherPostion == "2") {
            if (cartTypeList && cartTypeList.length && minPersonNum > 0) {
                if (addPersonSelecteds.length < minPersonNum) {
                    _Modal.show({
                        title: "",
                        content: "需至少添加" + minPersonNum + "名出行人",
                        textAlign: "center",
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                    return;
                }
            }
        }

        console.log(formSectionData);

        //模板表单信息
        var bookTempData = formSectionData.formData;

        //debugger;

        //验证表单录入
        if (bookTempData.TemplateItemObjs) {
            for (var i = 0; i < bookTempData.TemplateItemObjs.length; i++) {
                var titem = bookTempData.TemplateItemObjs[i];
                if (titem.MustWrite===2 && !titem.Content) {
                    _Modal.show({
                        title: "",
                        content: "请填写" + titem.Name,
                        textAlign: "center",
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                    return;
                }
            }
        }
        bookTempData.BizId = exchangeid;
        bookTempData.TemplateItem = JSON.stringify(bookTempData.TemplateItemObjs);

        var _exids = [];
        if (parseInt(exchangeid)) {
            _exids.push(parseInt(exchangeid));
        }

        //提交信息
        var submitDic = {
            skuid: skuid,
            ExchangCouponIds: _exids,
            UserID: pubuserid,
            BookDateId: selectDateId,
            BookDetailId: bookDetailId,
            TemplateData: bookTempData,
            TravelId: getTravelPersonsList()
        };
        console.log("提交信息：")
        console.log(submitDic);

        //如果是预约前置，则需要跳转到券订单提交页面
        if (preReserve) {

            //预约缓存Key
            var _reserveCacheKey = "CouponReserve_{0}_{1}".format(skuid, pubuserid);

            //清楚旧的缓存预约信息
            Store.Set(_reserveCacheKey, null);

            //缓存预约信息
            Store.Set(_reserveCacheKey, submitDic);

            gourl("/coupon/couponbook?skuid={0}&paynum={1}&userid={2}&fromwxuid={3}&groupid={4}&openid={5}&coid={6}&_isoneoff=1&_newpage=1".format(skuid, paynum, pubuserid, fromwxuid, groupid, openid, coid));
            return;
        }
        else {

            //继续预约

            showSpinner(true);

            $.post(_Config.APIUrl + "/api/Coupon/SubmitBookInfo", submitDic, function (_result) {

                console.log(_result);

                if (_result.RetCode === "1") {

                    try {

                        //刷新订单详情页
                        Global.Monito.Publisher("coupon", "couponorderdetail", 1);

                    } catch (e) {

                    }

                    _Modal.show({
                        title: "",
                        content: "你已预约成功!",
                        textAlign: "center",
                        confirm: function () {

                            _Modal.hide();

                            if (isapp) {

                                //app环境下的返回刷新机制
                                var param = {
                                    "refresh": 1,   //是否刷新:1
                                    "data": {},
                                }
                                zmjd.pageBack(param);
                            }
                            else {
                                history.back();
                            }
                        }
                    });

                }
                else {
                    _Modal.show({
                        title: "",
                        content: _result.Message,
                        textAlign: "center",
                        confirm: function () {
                            _Modal.hide();
                        }
                    });
                }

                showSpinner(false);
            });
        }

        return;
    });
});

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

var getTravelPersonsList = function () {
    //var _str = "";
    //if (addPersonSelecteds) {
    //    addPersonSelecteds.map(function (item, index) {
    //        if (_str != "") _str += ",";
    //        _str = _str + item.ID;
    //    });
    //}

    //return _str;

    var _list = [];
    if (addPersonSelecteds) {
        addPersonSelecteds.map(function (item, index) {
            _list.push(item.ID);
        });
    }

    return _list;
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

    if (_dic["travelPersonName"].AllTrim().length <= 0) {
        alert("请如实填写姓名");
        return;
    }
    else if (_dic["idnumber"].AllTrim().length <= 0) {
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
    }

    $("#person-panel").show();
}
if (writeOtherPostion == "2") {
    initPersonInfo();
}