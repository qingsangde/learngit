var selectedcar = new Array();
var monitorcar = new Array();

var time = 20000;
var monitorfalg = false;
var interval = null; //调度器对象。
var carsdata = null;
var selectedCarNO = null;
var UserCookie = null;
var lastSelectRowData;
$(function () {

    MapInit();
    Array.prototype.elremove = function (m) {
        if (isNaN(m) || m > this.length) { return false; }
        this.splice(m, 1);
    }

    $("#tb-run").mousemove(function (e) {
        if (monitorfalg == false) {
            $("#tb-run").css("background-image", "url(_styles/images/monitor-start-hov.png)");
        }
        else {
            $("#tb-run").css("background-image", "url(_styles/images/monitor-stop-hov.png)");
        }
    });
    $("#tb-run").mouseout(function (e) {
        if (monitorfalg == false) {
            $("#tb-run").css("background-image", "url(_styles/images/monitor-start.png)");
        }
        else {
            $("#tb-run").css("background-image", "url(_styles/images/monitor-stop.png)");
        }
    });
    // onRowContextMenu: contextMenu

    $('#data_grid').datagrid({
        onClickRow: function (rowIndex, rowData) {
            var longt = rowData["Long"];
            var lat = rowData["Lati"];

            $("#data_grid").datagrid("clearSelections");
            $("#data_grid").datagrid("checkRow", rowIndex);

            //
            getAddress(longt, lat);

            //
            selectedCarNO = rowData["CarNum"];
        },
        onRowContextMenu: function (e, rowIndex, rowData) { //右键时触发事件
            //三个参数：e里面的内容很多，真心不明白，rowIndex就是当前点击时所在行的索引，rowData当前行的数据
            e.preventDefault(); //阻止浏览器捕获右键事件
            $(this).datagrid("clearSelections"); //取消所有选中项
            $(this).datagrid("selectRow", rowIndex); //根据索引选中该行
            $('#menu').menu('show', {
                //显示右键菜单
                left: e.pageX, //在鼠标点击处显示菜单
                top: e.pageY
            });
            lastSelectRowData = rowData;
        },
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "CarNum") {
                $(this).datagrid("clearSelections"); //取消所有选中项
                $(this).datagrid("selectRow", rowIndex); //根据索引选中该行

                lastSelectRowData = getRowIndex(rowIndex);
                if (lastSelectRowData) {
                    OpenCarInfo();
                }
            }
        },

        selectOnCheck: false,
        checkOnSelect: false
    });


    //判断是否为单车用户
    UserCookie = GetUserInfo();

    if (UserCookie != null) {
        if (UserCookie["OneCarUser"] == 1) {//单车用户
            $('#tb-text7').hide();
            monitorcar = [];
            var tmpcid = UserCookie["UID"];
            monitorcar.push(tmpcid);
            $("#cars").val(tmpcid);
            $('#loading-track').window('open');
            GetCarLastTrack();
        }
        else {
            $('#tb-text7').show();
        }


        var roles = UserCookie.A_Name;
        var rolesarray = roles.split(',');
        var returnflag = GetQueryString("returnflag");
        if (arraycontains(rolesarray, '017') && returnflag == null)
        { $('#tb-text5').show(); }
        else
        { $('#tb-text5').hide(); }
    }

    var grrsysflag = GetTopQueryString("key");

    if (grrsysflag == "ALARM") {
        $('#tb-text2').hide();
        $('#tb-text5').hide();
    }
});


//返回行数据
function getRowIndex(index) {
    var data = getRowsData();
    if (data) {
        if (data.length > index) {
            return data[index];
        }
    }
    return "";
}

//返回当前页所有行数据
function getRowsData() {
    var data = $("#data_grid").datagrid("getRows");
    if (data) {
        return data;
    }
    return "";
}


function CallTrack() {
    parent.AutoShowTrack(lastSelectRowData);
}

