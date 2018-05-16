using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;


namespace SysService
{
    public class LogType
    {
        public enum LogTypeEnum
        {
            [Description("未知操作:excepetion")]
            excepetion = 0,
            [Description("用户登录:sys-user-login")]
            sysuserlogin = 1,
            [Description("自动登录:sys-user-autologin")]
            sysuserautologin = 2,
            [Description("修改密码:sys-user-updpwd")]
            sysuserupdpwd = 3,
            [Description("退出系统:sys-user-logout")]
            sysuserlogout = 4,
            [Description("获取权限车辆:sys-user-getallcar")]
            sysusergetallcar = 5,
            //[Description("心跳:sys-user-heart")]
            //sysuserheart = 6,
            [Description("获取最新定位信息:sys-usercar-getlasttrack")]
            sysusercargetlasttrack = 6,
            [Description("定位数据导出:sys-usercar-exportlasttrack")]
            sysusercarexportlasttrack = 7,
            [Description("轨迹回放-模糊查车:track-selectcars")]
            trackselectcars = 8,
            [Description("轨迹回放-查询轨迹:track-gettracks")]
            trackgettracks = 9,
            [Description("轨迹回放-导出轨迹:track-exporttracks")]
            trackexporttracks = 10,
            [Description("启动熄火查询:sys-startflameout-search")]
            sysstartflameoutsearch = 11,
            [Description("启动熄火查询导出:sys-startflameout-search-output")]
            sysstartflameoutsearchoutput = 12,
            [Description("警情统计查询:sta-mileagestatus-search")]
            stamileagestatussearch = 13,
            [Description("警情统计查询导出:sta-mileagestatus-search-output")]
            stamileagestatussearchoutput = 14,
            [Description("警情详细查询:sta-mileagestatus-info")]
            stamileagestatusinfo = 15,
            [Description("警情详细查询导出:sta-mileagestatus-info-output")]
            stamileagestatusinfooutput = 16,
            [Description("里程统计查询:sta-mileagecollect-search")]
            stamileagecollectsearch = 17,
            [Description("里程统计查询导出:sta-mileagecollect-search-output")]
            stamileagecollectsearchoutput = 18,
            [Description("里程详细查询:sta-mileagecollect-info")]
            stamileagecollectinfo = 19,
            [Description("里程详细查询导出:sta-mileagecollect-info-output")]
            stamileagecollectinfooutput = 20,
            [Description("停车统计查询:sta-parktotal-search")]
            staparktotalsearch = 21,
            [Description("停车统计查询导出:sta-parktotal-search-output")]
            staparktotalsearchoutput = 22,
            [Description("停车统计详细:sta-parktotal-info")]
            staparktotalinfo = 23,
            [Description("停车统计详细导出:sta-parktotal-info-output")]
            staparktotalinfooutput = 24,
            [Description("超速统计查询:sta-mileagespeed-search")]
            stamileagespeedsearch = 25,
            [Description("超速统计查询导出:sta-mileagespeed-search-output")]
            stamileagespeedsearchoutput = 26,
            [Description("超速统计详细:sta-mileagespeed-info")]
            stamileagespeedinfo = 27,
            [Description("超速统计详细导出:sta-mileagespeed-info-out")]
            stamileagespeedinfoout = 28,
            [Description("用户登录统计查询:sta-countlogin-search")]
            stacountloginsearch = 29,
            [Description("用户登录统计查询导出:sta-countlogin-search-out")]
            stacountloginsearchout = 30,
            [Description("未上线车辆统计查询:sta-caronline-search")]
            stacaronlinesearch = 31,
            [Description("未上线车辆统计查询导出:sta-caronline-search-output")]
            stacaronlinesearchoutput = 32,
            [Description("行驶速度分析查询:sta-searhspeed-search")]
            stasearhspeedsearch = 33,
            [Description("行驶速度分析导出:sta-searhspeed-search-output")]
            stasearhspeedsearchoutput = 34,
            [Description("车辆运行统计查询:sta-operationstatistics-search")]
            staoperationstatisticssearch = 35,
            [Description("车辆运行统计数据导出:sta-operationstatistics-search-output")]
            staoperationstatisticssearchoutput = 36,
            [Description("指令下发-设置终端最大速度:order-send-maxspeed")]
            ordersendmaxspeed = 37,
            [Description("指令下发-车辆呼转:order-send-calltransfer")]
            ordersendcalltransfer = 38,
            [Description("指令下发-车辆拍照:order-send-imagedown")]
            ordersendimagedown = 39,
            [Description("指令下发-行驶记录仪数据采集:order-send-recordcollection")]
            ordersendrecordcollection = 40,
            [Description("指令下发-车辆点名:order-send-positionsearch")]
            ordersendpositionsearch = 41,
            [Description("获取指令接收结果:order-receive-getall")]
            orderreceivegetall = 42,
            [Description("获取最新照片:order-receive-getphoto")]
            orderreceivegetphoto = 43,
            [Description("历史照片查询:sta-historyphotos-search")]
            stahistoryphotossearch = 44,
            [Description("单车历史照片查询:sta-historyphotos-onecar")]
            stahistoryphotosonecar = 45,
            [Description("异常下线统计：sta-abnormaloffline-search")]
            staabnormalofflinesearch = 46,
            [Description("异常下线统计数据导出：sta-abnormaloffline-search-output")]
            staabnormalofflinesearchoutput = 47,
            [Description("车辆离线统计：sta-offlineRA-search")]
            staofflineRAsearch = 48,
            [Description("车辆离线统计数据导出：sta-offlineRA-search-output")]
            staofflineRAsearchoutput = 49,
            [Description("车辆离线统计明细：sta-offlineRA-search-info")]
            staofflineRAsearchinfo = 50,
            [Description("车辆离线统计明细：sta-offlineRA-search-info-output")]
            staofflineRAsearchinfooutput = 51,
            [Description("指令下发-行驶记录仪设置:order-send-recordcmddown")]
            ordersendrecordcmddown = 52,
            [Description("信号量统计:sta-signal-search")]
            stasignalsearch = 53,
            [Description("信号量统计数据导出:sta-signal-search-output")]
            stasignalsearchoutput = 54,
            [Description("信号量统计明细:sta-signal-search-info")]
            stasignalsearchinfo = 55,
            [Description("信号量统计明细数据导出:sta-signal-search-info-output")]
            stasignalsearchinfooutput = 56,
            [Description("销贷业务车辆锁定设置:order-send-xiaodailockdown")]
            ordersendxiaodailockdown = 57,
            [Description("销贷业务车辆参数设置:order-send-xiaodaiparamdown")]
            ordersendxiaodaiparamdown = 58,
            [Description("A0EV手机端操作:a0ev")]
            A0EV = 59,
            [Description("销贷业务-车辆查询:xiaodai-car-query")]
            xiaodaicarquery = 60,
            [Description("定位监控-车辆信息查询:monitor-car-detailquery")]
            monitorcardetailquery = 61,
            [Description("App手机端操作")]
            AppWebService = 62,


