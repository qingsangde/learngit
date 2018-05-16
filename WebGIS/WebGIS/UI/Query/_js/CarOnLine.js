
var CID_sf = "";
var UserCookie;
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
    $('#txtuname').val(UserCookie.UName.toString());
    stagrid = $('#data_grid');
});

//获取未上线车辆列表
function queryData_col() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    UserCookie = GetUserInfo();
//    $('#STime').datebox('setValue', '2013-12-01');
//    $('#ETime').datebox('setValue', '2014-12-30');
//    var STime = $('#STime').datebox('getValue');
//    var ETime = $('#ETime').datebox('getValue');
//    var uname = $("#txtuname").val();
//    if (!dateComparison(STime, ETime)) {
//        $.messager.alert("操作提示", "查询时间不能大于31天！", "error");
//        return;
//    }
//    var STime = '2013-07-01';
    //    var ETime = '2013-07-08';
    if (!exp($("#txtcaronline").val())) {
        $.messager.alert("操作提示", "车辆未在线天数无效", "error");
        return;
    }

    var mydata = { "sid": "sta-caronline-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "uid": UserCookie.UID.toString(), "datime": $("#txtcaronline").val().toString(), "onecaruser": UserCookie["OneCarUser"].toString() };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//未上线车辆列表导出
function Export() {
    
    UserCookie = GetUserInfo();
    if (!exp($("#txtcaronline").val())) {
        $.messager.alert("操作提示", "车辆未在线天数无效", "error");
        return;
    }
    var mydata = { "sid": "sta-caronline-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "uid": UserCookie.UID.toString(), "datime": $("#txtcaronline").val().toString(), "onecaruser": UserCookie["OneCarUser"].toString() };
    ExcelExport(mydata);
}
//数字校验
function exp(value) {
    if (value) {
        return /^[0-9]*[1-9][0-9]*$/.test(value);
    } else {
        return true;
    }     
}