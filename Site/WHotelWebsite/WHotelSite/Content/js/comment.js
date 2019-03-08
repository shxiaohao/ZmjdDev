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

$(".commentDefaultBlock textarea").keydown(function () {
    var date = new Date();
    var timeOutId = setTimeout(checkWordLength(this), 400);
    if ((new Date() - date) < 400) {
        clearTimeout(timeOutId);
        timeOutId = setTimeout(checkWordLength(this), 400);
    }
});

$(".commentDefaultBlock textarea").change(function () {
    var date = new Date();
    var timeOutId = setTimeout(checkWordLength(this), 400);
    if ((new Date() - date) < 400) {
        clearTimeout(timeOutId);
        timeOutId = setTimeout(checkWordLength(this), 400);
    }
});

function checkWordLength(obj) {
    var $textArea = $(obj);
    var addInfoVal = $textArea.val();//内容
    var maxWordCount = parseInt($textArea.attr("data-maxwordcount"), 10);//内容的最大字符限制
    if (!!addInfoVal && maxWordCount > 0 && addInfoVal.length > maxWordCount) {
        alert("最多输入" + maxWordCount + "个字符");
        $textArea.val(addInfoVal.substring(0, maxWordCount));
        return false;
    }
}

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
    var catId = parseInt($textArea.attr("data-addInfoCat"), 10);

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

    TagIDs = JSON.stringify(TagIDs);
    tagAddInfos = JSON.stringify(tagAddInfos);

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
    
    TagIDs = JSON.stringify(TagIDs);
    tagAddInfos = JSON.stringify(tagAddInfos);

        
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

function insertNewFile(commentID, blockTagCatID, photoSURL, photoSecret, photoType, photoSize, photoWidth, photoHeight,fileName,catID) {
    var timeUnix = Math.round(new Date().getTime / 1000, 0);
    var source = 3;
    var requestType = 'InsertCommnetPhoto40';
    if(!blockTagCatID){
        blockTagCatID = -1;
    }
    
    var data2 = {
        AppID: source, CommentID: commentID, TagBlockCategory: blockTagCatID, PhotoSURL: photoSURL, PhotoSecret: photoSecret, PhotoType: photoType, PhotoSize: photoSize, PhotoWidth: photoWidth,
        PhotoHeight: photoHeight, TimeStamp: timeUnix, SourceID: source, RequestType: requestType, Sign: ""
    };
    $.ajax({
        type: 'POST',
        url: '/Comment/InsertCommnetPhoto40',
        data: data2,
        datatype: 'json',
        async: true,
        success: function (result) {
            window.picUpload.totalLength--;
            window.picUpload.delPic(catID, fileName);
            if (window.picUpload.totalLength === 0) {
                showSpinner(false);
                alert("点评提交成功！");
                window.location.href = '/personal/comment?isuncomment=False&start=0';//回已点评列表
            }
        },
        error: function () {
            alert('网络异常，请重试！');
        }
    });
}

