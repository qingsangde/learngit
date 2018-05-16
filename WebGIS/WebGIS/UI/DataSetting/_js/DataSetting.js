﻿
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

        var menu1 = { "menuid": "11",
            "menuname": "地图标注设置",
            "icon": "icon-syss",
            "url": "../MapMarkerSetting/markersetting.htm"
        };
        _menus.menus[0].menus.push(menu1);

        var menu2 = { "menuid": "12",
            "menuname": "活动区域维护",
            "icon": "icon-syss",
            "url": "../ActiveRegion/activeregion.htm"
        };
        _menus.menus[0].menus.push(menu2);

        var menu3 = { "menuid": "13",
            "menuname": "活动区域匹配车辆",
            "icon": "icon-syss",
            "url": "../ActiveRegion/activeregioncar.htm"
        };
        _menus.menus[0].menus.push(menu3);


        var menu4 = { "menuid": "14",
            "menuname": "试乘试驾路线维护",
            "icon": "icon-syss",
            "url": "../DriveLine/driveline.htm"
        };
        _menus.menus[0].menus.push(menu4);

        var menu5 = { "menuid": "15",
            "menuname": "试乘试驾匹配车辆",
            "icon": "icon-syss",
            "url": "../DriveLine/driverline_car.htm"
        };
        _menus.menus[0].menus.push(menu5);
    }
}
var _menus = {
    "menus": [
        {
            "menuid": "1",
            "icon": "icon-sys",
            "menuname": "数据设置",
            "menus": []
        }
    ]
}