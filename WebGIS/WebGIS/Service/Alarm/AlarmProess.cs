using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.IO;
using SysService;
using CommLibrary.Proto;

namespace WebGIS
{
    public class AlarmProess
    {
        private DataTable GetUserCars(string sysflag, string uid)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetUserCars"), Parameters).Tables[0];

            return dt;
        }
        private DataTable GetAppAlarmAlert(string sysflag, string cid, string uid, DateTime st, DateTime et)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid),new SqlParameter("@uid", uid),
                                        new SqlParameter("@st", st),new SqlParameter("@et", et)};
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetAppAlarmAlert"), Parameters).Tables[0];

            return dt;
        }
        private long GetCarCIDBySimCode(string sysflag, string sim)
        {
            long cid = long.Parse(sim);
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@SIMCode", sim) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetCidBySimcode"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cid = long.Parse(dt.Rows[0]["CID"].ToString());
            }
            return cid;
        }
        private long GetCarCID(string sysflag, string carno)
        {
            long cid = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@carno", carno), new SqlParameter("@lpc", "") };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QSProc_Car_SelectByCno"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cid = long.Parse(dt.Rows[0]["CID"].ToString());
            }
            return cid;
        }
        private long GetCidByTno(string sysflag, string tno)
        {
            long cid = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@tno", tno) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWCProc_LT_GetCidByTno"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cid = long.Parse(dt.Rows[0]["cid"].ToString());
            }
            return cid;
        }
        private string GetCarSIMCode(string sysflag, string cid)
        {
            string simcode = "";
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWCProc_LC_GetSimTnoByCid"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                simcode = dt.Rows[0]["SimCode"].ToString();
            }
            return simcode;
        }
        private long GetCarTNO(string sysflag, string cid)
        {
            long tno = 0;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_SC_GetTnoByCid"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                tno = long.Parse(dt.Rows[0]["tno"].ToString());
            }
            return tno;
        }
        private string GetCarCarno(string sysflag, string cid)
        {
            string carno = "";
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_SC_GetTnoByCid"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                carno = dt.Rows[0]["carno"].ToString();
            }
            return carno;
        }

        public ResponseAppResult AppGetAppAlarmAlert(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("st") || inparams["st"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少st或st为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("et") || inparams["et"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少et或et为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }
            try
            {
                string sysflag = inparams["sysflag"];
                long uid = long.Parse(inparams["uid"]);
                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                DateTime st = DateTime.Parse(inparams["st"]);
                DateTime et = DateTime.Parse(inparams["et"]);
                DataTable dt = GetAppAlarmAlert(WebProc.GetAppSysflagKey(sysflag), cid.ToString(), uid.ToString(), st, et);

                ResList res = new ResList();
                res.isallresults = 1;
                res.records = dt;
                res.total = dt.Rows.Count;
                res.size = 0;
                res.page = 0;

                Result = new ResponseAppResult(ResState.Success, "操作成功", res);




            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppGetCarLastTrack调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }

        public ResponseAppResult AppGetCarStatues(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }


            try
            {


                string sysflag = inparams["sysflag"];

                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                monitor mon = new monitor();
                WebGIS.RealtimeDataServer.CarRealData[] RealData = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), new long[] { cid });
                if (RealData.Length > 0)
                {
                    alarmcarstatuesalarm wd = new alarmcarstatuesalarm();
                    wd.cid = RealData[0].Carid.ToString();
                    wd.lat = RealData[0].Lati.ToString();
                    wd.longt = RealData[0].Long.ToString();
                    wd.onlinestatus = RealData[0].OnlineStatus.ToString();
                    wd.tdatetime = RealData[0].TDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    wd.tno = RealData[0].TNO.ToString();


                    if (RealData[0].sPositionAdditionalInfo != null)
                    {
                        wd = analyzeStatues(wd, RealData[0].sPositionAdditionalInfo);

                        Result = new ResponseAppResult(ResState.Success, "操作成功", wd);
                    }
                    else
                    {
                        SendOrderHander.Send_CTS_TermSearchRequest(WebProc.GetAppSysflagKey(sysflag), "", cid, long.Parse(wd.tno));
                        Result = new ResponseAppResult(ResState.Success, "操作失败，无车辆状态数据！", null);
                    }
                }

                else
                {
                    Result = new ResponseAppResult(ResState.Success, "操作失败，车辆不在线！", null);
                }



            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppGetCarLastTrack调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        private alarmcarstatuesalarm analyzeStatues(alarmcarstatuesalarm res, byte[] datas)
        {
            AlertorResolve alertorResolve = new AlertorResolve();
            //MemoryStream msAdditionalInfo = new MemoryStream(datas);
            //COM_PositionAdditionalInfo AdditionalInfo = ProtoBuf.Serializer.Deserialize<COM_PositionAdditionalInfo>(msAdditionalInfo);
            Dictionary<int, byte[]> customdataparams = EB_Analyze.AnalyzePosAddlInfo(datas);
            //wd.rawdata = MyTools.byteToHexStr(AdditionalInfo.sCustomData);
            if (customdataparams.ContainsKey(1))//报警信息
            {
                res.caralarms = dictcov(alertorResolve.analyzeAlertorInfo(customdataparams[1]));
            }
            if (customdataparams.ContainsKey(2))//状态信息
            {
                res.carstatues = dictcov(alertorResolve.analyzeStatues(customdataparams[2]));
            }
            if (customdataparams.ContainsKey(3))//电压信息
            {
                res.voltage = alertorResolve.analyzeEV(customdataparams[3][0]).ToString();
            }
            return res;
        }
        public ResponseAppResult AppSendFindCar(Dictionary<string, string> inparams)
        {
            inparams.Add("eventstate", "4");
            return AppSendControl(inparams);

        }
        public ResponseAppResult AlarmGetCarTrack(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("st") || inparams["st"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少st或st为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("et") || inparams["et"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少et或et！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];

                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                string st = inparams["st"];
                string et = inparams["et"];


                DataTable dt = GetTracksFromDB(WebProc.GetAppSysflagKey(sysflag), cid.ToString(), st, et);

                List<alarmcartrack> tracks = new List<alarmcartrack>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    alarmcartrack vv = new alarmcartrack();
                    double sp = double.Parse(dt.Rows[i]["T_Speed"].ToString());


                    //经纬度纠偏
                    double[] s = CarDataConvert.ConvertCoordToGCJ02(double.Parse(dt.Rows[i]["T_Long"].ToString()), double.Parse(dt.Rows[i]["T_Lati"].ToString()));

                    vv.lng = (int)(Math.Round(s[0], 8) * 1000000);
                    vv.lat = (int)(Math.Round(s[1], 8) * 1000000);
                    vv.sp = (int)sp * 10;
                    vv.dt = (int)(DateTime.Parse(dt.Rows[i]["T_DateTime"].ToString()) - DateTime.Parse("1970-01-01 00:00:00")).TotalSeconds;
                    tracks.Add(vv);

                }
                ResList res = new ResList();
                res.isallresults = 1;
                res.records = tracks.ToArray();
                res.total = tracks.Count;
                res.size = 0;
                res.page = 0;

                Result = new ResponseAppResult(ResState.Success, "操作成功", res);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AlarmGetCarTrack调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public DataTable GetTracksFromDB(string sysflag, string cid, string st, string et)
        {
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@cid", cid);
                oaPara[1] = new SqlParameter("@BEGINTime", st);
                oaPara[2] = new SqlParameter("@ENDTime", et);

                DataTable dt = new DataTable();
                return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_App_StoryTrack"), oaPara).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseAppResult AppSendControl(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("eventstate") || inparams["eventstate"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少eventstate或eventstate为空！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];
                monitor mon = new monitor();

                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                WebGIS.RealtimeDataServer.CarRealData[] RealData = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), new long[] { cid });
                if (RealData.Length > 0)
                {
                    if (RealData[0].OnlineStatus == 1)
                    {
                        long tno = GetCarTNO(WebProc.GetAppSysflagKey(sysflag), cid.ToString());
                        string eventstate = inparams["eventstate"];
                        AlertorResolve ar = new AlertorResolve();
                        if (eventstate == "4")
                        {
                            if (RealData[0].sPositionAdditionalInfo != null)
                            {
                                alarmcarstatuesalarm act = new alarmcarstatuesalarm();
                                act = analyzeStatues(act, RealData[0].sPositionAdditionalInfo);

                                // if(act.carstatues.GetValue(13))
                                KeyValuePair<int, int> accstates = (KeyValuePair<int, int>)act.carstatues.GetValue(19);
                                if (accstates.Value == 1)
                                {
                                    byte[] data = ar.GetParamDownData(int.Parse(eventstate));
                                    bool w = SendOrderHander.Send_CTS_TransmissionProtocol(WebProc.GetAppSysflagKey(sysflag), "", cid, tno, 0x8fc9, data);
                                    if (w)
                                    {
                                        Result = new ResponseAppResult(ResState.Success, "操作成功", "");
                                    }
                                    else
                                    {
                                        Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，后台服务中断！", "");
                                    }
                                }
                                else
                                {
                                    Result = new ResponseAppResult(ResState.OperationFailed, "车辆未进入警戒状态！", "");
                                }
                            }
                            else
                            {
                                Result = new ResponseAppResult(ResState.OperationFailed, "车辆未进入警戒状态！", "");
                            }
                        }
                        else
                        {
                            byte[] data = ar.GetParamDownData(int.Parse(eventstate));
                            bool w = SendOrderHander.Send_CTS_TransmissionProtocol(WebProc.GetAppSysflagKey(sysflag), "", cid, tno, 0x8fc9, data);
                            if (w)
                            {
                                Result = new ResponseAppResult(ResState.Success, "操作成功", "");
                            }
                            else
                            {
                                Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，后台服务中断！", "");
                            }

                        }
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "车辆已经离线，无法发送指令！", null);
                        return Result;
                    }

                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "车辆已经离线，无法发送指令！", null);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppSendControl调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppSendOBDDiagn(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }


            try
            {
                string sysflag = inparams["sysflag"];
                monitor mon = new monitor();
                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                WebGIS.RealtimeDataServer.CarRealData[] RealData = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), new long[] { cid });
                if (RealData.Length > 0)
                {
                    if (RealData[0].OnlineStatus == 1)
                    {
                        if (RealData[0].sPositionAdditionalInfo != null)
                        {
                            alarmcarstatuesalarm act = new alarmcarstatuesalarm();
                            act = analyzeStatues(act, RealData[0].sPositionAdditionalInfo);
                            KeyValuePair<int, int> obdstates = (KeyValuePair<int, int>)act.carstatues.GetValue(15);
                            // if(act.carstatues.GetValue(13))
                            KeyValuePair<int, int> accstates = (KeyValuePair<int, int>)act.carstatues.GetValue(13);
                            if (accstates.Value == 1)
                            {
                                if (obdstates.Value == 1)
                                {
                                    if (RealData[0].Speed == 0)
                                    {
                                        long tno = GetCarTNO(WebProc.GetAppSysflagKey(sysflag), cid.ToString());

                                        AlertorResolve ar = new AlertorResolve();
                                        byte[] data = new byte[0];
                                        bool w = SendOrderHander.Send_CTS_TransmissionProtocol(WebProc.GetAppSysflagKey(sysflag), "", cid, tno, 0x8fc7, data);
                                        if (w)
                                        {
                                            Result = new ResponseAppResult(ResState.Success, "操作成功", "");
                                        }
                                        else
                                        {
                                            Result = new ResponseAppResult(ResState.OperationFailed, "操作失败,后台服务中断！", "");
                                        }
                                    }
                                    else
                                    {
                                        Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，请停车后再进行检测！", null);
                                    }
                                }
                                else
                                {
                                    Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，OBD未连接！", null);
                                }
                            }
                            else
                            { Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，请将钥匙置为ON档", null);
                              
                            }
                        }
                        else
                        {
                            Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，无状态数据！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "车辆已经离线，无法发送指令！", null);
                        return Result;
                    }

                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "车辆已经离线，无法发送指令！", null);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppSendOBDDiagn调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppGetOBDDiagnResult(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }


            try
            {
              
                string sysflag = inparams["sysflag"];
                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                AlertorResolve ar = new AlertorResolve();
                RDSConfigModel rc = RDSConfig.GetRDS(WebProc.GetAppSysflagKey(sysflag));
                RealtimeDataServer.WCFServiceClient df = new RealtimeDataServer.WCFServiceClient();
                df.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                RealtimeDataServer.CarTransmissionProtocolInfo ctf = df.GetCarTransData(WebProc.GetAppSysflagKey(sysflag), cid);
                byte[] spai = null;

                //OBDDiagn res = new OBDDiagn();
                //res.VIN = "aa12345678935";
                //res.NormalUnitCount = 2;
                //res.FaultyUnitList = new List<OBDDiagn.diagn>();
                //OBDDiagn.diagn rrr = new OBDDiagn.diagn();
                //rrr.FaultyCodes = "12412";
                //rrr.UnitCode = 0x5;
                //rrr.UnitName = "允许进入/启动";
                //rrr.AsmName = "车身系统";
                //res.FaultyUnitList.Add(rrr);
                //OBDDiagn.diagn rrr2 = new OBDDiagn.diagn();
                //rrr2.FaultyCodes = "27452";
                //rrr2.UnitCode = 0x16;
                //rrr2.UnitName = "方向盘";
                //rrr2.AsmName = "车身系统";
                //res.FaultyUnitList.Add(rrr2);
                //res.NormalUnitList = new List<OBDDiagn.diagn>();
                //OBDDiagn.diagn rrr3 = new OBDDiagn.diagn();
                //rrr3.FaultyCodes = "00000";
                //rrr3.UnitCode = 0x1;
                //rrr3.UnitName = "发动机";
                //rrr3.AsmName = "动力总成系统";
                //res.NormalUnitList.Add(rrr3);
                //OBDDiagn.diagn rrr4 = new OBDDiagn.diagn();
                //rrr4.FaultyCodes = "00000";
                //rrr4.UnitCode = 0x3;
                //rrr3.AsmName = "底盘系统";
                // rrr4.UnitName = "ABS制动";
                //res.NormalUnitList.Add(rrr4);
                //Result = new ResponseAppResult(ResState.Success, "操作成功", res);


                if (ctf != null && ctf.TransData != null && ctf.TransData.ContainsKey(0x0FC7))
                {
                    spai = ctf.TransData[0x0FC7];// date[0].TransmissionProtocolInfo;
                    OBDDiagn res = ar.analyzeOBDDiagn(spai);
                    if (res != null)
                    {
                        Result = new ResponseAppResult(ResState.Success, "操作成功", res);
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.RetryAfter, "未收到OBD诊断数据，请稍候重试！", "");
                    }
                }

                else
                {
                    Result = new ResponseAppResult(ResState.RetryAfter, "未收到OBD诊断数据，请稍候重试！", "");
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppSendOBDDiagn调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppSendOBDDriver(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }


            try
            {
                string sysflag = inparams["sysflag"];
                monitor mon = new monitor();
                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                WebGIS.RealtimeDataServer.CarRealData[] RealData = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), new long[] { cid });
                if (RealData.Length > 0)
                {
                    if (RealData[0].OnlineStatus == 1 && RealData[0].sPositionAdditionalInfo != null && RealData[0].sPositionAdditionalInfo.Length > 0)
                    {
                        alarmcarstatuesalarm act = new alarmcarstatuesalarm();
                        act = analyzeStatues(act, RealData[0].sPositionAdditionalInfo);


                        KeyValuePair<int, int> accstates = (KeyValuePair<int, int>)act.carstatues.GetValue(13);
                        if (accstates.Value == 1)
                        {
                            long tno = GetCarTNO(WebProc.GetAppSysflagKey(sysflag), cid.ToString());

                            AlertorResolve ar = new AlertorResolve();
                            byte[] data = new byte[0];
                            bool w = SendOrderHander.Send_CTS_TransmissionProtocol(WebProc.GetAppSysflagKey(sysflag), "", cid, tno, 0x8fc8, data);
                            if (w)
                            {
                                Result = new ResponseAppResult(ResState.Success, "操作成功", "");
                            }
                            else
                            {
                                Result = new ResponseAppResult(ResState.OperationFailed, "操作失败,后台服务中断！", "");
                            }
                        }
                        else
                        {
                            Result = new ResponseAppResult(ResState.OperationFailed, "操作失败，请将钥匙置为ON档！", null);
                        }
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "车辆已经离线，无法发送指令！", null);
                        return Result;
                    }

                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "车辆已经离线，无法发送指令！", null);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppSendOBDDriver调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppGetOBDDriverResult(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }


            try
            {
                string sysflag = inparams["sysflag"];
                long cid = GetCarCIDBySimCode(sysflag, inparams["cid"]);
                AlertorResolve ar = new AlertorResolve();
                RDSConfigModel rc = RDSConfig.GetRDS(WebProc.GetAppSysflagKey(sysflag));
                RealtimeDataServer.WCFServiceClient df = new RealtimeDataServer.WCFServiceClient();
                df.Endpoint.Address = new System.ServiceModel.EndpointAddress(rc.WCFUrl);
                RealtimeDataServer.CarTransmissionProtocolInfo ctf = df.GetCarTransData(WebProc.GetAppSysflagKey(sysflag), cid);
                byte[] spai = null;

                if (ctf != null && ctf.TransData != null && ctf.TransData.ContainsKey(0x0FC8))
                {
                    spai = ctf.TransData[0x0FC8];// date[0].TransmissionProtocolInfo;
                    List<OBDDriver> res = ar.analyzeOBDDriver(spai);

                    Result = new ResponseAppResult(ResState.Success, "操作成功", res);
                }

                else
                {
                    Result = new ResponseAppResult(ResState.RetryAfter, "未收到ODB行车数据，请稍候重试！", "");
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppGetOBDDriverResult调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        private static Array dictcov(Dictionary<int, int> data)
        {


            return data.ToArray();
        }


    }
    public class alarmcartrack
    {
        public int dt;
        public int sp;
        public int lng;
        public int lat;
    }
    public class alarmcarstatuesalarm
    {
        public string cid;
        public string tno;
        public string longt;
        public string lat;
        public string tdatetime;
        public string onlinestatus;
        public string voltage;
        //public string rawdata;
        public Array carstatues;
        public Array caralarms;




    }

}