$("div.dptab a").click(function () {
    var $parentDiv = $(this).parents("div.dptab").eq(0);
    if (!$parentDiv.hasClass("singleDiv")) {
        var $a = $(this);
        if ($a.hasClass("select")) {
            $a.removeClass("select");
        }
        else {
            $a.addClass("select");
        }
    }
});

$("div.singleDiv a").click(function () {
    var $b = $(this);
    if (!$b.hasClass("select")) {
        $b.addClass("select");
    }
    var $neibour = $(this).siblings("a");
    if ($neibour.hasClass("select")) {
        $neibour.removeClass("select");
    }

    //清除多余定义 重新来制作
    var $parentDiv = $(this).parents("div.dptab").eq(0);
    var $div = $parentDiv.next("div");
    $div.hide();
    $div.find("input:text").val("");
});

function PreOrNextSection(isNext, currentSection, total) {
    var nextSection = 0;
    if(isNext){
        nextSection = currentSection + 1;
    }
    else{
        nextSection = currentSection - 1;
    }
    $("#currentSection").val(nextSection);//点击完更新最新的section

    $("#section_" + currentSection).hide();
    $("#section_" + nextSection).show();

    if (nextSection > 0) {
        $("#btnPreComment").show();
    }
    else {
        $("#btnPreComment").hide();
    }
    if (nextSection < total) {
        $("#btnNextComment").show();
    }
    else {
        $("#btnNextComment").hide();
    }
    if (nextSection === total) {
        $("#btnSubmitComment").show();
    }
    else {
        $("#btnSubmitComment").hide();
    }
}

