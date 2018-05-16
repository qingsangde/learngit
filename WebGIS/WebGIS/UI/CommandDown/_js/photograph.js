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
    if (CarArr != null && CarArr.length > 0) {
        $("#txtCarNo").attr('value', CarArr[0].carno);
    }
    //$("#dg").datagrid("loadData", cardata);
}

function sendImageDownOrder() {
    if (CarArr != null && CarArr.length > 0) {
        cid = CarArr[0].cid;
        tno = CarArr[0].tno;
        carno = CarArr[0].carno;

        var mydata = {
            "sid": "order-send-imagedown",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cid": cid,
            "tno": tno,
            "carno": carno,
            "ch": $('#ch').combobox('getValue')
        };
        BaseGetData(mydata, SendImageDownRes);
    }
    else {
        $.messager.alert('错误信息', '请先选择车辆！', 'error');
    }

}

function SendImageDownRes(res) {
    if (res.state == 100) {
        var resstr = "";
        var data = res.result;
        if (data != null) {
            SetOrderCount();
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