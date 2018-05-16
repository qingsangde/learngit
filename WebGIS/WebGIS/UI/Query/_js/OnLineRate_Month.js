var ms_STime;
var ms_ETime;
var UserCookie;
var alldataInfo;
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
});

function queryData_col() {
    var options = {};
    var s = "[[]]";
    options.columns = eval(s);
    $('#dg').datagrid(options); 
    $('#dg').datagrid('loadData', { total: 0, rows: [] });
    ms_STime = $('#STime').datebox('getValue');
    ms_ETime = $('#ETime').datebox('getValue');
    if (!ComYue(ms_STime, ms_ETime)) {
        $.messager.alert("操作提示", "查询时间不能跨越月份！", "error");
        return;
    }
    var mydata = {
        "sid": "car-onlinerate-month",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "stime": ms_STime,
        "etime": ms_ETime,
        "onecaruser": UserCookie["OneCarUser"].toString()
    };
    BindDataGrid_FrontPage_Info(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage_Info);
}

function Export() {
    ms_STime = $('#STime').datebox('getValue');
    ms_ETime = $('#ETime').datebox('getValue');
    var mydata = {
        "sid": "car-onlinerate-month-export",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "stime": ms_STime,
        "etime": ms_ETime,
        "onecaruser": UserCookie["OneCarUser"].toString()
    };
    ExcelExport(mydata);
}

function InitGridColumns(titledata) {
    var dataarr = titledata;
    var options = {};
    if (dataarr.length > 0) {
        var colCount = dataarr.length;
        var s = "";
        s = "[[";
        if (colCount > 0) {
            for (var i = 0; i < colCount; i++) {
                s = s + "{field:'" + dataarr[i].field + "',title:'" + dataarr[i].title + "',width:80},"
            }
            s = s.substr(0, s.length - 1)
        }

        s = s + "]]";
        options = {};
        options.columns = eval(s);
        $('#dg').datagrid(options);  
    }
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
                InitGridColumns(resdata.result.records[0]); //初始化datagrid列
                var arr = resdata;

                alldataInfo = arr.result.records[1];
                if (alldataInfo.length == 0) {
                    $.messager.alert("提示信息", "未检索到数据！");
                }
                var pg = $('#dg').datagrid("getPager");
                $("#dg").datagrid("loadData", alldataInfo.slice(0, 20));
                pg.pagination('refresh', {
                    total: alldataInfo.length,
                    pageNumber: 1,
                    pageSize: 20
                });
                CallBack(async, alldataInfo);
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
    var pg = $('#dg').datagrid("getPager");
    if (pg) {
        pg.pagination({
            onSelectPage: function (pageNumber, pageSize) {
                var start = (pageNumber - 1) * pageSize;
                var end = start + pageSize;

                $("#dg").datagrid("loadData", alldataInfo.slice(start, end));
                pg.pagination('refresh', {
                    total: alldataInfo.length,
                    pageNumber: pageNumber
                });
            }
        });
    }

}



