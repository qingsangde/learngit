var stagrid;
var mc_STime;
var mc_ETime;
var alldataInfo;
var sel_CID;
var UserCookie
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
    var date = new Date();
    var yesterday_milliseconds = date.getTime() - 1000 * 60 * 60 * 24;
    var yesterday = new Date();
    yesterday.setTime(yesterday_milliseconds);

    // var myDate = new Date();
    var st = yesterday.format("yyyy-MM-dd hh:mm:ss");
    var et = date.format("yyyy-MM-dd hh:mm:ss");
    $("#txtStime").datetimebox('setValue', st);
    $("#txtEtime").datetimebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
    $('#MapWindows').window('open');
    mapObj = new AMap.Map("iCenter", { doubleClickZoom: false });
    $('#MapWindows').window('close');
});


function doSelect() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    mc_STime = $('#txtStime').datetimebox('getValue');
    mc_ETime = $('#txtEtime').datetimebox('getValue');

    var mydata = {
                    "sid": "energy-analysis-list",
                    "sysuid": UserCookie["UID"],
                    "token": UserCookie["token"].toString(),
                    "sysflag": UserCookie["sysflag"].toString(),
                    "stime": mc_STime, 
                    "etime": mc_ETime,
                    "CarNo": $("#txtCarNo").val(),
                    "CarOwnName": $("#txtCarOwnName").val(),
                    "onecaruser": UserCookie["OneCarUser"].toString() 
                 };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//里程统计导出
function Export() {
    mc_STime = $('#txtStime').datetimebox('getValue');
    mc_ETime = $('#txtEtime').datetimebox('getValue');

    var mydata = { 
                    "sid": "energy-analysis-listexport",
                    "sysuid": UserCookie["UID"],
                    "token": UserCookie["token"].toString(),
                    "sysflag": UserCookie["sysflag"].toString(),
                    "stime": mc_STime,
                    "etime": mc_ETime,
                    "CarNo": $("#txtCarNo").val(),
                    "CarOwnName": $("#txtCarOwnName").val(),
                    "onecaruser": UserCookie["OneCarUser"].toString()
                    };
    ExcelExport(mydata);
}

//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "CarNo") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                queryDataInfo(sel_CID);
            }
        }
    });
    $("#data_grid_info").datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "FromLatLng" || field.toString() == "ToLatLng") {
                mapObj.clearMap();
                $('#MapWindows').window('open');
                arr = value.toString().replace(/[ ]/g, "").split(",");
                addMarker(arr[1], arr[0]);
            }
        }
    });

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

//能耗分析详细查询
function queryDataInfo(cid) {
    $('#data_grid_info').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    $('#InfoWindows').window('open');
    var mydata = {
                    "sid": "energy-analysis-detail",
                    "sysuid": UserCookie["UID"],
                    "token": UserCookie["token"].toString(),
                    "sysflag": UserCookie["sysflag"].toString(),
                    "stime": mc_STime,
                    "etime": mc_ETime,
                    "cid": cid 
                  };
    BindDataGrid_FrontPage_Info(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage_Info);
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

function ExportInfo() {
    var mydata = { 
                    "sid": "energy-analysis-detailexport",
                    "sysuid": UserCookie["UID"],
                    "token": UserCookie["token"].toString(),
                    "sysflag": UserCookie["sysflag"].toString(),
                    "stime": mc_STime,
                    "etime": mc_ETime,
                    "cid": sel_CID 
                 };
    ExcelExport(mydata);
}