function OpenCarInfo() {
    $('#loading-cardetail').window('open');
    if (lastSelectRowData != null) {
        var thiscid = lastSelectRowData["Carid"];
        //alert(thiscid);
        var mydata = {
            "sid": "monitor-car-detailquery",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cid": thiscid
        };
        BaseGetData(mydata, ShowCarDetail);

    }
}


function ShowCarDetail(obj) {
    $('#loading-cardetail').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            var CarArray = obj.result.records;
            if (CarArray.length > 0) {
                $('#lCarNo')[0].innerHTML = CarArray[0].CarNo;
                $('#lDPH')[0].innerHTML = CarArray[0].DPH;
                $('#lCarDName')[0].innerHTML = CarArray[0].CarDName;
                $('#lCarDT')[0].innerHTML = CarArray[0].CarDT;
                $('#lActiv')[0].innerHTML = CarArray[0].Active;
                $('#lLockS')[0].innerHTML = CarArray[0].LockStatus;
                $('#lRepaymentDate')[0].innerHTML = CarArray[0].RepaymentDate;
                $('#lRemindDays')[0].innerHTML = CarArray[0].RemindDays;
                $('#lDisconTimes')[0].innerHTML = CarArray[0].DisconTimes;
            }
            $('#w_CarInfo').window('open');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('车辆信息查询，服务器超时！');
            }
        }
    }
}
//回调函数，接收参数为选择的车辆数据行
function CallBackFun(obj) {
    var cardata = obj;
    var cars = "";
    if (cardata.length > 0) {
        if (cardata.length > 300) {
            $.messager.alert("提示信息", "所选择的车辆大于300辆，仅监控前300辆！", 'info');
            cardata.splice(300, cardata.length-300);
        }
        //monitorcar = [];
        $.each(cardata, function (i, item) {
            //            if (monitorcar.indexOf(item.cid) == -1) {
            //                monitorcar.push(item.cid);
            //            }
            if (!arraycontains(monitorcar, item.cid)) {
                monitorcar.push(item.cid);
            }
        });

    } else {
        cars = "";
    }
    cars = monitorcar.join(',');
    $("#cars").val(cars);
    //获取最后一点信息
    $('#loading-track').window('open');
    GetCarLastTrack();
    SetFunction(GetCarLastTrack); //保存方法指针，供指令回传使用
}


function selectCar() {
    //CarSelect(false, CallBackFun, monitorcar);
    CarSelect(false, CallBackFun, []);
}
function loadfinish(data) {
    var rowsdata = data.rows
    $.each(rowsdata, function (idx, val) {
        var tmpid = idx;
        var tmpobject = val;
        $.each(selectedcar, function (i, value) {
            if (tmpobject.Carid == value) {
                $("#data_grid").datagrid("checkRow", tmpid);
            }
        });
    });
}

function scar(rowIndex, rowData) {
    var btmp = false;
    $.each(selectedcar, function (i, value) {
        if (value == rowData.Carid) {
            btmp = true;
        }
    });
    if (!btmp) {
        selectedcar.push(rowData.Carid);
    }
}
function uscar(rowIndex, rowData) {
    $.each(selectedcar, function (i, item) {
        if (item == rowData.Carid) {
            selectedcar.elremove(i);
        }
    });
}
function monitorrun() {
    if (monitorfalg == false) {
        if (monitorcar.length <= 0) {
            $.messager.alert('提示信息', "先选择监控车辆！", 'info');
            return;
        }
        monitorfalg = true;
        //$('#loading-track').window('open');
        //先获取一下最新数据
        fun();
        interval = setInterval(fun, time);
        $("#tb-run").tooltip({
            content: "停止监控",
            position: 'top'
        });
    } else {
        clearInterval(interval);
        interval = null;
        monitorfalg = false;
        $("#tb-run").tooltip({
            content: "开始监控",
            position: 'top'
        });
    }
}
function fun() {
    if (monitorfalg == true) {
        GetCarLastTrack();
    }
}
function monitordelete() {
    var rows = $("#data_grid").datagrid("getChecked");
    $.each(rows, function (key, val) {
        var tmpcid = val.Carid;
        var index = $("#data_grid").datagrid("getRowIndex", val);
        $.each(monitorcar, function (i, item) {
            if (tmpcid == item) {
                monitorcar.elremove(i);
            }
        });
        $("#data_grid").datagrid("deleteRow", index);
    });
    GeneratePoint();
    selectedcar = [];
    var cars = monitorcar.join(',');
    $("#cars").val(cars);

    if (monitorcar.length == 0) {//全部删除完 就停止监控
        clearInterval(interval);
        interval = null;
        monitorfalg = false;
        $("#tb-run").tooltip({
            content: "开始监控",
            position: 'top'
        });
    }
}

