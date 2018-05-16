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
});

//获取未上线车辆列表
function queryData_col() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });

    var mydata = {
        "sid": "sys-distributioncarsta-search",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "onecaruser": UserCookie["OneCarUser"].toString(),
        "carno": $("#txtcarno").val(),
        "carownname": $("#txtcarown").val(),
        "stime":$('#STime').datetimebox('getValue'),
        "etime":$('#ETime').datetimebox('getValue')
    };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//未上线车辆列表导出
function Export() {

    var mydata = {
        "sid": "sys-distributioncarsta-output",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "carno": $("#txtcarno").val(),
        "carownname": $("#txtcarown").val(),
        "stime": $('#STime').datetimebox('getValue'),
        "etime": $('#ETime').datetimebox('getValue')
    };
    ExcelExport(mydata);
}
