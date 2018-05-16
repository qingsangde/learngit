using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;

namespace SysService
{
    public class sta_historyphoto
    {
        public ResponseResult queryHistoryPhotos(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string carnum;
            string cuid;
            string carownname;
            string line;
            string sysuid;
            string onecaruser;
            string tdh;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["etime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                
                stime = inparams["stime"];
                etime = inparams["etime"];
                sysflag = inparams["sysflag"];
                carnum = inparams["carnum"];
                cuid = inparams["cuid"];
                carownname = inparams["carownname"];
                line = inparams["line"];
                sysuid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                tdh = inparams["tdh"];
                //调用存储过程查询车辆超速信息
                //todo
                DataTable dt = GetHistoryPhotos(sysflag, sysuid, stime, etime, carnum, cuid, carownname, line, onecaruser,tdh);
                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private DataTable GetHistoryPhotos(string sysflag, string sysuid, string stime, string etime, string carnum, string cuid, string carownname, string line, string onecaruser, string tdh)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@StartTime", stime), 
                                            new SqlParameter("@EndTime", etime),  
                                            new SqlParameter("@CarNo", carnum), 
                                            new SqlParameter("@CUID", cuid), 
                                            new SqlParameter("@CarOwnName", carownname), 
                                            new SqlParameter("@Line", line), 
                                            new SqlParameter("@UID", sysuid),
                                            new SqlParameter("@OneCarUser",onecaruser),
                                            new SqlParameter("@Channel",tdh)
                                        };
            //DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SpeedAllSearch"), Parameters).Tables[0];
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_CarPhotos"), Parameters, null, 3600).Tables[0];
        }

        public ResponseResult queryOneCarHistoryPhoto(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string cid;
            string tdh;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("cid"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["etime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                stime = inparams["stime"];
                etime = inparams["etime"];
                sysflag = inparams["sysflag"];
                cid = inparams["cid"];
                tdh = inparams["tdh"];
                //调用存储过程查询车辆超速信息
                //todo
                DataTable dt = GetOneCarHistoryPhotos(sysflag, stime, etime, cid,tdh);
                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        private DataTable GetOneCarHistoryPhotos(string sysflag, string stime, string etime, string cid, string tdh)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@StartTime", stime), 
                                            new SqlParameter("@EndTime", etime),  
                                            new SqlParameter("@CID", cid),
                                            new SqlParameter("@Channel",tdh)
                                        };
            //DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SpeedAllSearch"), Parameters).Tables[0];
            return csh.FillDataSet(sysflag, WebProc.Proc("QSProc_Photo_SelectByCar"), Parameters, null, 3600).Tables[0];
        }
    }
}