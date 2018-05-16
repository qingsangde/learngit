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
    public class sta_distributioncarstat
    {
        public ResponseResult GetDistributionCarsStat(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string carno;
            string carownname;
            string uid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("carno") || !inparams.Keys.Contains("carownname"))
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
                carno = inparams["carno"];
                carownname = inparams["carownname"];
                uid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询车辆超速信息
                //todo
                DataTable dt = QueryDistributionCars(sysflag, carno, carownname, stime, etime,uid,onecaruser);
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

        private DataTable QueryDistributionCars(string key, string carno, string carownname, string st, string et,string uid,string onecaruser)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@uid", uid), 
                                            new SqlParameter("@OneCarUser", onecaruser),
                                            new SqlParameter("@carno", carno), 
                                            new SqlParameter("@carownname", carownname),  
                                            new SqlParameter("@begindate", st), 
                                            new SqlParameter("@enddate", et)
                                        };
            //DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SpeedAllSearch"), Parameters).Tables[0];
            return csh.FillDataSet(key, WebProc.Proc("QWGProc_App_ExtVINUploadQuery"), Parameters, null, 3600).Tables[0];
        }


        public ResponseResult GetDistributionCarsStatOutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string carno;
            string carownname;
            string uid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("carno") || !inparams.Keys.Contains("carownname"))
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
                carno = inparams["carno"];
                carownname = inparams["carownname"];
                uid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询车辆超速信息
                //todo
                DataTable dt = QueryDistributionCars(sysflag, carno, carownname, stime, etime,uid,onecaruser);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        dt.Rows[i - 1]["CID"] = i;
                    }
                }

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "所属企业", "车辆VIN码", "时间" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                npoiHelper.WorkbookName = "配送车辆信息" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\Query\";
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }
    }
}