            [Description("获取所有角色:sys-role-getallrole")]
            sysrolegetallrole = 63,
            [Description("获取所有组:sys-group-getallgroup")]
            sysgroupgetallgroup = 64,
            [Description("获取所有角色和组:sys-getall-rolesandgroups")]
            sysgetallrolesandgroups = 65,

            [Description("用户查询:sys-user-query")]
            sysuserquery = 66,
            [Description("用户新增:sys-user-add")]
            sysuseradd = 67,
            [Description("用户修改:sys-user-edit")]
            sysuseredit = 68,
            [Description("用户删除:sys-user-delete")]
            sysuserdelete = 69,
            [Description("用户列表导出:sys-user-export")]
            sysuserexport = 70,

            [Description("功能查询:sys-fun-query")]
            sysfunquery = 71,
            [Description("功能新增:sys-fun-add")]
            sysfunadd = 72,
            [Description("功能修改:sys-fun-edit")]
            sysfunedit = 73,
            [Description("功能删除:sys-fun-delete")]
            sysfundelete = 74,
            [Description("功能列表导出:sys-fun-export")]
            sysfunexport = 75,
            [Description("查询单个用户:sys-user-getbyid")]
            sysusergetbyid = 76,
            [Description("查询单个功能:sys-fun-getbyid")]
            sysfungetbyid = 77,

