var stagrid;
var ms_STime;
var ms_ETime;
var alldataInfo;
var CIDS;
var sel_CID;
var sel_signaltype;
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
    var st = myDate.format("yyyy-MM") + "-01";
    var et = myDate.format("yyyy-MM-dd");
    $("#STime").datebox('setValue', st);
    $("#ETime").datebox('setValue', et);
    stagrid = $('#data_grid');
    initgrid();
});

//信号量统计查询
function queryData_Ss() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    //$('#STime').datebox('setValue','2013-08-01');
    //$('#ETime').datebox('setValue', '2013-08-20');
    ms_STime = $('#STime').datebox('getValue') + " 00:00:00";
    ms_ETime = $('#ETime').datebox('getValue') + " 23:59:59";
    if (!ComYue(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
    //    ms_STime = '2013-08-01';
    //    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-signal-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//信号量统计导出
function Export() {

    var UserCookie = GetUserInfo();
    ms_STime = $('#STime').datebox('getValue') + " 00:00:00";
    ms_ETime = $('#ETime').datebox('getValue') + " 23:59:59";
    if (!ComYue(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "导出时间不能跨越月份！", "error");
        return;
    }
    //CIDS = '1,12,25';
    var mydata = { "sid": "sta-signal-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    ExcelExport(mydata);
}


//警情详细导出
function ExportInfo() {

    var UserCookie = GetUserInfo();
    var mydata = { "sid": "sta-signal-search-info-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "cid": sel_CID, "signaltype": sel_signaltype };
    ExcelExport(mydata);
}

//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (value.toString() == "0") {
                $.messager.alert("操作提示", "本条记录无统计明细！", "error");
                return;
            }
            if (field.toString() == "SPEED_NUM") {  //行驶记录仪速度信号
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "0";
                queryDataInfo(sel_CID, sel_signaltype);
            }
            else if (field.toString() == "ACC_NUM") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "1";
                queryDataInfo(sel_CID, sel_signaltype);
            }
            else if (field.toString() == "BRAKE_NUM") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "2";
                queryDataInfo(sel_CID, sel_signaltype);
            }
            else if (field.toString() == "TURN_LEFT_NUM") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "3";
                queryDataInfo(sel_CID, sel_signaltype);
            }
            else if (field.toString() == "TURN_RIGHT_NUM") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "4";
                queryDataInfo(sel_CID, sel_signaltype);
            }
            else if (field.toString() == "DISTANT_NUM") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "5";
                queryDataInfo(sel_CID, sel_signaltype);
            }
            else if (field.toString() == "NEAR_NUM") {
                sel_CID = getRowIndex(rowIndex)["CID"].toString();
                sel_signaltype = "6";
                queryDataInfo(sel_CID, sel_signaltype);
            }
        }
    });
}

//警情详细查询
function queryDataInfo(cid, signaltype) {
    $('#data_grid_info').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    $('#InfoWindows').window('open');
    var mydata = { "sid": "sta-signal-search-info", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": ms_STime, "etime": ms_ETime, "cid": cid, "signaltype": signaltype };
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