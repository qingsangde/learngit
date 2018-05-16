var stagrid;
var pt_STime;
var pt_ETime;
var alldataInfo;
var CIDS;
var sel_CID;
var sel_alarmtype;
var stoptime;
/*
初始化
*/
$(function () {
    stagrid = $('#data_grid');
    initgrid();
    $('#MapWindows').window('open');
    mapObj = new AMap.Map("iCenter", { doubleClickZoom: false });
    $('#MapWindows').window('close');
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
});

//查询停车统计
function queryData_Pt() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
//    $('#STime').datebox('setValue','2013-08-01');
//    $('#ETime').datebox('setValue', '2013-08-03');
    pt_STime = $('#STime').datetimebox('getValue');
    pt_ETime = $('#ETime').datetimebox('getValue');
    if (!ComYue(pt_STime, pt_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
//    ms_STime = '2013-08-01';
//    ms_ETime = '2013-08-20';

    //var CID = '18,12,1,2,3,4,5,6,7,8,9';
    //stoptime = '00:01';
    stoptime = $("#txtHou").val() + ':' + $("#txtMin").val();
    //CIDS = '1,12,25,';
    if (!exp($("#txtHou").val()) || !exp($("#txtMin").val()) || ($("#txtHou").val()) > 23 || ($("#txtMin").val()) > 59) {
        $.messager.alert("错误", "停车时长错误");
        return;
        
    }

    var mydata = { "sid": "sta-parktotal-search", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": pt_STime, "etime": pt_ETime, "stoptime": stoptime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//导出停车统计
function Export() {

    var UserCookie = GetUserInfo();
    pt_STime = $('#STime').datetimebox('getValue');
    pt_ETime = $('#ETime').datetimebox('getValue');
    if (!ComYue(pt_STime, pt_ETime)) {
        $.messager.alert("操作提示", "导出时间不能跨越月份！", "error");
        return;
    }
    stoptime = $("#txtHou").val() + ':' + $("#txtMin").val();
    //CIDS = '1,12,25,';
    var mydata = { "sid": "sta-parktotal-search-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": pt_STime, "etime": pt_ETime, "stoptime": stoptime, "carnum": $("#txtCph").val(), "cuid": $("#txtClyt").val(), "carownname": $("#txtSsqy").val(), "line": $("#txtYyxl").val(), "onecaruser": UserCookie["OneCarUser"].toString() };
    ExcelExport(mydata);
}
//画面项目初期化
function initgrid() {
    stagrid.datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "vcCarNo") {
                sel_CID = getRowIndex(rowIndex)["c_ID"].toString();
                queryDataInfo(sel_CID);
            }
        }
    });
    $("#data_grid_info").datagrid({
        onClickCell: function (rowIndex, field, value) {
            if (field.toString() == "T_lati") {
                mapObj.clearMap();
                $('#MapWindows').window('open');
                arr = value.toString().replace(/[ ]/g, "").split("-");
                addMarker(arr[1], arr[0]);
            }
        }
    });

}

//停车详细导出
function ExportInfo() {

    var UserCookie = GetUserInfo();
    var mydata = { "sid": "sta-parktotal-info-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": pt_STime, "etime": pt_ETime, "cid": sel_CID, "stoptime": stoptime };
    ExcelExport(mydata);
}


//停车详细查询
function queryDataInfo(cid) {
    $('#data_grid_info').datagrid('loadData', { total: 0, rows: [] });
    var UserCookie = GetUserInfo();
    $('#InfoWindows').window('open');
    var mydata = { "sid": "sta-parktotal-info", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(), "stime": pt_STime, "etime": pt_ETime, "cid": sel_CID, "stoptime": stoptime };
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
function CallBackFun(obj) {
    CIDS = '';
    var carNOs = '';
    for (var i = 0; i < obj.length; i++) {
        CIDS += obj[i].cid.toString() + ',';
        carNOs += obj[i].carno.toString() + ',';
    }
   
    carNOs = carNOs.substring(0, carNOs.length - 1);
    $('#txtCarNo').val(carNOs.toString());
}
//纯数字校验
function exp(value) {
    if (value) {
        return /^[0-9]*[0-9][0-9]*$/.test(value);
    } else {
        return true;
    }
}