var stagrid;
var msp_STime;
var msp_ETime;
var alldataInfo;
var sel_CID;
var UserCookie;
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
    var myDate = new Date();
    var st = myDate.format("yyyy-MM") + "-01 00:00:00";
    var et = myDate.format("yyyy-MM-dd hh:mm:ss");
    $("#STime").datetimebox('setValue', st);
    $("#ETime").datetimebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
});

//查询车辆超速信息
function queryData_Msp() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    msp_STime = $('#STime').datetimebox('getValue');
    msp_ETime = $('#ETime').datetimebox('getValue');

    var mydata = {
        "sid": "fence-alarm-sta-list",
        "uid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "st": msp_STime,
        "et": msp_ETime,
        "carno": $("#txtCph").val(),
        "cuid": $("#txtClyt").val(),
        "ownname": $("#txtSsqy").val(),
        "line": "", 
        "onecaruser": UserCookie["OneCarUser"].toString() };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//查询车辆超速信息导出
function Export() {

    msp_STime = $('#STime').datetimebox('getValue');
    msp_ETime = $('#ETime').datetimebox('getValue');


    var mydata = {
        "sid": "fence-alarm-sta-listexport",
        "uid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "st": msp_STime,
        "et": msp_ETime,
        "carno": $("#txtCph").val(),
        "cuid": $("#txtClyt").val(),
        "ownname": $("#txtSsqy").val(),
        "line": "",
        "onecaruser": UserCookie["OneCarUser"].toString()
    };
    ExcelExport(mydata);
}


//查询车辆超速信息详细导出
function ExportInfo() {
    var mydata = {
        "sid": "fence-alarm-sta-detailexport",
        "uid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "st": msp_STime,
        "et": msp_ETime,
        "cid": sel_CID
    };
    ExcelExport(mydata);
}

//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "CarNo") {
                sel_CID = getRowIndex(rowIndex)["cid"].toString();
                queryDataInfo(sel_CID);
            }
        }
    });
}

//查询车辆超速信息详细查询
function queryDataInfo(cid, alarmtype) {
    $('#data_grid_info').datagrid('loadData', { total: 0, rows: [] });
    $('#InfoWindows').window('open');
    //var mydata = { "sid": "sta-mileagespeed-info", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": msp_STime, "etime": msp_ETime, "cid": cid, "alarmtype": alarmtype };
    var mydata = {
        "sid": "fence-alarm-sta-detail",
        "uid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "st": msp_STime,
        "et": msp_ETime,
        "cid": sel_CID
       };
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
                    if (alldataInfo.length == 0) {
                        $.messager.alert("提示信息", "未检索到数据！");
                    }
                    var pg = $('#data_grid_info').datagrid("getPager");
                    $("#data_grid_info").datagrid("loadData", alldataInfo.slice(0, 10));
                    pg.pagination('refresh', {
                        total: alldataInfo.length,
                        pageNumber: 1,
                        pageSize: 10,
                        buttons: [{
                            iconCls: 'icon-excel',
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