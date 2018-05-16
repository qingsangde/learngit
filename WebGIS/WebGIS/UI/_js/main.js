var userInfo;
var UserCookie;
var AlarmArr = new Array();
var CurAlarmIndex = 0;
var UserCarArray = new Array();
window.onload = function () {

    //页面加载动画效果
    $('#loading-mask').fadeOut(2000, function () {
    });

};
var browser = 0;
$(function () {

    UserCookie = GetUserInfo();
    InitUserCars();
    document.getElementById('sysflag').value = UserCookie.sysflag;
    var ft = "";
    if (UserCookie.sysflag == "HRBKY" || UserCookie.sysflag == "HRBHY") {
        ft = "启明GPS监控中心  400-994-7888";
    } else if (UserCookie.sysflag == "YQWL" || UserCookie.sysflag == "JFJY") {
        ft = "启明GPS监控中心  400-118-2299";
    }
    var grrsysflag = GetQueryString("key");
    if (grrsysflag == "LIUTE") {
        var headlogo = document.getElementById('headlogo');
        headlogo.src = "_styles/images/logo2.png";
        var headlogo = document.getElementById('headtext');
        headlogo.innerText = "一汽解放柳州特种汽车有限公司";

    }
    if (grrsysflag == "ALARM") {
        $("#divphotoinfo").hide()
        $("#divabout").hide()
        $("tab-tools").width(100);
        var headlogo = document.getElementById('headtext');
        headlogo.innerText = "驾驶伙伴";
        ft = "监控中心  400-118-2299";
        $(document).attr("title", "驾驶伙伴");
    }
    if (grrsysflag == "DPTEST" || grrsysflag == "LIUTE" || grrsysflag == "JFJY") {
        $("#mmeny").show()
        $("#jfjydd").show()
//---------X80权限控制-------
        if (UserCookie.RID==24) {
            $('#tabs').tabs('close', 3);
        }
    }
    else {
        $("#mmeny").hide()
        $("#jfjydd").hide()
    }
    document.getElementById('foottitle').innerText = ft;

    InitLeftMenu();
    StartHeart(); //开始心跳
    browser = getGrowser();
    //    $('#docbody').layout();
    //    // 折叠 west panel   
    //    $('#docbody').layout('collapse', 'west');
    document.getElementById('labusername').innerText = UserCookie.UName;
    //$('#labusername').innerText ="欢迎您，" +UserCookie.UName;
    //显示报警信息接口未调试，暂时注释掉
    setInterval(function () {
        UpdateAlarm();
    }, 2000);
    ReceiveOrder();

    setInterval(function () {
        UpdatePicture();
    }, 2000);  //刷新指令结果显示图标

    if (UserCookie.RID == "25" || UserCookie.RID == "26") {
    } else {
        $("#tabs").tabs("close", "销贷业务");
    }
});

//轮询显示报警信息
function UpdateAlarm() {
    if (AlarmArr == null || AlarmArr.length == 0) {
        $("#divAlarm").hide();

    }
    else {
        $("#divAlarm").show();
        //alert(new Date("2014/08/09 08:01:33").format("yyyy-MM-dd hh:mm:ss"));
        if (CurAlarmIndex < (AlarmArr.length - 1)) {
            CurAlarmIndex = CurAlarmIndex + 1;
        }
        else if (CurAlarmIndex >= (AlarmArr.length - 1)) {
            CurAlarmIndex = 0;
        }
        var carno = AlarmArr[CurAlarmIndex].CarNum;
        var time = AlarmArr[CurAlarmIndex].TDateTime;
        var ainfo = AlarmArr[CurAlarmIndex].AlarmStr;
        /*Info中每个成员对象的字段定义如下：Alarm: 0
        AlarmStr: "无"
        AltitudeMeters: 0
        CarNum: "吉A-M3216"
        Carid: 1
        Heading: 0
        HeadingStr: "北"
        Lati: 43.8280945
        Long: 125.204826
        Speed: 0
        Status: 64
        StatusStr: "不定位"
        SumMiles: 2335372
        TDateTime: "2014-08-09T08:01:33"
        TNO: 2181208937
        */
        $('#spanAlarm')[0].innerHTML = carno + " " + time + " " + ainfo;
    }
}