            [Description("查询单个角色:sys-role-getbyid")]
            sysrolegetbyid = 78,
            [Description("查询角色已绑定功能信息:sys-bindfun-getbyrid")]
            sysbindfungetbyrid = 79,
            [Description("新增角色信息:sys-role-insert")]
            sysroleinsert = 80,
            [Description("维护角色功能关联关系:sys-role-updaterolefun")]
            sysroleupdaterolefun = 81,

            [Description("更新用户角色关联关系:sys-user-updateuserrole")]
            sysroleupdateuserrole = 82,
            [Description("更新用户组关联关系:sys-user-updateusergroup")]
            sysroleupdateusergroup = 83,

            [Description("更新角色信息:sys-role-update")]
            sysroleupdate = 84,
            [Description("删除角色信息:sys-role-delete")]
            sysroledelete = 85,
            [Description("判断角色是否可以删除:sys-role-judgedeleteenable")]
            sysrolejudgedeleteenable = 86,
            [Description("导出角色信息:sys-role-export")]
            sysroleexport = 87,
            [Description("维护功能角色关联关系:sys-role-updatefunrole")]
            sysroleupdatefunrole = 88,
            [Description("查询单个组:sys-group-getbyid")]
            sysgroupgetbyid = 89,
            [Description("查询组已绑定的车辆信息:sys-bindcar-getbygid")]
            sysbindcargetbygid = 90,
            [Description("查询组尚未绑定的车辆信息:sys-unbindcar-getbygid")]
            sysunbindcargetbygid = 91,
            [Description("新增组信息:sys-group-insert")]
            sysgroupinsert = 92,
            [Description("更新组信息:sys-group-update")]
            sysgroupupdate = 93,
            [Description("维护组与车辆关联关系:sys-group-updategroupcar")]
            sysgroupupdategroupcar = 94,
            [Description("判断组是否可以删除:sys-group-judgedeleteenable")]
            sysgroupjudgedeleteenable = 95,
            [Description("删除组信息:sys-group-delete")]
            sysgroupdelete = 96,
            [Description("导出组信息:sys-group-export")]
            sysgroupexport = 97,
            [Description("批量导入组与车辆关系:sys-group-importrelation")]
            sysgroupimportrelation = 98,
            [Description("获取所有功能:sys-fun-getallfun")]
            sysfungetallfun = 99,
            [Description("获取所有应用类型:sys-apptype-getallapptype")]
            sysapptypegetallapptype = 100,

