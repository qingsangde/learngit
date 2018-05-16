var selCID;
var AllPlaceLevel = new Array();
var FirstLevel = new Array();
var SecondLevel = new Array();
var ThirdLevel = new Array();
var UserCookie;
$(function () {
    UserCookie = GetUserInfo();
    var mydata = {
        "sid": "car-placelevel-get",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString()
    };
    BaseGetData(mydata, PlaceLevelInit);
});



function PlaceLevelInit(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            AllPlaceLevel = obj.result.records;
            FirstLevel = $.extend(true, [], AllPlaceLevel);
            FirstLevel = getfilter(FirstLevel, "administrativeLevel", "1");
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('服务器超时！');
            }
            else {
                $.messager.alert('错误信息', obj.msg, 'error');
            }
        }
    }
}


/**
* 从对象数组中获取属性为objPropery，值为objValue元素的对象
* @param Array arrCar  数组对象
* @param String objPropery  对象的属性
* @param String objPropery  对象的值
* @return Array 过滤后的数组
*/
function getfilter(arrCar, objPropery, objValue) {
    return $.grep(arrCar, function (cur, i) {
        return (cur[objPropery].toString() == objValue.toString())
    });
}

function getfilterFuzzy(arrCar, objPropery, objValue) {
    return $.grep(arrCar, function (cur, i) {
        return (cur[objPropery].toString().indexOf(objValue.toString()) == 0)
    });
}



function DoQuery() {
    var carno = $("#txtCarNo").val();
    var cid = $("#txtCID").val();
    var cuid = $("#txtCUID").val();
    var carownname = $("#txtCarOwnName").val();
    var line = $("#txtLine").val();
    var uid = UserCookie["UID"]


    var mydata = {
        "sid": "car-list-search",
        "sysuid": uid,
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "carno": carno,
        "cid": cid,
        "cuid": cuid,
        "carownname": carownname,
        "line": line
    };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}


function DoEditInit() {

    var selRow = $('#data_grid').datagrid('getSelected');
    if (selRow) {
        selCID = selRow["Cid"];
        var mydata = {
            "sid": "car-one-search",
            "sysuid": UserCookie["UID"].toString(),
            "token": UserCookie["token"].toString(),
            "sysflag": UserCookie["sysflag"].toString(),
            "cid": selCID
        };
        BaseGetData(mydata, EditInitRes);
    }
    else {
        $.messager.alert('错误信息', '请选择要编辑的车辆！', 'error');
    }
}

function EditInitRes(obj) {
    if (obj.state == 100) {
        $('#wCarEdit').window('open');
        var cartypetbl = obj.result.records[0];  //1车辆类型 [CTID],[CTNAME],CHECKED
        var cuidtbl = obj.result.records[1];    //2车辆用途 [CUID]
        var cargrouptbl = obj.result.records[2];    //3车辆组号[GROUPID],[GROUPDESC], CHECKED
        var energytypetbl = obj.result.records[3];  //4发动机类型 [PKey],[EnergyTypeName], CHECKED
        var servertbl = obj.result.records[4];      //5服务名称[SERV_ID],[SEVER_NAME], CHECKED
        var regiontbl = obj.result.records[5];      //6地区 [RE_ID],[REGIONNAME], CHECKED
        var cartbl = obj.result.records[6];         //7要修改的车辆信息

        if (cartbl.length > 0) {
            loadData_Edit_EditCar(cartbl[0]);
            loadData_Edit_Nationality(cartbl[0].CarNationality);
        }

        if (cartypetbl.length > 0) {
            loadData_Edit_CarTypeList(cartypetbl, cartbl[0].CTID);
        }
        if (cuidtbl.length > 0) {
            loadData_Edit_CarCUID(cuidtbl, cartbl[0].CUID);
        }
        if (cargrouptbl.length > 0) {
            loadData_Edit_Group(cargrouptbl, cartbl[0].CGroup);
        }
        if (energytypetbl.length > 0) {
            loadData_Edit_EnergyType(energytypetbl, cartbl[0].EnergyTypePKey);
        }

    }
    else {
        if (obj.state == 104) {
            LoginTimeout('服务器超时！');
        }
        else {
            $.messager.alert('错误信息', obj.msg, 'error');
        }
    }
}