//初始化用户权限车辆
function InitUserCars() {
    var mydata = {
        "sid": "sys-user-getallcar",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "onecaruser": UserCookie["OneCarUser"].toString()
    };
    BaseGetData(mydata, BindCarInfo);
}
var isf = true;
function BindCarInfo(obj) {
    if (obj != null) {
        if (obj.state == 100) {
            UserCarArray = obj.result.records;
            if (isf) {
                $.messager.alert('提示', '车辆信息获取成功！总计：' + obj.result.records.length, 'info');
                isf = false;
            }
            //王虎修改 增加定时获取用户车辆数据，用于数据中车辆在线状态的更新 定时5分钟
            setTimeout('InitUserCars', 300000);
        }
        else {
            if (obj.state == 104) {
                LoginTimeout('车辆信息获取，服务器超时！');
            }
        }
    }
}

//初始化导航菜单
function InitLeftMenu() {
    SetMenusByRole();
    //去动画效果
    $('#nav').accordion({
        animate: false
    });

}
function menuEven() {
    $('.easyui-accordion li a').click(function () {
        var tabTitle = $(this).children('.nav').text();
        var url = $(this).attr("rel");
        var menuid = $(this).attr("ref");
        var icon = getIcon(menuid, icon);
        addTab(tabTitle, url, icon);
        $('.easyui-accordion li div').removeClass("selected");
        $(this).parent().addClass("selected");
    }).hover(function () {
        $(this).parent().addClass("hover");
    }, function () {
        $(this).parent().removeClass("hover");
    });
}

//获取左侧导航的图标
function getIcon(menuid) {
    var icon = 'icon ';
    $.each(_menus.menus, function (i, n) {
        $.each(n.menus, function (j, o) {
            if (o.menuid == menuid) {
                icon += o.icon;
            }
        })
    })
    return icon;
}
function addTab(subtitle, url, icon) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url),
            closable: true,
            height: 200,
            icon: icon
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        //再次刷新
        var currTab = $('#tabs').tabs('getSelected');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url)
            }
        })
    }
}
var SelectCar;
function AutoShowTrack(car) {
    SelectCar = car;
    $('#tabs').tabs('select', '轨迹回放');
    //再次刷新
    var currTab = $('#tabs').tabs('getSelected');
    $('#tabs').tabs('update', {
        tab: currTab,
        options: {
            content: createFrame('Track/Track.html')
        }
    })
}
function createFrame(url) {
    var s = '<iframe scrolling="no" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
    return s;
}



function openwMyInfo() {

    var tabTitle = "客户信息";
    var url = "OA/S0006.htm";
    addTab(tabTitle, url, "icon icon-users");
    $('.easyui-accordion li div').removeClass("selected");
    $(this).parent().addClass("selected");
    slide();
}
function openwAbout() {

    $('#wAbout').window('open');
}


//添加管理按钮 子项跳转页面 2016-03-25 wx
function openCarManager() {
    $('#wCarManage').window('open');
    $('#wCarManage').window('center');
//    var userinfo = GetUserInfo();
//    var urlParamS = "?user=" + userinfo.UName + "&pass=" + userinfo.pwd + "&ip=10.44.30.213&port=3308&random=" + Math.random();
//    //   alert(urlParamS);
//    var tzURL = "http://dptest.qm.cn:3706/WebMIS/Car/FormListNew.aspx" + urlParamS;
//    //打开车辆管理页面
//    window.open(tzURL, 'newwindow');

}

function openFwjy() {
    var userinfo = GetUserInfo();
    var urlParamS = "?account=" + userinfo.UName + "&password=" + userinfo.pwd;
    var tzURL = "http://10.44.30.113:8088/UI/userLogin.html" + urlParamS;
    //打开服务救援页面
    window.open(tzURL, 'newwindow');
}
//*****************


