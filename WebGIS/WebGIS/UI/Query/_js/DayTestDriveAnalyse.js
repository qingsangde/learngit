var UserCookie;
//初始化
$(function () {
    UserCookie = GetUserInfo();
    setSSalesProvince("");
    setFun();


});


//轿贸-经销商角色控制
function setFun() {
    if (UserCookie.RID == "23") {
        $("#tdP").hide()
        $("#SDealersCode").textbox('setText', UserCookie.UName)
        $('#SDealersCode').textbox('disable', true);
    }
    var myDate = new Date();
    var et = myDate.format("yyyy-MM-dd hh:mm:ss");
    $("#STime").datebox('setValue', et);  
}

function queryTDDaySearch() {
    
    if ($('#STime').datebox('getValue') == "") {
        $.messager.alert('错误信息', '请选择查询月份', 'error');
        return;
    }
    if ($("#SCarType").val() == "") {
        $.messager.alert('错误信息', '请输入车型', 'error');
        return;
    }
    LoadDatagrid();
}

function LoadDatagrid() {
    var mydata = {
        "sid": "DayTestDriveAnalyse-getlist",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "STime": $('#STime').datebox('getValue'),       //查询年月
        "SSalesRegion": $('#SSalesRegion').combobox('getValue').trim(),        //销售大区
        "SSalesProvince": $('#SSalesProvince').combobox('getValue').trim(),    //经销商名称
        "SDealersName": $('#SDealersName').combobox('getValue').trim(),        //经销商名称
        "SDealersCode": $("#SDealersCode").textbox('getText'),       //经销商代码
        "SCarType": $("#SCarType").textbox('getText'),               //车型
        "SCarLicence": $("#SCarLicence").textbox('getText')          //车牌号
    };
    BaseGetData(mydata, setData);
}

function setData(resdata) {
    if (resdata.state == 100){
        if (resdata.result != null && resdata.result != "") {
            InitGridColumns(resdata.result.records[0]); //初始化datagrid列
            var alldataInfo = resdata.result.records[1];
            if (alldataInfo.length == 0) {
                $.messager.alert("提示信息", "未检索到数据！");
            }
            $('#data_grid').datagrid('loadData', alldataInfo);

            var date = $('#STime').textbox('getText').toString();
            $("#dateDetail1").attr("value", date + "年统计");
            var myDate = new Date();
            var et = myDate.format("yyyy-MM-dd hh:mm:ss");
            $("#dateDetail2").attr("value", et.toString().substring(0, 10));
        }

    } else {
    if (resdata.state == 104) {
        LoginTimeout('服务器超时！');
    }
    else {
        $.messager.alert('错误信息', resdata.msg.toString(), 'error');
        $('#data_grid').datagrid('loadData', { total: 0, rows: [] });

        var date = $('#STime').textbox('getText').toString();
        $("#dateDetail1").attr("value", date + "年统计");
        var myDate = new Date();
        var et = myDate.format("yyyy-MM-dd hh:mm:ss");
        $("#dateDetail2").attr("value", et.toString().substring(0, 10));
    }
            
    }
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
                s = s + "{field:'" + dataarr[i].field + "',title:'" + dataarr[i].title + "'},"
            }
            s = s.substr(0, s.length - 1)
        }

        s = s + "]]";
        options = {};
        options.columns = eval(s);
        $('#data_grid').datagrid(options);
    }
}

function setSSalesProvince(val) {
    var mydata = {
        "sid": "DayTestDriveAnalyse-getSDealersName",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "Type": "Province",
        "Area": val,
        "Province": ""
    };
    BaseGetData(mydata, setProvince);
    
    setSDealersName("");
}
function setProvince(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            var ProvinceArray = obj.result.records;
            var ProvinceData = [{
                "val": "",
                "text": "全部",
                "selected": true
            }];
            for (var i = 0; i < ProvinceArray.length; i++) {
                ProvinceData.push({
                    "val": ProvinceArray[i]["PROVINCE"],
                    "text": ProvinceArray[i]["PROVINCE"]
                });
            }
            $('#SSalesProvince').combobox('loadData', ProvinceData);
        }
    }
}


function setSDealersName(val) {
    var mydata = {
        "sid": "DayTestDriveAnalyse-getSDealersName",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "Type": "DealerName",
        "Area": $('#SSalesRegion').combobox('getValue').trim(),
        "Province": val
    };
    BaseGetData(mydata, setDealerName);
}

function setDealerName(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            var DealerNameArray = obj.result.records;
            var DealerNameData = [{
                "DealerCode": "",
                "DealerName": "全部",
                "selected": true
            }];
            for (var i = 0; i < DealerNameArray.length; i++) {
                DealerNameData.push({
                    "DealerCode": DealerNameArray[i]["NAME"],
                    "DealerName": DealerNameArray[i]["NAME"]
                });
            }
            $('#SDealersName').combobox('loadData', DealerNameData);
        }
    }
}


//数据导出
function Export() {
    var mydata = {
        "sid": "DayTestDriveAnalyse-export",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "STime": $('#STime').datebox('getValue'),       //查询年月
        "SSalesRegion": $('#SSalesRegion').combobox('getValue').trim(),        //销售大区
        "SSalesProvince": $('#SSalesProvince').combobox('getValue').trim(),    //经销商名称
        "SDealersName": $('#SDealersName').combobox('getValue').trim(),        //经销商名称
        "SDealersCode": $("#SDealersCode").textbox('getText'),       //经销商代码
        "SCarType": $("#SCarType").textbox('getText'),               //车型
        "SCarLicence": $("#SCarLicence").textbox('getText')          //车牌号
    };
    ExcelExport(mydata);
}