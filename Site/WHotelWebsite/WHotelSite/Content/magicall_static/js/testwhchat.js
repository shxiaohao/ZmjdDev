var messageType_UserWord = 0;
var messageType_UserAction = 1;
var messageType_KeFuWord = 2;
var messageType_KeFuAction = 3;

var messageType_UserAction_ClickChoice = 0;

setTimeout("MagiCallClientHeart()", 5000)//心跳

function MagiCallClientHeart()
{
    $.ajax({
        type: "GET",
        url: "/api/MagiCallAPI/MagiCallClientHeart",
        data: { userid: userID },
        success: function (data) {
        },
        error: function (XMLHttpRequest, textStatus) {
        },
        cache: false
    });
}

function MagiCallClientMessage( messageType,msg)
{
     $.ajax({
        type: "POST",
        url: "/api/MagiCallAPI/MagiCallClientMessage",
        data: { UserID: userID, messageType: messageType, msg: msg },
        success: function (data) {
        },
        error: function (XMLHttpRequest, textStatus) {
        },
        cache: false
    });
}


function startWith(str, startStr) {
    return str.indexOf(startStr) == 0
}

function dealWHAPPMsg(message, msg, im) {
    var returnVal = 0;
    if (startWith(message.value, "WHAPP:")) {
        //  alert("Hello!");

        im.sendTextMsg(message.value)
        returnVal = 1;
    }
    else {
        MagiCallClientMessage(messageType_KeFuWord, message.value);
    }
    //alert(message.value)
    return returnVal;
}



function sentWelcomeMsg(im) {
    var welcome = "WHAPP:html,欢迎使用MagiCall！<br/><br/>我是您的私人酒店顾问小祥。 <br/><br/>请留言您所需酒店的任意要求，我们将为您一站式解决。为您省时、省力、省钱。<br/><br/> 举例：上海周边开车2小时之内的亲子酒店，1000元以内，有儿童泳池，12/12日入住1晚，需要1间。";
    im.sendTextMsg(welcome);
}

function afterUserSendMsg(im, content)
{   
    if (isTodayFirstMessage == 0) {
        sentWaitingMsg(im);
    }
    MagiCallClientMessage(messageType_UserWord, content);
}

var isTodayFirstMessage = 0;
function sentWaitingMsg(im) {
       isTodayFirstMessage = 1;
        //var waitingMsg = "WHAPP:html,消息收到，请稍等";
        //im.sendTextMsg(waitingMsg);
}

//function sentOverWorkTimeMsg(im) {
//    var curHour = new Date().getHours;
//    if (curHour < 9) {
//        var OverWorkTimeMsg = "WHAPP:html,欢迎使用MagiCall！现在是休息时间，我们的工作时间是每天早上9：00-晚上12：00，节假日同。</br></br>请留言您所需酒店的任意要求，我们将为尽力您一站式解决，帮助您省时、省力、省钱。 </br></br>举例：上海周边开车2小时之内的亲子酒店，1000元以内，有儿童泳池，12/12日入住1晚，需要1间。";
//        im.sendTextMsg(OverWorkTime);
//    }
//}



function ShowWHAppMsg(id, strContent) {
    return [
                  "<div id='" + id + "' class='easemobWidget-left'>",
                    "<div class='WebimKeFuHeadPhoto'><img src='" + kefuHeadPhoto + "' alt='KeFuHeadPhoto' /></div>",
                    "<div class='easemobWidget-msg-wrapper'>",
                          "<i class='easemobWidget-corner'></i>",
                          "<div class='easemobWidget-msg-container'>",
                              ParseWHAppMsg(strContent),
                           "</div>",
                       "</div>",
                   "</div>"
    ].join('');

}

function ParseWHAppMsg(strContent) {
    var html = "";
    var cList = strContent.split(":");



    for (var i = 1; i < cList.length; i++) {
        var iList = cList[i].split(",");
        var itemType = iList[0];

        switch (itemType) {
            case "hotel":
                var hotelid = iList[1];
                var hotelname = iList[2];
                html = html + "<a href=\"whotelapp://www.zmjiudian.com/hotel/" + hotelid + "\" > " + hotelname + "</a>";
                break;
            case "html":
                html = html + "<p>" + strToHTML(iList[1]) + "</p>";
                break;
            default:
                html += html + "<p>" + iList.join(",") + "</p>"
                break;

        }
        return html;
    }
}


function strToHTML(str) {
    return str.replace("&gt;", ">").replace("&lt;", "<");
}