//初始化车辆信息
function loadData_Edit_EditCar(cardata) {
    $('#txtCarNoE').val(cardata.CarNo); //车牌号
    if (cardata.LicensePlateColor != null && cardata.LicensePlateColor != "") {
        $('#ddlLicensePlateColorE').combobox('setValue', cardata.LicensePlateColor);
    } //车牌颜色
    $('#txtCarNoRemarkE').val(cardata.CarNoRemark); //车牌号备注
    $('#txtFdjhE').val(cardata.FDJH);   //发动机号
    $('#txtDphE').val(cardata.DPH);     //底盘号

    if (cardata.CarColor != null && cardata.CarColor != "") {
        $('#ddlCarColorE').combobox('setValue', cardata.CarColor);
    }  //车辆颜色

    $('#txtvehicleNationalityE').val(cardata.VehicleNationality); //车籍地编码
    $("#txtvehicleNationalityE").attr("readonly", true);
    $('#txttransTypeE').val(cardata.TransType); //行业编码
    $('#txtvehicleTypeE').val(cardata.VehicleType); // 809车型

    //司机信息
    $('#txtCarDNameE').val(cardata.CarDName); // 司机姓名
    $('#txtCarDNoE').val(cardata.CarDNo); // 驾驶证号
    $('#txtCarDbzE').val(cardata.CarDBZ); // 司机备注
    $('#txtCarDaE').val(cardata.CarDA); // 司机住址
    $('#txtCarDtE').val(cardata.CarDT); // 司机电话

    //车主信息
    $('#txtCarOwnNameE').val(cardata.CarOwnName); // 车主名称
    $('#txtCarOwnAddE').val(cardata.CarOwnAdd); // 车主地址
    $('#txtCarOwnTelE').val(cardata.CarOwnTel); // 车主电话
    $('#txtCarOwnPasE').val(cardata.CarOwnPas); // 车主密码
    $('#txtowersIDE').val(cardata.OwersID); //许可证号

}

//初始化车型
function loadData_Edit_CarTypeList(cartypetbl, CTID) {
    //ddlCTNameE
    if (cartypetbl.length > 0) {
        $('#ddlCTNameE').combobox('loadData', cartypetbl);
        if (CTID != null && CTID != "") {
            $('#ddlCTNameE').combobox('setValue', CTID);
        }
    }
}

//初始化车辆用途
function loadData_Edit_CarCUID(cuidtbl, CUID) {
    //ddlCuidE
    if (cuidtbl.length > 0) {
        $('#ddlCuidE').combobox('loadData', cuidtbl);
        if (CUID != null && CUID != "") {
            $('#ddlCuidE').combobox('setValue', CUID);
        }
    }
}

//初始化车辆组
function loadData_Edit_Group(cargrouptbl, CGroup) {
    //ddlCGroupE
    if (cargrouptbl.length > 0) {
        $('#ddlCGroupE').combobox('loadData', cargrouptbl);
        if (CGroup != null && CGroup != "") {
            $('#ddlCGroupE').combobox('setValue', CGroup);
        }
    }
}

//初始化发动机类型
function loadData_Edit_EnergyType(energytypetbl, EnergyTypePKey) {
    if (energytypetbl.length > 0) {
        $('#ddlEnergyTypeE').combobox('loadData', energytypetbl);
        if (EnergyTypePKey != null && EnergyTypePKey != "") {
            $('#ddlEnergyTypeE').combobox('setValue', EnergyTypePKey);
        }
    }
}


