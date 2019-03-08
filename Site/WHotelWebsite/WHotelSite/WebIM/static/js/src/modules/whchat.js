var messageType_UserWord = 0;
var messageType_UserAction = 1;
var messageType_KeFuWord = 2;
var messageType_KeFuAction = 3;

var messageType_UserAction_ClickChoice = 0;
var messageType_UserAction_ClickHotel = 10;
var messageType_UserAction_RequirePrice = 20;
var messageType_UserAction_RequirePrice_ClickChoice = 21;
var messageType_UserAction_Other = 100;
 


window.setInterval("MagiCallClientHeart()", 15000);//心跳
function MagiCallClientHeart()
{
    easemobim.emajax({
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
    easemobim.emajax({
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
        im.sendTextMsg(message.value)
        returnVal = 1;
    }

    return returnVal;
}

 
function sentWelcomeMsg(me) {

    //if (welcomeMsg && welcomeMsg.length > 0) {
    //    var message = strTansNewLine(strToHTML(welcomeMsg)); 
    //    var msg = {
    //        data: message,
    //        type: 'txt',
    //        noprompt: true
    //    };
    //    me.receiveMsg(msg, 'txt');
    //}
}

function afterUserSendMsg(im, content)
{   
    if (isTodayFirstMessage == 0) {
        sentWaitingMsg(im);
    }
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

    strContent = strToHTML(strContent);
    if (strContent.length > 6) {
    //   alert(strContent);
        var tag = strContent.substring(0, 6);
        if (tag == "WHAPP:") {
            var json = strContent.substring(6);

            if (json.substring(0, 1) == "{") {
                var obj = JSON.parse(json);
                var feedbackType = obj.feedbackType;

                switch (feedbackType) {
                    case 3://"ChoiceList":
                        return GenChoiceHtml(obj, messageType_UserAction_ClickChoice);
                        break;
                  
                    case 10://"RequireHotelPrice":
                        return GenRequireHotelPriceHtml(obj);
                        break;
                    case 11://"RequireHotelPrice ChoiceList": 
                        return GenChoiceHtml(obj, messageType_UserAction_RequirePrice_ClickChoice);
                            break;
                }  
            }
            else {
                return ParseWHAppMsgPlat(strContent);
            }

        }
    }
}


function GenRequireHotelPriceHtml(obj) {
    var html = "";
    html += "<div class='require-hotel-price' >"
    if (obj.title) {
        html = html + "<div  class='title'>" + obj.title + "</div>";
    }
    var hotel = obj.packageInfo;
    html += "<div class=\"row\"   >";
    html += "   <div class=\"left\">";
    html += "       <div class=\"pn\">" + hotel.hotelName + "</div>";
    html += "       <div class=\"pn\">" + hotel.PCode + "</div>";
    html += "       <div class=\"pn\">" + FormatDate(hotel.checkIn ) + "入住 &nbsp;" + FormatDate(hotel.checkOut) + "离店</div>";
   // html += "       <div class=\"pn\">普通价：" + hotel.price  + "</div>";
    //html += "       <div class=\"pn\">会员价：" + hotel.VIPPrice + "</div>";
    html += "   </div>";
    html += "</div>";
    html += "</div>"
    return html;
}


function FormatDate(date)
{
    return new Date(Date.parse(date)).format("MM-dd");
}

function GenChoiceHtml(obj, choiceType) {
    var html = "";
    html += "<div class='choice-list' >"
    if (obj.title) {
        html = html + "<div  class='title'>" + obj.title + "</div>";
    } 
    for (var idx in obj.items) {
        var item = obj.items[idx];

      
        html += "<div class=\"row\"  onclick='UserClickChoice(" + item.idx +"," + choiceType +")'>";
        html += "   <div class=\"left\">";
        html += "       <div class=\"pn\">" +  item.content +"</div>";
        html += "   </div>";
        html += "   <div class=\"right r-c1\" ></div> ";
        html += "</div>";         
    } 
    html += "</div>"
    return html;
}


function UserClickChoice(idx, choiceType)
{    
    var msg = "{\"ActionType\":" + choiceType +
               ",\"ID\":" +idx +
               "}";
    msg = msg.replace(/\"/g,"\\\"");
    MagiCallClientMessage(messageType_UserAction, msg)
}

function ParseWHAppMsgPlat(strContent) {
    var html = "";

    var content = strContent.substring(6);

         var iList = content.split(",");
        var itemType = iList[0];
          

        switch (itemType) {
            case "hotel":
                var hotelid = iList[1];
                var hotelname = iList[2];
                var protocal = isAPP ? "whotelapp" : "http";
                html = html + "<a href=\"" + protocal +"://www.zmjiudian.com/hotel/" + hotelid + "\" > " + hotelname + "</a>";
                break;
            case "pichotel":
                var o = iList[1],
                a = iList[2];
                b = iList[3];
                d = iList[4];
                e = iList[5];
                f = iList[6];
                g = iList[7];
                h = iList[8];
                html = html + "<p>" + hotletoTable(o, a, b, d, e, f, g, h) + "</p>";
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


function strToHTML(str) {
    return str.replace(/\&gt;/g, ">")
        .replace(/\&lt;/g, "<")
    .replace(/\&quot;/g,"\"");
}
function hotletoTable(a, b, uri, score, price, cur, g, s) {
    var protocal = isAPP ? "whotelapp" : "http";
    var str = "<table>";
    str += "<tr><td width='30%' valign='top'><a class='a1' href=\'" + protocal + "://www.zmjiudian.com/hotel/" + a + "'><img style='display:inline-block' src='" + uri + "' width='60px'  height='60px'/></a></td>"; 
    str += "<td width='70%' style='font-weight:500'><a class='a1' href=\'" + protocal + "://www.zmjiudian.com/hotel/" + a + "'><p>" + b + "</p><p class='linefont'><span class='s1'>" + cur + price + "</span> 起  <span>" + score + "/5分</span></p></a></td></tr>";
    if (g != "" || s != "") {
        g = g.length > 17 ? g.substring(0, 17) + "..." : g;
        s = s.length > 17 ? s.substring(0, 17) + "..." : s;
        str += "<tr><td colspan='2'><hr class='hr1'></td></tr>";
        if (g != "") {
            str += "<tr><td colspan='2'><span class='zmjd-iconfont'>&#xe631;</span>:" + g + "</td></tr>"
        }
        if (s != "") {
            str += "<tr><td colspan='2'><span class='zmjd-iconfont'>&#xe632;</span>:" + s + "</td></tr>"
        }
    }
    return str
}
function strTansNewLine(str) {
    return str.replace("<br/>", "\r\n");
}


Date.prototype.pattern = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份           
        "d+": this.getDate(), //日           
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时           
        "H+": this.getHours(), //小时           
        "m+": this.getMinutes(), //分           
        "s+": this.getSeconds(), //秒           
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度           
        "S": this.getMilliseconds() //毫秒           
    };
    var week = {
        "0": "/u65e5",
        "1": "/u4e00",
        "2": "/u4e8c",
        "3": "/u4e09",
        "4": "/u56db",
        "5": "/u4e94",
        "6": "/u516d"
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    if (/(E+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}