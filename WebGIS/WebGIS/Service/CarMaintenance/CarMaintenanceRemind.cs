using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using WebGIS;
using System.Data;
using System.Data.SqlClient;

namespace SysService
{
    public class CarMaintenanceRemind
    {
        ComSqlHelper csh = new ComSqlHelper();

        public ResponseResult QueryCarMaintenanceRemind(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string carno;
            string token;
            string remindmileage;
            string uid;
            string onecaruser;
            if (!inparams.Keys.Contains("carno") || !inparams.Keys.Contains("token"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {

                if (inparams["token"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统回话标识错误", null);
                    return Result;
                }
                carno = inparams["carno"];
                token = inparams["token"];
                uid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                sysflag = inparams["sysflag"];
                remindmileage = inparams["remindmileage"];

                DataTable cardt = getCars(carno, token);
                DataTable carmaintenancedt = getCarMaintenanceData(sysflag, carno, uid, onecaruser);
                DataTable dtResult = new DataTable();

                if (cardt != null && cardt.Rows.Count > 0)
                {
                    dtResult = GetRemindData(sysflag, cardt, carmaintenancedt, remindmileage);
                }
                int Total = dtResult.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dtResult;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private DataTable GetRemindData(string sysflag, DataTable cardt, DataTable carmaintenancedt, string remindmileage)
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("CID");
            dtResult.Columns.Add("CarNo");
            dtResult.Columns.Add("SimCode");
            dtResult.Columns.Add("OwnTel");
            dtResult.Columns.Add("Mileage");
            dtResult.Columns.Add("MaintenanceMileage");
            dtResult.Columns.Add("MaintenanceTime");
            dtResult.Columns.Add("MaintenanceInterval");
            dtResult.Columns.Add("MaintenanceContent");

            int len = cardt.Rows.Count;
            if (len > 0)
            {
                List<long> cidlist = new List<long>();
                for (int i = 0; i < len; i++)
                {
                    cidlist.Add(Convert.ToInt64(cardt.Rows[i]["cid"].ToString()));
                }
                monitor mon = new monitor();
                WebGIS.RealtimeDataServer.CarRealData[] RealData = mon.CarRealDataByCids(sysflag, cidlist.ToArray());



                for (int i = 0; i < len; i++)
                {
                    DataRow dr = dtResult.NewRow();
                    string cid = cardt.Rows[i]["cid"].ToString();
                    string carno = cardt.Rows[i]["carno"].ToString();
                    string simcode = cardt.Rows[i]["sim"].ToString();
                    string owntel = cardt.Rows[i]["cownt"].ToString();
                    int curmiles = 0;
                    dr["CID"] = cid;
                    dr["CarNo"] = carno;
                    dr["SimCode"] = simcode;
                    dr["OwnTel"] = owntel;
                    WebGIS.RealtimeDataServer.CarRealData realtimeData = GetOneCarRealData(RealData, cid);
                    if (realtimeData != null)
                    {
                        curmiles = realtimeData.SumMiles;                       
                    }

                    dr["Mileage"] = curmiles.ToString();

                    if (carmaintenancedt != null && carmaintenancedt.Rows.Count > 0)
                    {
                        DataRow[] drArr = carmaintenancedt.Select("CID=" + cid.ToString());
                        if (drArr.Length > 0)
                        {
                            int LastMaintenanceMileage = Convert.ToInt32(drArr[0]["MaintenanceMileage"].ToString());
                            int LastMaintenanceInterval = Convert.ToInt32(drArr[0]["MaintenanceInterval"].ToString());
                            if (Int32.Parse(remindmileage) == 0)
                            {
                                dr["MaintenanceMileage"] = drArr[0]["MaintenanceMileage"].ToString();
                                dr["MaintenanceTime"] = drArr[0]["MaintenanceTime"].ToString();
                                dr["MaintenanceInterval"] = drArr[0]["MaintenanceInterval"].ToString();
                                dr["MaintenanceContent"] = drArr[0]["MaintenanceContent"].ToString();
                                dtResult.Rows.Add(dr);
                            }
                            else
                            {
                                if (Int32.Parse(remindmileage) != 0 && ((LastMaintenanceMileage + LastMaintenanceInterval - curmiles) <= Int32.Parse(remindmileage)))
                                {
                                    dr["MaintenanceMileage"] = drArr[0]["MaintenanceMileage"].ToString();
                                    dr["MaintenanceTime"] = drArr[0]["MaintenanceTime"].ToString();
                                    dr["MaintenanceInterval"] = drArr[0]["MaintenanceInterval"].ToString();
                                    dr["MaintenanceContent"] = drArr[0]["MaintenanceContent"].ToString();
                                    dtResult.Rows.Add(dr);
                                }
                            }


                        }
                        else
                        {
                            dr["MaintenanceMileage"] = "";
                            dr["MaintenanceTime"] = "";
                            dr["MaintenanceInterval"] = "";
                            dr["MaintenanceContent"] = "";
                            dtResult.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr["MaintenanceMileage"] = "";
                        dr["MaintenanceTime"] = "";
                        dr["MaintenanceInterval"] = "";
                        dr["MaintenanceContent"] = "";
                        dtResult.Rows.Add(dr);
                    }
                }
            }
            return dtResult;

        }

        private WebGIS.RealtimeDataServer.CarRealData GetOneCarRealData(WebGIS.RealtimeDataServer.CarRealData[] RealData, string cid)
        {
            if (RealData.Length > 0)
            {
                foreach (WebGIS.RealtimeDataServer.CarRealData data in RealData)
                {
                    if (data.Carid.ToString().Trim().Equals(cid.Trim()))
                    {
                        return data;
                    }
                }
            }

            return null;
        }

        private DataTable getCarMaintenanceData(string sysflag, string carno, string uid, string onecaruser)
        {
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@UID", uid);
                oaPara[1] = new SqlParameter("@OneCarUser", onecaruser);
                oaPara[2] = new SqlParameter("@CarNo", carno);

                return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_GetLastMaintenanceRecord"), oaPara).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private DataTable getCars(String carno, string token)
        {
            DataTable dt = null;
            try
            {
                SessionModel sm = new SessionModel();
                sm = SessionManager.GetSession(token);
                dt = sm.cars.Clone();
                DataRow[] rows = sm.cars.Select("carno like '%" + carno + "%'");
                int h = rows.Length;
                for (int i = 0; i < h; i++)
                {
                    dt.ImportRow(rows[i]);
                }
            }
            catch (Exception)
            {
                dt = null;
            }
            return dt;
        }


        public ResponseResult InsertCarMaintenanceRecord(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string cid;
            string mileage;
            string time;
            string interval;
            string content = "";
            try
            {
                cid = inparams["cid"];
                mileage = inparams["mileage"];
                time = inparams["time"];
                interval = inparams["interval"];
                sysflag = inparams["sysflag"];
                content = inparams["content"];

                int res = InsertRecord(sysflag, cid, mileage, time, interval, content);
                if (res > 0)
                {
                    Result = new ResponseResult(ResState.Success, "成功录入保养记录！", "成功录入保养记录！");
                }
                else
                {
                    Result = new ResponseResult(ResState.OtherError, "发生未知错误！", "");
                }
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private int InsertRecord(string sysflag, string cid, string mileage, string time, string interval, string content)
        {
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[5];
                oaPara[0] = new SqlParameter("@CID", cid);
                oaPara[1] = new SqlParameter("@MaintenanceMileage", mileage);
                oaPara[2] = new SqlParameter("@MaintenanceTime", time);
                oaPara[3] = new SqlParameter("@MaintenanceInterval", interval);
                oaPara[4] = new SqlParameter("@MaintenanceContent", content);

                return csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("QWGProc_QM_InsertMaintenanceRecord"), oaPara, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}