//初始化车籍地
function loadData_Edit_Nationality(nationnalityvalue) {
    InitDropDownEvent();
    $('#ddlLevel_First').combobox('loadData', FirstLevel);
    if (nationnalityvalue != null && nationnalityvalue != "") {
        var first = nationnalityvalue.toString().substr(0, 2) + "0000";
        $('#ddlLevel_First').combobox('setValue', nationnalityvalue.toString().substr(0, 2) + "0000");

        if (first == "110000" || first == "120000" || first == "310000" || first == "500000") { //直辖市
            $("#ddlLevel_Second_Div").hide();
            ThirdLevel = $.extend(true, [], AllPlaceLevel);
            ThirdLevel = getfilter(ThirdLevel, "administrativeLevel", "3");
            ThirdLevel = getfilterFuzzy(ThirdLevel, "numericCode", nationnalityvalue.toString().substr(0, 3));
            $('#ddlLevel_Third').combobox('loadData', ThirdLevel);
            $('#ddlLevel_Third').combobox('setValue', nationnalityvalue.toString());
        }
        else {
            $("#ddlLevel_Second_Div").show();
            SecondLevel = $.extend(true, [], AllPlaceLevel);
            SecondLevel = getfilter(SecondLevel, "administrativeLevel", "2");
            SecondLevel = getfilterFuzzy(SecondLevel, "numericCode", nationnalityvalue.toString().substr(0, 2));
            $('#ddlLevel_Second').combobox('loadData', SecondLevel);
            $('#ddlLevel_Second').combobox('setValue', nationnalityvalue.toString().substr(0, 4) + "00");


            ThirdLevel = $.extend(true, [], AllPlaceLevel);
            ThirdLevel = getfilter(ThirdLevel, "administrativeLevel", "3");
            ThirdLevel = getfilterFuzzy(ThirdLevel, "numericCode", nationnalityvalue.toString().substr(0, 4));
            $('#ddlLevel_Third').combobox('loadData', ThirdLevel);
            $('#ddlLevel_Third').combobox('setValue', nationnalityvalue.toString());
        }
    }
}

//车籍地联动事件
function InitDropDownEvent() {
    $("#ddlLevel_First").combobox({
        onChange: function (newvalue, oldvalue) {
            if (oldvalue != null && oldvalue != "") {
                if (newvalue == "110000" || newvalue == "120000" || newvalue == "310000" || newvalue == "500000") { //直辖市
                    $("#ddlLevel_Second_Div").hide();
                    $('#ddlLevel_Third').combobox('clear');
                    ThirdLevel = $.extend(true, [], AllPlaceLevel);
                    ThirdLevel = getfilter(ThirdLevel, "administrativeLevel", "3");
                    ThirdLevel = getfilterFuzzy(ThirdLevel, "numericCode", newvalue.toString().substr(0, 3));
                    if (ThirdLevel.length > 0) {
                        $('#ddlLevel_Third').combobox('loadData', ThirdLevel);
                        $('#ddlLevel_Third').combobox('setValue', ThirdLevel[0].numericCode);
                        $('#txtvehicleNationalityE').val(ThirdLevel[0].numericCode);
                    }
                }
                else {
                    $("#ddlLevel_Second_Div").show();
                    $('#ddlLevel_Second').combobox('clear');
                    SecondLevel = $.extend(true, [], AllPlaceLevel);
                    SecondLevel = getfilter(SecondLevel, "administrativeLevel", "2");
                    SecondLevel = getfilterFuzzy(SecondLevel, "numericCode", newvalue.toString().substr(0, 2));
                    if (SecondLevel.length > 0) {
                        $('#ddlLevel_Second').combobox('loadData', SecondLevel);
                        $('#ddlLevel_Second').combobox('setValue', SecondLevel[0].numericCode);
                    }

                    $('#ddlLevel_Third').combobox('clear');
                    ThirdLevel = $.extend(true, [], AllPlaceLevel);
                    ThirdLevel = getfilter(ThirdLevel, "administrativeLevel", "3");
                    ThirdLevel = getfilterFuzzy(ThirdLevel, "numericCode", SecondLevel[0].numericCode.toString().substr(0, 4));
                    if (ThirdLevel.length > 0) {
                        $('#ddlLevel_Third').combobox('loadData', ThirdLevel);
                        $('#ddlLevel_Third').combobox('setValue', ThirdLevel[0].numericCode);
                        $('#txtvehicleNationalityE').val(ThirdLevel[0].numericCode);
                    }
                }
            }
        }
    });
    $("#ddlLevel_Second").combobox({
        onChange: function (newvalue, oldvalue) {
            if (oldvalue != null && oldvalue != "") {
                $('#ddlLevel_Third').combobox('clear');
                ThirdLevel = $.extend(true, [], AllPlaceLevel);
                ThirdLevel = getfilter(ThirdLevel, "administrativeLevel", "3");
                ThirdLevel = getfilterFuzzy(ThirdLevel, "numericCode", newvalue.toString().substr(0, 4));
                $('#ddlLevel_Third').combobox('loadData', ThirdLevel);
                $('#ddlLevel_Third').combobox('setValue', ThirdLevel[0].numericCode);
                $('#txtvehicleNationalityE').val(ThirdLevel[0].numericCode);
            }
        }
    });


    $("#ddlLevel_Third").combobox({
        onSelect: function (record) {
            $('#txtvehicleNationalityE').val(record.numericCode);
        }
    });
}