function openwHelp() {
    $('#wHelp').window('open');
}
function openwMyPWD() {
    $('#txtKey').val('');
    $('#wMyPWD').window('open');
}
//更新密码
function rePwd() {
    if ($('#fPwd').form('validate')) {
        var mydata = {
            "sid": "sys-user-updpwd",
            "sysuid": UserCookie["UID"],
            "sysflag": UserCookie["sysflag"].toString(),
            "token": UserCookie["token"].toString(),
            "onecaruser": UserCookie["OneCarUser"].toString(),
            "newpassword": $('#txtNewPwd').val()
        };
        BaseGetData(mydata, UpdPwdRes);
    }
}
//更新密码成功回调
function UpdPwdRes(msg) {
    var data = msg.result;
    var state = msg['state'];
    if (state == 100) {
        if (data.status && data.status > 0) {
            delCookie("username");
            $.messager.alert('密码修改', '密码修改成功,请重新登入!', '', Logout);
        } else {
            $.messager.alert('密码修改', '密码修改失败!', 'error');
        }
    }
    else {
        $.messager.alert('错误信息', msg['msg'], 'error');
    }
}
//注销
function openLock() {
    var mydata = {
        "sid": "sys-user-logout",
        "sysuid": UserCookie["UID"],
        "sysflag": UserCookie["sysflag"].toString(),
        "token": UserCookie["token"].toString(),
        "onecaruser": UserCookie["OneCarUser"].toString()
    };
    BaseGetData(mydata, closeWin, false);
}
//关闭当前页
function closeWin() {
    //delCookie("UserCookie");   
    //    window.opener = null;
    //    window.open('', '_self');
    //    window.close(); 
    Logout();
}

function closeMyPWD() {
    $('#wMyPWD').window('close');
}



function closeLock() {
    if ($('#fLock').form('validate')) {
        $('#wLock').window('close');
    }
}

$.extend($.fn.validatebox.defaults.rules, {
    rePwd: {
        validator: function (value, param) {
            return value == $("#txtNewPwd").val();
        }
    }
});

function slide() {
    $.messager.show({
        title: '系统消息',
        msg: '',
        timeout: 5000,
        showType: 'slide'
    });
}
var iframe2loadfalg = false;
var iframe3loadfalg = false;
var iframe4loadfalg = false;
var iframe5loadfalg = false;
var iframe6loadfalg = false;
var iframe7loadfalg = false;
var iframe8loadfalg = false;
var iframe9loadfalg = false;
var iframe10loadfalg = false;
var iframe11loadfalg = false;
function selPrivateTab(title, index) {
    var grrsysflag = GetQueryString("key");
    if (index == 0) {
        //
    } else if (index == 1) {

        var ifr2 = document.getElementById("iframe2");
        if (!iframe2loadfalg) {

            ifr2.src = "Track/Track.html";
            iframe2loadfalg = true;
        }
    }
//     else if (index == 2) {
//        var ifr3 = document.getElementById("iframe3");
//        if (!iframe3loadfalg) {

//            ifr3.src = "Query/Query.html?key=" + grrsysflag;
//            iframe3loadfalg = true;
//        }
//    }
//    else if (index == 3) {
//        var ifr4 = document.getElementById("iframe4");
//        if (!iframe4loadfalg) {

//            ifr4.src = "DataSetting/DataSetting.htm";
//            iframe4loadfalg = true;
//        }
//    }
    //----------------
    else if (index == 2) {
        var ifr5 = document.getElementById("iframe5");
        if (!iframe4loadfalg) {

            ifr5.src = "XiaoDai/XiaoDai.htm";
            iframe5loadfalg = true;
        }
    }
    //----------------
    // else if (index == 4) {
//        var ifr5 = document.getElementById("iframe5");
//        if (!iframe5loadfalg) {

//            ifr5.src = "XiaoDai/XiaoDai.htm";
//            iframe5loadfalg = true;
//        }
//    } else if (index == 5) {
//        var ifr6 = document.getElementById("iframe6");
//        if (!iframe6loadfalg) {
//            var userinfo = GetUserInfo();
//            if (userinfo) {
//                var urlParamS = "?user=" + userinfo.UName + "&pass=" + userinfo.pwd + "&ip=10.44.30.213&port=3308";
//                ifr6.src = "http://cc.faw.com.cn/NSQV_LQTest/NSQV/ZhenDuan.htm" + urlParamS;
//                iframe6loadfalg = true;
//            }
//        }
//    } else if (index == 6) {
//        var ifr7 = document.getElementById("iframe7");
//        if (!iframe7loadfalg) {
//            ifr7.src = "YouWeiVideo.html";
//            iframe7loadfalg = true;
//        }
//    } else if (index == 7) {
//        var ifr8 = document.getElementById("iframe8");
//        if (!iframe8loadfalg) {
//            ifr8.src = "Fence/fencemain.htm";
//            iframe8loadfalg = true;
//        }
//    } else if (index == 8) {
//        var ifr9 = document.getElementById("iframe9");
//        if (!iframe9loadfalg) {
//            ifr9.src = "CarDataStatistic/statistic.htm";
//            iframe9loadfalg = true;
//        }
//    } else if (index == 9) {
//        var ifr10 = document.getElementById("iframe10");
//        if (!iframe10loadfalg) {
//            ifr10.src = "DrivingAnalysis/DrivingAnalysis.htm";
//            iframe10loadfalg = true;
//        }
//    } else if (index == 10) {
//        var ifr11 = document.getElementById("iframe11");
//        if (!iframe10loadfalg) {
//            ifr11.src = "park/park.htm";
//            iframe11loadfalg = true;
//        }
//    }
    //var asd=  $('#tabs').tabs('getTab', index);  


}

