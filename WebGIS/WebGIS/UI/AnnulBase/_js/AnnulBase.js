var UserCookie;
var operatetype = "New";
var EditCID = 0;
var userrole; //用户角色
var R_JXS = "25";
var R_QM = "26";
//--------------------导出用------------------------
var exportcarno;
var exportcarvin;
var exportsimCode;
var exporttno;
var exportloanOrg;
var exportstatus;
var exportrepaySDate;
var exportrepayEDate;
var exportserviceSDate;
var exportserviceEDate;
//--------------------导出用------------------------

$(function () {
    UserCookie = GetUserInfo();
    GetAllCmb();
    setPage();
});

$.extend($.fn.validatebox.defaults.rules, {
    //验证开始时间小于结束时间
    md: {
        validator: function (value, param) {
            startTime = $(param[0]).datetimebox('getValue');
            var d1 = $.fn.datebox.defaults.parser(startTime);
            var d2 = $.fn.datebox.defaults.parser(value);
            varify = (startTime == "") || d2 >= d1;
            return varify;
        },
        message: '结束时间必须大于开始时间！'
    }
})

function setPage() {
    //画面制御
    if (UserCookie.RID == R_JXS) { //经销商.新增按钮显示
        $('#btnAdd').css({ "display": "inline" });
    } else if (UserCookie.RID == R_QM) { //启明.新增按钮不显示
        $('#btnAdd').css({ "display": "none" });
    }
    //设置贷款期数最大长度
    $('#eLendPeriods').numberbox('textbox').attr('maxlength', 2);
    //    LoadDatagrid();
}

//贷款开始日期、贷款期数计算贷款结束日期
function calculateDate(newValue, oldValue) {
    var lenddate = $('#elenddate').datebox('getValue');
    var loanPeriods = $("#eLendPeriods").val();
    if (lenddate == '' || loanPeriods == '' || lenddate.lenth < 1 || loanPeriods.lenth < 1) {
        return;
    } else {
        //取得月份
        var date = $.fn.datebox.defaults.parser(lenddate);
        var newdate = $.fn.datebox.defaults.formatter(date);
        var result = addMoth(newdate, parseInt(loanPeriods));
        $('#elendEnddate').datebox('setValue', result);
    }
}

//计算日期
function addMoth(d, m) {
    var ds = d.split('-'), _d = ds[2] - 0;
    var nextM = new Date(ds[0], ds[1] - 1 + m + 1, 0);
    var max = nextM.getDate();
    d = new Date(ds[0], ds[1] - 1 + m, _d > max ? max : _d);
    return d.toLocaleDateString().match(/\d+/g).join('-')
}


//贷款期数和贷款开始日期改变事件
function ConditionChange(newValue, oldValue) {
    calculateDate(newValue, oldValue);
}


////sim卡改变事件
var value;
function SimTextChange(newValue,oldValue) {
    console.log(newValue);
    if (newValue != undefined) {
        value = newValue;
        var mydata = {
            "sid": "annulbase-getSimCombo",
            "sysuid": UserCookie["UID"],
            "token": UserCookie["token"],
            "simcode": newValue,
            "sysflag": UserCookie["sysflag"]
        };
        BaseGetData(mydata, SetSimCmb);
    }
}

//生成sim卡下拉框
function SetSimCmb(obj) {
    if (obj.state == 100) {
        setTimeout(SetSimText(obj),  1000);
    }
}

//赋值
function SetSimText(obj) {
    var cmbSim = obj.result.records; //sim卡下拉框
    $('#simcode').combobox({ data: cmbSim, valueField: "SimCode", textField: "SimCode" });
    $('#simcode').combobox().next('span').find('input').focus();
    $("#simcode").combobox('setText', value);
 }

////sim卡下拉框
//function SimInit(obj) {
//    $("#SimPanel").html("");
//    var option;
//    var options;
//    $.each(obj.result.records, function (index, value) {
//        option = $("<span>" + value.SimCode + "</span><br/>");
//        $("#SimPanel").append(option);
//    });
//    // 设置【sim卡号】下拉框
//    $("#SimPanel").appendTo($("#simcode").combobox("panel"));
//    $('#simcode').combobox().next('span').find('input').focus();
//    $('#simcode').combobox('setText', value);

//}


