var UserCookie;
var cid;
var tno;
var carno;
var energytype;
$(function () {
    UserCookie = GetUserInfo();
    var myDate = new Date();
    $('#selActiv').combobox('setValue', '-1');
    $('#selLockStatus').combobox('setValue', '-1');
    $('#selTimeLimit').combobox('setValue', '0');

    var grrsysflag = GetTopQueryString("key");
    if (grrsysflag == "LIUTE") {
        //$('#trlockinfo').hide()
    }

});

function selectCar() {
    $('#loading-car').window('open');
    var qCarNo = $("#txtCarNo").val();
    var qVIN = $("#txtVIN").val();
    var qActive = $('#selActiv').combobox("getValue");
    var qLockStatus = $('#selLockStatus').combobox("getValue");
    var qTimeLimit = $('#selTimeLimit').combobox("getValue");
    var mydata = {
        "sid": "xiaodai-car-query",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "carno": qCarNo,
        "vin": qVIN,
        "active": qActive,
        "lockstatus": qLockStatus,
        "timelimit": qTimeLimit
    };
    BaseGetData(mydata, BindCarData);
}

function BindCarData(obj) {
    $('#loading-car').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            var CarArray = obj.result.records;
            $("#dg").datagrid("loadData", CarArray);
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('车辆信息查询，服务器超时！');
            }
        }
    }
}



function sendXiaodaiLockOrder() {
    var CarArr = $("#dg").datagrid('getSelections');
    if (CarArr.length > 0) {
        if (CarArr[0].os == "停止") {
            $.messager.alert('错误信息', "车辆不在线，无法下发指令！", 'error');
        }
        else {
            cid = CarArr[0].CID;
            tno = CarArr[0].TNO;
            carno = CarArr[0].CarNo;
            //        alert(cid+"-"+tno+"-"+carno);

            var paramvalue = $("input[name='LockType']:checked").val();
            var LDataType = $("input[name='LDataType']:checked").val();
            var rev = "";
            var nj = "";
            var orderdesc = "";
            var confirmstr = "";
            var lockreason = "";
            var unlockreason = "";
            switch (paramvalue) {
                //        case "0x00":       
                //            orderdesc = "取消所有紧急限制";       
                //            confirmstr = "是否确认对车辆 " + carno + " 下发 " + orderdesc + " 指令？";       
                //            break;  
                case "0xA8":

                    rev = $("#lockRev").val(); //锁车转速
                    nj = $("#lockNj").val(); //锁车扭矩
                    orderdesc = "立即锁车";
                    confirmstr = "是否确认对车辆 " + carno + " 下发 " + orderdesc + " 指令？\r";
                    if (LDataType == 1 && rev != "") {
                        confirmstr += "锁车转速:" + rev + "rpm";
                    }
                    if (LDataType == 2 && nj != "") {
                        confirmstr += "锁车扭矩:" + nj + "%";
                    }
                    lockreason = $("#lockDesc").val(); //锁车原因
                    break;
                case "0x8A":
                    rev = 3500; //解锁转速
                    orderdesc = "立即解锁";
                    confirmstr = "是否确认对车辆 " + carno + " 下发 " + orderdesc + " 指令？\r" + " 解锁转速:" + rev + "rpm";
                    unlockreason = $("#unlockDesc").val();  //解锁原因
                    break;
                //        case "0xAA":       
                //            orderdesc = "已还清贷款";       
                //            confirmstr = "是否确认对车辆 " + carno + " 下发 " + orderdesc + " 指令？";       
                //            break;       
            }

            var mydata = {
                "sid": "order-send-xiaodailockdown",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "cid": cid,
                "tno": tno,
                "carno": carno,
                "paramvalue": paramvalue,
                "rev": rev,
                "nj": nj,
                "LDataType": LDataType,
                "lockreason": lockreason,
                "unlockreason": unlockreason
            };


            if (confirm(confirmstr)) {
                BaseGetData(mydata, SendXiaodaiParamRes);
            }
        }
    }
    else {
        $.messager.alert('错误信息', "请选择车辆！", 'error');
    }
}

