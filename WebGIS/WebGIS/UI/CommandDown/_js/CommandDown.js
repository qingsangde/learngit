
$(function () {
    InitLeftMenu();
})
var initpage;
//初始化导航菜单
function InitLeftMenu() {
    //去动画效果
    $('#nav').accordion({
        animate: false
    });
    //依据用户权限初始化菜单参数
    SetMenusByRole();
    $.each(_menus.menus, function (i, n) {

        var menulist = new Array();
        menulist.push('<ul>');
        $.each(n.menus, function (j, o) {
            //            menulist.push('<li><div>');
            //            menulist.push('<a ref="' + o.menuid + '" href="#" rel="' + o.url + '" onclick="menuClick(\'' + o.url + '\',\'' + o.menuname + '\')">');
            //            //menulist.push(o.menuname);
            //            menulist.push('<span class="icon ' + o.icon + '" >&nbsp;</span>' + o.menuname);
            //            menulist.push('</a></div></li>');
            menulist.push('<li><div onclick="menuClick(\'' + o.url + '\',\'' + o.menuname + '\')">');
            //menulist.push('<a ref="' + o.menuid + '" href="#" rel="' + o.url + '" onclick="menuClick(\'' + o.url + '\',\'' + o.menuname + '\')">');
            //menulist.push(o.menuname);
            menulist.push('<span class="icon ' + o.icon + '" >&nbsp;</span>' + o.menuname);
            //menulist.push('</a>');
            menulist.push('</div></li>');
            if (i == 0 && j == 0)
                initpage = o;
        })
        menulist.push('</ul>');

        $('#nav').accordion('add', {
            title: n.menuname,
            content: menulist.join(''),
            iconCls: 'icon ' + n.icon
        });
    });


    $('#nav').accordion({
        animate: true
    });
    if (initpage) {
        menuClick(initpage.url, initpage.menuname);
    }

}
function menuClick(url, name) {
    document.getElementById('mainiframe').src = url;

    $('#mainPanle').panel({ title: name });
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

function SetMenusByRole() {

    var roles = "";
    var userinfo = GetUserInfo();
    if (userinfo) {
        roles = userinfo.A_Name;
        var rolesarray = roles.split(',');

        if (arraycontains(rolesarray, '015')) {
            var speedmenu = { "menuid": "10",
                "menuname": "超速设置",
                "icon": "icon-syss",
                "url": "maxspeedset.htm"
            };

            _menus.menus[0].menus.push(speedmenu);
        }
        if (arraycontains(rolesarray, '016')) {
            var callmenu = { "menuid": "11",
                "menuname": "车辆呼转",
                "icon": "icon-syss",
                "url": "calltransfer.htm"
            };
            _menus.menus[0].menus.push(callmenu);
        }
        if (arraycontains(rolesarray, '017')) {
            var photomenu = { "menuid": "12",
                "menuname": "车辆拍照",
                "icon": "icon-syss",
                "url": "photograph.htm"
            };
            _menus.menus[0].menus.push(photomenu);
        }
        if (arraycontains(rolesarray, '018')) {
            var infomenu = { "menuid": "13",
                "menuname": "行驶记录仪采集",
                "icon": "icon-syss",
                "url": "recorddatacollect.htm"
            };
            _menus.menus[0].menus.push(infomenu);
        }

        if (arraycontains(rolesarray, '021')) {
            var infomenu0 = { "menuid": "13",
                "menuname": "行驶记录仪参数设置",
                "icon": "icon-syss",
                "url": "driverrecorddownset.htm"
            };
            _menus.menus[0].menus.push(infomenu0);
        }


        var infomenu11 = { "menuid": "14",
            "menuname": "终端参数设置",
            "icon": "icon-syss",
            "url": "CarTerminalParamDown.htm"
        };
        _menus.menus[0].menus.push(infomenu11);
        //        if (arraycontains(rolesarray, '25')) {
        //            var carmenu = { "menuid": "10",
        //                "menuname": "警情统计",
        //                "icon": "icon-syss",
        //                "url": "MileageStatus.htm"
        //            };
        //            _menus.menus[0].menus.push(carmenu);
        //        }
        //        if (arraycontains(rolesarray, '26')) {

        //            var stationmenu = { "menuid": "11",
        //                "menuname": "里程统计",
        //                "icon": "icon-syss",
        //                "url": "MileageCollect.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
        //        if (arraycontains(rolesarray, '27')) {

        //            var stationmenu = { "menuid": "12",
        //                "menuname": "启动熄火查询",
        //                "icon": "icon-syss",
        //                "url": "StartFlameout.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
        //        if (arraycontains(rolesarray, '28')) {

        //            var stationmenu = { "menuid": "13",
        //                "menuname": "行驶速度分析",
        //                "icon": "icon-syss",
        //                "url": "SearhSpeed.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
        //        if (arraycontains(rolesarray, '29')) {

        //            var stationmenu = { "menuid": "14",
        //                "menuname": "停车统计",
        //                "icon": "icon-syss",
        //                "url": "ParkTotal.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
        //        if (arraycontains(rolesarray, '30')) {

        //            var stationmenu = { "menuid": "15",
        //                "menuname": "超速统计",
        //                "icon": "icon-syss",
        //                "url": "MileageSpeed.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
        //        if (arraycontains(rolesarray, '35')) {

        //            var stationmenu = { "menuid": "16",
        //                "menuname": "用户登录统计",
        //                "icon": "icon-syss",
        //                "url": "CountLogin.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
        //        if (arraycontains(rolesarray, '36')) {

        //            var stationmenu = { "menuid": "17",
        //                "menuname": "未上线车辆统计",
        //                "icon": "icon-syss",
        //                "url": "CarOnLine.htm"
        //            };
        //            _menus.menus[0].menus.push(stationmenu);
        //        }
    }

}
var _menus = {
    "menus": [
        {
            "menuid": "1",
            "icon": "icon-sys",
            "menuname": "指令下发",
            "menus": []
        }
    //            ,
    //             {
    //                 "menuid": "2",
    //                 "icon": "icon-sys",
    //                 "menuname": "统计设置",
    //                 "menus": []
    //             }
    ]
}