$.fn.datebox.defaults.formatter = function (date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + m + '-' + d;
}

//可定制列
function DispalySettingInit(obj) {
    var data = [
                { key: "CarNo", value: "车牌号" },
                { key: "DPH", value: "VIN" },
                { key: "SimCode", value: "SIM卡号" },
                { key: "TNO", value: "终端ID" },
                { key: "ServiceEDay", value: "服务有效期止日" },
                { key: "RepaymentDateTemp", value: "到期还款日" },
                { key: "PaymentAccount", value: "到期还款金额" },
                { key: "PayStatus", value: "还款状态" },
                { key: "Latedays", value: "逾期天数" },
                { key: "LockStatusName", value: "锁车状态" },
                ];
    var option;
    var options;
    $.each(data, function (index, obj) {
        option = $("<input type='checkbox' name='DispalySettingCheckbox' value='" + obj.key + "' checked/><span>" + obj.value + "</span><br/>");
        $("#DispalySettingPanel").append(option);
    });
    // 冻结车牌号列
    $("[value='CarNo']").prop("disabled", true);
    // 设置【显示设置】下拉框
    $("#DispalySettingPanel").appendTo($("#DispalySetting").combobox("panel"));
    $('#DispalySetting').combobox('setText', "显示设置");
    $("#DispalySettingPanel input").click(function () {
        if ($(this).prop("checked")) {
            $("#data_grid").datagrid("showColumn", $(this).val());
        } else {
            $("#data_grid").datagrid("hideColumn", $(this).val());
        }

    });
}

//点击查询按钮 查询事件
function DoQuery() {
    // 校验
    if (!$("#txtRepayEDate").datebox("isValid") || !$("#txtServiceEDate").datebox("isValid")) {
        return null;
    }
    LoadDatagrid();
}

