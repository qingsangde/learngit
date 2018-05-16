var UserCookie;
var operatetype = "New";
var dealer = false;
var RegionArray = new Array();
var EditCID = 0;
var OrgLevel = new Array();
var LockLevel = new Array();
var EngineLevel = new Array();
var CustomerLevel = new Array();
var LoanLevel = new Array();

$(function () {
    UserCookie = GetUserInfo();
    GetAllCmb();
    setPage();

});

function setPage() {
//    $('#loading-track').window('open');
    LoadDatagrid();
}

///**
//* 从对象数组中获取属性为objPropery，值为objValue元素的对象
//* @param Array arrCar  数组对象
//* @param String objPropery  对象的属性
//* @param String objPropery  对象的值
//* @return Array 过滤后的数组
//*/
//function getfilter(arrCar, objPropery, objValue) {
//    return $.grep(arrCar, function (cur, i) {
//        return (cur[objPropery].toString() == objValue.toString())
//    });
//}

//function getfilterFuzzy(arrCar, objPropery, objValue) {
//    return $.grep(arrCar, function (cur, i) {
//        return (cur[objPropery].toString().indexOf(objValue.toString()) == 0)
//    });
//}



//点击查询按钮 查询事件
function DoQuery() {
    LoadDatagrid();
}

//查询方法
function LoadDatagrid() {
//取得查询条件
    var carno = $("#txtCarNo").val();//车牌号
    var carvin = $("#txtVIN").val();//车辆VIN
    var simCode = $("#txtSimCode").val(); //sim卡号
    var tno = $("#txtTNO").val();//终端ID
    var loanOrg = $('#cmbLoanOrg').combobox('getValue'); //放款机构code
    var status = $('#cmbStatus').combobox('getValue'); //锁车状态code
    var repaySDate = $('#txtRepaySDate').datebox('getValue'); //到期还款日开始日期
    var repayEDate = $('#txtRepayEDate').datebox('getValue'); //到期还款日结束日期
    var serviceSDate = $('#txtServiceSDate').datebox('getValue'); //服务有效期止日开始日期
    var serviceEDate = $('#txtServiceEDate').datebox('getValue'); //服务有效期止日结束日期

    var uid = UserCookie["UID"];
    var mydata = {
        "sid": "annulBase-list-search",
        "sysuid": uid,
        "token": UserCookie["token"].toString(),
        "sysflag": "YQWL",
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
    BaseGetData(mydata, setData);
}

//查询后回调
function setData(obj) {
    $('#loading-track').window('close');
    if (obj != null) {
        if (obj.state == 100) {
            RegionArray = obj.result.records;
            $('#data_grid').datagrid({ idField: "CID", loadFilter: pagerFilter }).datagrid('loadData', RegionArray);
            $('#data_grid').datagrid('clearSelections');
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('基础信息查询，服务器超时！');
            }
        }
    }
}

//分页
function pagerFilter(data) {
    if (typeof data.length == 'number' && typeof data.splice == 'function') {	// is array
        data = {
            total: data.length,
            rows: data
        }
    }
    var dg = $(this);
    var opts = dg.datagrid('options');
    var pager = dg.datagrid('getPager');
    pager.pagination({
        onSelectPage: function (pageNum, pageSize) {
            opts.pageNumber = pageNum;
            opts.pageSize = pageSize;
            pager.pagination('refresh', {
                pageNumber: pageNum,
                pageSize: pageSize
            });
            dg.datagrid('loadData', data);
        }
    });
    if (!data.originalRows) {
        data.originalRows = (data.rows);
    }
    var start = (opts.pageNumber - 1) * parseInt(opts.pageSize);
    var end = start + parseInt(opts.pageSize);
    data.rows = (data.originalRows.slice(start, end));
    return data;
}

//点击新建按钮
function openAdd() {
    //用户角色判断

    $("#lblMessage").html("基础信息新增成功");

    $('#baseInfoEdit').window({
        title: '基础信息新建',
        iconCls: 'icon-add'
    });

    $('#baseInfoEdit').window('open');


    //画面制御
        //sim卡可编辑
        $("#simcode").validatebox({
            editable : true
        });
        //确定按钮显示
        $("#btnsure").attr("disabled",false);

}