$("#btnPreComment").click(function () {
    var section = $.trim($("#currentSection").val());//section相当于blockInfo数组的index
    var hotelID = $.trim($("#currentHotel").val());
    var orderID = $.trim($("#currentOrder").val());
    var final = $.trim($("#finalSection").val());

    if (!section || !final) {
        alert("参数丢失");
        return false;
    }

    var cursection = parseInt(section, 10);
    var finalSection = parseInt(final, 10);

    var $currentSectionDiv = $("#section_" + cursection);

    var $textArea = $currentSectionDiv.find("textarea").eq(0);
    var addInfo = $.trim($textArea.val());
    var catId = parseInt($textArea.attr("data-addInfoCat"),10);

    var isPass = true;
    $currentSectionDiv.find(".requiredDiv").each(function () {
        var length = $(this).find(".select").length;
        var blockCatId = parseInt($(this).attr("data-Cat"),10);
        if (blockCatId === catId) {            
            var minMaxArray = $(this).prev("input").val().split("-");
            if (minMaxArray.length !== 2) {
                alert("限制数量不全");
                isPass = false;
                return isPass;
            }
            var min = parseInt(minMaxArray[0], 10);
            var max = parseInt(minMaxArray[1], 10);
            if (min === 1 && (length == 0 && !addInfo)) {
                alert("标签块或补充信息必须选填一个哦");
                isPass = false;
                return isPass;
            }
            else if (min !== 1 && length == 0) {
                alert("标签块必须选哦");
                isPass = false;
                return isPass;
            }
        }
        else if(length == 0) {
            alert("标签块必须选哦");
            isPass = false;
            return isPass;
        }

        //var isSingle = $(this).hasClass(".singleDiv");

        //if (!isSingle && length === 0) {
        //    alert("标签块必须选哦");
        //    return false;
        //}
        //else if (isSingle && (length == 0 && !addInfo)) {
        //    alert("标签块或补充信息必须选填一个哦");
        //    return false;
        //}
    });

    if(!isPass){
        return false;
    }

    var TagIDs = [];
    var tagAddInfos = [];    

    $("div.dptab a[data-tagID].select").each(function () {
        TagIDs.push(JSON.stringify(parseInt($(this).attr("data-tagID"), 10)));

        var cat = $(this).attr("data-addInfoCat");
        if (cat) {
            var content = $.trim($(this).html());
            var catID = parseInt(cat, 10);
            var temp = { info: content, catID: catID };
            tagAddInfos.push(temp);
        }
    });

    $("textarea[data-addInfoCat]").each(function () {
        var cat = $(this).attr("data-addInfoCat");
        if (cat) {
            var content = $.trim($(this).val());
            if (content) {
                var catID = parseInt(cat, 10);
                var temp = { info: content, catID: catID };
                tagAddInfos.push(temp);
            }
        }
    });

    //if (TagIDs.length == 0) {
    //    //PreOrNextSection(false, cursection, finalSection);
    //    alert("标签需要选择");
    //    return false;
    //}

    TagIDs = JSON.stringify(TagIDs);
    tagAddInfos = JSON.stringify(tagAddInfos);

    //var ationalComment = $.trim($("#currentAdditionalInfo").val());additionalComment: ationalComment, 

    var data = { hotel: hotelID, order: orderID, tagIDs: TagIDs, section: section, tagAddInfos: tagAddInfos };
    
    $.ajax({
        type: 'POST',
        url: '/Comment/SubmitSection',
        data: data,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success === true) {
                PreOrNextSection(false, cursection, finalSection);
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("#btnNextComment").click(function () {
    var section = $.trim($("#currentSection").val());//section相当于blockInfo数组的index
    var hotelID = $.trim($("#currentHotel").val());
    var orderID = $.trim($("#currentOrder").val());
    var final = $.trim($("#finalSection").val());
    
    if (!section || !final) {
        alert("参数丢失");
        return false;
    }

    var cursection = parseInt(section,10);
    var finalSection = parseInt(final, 10);

    var $currentSectionDiv = $("#section_" + cursection);

    var $textArea = $currentSectionDiv.find("textarea").eq(0);
    var addInfo = $.trim($textArea.val());
    var catId = parseInt($textArea.attr("data-addInfoCat"), 10);

    var isPass = true;
    $currentSectionDiv.find(".requiredDiv").each(function () {
        var length = $(this).find(".select").length;
        var blockCatId = parseInt($(this).attr("data-Cat"), 10);
        if (blockCatId === catId) {
            var minMaxArray = $(this).prev("input").val().split("-");
            if (minMaxArray.length !== 2) {
                alert("限制数量不全");
                isPass = false;
                return isPass;
            }
            var min = parseInt(minMaxArray[0], 10);
            var max = parseInt(minMaxArray[1], 10);
            if (min === 1 && (length == 0 && !addInfo)) {
                alert("标签块或补充信息必须选填一个哦");
                isPass = false;
                return isPass;
            }
            else if (min !== 1 && length == 0) {
                alert("标签块必须选哦");
                isPass = false;
                return isPass;
            }
        }
        else if (length == 0) {
            alert("标签块必须选哦");
            isPass = false;
            return isPass;
        }

        //var isSingle = $(this).hasClass(".singleDiv");

        //if (!isSingle && length === 0) {
        //    alert("标签块必须选哦");
        //    return false;
        //}
        //else if (isSingle && (length == 0 && !addInfo)) {
        //    alert("标签块或补充信息必须选填一个哦");
        //    return false;
        //}
    });

    if (!isPass) {
        return false;
    }

    var TagIDs = [];
    var tagAddInfos = [];
    $("div.dptab a[data-tagID].select").each(function () {
        TagIDs.push(JSON.stringify(parseInt($(this).attr("data-tagID"), 10)));

        var cat = $(this).attr("data-addInfoCat");
        if (cat) {
            var content = $.trim($(this).html());
            var catID = parseInt(cat, 10);
            var temp = { info: content, catID: catID };
            tagAddInfos.push(temp);
        }
    });

    $("textarea[data-addInfoCat]").each(function () {
        var cat = $(this).attr("data-addInfoCat");
        if (cat) {
            var content = $.trim($(this).val());
            if (content) {
                var catID = parseInt(cat, 10);
                var temp = { info: content, catID: catID };
                tagAddInfos.push(temp);
            }
        }
    });

    //if (TagIDs.length == 0) {
    //    //PreOrNextSection(false, cursection, finalSection);
    //    alert("标签需要选择");
    //    return false;
    //}

    TagIDs = JSON.stringify(TagIDs);
    tagAddInfos = JSON.stringify(tagAddInfos);

    //var ationalComment = $.trim($("#currentAdditionalInfo").val());additionalComment: ationalComment, 
    
    var data = { hotel: hotelID, order: orderID, tagIDs: TagIDs, section: section, tagAddInfos: tagAddInfos };
    
    $.ajax({
        type: 'POST',
        url: '/Comment/SubmitSection',
        data: data,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success === true) {
                PreOrNextSection(true, cursection, finalSection);
                //window.location.href = '/Comment/Section?section=' + section + '&hotel=' + hotelID + '&order=' + orderID;
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

$("#btnSubmitComment").click(function () {
    var section = $.trim($("#currentSection").val());//section相当于blockInfo数组的index
    var hotelID = $.trim($("#currentHotel").val());
    var orderID = $.trim($("#currentOrder").val());
    var final = $.trim($("#finalSection").val());

    var data = null;
    //ToDo 如果页面增加则会导致Bug
    if (section == final) {
        var score = parseInt($("#score").val(),10);
        if(!score){
            alert("请您先打分再提交点评");
            return false;
        }
        var recommend = $("#sayyes").hasClass("select");        
        //var timeUnix = Math.round(new Date().getTime / 1000, 0);
        //var source = 3;
        //var requestType = 'SubmitComment40';
        //var sign = getSign(timeUnix, source, requestType);

        //var sign = getSign('submitComment40');
        //data = { hotel: hotelID, order: orderID, score: score, recommend: recommend, TimeStamp: timeUnix, SourceID: source, RequestType: requestType, Sign: sign };

        data = { hotel: hotelID, order: orderID, score: score, recommend: recommend, isSubmit: true };
    }
    else {
        var TagIDs = [];
        var tagAddInfos = [];
        $("div.dptab a[data-tagID].select").each(function () {
            TagIDs.push(JSON.stringify(parseInt($(this).attr("data-tagID"), 10)));

            var cat = $(this).attr("data-addInfoCat");
            if (cat) {
                var content = $.trim($(this).html());
                var catID = parseInt(cat, 10);
                var temp = { info: content, catID: catID };
                tagAddInfos.push(temp);
            }
        });

        $("textarea[data-addInfoCat]").each(function () {
            var cat = $(this).attr("data-addInfoCat");
            if (cat) {
                var content = $.trim($(this).val());
                if (content) {
                    var catID = parseInt(cat, 10);
                    var temp = { info: content, catID: catID };
                    tagAddInfos.push(temp);
                }
            }
        });

        TagIDs = JSON.stringify(TagIDs);
        tagAddInfos = JSON.stringify(tagAddInfos);

        //var ationalComment = $.trim($("#currentAdditionalInfo").val()); additionalComment: ationalComment
        data = { hotel: hotelID, order: orderID, tagIDs: TagIDs, section: section, tagAddInfos: tagAddInfos };
    }

    $.ajax({
        type: 'POST',
        url: '/Comment/SubmitSection',
        data: data,
        datatype: 'json',
        async: true,
        success: function (result) {
            if (result.Success === true) {
                section = 1 + parseInt(section, 10);
                window.location.href = '/Comment/Section?section=' + section + '&hotel=' + hotelID + '&order=' + orderID;
            }
            else if (result.Success === 0) {
                alert("提交点评成功！");
                $("#commentID").val(result.CommentID);
                var hiddenFieldList = $("#fileList").find("input");
                var files = $("#fileList").find("img");
                var length = hiddenFieldList.length;
                if (length > 0) {
                    for (var i = 0; i < length; i++) {
                        var hiddenField = hiddenFieldList.eq(i);
                        var valueStrArray = hiddenField.val().split("&");
                        var picPath = valueStrArray[0].split("=")[1];
                        var photoSecret = valueStrArray[1].split("=")[1];
                        var imageType = valueStrArray[2].split("=")[1];
                        var file_size = valueStrArray[3].split("=")[1];
                        var image_width = valueStrArray[4].split("=")[1];
                        var image_height = valueStrArray[5].split("=")[1];
                        insertNewFile(result.CommentID, picPath, photoSecret, imageType, file_size, image_width, image_height);
                    }
                    //window.location.href = "/Comment/Finish?CommentID=" + result.CommentID;//在当前页面跳转
                }
                window.location.href = '/personal/comment?isuncomment=False&start=0';//回已点评列表
                //else {
                //    alert("照片还未上传");
                //    return false;
                //}
                //ToDo跳转页面
                //$("#btnSubmitComment").hide();
                //alert("提交点评成功,可继续提交图片。");
                //$("#shareDiv").show();
                //GetHtmlOfShare(result.CommentID);
                //<wb:share-button addition="number" type="button" count="y" type="icon" size="small" pic="@" language="zh_cn" appkey="2410638867" title="@ @"></wb:share-button>
                //window.location.href = '/personal/comment';
            }
            else {
                //$("#btnSubmitComment").show();
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
});

function GetHtmlOfShare(commentID) {
    var data = { CommentID: commentID };
    $.post("/Comment/GetCommentShareInfo", data, function (result) {
        if (result) {
            var title = result.title + ':' + result.Content;
            var photo = result.photoUrl;
            var htmlStr = '<input addition="number" type="button" count="y" type="icon" size="small" pic="' + photo + '" language="zh_cn" appkey="2410638867" title="' + title + '" ></input>';
            $("#shareDiv").html(htmlStr);

            //WB2.anyWhere(function (W) {
            //    W.widget.shareButton({
            //        'id': 'wb_share',
            //        'count': 'y',
            //        'title':title,
            //        'pic':photo,
            //        'appkey': '2410638867',
            //        'callback': function (o) {
            //            alert("分享成功！");
            //        }
            //    });
            //});
        }
    });
}

$("#btnCancelComment").click(function () {
    window.location.href = '/personal/comment';
});

$("#btnReturnCommentList").click(function () {
    var isGive = window.confirm("该操作会放弃已点评的内容，确定继续吗？");
    if (isGive) {
        window.location.href = '/personal/comment';
    }
});

$("#starlist li").hover(function () {
    var num = $(this).attr("id");
    var title = $(this).attr("title");
    if (num != 'rateText') {
        InitStartList(parseInt(num, 10));
        $("#rateText").html(title);
    }
},function () {
    var score = parseInt($("#score").val(), 10);
    var title = $(this).attr("title");
    if (!score) {
        InitStartList(0);
        score2rate(0);
    }
    else {
        InitStartList(score);
        score2rate(score);
    }
});

$("#starlist li").click(function () {
    if ($(this).attr("id") === 'rateText') {
        return false;
    }
    var value = parseInt($(this).attr("id"), 10);
    var title = $(this).attr("title");
    InitStartList(value);
    $("#score").val(value);
    score2rate(value);
});

function InitStartList(yellowNum) {
    /*初始化状态*/
    if (typeof yellowNum == 'number') {
        if (yellowNum == 0) {
            $("#starlist").find(".yellowstar").hide();
            $("#starlist").find(".whitestar").show();
        }
        else if (yellowNum == 1) {
            $("#starlist").find(".yellowstar").hide();
            $("#starlist").find(".whitestar").show();
            $("#1").find("img").eq(0).show();
            $("#1").find("img").eq(1).hide();
        }
        else if (yellowNum == 2) {
            $("#starlist").find(".yellowstar").hide();
            $("#starlist").find(".whitestar").show();
            $("#1").find("img").eq(0).show();
            $("#2").find("img").eq(0).show();
            $("#1").find("img").eq(1).hide();
            $("#2").find("img").eq(1).hide();
        }
        else if (yellowNum == 3) {
            $("#starlist").find(".yellowstar").show();
            $("#starlist").find(".whitestar").hide();
            $("#4").find("img").eq(0).hide();
            $("#5").find("img").eq(0).hide();
            $("#4").find("img").eq(1).show();
            $("#5").find("img").eq(1).show();
        }
        else if (yellowNum == 4) {
            $("#starlist").find(".yellowstar").show();
            $("#starlist").find(".whitestar").hide();
            $("#5").find("img").eq(0).hide();
            $("#5").find("img").eq(1).show();
        }
        else if (yellowNum == 5) {
            $("#starlist").find(".yellowstar").show();
            $("#starlist").find(".whitestar").hide();
        }
    }
}

function score2rate(score) {
    if (score === 0) {
        $("#rateText").html("");
    }
    else if (score === 1) {
        $("#rateText").html("非常差");
    }
    else if (score === 2) {
        $("#rateText").html("较差");
    }
    else if (score === 3) {
        $("#rateText").html("一般");
    }
    else if (score === 4) {
        $("#rateText").html("较好");
    }
    else if (score === 5) {
        $("#rateText").html("非常好");
    }
}

function insertNewFile(commentID, photoSURL, photoSecret, photoType, photoSize, photoWidth, photoHeight) {
    var timeUnix = Math.round(new Date().getTime / 1000, 0);
    var source = 3;
    var requestType = 'InsertCommnetPhoto40';
    //var sign = getSign(timeUnix, source, requestType);
    var data2 = {
        AppID: source, CommentID: commentID, PhotoSURL: photoSURL, PhotoSecret: photoSecret, PhotoType: photoType, PhotoSize: photoSize, PhotoWidth: photoWidth,
        PhotoHeight: photoHeight, TimeStamp: timeUnix, SourceID: source, RequestType: requestType, Sign: ""
    };

    $.ajax({
        type: 'POST',
        url: '/Comment/InsertCommnetPhoto40',
        data: data2,
        datatype: 'json',
        async: false,
        success: function (result) {
            var commentId = parseInt($("#commentID").val(), 10);
            if(!$("#commentID").val()){
                $("#commentID").val(commentId);
            }
            //if (result.Success === 0) {
            //    //alert("提交图片成功！");
            //}
            //else {
            //    //alert("提交图片不成功！");
            //}
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

$("#writeComment").click(function () {
    //var isAllow = true;
    //判断是否可以写点评
    var hotelID = parseInt($(this).attr("data-value"),10);
    var data = { hotelID: hotelID };

    //$.ajax({
    //    type: 'POST',
    //    url: '/Comment/GetUserCanWriteCommentOrderID',
    //    data: data,
    //    datatype: 'json',
    //    async: false,
    //    success: function (result) {
    //        if (result.orderID === 0) {
    //            alert(result.msg);
    //            isAllow = false;
    //        }
    //    },
    //    error: function () {
    //        alert('网络异常，请重试！');
    //    }
    //});

    $.get('/Comment/GetUserCanWriteCommentOrderID', data, function (result) {
        if (result.orderID != 0) {
            if (!window.isMobile) {
                var newwindow = window.open();
                newwindow.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=" + result.orderID;
            }
            else {
                window.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=" + result.orderID;
            }
        }
        else if (result.orderID === 0 && result.canWrite) {
            if (!window.isMobile) {
                var newwindow = window.open();
                newwindow.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=0";
            }
            else {
                window.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=0";
            }
        }
        else {
            alert(result.msg);
            return false;
        }
    });
});

$(document).ready(function () {
    var search = window.location.search;
    var path = window.location.pathname;
    if (path === "/personal/comment") {
        var flag = search === "" || search.indexOf("isuncomment=True", 0) !== -1;
        if (flag) {
            if (!$("#undonecomment").hasClass("cur")) {
                $("#undonecomment").addClass("cur");
            }
            $("#donecomment").removeClass("cur");
        }
        else {
            if (!$("#donecomment").hasClass("cur")) {
                $("#donecomment").addClass("cur");
            }
            $("#undonecomment").removeClass("cur");
        }
    }
});

/*点评成功后的页面 组件内的html无法获得*/
//$("#shareLink").click(function(){
//    $("a.share_btn").click();
//});

//设置点评地址
function changeConfig() {
    var shareUrl = $(this).attr("data-tag");
    $("#shareUrl").val(shareUrl);
}

$("a.bds_weixin").hover(changeConfig, null);

$("a.bds_tsina").hover(changeConfig, null);


/*写点评的验证 包括标签块选择数量的范围 弹出文本框写替换旧的标签内容*/

$("div.dptab a").click(function () {
    var $parentDiv = $(this).parents("div.dptab").eq(0);
    if (!$parentDiv.hasClass("singleDiv")) {
        var minMaxArray = $parentDiv.prev("input").val().split("-");
        if (minMaxArray.length !== 2) {
            alert("限制数量不全");
            return false;
        }
        var min = parseInt(minMaxArray[0], 10);
        var max = parseInt(minMaxArray[1], 10);
        if (max === 0) {
            alert("最多为0不可选");
            return false;
        }
        else if (min > max) {
            alert("最小值超过最大值");
            return false;
        }
        else {
            var chosenNum = $parentDiv.find("a.select").length;
            if (chosenNum === max+1) {
                //ToDo 最多选max个tag，请考虑一下吧
                //$parentDiv.after('<div><span>最多能选' + max + '个标签哦，考虑一下~</span></div>');
                alert('最多能选' + max + '个标签哦，考虑一下~');
                if ($(this).hasClass("select")) {
                    $(this).removeClass("select");//取消多选的样式
                }
                return false;//阻止之后的弹出文本框操作
            }
        }
    }
});

$("div.dptab a[data-addInfoTip]").click(function () {
    var $parentDiv = $(this).parents("div.dptab").eq(0);
    //ToDo tag块下方出现文本框(缺省是tip) 填入新值替换旧值
    var tip = $.trim($(this).attr("data-addInfoTip"));
    var catID = $.trim($(this).attr("data-addInfoCat"));
    var tagID = $.trim($(this).attr("data-tagID"));
    
    var $div = $parentDiv.next("div");
    var $text = $div.find("input:text");
    $text.attr("placeholder", tip);
    $text.attr("data-addInfoCat", catID);
    $text.attr("data-tagID", tagID);
    $text.val("");
    $text.focus();
    $div.show();
});

$("div input:text").keypress(function (e) {
    var keyCode = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
    if (keyCode == 13) {
        var tagID = $(this).attr("data-tagID");
        var value = $.trim($(this).val());
        if (!value) {
            alert("标签内容不能为空");
            return false;
        }
        $(this).parents("div").eq(0).prev("div.dptab").find("a.select").each(function () {
            var tagID2 = $(this).attr("data-tagID");
            if (tagID === tagID2) {
                $(this).html(value);
            }
        });

        var $text = $(this);
        $text.parents("div").eq(0).hide();
        $text.val("");
    }
});