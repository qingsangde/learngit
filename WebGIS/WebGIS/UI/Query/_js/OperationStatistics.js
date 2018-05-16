var stagrid;
var ms_STime;
var ms_ETime;
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
    var myDate = new Date();
    var yesterday = new Date(myDate.getTime() - 24 * 60 * 60 * 1000); 
    var st = yesterday.format("yyyy-MM-dd") + " 00:00:00";
    var et = yesterday.format("yyyy-MM-dd") + " 23:59:59";
    $("#STime").datetimebox('setValue', st);
    $("#ETime").datetimebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
    $('#MapWindows').window('open');
    mapObj = new AMap.Map("iCenter", { doubleClickZoom: false });
    $('#MapWindows').window('close');
});

//车辆运行统计查询
function queryData_Os() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    //$('#STime').datebox('setValue','2013-08-01');
    //$('#ETime').datebox('setValue', '2013-08-20');
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    if (!ComRi(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越1天！", "error");
        return;
    }
//    ms_STime = '2013-08-01';
//    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-operationstatistics-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "CarNum": $("#txtCph").val(), "CUID": $("#txtClyt").val(), "CarOwnName": $("#txtSsqy").val(), "Line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//车辆运行统计导出
function Export() {

    var UserCookie = GetUserInfo();
    ms_STime = $('#STime').datetimebox('getValue');
    ms_ETime = $('#ETime').datetimebox('getValue');
    if (!ComRi(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越1天！", "error");
        return;
    }
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-operationstatistics-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "CarNum": $("#txtCph").val(), "CUID": $("#txtClyt").val(), "CarOwnName": $("#txtSsqy").val(), "Line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    ExcelExport(mydata);
}




//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "StartCoord" || field.toString() == "EndCoord") {

                arr = value.toString().replace(/[ ]/g, "").split("-");
                if (arr[0] == '') {

                } else {
                    mapObj.clearMap();
                    $('#MapWindows').window('open');
                    addMarker(arr[1], arr[0]);
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






//选车
function selectCar() {
    CarSelect(false, CallBackFun);
}
//选车回调
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