//点击确认按钮
function DoSure() {
    //根据SIM卡号查询数据
    var simcode = $("#simcode").val();
    var mydata = {
        "sid": "annulBase-sim-search",
        "sysuid": UserCookie["UID"].toString(),
        "token": UserCookie["token"].toString(),
        "sysflag": UserCookie["sysflag"].toString(),
        "simcode": simcode
    };
    BaseGetData(mydata, EditSimRes);
}

//页面初始化生成下拉框
function GetAllCmb(){
        var mydata = {
            "sid": "annulBase-getCombo",
            "sysuid": UserCookie["UID"].toString(),
            "token": UserCookie["token"].toString(),
            "sysflag": UserCookie["sysflag"].toString()
        };
        BaseGetData(mydata, SetAllCmb);

}

//取得下拉框回调生成下拉框
function SetAllCmb(obj){
    if (obj.state == 100) {
        var cmbOrg = obj.result.records[0];//放款机构下拉框
        var cmbLock = obj.result.records[1];//锁车状态下拉框
        //var carType = obj.result.records[2];//车辆类型下拉框
        var comEngine = obj.result.records[3];//发动机类型下拉框
        var comCustomer = obj.result.records[4];//客户类型下拉框
        var comLoan = obj.result.records[5];//贷款期数下拉框
        OrgLevel = $.extend(true, ["",""], cmbOrg);
        LockLevel = $.extend(true, ["", ""], cmbLock);
        EngineLevel = $.extend(true, ["", ""], comEngine);
        CustomerLevel = $.extend(true, ["", ""], comCustomer);
        LoanLevel = $.extend(true, ["", ""], comLoan);
        $('#cmbLoanOrg').combobox('loadData', OrgLevel);//放款机构
        $('#cmbStatus').combobox('loadData', LockLevel);//锁车状态
        //车辆类型
        $('#eOrgNo').combobox('loadData', OrgLevel);//放款机构

        $('#eCarEngineType').combobox('loadData', EngineLevel);//发动机类型
        $('#eCustomerType').combobox('loadData', CustomerLevel);//客户类型
        $('#eLendPeriods').combobox('loadData', LoanLevel); //贷款期数
    }
}

//查询sim卡号后回调赋值
function EditSimRes(obj) {
    if (obj.state == 100) {

        var carMessage = obj.result.records[0];  //1车辆信息
        //车辆信息赋值
        $("#eCID").val(carMessage.CID); //车牌ID隐藏项
        $("#eCarNo").val(carMessage.CarNo);//车牌号码
        $("#eCarVin").val(carMessage.DPH);//车架VIN码
        $("#eCarType").val(carMessage.CTID);//车型(车辆品牌)
        $("#eCarEngine").combobox('setValue',carMessage.FDJH);//发动机号
        $("#eCarEngineType").combobox('setValue',carMessage.EnergyTypePKey);//引擎类型
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
    //新增保存
    if (operatetype == "New") {

        //取得页面数据
        var mydata = {
            "sid": "annulBase-addSave",
            "sysuid": uid,
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString()
        };
        BaseGetData(mydata, OperateRegionRes);
    }
    else {
        //修改保存
        var mydata = {
            "sid": "annulBase-editSave",
            "sysuid": uid,
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),

            "regionid": EditCID
        };
        BaseGetData(mydata, OperateRegionRes);
    }
    $('#baseInfoEdit').window('close');
    $('#Div1').window('open');
}

//保存后确认
function DoAgree() {
    $('#Div1').window('close');
}


//点击修改按钮事件
function openEdit() {
    $("#lblMessage").html("基础信息修改成功");
    var rows = $('#data_grid').datagrid('getSelections');
    if (rows.length == 1) {
        operatetype = "Edit";
        EditCID = rows[0].CID;
        //弹出修改页面
                var mydata = {
            "sid": "annulBase-one-search",
            "sysuid": UserCookie["UID"].toString(),
            "token": UserCookie["token"].toString(),
            "sysflag": UserCookie["sysflag"].toString(),
            "cid": EditCID
        };

        BaseGetData(mydata, EditInitRes);
    }
    else {
        $.messager.alert('错误信息', '请选择一行！', 'error');
    }
}


