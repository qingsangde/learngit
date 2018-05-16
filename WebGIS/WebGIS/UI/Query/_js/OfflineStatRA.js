var stagrid;
var ms_STime;
var ms_ETime;
var alldataInfo;
var CIDS;
var sel_CID;
$(function () {
    UserCookie = GetUserInfo();
    //    if (UserCookie.OneCarUser == "1") {
    //        CIDS = UserCookie.UID.toString();
    //        $('#txtCarNo').val(UserCookie.UName.toString());
    //        $("#tb-selcar").attr({ "disabled": "disabled" });
    //        $("#tb-selcar").css("display", "none");  
    //    }
    var myDate = new Date();
    var st = myDate.format("yyyy-MM") + "-01 00:00:00";
    var et = myDate.format("yyyy-MM-dd hh:mm:ss");
    $("#STime").datetimebox('setValue', st);
    $("#ETime").datetimebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
    $('#MapWindows').window('open');
    mapObj = new AMap.Map("iCenter", { doubleClickZoom: false });
    $('#MapWindows').window('close');
});

//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "CarNo") {
                //打开子页面
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                //alert(sel_CID);
                queryDataInfo(sel_CID);
            }
        }
    });

    $("#data_grid_info").datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "FromAddress" || field.toString() == "ToAddress") {
                arr = value.toString().replace(/[ ]/g, "").split(",");
                if (changeTwoDecimal_f(arr[0]) == '0.00') {
                    mapObj.clearMap();
                    $('#MapWindows').window('close');
                } else {
                    mapObj.clearMap();
                    $('#MapWindows').window('open');
                    addMarker(arr[0], arr[1]);
                }
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

function queryData_Os() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    //$('#STime').datebox('setValue','2013-08-01');
    //$('#ETime').datebox('setValue', '2013-08-20');
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    if (!ComYue(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
    var offInterval = $("#txtMinute").val();
    //    ms_STime = '2013-08-01';
    //    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-offlineRA-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "CarNum": $("#txtCph").val(), "CUID": $("#txtClyt").val(), "CarOwnName": $("#txtSsqy").val(), "Line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString(), "num": offInterval };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}


function Export() {
    var UserCookie = GetUserInfo();
    //$('#STime').datebox('setValue','2013-08-01');
    //$('#ETime').datebox('setValue', '2013-08-20');
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    if (!ComYue(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
    var offInterval = $("#txtMinute").val();
    //    ms_STime = '2013-08-01';
    //    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-offlineRA-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "CarNum": $("#txtCph").val(), "CUID": $("#txtClyt").val(), "CarOwnName": $("#txtSsqy").val(), "Line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString(), "num": offInterval };
    ExcelExport(mydata);

}

//保留两位小数
function changeTwoDecimal_f(x) {
    var f_x = parseFloat(x);
    if (isNaN(f_x)) {
        alert('function:changeTwoDecimal->parameter error');
        return false;
    }
    var f_x = Math.round(x * 100) / 100;
    var s_x = f_x.toString();
    var pos_decimal = s_x.indexOf('.');
    if (pos_decimal < 0) {
        pos_decimal = s_x.length;
        s_x += '.';
    }
    while (s_x.length <= pos_decimal + 2) {
        s_x += '0';
    }
    return s_x;
}

function queryDataInfo(cid) {

    $('#data_grid_info').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    var offInterval = $("#txtMinute").val();
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    $('#InfoWindows').window('open');
    var mydata = { "sid": "sta-offlineRA-search-info", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "cid": cid, "num": offInterval };
    BindDataGrid_FrontPage_Info(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage_Info);

}

function BindDataGrid_FrontPage_Info(UrlData, CallBack, Async) {
    $('#loading-track').window('open');
    var async = true;
    if (Async == false) {
        async = false;
    }
    var demo = O2String(UrlData);
    var rootpath = getBasePath();
    reqDate = new Date();
    $.ajax({
        type: 'POST',
        async: async, //此标记标示同步
        url: rootpath + '/Service/HttpService.ashx?random=' + Math.random(),
        //timeout: 1000000,
        dataType: 'json',
        data: demo,
        error: function (d) {
            $('#loading-track').window('close');
            $.messager.alert("查询失败", "错误代码：" + d.status + "  " + d.statusText);
        },
        success: function (resdata) {
            $('#loading-track').window('close');
            if (resdata.state == 104) {
                LoginTimeout('服务器超时！');
            }
            if (resdata.result != null && resdata.result != "") {
                var arr = resdata;

                alldataInfo = arr.result.records;
                if (resdata.result.total == 0) {
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
                CallBack(async, alldata);
                respDate = new Date();
                $.messager.alert("查询成功", "总计:" + alldataInfo.length + ",耗时:" + (respDate - reqDate) / 1000 + "s");
                // }
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
    var UserCookie = GetUserInfo();

    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    var offInterval = $("#txtMinute").val();

    var mydata = { "sid": "sta-offlineRA-search-info-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "cid":sel_CID, "num": offInterval };
    ExcelExport(mydata);
}

