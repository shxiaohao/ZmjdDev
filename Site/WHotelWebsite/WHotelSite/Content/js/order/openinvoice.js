
var phone = $("#phone").val();
var hotelId = $("#hotelId").val();
var pid = $("#pid").val();
var userID = $("#userID").val();
var packageType = $("#packageType").val();
var orderID = $("#orderID").val();

var _Config = new Config();
//function goUrl(url) {
//    gourl(url);
//}

$(function () {
    var vueData = new Vue({
        el: "#vuedata",
        data: {

            AddInvoiceModel: {
                OrderId: orderID,
                ReceivePeopleInformationId: 0,
                invoice: {
                    Address: "",                         //收件地址
                    Contact: "",                         //收件人
                    TelPhone: "",                        //收件人手机号
                    Title: "",                           //发票标题
                    Type: 1,                             //0代订房费 1 服务费 2会务费 3酒店开据4旅游费
                    TypeName: "",
                    ShippingType: "",                    //快递地址
                    TaxNumber: "",                       //纳税号
                    TicketOpeningType: 0,                //开发票类型 0个人1企业
                    InvoiceFormType: 0,                  //发票形式 0电子发票 1纸质发票
                    Email: "",                           //邮箱
                    PayType: 0                           //支付类型 0 不需要支付 1货币支付 2积分支付
                }
            },
            //invoiceInfo: {},
            InvoiceType: {},
            Titels: [],
            Description: "",
            DescriptionURL: "",
            Tips: ""
        },
        methods: {
            selectTickeOpenType: function (param) {
                vueData.AddInvoiceModel.invoice.TicketOpeningType = param;
            },
            openPaper: function () {
                _Modal.show({
                    title: "",
                    content: "纸质发票目前仅支持在周末酒店APP内申请开具，请下载周末酒店APP在订单详情页提交开票信息。",
                    confirmText: "下载APP",
                    confirm: function () {
                        location.href = "http://app.zmjiudian.com";//app下载页面
                    },
                    showCancel: true,
                    cancel: function () {
                        _Modal.hide();
                    },
                    showClose: false
                });
                //alert("需要下载APP，在APP中开纸质发票！");
            },
            selectContent: function () {
                $("._selector-model").show();
                $("._selector").show();
            },
            hideInvoiceContentSelected: function () {
                $("._selector-model").hide();
                $("._selector").hide();
            },
            selectTit: function () {
                $("._selectorTitel-model").show();
            },
            selecedContent: function (id, name) {
                vueData.AddInvoiceModel.invoice.Type = id;
                vueData.AddInvoiceModel.invoice.TypeName = name;
                $("._selector-model").hide();
                $("._selector").hide();
            },
            hidepage: function () {
                $("._selectorTitel-model").hide();
            },
            selectedtit: function (tit) {
                vueData.AddInvoiceModel.invoice.Title = tit;
                $("._selectorTitel-model").hide();
            },
            add_tit: function () {
                vueData.AddInvoiceModel.invoice.Title = $(".invoicetit").val();
                $("._selectorTitel-model").hide();
            },
            submitInvoice: function () {
                if (vueData.AddInvoiceModel.invoice.TicketOpeningType == 1) {
                    if (vueData.AddInvoiceModel.invoice.TaxNumber.length >= 15 && vueData.AddInvoiceModel.invoice.TaxNumber.length <= 20) {

                    }
                    else {
                        alert("请输入正确的税务代码！");
                        return false;
                    }
                }
                else {
                    var myReg = /^[a-zA-Z0-9_-]+@([a-zA-Z0-9]+\.)+(com|cn|net|org)$/;
                    if (!myReg.test(vueData.AddInvoiceModel.invoice.Email)) {
                        alert("请输入正确的邮箱");
                        return false;
                    }
                }

                _Modal.show({
                    title: "提交信息",
                    content: "电子发票在信息提交后10日内，发送至您的指定邮箱，发票信息提交后不可修改",
                    confirmText: "确定提交",
                    confirm: function () {
                        $.ajax({
                            url: _Config.APIUrl + "/api/Order/AddInvoiceInfoNew",
                            data: vueData.AddInvoiceModel,// { HotelId: hotelId, PackageType: packageType, Phone: phone, Pid: pid, UserID: userID },
                            type: "POST",
                            success: function (_data) {
                                if (_data.Success == 0) {
                                    alert("提交成功");
                                }
                                else {
                                    alert(_data.Message);
                                }
                                window.location.href = document.referrer;
                                //window.location.href = window.location.href;
                            }
                        })
                    },
                    showCancel: true,
                    cancel: function () {
                        _Modal.hide();
                    },
                    showClose: false
                });

            }
        }
    })
    //var param = {
    //    "HotelId": 30707,
    //    "PackageType": 1,
    //    "Phone": '',
    //    "Pid": 3802,
    //    "UserID": 4512304
    //}
    var param = {
        "HotelId": hotelId,
        "PackageType": packageType,
        "Phone": phone,
        "Pid": pid,
        "UserID": userID
    }
    $.ajax({
        url: _Config.APIUrl + "/api/accounts/GetInvoiceInfo",
        data: param,// { HotelId: hotelId, PackageType: packageType, Phone: phone, Pid: pid, UserID: userID },
        type: "POST",
        success: function (_data) {
            console.log(_data);
            Vue.nextTick(function () {
                vueData.AddInvoiceModel.invoice.Title = _data.Title.length > 0 ? _data.Title[0] :"请添加发票抬头";
                vueData.AddInvoiceModel.invoice.Type = _data.InvoiceType[0]["TypeId"];
                vueData.AddInvoiceModel.invoice.TypeName = _data.InvoiceType[0]["TypeName"];
                vueData.InvoiceType = _data.InvoiceType
                vueData.Titels = _data.Title;
                vueData.Tips = _data.Tips;

                vueData.Description = _data.Description;
                vueData.DescriptionURL = _data.DescriptionURL;
            })
        }
    })

})
