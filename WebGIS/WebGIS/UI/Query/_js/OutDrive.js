
var CID_sf = "";
/*
初始化
*/
$(function () {
    stagrid = $('#data_grid');
    initgrid();
    $('#InfoWindows').window('open');
    mapObj = new AMap.Map("iCenter", { doubleClickZoom: false });
    $('#InfoWindows').window('close');
    UserCookie = GetUserInfo();
    if (UserCookie.OneCarUser == "1") {
        CID_sf = UserCookie.UID.toString();
        $('#txtCarNo').val(UserCookie.UName.toString());
        $("#tb-selcar").attr({ "disabled": "disabled" });
        $("#tb-selcar").css("display", "none");
    }

    var date = new Date();
    var yesterday_milliseconds = date.getTime() - 1000 * 60 * 60 * 24;
    var yesterday = new Date();
    yesterday.setTime(yesterday_milliseconds);

    var st = yesterday.format("yyyy-MM-dd ");
    var et = date.format("yyyy-MM-dd");
    $("#STime").datetimebox('setValue', st);
    $("#ETime").datetimebox('setValue', et);
});

//偏离试驾路线查询列表
function queryData_OD() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    //    $('#STime').datebox('setValue', '2013-07-01');
    //    $('#ETime').datebox('setValue', '2013-07-08');
    var STime = $('#STime').datetimebox('getValue');
    var ETime = $('#ETime').datetimebox('getValue');

    //    var STime = '2013-07-01';
    //    var ETime = '2013-07-08';

    var mydata = { "sid": "sys-outdrive-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "Page": "1", "Size": defultSize, "stime": STime, "etime": ETime, "cid": CID_sf };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);


    




}

//偏离试驾路线查询列表导出
function Export() {

    var UserCookie = GetUserInfo();
    var STime = $('#STime').datetimebox('getValue');
    var ETime = $('#ETime').datetimebox('getValue');
    //    var STime = '2013-07-01';
    //    var ETime = '2013-07-08';

    var mydata = { "sid": "sys-outdrive-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "Page": "1", "Size": defultSize, "stime": STime, "etime": ETime, "cid": CID_sf };
    ExcelExport(mydata);
}




//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "BEGINPOSITION") {
                mapObj.clearMap();
                $('#InfoWindows').window('open');
                arr = value.toString().replace(/[ ]/g, "").split("-");
                addMarker(arr[1], arr[0]);
            }
            if (field.toString() == "ENDPOSITION") {
                mapObj.clearMap();
                $('#InfoWindows').window('open');
                arr = value.toString().replace(/[ ]/g, "").split("-");
                addMarker(arr[1], arr[0]);
            }
        }
    });
}


//单选选车
function selectCarSingle() {
    CarSelect(true, CallBackFun);
}
//选车回调
function CallBackFun(obj) {
    CID_sf = "";
    CID_sf = obj[0].cid.toString();
    var carNOs = obj[0].carno.toString();
    $('#txtCarNo').val(carNOs.toString());
}

