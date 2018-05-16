var stagrid;
var mc_STime;
var mc_ETime;
var alldataInfo;
var CIDS;
var sel_CID;
var sel_alarmtype;
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
//    if (UserCookie.OneCarUser == "1") {
//        CIDS = UserCookie.UID.toString();
//        $('#txtCarNo').val(UserCookie.UName.toString());
//        $("#tb-selcar").attr({ "disabled": "disabled" });
//        $("#tb-selcar").css("display", "none");  
    //    }
    var date = new Date(); 
    var yesterday_milliseconds=date.getTime()-1000*60*60*24;       
    var yesterday = new Date();       
     yesterday.setTime(yesterday_milliseconds);  

   // var myDate = new Date();
     var st = yesterday.format("yyyy-MM-dd");
    var et = yesterday.format("yyyy-MM-dd");
    $("#STime").datebox('setValue', st);
    $("#ETime").datebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
    $('#MapWindows').window('open');
    mapObj = new AMap.Map("iCenter", { doubleClickZoom: false });
    $('#MapWindows').window('close');
});

//查询里程统计
function queryData_Mc() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
//    $('#STime').datebox('setValue','2013-08-01');
//    $('#ETime').datebox('setValue', '2013-08-03');
    mc_STime = $('#STime').datebox('getValue') + " 00:00:00";
    mc_ETime = $('#ETime').datebox('getValue') + " 23:59:59";
    if (!ComYue(mc_STime, mc_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }

    var mydata = { "sid": "sta-mileagecollect-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": mc_STime, "etime": mc_ETime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//里程统计导出
function Export() {

    var UserCookie = GetUserInfo();
    mc_STime = $('#STime').datebox('getValue') + " 00:00:00";
    mc_ETime = $('#ETime').datebox('getValue') + " 23:59:59";
    if (!ComYue(mc_STime, mc_ETime)) {
        $.messager.alert("操作提示", "导出时间不能跨越月份！", "error");
        return;
    }
    //CIDS = '1,25';
    var mydata = { "sid": "sta-mileagecollect-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": mc_STime, "etime": mc_ETime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    ExcelExport(mydata);
}
//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "T_Lati_T_Long" || field.toString() == "Last_Lati_Last_Long") {
                mapObj.clearMap();
                $('#MapWindows').window('open');
                arr = value.toString().replace(/[ ]/g, "").split("-");
                addMarker(arr[1], arr[0]);
            } else if (field.toString() == "CarNo") {
                sel_CID = getRowIndex(rowIndex)["cid"].toString();
                queryDataInfo(sel_CID);
            }
        }
    });
    $("#data_grid_info").datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "FromLngLat" || field.toString() == "ToLngLat") {
                mapObj.clearMap();
                $('#MapWindows').window('open');
                arr = value.toString().replace(/[ ]/g, "").split(",");
                addMarker(arr[0], arr[1]);
            }
        }
    });

}

//里程详细导出
function ExportInfo() {

    var UserCookie = GetUserInfo();
    var mydata = { "sid": "sta-mileagecollect-info-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": mc_STime, "etime": mc_ETime, "cid": sel_CID };
    ExcelExport(mydata);
}


//里程详细查询
function queryDataInfo(cid) {
    $('#data_grid_info').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    $('#InfoWindows').window('open');
    var mydata = { "sid": "sta-mileagecollect-info", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": mc_STime, "etime": mc_ETime, "cid": cid };
    BindDataGrid_FrontPage_Info(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage_Info);
}


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
    var data = stagrid.datagrid("getRows");
    if (data) {
        return data;
    }
    return "";
}

function QueryPrevMonthData() {
    var today = new Date();
    var year = today.getFullYear();
    var month = today.getMonth();
    var lastYear;
    var lastMonth;
    if (month == 0) {
        lastYear = year - 1;
        lastMonth = 11;
    }
    else {
        lastYear = year;
        lastMonth = month - 1;
    }
    var last00 = new Date(lastYear, lastMonth, 0);
    var last_milliseconds = last00.getTime() + 1000 * 60 * 60 * 24;
    var sdate = new Date();
    sdate.setTime(last_milliseconds);
    var edate = new Date(year, month, 0);
    $("#STime").datebox('setValue', sdate.format("yyyy-MM-dd"));
    $("#ETime").datebox('setValue', edate.format("yyyy-MM-dd"));
    queryData_Mc();
}




/*
绑定DataGrid 前端分页
UrlData：URL参数
CallBack：回调函数
Async：同步异步
*/
function BindDataGrid_FrontPage_Info(UrlData, CallBack, Async) {
    $('#loading-track').window('open');
    var async = true;
    if (Async == false) {
        async = false;
    }
    var demo = O2String(UrlData);
    var rootpath = getBasePath();
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        dataType: 'json',
        data: demo,
        success: function (resdata) {
            $('#loading-track').window('close');
            if (resdata != null && resdata != "") {
                var arr = resdata;
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                }
                else {

                    alldataInfo = arr.result.records;
                    if (alldataInfo.total == 0) {
                        $.messager.alert("提示信息", "未检索到数据！");
                    }
                    var pg = $('#data_grid_info').datagrid("getPager");
                    $("#data_grid_info").datagrid("loadData", alldataInfo.slice(0, 10));
                    
                    pg.pagination('refresh', {
                        total: alldataInfo.length,
                        pageNumber: 1,
                        pageSize: 10,
                        buttons: [{
                            iconCls: 'icon-save',
                            handler: function () { ExportInfo(); }
                        }]
                    });
                    CallBack(async, alldataInfo);
                }
            } else {
                $.messager.alert("提示信息", resdata.msg.toString());
            }
        }
    });
}
/*
绑定成功DataGrid回调函数 前端分页
Async：同步异步
*/
function BingDataGrid_LoadSuccessCallBack_FrontPage_Info(Async, urlData) {
    var pg = $('#data_grid_info').datagrid("getPager");
    if (pg) {
        pg.pagination({
            onSelectPage: function (pageNumber, pageSize) {
                var start = (pageNumber - 1) * pageSize;
                var end = start + pageSize;

                $("#data_grid_info").datagrid("loadData", alldataInfo.slice(start, end));
                pg.pagination('refresh', {
                    total: alldataInfo.length,
                    pageNumber: pageNumber
                });
            }
        });
    }

}


function selectCar() {
    CarSelect(false, CallBackFun);
}
function CallBackFun(obj) {
    CIDS = '';
    var carNOs = '';
    for (var i = 0; i < obj.length; i++) {
        CIDS += obj[i].cid.toString() + ',';
        carNOs += obj[i].carno.toString() + ',';
    }
    CIDS = CIDS.substring(0, CIDS.length - 1);
    carNOs = carNOs.substring(0, carNOs.length - 1);
    $('#txtCarNo').val(carNOs.toString());
}