//点击删除
function DoDelete() {

    var rows = $('#data_grid').datagrid('getSelections');
    var ids = "";
    if (rows != null && rows.count > 0) {
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].CID + ",";
        }
        ids = ids.substring(0, ids.length - 1);

        $.messager.confirm("操作提示", "您确定删除所选内容吗？", function (data) {
            if (data) {
                var mydata = {
                    "sid": "annulBase-delete",
                    "sysuid": UserCookie["UID"],
                    "sysflag": UserCookie["sysflag"].toString(),
                    "token": UserCookie["token"].toString(),
                    "ids": ids
                };
                BaseGetData(mydata, deleteBaseInfoRes);
            }
            else {
                return;
            }
        });
    }
    else {
        $.messager.alert('系统提示：', '请先选择行！', 'error');

    }
}

    //删除后回调
function deleteBaseInfoRes(obj) {
        if (obj != null) {
            if (obj.state == 100) {
                $.messager.alert("系统提示：", obj.msg, "info");
                //            setTimeout(function () {
                //                $('#details').window('close');
                //            }, 1000);
                DoQuery();
            }
            else {
                $.messager.alert("系统提示：", obj.msg, "error");

                //            setTimeout(function () {
                //                $('#details').window('close');
                //            }, 1000);

            }
        }
    }

    //修改页面初始化
    function EditInitRes(obj) {
        //画面制御
        //sim卡不可编辑
        $("#simcode").validatebox({
            editable: false
        });
        //确定按钮不显示
        $("#btnsure").attr("disabled", true);

        if (obj.state == 100) {

            $('#baseInfoEdit').window({
                title: '基础信息修改',
                iconCls: 'icon-edit'
            });
            $('#baseInfoEdit').window('open');

            var qscarMessage = obj.result.records[0];  //车辆信息
            var alcarMessage = obj.result.records[1];    //车辆补充信息
            var loanMessage = obj.result.records[2];    //贷款信息

            //页面赋值
            $("#eCarNo").val(qscarMessage.CarNo);
            $("#eCarVin").val(qscarMessage.DPH);
            $("#eCarBrand").val(alcarMessage.CBrand);
            $("#eCarType").combobox('setValue', qscarMessage.DPH);
            $("#eCarEngine").val(qscarMessage.FDJH);
            $("#eCarEngineType").combobox('setValue', qscarMessage.EnergyTypePKey);
            $("#eCustomerType").combobox('setValue', alcarMessage.CustomerType);
            $("#eCustomerName").val(alcarMessage.CustomerName);
            $("#eLinkman").val(alcarMessage.ContactMan);
            $("#ePhone").val(alcarMessage.ContactNumber);
            $("#eAddress").val(alcarMessage.ContactAddress);
            $("#eCarOwnName").val(qscarMessage.CarOwnName);
            $("#eCarOwnTel").val(qscarMessage.CarOwnTel);
            $("#eLoanContractNo").val(loanMessage.LoanContractNo);
            $("#eOrgNo").combobox('setValue', loanMessage.OrgNo);
            $("#eLendDate").datebox('setValue', loanMessage.LendDate);
            $("#eLendPeriods").combobox('setValue', loanMessage.LendPeriods);
            $("#ePaymentAccount").val(loanMessage.PaymentAccount);
            $("#eServiceSDay").datebox('setValue', alcarMessage.ServiceSDay);
            $("#eServiceEDay").datebox('setValue', alcarMessage.ServiceEDay);

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

    //编辑保存
    function DoSaveEdit() {
        var sysflag = "YQWL";//数据库标识
        var sysuid = "";
        var cid = EditCID; //车辆CID
        //新增时车辆ID
        //车辆信息
        var carno = $('#eCarNo').val().trim();//车牌号码
        var carvin = $('#eCarVin').val().trim(); //车架VIN码
        var carbrand = $('#eCarBrand').val().trim(); //车辆品牌
        var cartype = $('#eCarType').combobox('getValue');//车辆类型
        var fdjh = $('#eCarEngine').val().trim();//发动机号
        var carenginetype = $('#eCarEngineType').combobox('getValue'); //发动机类型
        //客户信息
        var custype = $('#eCustomerType').combobox('getValue'); //客户类型
        var cusname = $('#eCustomerName').val().trim(); //客户名称
        var contactman = $('#eLinkman').val().trim(); //联系人
        var contactnumber = $('#eCustomerName').val().trim(); //联系电话
        var contactaddress = $('#eCustomerName').val().trim(); //联系地址
        var carownname = $('#eCarOwnName').val().trim(); //车主姓名
        var carowntel = $('#eCarOwnTel').val().trim(); //车主电话
        //贷款信息
        var contractno = $('#eLoanContractNo').val().trim(); //贷款合同号
        var orgno = $('#eOrgNo').combobox('getValue'); //放款机构
        var lenddate = $('#eLendDate').datebox('getValue'); //放款日期
        var lendPeriods = $('#eLendPeriods').combobox('getValue'); //贷款期数
        var paymentaccount = $('#ePaymentAccount').val().trim(); //到期还款金额
        //服务有效期
        var servicesday = $('#eServiceSDay').datebox('getValue'); //服务有效期开始日期
        var serviceeday = $('#eServiceEDay').datebox('getValue'); //服务有效期结束日期
        
        var mydata = {
            "sid": "annul-edit-save",
            "sysflag": sysflag,
            "token": "",
            "cid": cid,
            "carno": carno,
            "carvin": carvin,
            "carbrand": carbrand,
            "cartype": cartype,
            "fdjh": fdjh,
            "carenginetype": carenginetype,
            "custype": custype,
            "cusname": cusname,
            "contactman": contactman,
            "contactnumber": contactnumber,
            "contactaddress": contactaddress,
            "carownname": carownname,
            "carowntel": carowntel,
            "contractno": contractno,
            "orgno": orgno,
            "lenddate": lenddate,
            "lendPeriods": lendPeriods,
            "paymentaccount": paymentaccount,
            "servicesday": servicesday,
            "serviceeday": serviceeday,
            "sysuid": sysuid
        };

        BaseGetData(mydata, EditSaveRes);
    }

    //新增保存
    function DoSaveAdd() {
        var sysflag = "YQWL"; //数据库标识
        var sysuid = "";
        var cid = $("#eCID").val().trim() ; //车辆CID
        //新增时车辆ID
        //车辆信息
        var carno = $('#eCarNo').val().trim(); //车牌号码
        var carvin = $('#eCarVin').val().trim(); //车架VIN码
        var carbrand = $('#eCarBrand').val().trim(); //车辆品牌
        var cartype = $('#eCarType').combobox('getValue'); //车辆类型
        var fdjh = $('#eCarEngine').val().trim(); //发动机号
        var carenginetype = $('#eCarEngineType').combobox('getValue'); //发动机类型
        //客户信息
        var custype = $('#eCustomerType').combobox('getValue'); //客户类型
        var cusname = $('#eCustomerName').val().trim(); //客户名称
        var contactman = $('#eLinkman').val().trim(); //联系人
        var contactnumber = $('#eCustomerName').val().trim(); //联系电话
        var contactaddress = $('#eCustomerName').val().trim(); //联系地址
        var carownname = $('#eCarOwnName').val().trim(); //车主姓名
        var carowntel = $('#eCarOwnTel').val().trim(); //车主电话
        //贷款信息
        var contractno = $('#eLoanContractNo').val().trim(); //贷款合同号
        var orgno = $('#eOrgNo').combobox('getValue'); //放款机构
        var lenddate = $('#eLendDate').datebox('getValue'); //放款日期
        var lendPeriods = $('#eLendPeriods').combobox('getValue'); //贷款期数
        var paymentaccount = $('#ePaymentAccount').val().trim(); //到期还款金额
        //服务有效期
        var servicesday = $('#eServiceSDay').datebox('getValue'); //服务有效期开始日期
        var serviceeday = $('#eServiceEDay').datebox('getValue'); //服务有效期结束日期

        var mydata = {
            "sid": "annul-add-save",
            "sysflag": sysflag,
            "token": "",
            "cid": cid,
            "carno": carno,
            "carvin": carvin,
            "carbrand": carbrand,
            "cartype": cartype,
            "fdjh": fdjh,
            "carenginetype": carenginetype,
            "custype": custype,
            "cusname": cusname,
            "contactman": contactman,
            "contactnumber": contactnumber,
            "contactaddress": contactaddress,
            "carownname": carownname,
            "carowntel": carowntel,
            "contractno": contractno,
            "orgno": orgno,
            "lenddate": lenddate,
            "lendPeriods": lendPeriods,
            "paymentaccount": paymentaccount,
            "servicesday": servicesday,
            "serviceeday": serviceeday,
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
