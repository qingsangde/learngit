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
    public class sta_mileagecollect
    {
        /// <summary>
        /// 获取里程统计信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilCollect(Dictionary<string, string> inparams)
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
                //调用存储过程查询里程统计信息
                //todo
                DataTable dt = GetMilCollectByIds(sysflag, sysuid, stime, etime, carnum, cuid, carownname, line, onecaruser);
                //dt.Merge(
                dt.Columns.Add("No_ID", Type.GetType("System.String"));
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    dt.Rows[i - 1]["No_ID"] = i;
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
        /// 获取里程统计详细信息导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilCollectInfooutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            int cid;
            string sysflag;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag"))
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
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = int.Parse(inparams["cid"].ToString());
                sysflag = inparams["sysflag"];

                //调用存储过程查询里程统计详细信息
                //todo
                DataTable dt = GetCollectInfoById(sysflag, cid, stime, etime);

                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    dt.Rows[i - 1]["CID"] = i;
                }
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "日期", "起始时间", "起始经纬度", "结束时间", "结束经纬度", "当日里程", "总里程" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                npoiHelper.WorkbookName = "里程统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\Query\";
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }

        }

        /// <summary>
        /// 查询里程统计信息导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilCollectoutput(Dictionary<string, string> inparams)
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
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime")  || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("carnum") || !inparams.Keys.Contains("cuid") || !inparams.Keys.Contains("carownname") || !inparams.Keys.Contains("line"))
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
                //调用存储过程查询查询里程统计信息
                //todo
                DataTable contentData = GetMilCollectByIds(sysflag, sysuid, stime, etime, carnum, cuid, carownname, line,onecaruser);
                for (int i = 1; i <= contentData.Rows.Count; i++)
                {
                    contentData.Rows[i - 1]["cid"] = i;
                }
                contentData.Columns.Remove("CTOIL");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "起始时间", "结束时间", "起始地址", "结束地址", "行驶里程", "行驶油耗" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "里程统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\Query\";
                string sd = basepath + filePath;
                npoiHelper.saveExcel(sd);

                Result = new ResponseResult(ResState.Success, "", filePath + npoiHelper.WorkbookName);
                return Result;
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OtherError, "", ex.Message + ex.StackTrace);
                return Result;
            }
        }

        /// <summary>
        /// 调用存储过程获取里程统计列表
        /// </summary>
        /// <param name="sysflag">系统标识</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetMilCollectByIds(string sysflag, string uid, string stime, string etime, string carnum, string cuid, string carownname, string line, string onecaruser)
        {
            ComSqlHelper csh = new ComSqlHelper();
            //int fgcd = 500;
            //cid = cid.TrimEnd(',');
            //string[] cidarray = cid.Split(',');
            //string subcids = "";
            //int count = 0;
            //DataTable resdt = null;
            //for (int i = 0; i < cidarray.Length; i++)
            //{
            //    subcids += cidarray[i] + ",";
            //    count++;
            //    if (count >= fgcd || i == cidarray.Length - 1)
            //    {
            //        //subcids = subcids.TrimEnd(',');
            SqlParameter[] Parameters = { new SqlParameter("@T_TimeBegin", stime), new SqlParameter("@T_TimeEnd", etime), new SqlParameter("@CarNum", carnum), new SqlParameter("@CUID", cuid), new SqlParameter("@CarOwnName", carownname), new SqlParameter("@Line", line), new SqlParameter("@UID", uid), new SqlParameter("@OneCarUser", onecaruser) };
                    return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_MileageCollectNewTest"), Parameters, null, 3600).Tables[0];
            //        DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_MileageCollect"), Parameters).Tables[0];
            //        if (resdt == null)
            //        {
            //            resdt = tempdt.Copy();
            //        }
            //        else
            //        {
            //            //foreach (DataRow dr in tempdt.Rows)
            //            //{
            //            //    resdt.ImportRow(dr);
            //            //}
            //            resdt.Merge(tempdt);
            //        }
            //        count = 0;
            //        subcids = "";
            //    }
            //}
            //return resdt;
        }

        /// <summary>
        /// 获取里程统计详细信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getCollectInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            int cid;
            string sysflag;
            int alarmtype;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag"))
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
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = int.Parse(inparams["cid"].ToString());
                sysflag = inparams["sysflag"];


                //调用存储过程查询里程明细数据
                //todo
                DataTable dt = GetCollectInfoById(sysflag, cid, stime, etime);

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
        /// 调用存储过程获取里程统计详细信息
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetCollectInfoById(string sysflag, int cid, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@C_No", cid), new SqlParameter("@T_TimeBegin", stime), new SqlParameter("@T_TimeEnd", etime) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_MileageDetails"), Parameters, null, 300).Tables[0];
        }

    }



}