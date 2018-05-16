var UserCookie;
var CID_sf = "";
/*
初始化
*/
$(function () {
    UserCookie = GetUserInfo();
    
    setSSalesProvince("");
    if (UserCookie.RID == "23") {
        $("#tdP").hide()
        $("#SDealersCode").textbox('setText', UserCookie.UName)
        $('#SDealersCode').textbox('disable', true);
    }
});

//查询数据列表
function queryData_Sf() {

    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    UserCookie = GetUserInfo();
    var STime = $('#STime').combobox('getValue').trim();       //查询年月

    var dealercode = $('#SDealersCode').textbox('getText');
    var cartype = $('#SCarType').textbox('getText');
    var carno = $('#SCarLicence').textbox('getText');

    var area = $('#SSalesRegion').combobox('getValue');
    var province = $('#SSalesProvince').combobox('getValue');
    var dealername = $('#SDealersName').combobox('getValue');


    var mydata = { "sid": "MonthTestDriveAnalyse-getlist-new", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),

        "Page": "1", "Size": defultSize, "stime": STime, "area": area, "province": province,

        "dealername": dealername, "dealercode": dealercode, "cartype": cartype, "carno": carno
    };

    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);

}

//查询车辆启动熄火数据列表导出
function Export() {

    $('#data_grid').datagrid('loadData', { total: 0, rows: [] });
    UserCookie = GetUserInfo();
    var STime = $('#STime').combobox('getValue').trim(); 



    var dealercode = $('#SDealersCode').textbox('getText');
    var cartype = $('#SCarType').textbox('getText');
    var carno = $('#SCarLicence').textbox('getText');

    var area = $('#SSalesRegion').combobox('getValue');
    var province = $('#SSalesProvince').combobox('getValue');
    var dealername = $('#SDealersName').combobox('getValue');


    var mydata = { "sid": "MonthTestDriveAnalyse-getlist-new-output", "sysuid": UserCookie["UID"], "token": UserCookie["token"].toString(), "sysflag": UserCookie["sysflag"].toString(),

        "Page": "1", "Size": defultSize, "stime": STime, "area": area, "province": province,

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