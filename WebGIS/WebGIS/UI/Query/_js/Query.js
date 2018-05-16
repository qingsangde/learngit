
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
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
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
var _menus = {
    "menus": []
}

function SetMenusByRole() {
    var roles = "";
    var userinfo = GetUserInfo();
    var sysflag = getCookie("sysflag");
    if (userinfo) {
        roles = userinfo.A_Name;
        var rolesarray = roles.split(',');
        var returnflag = GetQueryString("returnflag");
        var grrsysflag = GetQueryString("key");

        var menu = {
            "menuid": "1",
            "icon": "icon-sys",
            "menuname": "统计分析",
            "menus": []
        };

        var menu00 = { "menuid": "00",
            "menuname": "启动熄火查询",
            "icon": "icon-syss",
            "url": "StartFlameout.htm"
        };
        

        if (userinfo.RID == 23) {
            menu.menus.push(menu00);
        }

        var menu01 = { "menuid": "01",
            "menuname": "试乘试驾查询",
            "icon": "icon-syss",
            "url": "TestDrive.htm"
        };
     
        if (userinfo.RID == 23) {
            menu.menus.push(menu01);
        }

        var menu02 = { "menuid": "02",
            "menuname": "偏离试驾路线查询",
            "icon": "icon-syss",
            "url": "OutDrive.htm"
        };
       
        if (userinfo.RID == 23) {
            menu.menus.push(menu02);
        }

        var menu03 = { "menuid": "03",
            "menuname": "驶出活动区域查询",
            "icon": "icon-syss",
            "url": "OutArea.htm"
        };
       
        if (userinfo.RID == 23) {
            menu.menus.push(menu03);
        }

        var menu04 = { "menuid": "04",
            "menuname": "试乘试驾分析表-分日",
            "icon": "icon-syss",
            "url": "DayTestDriveAnalyseNew.htm"
        };
        menu.menus.push(menu04);

        var menu05 = { "menuid": "05",
            "menuname": "试乘试驾分析表-分月",
            "icon": "icon-syss",
            "url": "MonthTestDriveAnalyseNew.htm"
        };
        menu.menus.push(menu05);


        var menu06 = { "menuid": "06",
            "menuname": "试乘试驾分析表-全国",
            "icon": "icon-syss",
            "url": "ComTestDriveAnalyse.htm"
        };

        if (userinfo.RID == 22) {
            menu.menus.push(menu06);
        }
        



        _menus.menus.push(menu);
    }

}

