using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using CommLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SysService;
using WebGIS.Service;
using WebGIS.sys;

//using Newtonsoft.Json.Linq;

namespace WebGIS
{
    /// <summary>
    /// Service 的摘要说明
    /// </summary>
    public class HttpService : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            SessionModel sm = new SessionModel();
            string token = "";
            string sid = "";
            string sysuid = "0";
            string sysflag = "";
            string onecaruser = "0";
            SysService.LogModel logmodel = new SysService.LogModel();
            //写入操作日志
            sys_log slog = new sys_log();
            ResponseResult Result = null;
            context.Response.ContentType = "text/plain";
            try
            {

                Dictionary<string, string> inparams = GetParams(context);
                if (inparams == null || inparams.Count == 0)
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "参数错误！参数格式为JSON格式数据，详见服务接口文档。", null);
                }
                else
                {
                    if (BaseParamsCheck(inparams, out Result))
                    {
                        if (inparams["sysflag"].Equals("A0EV"))
                        {
                        }
                        //inparams["sysflag"] = WebProc.GetHttpSysflagKey(inparams["sysflag"]);
                        sysflag = inparams["sysflag"];
                        sid = inparams["sid"];
                        if (inparams.Keys.Contains("sysuid"))
                        {
                            sysuid = inparams["sysuid"];

                        }


                        if (inparams.Keys.Contains("token"))
                        {
                            token = inparams["token"];

                            sm = SessionManager.GetSession(token);
                            if (sm != null)
                            {
                                if (sm.onecaruser)
                                {
                                    onecaruser = "1";
                                }
                                else
                                {
                                    onecaruser = "0";
                                }
                            }
                        }
                        //A0EV服务接口
                        if (inparams["sysflag"].Equals("A0EV"))
                        {
                            inparams["sysflag"] = WebProc.GetHttpSysflagKey(inparams["sysflag"]);
                            //#region A0EV手机端服务接口
                            ////A0EVProess a0ev = new A0EVProess();
                            //logmodel.LogType = (int)LogType.LogTypeEnum.A0EV;
                            ////A0EV系统挂载与长春企业平台
                            ////inparams["sysflag"] = "YQWL";
                            //switch (sid)
                            //{
                            //    case "aoev-car-state":
                            //        Result = a0ev.GetCarState(inparams);

                            //        break;
                            //    case "aoev-car-wakeup":
                            //        Result = a0ev.SendCarWakeup(inparams);

                            //        break;
                            //    case "aoev-charging-send":
                            //        Result = a0ev.ChargingSend(inparams);

                            //        break;
                            //    case "aoev-charging-reserve":
                            //        Result = a0ev.ChargingReserve(inparams);

                            //        break;
                            //    case "aoev-charging-reserveview":
                            //        Result = a0ev.ChargingReserveView(inparams);

                            //        break;
                            //    case "aoev-charging-reservesend":
                            //        Result = a0ev.ChargingReserveSend(inparams);

                            //        break;
                            //    case "aoev-charging-pause":
                            //        Result = a0ev.ChargingPause(inparams);

                            //        break;
                            //    case "aoev-aircondition-switch":
                            //        Result = a0ev.AirconditionSwitch(inparams);
                            //        break;
                            //    case "aoev-car-usercars":
                            //        Result = a0ev.GetUserCars(inparams);

                            //        break;
                            //    case "aoev-transprot-send":
                            //        Result = a0ev.TransmissionProtocol(inparams);

                            //        break;
                            //}
                            //#endregion
                        }
                        else
                        {
                            inparams["sysflag"] = WebProc.GetHttpSysflagKey(inparams["sysflag"]);
                            #region WebGIS2.0
                            if (sid.Equals("sys-user-login"))
                            {
                                //根据服务sid，确定日志类型和内容，以下分支同，不再做注释
                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysuserlogin;
                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysuserlogin);

                                string CheckCode = null;
                                if (context.Session["CheckCode"] != null)
                                {
                                    CheckCode = context.Session["CheckCode"].ToString();
                                }
                                sys_user sumodel = new sys_user();
                                Result = sumodel.login(inparams, CheckCode);
                                if (Result.state == (int)ResState.Success)
                                {
                                    sys_user_login_res res = (sys_user_login_res)Result.result;
                                    sysuid = res.UID;//如果是用户登录操作，则登录验证完，才知道用户id，此处赋值，供保存操作日志用
                                    onecaruser = res.OneCarUser;
                                }

                            }
                            else if (sid.Equals("sys-user-autologin"))
                            {
                                //根据服务sid，确定日志类型和内容，以下分支同，不再做注释
                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysuserautologin;
                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysuserautologin);

                                sys_user sumodel = new sys_user();
                                Result = sumodel.userLogin(inparams);
                                if (Result.state == (int)ResState.Success)
                                {
                                    sys_user_login_res res = (sys_user_login_res)Result.result;
                                    sysuid = res.UID;//如果是用户登录操作，则登录验证完，才知道用户id，此处赋值，供保存操作日志用
                                    onecaruser = res.OneCarUser;
                                }

                            }
                            else if (sid.Equals("sys-user-heart"))
                            {
                                if (SessionManager.IsActive(token))
                                {
                                    Result = new ResponseResult(ResState.Success, "心跳成功", "");
                                }
                                else
                                {
                                    Result = new ResponseResult(ResState.NotLogin, "登录超时", "");
                                }
                            }
                            else if (sid.Equals("sys-user-logout"))
                            {
                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysuserlogout;
                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysuserlogout);

                                SessionManager.CloseSession(token);
                                sys_user sumodel = new sys_user();
                                Result = sumodel.userLogout(inparams);


                            }
                            //else if (sid.Equals("sys-user-getallcar"))
                            //{
                            //    sys_car scar = new sys_car();
                            //    Result = scar.GetOptionalCarsNew(inparams);

                            //    logmodel.LogType = (int)LogType.LogTypeEnum.sysusergetallcar;
                            //    logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysusergetallcar);
                            //}
                            else
                            {
                                //token = inparams["token"];
                                // 依据token 进行验证登陆和鉴权操作
                                bool isActive = SessionManager.IsActiveAndReset(token);

                                if (isActive)//这里做了更改！！
                                {
                                    //鉴权操作
                                    bool isAuth = SessionManager.Authentication(token, sid);
                                    if (isAuth)//这里做了更改！！
                                    {
                                        switch (sid)
                                        {
                                            case "sys-user-getallcar":

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysusergetallcar;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysusergetallcar);

                                                sys_car scar = new sys_car();
                                                Result = scar.GetOptionalCarsNew(inparams);

                                                break;
                                            case "sys-user-updpwd":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysuserupdpwd;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysuserupdpwd);

                                                sys_user sumodel = new sys_user();
                                                Result = sumodel.userChgPwd(inparams);

                                                break;
                                            case "sys-startflameout-search"://启动熄火查询

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysstartflameoutsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysstartflameoutsearch);

                                                sta_startflameout sys_sta = new sta_startflameout();
                                                Result = sys_sta.getSFCar(inparams);

                                                break;
                                            case "sys-startflameout-search-output"://启动熄火查询导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysstartflameoutsearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysstartflameoutsearchoutput);

                                                sta_startflameout sys_staout = new sta_startflameout();
                                                Result = sys_staout.getSFCaroutput(inparams);

                                                break;
                                            case "sta-mileagestatus-search"://警情统计查询

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagestatussearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagestatussearch);

                                                sta_mileagestatus sta_mile_sea = new sta_mileagestatus();
                                                Result = sta_mile_sea.getMilStatus(inparams);

                                                break;
                                            case "sta-mileagestatus-search-output"://警情统计查询导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagestatussearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagestatussearchoutput);

                                                sta_mileagestatus sta_mile_seaout = new sta_mileagestatus();
                                                Result = sta_mile_seaout.getMilStatusoutput(inparams);

                                                break;
                                            case "sta-mileagestatus-info"://警情详细

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagestatusinfo;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagestatusinfo);

                                                sta_mileagestatus sta_mile_in = new sta_mileagestatus();
                                                Result = sta_mile_in.getMilStatusInfo(inparams);

                                                break;
                                            case "sta-mileagestatus-info-output"://警情详细导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagestatusinfooutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagestatusinfooutput);

                                                sta_mileagestatus sta_mile_in_output = new sta_mileagestatus();
                                                Result = sta_mile_in_output.getMilStatusInfooutput(inparams);

                                                break;
                                            case "sta-mileagecollect-search"://里程统计查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagecollectsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagecollectsearch);

                                                sta_mileagecollect sta_milecollect = new sta_mileagecollect();
                                                Result = sta_milecollect.getMilCollect(inparams);

                                                break;
                                            case "sta-mileagecollect-info"://里程详细信息查询

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagecollectinfo;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagecollectinfo);

                                                sta_mileagecollect sta_milecollect_info = new sta_mileagecollect();
                                                Result = sta_milecollect_info.getCollectInfo(inparams);

                                                break;
                                            case "sta-mileagecollect-search-output"://里程统计查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagecollectsearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagecollectsearchoutput);

                                                sta_mileagecollect sta_milecollect_out = new sta_mileagecollect();
                                                Result = sta_milecollect_out.getMilCollectoutput(inparams);

                                                break;
                                            case "sta-mileagecollect-info-output"://里程详细信息查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagecollectinfooutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagecollectinfooutput);

                                                sta_mileagecollect sta_milecollectin_out = new sta_mileagecollect();
                                                Result = sta_milecollectin_out.getMilCollectInfooutput(inparams);

                                                break;
                                            case "sta-parktotal-search"://停车统计查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staparktotalsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.staparktotalsearch);

                                                sta_parktotal sat_par = new sta_parktotal();
                                                Result = sat_par.getParkTotal(inparams);

                                                break;
                                            case "sta-parktotal-search-output"://停车统计查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staparktotalsearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.staparktotalsearchoutput);

                                                sta_parktotal sat_parout = new sta_parktotal();
                                                Result = sat_parout.getParkTotaloutput(inparams);

                                                break;
                                            case "sta-parktotal-info"://停车统计详细
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staparktotalinfo;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.staparktotalinfo);

                                                sta_parktotal sat_parin = new sta_parktotal();
                                                Result = sat_parin.getParkTotalInfo(inparams);

                                                break;
                                            case "sta-parktotal-info-output"://停车统计详细导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staparktotalinfooutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.staparktotalinfooutput);

                                                sta_parktotal sat_parin_out = new sta_parktotal();
                                                Result = sat_parin_out.getParkTotalinfooutput(inparams);

                                                break;
                                            case "sta-mileagespeed-search"://超速统计查询

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagespeedsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagespeedsearch);

                                                sta_mileagespeed sat_milspeedsearch = new sta_mileagespeed();
                                                Result = sat_milspeedsearch.getMileageSpeed(inparams);

                                                break;
                                            case "sta-mileagespeed-search-output"://超速统计查询导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagespeedsearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagespeedsearchoutput);

                                                sta_mileagespeed sat_milspeedsearchout = new sta_mileagespeed();
                                                Result = sat_milspeedsearchout.getMileageSpeedoutput(inparams);

                                                break;
                                            case "sta-mileagespeed-info"://超速统计详细信息

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagespeedinfo;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagespeedinfo);

                                                sta_mileagespeed sat_milspeedinfo = new sta_mileagespeed();
                                                Result = sat_milspeedinfo.getMileageSpeedInfo(inparams);

                                                break;
                                            case "sta-mileagespeed-info-out"://超速统计详细信息导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stamileagespeedinfoout;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stamileagespeedinfoout);

                                                sta_mileagespeed sat_milspeedinfoout = new sta_mileagespeed();
                                                Result = sat_milspeedinfoout.getMileageSpeedInfoout(inparams);

                                                break;
                                            case "sta-countlogin-search"://用户登录统计查询

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stacountloginsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stacountloginsearch);

                                                sta_countlogin sat_clsearch = new sta_countlogin();
                                                Result = sat_clsearch.getCountLogin(inparams);

                                                break;
                                            case "sta-countlogin-search-out"://用户登录统计查询导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stacountloginsearchout;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stacountloginsearchout);

                                                sta_countlogin sat_clsearchout = new sta_countlogin();
                                                Result = sat_clsearchout.getCountLoginOut(inparams);

                                                break;
                                            case "sta-caronline-search"://未上线车辆统计查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stacaronlinesearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stacaronlinesearch);

                                                sta_caronline sat_carolsearch = new sta_caronline();
                                                Result = sat_carolsearch.getCarOnLine(inparams);

                                                break;
                                            case "sta-caronline-search-output"://未上线车辆统计查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stacaronlinesearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stacaronlinesearchoutput);

                                                sta_caronline sat_carolsearchout = new sta_caronline();
                                                Result = sat_carolsearchout.getCarOnLineOut(inparams);

                                                break;
                                            case "sta-searhspeed-search"://行驶速度分析查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stasearhspeedsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stasearhspeedsearch);

                                                sta_searhspeed sat_sspeedsearch = new sta_searhspeed();
                                                Result = sat_sspeedsearch.getSearhSpeed(inparams);

                                                break;
                                            case "sta-searhspeed-search-output"://行驶速度分析查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stasearhspeedsearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stasearhspeedsearchoutput);

                                                sta_searhspeed sat_sspeedsearchout = new sta_searhspeed();
                                                Result = sat_sspeedsearchout.getSearhSpeedoutput(inparams);

                                                break;
                                            case "sta-operationstatistics-search"://车辆运行统计查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staoperationstatisticssearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.staoperationstatisticssearch);

                                                sta_operationstatistics sat_operationstatisticssearch = new sta_operationstatistics();
                                                Result = sat_operationstatisticssearch.getOperationStatistics(inparams);

                                                break;
                                            case "sta-operationstatistics-search-output"://车辆运行统计导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staoperationstatisticssearchoutput;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.staoperationstatisticssearchoutput);

                                                sta_operationstatistics sat_operationstatisticsout = new sta_operationstatistics();
                                                Result = sat_operationstatisticsout.getOStatisticsoutput(inparams);

                                                break;

                                            case "sys-usercar-getlasttrack":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysusercargetlasttrack;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysusercargetlasttrack);
                                                monitor syscar = new monitor();
                                                Result = syscar.getCarLastTrack(inparams);

                                                break;

                                            case "sys-usercar-exportlasttrack":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysusercarexportlasttrack;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.sysusercarexportlasttrack);
                                                monitor syscarexport = new monitor();
                                                //Result = syscarexport.EasyUIGrid2Excel(context, inparams);
                                                Result = syscarexport.getCarLastTrackOut(inparams);

                                                //Result = new ResponseResult(ResState.Success, "操作成功","");
                                                break;


                                            case "track-selectcars": //轨迹回放-模糊查车
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.trackselectcars;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackselectcars);
                                                SysService.CarInfo mycars = new SysService.CarInfo();
                                                Result = mycars.getCarNos(inparams);

                                                break;
                                            case "track-gettracks": //轨迹回放-查询轨迹
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.trackgettracks;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackgettracks);
                                                Track mytracks = new Track();
                                                Result = mytracks.getTracks(inparams);

                                                break;
                                            case "track-exporttracks": //轨迹回放-导出轨迹
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.trackexporttracks;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackexporttracks);
                                                Track mytracksex = new Track();
                                                Result = mytracksex.getTracksOutPut(inparams);

                                                break;


                                            //----add by kangp for 柳特驾驶行为分析------------2016-2-23------------                 
                                            case "driving-analysis": //柳特驾驶行为分析
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivinganalysis;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackexporttracks);
                                                DrivingAnalysis mytracksex1 = new DrivingAnalysis();
                                                Result = mytracksex1.getData(inparams);

                                                break;

                                            case "energy-analysis-list":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.energyanalysislist;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackexporttracks);
                                                EnergyAnalysis energyAnalysis = new EnergyAnalysis();
                                                Result = energyAnalysis.QueryList(inparams);

                                                break;

                                            case "energy-analysis-detail":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.energyanalysisdetail;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackexporttracks);
                                                EnergyAnalysis energyAnalysis1 = new EnergyAnalysis();
                                                Result = energyAnalysis1.QueryDetail(inparams);

                                                break;

                                            case "energy-analysis-listexport":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.energyanalysislistexport;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackexporttracks);
                                                EnergyAnalysis energyAnalysis2 = new EnergyAnalysis();
                                                Result = energyAnalysis2.ListExport(inparams);

                                                break;

                                            case "energy-analysis-detailexport":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.energyanalysisdetailexport;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.trackexporttracks);
                                                EnergyAnalysis energyAnalysis3 = new EnergyAnalysis();
                                                Result = energyAnalysis3.DetailExport(inparams);

                                                break;

                                            case "order-send-maxspeed"://指令下发-最大速度设置
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendmaxspeed;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendmaxspeed);
                                                TermParamDown osd = new TermParamDown();
                                                Result = osd.sendOverSpeedOrder(inparams);
                                                break;
                                            case "order-send-calltransfer"://指令下发-车辆呼转
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendcalltransfer;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendcalltransfer);
                                                TermParamDown osd0 = new TermParamDown();
                                                Result = osd0.sendCallTransferOrder(inparams);
                                                break;
                                            case "order-send-imagedown"://指令下发-车辆拍照
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendimagedown;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendimagedown);
                                                ImageRequestDown ird = new ImageRequestDown();
                                                Result = ird.sendImageDownOrder(inparams);
                                                break;
                                            case "order-send-timeinterval"://指令下发-设置设备上传时间间隔
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendtimeinterval;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendimagedown);
                                                TermParamDown osd1 = new TermParamDown();
                                                Result = osd1.sendTimeIntervalOrder(inparams);
                                                break;
                                            case "order-send-recordcollection"://指令下发-行驶记录仪数据采集
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendrecordcollection;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendrecordcollection) + "命令字：" + inparams["cmd"];
                                                DriveRecordCollectRequest drc = new DriveRecordCollectRequest();
                                                Result = drc.sendDriveRecordCollectOrder(inparams);
                                                break;
                                            case "order-send-recordcmddown"://指令下发-行驶记录仪设置
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendrecordcmddown;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendrecordcollection) + "命令字：" + inparams["cmd"];
                                                DriverRecordCMDDown drcd = new DriverRecordCMDDown();
                                                Result = drcd.sendDriveRecordCMDDownOrder(inparams);
                                                break;
                                            case "order-send-positionsearch"://指令下发-车辆点名
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendpositionsearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.ordersendpositionsearch);
                                                TermSearchRequestDown trd = new TermSearchRequestDown();
                                                Result = trd.sendTermSearchOrder(inparams);
                                                break;
                                            case "order-receive-getall":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.orderreceivegetall;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.orderreceivegetall);
                                                OrderResponse or = new OrderResponse();
                                                Result = or.QueryOrderResultOut(inparams);
                                                break;
                                            case "order-receive-getphoto":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.orderreceivegetphoto;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.orderreceivegetphoto);
                                                OrderPhotoResponse opr = new OrderPhotoResponse();
                                                Result = opr.GetLastPhoto(inparams);
                                                break;
                                            case "sta-historyphotos-search":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stahistoryphotossearch;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stahistoryphotossearch);
                                                sta_historyphoto hp = new sta_historyphoto();
                                                Result = hp.queryHistoryPhotos(inparams);
                                                break;
                                            case "sta-historyphotos-onecar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stahistoryphotosonecar;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.stahistoryphotosonecar);
                                                sta_historyphoto hponecar = new sta_historyphoto();
                                                Result = hponecar.queryOneCarHistoryPhoto(inparams);
                                                break;
                                            case "sta-abnormaloffline-search":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staabnormalofflinesearch;
                                                sta_abnormaloffline abnormaloffline = new sta_abnormaloffline();
                                                Result = abnormaloffline.getAbnormalOfflineStatistic(inparams);
                                                break;
                                            case "sta-abnormaloffline-search-output":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staabnormalofflinesearchoutput;
                                                sta_abnormaloffline abnormaloffline0 = new sta_abnormaloffline();
                                                Result = abnormaloffline0.getAbnormalOfflineStatisticsoutput(inparams);
                                                break;
                                            case "sta-offlineRA-search":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staofflineRAsearch;
                                                sta_offlineRA staofflineRA = new sta_offlineRA();
                                                Result = staofflineRA.getOfflineStatistic(inparams);
                                                break;
                                            case "sta-offlineRA-search-output":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staofflineRAsearchoutput;
                                                sta_offlineRA staofflineRA0 = new sta_offlineRA();
                                                Result = staofflineRA0.getOfflineStatisticOutPut(inparams);
                                                break;
                                            case "sta-offlineRA-search-info"://离线统计明细
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staofflineRAsearchinfo;
                                                sta_offlineRA staofflineRAinfo = new sta_offlineRA();
                                                Result = staofflineRAinfo.getOfflineStatisticDetail(inparams);
                                                break;
                                            case "sta-offlineRA-search-info-output"://离线统计明细
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.staofflineRAsearchinfooutput;
                                                sta_offlineRA staofflineRAinfoout = new sta_offlineRA();
                                                Result = staofflineRAinfoout.getOfflineStatisticDetailOutput(inparams);
                                                break;
                                            case "sta-signal-search"://信号量统计
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stasignalsearch;
                                                sta_signalStat signalStat = new sta_signalStat();
                                                Result = signalStat.getSignalStatistic(inparams);
                                                break;
                                            case "sta-signal-search-output"://信号量统计数据导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stasignalsearchoutput;
                                                sta_signalStat signalStatout = new sta_signalStat();
                                                Result = signalStatout.getSignalStatisticsoutput(inparams);
                                                break;
                                            case "sta-signal-search-info"://信号量统计明细
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stasignalsearchinfo;
                                                sta_signalStat signalStatinfo = new sta_signalStat();
                                                Result = signalStatinfo.getSignalInfo(inparams);
                                                break;
                                            case "sta-signal-search-info-output"://信号量统计明细数据导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stasignalsearchinfooutput;
                                                sta_signalStat signalStatinfoout = new sta_signalStat();
                                                Result = signalStatinfoout.getSignalInfoStatisticsoutput(inparams);
                                                break;
                                            case "order-send-xiaodailockdown"://消贷业务车辆锁定设置
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendxiaodailockdown;
                                                TransmissionProtocolDown tpd = new TransmissionProtocolDown();
                                                Result = tpd.sendXiaoDaiLockOrder(inparams);
                                                break;
                                            case "order-send-xiaodaivindown": //销贷业务激活/关闭发动机销贷功能
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendxiaodaiparamdown;
                                                TransmissionProtocolDown tpd2 = new TransmissionProtocolDown();
                                                Result = tpd2.sendXiaoDaiActivateOrder(inparams);
                                                break;
                                            case "order-send-xiaodaiparamdown"://消贷业务车辆参数设置
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.ordersendxiaodaiparamdown;
                                                TransmissionProtocolDown tpd1 = new TransmissionProtocolDown();
                                                Result = tpd1.sendXiaoDaiParamOrder(inparams);
                                                break;
                                            case "xiaodai-car-query"://消贷业务车辆查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.xiaodaicarquery;
                                                sys_car carquery = new sys_car();
                                                Result = carquery.getXiaoDaiCarSeach(inparams);
                                                break;
                                            case "monitor-car-detailquery"://定位监控-车辆详细信息查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.monitorcardetailquery;
                                                sys_car carquery0 = new sys_car();
                                                Result = carquery0.getCarDetailSearch(inparams);
                                                break;
                                            case "sys-distributioncarsta-search":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysdistributioncarstasearch;
                                                sta_distributioncarstat stadis = new sta_distributioncarstat();
                                                Result = stadis.GetDistributionCarsStat(inparams);
                                                break;
                                            case "sys-distributioncarsta-output":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysdistributioncarstaoutput;
                                                sta_distributioncarstat stadis0 = new sta_distributioncarstat();
                                                Result = stadis0.GetDistributionCarsStatOutput(inparams);
                                                break;

                                            case "fence-getlist":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencegetlist;
                                                Fence ff = new Fence();
                                                Result = ff.getFenceList(inparams);
                                                break;

                                            case "fence-addedit":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fenceaddedit;
                                                Fence ff0 = new Fence();
                                                Result = ff0.AddEditFence(inparams);
                                                break;
                                            case "fence-delete":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencedelete;
                                                Fence ff1 = new Fence();
                                                Result = ff1.DelFence(inparams);
                                                break;
                                            case "fence-getfencecar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencegetfencecar;
                                                FenceCar fc1 = new FenceCar();
                                                Result = fc1.getFenceCarsList(inparams);
                                                break;
                                            case "fence-setfencecar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencesetfencecar;
                                                FenceCar fc2 = new FenceCar();
                                                Result = fc2.SetFenceCar(inparams);
                                                break;

                                            case "fence-alarm-sta-list"://电子围栏报警统计-列表查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencealarmstalist;
                                                FenceAlarmStatistic fas = new FenceAlarmStatistic();
                                                Result = fas.FenceAlarmStatList(inparams);
                                                break;

                                            case "fence-alarm-sta-detail"://电子围栏报警统计-明细查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencealarmstadetail;
                                                FenceAlarmStatistic fas1 = new FenceAlarmStatistic();
                                                Result = fas1.FenceAlarmStatDetail(inparams);
                                                break;

                                            case "fence-alarm-sta-listexport":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencealarmstalistexport;
                                                FenceAlarmStatistic fas2 = new FenceAlarmStatistic();
                                                Result = fas2.FenceAlarmStatListExport(inparams);
                                                break;

                                            case "fence-alarm-sta-detailexport":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.fencealarmstadetailexport;
                                                FenceAlarmStatistic fas3 = new FenceAlarmStatistic();
                                                Result = fas3.FenceAlarmStatDetailExport(inparams);
                                                break;
                                            case "sys-faultstatistic-list-search":  //故障码统计-列表
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysfaultstatisticsearch;
                                                FaultStatistic fs = new FaultStatistic();
                                                Result = fs.QueryFaultList(inparams);
                                                break;

                                            case "sys-faultstatistic-list-export"://故障码统计-列表导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysfaultstatisticexport;
                                                FaultStatistic fs0 = new FaultStatistic();
                                                Result = fs0.ExportFaultList(inparams);
                                                break;
                                            case "sys-faultstatistic-search":  //故障码统计-明细
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysfaultstatisticsearch;
                                                FaultStatistic fs1 = new FaultStatistic();
                                                Result = fs1.QueryFaultDetailList(inparams);
                                                break;

                                            case "sys-faultstatistic-export"://故障码统计-明细导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysfaultstatisticexport;
                                                FaultStatistic fs2 = new FaultStatistic();
                                                Result = fs2.ExportFaultDetailList(inparams);
                                                break;

                                            case "sta-carmaintenanceremind":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.stacarmaintenanceremind;
                                                CarMaintenanceRemind cmr = new CarMaintenanceRemind();
                                                Result = cmr.QueryCarMaintenanceRemind(inparams);
                                                break;

                                            case "sys-carmaintenancerecord-insert":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.syscarmaintenancerecordinsert;
                                                CarMaintenanceRemind cmr1 = new CarMaintenanceRemind();
                                                Result = cmr1.InsertCarMaintenanceRecord(inparams);
                                                break;

                                            case "sys-carinfomationsend":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.syscarinfomationsend;
                                                InfoDown infod = new InfoDown();
                                                Result = infod.QueryCarMaintenanceData(inparams);
                                                break;

                                            case "car-list-search":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.carlistsearch;
                                                CarManage cm = new CarManage();
                                                Result = cm.getCarList(inparams);
                                                break;

                                            case "car-one-search":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.caronesearch;
                                                CarManage cm1 = new CarManage();
                                                Result = cm1.getOneCar(inparams);
                                                break;

                                            case "car-placelevel-get":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.carplacelevelget;
                                                CarManage cm2 = new CarManage();
                                                Result = cm2.getAllCarPlaceLevel(inparams);
                                                break;

                                            case "car-edit-save":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.careditsave;
                                                CarManage cm3 = new CarManage();
                                                Result = cm3.SaveCarInfo(inparams);
                                                break;

                                            case "car-onlinerate-month":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.caronlineratemonth;
                                                sta_onlinerate sonlinerate = new sta_onlinerate();
                                                Result = sonlinerate.getCarOnlineRate_Month(inparams);
                                                break;

                                            case "car-onlinerate-month-export":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.caronlineratemonthexport;
                                                sta_onlinerate sonlinerate0 = new sta_onlinerate();
                                                Result = sonlinerate0.getCarOnlineRate_Month_Export(inparams);
                                                break;

                                            case "marker-getlist":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.markergetlist;
                                                MarkerSetting showmarker = new MarkerSetting();
                                                Result = showmarker.getMarkerList(inparams);
                                                break;

                                            case "markersetting-addmarker":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.markersettingaddmarker;
                                                MarkerSetting addmarker = new MarkerSetting();
                                                Result = addmarker.CreateMarker(inparams);
                                                break;

                                            case "marker-delete":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.markerdeletemarker;
                                                MarkerSetting delmarker = new MarkerSetting();
                                                Result = delmarker.DelMarker(inparams);
                                                break;
                                            case "dealer-getbyuid":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.dealergetbyuid;
                                                sys_dealer sdealer = new sys_dealer();
                                                Result = sdealer.getLoginDealer(inparams);
                                                break;

                                            case "activeregion-getlist":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.activeregiongetlist;
                                                regionmanage regionM = new regionmanage();
                                                Result = regionM.getRegionList(inparams);
                                                break;

                                            case "activeregion-add":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.activeregionadd;
                                                regionmanage regionM1 = new regionmanage();
                                                Result = regionM1.AddRegion(inparams);
                                                break;

                                            case "activeregion-edit":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.activeregionedit;
                                                regionmanage regionM2 = new regionmanage();
                                                Result = regionM2.EditRegion(inparams);
                                                break;

                                            case "activeregion-delete":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.activeregiondelete;
                                                regionmanage regionM3 = new regionmanage();
                                                Result = regionM3.DeleteRegion(inparams);
                                                break;

                                            case "activeregion-getrelcar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.activeregiongetrelcar;
                                                regionmanage regionM4 = new regionmanage();
                                                Result = regionM4.getRegionCarsList(inparams);
                                                break;

                                            case "activeregion-setrelcar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.activeregionsetrelcar;
                                                regionmanage regionM5 = new regionmanage();
                                                Result = regionM5.SetRegionCar(inparams);
                                                break;


                                            case "driveline-getlist":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivelinegetlist;
                                                DriveLineManage dlm = new DriveLineManage();
                                                Result = dlm.getDriveLineList(inparams);
                                                break;

                                            case "driveline-add":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivelineadd;
                                                DriveLineManage dlm1 = new DriveLineManage();
                                                Result = dlm1.AddDriveLine(inparams);
                                                break;

                                            case "driveline-getmarker":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivelinegetmarker;
                                                DriveLineManage dlm2 = new DriveLineManage();
                                                Result = dlm2.getOneLine(inparams);
                                                break;

                                            case "driveline-delete":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivelinedelete;
                                                DriveLineManage dlm3 = new DriveLineManage();
                                                Result = dlm3.DeleteDriveLines(inparams);
                                                break;

                                            case "driveline-getrelcar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivelinegetrelcar;
                                                DriveLineManage dlm4 = new DriveLineManage();
                                                Result = dlm4.getLineCarsList(inparams);
                                                break;

                                            case "driveline-setrelcar":
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.drivelinesetrelcar;
                                                DriveLineManage dlm5 = new DriveLineManage();
                                                Result = dlm5.SetLineCar(inparams);
                                                break;


                                            case "sys-testdrive-search"://试乘试驾查询

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.systestdrivesearch;

                                                sta_testdrive test_drive = new sta_testdrive();

                                                Result = test_drive.getTCDetail(inparams);
                                                break;



                                            case "sys-outdrive-search"://偏离试驾路线查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutdrivesearch;
                                                sta_outdrive out_drive = new sta_outdrive();
                                                Result = out_drive.getOCDetail(inparams);
                                                break;

                                            case "sys-outarea-search"://驶出活动范围查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearch;
                                                sta_outarea out_area = new sta_outarea();
                                                Result = out_area.getOADetail(inparams);
                                                break;

                                            case "sys-testdrive-search-output"://试乘试驾查询导出

                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.systestdrivesearchoutput;

                                                sta_testdrive test_drive_out = new sta_testdrive();

                                                Result = test_drive_out.getTCOutput(inparams);
                                                break;


                                            case "sys-outdrive-search-output"://偏离试驾路线查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutdrivesearchoutput;
                                                sta_outdrive out_drive_out = new sta_outdrive();
                                                Result = out_drive_out.getOCOutput(inparams);
                                                break;

                                            case "sys-outarea-search-output"://驶出活动范围查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                sta_outarea out_area_out = new sta_outarea();
                                                Result = out_area_out.getOAOutput(inparams);
                                                break;

                                            case "DayTestDriveAnalyse-getlist"://试乘试驾日分析表
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.daytestdriveanalysgetlist;
                                                sta_DayTestDriveAnalyse sta_DayTestDriveAnalyse = new sta_DayTestDriveAnalyse();
                                                Result = sta_DayTestDriveAnalyse.getDayAnalyseList(inparams);
                                                break;

                                            case "DayTestDriveAnalyse-getlist-new"://试乘试驾日分析表-new
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.daytestdriveanalysgetlist;
                                                sta_DayTestDriveAnalyse_new sta_DayTestDriveAnalyse_new = new sta_DayTestDriveAnalyse_new();
                                                Result = sta_DayTestDriveAnalyse_new.getSFCar(inparams);
                                                break;
                                            //-------add by kangpeng ----20160713---------- 
                                            case "DayTestDriveAnalyse-getlist-new-output"://试乘试驾日分析表-new导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;

                                                sta_DayTestDriveAnalyse_new sta_DayTestDriveAnalyse_new_out = new sta_DayTestDriveAnalyse_new();
                                                Result = sta_DayTestDriveAnalyse_new_out.getSFCaroutput(inparams);
                                                break;

                                            case "MonthTestDriveAnalyse-getlist-new"://试乘试驾月分析表-new
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.daytestdriveanalysgetlist;
                                                sta_MonthTestDriveAnalyse_new sta_MonthTestDriveAnalyse_new = new sta_MonthTestDriveAnalyse_new();
                                                Result = sta_MonthTestDriveAnalyse_new.getSFCar(inparams);
                                                break;

                                            case "MonthTestDriveAnalyse-getlist-new-output"://试乘试驾月分析表-new导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;

                                                sta_MonthTestDriveAnalyse_new sta_MonthTestDriveAnalyse_new_out = new sta_MonthTestDriveAnalyse_new();
                                                Result = sta_MonthTestDriveAnalyse_new_out.getSFCaroutput(inparams);
                                                break;
                                            //-------add by kangpeng ----20160713---------- 
                                              

                                            case "MonthTestDriveAnalyse-getlist"://试乘试驾月分析表
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.monthtestdriveanalysgetlist;
                                                sta_MonthTestDriveAnalyse sta_MonthTestDriveAnalyse = new sta_MonthTestDriveAnalyse();
                                                Result = sta_MonthTestDriveAnalyse.getMonthAnalyseList(inparams);
                                                break;

                                            case "DayTestDriveAnalyse-getSDealersName"://试乘试驾日分析表--经销商名称关联查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.daytestdriveanalysgetSDealersName;
                                                sta_DayTestDriveAnalyse sta_DayTestGetDealersName = new sta_DayTestDriveAnalyse();
                                                Result = sta_DayTestGetDealersName.getDealerName(inparams);
                                                break;

                                            case "DayTestDriveAnalyse-export"://试乘试驾日分析表--数据导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.daytestdriveanalyseexport;
                                                sta_DayTestDriveAnalyse sta_DayTestExport = new sta_DayTestDriveAnalyse();
                                                Result = sta_DayTestExport.doExport(inparams);
                                                break;

                                            case "MonthTestDriveAnalyse-export"://试乘试驾月分析表--数据导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.monthtestdriveanalyseexport;
                                                sta_MonthTestDriveAnalyse sta_MonthTestExport = new sta_MonthTestDriveAnalyse();
                                                Result = sta_MonthTestExport.doExport(inparams);
                                                break;
                                                
                                            case "ComTestDriveAnalyse-getlist"://试乘试驾综合分析表
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.daytestdriveanalysgetlist;
                                                sta_ComTestDriveAnalyse sta_ComTestDriveAnalyse = new sta_ComTestDriveAnalyse();
                                                Result = sta_ComTestDriveAnalyse.getSFCar(inparams);
                                                break;

                                            case "ComTestDriveAnalyse-output"://试乘试驾综合查询导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                sta_ComTestDriveAnalyse sta_ComTestDriveAnalyse_out = new sta_ComTestDriveAnalyse();
                                                Result = sta_ComTestDriveAnalyse_out.getSFCaroutput(inparams);
                                                break;
                                            //----add by  for 销售贷款基础信息维护------------2016-1-16------------  

                                            case "annulBase-getCombo"://销售贷款基础信息维护取得页面下拉框
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_combo = new AnnulBase();
                                                Result = annulBase_combo.getAllCmbOnLoad(inparams);
                                                break;

                                            case "annulBase-list-search"://销售贷款基础信息维护查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_list = new AnnulBase();
                                                Result = annulBase_list.getAnnulBaseList(inparams);
                                                break;

                                            case "annulBase-sim-search"://基础信息维护新增时根据sim卡查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_sim_search = new AnnulBase();
                                                Result = annulBase_sim_search.searchBySim(inparams);
                                                break;

                                            case "annulbase-getSimCombo"://基础信息维护sim卡改变生成下拉框
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulbase_getSimCombo = new AnnulBase();
                                                Result = annulbase_getSimCombo.getSimCmb(inparams);
                                                break;

                                            case "annulBase-one-search"://销售贷款基础信息维护编辑时根据id查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_one = new AnnulBase();
                                                Result = annulBase_one.getEditMessageByCid(inparams);
                                                break;

                                            case "annul-add-save"://销售贷款基础信息维护新增保存
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_addSave = new AnnulBase();
                                                Result = annulBase_addSave.addSaveBaseInfo(inparams);
                                                break;

                                            case "annul-edit-save"://销售贷款基础信息维护修改保存
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_editSave = new AnnulBase();
                                                Result = annulBase_editSave.editSaveBaseInfo(inparams);
                                                break;

                                            case "annulBase-list-export"://销售贷款基础信息维护导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulBase annulBase_export = new AnnulBase();
                                                Result = annulBase_export.doExport(inparams);
                                                break;

                                            case "annulLog-list-search"://销售贷款系统日志查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulLog AnnulLog_list = new AnnulLog();
                                                Result = AnnulLog_list.getAnnulLogList(inparams);
                                                break;

                                            case "annulLog-get-combobox"://销售贷款系统日志下拉框取得
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulLog annulLog_getcmb = new AnnulLog();
                                                Result = annulLog_getcmb.getAllCmbOnLoad(inparams);
                                                break;

                                            case "annulLog-list-export"://销售贷款基础信息维护导出
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                AnnulLog annulLog_export = new AnnulLog();
                                                Result = annulLog_export.doExport(inparams);
                                                break;

                                            case "loanOrg-list-search"://销售贷款基础信息维护查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                LoanOrg loanOrg_list = new LoanOrg();
                                                Result = loanOrg_list.getLoanOrgList(inparams);
                                                break;


                                            case "loanOrg-one-search"://销售贷款机构信息维护编辑时根据id查询
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                LoanOrg loanOrg_one = new LoanOrg();
                                                Result = loanOrg_one.editByOrgNo(inparams);
                                                break;

                                            case "loanOrg-add-save"://销售贷款机构信息维护新增保存
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                LoanOrg loanOrg_addSave = new LoanOrg();
                                                Result = loanOrg_addSave.addSaveLoanOrg(inparams);
                                                break;

                                            case "loanOrg-edit-save"://销售贷款机构信息维护修改保存
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                LoanOrg loanOrg_editSave = new LoanOrg();
                                                Result = loanOrg_editSave.editSaveOrgNo(inparams);
                                                break;

                                            case "loanOrg-delete"://销售贷款机构信息维护删除
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.sysoutareasearchoutput;
                                                LoanOrg loanOrg_delete = new LoanOrg();
                                                Result = loanOrg_delete.deleteLoanOrg(inparams);
                                                break;

                                            //----add by  for 销售贷款------------2016-1-16------------

                                            case "al-org-get"://销贷-查询放款机构
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.alorgget;
                                                Result = new LockApplication().getOrg(inparams);
                                                break;

                                            case "al-dict-get"://销贷-查询字典项
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.aldictget;
                                                Result = new LockApplication().getDict(inparams);
                                                break;

                                            case "al-Tertype-get"://销贷-查询终端型号
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.altertypeget;
                                                Result = new LockApplication().getTertype(inparams);
                                                break;

                                            case "al-EnergyType-get"://销贷-查询发动机类型
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.alenergytypeget;
                                                Result = new LockApplication().getEnergyType(inparams);
                                                break;

                                            case "al-lock-search"://销贷-查询锁车解锁数据
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.allocksearch;
                                                Result = new LockApplication().conditionSearch(inparams);
                                                break;

                                            case "al-lock-export"://销贷-导出锁车解锁数据
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.allockexport;
                                                Result = new LockApplication().export(inparams);
                                                break;

                                            case "al-lock-apply"://销贷-申请锁车解锁
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.allockapply;
                                                Result = new LockApplication().apply(inparams);
                                                break;

                                            case "al-lock-set"://销贷-审批锁车解锁
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.allockset;
                                                Result = new LockApplication().approve(inparams);
                                                break;

                                            case "al-parameter-search"://销贷-查询参数设置一览数据
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.alparametersearch;
                                                Result = new ParameterSetting().conditionSearch(inparams);
                                                break;

                                            case "al-parameter-export"://销贷-导出参数设置一览数据
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.alparamterexport;
                                                Result = new ParameterSetting().export(inparams);
                                                break;

                                            case "al-parameter-set"://销贷-参数设置
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.alparameterset;
                                                Result = new ParameterSetting().set(inparams);
                                                break;

                                            case "al-parameter-active"://销贷-激活设置
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.alparameteractive;
                                                Result = new ParameterSetting().active(inparams);
                                                break;


                                            default:
                                                Result = new ResponseResult(ResState.NotExist, "sid错误，服务不存在", "");
                                                logmodel.LogType = (int)SysService.LogType.LogTypeEnum.excepetion;
                                                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.excepetion) + " sid错误，服务不存在";
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Result = new ResponseResult(ResState.NoPermission, "无权限操作", "");
                                        logmodel.LogType = (int)SysService.LogType.LogTypeEnum.excepetion;
                                        //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.excepetion) + " 无权限操作";
                                    }
                                }
                                else
                                {
                                    Result = new ResponseResult(ResState.NotLogin, "用户未登录或登录超时", "");
                                    logmodel.LogType = (int)SysService.LogType.LogTypeEnum.excepetion;
                                    //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.excepetion) + " 用户未登录或登录超时";
                                }

                            }
                            #endregion
                        }
                    }
                    else
                    {
                        //Result = new ResponseResult(ResState.ParamsImperfect, "参数错误！参数不完整，详见服务接口文档。", null);
                    }
                    //定位监控定时取最新轨迹不记录日志，因操作太频繁
                    if (!sid.Equals("sys-usercar-getlasttrack") && !sid.Equals("sys-user-heart") && !sid.Equals("order-receive-getall") && !sid.Contains("aoev"))
                    {
                        logmodel.SysFlag = sysflag;
                        logmodel.SysName = "WebGIS";
                        logmodel.UserId = Int32.Parse(sysuid);
                        if ((ResState)Result.state == ResState.Success)
                        {
                            logmodel.LogResult = "成功";
                            logmodel.LogContent = "操作成功";
                        }
                        else
                        {
                            logmodel.LogResult = "失败";
                            logmodel.LogContent = ((ResState)Result.state).ToString() + "-" + Result.msg;
                        }
                        logmodel.OneCarUser = onecaruser;
                        slog.WriteSysLog(logmodel);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("执行异常：", ex);
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");

                //logmodel.LogContent = LogType.GetEnumName(LogType.LogTypeEnum.excepetion) + ex.Message;
                if (!string.IsNullOrEmpty(sysuid))
                {
                    logmodel.UserId = Int32.Parse(sysuid);
                }
                else
                {
                    logmodel.UserId = -1;
                }
                logmodel.SysFlag = sysflag;
                logmodel.SysName = "WebGIS";
                logmodel.LogContent = ((ResState)Result.state).ToString() + "-" + ex.Message;
                logmodel.LogResult = "失败";
                logmodel.OneCarUser = onecaruser;
                //slog.WriteSysLog(logmodel);
            }

            Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            string strRes = JsonConvert.SerializeObject(Result, timeConverter);
            context.Response.Write(strRes.Replace("null", "\"\""));
        }

        private Dictionary<string, string> GetParams(HttpContext context)
        {
            try
            {
                Dictionary<string, string> inparams = new Dictionary<string, string>();
                HttpRequest request = context.Request;
                if (request.HttpMethod == "POST")
                {
                    Stream stream = request.InputStream;
                    StreamReader streamReader = new StreamReader(stream);
                    string json = streamReader.ReadToEnd();
                    LogHelper.WriteInfo("HttpService PostParams:" + json);
                    JObject InParam = (JObject)JsonConvert.DeserializeObject(json);
                    if (InParam != null)
                    {
                        foreach (var p in InParam)
                        {
                            //inparams.Add(p.Key, JsonConvert.SerializeObject(p.Value));
                            inparams.Add(p.Key, p.Value.ToString());
                        }
                    }
                }
                else
                {
                    foreach (var s in request.QueryString.AllKeys)
                    {
                        inparams.Add(System.Web.HttpUtility.UrlDecode(s).ToString(), System.Web.HttpUtility.UrlDecode(request[s]).ToString());
                    }
                    LogHelper.WriteInfo("HttpService GetParams:" + request.Url.Query);
                }
                return inparams;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("GetParams调用异常", ex);
                return null;
            }
        }
        /// <summary>
        /// 基础参数完整性检查
        /// </summary>
        /// <param name="inparams"></param>
        /// <param name="resp"></param>
        /// <returns></returns>
        private bool BaseParamsCheck(Dictionary<string, string> inparams, out ResponseResult resp)
        {
            resp = new ResponseResult();
            bool res = true;
            string mes = "缺少参数";
            if (!inparams.Keys.Contains("sid"))
            {
                res = false;
                resp.state = 102;
                mes += "sid，";
            }
            if (!inparams.Keys.Contains("sysflag"))
            {
                res = false;
                resp.state = 102;
                mes += "sysflag，";
            }
            if (inparams.Keys.Contains("sysflag") && inparams["sysflag"].Equals("A0EV"))
            {
                resp.msg = mes;
                return res;
            }
            if (!inparams.Keys.Contains("sid") && !(inparams["sid"].Equals("sys-user-login") || !inparams["sid"].Equals("sys-user-logout")) && !inparams.Keys.Contains("sysuid"))
            {
                res = false;
                resp.state = 102;
                mes += "sysuid，";
            }
            if (!inparams.Keys.Contains("sid") && !(inparams["sid"].Equals("sys-user-login") || !inparams["sid"].Equals("sys-user-logout")) && !inparams.Keys.Contains("token"))
            {
                res = false;
                resp.state = 102;
                mes += "token，";
            }
            resp.msg = mes;
            return res;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 响应前台数据结果模型
    /// </summary>
    public class ResponseResult
    {
        public ResponseResult()
        {
            rtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Msg"></param>
        /// <param name="Result">Select操作时此处使用ResList类型对象</param>
        public ResponseResult(ResState State, string Msg, object Result)
        {
            state = (int)State;
            msg = Msg;
            result = Result == null ? "" : Result;
            rtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Msg"></param>
        /// <param name="Result">Select操作时此处使用ResList类型对象</param>
        public ResponseResult(ResState State, string Msg, ResList Result)
        {
            state = (int)State;
            msg = Msg;
            result = Result == null ? "" : (object)Result;
            rtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public int state;
        public string msg;
        public object result;
        public string rtime;
    }
    public class ResList
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int total;
        /// <summary>
        /// 是否返回全部结果
        /// </summary>
        public int isallresults;

        /// <summary>
        /// 页长度
        /// </summary>
        public int size;
        /// <summary>
        /// 总页数
        /// </summary>
        public int page;
        /// <summary>
        /// 数组/DataTable/List等
        /// </summary>
        public object records;
    }

    /// <summary>
    /// 操作结果状态 枚举 
    /// </summary>
    public enum ResState
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 100,
        /// <summary>
        /// 操作失败
        /// </summary>
        OperationFailed = 101,
        /// <summary>
        /// 参数不完整
        /// </summary>
        ParamsImperfect = 102,
        /// <summary>
        /// 无权限
        /// </summary>
        NoPermission = 103,
        /// <summary>
        /// 未登陆/登陆超时
        /// </summary>
        NotLogin = 104,
        /// <summary>
        /// 服务不存在
        /// </summary>
        NotExist = 105,

        /// <summary>
        /// 验证码错误
        /// </summary>
        NoCheckCode = 106,
        /// <summary>
        /// 其他
        /// </summary>
        OtherError = 107,
        /// <summary>
        /// 稍后重试
        /// </summary>
        RetryAfter = 201
    }
}