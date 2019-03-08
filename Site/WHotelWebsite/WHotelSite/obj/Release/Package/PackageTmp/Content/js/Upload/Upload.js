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
    if (imgLength === 8) {
        alert("最多上传" + imgLength + "张照片");
        return false;
    }
    return $("#file").click();
    //if (commentID) {
    //    var imgLength = $("#fileList").find("img").length;
    //    if (imgLength === 8) {
    //        alert("最多上传" + imgLength + "张照片");
    //        return false;
    //    }
    //    return $("#file").click();
    //}
    //else
    //{
    //    alert("提交图片时，请先提交点评");
    //    return false;
    //    //return $("#file").click();
    //}
});

function changeFile() {
    var files = document.getElementById('file').files;
    if (files == null || files.length == 0) {
        return;
    }

    var imgLength = $("#fileList").find("img").length;
    if (imgLength + files.length > 8) {
        alert("最多上传8张照片");
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
            var newPicName = getPicPath(commentID,i);
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

function handleFiles(file, picPath) {
    var fileList = document.getElementById("fileList");    

    var $fileList = $("#fileList");
    $fileList.find("span").remove();

    var length = parseInt($("#fileNum").val(), 10) + 1;
    $("#fileNum").val(length);
    var value = length + "/" + 8;

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
    if (imgNum > 0) {
        $valueSpan.text(imgNum + "/" + 8);
    }
    else {
        $valueSpan.remove();//移除图片的数量
    }
}

function getPicPath(CommentID, picFlowNum) {
    var datetime = new Date();
    var date = datetime.getDate();
    var hour = datetime.getHours();
    var minute = datetime.getMinutes();
    var second = datetime.getSeconds();
    //var imgLength = $("#fileList").find("img").length;
    //var picFlowNum = imgLength;
    return CommentID + getTimeCount2Char(date) + getTimeCount2Char(hour) + getTimeCount2Char(minute) + getTimeCount2Char(second) + picFlowNum;
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

//function showImage(path, name, nwidth) {
//    $('#file').before('<a href="javascript:;"><img src="' + path + '" class="dpphoto2" title="' + name + '" alt="" onload="resizePic(' + nwidth + ');" /></a>');
//}

//function resizePic(nwidth) {
//    if (!nwidth) {
//        nwidth = 371;
//    }
//    var width = $(this).width();
//    var rate = parseInt(nwidth * 100 / width, 10);
//    var height = $(this).height();
//    $(this).css('width', parseInt(width * rate / 100),10);
//    $(this).css('height', parseInt(height * rate / 100),10);
//}

// demo stuff
function addLog(data) {
    var elem = document.createElement("ul");
    for (var key in data) {
        if (key === 'path') {
            elem.innerHTML += '<li><strong>' + key + ':</strong>' + '<a target="_blank"  href="http://demonstration.b0.upaiyun.com' + data[key] + '">' + data[key] + '</a>' + '</li>';
        } else {
            elem.innerHTML += '<li><strong>' + key + ':</strong>' + data[key] + '</li>';
        }
    }
    var log = document.getElementById("log");
    log.appendChild(elem);
}

//document.addEventListener('uploaded', function (e) {
//    addLog(e.detail);
//});