//该酒店是否可写点评
$("#writeComment").click(function () {
    var hotelID = parseInt($(this).attr("data-value"),10);
    var data = { hotelID: hotelID };

    $.get('/Comment/GetUserCanWriteComment', data, function (result) {
        if (result.orderID != 0 && result.canWrite) {
            //if (!window.isMobile) {
            //    var newwindow = window.open();
            //    newwindow.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=" + result.orderID;
            //}
            //else {
            //    window.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=" + result.orderID;
            //}
            window.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=" + result.orderID;
        }
        else if (result.orderID === 0 && result.canWrite) {
            //if (!window.isMobile) {
            //    var newwindow = window.open();
            //    newwindow.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=0";
            //}
            //else {
            //    window.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=0";
            //}
            window.location.href = "/Comment/Section?hotel=" + hotelID + "&section=0&order=0";
        }
        else {
            alert(result.Message);
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

$(".customTagContent").keypress(function (e) {
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

function submitComment() {
    var commentID = parseInt($("#commentID").val(), 10);
    if (commentID == 0) {
        submitCommentContent();
    }
    else{
        submitCommentPics(commentID);
    }
}

function submitCommentPics(commentID) {    
    var picsCount = 0;
    for (var catID in window.picUpload.picDics) {
        if (window.picUpload.picDics.hasOwnProperty(catID)) {
            var files2 = window.picUpload.picDics[catID];
            if(!!files2 && files2.length > 0){
                picsCount += files2.length;
            }
        }
    }
    if (picsCount !== 0) {
        window.picUpload.totalLength = picsCount;
        var config = {
            bucket: 'whphoto',
            expiration: parseInt((new Date().getTime() + 3600000) / 1000),
            form_api_secret: 'Mbu7g+t64a0dWPfPpkzEUEiKJHc='
        };
        var instance = new Sand(config);
        instance.setPicCount(picsCount);

        for (var catID in window.picUpload.picDics) {
            if (window.picUpload.picDics.hasOwnProperty(catID)) {
                var files = window.picUpload.picDics[catID];
                if (!!files && files.length > 0) {
                    submitCommentPicsByCat2(commentID, catID, files, instance);
                }
            }
        }
        showSpinner(true);
    }
}

function submitCommentContent(){
    var section = $.trim($("#currentSection").val());//section相当于blockInfo数组的index
    var hotelID = $.trim($("#currentHotel").val());
    var orderID = $.trim($("#currentOrder").val());
    var final = $.trim($("#finalSection").val());

    var data = null;
    //ToDo 如果页面增加则会导致Bug
    if (section == final) {
        var score = parseInt($("#score").val(), 10);
        if (!score) {
            alert("请您先打分再提交点评");
            return false;
        }
        var recommend = $("#sayyes").hasClass("select");
        var TagIDs = [];
        var tagAddInfos = [];
        $("div.lastSection a[data-tagID].select").each(function () {
            TagIDs.push(JSON.stringify(parseInt($(this).attr("data-tagID"), 10)));

            var cat = $(this).attr("data-addInfoCat");
            if (cat) {
                var content = $.trim($(this).html());
                var catID = parseInt(cat, 10);
                var temp = { info: content, catID: catID };
                tagAddInfos.push(temp);
            }
        });

        $("div.lastSection textarea[data-addInfoCat]").each(function () {
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

        data = { hotel: hotelID, order: orderID, section: section, score: score, recommend: recommend, isSubmit: true, tagIDs: TagIDs, tagAddInfos: tagAddInfos };
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
            }
            else if (result.Success === 0) {
                //alert("点评提交成功,正在上传点评图片...");
                $("#commentID").val(result.CommentID);
                var picCount = $(".fileList").find("img").length;
                if (picCount > 0) {
                    var id_setTimeout = setTimeout(timeOut_ReturnList, 45000);
                    submitCommentPics(result.CommentID);
                }
                else {
                    alert("点评提交成功,即将进入已点评列表页...");
                    window.location.href = '/personal/comment?isuncomment=False&start=0';//回已点评列表
                }
            }
            else {
                alert(result.Message + "，点评提交失败！");
            }
        },
        error: function () {
            alert('网络出现异常错误，请联系网站管理员！');
        }
    });
}

function timeOut_ReturnList(){
    alert("上传图片操作已超时,请重新提交");
    showSpinner(false);
}

function submitCommentPicsByCat(commentID, catID, fileInput, instance) {
    var files = fileInput.files;
    var length = files.length;
    if(length === 0 || commentID === 0 || catID === 0){
        return false;
    }

    var contentSecret = getcontentSecret(8) || 'whhotels';
    var options = {
        'notify_url': 'http://upyun.com',
        'content-secret': contentSecret,
        'commentID':commentID
    };

    instance.setOptions(options);
    for (var i = 0; i < length; i++){
        var file = files[i];
        var fileName = file.name;
        var ext = '.' + fileName.split('.').pop();
        var newPicName = getPicPath(commentID, i, catID);
        var PhotoSURL = newPicName + ext;
        var picPath = '/' + PhotoSURL;
        instance.upload(picPath, file, catID, i);
    }
}

function submitCommentPicsByCat2(commentID, catID, files, instance) {
    var length = files.length;
    if (length === 0 || commentID === 0 || catID === 0) {
        return false;
    }

    var contentSecret = getcontentSecret(8) || 'whhotels';
    var options = {
        'notify_url': 'http://upyun.com',
        'content-secret': contentSecret,
        'commentID': commentID
    };

    instance.setOptions(options);
    for (var i = 0; i < length; i++) {
        var file = files[i];
        var fileName = file.name;
        var ext = '.' + fileName.split('.').pop();
        var newPicName = getPicPath(commentID, i, catID);
        var PhotoSURL = newPicName + ext;
        var picPath = '/' + PhotoSURL;
        instance.upload(picPath, file, catID, i);
    }
}

addEventHandler(document, 'uploadfinish', function (e) {
    //单张图片上传完成 后续处理
    var res = e.detail;
    if (res.path) {
        var picPath = res.path;
        var bucket_name = res.bucket_name;
        var image_width = res.image_width;
        var image_height = res.image_height;
        var mimetype = res.mimetype;
        var imageType = mimetype.split('/')[1];
        var file_size = res.file_size;
        var photoSecret = res.contentsecret;
        var commentID = res.commentID;
        var blockTagCatID = res.TagBlockCategory;
        insertNewFile(commentID, blockTagCatID, picPath, photoSecret, imageType, file_size, image_width, image_height,"匿名图片",-1);
        if (res.picsCount <= 0) {
            showSpinner(false);
            alert("点评提交成功,即将进入已点评列表页...");
            window.location.href = '/personal/comment?isuncomment=False&start=0';//回已点评列表
        }
        if (res._failPicArray.length > 0) {
            console.log(res._failPicArray);
        }
    }
});

function addEventHandler(objectelement, eventname, eventhandler) {
    if (document.attachEvent)//ie浏览器
    {
        objectelement.attachEvent("on" + eventname, eventhandler);
    }
    else if (document.addEventListener) //非ie浏览器
    {
        objectelement.addEventListener(eventname, eventhandler, false);
    }
}