function sendXiaodaiVINParamOrder() {
    var CarArr = $("#dg").datagrid('getSelections');
    if (CarArr.length > 0) {
        if (CarArr[0].os == "停止") {
            $.messager.alert('错误信息', "车辆不在线，无法下发指令！", 'error');
        }
        else {
            cid = CarArr[0].CID;
            tno = CarArr[0].TNO;
            carno = CarArr[0].CarNo;
            energytype = CarArr[0].ProtocolETPKey;  //发动机类型
            //        alert(cid + "-" + tno + "-" + carno);
            var vin = 0;
            var vintype = $("input[name='VinType']:checked").val();
            var rev = "0";
            var orderdesc = "";
            var confirmstr = "";
            if (vintype == "0xAA") {
                //rev = $("#vinlockRev").val(); //绑定转速
                orderdesc = "销贷功能激活";
                confirmstr = "是否确认对车辆 " + carno + " 下发 " + orderdesc + " 指令？";
            }
            else if (vintype == "0x55") {
                //rev = $("#vinunlockRev").val(); //解绑转速
                orderdesc = "销贷功能取消";
                confirmstr = "是否确认对车辆 " + carno + " 下发 " + orderdesc + " 指令？";
            }

            //    if (vin == "") {
            //        $.messager.alert('错误信息', "请输入VIN号码！", 'error');
            //     }
            //    else if (vin.length != 17) {
            //        $.messager.alert('错误信息', "请输入17位VIN号码！", 'error');
            //     }
            //    else {
            var mydata = {
                "sid": "order-send-xiaodaivindown",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "cid": cid,
                "tno": tno,
                "carno": carno,
                "vin": vin,
                "vintype": vintype,
                "energytype": energytype
            };
            if (confirm(confirmstr)) {
                BaseGetData(mydata, SendXiaodaiParamRes);
            }
        }
    }
    else {
        $.messager.alert('错误信息', "请选择车辆！", 'error');
    }
}

function sendXiaodaiParamOrder() {
    var CarArr = $("#dg").datagrid('getSelections');
    if (CarArr.length > 0) {
        if (CarArr[0].os == "停止") {
            $.messager.alert('错误信息', "车辆不在线，无法下发指令！", 'error');
        }
        else {
            cid = CarArr[0].CID;
            tno = CarArr[0].TNO;
            carno = CarArr[0].CarNo;

            var confirmstr = "";

            var datevalue = $("#lTime").datebox('getValue');
            var dayvalue = $("#txtDays").val();
            var minutevalue = $("#txtMinutes").val();
            minutevalue = parseInt(minutevalue) * 1440;
            var mydata = {
                "sid": "order-send-xiaodaiparamdown",
                "sysuid": UserCookie["UID"],
                "sysflag": UserCookie["sysflag"].toString(),
                "token": UserCookie["token"].toString(),
                "cid": cid,
                "tno": tno,
                "carno": carno,
                "datevalue": datevalue,
                "dayvalue": dayvalue,
                "minutevalue": minutevalue
            };

            confirmstr = "是否确认对车辆 " + carno + " 下发以下参数设置指令？\r";
            if (datevalue != "") {
                confirmstr += " 设置还款日期：" + datevalue + "\r";
            }

            if (dayvalue != "") {
                confirmstr += " 还款到期日提醒：" + dayvalue + "\r";
            }

            if (minutevalue != "") {
                confirmstr += " 无通信连接最长连续时间：" + minutevalue + "\r";
            }
            if (confirm(confirmstr)) {
                BaseGetData(mydata, SendXiaodaiParamRes);
            }
        }
    }
    else {
        $.messager.alert('错误信息', "请选择车辆！", 'error');
    }
}


function SendXiaodaiParamRes(res) {
    if (res.state == 100) {
        var resstr = "";
        var data = res.result;
        if (data != null) {
            SetOrderCount();
            if (data.Res == true) {
                resstr += "车辆 " + data.CarNo + " " + data.Desc + " 已下发!" + "\n";
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