            [Description("物流统计-配送车辆查询:sys-distributioncarsta-search")]
            sysdistributioncarstasearch = 101,
            [Description("物流统计-配送车辆导出:sys-distributioncarsta-output")]
            sysdistributioncarstaoutput = 102,
            [Description("电子围栏-获取列表:fence-getlist")]
            fencegetlist = 103,
            [Description("电子围栏-维护:fence-addedit")]
            fenceaddedit = 104,
            [Description("电子围栏-删除:fence-delete")]
            fencedelete = 105,
            [Description("电子围栏-获取关联车辆:fence-getfencecar")]
            fencegetfencecar = 106,
            [Description("电子围栏-设置关联车辆:fence-setfencecar")]
            fencesetfencecar = 107,
            [Description("柳特驾驶行为分析:driving-analysis")]
            drivinganalysis = 108,
            [Description("柳特能耗分析-总表查询:energy-analysis-list")]
            energyanalysislist = 109,
            [Description("柳特能耗分析-明细查询:energy-analysis-detail")]
            energyanalysisdetail = 110,
            [Description("柳特能耗分析-总表导出:energy-analysis-listexport")]
            energyanalysislistexport = 111,
            [Description("柳特能耗分析-明细导出:energy-analysis-detailexport")]
            energyanalysisdetailexport = 112,
            [Description("电子围栏-电子围栏报警统计-列表:fence-alarm-sta-list")]
            fencealarmstalist = 113,
            [Description("电子围栏-电子围栏报警统计-明细:fence-alarm-sta-detail")]
            fencealarmstadetail = 114,
            [Description("电子围栏-电子围栏报警统计-列表导出:fence-alarm-sta-listexport")]
            fencealarmstalistexport = 115,
            [Description("电子围栏-电子围栏报警统计-明细导出:fence-alarm-sta-detailexport")]
            fencealarmstadetailexport = 116,
            [Description("驻车提示系统统计分析-故障码统计:sys-faultstatistic-search")]
            sysfaultstatisticsearch = 117,
            [Description("驻车提示系统统计分析-故障码统计结果导出:sys-faultstatistic-export")]
            sysfaultstatisticexport = 118,
            [Description("批量删除组与车辆关系:sys-group-deleterelation")]
            sysgroupdeleterelation = 119,

            [Description("车辆保养提醒统计:sta-carmaintenanceremind")]
            stacarmaintenanceremind = 120,
            [Description("录入车辆保养记录:sys-carmaintenancerecord-insert")]
            syscarmaintenancerecordinsert = 121,
            [Description("下发车辆保养通知:sys-carmaintenancenotice-down")]
            syscarmaintenancenoticedown = 122,
            [Description("活动消息推送:sys-carinfomationsend")]
            syscarinfomationsend = 123,

            [Description("指令下发-设置设备上传时间间隔:order-send-timeinterval")]
            ordersendtimeinterval = 124,

            [Description("车辆管理-列表查询:car-list-search")]
            carlistsearch = 125,
            [Description("车辆管理-单车信息查询:car-one-search")]
            caronesearch = 126,
            [Description("车辆管理-获取所有车籍地信息（省市区）:car-placelevel-get")]
            carplacelevelget = 127,
            [Description("车辆管理-编辑保存:car-edit-save")]
            careditsave = 128,
            [Description("车辆月上线率统计:car-onlinerate-month")]
            caronlineratemonth = 129,
            [Description("车辆月上线率统计结果导出:car-onlinerate-month-export")]
            caronlineratemonthexport = 130,
            [Description("车辆年上线率统计:car-onlinerate-year")]
            caronlinerateyear = 131,
            [Description("车辆年上线率统计结果导出:car-onlinerate-year-export")]
            caronlinerateyearexport = 132,


            [Description("地图标注-列表查询:marker-getlist")]
            markergetlist = 133,
            [Description("地图标注-添加标注:markersetting-addmarker")]
            markersettingaddmarker = 134,
            [Description("地图标注-删除标注:marker-delete")]
            markerdeletemarker = 135,
            [Description("试乘试驾日分析表-列表查询:DayTestDriveAnalyse-getlist")]
            daytestdriveanalysgetlist = 136,
            [Description("试乘试驾日分析表-经销商名称关联:DayTestDriveAnalyse-getSDealersName")]
            daytestdriveanalysgetSDealersName = 137,
            [Description("试乘试驾月分析表-列表查询:MonthTestDriveAnalyse-getlist")]
            monthtestdriveanalysgetlist = 138,

