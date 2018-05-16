
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

            menulist.push('<li><div style="font-size:16px" onclick="menuClick(\'' + o.url + '\',\'' + o.menuname + '\')">');
            menulist.push('<span class="icon ' + o.icon + '" >&nbsp;</span>' + o.menuname);
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
        var applymenu = { "menuid": "10",
            "menuname": "锁车/解锁申请",
            "icon": "icon-syss",
            "url": "../LockApplication/LockApplication.htm"
        };
        var lockmenu = { "menuid": "11",
            "menuname": "锁车/解锁",
            "icon": "icon-syss",
            "url": "../LockApplication/LockApplication.htm"
        };

        var parammenu = { "menuid": "12",
            "menuname": "参数设置",
            "icon": "icon-syss",
            "url": "../ParameterSetting/ParameterSetting.htm"
        };

        var activemenu = { "menuid": "13",
            "menuname": "激活服务设置",
            "icon": "icon-syss",
            "url": "../ParameterSetting/ParameterSetting.htm"
        };
        var menu1 = { "menuid": "14",
            "menuname": "信息维护",
            "icon": "icon-syss",
            "url": "../AnnulBase/AnnulBase.htm"
        };
        var menu2 = { "menuid": "15",
            "menuname": "系统日志",
            "icon": "icon-syss",
            "url": "../AnnulLog/AnnulLog.htm"
        };
        _menus.menus[0].menus.push(menu1);
        // 经销商菜单
        if (userinfo.RID == 25) {
            _menus.menus[0].menus.push(applymenu);
            _menus.menus[0].menus.push(parammenu);//需求变更，参数设置模块隐藏
        }
        // 启明菜单
        else if (userinfo.RID == 26) {
            _menus.menus[0].menus.push(activemenu);
            _menus.menus[0].menus.push(lockmenu);
        }
        _menus.menus[0].menus.push(menu2);

    }

}
var _menus = {
    "menus": [
        {
            "menuid": "1",
            "icon": "icon-sys",
            "menuname": "销贷业务",
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