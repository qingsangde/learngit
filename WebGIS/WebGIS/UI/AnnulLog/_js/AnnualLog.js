var UserCookie;
var operatetype = "New";
var userRole; //用户角色
var R_JXS = "25";
var R_QM = "26";
//--------------------导出用------------------------
var exportcarNo;
var exportorgNo;
var exportapplyId;
var exportapplyReason;
var exportapplyTimeStart;
var exportapplyTimeEnd;
var exportapproveId;
var exportlockStatusCd;
var exportapproveTimeStart;
var exportapproveTimeEnd;
var exportlockWayCd;
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
    //    $('#loading-track').window('open');
    //画面制御
    if (UserCookie.RID == R_JXS) { //经销商.新增按钮显示
        $('#btnAdd').css({ "display": "inline" });
    } else if (UserCookie.RID == R_QM) { //启明.新增按钮不显示
        $('#btnAdd').css({ "display": "none" });
    }
    //    LoadDatagrid();
}

//可定制列
function DispalySettingInit(obj) {
    var data = [
                { key: "CarNumber", value: "车牌号" },
                { key: "OrgName", value: "放款机构" },
                { key: "ApplyId", value: "申请人" },
                { key: "ApplyTime", value: "申请时间" },
                { key: "ApplyReason", value: "申请内容" },
                { key: "ApproveId", value: "审批人" },
                { key: "ApproveTime", value: "审批时间" },
                { key: "LockStatus", value: "锁车状态" },
                { key: "LockWay", value: "锁车途径" },
                { key: "FullName", value: "操作人员" },
                { key: "OperateTime", value: "操作时间" },
                ];
    var option;
    var options;
    $.each(data, function (index, obj) {
        option = $("<input type='checkbox' name='DispalySettingCheckbox' value='" + obj.key + "' checked/><span>" + obj.value + "</span><br/>");
        $("#DispalySettingPanel").append(option);
    });
    // 冻结车牌号列
    $("[value='CarNumber']").prop("disabled", true);
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
    if (!$("#txtApplyTimeEnd").datebox("isValid") || !$("#txtApproveTimeEnd").datebox("isValid")) {
        return null;
    }
    LoadDatagrid();
}

//查询方法
function LoadDatagrid() {
    //取得查询条件
    var carNo = $("#txtCarNo").val(); //车牌号
    var orgNo = $("#cmbOrg").combobox('getValue'); //放款机构code
    var applyId = $("#txtApplyId").val(); //申请ID
    var applyReason = $("#cmbApplyReason").combobox('getValue'); //申请内容
    var applyTimeStart = $('#txtApplyTimeStart').datebox('getValue'); //申请开始时间
    var applyTimeEnd = $('#txtApplyTimeEnd').datebox('getValue'); //申请结束时间
    var approveId = $('#txtApproveId').val();  //审批ID
    var lockStatusCd = $('#cmbLockStatus').combobox('getValue'); //锁车状态cd
    var approveTimeStart = $('#txtApproveTimeStart').datebox('getValue'); //审批开始时间
    var approveTimeEnd = $('#txtApproveTimeEnd').datebox('getValue'); //审批结束时间
    var lockWayCd = $('#cmbLockway').combobox('getValue'); //锁车途径cd

    //------------------导出用------------------
    exportcarNo = carNo;
    exportorgNo = orgNo;
    exportapplyId = applyId;
    exportapplyReason = applyReason;
    exportapplyTimeStart = applyTimeStart;
    exportapplyTimeEnd = applyTimeEnd;
    exportapproveId = approveId;
    exportlockStatusCd = lockStatusCd;
    exportapproveTimeStart = approveTimeStart;
    exportapproveTimeEnd = approveTimeEnd;
    exportlockWayCd = lockWayCd;
    //------------------导出用------------------

    var uid = UserCookie["UID"];
    var mydata = {
        "sid": "annulLog-list-search",
        "sysuid": uid,
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"],
        "userrole": UserCookie.RID,
        "carNo": carNo,
        "orgNo": orgNo,
        "applyId": applyId,
        "applyReason": applyReason,
        "applyTimeStart": applyTimeStart,
        "applyTimeEnd": applyTimeEnd,
        "approveId": approveId,
        "lockStatusCd": lockStatusCd,
        "approveTimeStart": approveTimeStart,
        "approveTimeEnd": approveTimeEnd,
        "lockWayCd": lockWayCd
    };
    BindDataGrid_FrontPage(mydata, BingDataGrid_LoadSuccessCallBack_FrontPage);
}

//点击导出按钮
function DoExport() {

    var uid = UserCookie["UID"];
    var mydata = {
        "sid": "annulLog-list-export",
        "sysuid": uid,
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"],
        "userrole": UserCookie.RID,
        "carNo": exportcarNo,
        "orgNo": exportorgNo,
        "applyId": exportapplyId,
        "applyReason": exportapplyReason,
        "applyTimeStart": exportapplyTimeStart,
        "applyTimeEnd": exportapplyTimeEnd,
        "approveId": exportapproveId,
        "lockStatusCd": exportlockStatusCd,
        "approveTimeStart": exportapproveTimeStart,
        "approveTimeEnd": exportapproveTimeEnd,
        "lockWayCd": exportlockWayCd
    };
    ExcelExport(mydata);

}

//页面初始化生成下拉框
function GetAllCmb() {
    //显示设置
    DispalySettingInit();
    var mydata = {
        "sid": "annulLog-get-combobox",
        "sysuid": UserCookie["UID"],
        "token": UserCookie["token"],
        "sysflag": UserCookie["sysflag"]
    };
    BaseGetData(mydata, SetAllCmb);

}

//取得下拉框回调生成下拉框
function SetAllCmb(obj) {
    if (obj.state == 100) {
        var cmbOrg = obj.result.records[0]; //放款机构下拉框
        var cmbLockStatus = obj.result.records[1]; //锁车状态下拉框
        var cmbLockWay = obj.result.records[2]; //锁车方式
        var cmbApplyLockStatus = obj.result.records[3]; //申请内容

        //声明空白项
        var whiteOrg = { "OrgNo": "", "OrgName": "-请选择-" };
        var whiteLockStatus = { "LockStatusCode": "", "LockStatusName": "-请选择-" };
        var whiteLockWay = { "LockWayCode": "", "LockWayName": "-请选择-" };
        var whiteApplyLockStatus = { "ApplyLockStatusCode": "", "ApplyLockStatusName": "-请选择-" };
        cmbOrg.unshift(whiteOrg);
        cmbLockStatus.unshift(whiteLockStatus);
        cmbLockWay.unshift(whiteLockWay);
        cmbApplyLockStatus.unshift(whiteApplyLockStatus);

        $('#cmbOrg').combobox('loadData', cmbOrg); //放款机构
        $('#cmbLockStatus').combobox('loadData', cmbLockStatus); //锁车状态
        $('#cmbLockway').combobox('loadData', cmbLockWay); //锁车方式
        $('#cmbApplyReason').combobox('loadData', cmbApplyLockStatus); //申请内容
    }
}