$(function ()
{
    var initOptionClick = function ()
    {
        var options = $(".option-item");
        options.each(function () {
            var tp = $(this).data("type");
            $(this).click(function ()
            {
                showSelectPanel(tp);
            });
        });
    };

    var showSelectPanel = function (tp)
    {
        var selectObj = $(".select-panel-" + tp);
        selectObj.fadeIn();
        $(".select-panel-model").show();
    };

    var initSelectPanel = function (setObj)
    {
        var typeid = setObj.data("typeid");
        var options = setObj.find(".options");
        var itemList = options.find(".item");

        //set height
        var winHeight = $(window).height();
        setObj.css("height", winHeight - 30);
        options.css("height", winHeight - 135);

        //build ok click
        var okBtn = setObj.find(".btns .ok");
        okBtn.click(function ()
        {
            var optionitem = $(this).data("optionitem");

            //hide
            $(".select-panel-model").hide();
            setObj.fadeOut();

            //set value
            var getValObj = $("." + optionitem).find(".right .val");
            var setTxt = getSelItemTxt(itemList);
            if (setTxt == "") {
                getValObj.text("选择");
                getValObj.removeClass("val-ok");
            }
            else {
                getValObj.text(setTxt);
                getValObj.addClass("val-ok");
            }
            
            //set info
            var setInfo = getSelItemInfo(typeid, itemList);
            $("." + optionitem).data("saveinfo", setInfo);
        });

        //init item click
        itemList.each(function ()
        {
            var thisItem = $(this);

            thisItem.click(function ()
            {
                //是否支持多选
                var mulsel = setObj.data("mulsel");
                if (mulsel == "0")
                {
                    itemList.each(function () { $(this).removeClass("item-sel"); $(this).data("s", "0"); });
                }
                selectOptionItemClick($(this));
            });

            var input = thisItem.find("input");
            if (input) {
                input.blur(function ()
                {
                    thisItem.data("val", $(this).val());
                    if ($(this).val() != "") {
                        thisItem.addClass("item-sel");
                        thisItem.data("s", "1");
                    }
                    else {
                        thisItem.removeClass("item-sel");
                        thisItem.data("s", "0");
                    }
                });
            }
        });
    };

    var selectOptionItemClick = function (thisObj)
    {
        var s = thisObj.data("s");
        if (s == "1") {
            thisObj.removeClass("item-sel");
            thisObj.data("s", "0");
        }
        else {
            thisObj.addClass("item-sel");
            thisObj.data("s", "1");
        }
    };

    var getSelItemTxt = function (itemList)
    {
        var txt = "";
        itemList.each(function ()
        {
            if ($(this).data("s") == "1")
            {
                if (txt != "") txt += ",";
                txt += $(this).data("val");
            }
        });

        return txt;
    };

    var getSelItemInfo = function (typeid, itemList) {
        var val = "";
        itemList.each(function ()
        {
            if ($(this).data("s") == "1")
            {
                if (val != "") val += ";";

                var id = $(this).data("id");
                var itemVal = typeid + "," + id + "," + $(this).data("val");

                val += itemVal;
            }
        });

        return val;
    };

    var subFun = function ()
    {
        var userid = $("#userId").val();
        //if (userid == "" || userid == "0") {
        //    alert("用户信息有误，提交失败");
        //    return;
        //}

        var regname = $("#regname").val();
        var regtell = $("#regtell").val();

        //获取4种tag类型分别的设置信息
        var tags = "";
        if (tags != "") tags += ";";
        tags += $(".option-item-work").data("saveinfo");
        if (tags != "") tags += ";";
        tags += $(".option-item-hotel").data("saveinfo");
        if (tags != "") tags += ";";
        tags += $(".option-item-level").data("saveinfo");
        if (tags != "") tags += ";";
        tags += $(".option-item-travel").data("saveinfo");

        var dic = {};
        dic["userId"] = userid;
        dic["tags"] = tags;
        dic["name"] = regname;
        dic["tell"] = regtell;
        dic["mail"] = "";

        $.get('/Account/SaveUserTag', dic, function (content)
        {
            var msg = content.Message;
            var suc = content.Success;

            switch (suc) {
                case 0:
                    {
                        goRegisterCompleted(userid);
                        break;
                    }
                default:
                    {
                        alert("抱歉，提交失败");
                        break;
                    }
            }
        });
    };

    //跳转至报名成功页面
    var goRegisterCompleted = function (userid) {
        location.href = "/inspector/RegisterCompleted?userid=" + userid;
    }

    initOptionClick();
    initSelectPanel($(".select-panel-work"));
    initSelectPanel($(".select-panel-hotel"));
    initSelectPanel($(".select-panel-level"));
    initSelectPanel($(".select-panel-travel"));
    $(".sub-panel .sub").click(subFun);
});