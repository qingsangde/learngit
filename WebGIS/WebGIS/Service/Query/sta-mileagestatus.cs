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
    public class sta_mileagestatus
    {

        /// <summary>
        /// 警情统计查询
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilStatus(Dictionary<string, string> inparams)
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
                //调用存储过程查询警情信息
                //todo
                DataTable dt = GetMilStatusByIds(sysflag, sysuid, stime, etime, carnum, cuid, carownname, line,onecaruser);
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
        /// 警情统计详细导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilStatusInfooutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            int cid;
            string sysflag;
            int alarmtype;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("alarmtype"))
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
                if (inparams["alarmtype"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "警情类型错误", null);
                    return Result;
                }

                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = int.Parse(inparams["cid"].ToString());
                sysflag = inparams["sysflag"];
                alarmtype = int.Parse(inparams["alarmtype"].ToString());
                //调用存储过程查询警情统计详细
                //todo
                DataTable dt = GetMilStatusInfoById(sysflag, cid, stime, etime, alarmtype);
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    dt.Rows[i - 1]["No_ID"] = i;
                }
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "序号", "车牌号", "GPS时间", "纬度-经度", "警情状态" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(dt);
                npoiHelper.WorkbookName = "警情统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        /// 警情统计导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilStatusoutput(Dictionary<string, string> inparams)
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
                //调用存储过程查询警情数据
                //todo
                DataTable contentData = GetMilStatusByIds(sysflag, sysuid, stime, etime, carnum, cuid, carownname, line,onecaruser);
                contentData.Columns.Remove("CID");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "公司名称", "劫警", "剪线报警", "超速统计" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "警情统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        /// 调用存储过程查询警情数据
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetMilStatusByIds(string sysflag, string uid, string stime, string etime, string carnum, string cuid, string carownname, string line,string onecaruser)
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
            //        subcids = subcids.TrimEnd(',');
            SqlParameter[] Parameters = { new SqlParameter("@T_TimeBegin", stime), new SqlParameter("@T_TimeEnd", etime), new SqlParameter("@CarNum", carnum), new SqlParameter("@CUID", cuid), new SqlParameter("@CarOwnName", carownname), new SqlParameter("@Line", line), new SqlParameter("@UID", uid), new SqlParameter("@OneCarUser", onecaruser) };
                    return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_MileageStatusTest"), Parameters, null, 3600).Tables[0];
            //        DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_MileageStatus"), Parameters, null, 100).Tables[0];
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
        /// 查询警情详细信息
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getMilStatusInfo(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            int cid;
            string sysflag;
            int alarmtype;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("alarmtype"))
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
                if (inparams["alarmtype"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "警情类型错误", null);
                    return Result;
                }
                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = int.Parse(inparams["cid"].ToString());
                sysflag = inparams["sysflag"];
                alarmtype = int.Parse(inparams["alarmtype"].ToString());

                //调用存储过程查询警情详细信息
                //todo
                DataTable dt = GetMilStatusInfoById(sysflag, cid, stime, etime, alarmtype);
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
        /// 调用存储过程查询警情详细信息
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <param name="alarmtype">警情类型</param>
        /// <returns></returns>
        public static DataTable GetMilStatusInfoById(string sysflag, int cid, string stime, string etime, int alarmtype)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@C_ID", cid), new SqlParameter("@T_TimeBegin", stime), new SqlParameter("@T_TimeEnd", etime), new SqlParameter("@AlarmType", alarmtype) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_AlarmDetails"), Parameters, null, 300).Tables[0];

        }

    }



}