//查询方法
function LoadDatagrid() {
    //取得查询条件
    var carno = $("#txtCarNo").val(); //车牌号
    var carvin = $("#txtVIN").val(); //车辆VIN
    var simCode = $("#txtSimCode").val(); //sim卡号
    var tno = $("#txtTNO").val(); //终端ID
    var loanOrg = $('#cmbLoanOrg').combobox('getValue'); //放款机构code
    var status = $('#cmbStatus').combobox('getValue'); //锁车状态code
    var repaySDate = $('#txtRepaySDate').datebox('getValue'); //到期还款日开始日期
    var repayEDate = $('#txtRepayEDate').datebox('getValue'); //到期还款日结束日期
    var serviceSDate = $('#txtServiceSDate').datebox('getValue'); //服务有效期止日开始日期
    var serviceEDate = $('#txtServiceEDate').datebox('getValue'); //服务有效期止日结束日期

    //------------------导出用------------------
    exportcarno = carno;
    exportcarvin = carvin;
    exportsimCode = simCode;
    exporttno = tno;
    exportloanOrg = loanOrg;
    exportstatus = status;
    exportrepaySDate = repaySDate;
    exportrepayEDate = repayEDate;
    exportserviceSDate = serviceSDate;
    exportserviceEDate = serviceEDate;
    //------------------导出用------------------

    var uid = UserCookie["UID"];
    var mydata = {
        "sid": "annulBase-list-search",
        "sysuid": uid,
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"],
        "userrole": UserCookie.RID,
        "carno": carno,
        "carvin": carvin,
        "simCode": simCode,
        "tno": tno,
        "loanOrg": loanOrg,
        "status": status,
        "repaySDate": repaySDate,
        "repayEDate": repayEDate,
        "serviceSDate": serviceSDate,
        "serviceEDate": serviceEDate
    };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//页面初始化生成下拉框
function GetAllCmb() {
    //显示设置下拉框
    DispalySettingInit();
    var mydata = {
        "sid": "annulBase-getCombo",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"]
    };
    BaseGetData(mydata, SetAllCmb);

}

function SetSelectOrgCmb() {
    var initData = {
        "sid": "al-org-get",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString()
    };
    BaseGetData(initData, ConditionSelectOrgNoInit,false);
}

function SetEditOrgCmb() {
    var initData = {
        "sid": "al-org-get",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString()
    };
    BaseGetData(initData, ConditionEditOrgNoInit, false);
}

function ConditionCmbInit(obj, cmbSelector, valueField, textField) {
    if (obj != null) {
        if (obj.state == 100) {
            //查询条件放款机构
            if (cmbSelector == "#cmbLoanOrg") {
                var whiteOrg = { "OrgNo": "", "OrgName": "-请选择-" };
                obj.result.records.unshift(whiteOrg);
            }
            $(cmbSelector).combobox({ data: obj.result.records, valueField: valueField, textField: textField });
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

//查询条件放款机构下拉框
function ConditionEditOrgNoInit(obj) {
    ConditionCmbInit(obj, "#eOrgNo", "OrgNo", "OrgName");
}

//编辑页面放款机构下拉框
function ConditionSelectOrgNoInit(obj) {
    ConditionCmbInit(obj, "#cmbLoanOrg", "OrgNo", "OrgName");
}



//取得下拉框回调生成下拉框
function SetAllCmb(obj) {
    if (obj.state == 100) {
        var cmbOrg = obj.result.records[0]; //查询条件放款机构下拉框
        var cmbLock = obj.result.records[1]; //锁车状态下拉框
        var cmbManufacturer = obj.result.records[2]; //生产厂家
        var cmbCustomer = obj.result.records[3]; //客户类型下拉框
        //声明空白项
        var whiteOrg = { "OrgNo": "", "OrgName": "-请选择-" };
        var whiteLockStatus = { "LockCode": "", "LockName": "-请选择-" };
        cmbOrg.unshift(whiteOrg);
        cmbLock.unshift(whiteLockStatus);
        $('#cmbLoanOrg').combobox({ data: cmbOrg, valueField: "OrgNo", textField: "OrgName" }); //查询条件放款机构
        $('#cmbStatus').combobox('loadData', cmbLock); //锁车状态
        $('#eManufacturer').combobox('loadData', cmbManufacturer); //生产厂家
        $('#eCustomerType').combobox('loadData', cmbCustomer); //客户类型
    }
}

//点击新建按钮
function openAdd() {

    $("#lblMessage").html("基础信息新增成功");
    operatetype == "New";
    $('#baseInfoEdit').window({
        title: '基础信息新建',
        iconCls: 'icon-add'
    });

    $('#baseInfoEdit').window('open');

    //画面制御
    //SIM卡号下拉框制御
    $("#simcode").combobox("clear").combobox("enable");

    $('#Div3').css({ "display": "inline" });
    //保存按钮不可用
    $("#btnSave").attr("disabled", true);
    $("#btnSave").removeClass("btnGreen");
    $("#btnSave").addClass("btnBlue");

    $("#simcode").combobox("clear");
    //车辆信息
    $("#eCarNo").textbox("clear").textbox("disable");
    $("#eCarVin").html("");
    $("#eCarType").html("");
    $("#eCarEngine").html("");
    $("#eCarEngineType").html("");
    $("#eCarOwnName").html("");
    $("#eCarOwnTel").html("");
    $("#eManufacturer").combobox("clear").combobox("disable");
    //客户信息
    $("#eCustomerType").combobox("clear").combobox("disable");
    $("#eCustomerName").textbox("clear").textbox("disable");
    $("#eLinkman").textbox("clear").textbox("disable");
    $("#ePhone").textbox("clear").textbox("disable");
    $("#eAddress").textbox("clear").textbox("disable");
    //贷款信息
    $("#eLoanContractNo").textbox("clear").textbox("disable");
    $("#eOrgNo").combobox("clear").combobox("disable");
    $("#elenddate").datebox("clear").datebox("disable");
    $("#elendEnddate").datebox("clear").datebox("disable");
    $("#eLendPeriods").textbox("clear").textbox("disable");
    $("#ePaymentAccount").numberbox("clear").numberbox("disable");
    $("#ePaymentDueDay").datebox("clear").datebox("disable");
    //服务有效期
    $("#eServiceSDay").datebox("clear").datebox("disable");
    $("#eServiceEDay").datebox("clear").datebox("disable");
}

//输入sim卡号点击确认按钮
function DoSure() {
    if (!$("#simcode").textbox("isValid")) {
        $.messager.alert('提示信息', '请输入SIM卡号！');
        return null;
    }
    //根据SIM卡号查询数据
    var simcode = $("#simcode").combobox('getText');
    var mydata = {
        "sid": "annulBase-sim-search",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"],
        "simcode": simcode
    };
    BaseGetData(mydata, EditSimRes);
}

//查询sim卡号后回调赋值
function EditSimRes(obj) {
    if (obj.state == 100) {
        //返回一行的情况
        if (obj.result.total == 1) {
            SetEditOrgCmb();

            //画面制御
            //SIM卡号下拉框制御
            $("#simcode").combobox("disable");
            //保存按钮可用
            $("#btnSave").attr("disabled", false);
            $("#btnSave").removeAttr("disabled");
            $("#btnSave").removeClass("btnBlue");
            $("#btnSave").addClass("btnGreen");
            //车辆信息
            $("#eCarNo").textbox("enable");
            $("#eManufacturer").combobox("enable");
            //客户信息
            $("#eCustomerType").combobox("enable");
            $("#eCustomerName").textbox("enable");
            $("#eLinkman").textbox("enable");
            $("#ePhone").textbox("enable");
            $("#eAddress").textbox("enable");
            //贷款信息
            $("#eLoanContractNo").textbox("enable");
            $("#eOrgNo").combobox("enable");
            $("#elenddate").datebox("enable");
            $("#elendEnddate").datebox("enable");
            $("#eLendPeriods").textbox("enable");
            $("#ePaymentAccount").numberbox("enable");
            $("#ePaymentDueDay").datebox("enable");
            //服务有效期
            $("#eServiceSDay").datebox("disable");
            $("#eServiceEDay").datebox("disable");

            var carMessage = obj.result.records[0][0];  //1车辆信息

            //车辆信息赋值
            $("#eCID").textbox("setValue", carMessage.CID); //车牌ID隐藏项
            $("#eCarNo").textbox("setValue", carMessage.CarNo); //车牌号码
            $("#eCarVin").html(carMessage.DPH); //车架VIN码
            //$("#eManufacturer").combobox("setValue", carMessage.Manufacturer); //生产厂家
            $('#eCarType').html(carMessage.CTNAME); //车型名
            $('#eCarEngine').html(carMessage.FDJH); //发动机号
            $('#eCarEngineType').html(carMessage.EnergyTypeName); //引擎类型名
            $('#eCarOwnName').html(carMessage.CarOwnName); //车主姓名
            $('#eCarOwnTel').html(carMessage.CarOwnTel); //车主电话
        } else if (obj.result.total == 0) {
            $.messager.alert('提示信息', '请输入正确的SIM卡号');
        } else {
            $.messager.alert('提示信息', '请输入更准确的SIM卡号');
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

//点击取消按钮
function DoCancel() {
    $('#baseInfoEdit').window('close');
}

//点击保存按钮
function DoSave() {
    var uid = UserCookie["UID"];
    //用户角色是经销商,校验必输项
    if (UserCookie.RID == R_JXS) {
        if (!$("#eLendPeriods").textbox("isValid") || !$("#ePaymentAccount").numberbox("isValid") || !$("#eCarNo").textbox("isValid")) {
            $.messager.alert('提示信息', '必输项不能为空！');
            return null;
        }
    } else {
        // 日期校验
        if (!$("#eServiceEDay").datebox("isValid")) {
            return null;
        }
    }

    if ($("#eOrgNo").combobox('getText') == "") {
        $.messager.alert('提示信息', '必输项不能为空！');
        return null;
    }

    var sid;
    var cid;

    //新增
    if (operatetype == "New") {
        sid = "annul-add-save";
        cid = $("#eCID").val().trim(); //车辆CID
    }
    else {
        sid = "annul-edit-save";
        cid = EditCID; //车辆CID
    }

    //保存
    DoSaveMessage(sid, cid);

    //查询页面生成放款机构下拉框
    SetSelectOrgCmb();
    $('#baseInfoEdit').window('close');
}

//保存后确认
function DoAgree() {
    $('#Div1').window('close');
}


//点击修改按钮事件
function openEdit() {
    //-------------------------------
    //用户角色判断
    //画面制御
    //sim卡区域不显示
    $('#Div3').css({ "display": "none" });

    //保存按钮可用
    $("#btnSave").attr("disabled", false);
    $("#btnSave").removeAttr("disabled");
    $("#btnSave").removeClass("btnBlue");
    $("#btnSave").addClass("btnGreen");

    if (UserCookie.RID == R_JXS) { //经销商.车辆信息、客户信息、贷款信息可编辑
        //车辆信息
        $("#eManufacturer").combobox("enable");
        $("#eCarNo").textbox("enable");
        //客户信息
        $("#eCustomerType").combobox("enable");
        $("#eCustomerName").textbox("enable");
        $("#eLinkman").textbox("enable");
        $("#ePhone").textbox("enable");
        $("#eAddress").textbox("enable");
        //贷款信息
        $("#eLoanContractNo").textbox("enable");
        $("#eOrgNo").combobox("enable");
        $("#elenddate").datebox("enable");
        $("#elendEnddate").datebox("enable");
        $("#eLendPeriods").textbox("enable");
        $("#ePaymentAccount").numberbox("enable");
        $("#ePaymentDueDay").datebox("enable");
        //服务有效期
        $("#eServiceSDay").datebox("disable");
        $("#eServiceEDay").datebox("disable");

    } else if (UserCookie.RID == R_QM) { //启明.仅服务有效期可编辑

        //车辆信息
        $("#eManufacturer").combobox("disable");
        $("#eCarNo").textbox("disable");
        //客户信息
        $("#eCustomerType").combobox("disable");
        $("#eCustomerName").textbox("disable");
        $("#eLinkman").textbox("disable");
        $("#ePhone").textbox("disable");
        $("#eAddress").textbox("disable");
        //贷款信息
        $("#eLoanContractNo").textbox("disable");
        $("#eOrgNo").combobox("disable");
        $("#elenddate").datebox("disable");
        $("#elendEnddate").datebox("disable");
        $("#eLendPeriods").textbox("disable");
        $("#ePaymentAccount").numberbox("disable");
        $("#ePaymentDueDay").datebox("disable");
        //服务有效期
        $("#eServiceSDay").datebox("enable");
        $("#eServiceEDay").datebox("enable");
    }
    //-------------------------------

    $("#lblMessage").html("基础信息修改成功");
    var rows = $('#data_grid').datagrid('getSelections');
    if (rows.length == 1) {
        operatetype = "Edit";
        EditCID = rows[0].CID;
        //弹出修改页面
        var mydata = {
            "sid": "annulBase-one-search",
            "sysuid": UserCookie["UID"],
            "token": UserCookie["token"],
            "sysflag": UserCookie["sysflag"],
            "cid": EditCID
        };
        BaseGetData(mydata, EditInitRes);
    }
    else {
        $.messager.alert('错误信息', '请选择一行！', 'error');
    }
}

//修改页面初始化
function EditInitRes(obj) {
    if (obj.state == 100) {
        SetEditOrgCmb();
                $('#baseInfoEdit').window({
            title: '基础信息修改',
            iconCls: 'icon-edit'
        });
        $('#baseInfoEdit').window('open');

        var qscarMessage = obj.result.records[0][0];  //车辆信息
        var alcarMessage = obj.result.records[1][0];    //车辆补充信息
        var loanMessage = obj.result.records[2][0];    //贷款信息

        //页面赋值
        $("#eCarNo").textbox("setValue", qscarMessage.CarNo);
        $("#eCarVin").html(qscarMessage.DPH);
        $("#eManufacturer").combobox("setValue", alcarMessage.Manufacturer);
        $('#eCarType').html(qscarMessage.CTNAME);
        $("#eCarEngine").html(qscarMessage.FDJH);
        $('#eCarEngineType').html(qscarMessage.EnergyTypeName);
        $('#eCustomerType').combobox('setValue', alcarMessage.CustomerType);
        $("#eCustomerName").textbox("setValue", alcarMessage.CustomerName);
        $("#eLinkman").textbox("setValue", alcarMessage.ContactMan);
        $("#ePhone").textbox("setValue", alcarMessage.ContactNumber);
        $("#eAddress").textbox("setValue", alcarMessage.ContactAddress);
        $("#eCarOwnName").html(qscarMessage.CarOwnName);
        $("#eCarOwnTel").html(qscarMessage.CarOwnTel);
        $("#eLoanContractNo").textbox("setValue", loanMessage.LoanContractNo);
        $('#eOrgNo').combobox('setValue', loanMessage.OrgNo);
        $('#elenddate').datebox('setValue', loanMessage.LendDate);
        $('#elendEnddate').datebox('setValue', loanMessage.LendEndDate);
        $('#eLendPeriods').textbox('setValue', loanMessage.LendPeriods);
        $("#ePaymentAccount").numberbox("setValue", loanMessage.PaymentAccount);
        $("#ePaymentDueDay").datebox("setValue", loanMessage.PaymentDueDay); 
        $('#eServiceSDay').datebox('setValue', alcarMessage.ServiceSDay);
        $('#eServiceEDay').datebox('setValue', alcarMessage.ServiceEDay);
        if (UserCookie.RID == R_JXS) {
            $("#eOrgNo").combobox("enable");
        } else if (UserCookie.RID == R_QM) {
            $("#eOrgNo").combobox("disable");
        } else {
            $("#eOrgNo").combobox("enable");
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

//点击导出按钮
function DoExport() {

    var uid = UserCookie["UID"];
    var mydata = {
        "sid": "annulBase-list-export",
        "sysuid": uid,
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"],
        "userrole": UserCookie.RID,
        "carno": exportcarno,
        "carvin": exportcarvin,
        "simCode": exportsimCode,
        "tno": exporttno,
        "loanOrg": exportloanOrg,
        "status": exportstatus,
        "repaySDate": exportrepaySDate,
        "repayEDate": exportrepayEDate,
        "serviceSDate": exportserviceSDate,
        "serviceEDate": exportserviceEDate
    };
    ExcelExport(mydata);

}

//保存
function DoSaveMessage(sid, cid) {
    var sysflag = UserCookie["sysflag"]; //数据库标识
    var sysuid = UserCookie["UID"];
    //车辆信息
    var manufacturer = $('#eManufacturer').combobox('getValue'); //生产厂家
    var carno = $('#eCarNo').val().trim(); //车牌号
    //客户信息
    var custype = $('#eCustomerType').combobox('getValue'); //客户类型
    var cusname = $("#eCustomerName").val().trim(); //客户名称
    var contactman = $('#eLinkman').val().trim(); //联系人
    var contactnumber = $('#ePhone').val().trim(); //联系电话
    var contactaddress = $('#eAddress').val().trim(); //联系地址
    //贷款信息
    var contractno = $("#eLoanContractNo").val().trim(); //贷款合同号
    var orgno = $('#eOrgNo').combobox('getValue'); //放款机构ID
    var orgnoname = $('#eOrgNo').combobox('getText'); //放款机构名
    var lenddate = $('#elenddate').datebox('getValue'); //贷款开始日期
    var lendEnddate = $('#elendEnddate').datebox('getValue'); //贷款结束日期
    var lendPeriods = $('#eLendPeriods').val().trim(); //贷款期数
    var paymentaccount = $("#ePaymentAccount").val().trim(); //到期还款金额
    var paymentDueDay = $('#ePaymentDueDay').datebox('getValue'); //到期还款日
    //服务有效期
    var servicesday = $('#eServiceSDay').datebox('getValue'); //服务有效期开始日期
    var serviceeday = $('#eServiceEDay').datebox('getValue'); //服务有效期结束日期
    //用户角色

    var mydata = {
        "sid": sid,
        "sysflag": sysflag,
        "token": UserCookie["token"],
        "userrole": UserCookie.RID,
        "cid": cid,
        "carno": carno,
        "manufacturer": manufacturer,
        "customerType": custype,
        "customerName": cusname,
        "contactman": contactman,
        "contactnumber": contactnumber,
        "contactaddress": contactaddress,
        "contractno": contractno,
        "orgno": orgno,
        "orgnoname": orgnoname,
        "lenddate": lenddate,
        "lendenddate": lendEnddate,
        "lendPeriods": lendPeriods,
        "paymentaccount": paymentaccount,
        "paymentDueDay": paymentDueDay,
        "servicesday": servicesday,
        "serviceeday": serviceeday,
        "sysuid": sysuid
    };

    BaseGetData(mydata, EditSaveRes);
}

function EditSaveRes(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            //刷新一览
            LoadDatagrid();
            $('#Div1').window('open');
            //                $.messager.alert('提示信息', obj.msg, 'info');
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