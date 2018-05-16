using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;
using WebGIS;

namespace SysService
{
    public class FenceAlarmStatistic
    {
        public ResponseResult FenceAlarmStatList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string carno;
            string cuid;
            string ownname;
            string line;
            string st;
            string et;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                carno = inparams["carno"];
                cuid = inparams["cuid"];
                ownname = inparams["ownname"];
                line = inparams["line"];
                st = inparams["st"];
                et = inparams["et"];


                DataTable dt = getFenceAlarmStatList(sysflag, uid, carno, cuid, ownname, line, st, et);
                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                res.isallresults = 0;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }

        public ResponseResult FenceAlarmStatListExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string carno;
            string cuid;
            string ownname;
            string line;
            string st;
            string et;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                carno = inparams["carno"];
                cuid = inparams["cuid"];
                ownname = inparams["ownname"];
                line = inparams["line"];
                st = inparams["st"];
                et = inparams["et"];


                DataTable contentData = getFenceAlarmStatList(sysflag, uid, carno, cuid, ownname, line, st, et);

                contentData.Columns.Remove("cid");
                contentData.Columns.Remove("Line");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "所属企业", "车辆用途", "进区域次数", "出区域次数" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "电子围栏报警统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }
        public ResponseResult FenceAlarmStatDetail(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string cid;
            string st;
            string et;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                cid = inparams["cid"];
                st = inparams["st"];
                et = inparams["et"];


                DataTable dt = getFenceAlarmStatDetailList(sysflag, uid, cid, st, et);
                int Total = dt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = dt;
                res.isallresults = 0;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }


        public ResponseResult FenceAlarmStatDetailExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string uid;
            string cid;
            string st;
            string et;

            try
            {
                sysflag = inparams["sysflag"];
                uid = inparams["uid"];
                cid = inparams["cid"];
                st = inparams["st"];
                et = inparams["et"];


                DataTable contentData = getFenceAlarmStatDetailList(sysflag, uid, cid, st, et);
                contentData.Columns.Remove("cid");
                contentData.Columns.Remove("Line");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "报警区域", "报警类型", "报警时间", "车牌号", "所属企业", "车辆用途" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "电子围栏报警统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }


        private DataTable getFenceAlarmStatList(string key, string uid, string carno, string cuid, string ownname, string line, string st, string et)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[7];
                oaPara[0] = new SqlParameter("@UID", Int32.Parse(uid));
                oaPara[1] = new SqlParameter("@CarNo", carno);
                oaPara[2] = new SqlParameter("@CUID", cuid);
                oaPara[3] = new SqlParameter("@CarOwnName", ownname);
                oaPara[4] = new SqlParameter("@Line", line);
                oaPara[5] = new SqlParameter("@StartTime", st);
                oaPara[6] = new SqlParameter("@EndTime", et);

                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("QSProc_QS_FenceAlarmStat"), oaPara, "fencealarmstattable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private DataTable getFenceAlarmStatDetailList(string key, string uid, string cid, string st, string et)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[4];
                oaPara[0] = new SqlParameter("@UID", Int32.Parse(uid));
                oaPara[1] = new SqlParameter("@Cid", long.Parse(cid));
                oaPara[2] = new SqlParameter("@StartTime", st);
                oaPara[3] = new SqlParameter("@EndTime", et);

                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("QSProc_QS_FenceAlarmStatDetail"), oaPara, "fencealarmdetailtable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}