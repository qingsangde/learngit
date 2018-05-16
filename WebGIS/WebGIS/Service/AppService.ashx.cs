using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

using System.IO;
using System.Web.SessionState;
using Newtonsoft.Json.Linq;
using WebGIS.sys;
using SysService;
using CommLibrary.Log;
using CommLibrary;
using WebGIS.Service.App;

//using Newtonsoft.Json.Linq;

namespace WebGIS
{
    /// <summary>
    /// Service 的摘要说明
    /// </summary>
    public class AppService : IHttpHandler, IRequiresSessionState
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
            ResponseAppResult Result = null;
            context.Response.ContentType = "text/plain";
            try
            {

                Dictionary<string, string> inparams = GetParams(context);
                if (inparams == null || inparams.Count == 0)
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "参数错误！格式为JSON格式数据，详见服务接口文档。", null);
                }
                else
                {
                    if (BaseParamsCheck(inparams, out Result))
                    {
                        if (inparams.ContainsKey("sysflag"))
                        {
                            inparams["sysflag"] = WebProc.GetAppSysflagKey(inparams["sysflag"]);
                            sysflag = inparams["sysflag"];
                           
                        } 
                        sid = inparams["sid"];
                        sysuid = inparams.Keys.Contains(sysuid) ? inparams["sysuid"] : "0";
                        logmodel.LogType = (int)SysService.LogType.LogTypeEnum.AppWebService;
                        AlarmProess ap = new AlarmProess();
                        AppProess app = new AppProess();
                        AppvdProess appvd = new AppvdProess();
                        ExtYQWL ey = new ExtYQWL();
                        switch (sid)
                        {
                            #region APP基本公共接口
                            case "app-car-getallonline":
                                Result = app.AppGetAllOnline(inparams);
                                break;
                            case "app-user-register":
                                Result = app.AppUserRegister(inparams);
                                break;
                            case "app-user-smscheckcode":       //获取短信验证码
                                Result = app.AppSMSCode(inparams);
                                break;
                            case "app-user-login":
                                Result = app.AppUserLogin(inparams);
                                break;
                            case "app-user-login2":
                                Result = app.AppUserLogin2(inparams);
                                break;
                            case "app-user-changepwd":
                                Result = app.AppUserChangePwd(inparams);
                                break;
                            case "app-car-bind":
                                Result = app.AppUserCarBind(inparams);  //用户车辆绑定
                                break;
                            case "app-user-update":
                                Result = app.AppUserUpdate(inparams);
                                break;
                            case "app-car-usercars":
                                Result = app.AppGetUserCars(inparams);
                                break;
                            case "app-car-seachusercars":
                                Result = app.AppSeachUserCars(inparams);    //获取车辆列表
                                break;
                            case "app-car-unwrap":
                                Result = app.AppUnwrapUserCars(inparams);
                                break;
                            case "app-car-lasttrack":
                                Result = app.AppGetCarLastTrack(inparams);
                                break;
                            case "app-car-track":
                                Result = app.AppGetCarTrack(inparams);
                                break;
                            case "app-user-resetpwd":        //重置密码
                                Result = app.AppUserResetPwd(inparams);
                                break;
                            case "app-car-seachforpages":        //用户车辆检索 带分页
                                Result = app.AppSeachForPages(inparams);
                                break;
                            case "app-car-onlinecount":        //用户车辆在线状态统计
                                Result = app.AppCarOnlineCount(inparams);
                                break;
                            #endregion
                            #region 报警器专用接口
                            case "alarm-car-track":
                                Result = ap.AlarmGetCarTrack(inparams);
                                break;
                            case "alarm-car-find":
                                Result = ap.AppSendFindCar(inparams);
                                break;
                            case "alarm-car-status":
                                Result = ap.AppGetCarStatues(inparams);
                                break;
                            case "alarm-car-remotecontrol":
                                Result = ap.AppSendControl(inparams);
                                break;
                            case "alarm-car-alertview":
                                Result = ap.AppGetAppAlarmAlert(inparams);
                                break;
                            case "alarm-obd-sendobddiagn":
                                Result = ap.AppSendOBDDiagn(inparams);
                                break;
                            case "alarm-obd-sendobddriver":
                                Result = ap.AppSendOBDDriver(inparams);
                                break;
                            case "alarm-obd-getobddiagn":
                                Result = ap.AppGetOBDDiagnResult(inparams);
                                break;
                            case "alarm-obd-getobddriver":
                                Result = ap.AppGetOBDDriverResult(inparams);
                                break;
                            #endregion
                            #region 外部专用接口
                            case "ext-yqwl-vinupload":
                                Result = ey.AppVINUpload(inparams);
                                break;
                            case "ext-yqwl-carowner":
                                Result = ey.AppGetCarInfoByCarno(inparams);
                                break;
                            #endregion
                            #region APP应用端接口
                            case "appvd-user-register":     //用户注册
                                Result = appvd.AppvdUserRegister(inparams);
                                break;
                            case "appvd-user-login":        //用户登录
                                Result = appvd.AppUserLogin(inparams);
                                break;
                            case "appvd-user-update":       //用户修改信息
                                Result = appvd.AppvdUserUpdate(inparams);
                                break;
                            case "appvd-apk-version":        //获取APK指定版本
                                Result = appvd.AppvdGetApkVersion(inparams);
                                break;
                            #endregion
                        }
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.ParamsImperfect, "参数错误！参数不完整，详见服务接口文档。", null);
                    }
                    Result.sid = inparams.Keys.Contains("sid") ? inparams["sid"] : "";
                    Result.sequ = inparams.Keys.Contains("sequ") ? inparams["sequ"] : "";
                    //定位监控定时取最新轨迹不记录日志，因操作太频繁
                    if (sid != "" && !sid.Equals("alarm-car-lasttrack") && sysflag!="")
                    {
                        logmodel.SysFlag = sysflag;
                        logmodel.SysName = "AppService";
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
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, "");

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
                logmodel.SysName = "WebGISAppService";
                logmodel.LogContent = ((ResState)Result.state).ToString() + "-" + ex.Message;
                logmodel.LogResult = "失败";
                logmodel.OneCarUser = onecaruser;
                //slog.WriteSysLog(logmodel);
            }

            Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();

            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            Result.msg = Result.msg.Replace("\"", "");
            string strRes = JsonConvert.SerializeObject(Result, timeConverter);
            if (Result.state != 200)
            {
                LogHelper.WriteInfo("AppService Response:" + strRes);
            }
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
                    LogHelper.WriteInfo("AppService PostParams:" + json);
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
                    LogHelper.WriteInfo("AppService GetParams:" + request.Url.Query);
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
        private bool BaseParamsCheck(Dictionary<string, string> inparams, out ResponseAppResult resp)
        {
            resp = new ResponseAppResult();
            bool res = true;
            string mes = "缺少参数";
            if (!inparams.Keys.Contains("sid"))
            {
                res = false;
                resp.state = 102;
                mes += "sid，";
            }
            if (!inparams.Keys.Contains("sysflag") && "app-user-login2" != inparams["sid"].Trim())
            {
                res = false;
                resp.state = 102;
                mes += "sysflag，";
            }


            //if (!inparams.Keys.Contains("sid") && !(inparams["sid"].Equals("sys-user-login") || !inparams["sid"].Equals("sys-user-logout")) && !inparams.Keys.Contains("token"))
            //{
            //    res = false;
            //    resp.state = 102;
            //    mes += "token，";
            //}
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
    public class ResponseAppResult
    {
        public ResponseAppResult()
        {
            rtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <param name="Msg"></param>
        /// <param name="Result">Select操作时此处使用ResList类型对象</param>
        public ResponseAppResult(ResState State, string Msg, object Result)
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
        public ResponseAppResult(ResState State, string Msg, ResList Result)
        {
            state = (int)State;
            msg = Msg;
            result = Result == null ? "" : (object)Result;
            rtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public int state;
        public string msg;
        public string rtime;
        public string sid;
        public string sequ;
        public object result;
    }


}