function GetCarLastTrack() {
    var ret = "";
    ret = $("#cars").val();
    if (ret.length <= 0) {
        $('#loading-track').window('close');
        $.messager.alert('提示信息', "请选择监控车辆！", 'info');
        return;
    }
    //var UserCookie = GetUserInfo();
    var paramdata = { "sid": "sys-usercar-getlasttrack", "CIDs": ret, "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString() };
    BaseGetData(paramdata, SuccessCallBack);
}

function SuccessCallBack(msg) {
    $('#loading-track').window('close');
    var arr = msg;
    if (arr['state'] == 100) {
        var data = arr['result'];
        carsdata = data.records;
        //判断是否为报警信息，如果有报警信息，传送到main页面
        alarmreport(carsdata);
        $('#data_grid').datagrid({
            data: data.records
        });
        GeneratePoint(selectedCarNO);
    } else {
        $.messager.alert('错误信息', arr['msg'], 'error');
    }
}

function alarmreport(data) {
    if (data.length <= 0) {
        return
    }
    var alarmdata = new Array();
    for (var i = 0; i < data.length; i++) {
        //if (data[i].Alarm == 0 && data[i].Alarm808 == 0 && data[i].AlarmExt808 == 0)
        if (data[i].AlarmStr == "无" || data[i].AlarmStr == "正常") {
            continue;
        } else {
            alarmdata.push(data[i]);
        }
    }
    if (alarmdata.length > 0) {
        SendAlarmInfo(alarmdata);
    }

}
function ExportCarLastTrack() {
    var ret = "";
    ret = $("#cars").val();
    if (ret.length <= 0) {
        $('#loading-track').window('close');
        //$.messager.alert('提示信息', "请选择监控车辆！", 'info');
        return;
    }
    //var UserCookie = GetUserInfo();
    var paramdata = { "sid": "sys-usercar-exportlasttrack", "CIDs": ret, "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString() };
    ExcelExport(paramdata);
}
function ExportExcel() {
    // 返回grid的所有可见行给后端供导出Excel用
    var rows = $('#data_grid').datagrid("getRows");
    if (rows.length == 0) {
        //msgShow("没有数据可供导出");
        return;
    }
    AddRunningDiv();

    //返回grid的所有列的选项title、列宽等
    // var columns = $('#userlist').datagrid("options").columns;

    //定制DataGrid的columns信息,只返回{field:,title:}
    var columns = new Array();
    var fields = $('#data_grid').datagrid('getColumnFields');
    for (var i = 0; i < fields.length; i++) {
        var opts = $('#data_grid').datagrid('getColumnOption', fields[i]);
        var column = new Object();
        column.field = opts.field;
        column.title = opts.title;
        columns.push(column);
    }
    var excelWorkSheet = new Object();
    excelWorkSheet.rows = rows;
    excelWorkSheet.columns = columns;
    excelWorkSheet.sheetName = "ExportCarTrack";

    var strRet = JSON.stringify(excelWorkSheet);

    //
    //var UserCookie = GetUserInfo();

    //
    var paramdata = { "sid": "sys-usercar-exportlasttrack", "excelWorkSheet": strRet, "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString() };

    CarTrackExcelExport(paramdata);

    //    //
    //    //BaseGetData(paramdata, SuccessCallBack);

    //    var rootpath = getBasePath();
    //    var async = true;
    //    var Requrl = rootpath + '/Service/HttpService.ashx?random=' + Math.random();
    //    // alert(1)
    ////    $.ajax({
    ////        type: 'POST',
    ////        async: async, //此标记标示同步
    ////        url: Requrl,
    ////        dataType: 'json',
    ////        data: JSON.stringify(paramdata),
    ////        success: function (msg) {
    ////            if (msg.state == 104) {
    ////                LoginTimeout('服务器超时！');
    ////            }
    ////            else {
    ////                SuccessCallBack(msg, re_data);
    ////            }
    ////        }
    ////        });

    //    //get
    //    Requrl += '&sid=sys-usercar-exportlasttrack&sysuid=';
    //    Requrl += UserCookie["UID"];.
    //    Requrl += '&token=';
    //    Requrl += UserCookie["token"].toString();
    //    Requrl += '&sysflag=';
    //    Requrl += UserCookie["sysflag"].toString();
    //    Requrl += '&excelWorkSheet=';
    //    Requrl += strRet.toString();



    //    location.href = Requrl;

}

function CarTrackExcelExport(UrlData) {
    var async = true;
    var demo = JSON.stringify(UrlData);
    var rootpath = getBasePath();

    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        dataType: 'json',
        data: demo,
        success: function (resdata) {
            //
            $('#loading-track').window('close');

            MoveRunningDiv();
            if (resdata != null && resdata != "") {
                var arr = resdata;
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {
                    if (resdata.state == 100) {
                        var asa = rootpath + '\\' + resdata.result;
                        window.open(asa, '_self');
                        //$.messager.alert("操作提示", resdata.result, "ok");
                    }
                    else {
                        $.messager.alert("操作失败", resdata.msg, "ok");
                    }
                }
            }
        }
    });
}


function PositionSearch() { //车辆点名
    var rows = $("#data_grid").datagrid("getChecked");
    if (rows.length <= 0) {
        $.messager.alert('提示信息', "请选择车辆！", 'error');
    }
    else if (rows.length > 1) {
        $.messager.alert('提示信息', "点名只能选择1辆车！", 'error');
    }
    else {
        var cid = rows[0].Carid;
        var tno = rows[0].TNO;
        var carno = rows[0].CarNum;
        var mydata = {
            "sid": "order-send-positionsearch",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cid": cid,
            "tno": tno,
            "carno": carno
        };
        BaseGetData(mydata, SendPostionSearchRes);
    }
}

function SendPostionSearchRes(res) {
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

function PohtoCut() { //立即拍照
    var rows = $("#data_grid").datagrid("getChecked");
    if (rows.length <= 0) {
        $.messager.alert('提示信息', "请选择车辆！", 'error');
    }
    else if (rows.length > 1) {
        $.messager.alert('提示信息', "立即拍照只能选择1辆车！", 'error');
    }
    else {
        var cid = rows[0].Carid;
        var tno = rows[0].TNO;
        var carno = rows[0].CarNum;
        var mydata = {
            "sid": "order-send-imagedown",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "cid": cid,
            "tno": tno,
            "carno": carno,
            "ch": "1"
        };
        BaseGetData(mydata, SendImageRequestDownRes);
    }
}

function SendImageRequestDownRes(res) {
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

//------------------界面事件控制模型--------------
function showGridRows() {
    $('#w').window('open');
}
function showRows(ev) {
    if ($(ev)[0].checked) {
        $('#data_grid').datagrid('showColumn', $(ev)[0].name);
    }
    else {
        $('#data_grid').datagrid('hideColumn', $(ev)[0].name);
    }
}

function onComoItemSelectd(record) {
    var value = $('#cc').combobox('getValue');
    time = parseInt(value);

    if (monitorfalg == true) {
        clearInterval(interval);
        interval = null;
        interval = setInterval(fun, time);
    }
}