function SetMenusByRole() {
    var grrsysflag = GetQueryString("key");
    var roles = "";
    var userinfo = GetUserInfo();
//    if (userinfo) {
//        roles = userinfo.A_Name;
//        var rolesarray = roles.split(',');
//        if (arraycontains(rolesarray, '001')) {
//            $('#tabs').tabs('getTab', '定位监控').panel('options').tab.show();

//        }
//        else {
//            $('#tabs').tabs('getTab', '定位监控').panel('options').tab.hide();

//        }
//        if (arraycontains(rolesarray, '002') ) {
//            $('#tabs').tabs('getTab', '轨迹回放').panel('options').tab.show();

//        }
//        else {
//            $('#tabs').tabs('getTab', '轨迹回放').panel('options').tab.hide();

//        }
//        if (arraycontains(rolesarray, '003')  ) {
//            $('#tabs').tabs('getTab', '统计分析').panel('options').tab.show();

//        }
//        else {
//            $('#tabs').tabs('getTab', '统计分析').panel('options').tab.hide();

//        }
//        if (arraycontains(rolesarray, '014') && grrsysflag != "LIUTE" ) {
//            $('#tabs').tabs('getTab', '指令下发').panel('options').tab.show();

//        }
//        else {
//            $('#tabs').tabs('getTab', '指令下发').panel('options').tab.hide();

//        }

//        if (grrsysflag == "YQWL" || grrsysflag == "DPTEST") {
//            $('#tabs').tabs('getTab', '视频监控').panel('options').tab.show();

//        }
//        else {
//            $('#tabs').tabs('getTab', '视频监控').panel('options').tab.hide();
//        }
//        if (grrsysflag == "JFJY" || grrsysflag == "LIUTE" || grrsysflag == "DPTEST") {
//            $('#tabs').tabs('getTab', '销贷业务').panel('options').tab.show();
//            $('#tabs').tabs('getTab', '车辆诊断').panel('options').tab.show();
//            $('#tabs').tabs('getTab', '能耗分析').panel('options').tab.show();
//            $('#tabs').tabs('getTab', '驾驶行为分析').panel('options').tab.show();
//        }
//        else {
//            $('#tabs').tabs('getTab', '销贷业务').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '车辆诊断').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '能耗分析').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '驾驶行为分析').panel('options').tab.hide();
//        }
//        if (grrsysflag == "ALARM" || grrsysflag == "DPTEST") {
//            $('#tabs').tabs('getTab', '驻车提示').panel('options').tab.show();

//        }
//        else {
//            $('#tabs').tabs('getTab', '驻车提示').panel('options').tab.hide();
//        }
//        if (!arraycontains(rolesarray, '001') && arraycontains(rolesarray, '002')) {
//            $('#tabs').tabs('select', '轨迹回放');
//        }
//        if (!arraycontains(rolesarray, '001') && !arraycontains(rolesarray, '002') && arraycontains(rolesarray, '003')) {
//            $('#tabs').tabs('select', '统计分析');
//        }

//        if (grrsysflag == "ALARM") {
//            $('#tabs').tabs('getTab', '电子围栏').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '轨迹回放').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '统计分析').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '指令下发').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '视频监控').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '销贷业务').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '车辆诊断').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '能耗分析').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '驾驶行为分析').panel('options').tab.hide();

//        }
//        if (grrsysflag == "LIUTE") {
//           
//            $('#tabs').tabs('getTab', '指令下发').panel('options').tab.hide();
//            $('#tabs').tabs('getTab', '视频监控').panel('options').tab.hide();            
//            $('#tabs').tabs('getTab', '驻车提示').panel('options').tab.hide();
//        }
//    }

}

function UpdateAlarmInfo(Info) {
    AlarmArr = [];
    AlarmArr = Info;
}
