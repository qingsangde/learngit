var UserCookie;
var CarArr = new Array();
var cid;
var tno;
var carno;


$(function () {
    UserCookie = GetUserInfo();
});

function selectCar() {
    CarSelect(true, CallBackFun);
}

function CallBackFun(cardata) {
    CarArr = cardata;
    $("#dg").datagrid("loadData", cardata);
}

function setDriverRecordCMD() {
    if (CarArr == null || CarArr.length <= 0) {
        $.messager.alert('错误信息', '请先选择车辆！', 'error');
        return;
    }
    else {
        cid = CarArr[0].cid;
        tno = CarArr[0].tno;
        carno = CarArr[0].carno;
        var platenum="";
        var vin = "";
        var platetype = ""
        var quotient = "";
        var settype = $("input[name='SetType']:checked").val();

        switch (settype) {
            case "0x82":
                platenum = $('#txtCarNo').val();
                platetype = $('#txtCarType').val();
                vin = $('#txtVIN').val();

                break;

            case "0xC3":
                quotient = $('#txtCarCharacterQuotient').val();
                break;
        }

        var mydata = {
            "sid": "order-send-recordcmddown",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cid": cid,
            "tno": tno,
            "carno": carno,
            "cmd": settype,
            "platenum": platenum,
            "platetype": platetype,
            "vin": vin,
            "quotient": quotient
        };
        BaseGetData(mydata, SendRecordDownRes);
    }
}

function SendRecordDownRes(res) {
    if (res.state == 100) {
        var resstr = "";
        var data = res.result;
        if (data != null) {
            if (data.Res == true) {
                resstr += "车辆 " + data.CarNo + " " + data.Desc + " 成功!" + "\n";
            }
            else {
                resstr += "车辆 " + data.CarNo + " " + data.Desc + " 失败!" + "\n";
            }


            $.messager.alert('系统提示', resstr, 'info');
        }
        else {
            $.messager.alert('错误信息', "未知错误！", 'error');
        }
    }
    else {
        if (res.state == 104) {
            LoginTimeout('服务器超时！');
        }
        else {
            $.messager.alert('错误信息', res.msg, 'error');
        }
    }
}