            [Description("X80-获取登录的经销商信息:dealer-getbyuid")]
            dealergetbyuid = 140,
            [Description("活动区域-列表查询:activeregion-getlist")]
            activeregiongetlist = 141,
            [Description("活动区域-添加:activeregion-add")]
            activeregionadd = 142,
            [Description("活动区域-编辑:activeregion-edit")]
            activeregionedit = 143,
            [Description("活动区域-删除:activeregion-delete")]
            activeregiondelete = 144,
            [Description("活动区域-匹配车辆获取:activeregion-getrelcar")]
            activeregiongetrelcar = 145,
            [Description("活动区域-匹配车辆设置:activeregion-setrelcar")]
            activeregionsetrelcar = 146,

            [Description("试乘试驾线路-列表查询:driveline-getlist")]
            drivelinegetlist = 147,
            [Description("试乘试驾线路-添加:driveline-add")]
            drivelineadd = 148,
            [Description("试乘试驾线路-编辑:driveline-edit")]
            drivelineedit = 149,
            [Description("试乘试驾线路-删除:driveline-delete")]
            drivelinedelete = 150,
            [Description("试乘试驾线路-匹配车辆获取:driveline-getrelcar")]
            drivelinegetrelcar = 151,
            [Description("试乘试驾线路-匹配车辆设置:driveline-setrelcar")]
            drivelinesetrelcar = 152,
            [Description("试乘试驾线路-获取线路点坐标:driveline-getmarker")]
            drivelinegetmarker = 153,

            [Description("试乘试驾查询：sys-testdrive-search")]
            systestdrivesearch = 154,
            [Description("偏离试驾路线查询：sys-outdrive-search")]
            sysoutdrivesearch = 155,
            [Description("驶出活动范围查询：sys-outarea-search")]
            sysoutareasearch = 156,
            [Description("试乘试驾查询-导出：sys-testdrive-search-output")]
            systestdrivesearchoutput = 157,
            [Description("偏离试驾路线查询-导出：sys-outdrive-search-output")]
            sysoutdrivesearchoutput = 158,
            [Description("驶出活动区域查询-导出：sys-outarea-search-output")]
            sysoutareasearchoutput = 159,

            [Description("试乘试驾日分析表-导出:DayTestDriveAnalyse-export")]
            daytestdriveanalyseexport = 160,
            [Description("试乘试驾月分析表-导出:MonthTestDriveAnalyse-export")]
            monthtestdriveanalyseexport = 161,

            [Description("销贷-查询放款机构:al-org-get")]
            alorgget = 162,
            [Description("销贷-查询字典项:al-dict-get")]
            aldictget = 163,
            [Description("销贷-查询终端型号:al-Tertype-get")]
            altertypeget = 164,
            [Description("销贷-查询发动机类型al-EnergyType-get")]
            alenergytypeget = 165,
            [Description("销贷-查询锁车解锁数据:al-lock-search")]
            allocksearch = 166,
            [Description("销贷-导出锁车解锁数据:al-lock-export")]
            allockexport = 167,
            [Description("销贷-申请锁车解锁:al-lock-apply")]
            allockapply = 168,
            [Description("销贷-审批锁车解锁:al-lock-set")]
            allockset = 169,
            [Description("销贷-查询参数设置一览数据:al-parameter-search")]
            alparametersearch = 170,
            [Description("销贷-导出参数设置一览数据:al-parameter-export")]
            alparamterexport = 171,
            [Description("销贷-参数设置:al-parameter-set")]
            alparameterset = 172,
            [Description("al-parameter-active:销贷-激活设置")]
            alparameteractive = 173
        }


        /// <summary>
        /// 让枚举中使用特殊字符。
        /// 原理：让枚举中的值都拥有Attribute属性，然后通过在属性里设置特殊字符。从而返回属性的ToString()
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetEnumName(Enum en)
        {
            Type temType = en.GetType();
            MemberInfo[] memberInfos = temType.GetMember(en.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                object[] objs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs != null && objs.Length > 0)
                {
                    return ((DescriptionAttribute)objs[0]).Description;
                }
            }
            return en.ToString();
        }
    }
}