function DoSaveEdit() {
    var sysflag = UserCookie["sysflag"].toString().trim();     //数据库标识
    var cid = selCID;                                   //车辆CID
    var carno = $('#txtCarNoE').val().trim();
    var licenseplatecolor = $('#ddlLicensePlateColorE').combobox('getValue');
    var carnoremark = $('#txtCarNoRemarkE').val().trim();
    var fdjh = $('#txtFdjhE').val().trim();
    var dph = $('#txtDphE').val().trim();
    var energytypepkey = $('#ddlEnergyTypeE').combobox('getValue');
    var ctid = $('#ddlCTNameE').combobox('getValue');
    var cuid = $('#ddlCuidE').combobox('getValue');
    var carcolor = $('#ddlCarColorE').combobox('getValue');
    var cgroup = $('#ddlCGroupE').combobox('getValue');
    var carnationality = $('#txtvehicleNationalityE').val().trim();
    var vehiclenationality = $('#txtvehicleNationalityE').val().trim();
    var transtype = $('#txttransTypeE').val().trim();
    var vehicletype = $('#txtvehicleTypeE').val().trim();
    var cardname = $('#txtCarDNameE').val().trim();
    var cardno = $('#txtCarDNoE').val().trim();
    var cardbz = $('#txtCarDbzE').val().trim();
    var carda = $('#txtCarDaE').val().trim();
    var cardt = $('#txtCarDtE').val().trim();
    var carownname = $('#txtCarOwnNameE').val().trim();
    var carownadd = $('#txtCarOwnAddE').val().trim();
    var carowntel = $('#txtCarOwnTelE').val().trim();
    var carownpas = $('#txtCarOwnPasE').val().trim();
    var owersid = $('#txtowersIDE').val().trim();
    var sysuid = UserCookie["UID"];

    var mydata = {
        "sid": "car-edit-save",
        "sysflag": sysflag,
        "token": UserCookie["token"].toString(),
        "cid": cid,
        "carno": carno,
        "licenseplatecolor": licenseplatecolor,
        "carnoremark": carnoremark,
        "fdjh": fdjh,
        "dph": dph,
        "energytypepkey": energytypepkey,
        "ctid": ctid,
        "cuid": cuid,
        "carcolor": carcolor,
        "cgroup": cgroup,
        "carnationality": carnationality,
        "vehiclenationality": vehiclenationality,
        "transtype": transtype,
        "vehicletype": vehicletype,
        "cardname": cardname,
        "cardno": cardno,
        "cardbz": cardbz,
        "carda": carda,
        "cardt": cardt,
        "carownname": carownname,
        "carownadd": carownadd,
        "carowntel": carowntel,
        "carownpas": carownpas,
        "owersid": owersid,
        "sysuid": sysuid
    };

    BaseGetData(mydata, EditSaveRes);
}

function EditSaveRes(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            $.messager.alert('提示信息', obj.msg, 'info');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('服务器超时！');
            }
            else {
                $.messager.alert('错误信息', obj.msg, 'error');
            }
        }
    }
}