var PubUserCookie;
$(function () {
    // 取得用户信息
    PubUserCookie = GetUserInfo();
    // 初始化下拉框
    SetCmb();
});
$.extend($.fn.validatebox.defaults.rules, {
    //验证开始时间小于结束时间
    md: {
        validator: function (value, param) {
            startTime = $(param[0]).datetimebox('getValue');
            var d1 = $.fn.datebox.defaults.parser(startTime);
            var d2 = $.fn.datebox.defaults.parser(value);
            varify = (startTime == '') || d2 > d1;
            return varify;
        },
        message: '结束时间必须大于开始时间！'
    }
})

function SetCmb() {
    var initData = {
        "sid": "al-org-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString()
    };
    BaseGetData(initData, ConditionOrgNoInit, false);
    initData = {
        "sid": "al-dict-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "dictType": "LockStatus"
    };
    BaseGetData(initData, ConditionLockStatusInit, false);
    initData = {
        "sid": "al-dict-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "dictType": "PayStatus"
    };
    BaseGetData(initData, ConditionPayStatusInit, false);
    DispalySettingInit();
}

function ConditionCmbInit(obj, cmbSelector, valueField, textField) {
    if (obj != null) {
        if (obj.state == 100) {
            addEmptyItem(obj.result.records, valueField, textField);
            $(cmbSelector).combobox({ valueField: valueField, textField: textField });
            $(cmbSelector).combobox("loadData", obj.result.records);
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

function ConditionOrgNoInit(obj) {
    ConditionCmbInit(obj, "#ConditionOrgNo", "OrgNo", "OrgName");
}

function ConditionLockStatusInit(obj) {
    ConditionCmbInit(obj, "#ConditionLockStatus", "DictCd", "DictName");
}

function ConditionPayStatusInit(obj) {
    ConditionCmbInit(obj, "#ConditionPayStatus", "DictCd", "DictName");
}

function DispalySettingInit(obj) {
    var data = [
                { key: "CarNo", value: "车牌号" },
                { key: "DPH", value: "VIN" },
                { key: "SimCode", value: "SIM卡号" },
                { key: "TNO", value: "终端ID" },
                { key: "ServiceEDay", value: "服务有效期止日" },
                { key: "RepaymentDateTemp", value: "到期还款日" },
                { key: "RemindDaysTemp", value: "还款到期日提前提醒天数" },
                { key: "DisconTimesTemp", value: "无通讯连接最长连续时间/天" },
                { key: "OverDays", value: "逾期天数" },
                { key: "PayStatus", value: "还款状态" },
                { key: "LockStatus", value: "锁车状态" },
                { key: "Active", value: "激活状态" }
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

function CheckCondition() {
    // 校验
    if (!$("#ConditionLendDateTo").datebox("isValid") || !$("#ConditionServiceEDayTo").datebox("isValid")) {
        return null;
    }
    // 取值
    var ConditionCarNo = $("#ConditionCarNo").val();
    var ConditionDPH = $("#ConditionDPH").val();
    var ConditionSimCode = $("#ConditionSimCode").val();
    var ConditionTNO = $("#ConditionTNO").val();
    var ConditionOrgNo = $("#ConditionOrgNo").combobox("getValue");
    var ConditionLockStatus = $("#ConditionLockStatus").combobox("getText");
    var ConditionLendDateFrom = $("#ConditionLendDateFrom").datebox("getValue");
    var ConditionLendDateTo = $("#ConditionLendDateTo").datebox("getValue");
    var ConditionServiceEDayFrom = $("#ConditionServiceEDayFrom").datebox("getValue");
    var ConditionServiceEDayTo = $("#ConditionServiceEDayTo").datebox("getValue");
    var ConditionLeftDays = $("#ConditionLeftDays").val();
    var ConditionPayStatus = $("#ConditionPayStatus").combobox("getText");
    if ($("#ConditionLockStatus").combobox("getValue") == " ") {
        ConditionLockStatus = "";
    }
    if ($("#ConditionPayStatus").combobox("getValue") == " ") {
        ConditionPayStatus = "";
    }
    var mydata = {
        "sid": "al-parameter-search",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "rid": PubUserCookie["RID"].toString(),
        "ConditionCarNo": ConditionCarNo,
        "ConditionDPH": ConditionDPH,
        "ConditionSimCode": ConditionSimCode,
        "ConditionTNO": ConditionTNO,
        "ConditionOrgNo": ConditionOrgNo,
        "ConditionLockStatus": ConditionLockStatus,
        "ConditionLendDateFrom": ConditionLendDateFrom,
        "ConditionLendDateTo": ConditionLendDateTo,
        "ConditionServiceEDayFrom": ConditionServiceEDayFrom,
        "ConditionServiceEDayTo": ConditionServiceEDayTo,
        "ConditionLeftDays": ConditionLeftDays,
        "ConditionPayStatus": ConditionPayStatus
    };
    return mydata;
}

function DoQuery() {
    var mydata = CheckCondition();
    if (mydata != null) {
        BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
    }
    $("#jxsArea").panel("close");
    $("#qmArea").panel("close");
}

function DoExport() {
    var mydata = CheckCondition();
    if (mydata != null) {
        mydata["sid"] = "al-parameter-export";
        ExcelExport(mydata);
    }
}

function DoParameterSet() {
    // 校验
    if (!$("#RepaymentDateTemp").datebox("isValid") ||
        !$("#RemindDaysTemp").numberbox("isValid") ||
        !$("#DisconTimesTemp").numberbox("isValid")) {
        return;
    }

    var selectedRow = $("#data_grid").datagrid("getSelected");
    var mydata = {
        "sid": "al-parameter-set",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "uid": PubUserCookie["UID"],
        "cid": selectedRow.CID,
        "tno": selectedRow.TNO,
        "datevalue": $("#RepaymentDateTemp").datebox("getValue"),
        "dayvalue": $("#RemindDaysTemp").numberbox("getValue"),
        // 天数转换为秒 传递给webservice接口
        "minutevalue": $("#DisconTimesTemp").numberbox("getValue") * 24 * 60,
        "CarNo": selectedRow.CarNo,
        "OrgNo": selectedRow.OrgNo,
        "Lockstatus": selectedRow.LockstatusCd
    };
    var message = "是否对" + selectedRow.CarNo + "发送参数设置指令";
    $.messager.confirm('确认对话框', message, function (isOk) {
        if (isOk) {
            BaseGetData(mydata, function (resdata) {
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                } else {
                    $.messager.alert("提示信息", resdata.msg.toString());
                }
            });
        }
    });
}

function DoActiveSet() {
    var selectedRow = $("#data_grid").datagrid("getSelected");
    var mydata = {
        "sid": "al-parameter-active",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "uid": PubUserCookie["UID"],
        "cid": selectedRow.CID,
        "tno": selectedRow.TNO,
        "vin": selectedRow.DPH,
        "energytype": selectedRow.ProtocolETPKey,
        "orderType": "",
        "CarNo": selectedRow.CarNo,
        "OrgNo": selectedRow.OrgNo,
        "Lockstatus": selectedRow.LockstatusCd
    };
    var command = "";
    // 选中激活的情况
    if ($("#Active").prop("checked")) {
        // 指令类型设置为0xAA
        mydata["orderType"] = 170;
        command = "激活";
    }
    // 选中关闭的情况
    else if ($("#Close").prop("checked")) {
        // 指令类型设置为0x55
        mydata["orderType"] = 85;
        command = "关闭";
    } else {
        $.messager.alert("错误信息", "请选择一种指令", "error");
    }

    var message = "是否对" + selectedRow.CarNo + "发送" + command + "指令";
    $.messager.confirm('确认对话框', message, function (isOk) {
        if (isOk) {
            BaseGetData(mydata, function (resdata) {
                if (resdata.state == 104) {
                    LoginTimeout('服务器超时！');
                } else {
                    $.messager.alert("提示信息", resdata.msg.toString());
                }
            });
        }
    });
}

function DoOpenWindow() {
    $("#ConditionCarNo").textbox("clear");
    $("#ConditionDPH").textbox("clear");
    $("#ConditionSimCode").textbox("clear");
    $("#ConditionTNO").textbox("clear");
    $("#ConditionOrgNo").combobox("select", " ");
    $("#ConditionLockStatus").combobox("select", " ");
    $("#ConditionLendDateFrom").datebox("clear");
    $("#ConditionLendDateTo").datebox("clear");
    $("#ConditionServiceEDayFrom").datebox("clear");
    $("#ConditionServiceEDayTo").datebox("clear");
    $("#ConditionLeftDays").textbox("clear");
    $("#ConditionPayStatus").combobox("select", " ");
    $("#data_grid").datagrid('loadData', { total: 0, rows: [] });
    $('#carWindow').window('open');
}

function DoCheckCar() {
    var selectedRow = $("#data_grid").datagrid("getSelected");
    if (selectedRow != null) {
        if (PubUserCookie.RID == "26" && selectedRow.AuditStatusCd == "02") {
            $.messager.alert('提示信息', "当前申请已经完成审核！", 'info');
        } else {
            $('#carWindow').window('close');
            $("#datagrid").datagrid("loadData", { total: 1, rows: [selectedRow] });
            CheckRow(null, selectedRow);
        }
    } else {
        $.messager.alert('错误信息', "请先选中一行！", 'error');
    }
}

function CheckRow(index, row) {
    // 隐藏操作区域
    $("#OpearateArea").panel("close");
    $("#jxsArea").panel("close");
    $("#qmArea").panel("close");
    // 清空操作区域
    $("#RepaymentDateTemp").datebox("clear");
    $("#RemindDaysTemp").numberbox("clear");
    $("#DisconTimesTemp").numberbox("clear");
    $("#Active").prop("checked", false);
    $("#Close").prop("checked", false);
    // 经销商权限的情况
    if (PubUserCookie.RID == "25") {
        // 显示经销商操作区域
        $("#OpearateArea").panel("open");
        $("#jxsArea").panel("open");
    }
    // 启明权限的情况
    else if (PubUserCookie.RID == "26") {
        // 显示启明操作区域
        $("#OpearateArea").panel("open");
        $("#qmArea").panel("open");
    }
}

function ConditionCarNoChange(newValue, oldValue) {
    // 用户输入的车牌号不是空的情况
    if (newValue != "") {
        // 清空其他检索条件 并设置状态为不可编辑
        $("#ConditionDPH").textbox("clear").textbox("disable");
        $("#ConditionSimCode").textbox("clear").textbox("disable");
        $("#ConditionTNO").textbox("clear").textbox("disable");
        $("#ConditionOrgNo").combobox("clear").combobox("disable");
        $("#ConditionLockStatus").combobox("clear").combobox("disable");
        $("#ConditionLendDateFrom").datebox("clear").datebox("disable");
        $("#ConditionLendDateTo").datebox("clear").datebox("disable");
        $("#ConditionServiceEDayFrom").datebox("clear").datebox("disable");
        $("#ConditionServiceEDayTo").datebox("clear").datebox("disable");
        $("#ConditionLeftDays").textbox("clear").textbox("disable");
        $("#ConditionPayStatus").combobox("clear").combobox("disable");
    }
    // 用户输入的车牌号为空的情况
    else {
        // 设置其他检索条件的状态为可编辑 
        $("#ConditionDPH").textbox("clear").textbox("enable");
        $("#ConditionSimCode").textbox("clear").textbox("enable");
        $("#ConditionTNO").textbox("clear").textbox("enable");
        $("#ConditionOrgNo").combobox("clear").combobox("enable");
        $("#ConditionLockStatus").combobox("clear").combobox("enable");
        $("#ConditionLendDateFrom").datebox("clear").datebox("enable");
        $("#ConditionLendDateTo").datebox("clear").datebox("enable");
        $("#ConditionServiceEDayFrom").datebox("clear").datebox("enable");
        $("#ConditionServiceEDayTo").datebox("clear").datebox("enable");
        $("#ConditionLeftDays").textbox("clear").textbox("enable");
        $("#ConditionPayStatus").combobox("clear").combobox("enable");
    }
}

function dateFormat(value, row, index) {
    if (value != null && value != "" && value.length >= 10) {
        return value.substr(0, 10);
    } else {
        return value;
    }
}

function addEmptyItem(records, valueField, textField) {
    var strEmptyItem = '{"' + textField + '": "--请选择--","' + valueField + '": " "' + '}';
    var emptyItem = JSON.parse(strEmptyItem); ;
    records.unshift(emptyItem);
}