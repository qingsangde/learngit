var stagrid;
var ms_STime;
var ms_ETime;
var alldataInfo;
var CIDS;
var sel_CID;
var sel_alarmtype;

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
            if (field.toString() == "OffCoord") {

                arr = value.toString().replace(/[ ]/g, "").split(",");

                if (changeTwoDecimal_f(arr[0]) == '0.00') {
                    mapObj.clearMap();
                    $('#MapWindows').window('close');
                } else {
                    mapObj.clearMap();
                    $('#MapWindows').window('open');
                    addMarker(arr[1], arr[0]);
                }
            }
        }
    });
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


//异常下线统计
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

    //    ms_STime = '2013-08-01';
    //    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-abnormaloffline-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "CarNum": $("#txtCph").val(), "CUID": $("#txtClyt").val(), "CarOwnName": $("#txtSsqy").val(), "Line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString(), "num":1000 };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//异常下线统计导出
function Export() {

    var UserCookie = GetUserInfo();
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    if (!ComYue(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-abnormaloffline-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "CarNum": $("#txtCph").val(), "CUID": $("#txtClyt").val(), "CarOwnName": $("#txtSsqy").val(), "Line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString(), "num":0 };
    ExcelExport(mydata);
}