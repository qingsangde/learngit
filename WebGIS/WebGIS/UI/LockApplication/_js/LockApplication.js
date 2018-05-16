var PubUserCookie;
$(function () {
    // 取得用户信息
    PubUserCookie = GetUserInfo();
    // 初始化下拉框
    SetCmb();
    // 根据角色显示画面项目
    HideByRole();
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

function HideByRole() {
    // 经销商
    if (PubUserCookie.RID == "25") {
        $(".jxs").hide();
        $("#data_grid").datagrid("hideColumn", "AuditStatus");
        $("#data_grid").datagrid("hideColumn", "ApplyLockStatus");
        $("#data_grid").datagrid("hideColumn", "ApplyCompany");
        $("#data_grid").datagrid("hideColumn", "ApplyReason");
        $("#data_grid").datagrid("hideColumn", "ApplyTime");
        $("#data_grid").datagrid("hideColumn", "AuditStatus");
        $("#data_grid").datagrid("hideColumn", "ApplyLockStatus");
        $("#data_grid").datagrid("hideColumn", "ApplyCompany");
        $("#data_grid").datagrid("hideColumn", "ApplyReason");
        $("#data_grid").datagrid("hideColumn", "ApplyTime");
    } 
}

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
    initData = {
        "sid": "al-Tertype-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString()
    };
    BaseGetData(initData, ConditionTertypenumInit, false);
    initData = {
        "sid": "al-EnergyType-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString()
    };
    BaseGetData(initData, ConditionEnergyTypePKeyInit, false);
    initData = {
        "sid": "al-dict-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "dictType": "ApproveStatus"
    };
    BaseGetData(initData, ConditionAuditStatusCdInit, false);
    initData = {
        "sid": "al-dict-get",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "dictType": "ActiveStatus"
    };
    BaseGetData(initData, ConditionActiv, false);
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

function ConditionTertypenumInit(obj) {
    ConditionCmbInit(obj, "#ConditionTertypenum", "Tertypenum", "Ter_name");
}

function ConditionEnergyTypePKeyInit(obj) {
    ConditionCmbInit(obj, "#ConditionEnergyTypePKey", "PKey", "EnergyTypeName");
}

function ConditionAuditStatusCdInit(obj) {
    ConditionCmbInit(obj, "#ConditionAuditStatusCd", "DictCd", "DictName");
}

function ConditionActiv(obj) {
    ConditionCmbInit(obj, "#ConditionActiv", "DictCd", "DictName");
}

function DispalySettingInit(obj) {
    var data = [
                { key: "CarNo", value: "车牌号" },
                { key: "DPH", value: "VIN" },
                { key: "SimCode", value: "SIM卡号" },
                { key: "TNO", value: "终端ID" },
                { key: "ServiceEDay", value: "服务有效期止日" },
                { key: "PaymentDueDay", value: "到期还款日" },
                { key: "PaymentAccount", value: "到期还款金额" },
                { key: "PayStatus", value: "还款状态" },
                { key: "OverDays", value: "逾期天数" },
                { key: "LockStatus", value: "锁车状态" }
                ];
    // 启明
    if (PubUserCookie.RID == 26) {
        data.push({ key: "AuditStatus", value: "审核状态" });
        data.push({ key: "ApplyCompany", value: "申请公司" });
        data.push({ key: "ApplyLockStatus", value: "申请内容" });
        data.push({ key: "ApplyReason", value: "申请理由" });
        data.push({ key: "ApplyTime", value: "申请时间" });
    }
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
    var ConditionLeftDays = $("#ConditionLeftDays").numberbox('getValue');
    var ConditionActiv = $("#ConditionActiv").combobox("getText");
    var ConditionPayStatus = "";
    var ConditionTertypenum = "";
    var ConditionEnergyTypePKey = "";
    var ConditionAuditStatusCd = "";
    if ($("#ConditionLockStatus").combobox("getValue") == " ") {
        ConditionLockStatus = "";
    }
    if ($("#ConditionActiv").combobox("getValue") == " ") {
        ConditionActiv = "";
    }
    // 启明权限
    if (PubUserCookie.RID == 26) {
        ConditionPayStatus = $("#ConditionPayStatus").combobox("getText");
        ConditionTertypenum = $("#ConditionTertypenum").combobox("getValue");
        ConditionEnergyTypePKey = $("#ConditionEnergyTypePKey").combobox("getValue");
        ConditionAuditStatusCd = $("#ConditionAuditStatusCd").combobox("getValue");
        if ($("#ConditionPayStatus").combobox("getValue") == " ") {
            ConditionPayStatus = "";
        }
    }

    var mydata = {
        "sid": "al-lock-search",
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
        "ConditionPayStatus": ConditionPayStatus,
        "ConditionTertypenum": ConditionTertypenum,
        "ConditionEnergyTypePKey": ConditionEnergyTypePKey,
        "ConditionAuditStatusCd": ConditionAuditStatusCd,
        "ConditionActiv": ConditionActiv
    };

    return mydata;
}

function DoQuery() {
    var mydata = CheckCondition();
    if (mydata != null) {
        BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
    }
    $("#OpearateArea").panel("close");
}

function DoExport() {
    var mydata = CheckCondition();
    if (mydata != null) {
        mydata["sid"] = "al-lock-export";
        ExcelExport(mydata);
    }
}

function DoApply() {
    // 校验
    if (!$("#ApplyReason").textbox("isValid")) {
        return;
    }
    var selectedRow = $("#data_grid").datagrid("getSelected");

    // 启明的场合
    if (PubUserCookie.RID == 26) {
        if (!CheckServiceEDay(selectedRow.ServiceEDay)) {
            $.messager.alert("提示信息", "服务费已到期，请缴费后再申请!");
            return;
        }

        if (!CheckActiv(selectedRow.Activ)) {
            $.messager.alert("提示信息", "服务未激活，请激活后再申请!");
            return;
        }
    }

    var mydata = {
        "sid": "al-lock-apply",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "ApplyLockStatusCd": '',
        "ApplyReason": $("#ApplyReason").val(),
        "CID": selectedRow.CID
    };
    // 根据选中项 提示消息
    var service = "";
    if ($("#LockApply").prop("checked")) {
        mydata["ApplyLockStatusCd"] = $("#LockApply").val();
        service = "锁车";
    } else if ($("#OpenApply").prop("checked")) {
        mydata["ApplyLockStatusCd"] = $("#OpenApply").val();
        service = "解锁";
    } else {
        $.messager.alert("错误信息", "请选择要申请的指令！", "error");
        return;
    }
    var message = "是否申请" + selectedRow.CarNo + "的" + service + "服务";
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

function DoBeforeSet() {

    // 校验
    if (!$("#Speed").numberbox("isValid") ||
        !$("#BeforeValue").numberbox("isValid") ||
        !$("#Torque").numberbox("isValid") ||
        !$("#BeforeReason").textbox("isValid")) {
        return;
    }
    var selectedRow = $("#data_grid").datagrid("getSelected");

    // 启明的场合
    if (PubUserCookie.RID == 26) {
        if (!CheckServiceEDay(selectedRow.ServiceEDay)) {
            $.messager.alert("提示信息", "服务费已到期，请缴费后再设置!");
            return;
        }

        if (!CheckActiv(selectedRow.Activ)) {
            $.messager.alert("提示信息", "服务未激活，请激活后再设置!");
            return;
        }
    }

    var mydata = {
        "sid": "al-lock-set",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "id": selectedRow.id,
        "uid": PubUserCookie["UID"],
        "cid": selectedRow.CID,
        "tno": selectedRow.TNO,
        "orderType": "",
        "lockType": "",
        "torque": "",
        "rotspeed": "",
        "lockreason": "",
        "unlockreason": ""
    };
    var command = "";
    // 前装解锁
    if ($("#OpenBefore").prop("checked")) {
        // 发送0x8A指令
        mydata["orderType"] = 138;
        mydata["lockType"] = $("#OpenBefore").val();
        // 设置解锁原因
        mydata["unlockreason"] = $("#BeforeReason").val();
        // 设置解锁值
        if ($("#OpenBefore").val() == "02") {
            mydata["rotspeed"] = $("#BeforeValue").numberbox("getValue");
        } else if ($("#OpenBefore").val() == "04") {
            mydata["torque"] = $("#BeforeValue").numberbox("getValue");
        }
        command = "解锁";
    }
    // 前装锁车转速
    else if ($("#LockSpeed").prop("checked")) {
        // 发送0xA8指令
        mydata["orderType"] = 168;
        mydata["lockType"] = $("#LockSpeed").val();
        mydata["rotspeed"] = $("#Speed").val();
        // 设置锁车原因
        mydata["lockreason"] = $("#BeforeReason").val();
        command = "锁车转速";
    }
    // 前装锁车扭矩
    else if ($("#LockTorque").prop("checked")) {
        // 发送0xA8指令
        mydata["orderType"] = 168;
        mydata["lockType"] = $("#LockTorque").val();
        mydata["torque"] = $("#Torque").val();
        // 设置锁车原因
        mydata["lockreason"] = $("#BeforeReason").val();
        command = "锁车扭矩";
    }

    //  启明用户设置时须校验密码
    $.messager.prompt('确认信息', '请输入密码：', function (r) {
        if (r) {
            //校验密码是否正确
            if (r != PubUserCookie["pwd"]) {//todo
                $.messager.alert("错误信息", "密码错误!", "error");
            } else {
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
        }
    });
}

function DoAfterSet() {
    // 校验
    if (!$("#AfterReason").textbox("isValid")) {
        return;
    }
    var selectedRow = $("#data_grid").datagrid("getSelected");

    // 启明的场合
    if (PubUserCookie.RID == 26) {
        if (!CheckServiceEDay(selectedRow.ServiceEDay)) {
            $.messager.alert("提示信息", "服务费已到期，请缴费后再设置!");
            return;
        }

        if (!CheckActiv(selectedRow.Activ)) {
            $.messager.alert("提示信息", "服务未激活，请激活后再设置!");
            return;
        }
    }

    var mydata = {
        "sid": "al-lock-set",
        "sysuid": PubUserCookie["UID"],
        "token": PubUserCookie["token"].toString(),
        "sysflag": PubUserCookie["sysflag"].toString(),
        "id": selectedRow.id,
        "uid": PubUserCookie["UID"],
        "cid": selectedRow.CID,
        "tno": selectedRow.TNO,
        "orderType": "",
        "lockType": "",
        "torque": "",
        "rotspeed": "",
        "lockreason": "",
        "unlockreason": ""
    };
    var command = "";
    // 后装解锁
    if ($("#OpenAfter").prop("checked")) {
        // 发送0x8A指令
        mydata["orderType"] = 138;
        mydata["lockType"] = $("#OpenAfter").val();
        // 设置解锁原因
        mydata["unlockreason"] = $("#AfterReason").val();
        command = "解锁";
    }
    // 后装断油
    else if ($("#LockSpeed").prop("checked")) {
        // 发送0xA8指令
        mydata["orderType"] = 168;
        mydata["lockType"] = $("#LockOil").val();
        // 设置锁车原因
        mydata["lockreason"] = $("#AfterReason").val();
        command = "断油";
    }
    // 后装断电
    else if ($("#LockTorque").prop("checked")) {
        // 发送0xA8指令
        mydata["orderType"] = 168;
        mydata["lockType"] = $("#LockElectric").val();
        // 设置锁车原因
        mydata["lockreason"] = $("#AfterReason").val();
        command = "断电";
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
    $("#ConditionPayStatus").combobox("select", " ");
    $("#ConditionActiv").combobox("select", " ");
    $("#ConditionServiceEDayFrom").datebox("clear");
    $("#ConditionServiceEDayTo").datebox("clear");
    $("#ConditionActiv").combobox("select", " ");
    $("#ConditionServiceEDayFrom").datebox("clear");
    $("#ConditionServiceEDayTo").datebox("clear");
    $("#ConditionAuditStatusCd").combobox("select", " ");
    $("#ConditionTertypenum").combobox("select", " ");
    $("#ConditionEnergyTypePKey").combobox("select", " ");
    $("#data_grid").datagrid('loadData', { total: 0, rows: [] });
    $('#carWindow').window('open');
}

function DoCheckCar() {
    var selectedRow = $("#data_grid").datagrid("getSelected");
    if (selectedRow != null) {
        if (PubUserCookie.RID == "26" && selectedRow.AuditStatusCd == "02") {
            $.messager.alert('提示信息', "当前申请已经完成审核！", 'info');
        } else if (PubUserCookie.RID == "26" && (selectedRow.InstallType != "0" && selectedRow.InstallType != "1")) {
            $.messager.alert('提示信息', "当前车辆的安装方式未知！", 'info');
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
    $("#Speed").numberbox("clear");
    $("#Torque").numberbox("clear");
    $("#ApplyReason").val("");
    $("#BeforeReason").val("");
    $("#AfterReason").val("");
    $("#OpenBefore").val("");
    $("#OpenAfter").val("");
    // 经销商权限
    if (PubUserCookie.RID == "25") {
        // 车辆当前锁车状态是未锁车的情况
        if (row.LockstatusCd == "0") {
            // 显示经销商操作区域
            $("#OpearateArea").panel("open");
            $("#jxsArea").panel("open");
            // 使能锁车 选中锁车 失能解锁
            $("#LockApply").removeAttr("disabled");
            $("#LockApply").prop("checked", true);
            $("#OpenApply").prop("disabled", true);
        }
        // 车辆当前锁车状态是锁车的情况
        else if (row.LockstatusCd == "1" || row.LockstatusCd == "2" || row.LockstatusCd == "3") {
            // 显示经销商操作区域
            $("#OpearateArea").panel("open");
            $("#jxsArea").panel("open");
            // 使能解锁 选中解锁 失能锁车
            $("#OpenApply").removeAttr("disabled");
            $("#OpenApply").prop("checked", true);
            $("#LockApply").prop("disabled", true);
        }
        // 车辆当前锁车状态是未知的情况
        else {
            // 显示经销商操作区域
            $("#OpearateArea").panel("open");
            $("#jxsArea").panel("open");
            // 使能解锁 使能锁车
            $("#OpenApply").removeAttr("disabled");
            $("#OpenApply").prop("checked", false);
            $("#LockApply").removeAttr("disabled");
            $("#LockApply").prop("checked", false);
        }
    }
    // 启明权限的情况
    else if (PubUserCookie.RID == "26") {
        if (row.AuditStatusCd == "02") {
            $.messager.alert('提示信息', "当前申请已经完成审核！", 'info');
        } else {
            // 显示启明操作区域
            $("#OpearateArea").panel("open");
            $("#qmArea").panel("open");
            // 判断安装方式
            var isBefore;
            if (row.InstallType == "0") {
                isBefore = true;
            } else if (row.InstallType == "1") {
                isBefore = false;
            }
            // 前装车的情况
            if (isBefore) {
                // 显示前装区域 隐藏后装区域
                $("#t-before").show();
                $("#t-after").hide();
            } else {
                // 显示后装区域 隐藏前装区域
                $("#t-before").hide();
                $("#t-after").show();
            }
            CheckLockType(isBefore, row);
        }
    }
}

function CheckLockType(isBefore, row) {
    // 当前车辆申请解锁的情况
    if (row.ApplyLockStatusCd != "01") {
        // 前装的情况
        if (isBefore) {
            $("#LockSpeed").prop("disabled", true);
            $("#Speed").numberbox("disable").numberbox("clear");
            $("#OpenBefore").removeAttr("disabled");
            $("#OpenBefore").prop("checked", true); ;
            $("#BeforeValue").numberbox("enable").numberbox("clear");
            $("#LockTorque").prop("disabled", true);
            $("#Torque").numberbox("disable").numberbox("clear");
            $("#BeforeReason").textbox("enable").textbox("clear");
            $("#LockOil").prop("disabled", true);
            $("#OpenAfter").prop("disabled", true);
            $("#LockElectric").prop("disabled", true);
            $("#AfterReason").textbox("disable");
            // 锁车方式为转速
            if (row.LockType == "0") {
                // 指令类型02-解锁转速
                $("#OpenBefore").val("02");
            }
            // 锁车方式为扭矩
            else if (row.LockType == "1") {
                // 指令类型02-解锁扭矩
                $("#OpenBefore").val("04");
            }
        }
        // 后装的情况
        else {
            $("#LockSpeed").prop("disabled", true);
            $("#Speed").numberbox("disable").numberbox("clear");
            $("#OpenBefore").prop("disabled", true);
            $("#BeforeValue").numberbox("disable").numberbox("clear");
            $("#LockTorque").prop("disabled", true);
            $("#Torque").numberbox("disable").numberbox("clear");
            $("#BeforeReason").textbox("disable").textbox("clear");
            $("#LockOil").prop("disabled", true);
            $("#OpenAfter").removeAttr("disabled");
            $("#OpenAfter").prop("checked", true);
            $("#LockElectric").prop("disabled", true);
            $("#AfterReason").textbox("enable").textbox("clear");
            // 锁车方式为电
            if (row.LockType == "2") {
                // 指令类型08-解除控制油量
                $("OpenBefore").val("08");
            }
            // 锁车方式为油
            else if (row.LockType == "3") {
                // 指令类型10-解除控制电量
                $("OpenBefore").val("10");
            }
        }
    }
    // 当前车辆申请锁车的情况
    else {
        // 锁车方式为转速
        if (row.LockType == "0") {
            $("#LockSpeed").removeAttr("disabled");
            $("#LockSpeed").prop("checked", true);
            $("#Speed").numberbox("enable").numberbox("clear");
            $("#OpenBefore").prop("disabled", true);
            $("#BeforeValue").numberbox("disable").numberbox("clear");
            $("#LockTorque").prop("disabled", true);
            $("#Torque").numberbox("disable").numberbox("clear");
            $("#BeforeReason").textbox("enable").textbox("clear");
            $("#LockOil").prop("disabled", true);
            $("#OpenBefore").prop("disabled", true);
            $("#LockElectric").prop("disabled", true);
            $("#AfterReason").textbox("disable").textbox("clear");
        }
        // 锁车方式为扭矩
        else if (row.LockType == "1") {
            $("#LockSpeed").prop("disabled", true);
            $("#Speed").numberbox("disable").numberbox("clear");
            $("#OpenBefore").prop("disabled", true);
            $("#BeforeValue").numberbox("disable").numberbox("clear");
            $("#LockTorque").removeAttr("disabled");
            $("#LockTorque").prop("checked", true);
            $("#Torque").numberbox("enable").numberbox("clear");
            $("#BeforeReason").textbox("enable").textbox("clear");
            $("#LockOil").prop("disabled", true);
            $("#OpenBefore").prop("disabled", true);
            $("#LockElectric").prop("disabled", true);
            $("#AfterReason").textbox("disable").textbox("clear");
        }
        // 锁车方式为电
        else if (row.LockType == "2") {
            $("#LockSpeed").prop("disabled", true);
            $("#Speed").numberbox("disable").numberbox("clear");
            $("#OpenBefore").removeAttr("disabled");
            $("#BeforeValue").numberbox("disable").numberbox("clear");
            $("#LockTorque").prop("disabled", true);
            $("#Torque").numberbox("disable").numberbox("clear");
            $("#BeforeReason").textbox("disable").textbox("clear");
            $("#LockOil").prop("disabled", true);
            $("#OpenAfter").prop("disabled", true);
            $("#LockElectric").removeAttr("disabled");
            $("#LockElectric").prop("checked", true);
            $("#AfterReason").textbox("enable").textbox("clear");
        }
        // 锁车方式为油
        else if (row.LockType == "3") {
            $("#LockSpeed").prop("disabled", true);
            $("#Speed").numberbox("disable").numberbox("clear");
            $("#OpenBefore").removeAttr("disabled");
            $("#BeforeValue").numberbox("disable").numberbox("clear");
            $("#LockTorque").prop("disabled", true);
            $("#Torque").numberbox("disable").numberbox("clear");
            $("#BeforeReason").textbox("disabled").textbox("clear");
            $("#LockOil").removeAttr("disabled");
            $("#LockOil").prop("checked", true);
            $("#OpenAfter").prop("disabled", true);
            $("#LockElectric").prop("disabled", true);
            $("#AfterReason").textbox("enable").textbox("clear");
        }
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
        $("#ConditionLeftDays").numberbox("clear").numberbox("disable");
        $("#ConditionPayStatus").combobox("clear").combobox("disable");
        $("#ConditionTertypenum").combobox("clear").combobox("disable");
        $("#ConditionEnergyTypePKey").combobox("clear").combobox("disable");
        $("#ConditionActiv").combobox("clear").combobox("disable");
        $("#ConditionAuditStatusCd").combobox("clear").combobox("disable");
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
        $("#ConditionLeftDays").numberbox("clear").numberbox("enable");
        $("#ConditionPayStatus").combobox("clear").combobox("enable");
        $("#ConditionTertypenum").combobox("clear").combobox("enable");
        $("#ConditionEnergyTypePKey").combobox("clear").combobox("enable");
        $("#ConditionActiv").combobox("clear").combobox("enable");
        $("#ConditionAuditStatusCd").combobox("clear").combobox("enable");
    }
}

function CheckServiceEDay(ServiceEDay) {
    var curServiceEDay = ServiceEDay.substr(0, 10);
    if (curServiceEDay < GetNowFormatDate()) {
        return false;
    } else {
        return true;
    }
}

function CheckActiv(Activ) {
    if (Activ != "已激活") {
        return false;
    } else {
        return true;
    }
}

function GetNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = year + seperator1 + month + seperator1 + strDate;
    return currentdate;
}

function dateFormat(value, row, index) {
    if (value != null && value != "" && value.length >= 10) {
        return value.substr(0, 10);
    } else {
        return value;
    }
}

function formatPrice(value, row, index) {
    if (row != null) {
        return (parseFloat(value).toFixed(2) + '').replace(/\d{1,3}(?=(\d{3})+(\.\d*)?$)/g, '$&,');
    }
}

function addEmptyItem(records, valueField, textField) {
    var strEmptyItem = '{"' + textField + '": "--请选择--","' + valueField + '": " "' + '}';
    var emptyItem = JSON.parse(strEmptyItem); ;
    records.unshift(emptyItem);
}
