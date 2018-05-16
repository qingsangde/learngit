using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Web;


namespace SysService
{
    public class sta_countlogin
    {
        /// <summary>
        /// 获取用户登录统计信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCountLogin(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string uname;
            string sysflag;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("uname") || !inparams.Keys.Contains("sysflag"))
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
                if (inparams["uname"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "用户错误", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                uname = inparams["uname"];
                sysflag = inparams["sysflag"];
                //调用存储过程查询用户登录统计信息
                //todo
                DataTable dt = GetUCountLogin(sysflag, uname, stime, etime);
                dt.Columns.Add("NO", Type.GetType("System.String"));
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    dt.Rows[i - 1]["NO"] = i;
                }

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

        /// <summary>
        /// 调用存储过程获取用户登录统计信息
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="uname">用户名</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetUCountLogin(string sysflag, string uname, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@Company", uname), new SqlParameter("@startTime", stime), new SqlParameter("@endtime", etime) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QS_SearchLogin"), Parameters, "userlogintb", 180).Tables[0];

        }

        /// <summary>
        /// 获取用户登录统计信息导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCountLoginOut(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string uname;
            string sysflag;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("uname") || !inparams.Keys.Contains("sysflag"))
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
                if (inparams["uname"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "用户错误", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                uname = inparams["uname"];
                sysflag = inparams["sysflag"];
                //调用存储过程查询用户登录统计信息
                //todo
                DataTable contentData = GetUCountLogin(sysflag, uname, stime, etime);
                contentData.Columns.Add("NO").SetOrdinal(0);
                for (int i = 1; i <= contentData.Rows.Count; i++)
                {
                    contentData.Rows[i - 1]["NO"] = i;
                }
                contentData.Columns.Remove("Column1");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "用户名称", "登录状态", "登录时间", "退出状态", "退出时间" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "用户登录统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath =  @"UI\Excel\Query\" ;
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch(Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }
        }
    }



}