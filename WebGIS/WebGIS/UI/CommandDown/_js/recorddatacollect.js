var UserCookie;
var CarArr = new Array();
var cid;
var tno;
var carno;
var collecttype;
$(function () {
    UserCookie = GetUserInfo();
    var myDate = new Date();
    var st = myDate.format("yyyy-MM-dd hh:mm:ss");
    var et = myDate.format("yyyy-MM-dd hh:mm:ss");
    $("#sTime").datetimebox('setValue', st);
    $("#eTime").datetimebox('setValue', et);
    TypeChange();
});

function selectCar() {
    CarSelect(true, CallBackFun);
}

function CallBackFun(cardata) {
    CarArr = cardata;
    $("#dg").datagrid("loadData", cardata);
}

function TypeChange() {
    collecttype = $("input[name='CollectType']:checked").val();
    //alert(collecttype);
    if (collecttype == "0x00" || collecttype == "0x01" || collecttype == "0x02" || collecttype == "0x03" || collecttype == "0x04" || collecttype == "0x05" || collecttype == "0x06" || collecttype == "0x07") {
        $("#ordertimediv").hide();
    }
    else {
        $("#ordertimediv").show();
    }
}

function sendRecordCollectionOrder() {
    var starttime; //开始时间
    var endtime; //结束时间
    var ms;
    if (CarArr == null || CarArr.length <= 0) {
        $.messager.alert('错误信息', '请先选择车辆！', 'error');
        return;
    }
    else {
        switch (collecttype) {
            case "0x00":
            case "0x01":
            case "0x02":
            case "0x03":
            case "0x04":
            case "0x05":
            case "0x06":
            case "0x07":
                starttime = 0;
                endtime = 0;
                break;
            case "0x08": //采集指定的行驶速度记录,有时间参数，跨度不超过1小时
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 1 * 60 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集行驶速度记录时间范围不能超过1小时！', 'error');
                    return;
                }
                break;
            case "0x09": //采集指定的位置信息记录,有时间参数，跨度不超过1小时
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 1 * 60 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集位置信息记录时间范围不能超过1小时！', 'error');
                    return;
                }
                break;
            case "0x10": //采集指定的事故疑点记录,有时间参数，跨度不超过1分钟
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 1 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集事故疑点记录时间范围不能超过1分钟！', 'error');
                    return;
                }
                break;
            case "0x11": //采集指定的超时驾驶记录,有时间参数，跨度不超过7天
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 7 * 24 * 60 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集超时驾驶记录时间范围不能超过7天！', 'error');
                    return;
                }
                break;
            case "0x12": //采集指定的驾驶人身份记录,有时间参数，跨度不超过7天
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 7 * 24 * 60 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集驾驶人身份记录时间范围不能超过7天！', 'error');
                    return;
                }
                break;
            case "0x13": //采集指定的外部供电记录,有时间参数，跨度不超过7天
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 7 * 24 * 60 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集外部供电记录时间范围不能超过7天！', 'error');
                    return;
                }
                break;
            case "0x14": //采集指定的参数修改记录,有时间参数，跨度不超过7天
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 7 * 24 * 60 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集参数修改记录时间范围不能超过7天！', 'error');
                    return;
                }
                break;
            case "0x15": //采集指定的速度状态日志,有时间参数，跨度不超过1分钟
                starttime = $("#sTime").datetimebox('getValue'); //开始时间
                endtime = $("#eTime").datetimebox('getValue'); //结束时间
                ms = 1 * 60 * 1000;
                if (!dateCompare(starttime, endtime, ms)) {
                    $.messager.alert('错误信息', '采集速度状态日志时间范围不能超过1分钟！', 'error');
                    return;
                }
                break;
            default:
                break;
            // n 与 case 1 和 case 2 不同时执行的代码    
        }
        //alert("验证通过，执行下发指令操作！");

        cid = CarArr[0].cid;
        tno = CarArr[0].tno;
        carno = CarArr[0].carno;

        var mydata = {
            "sid": "order-send-recordcollection",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cid": cid,
            "tno": tno,
            "carno": carno,
            "cmd": collecttype,
            "start": starttime,
            "end": endtime
        };
        BaseGetData(mydata, SendRecordCollectionRes);

    }
}

function SendRecordCollectionRes(res) {
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

////JS日期比较判断时间范围是否在规定范围内
//function dateCompare(start, end, ms) {//ms:是毫秒数，检查两个时间的差值，是否在指定的毫秒范围内
//    var startD = new Date(Date.parse(start.replace(/-/g, "/")));
//    var endD = new Date(Date.parse(end.replace(/-/g, "/")));
//    var ms0 = parseInt(endD.getTime() - startD.getTime());
//    if (ms0 > ms) {
//        return false;
//    } else {
//        return true;
//    }
//}

