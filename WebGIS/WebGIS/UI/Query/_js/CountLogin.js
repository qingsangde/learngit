
var CID_sf = "";
var UserCookie;
var STime = "";
var ETime = "";
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
    $('#txtuname').val(UserCookie.UName.toString());
    stagrid = $('#data_grid');
});

//获取用户登录统计信息
function queryData_cl() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    UserCookie = GetUserInfo();
    STime = $('#STime').datetimebox('getValue');
    ETime = $('#ETime').datetimebox('getValue');
    var uname = $("#txtuname").val();
//    if (!dateComparison(STime, ETime)) {
//        $.messager.alert("操作提示", "查询时间不能大于31天！", "error");
//        return;
//    }
//    var STime = '2013-07-01';
//    var ETime = '2013-07-08';

    var mydata = { "sid": "sta-countlogin-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": STime, "etime": ETime, "uname": uname };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//获取用户登录统计信息导出
function Export() {
    
    UserCookie = GetUserInfo();
    STime = $('#STime').datetimebox('getValue');
    ETime = $('#ETime').datetimebox('getValue');
//    var STime = '2013-07-01';
//    var ETime = '2013-07-08';
    var uname = $("#txtuname").val();
    var mydata = { "sid": "sta-countlogin-search-out", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": STime, "etime": ETime, "uname": uname };
    ExcelExport(mydata);
}


//单选
function selectCarSingle() {
    CarSelect(true, CallBackFun);
}

function CallBackFun(obj) {
    CID_sf = "";
    CID_sf = obj[0].cid.toString();
}

