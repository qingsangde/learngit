var UserCookie;
var CID_sf = "";
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
    var myDate = new Date();
    var st = myDate.format("yyyy-MM") + "-01 00:00:00";
    var et = myDate.format("yyyy-MM-dd hh:mm:ss");
    $("#STime").datebox('setValue', st);
    $("#ETime").datebox('setValue', et);
    setSSalesProvince("");

});

//查询数据列表
function queryData_Sf() {

    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    UserCookie = GetUserInfo();
    var STime = $('#STime').datebox('getValue');
    var ETime = $('#ETime').datebox('getValue');


    var dealercode = $('#dealercode').textbox('getText');
    var cartype = $('#cartype').textbox('getText');
    var carno = $('#carno').textbox('getText');

    var area = $('#SSalesRegion').combobox('getValue');
    var province = $('#SSalesProvince').combobox('getValue');
    var dealername = $('#SDealersName').combobox('getValue');
    

    var mydata = { "sid": "ComTestDriveAnalyse-getlist", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),

        "Page": "1", "Size": defultSize, "stime": STime, "etime": ETime,"area": area, "province": province,

        "dealername": dealername, "dealercode": dealercode, "cartype": cartype, "carno": carno
    };

    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);

}

//查询车辆启动熄火数据列表导出
function Export() {
    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    UserCookie = GetUserInfo();
    var STime = $('#STime').datebox('getValue');
    var ETime = $('#ETime').datebox('getValue');


    var dealercode = $('#dealercode').textbox('getText');
    var cartype = $('#cartype').textbox('getText');
    var carno = $('#carno').textbox('getText');

    var area = $('#SSalesRegion').combobox('getValue');
    var province = $('#SSalesProvince').combobox('getValue');
    var dealername = $('#SDealersName').combobox('getValue');


    var mydata = { "sid": "ComTestDriveAnalyse-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),

        "Page": "1", "Size": defultSize, "stime": STime, "etime": ETime, "area": area, "province": province,

        "dealername": dealername, "dealercode": dealercode, "cartype": cartype, "carno": carno
    };

    ExcelExport(mydata);
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