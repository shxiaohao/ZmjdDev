$(document).ready(function () {
    window.URL = window.URL || window.webkitURL;
    if (judgeIE()) {
        Array.prototype.indexOf = function (val) {
            for (var i = 0; i < this.length; i++) {
                if (this[i] == val) {
                    return i;
                }
            }
            return -1;
        };
    }
    window.picUpload = {};
    picUpload.picDics = {};
    picUpload.totalLength = 0;
    picUpload.isExist = function (catID, fileName) {
        var isExist = false;
        if (!!fileName) {            
            if(catID === 0){
                for (var catID in window.picUpload.picDics) {
                    if (window.picUpload.picDics.hasOwnProperty(catID)) {
                        var files = window.picUpload.picDics[catID];
                        if (!!files) {
                            for (var file in files) {
                                if (file.name === fileName) {
                                    isExist = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (isExist) {
                        break;
                    }
                }
            }
            else {
                var files = window.picUpload.picDics[catID];
                if(!!files){
                   for(var file in files){
                       if(file.name === fileName){
                           isExist = true;
                           break;
                       }
                   }
                }
            }
        }
        return isExist;
    };
    picUpload.addPic = function () {

    };
    picUpload.delPic = function (catID, fileName) {
        if (!!fileName) {
            if (catID === 0) {
                for (var catID in window.picUpload.picDics) {
                    if (window.picUpload.picDics.hasOwnProperty(catID)) {
                        var files = window.picUpload.picDics[catID];
                        if (!!files) {
                            var length = files.length;
                            for (var i = length - 1; i >= 0;i--) {
                                if (files[i].name === fileName) {
                                    files.splice(i, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else {
                var files = window.picUpload.picDics[catID];
                if (!!files) {
                    var length = files.length;
                    for (var i = length - 1; i >= 0; i--) {
                        if (files[i].name === fileName) {
                            files.splice(i, 1);
                            break;
                        }
                    }
                }
            }
        }
    };
    picUpload.uploadPic = function () {

    };
});

//判断是否IE浏览器
function judgeIE() {
    var sUserAgent = navigator.userAgent;
    var isOpera = sUserAgent.indexOf("Opera") > -1;
    var isIE = sUserAgent.indexOf("compatible") > -1 && sUserAgent.indexOf("MSIE") > -1 && !isOpera;
    return isIE;
}

$("#upload").click(function () {
    var commentID = $("#commentID").val();
    var imgLength = $("#fileList").find("img").length;

    var picMaxCount = parseInt($("#commentMaxPicCount").val(), 10);

    if (imgLength === picMaxCount) {
        alert("最多上传" + imgLength + "张照片");
        return false;
    }
    return $("#file").click();
});

function changeFile() {
    var files = document.getElementById('file').files;
    if (files == null || files.length == 0) {
        return;
    }

    var imgLength = $("#fileList").find("img").length;
    var picMaxCount = parseInt($("#commentMaxPicCount").val(), 10);

    if (imgLength + files.length > picMaxCount) {
        alert("最多上传" + picMaxCount + "张照片");
        return false;
    }

    var config = {
        bucket: 'whphoto',
        expiration: parseInt((new Date().getTime() + 3600000) / 1000),
        // 尽量不要使用直接传表单 API 的方式，以防泄露造成安全隐患
        form_api_secret: 'Mbu7g+t64a0dWPfPpkzEUEiKJHc='
    };

    var instance = new Sand(config);
    var contentSecret = getcontentSecret(8) || 'whhotels';
    var options = {
        'notify_url': 'http://upyun.com',
        'content-secret': contentSecret
    };

    instance.setOptions(options);
    var commentID = parseInt($("#commentID").val(), 10);

    var msgObject = {};
    for (var i = 0; i < files.length;i++){
        var file = files[i];
        var msg = checkFile(file);
        if (msg) {
            msgObject[file.name] = msg;
        }
    }
    var msgArray = [];
    //空对象可以转换成bool值
    if (msgObject) {
        for (var i in msgObject) {
            if (msgObject.hasOwnProperty(i)) {
                msgArray.push("照片" + i + msgObject[i]);
            }
        }
    }
    if (msgArray.length > 0) {
        var msg = msgArray.join('\n');
        alert(msg);
        return false;
    }
    else {
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var fileName = file.name;
            var ext = '.' + fileName.split('.').pop();
            var newPicName = getPicPath(commentID,i,0);
            var PhotoSURL = newPicName + ext;
            var picPath = '/' + PhotoSURL;
            instance.upload(picPath, file);
            handleFiles(file,picPath);//处理图片
        }
    }
}

$("#file").change(changeFile);

function getcontentSecret(num) {
    var dic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var contentSecret = [];
    for (var i = 1; i <= num; i++) {
        contentSecret.push(dic.charAt(Math.floor(Math.random()*61)));
    }
    return contentSecret.join('');
}

//$("#file").change(handleFiles);
function checkFile(file) {
    var msg = "";
    if (file.size > 5242880) {
        msg += " 超过大小限制";
    }
    var ext = '.' + file.name.split('.').pop();
    var extName = ext.toLowerCase();
    if (extName !== '.jpeg' && extName !== '.jpg' && extName != '.png') {
        msg += " 格式不正确";
    }
    return msg;
}

function checkFileValidation(file,catID) {
    var msg = "";
    var isExist = picUpload.isExist(catID, file.name);
    if (isExist) {
        msg += " 图片已存在";
    }
    else{
        if (file.size > 3145728) {
            msg += " 大小超过了3M限制";
        }
        var ext = '.' + file.name.split('.').pop();
        var extName = ext.toLowerCase();
        if (extName !== '.jpeg' && extName !== '.jpg' && extName != '.png') {
            msg += " 格式不正确";
        }
    }
    return msg;
}

function handleFiles(file, picPath) {
    var fileList = document.getElementById("fileList");    

    var $fileList = $("#fileList");
    $fileList.find("span").remove();

    var length = parseInt($("#fileNum").val(), 10) + 1;
    $("#fileNum").val(length);

    var picMaxCount = parseInt($("#commentMaxPicCount").val(), 10);

    var value = length + "/" + picMaxCount;

    var img = new Image();
    img.className = 'dpphoto2';
    img.name = file.name;

    if (window.URL) {
        //File API
        //alert(files[0].name + "," + files[0].size + " bytes");
        img.src = window.URL.createObjectURL(file); //创建一个object URL，并不是你的本地路径
        img.onload = function (e) {
            window.URL.revokeObjectURL(this.src); //图片加载后，释放object URL
        }
        //var cellDiv = document.createElement("div");
        //cellDiv.className = picPath;
        //cellDiv.style = "position:relative";
        //fileList.appendChild(cellDiv);
        //cellDiv.appendChild(img);

        //$(img).hover(showDelX, hideDelX);

        //var $sapn = $('<span onclick="DelPic();" title="删除图片" class="divX">X</span>');
        //$sapn.css({ left: $(img).position().left + img.width - $sapn.width * 2, top: $(img).position().top });
        //$(cellDiv).append($sapn);
        fileList.appendChild(img);
        $fileList.append('<span style="color:black;font-size:small">' + value + '</span>');
    }
    else if (window.FileReader) {
        //opera不支持createObjectURL/revokeObjectURL方法。我们用FileReader对象来处理
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            //alert(files[0].name + "," + e.total + " bytes");
            img.src = this.result;

            //var cellDiv = document.createElement("div");
            //cellDiv.className = picPath;
            //cellDiv.style = "position:relative";
            //fileList.appendChild(cellDiv);
            //cellDiv.appendChild(img);

            //$(img).hover(showDelX, hideDelX);

            //var $sapn = $('<span onclick="DelPic();" title="删除图片" class="divX">X</span>');
            //$sapn.css({ left: $(img).position().left + img.width - $sapn.width * 2, top: $(img).position().top });
            //$(cellDiv).append($sapn);
            fileList.appendChild(img);
            $fileList.append('<span style="color:black;font-size:small">' + value + '</span>');
        }
    }
    //else {
    //    //ie
    //    obj.select();
    //    obj.blur();
    //    var nfile = document.selection.createRange().text;//IE怎么对付多个文件上传？？？ToDo
    //    document.selection.empty();
    //    img.src = nfile;
    //    img.onload = function () {
    //        //alert(nfile + "," + img.fileSize + " bytes");
    //    }
    //    fileList.style.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod='scale',src='" + nfile + "')";
    //}
    //ToDo 验证重复
    //existFileNames = new Array();
    //if (existNames.length == 0 || existNames.indexOf(img.name) > -1) {
    //    existNames.push(img.name);
    //}
    //else {
    //    alert("该文件已上传");
    //    return false;
    //}
}

//显示X
function showDelX() {
    $(this).siblings(".divX").show();
}

//隐藏X
function hideDelX() {
    $(this).siblings(".divX").hide();
}

//删除图片
function DelPic() {
    var $divSpan = $(this).parents("div").eq(0);
    var hiddenFd = $.trim($divSpan.attr("class"));
    $("#" + hiddenFd).remove();//移除图档位置
    $divSpan.remove();//把所在div移除

    var $fileDiv = $("#fileList");
    var $valueSpan = $fileDiv.find("span:last-child");
    var imgNum = parseInt($fileDiv.find("img").length);

    var picMaxCount = parseInt($("#commentMaxPicCount").val(), 10);

    if (imgNum > 0) {
        $valueSpan.text(imgNum + "/" + picMaxCount);
    }
    else {
        $valueSpan.remove();//移除图片的数量
    }
}

function getPicPath(CommentID, picFlowNum, catID) {
    var datetime = new Date();
    var date = datetime.getDate();
    var hour = datetime.getHours();
    var minute = datetime.getMinutes();
    var second = datetime.getSeconds();
    //var imgLength = $("#fileList").find("img").length;
    //var picFlowNum = imgLength;
    return CommentID + getTimeCount2Char(date) + getTimeCount2Char(hour) + getTimeCount2Char(minute) + getTimeCount2Char(second) + picFlowNum + catID;
}

function getTimeCount2Char(time)
{
    if(time >= 60)
    {
        return time;
    }
    var dic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    return dic.charAt(time);
}

function addImage(obj) {
    var $form = $(obj).parents("form");
    var $fileList = $form.siblings(".fileList");
    var length = $fileList.find("img").length;

    var picMaxCount = parseInt($("#commentMaxPicCount").val(), 10);

    if(length === picMaxCount){
        alert("您已选了" + picMaxCount + "张图片，上传新图片之前请先删除已有照片");
        return false;
    }
    var $fileInput = $form.find(".inputFile").eq(0);
    $fileInput.click();
}

$("body").delegate(".inputFile", "change", fileInputChange);

function fileInputChange() {
    var $fileInput = $(this);
    var catID = parseInt($fileInput.attr("title"), 10);
    var $form = $fileInput.parent("form");
    var files = this.files;
    var filesLength = files.length;
    if (filesLength == 0) {
        return false;
    }

    var $fileList = $form.siblings(".fileList");
    var existFileLength = $fileList.find("img").length;

    var picMaxCount = parseInt($("#commentMaxPicCount").val(), 10);

    if (filesLength + existFileLength > picMaxCount) {
        alert("最多上传" + picMaxCount + "张照片");
        clearInputUpload($fileInput);
        return false;
    }

    var msgObject = {};
    
    for (var i = filesLength-1; i >= 0; i--) {
        var file = files[i];
        var msg = checkFileValidation(file,catID);
        if (msg) {
            msgObject[file.name] = msg;
        }
    }

    var msgArray = [];
    //空对象可以转换成bool值
    if (!!msgObject) {
        for (var i in msgObject) {
            if (msgObject.hasOwnProperty(i)) {
                msgArray.push("照片" + i + ":" + msgObject[i]);
            }
        }
    }
    if (msgArray.length > 0) {
        var msg = msgArray.join('\n');
        alert(msg);
        clearInputUpload($fileInput);
        return false;
    }
    else {
        //var fileList = $fileList[0];//jquery object transfer as dom object
        //$fileList.find("span").remove();//移除当前数量标签
        if (!window.picUpload.picDics[catID]) {
            window.picUpload.picDics[catID] = [];
        }
        for (var i = 0; i < filesLength; i++) {
            var file = files[i];
            window.picUpload.picDics[catID].push(file);
            preLoadFiles(file, $fileList,catID);//处理图片
        }
    }
}

function clearInputUpload($fileInput) {
    $fileInput.after($fileInput.clone().val(""));
    $fileInput.remove();
}

function preLoadFiles(file,$fileList,catID) {

    var img = new Image();
    img.className = 'dpphoto2';
    img.name = file.name;
    img.title = file.name + "," + catID;
    if (window.URL) {
        //File API
        img.src = window.URL.createObjectURL(file); //创建一个object URL，并不是你的本地路径
        
        img.onload = function (e) {
            window.URL.revokeObjectURL(this.src); //图片加载后，释放object URL
            $fileList.append(img);
        }
    }
    else if (window.FileReader) {
        //opera不支持createObjectURL/revokeObjectURL方法。我们用FileReader对象来处理
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            img.src = this.result;
            $fileList.append(img);
        }
    }
}

$(".fileList").delegate("img", "click", function (data) {
    var isTrue = confirm("确认删除该图片吗？");
    if(isTrue){
        delImage(this);
    }
});

function delImage(obj) {
    var gaga = $(obj).attr("title").split(',');
    var fileName = gaga[0];
    var catID = parseInt(gaga[1], 10);
    window.picUpload.delPic(catID,fileName);
    $(obj).remove();
}