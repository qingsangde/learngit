var UserCookie;
var CarArr = new Array();
var cids = new Array();
var tnos = new Array();
var carnos = new Array();
$(function () {
    UserCookie = GetUserInfo();
});

function selectCar() {
    CarSelect(false, CallBackFun);
}

function CallBackFun(cardata) {
    CarArr = cardata;
    $("#dg").datagrid("loadData", cardata);
}

function sendMaxSpeedOrder() {
    cids = [];
    tnos = [];
    carnos = [];
    if (CarArr.length > 0) {
        $.each(CarArr, function (i, obj) {
            cids.push(obj.cid);
            tnos.push(obj.tno);
            carnos.push(obj.carno);
        });
        var mydata = {
            "sid": "order-send-maxspeed",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cids": cids.join(","),
            "tnos": tnos.join(","),
            "carnos":carnos.join(","),
            "maxspeed": $('#txtmaxspeed').val()
        };
        BaseGetData(mydata, SendMaxSpeedRes);
    }
    else {
        $.messager.alert('错误信息', '请先选择车辆！', 'error');
    }
    
}

function SendMaxSpeedRes(res) {
    if (res.state == 100) {
        var resstr = "";
        var dataarr = res.result;
        if (dataarr != null) {
            if (dataarr.length > 0) {
                var len = dataarr.length;
                for (var i = 0; i < len; i++) {
                    //SetOrderCount();
                    if (dataarr[i].Res == true) {
                        resstr += "车辆 " + dataarr[i].CarNo + " " + dataarr[i].Desc + " 成功!" + "\n";
                    }
                    else {
                        resstr += "车辆 " + dataarr[i].CarNo + " " + dataarr[i].Desc + " 失败!" + "\n";
                    }
                }
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