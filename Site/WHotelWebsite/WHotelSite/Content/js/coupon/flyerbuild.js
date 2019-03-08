$(function () {
    
    showSpinner.prefetch();

    var pubuserid = $("#userid").val();

    //提交绑定常旅客
    $("#sub-link").click(function () {

        showSpinner(true);

        var _acountNo = $("#accountNo").val().AllTrim();
        var _otherName = $("#otherName").val();
        var _preName = $("#preName").val();

        var _subDic = {};
        _subDic["userid"] = pubuserid;
        _subDic["acountNo"] = _acountNo;
        _subDic["otherName"] = _otherName;
        _subDic["preName"] = _preName;

        //test
        //Global.Monito.Publisher("coupon", "wallet", 1);
        //window.history.back();
        //return;

        $.get('/Coupon/AddFlyerInfo', _subDic, function (result) {

            showSpinner(false);
            
            var _msg = result.Message;
            var _suc = result.Success; //suc = "1";

            if (_suc == 0) {
                
                console.log(_msg)

                //常旅客绑定成功后，标识全局变量需要
                //刷新前一页
                Global.Monito.Publisher("coupon", "wallet", 1);
                //刷新钱包页
                Global.Monito.Publisher("coupon", "wallet2", 1);

                window.history.back();
            }
            else {
                alert(_msg);
            }
        });
    });

    //text blur
    var formInputCheck = function () {
        
        var _acountNo = $("#accountNo").val().AllTrim();
        var _otherName = $("#otherName").val();
        var _preName = $("#preName").val();

        if (_acountNo.length == 12 && _otherName.length > 0 && _preName.length > 0) {
            $("#sub-link").show();
            $("#sub-link0").hide();
        }
        else {
            $("#sub-link0").show();
            $("#sub-link").hide();
        }
    }

    var cpLock = false;
    var accountNoInp = $("#accountNo")[0];
    accountNoInp.addEventListener('compositionstart', function () { cpLock = true; })
    accountNoInp.addEventListener('compositionend', function () { cpLock = false; })
    accountNoInp.addEventListener('input', function () {
        if (!cpLock) {
            if (accountNoInp.value.length == 12) {
                accountNoInp.value = accountNoInp.value.replace(/(\w{4})/ig, '$1 ').Trim();
            }

            formInputCheck();
        }
    });
    $("#accountNo").blur(function () {

        formInputCheck();
    });

    var otherNameInp = $("#otherName")[0];
    otherNameInp.addEventListener('compositionstart', function () { cpLock = true; })
    otherNameInp.addEventListener('compositionend', function () { cpLock = false; })
    otherNameInp.addEventListener('input', function () {
        if (!cpLock) {
            otherNameInp.value = otherNameInp.value.replace(/[^a-zA-Z]/ig, '')

            formInputCheck();
        }
    });
    $("#otherName").blur(function () {

        formInputCheck();
    });

    var preNameInp = $("#preName")[0];
    preNameInp.addEventListener('compositionstart', function () { cpLock = true; })
    preNameInp.addEventListener('compositionend', function () { cpLock = false; })
    preNameInp.addEventListener('input', function () {
        if (!cpLock) {
            preNameInp.value = preNameInp.value.replace(/[^a-zA-Z]/ig, '')

            formInputCheck();
        }
    });
    $("#preName").blur(function () {

        formInputCheck();
    });
});