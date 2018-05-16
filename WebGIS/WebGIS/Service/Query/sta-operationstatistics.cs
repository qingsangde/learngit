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
    public class sta_operationstatistics
    {

        /// <summary>
        /// 车辆运行统计查询
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getOperationStatistics(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string CarNum;
            string CUID;
            string CarOwnName;
            string Line;
            string sysuid;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("CarNum") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("CUID") || !inparams.Keys.Contains("CarOwnName") || !inparams.Keys.Contains("Line"))
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
                CarNum = inparams["CarNum"];
                CUID = inparams["CUID"];
                CarOwnName = inparams["CarOwnName"];
                Line = inparams["Line"];
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询警情信息
                //todo
                DataTable dt = GetOperationStatisticsByIds(sysflag, sysuid, stime, etime, CarNum, CUID, CarOwnName, Line,onecaruser);
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
        /// 车辆运行统计导出
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getOStatisticsoutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string CarNum;
            string CUID;
            string CarOwnName;
            string Line;
            string sysuid;
            string sysflag;
            string onecaruser;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("CarNum") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("CUID") || !inparams.Keys.Contains("CarOwnName") || !inparams.Keys.Contains("Line"))
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
                CarNum = inparams["CarNum"];
                CUID = inparams["CUID"];
                CarOwnName = inparams["CarOwnName"];
                Line = inparams["Line"];
                sysflag = inparams["sysflag"];
                sysuid = inparams["sysuid"];
                onecaruser = inparams["onecaruser"];
                //调用存储过程查询警情数据
                //todo

                DataTable contentData = GetOperationStatisticsByIds(sysflag, sysuid, stime, etime, CarNum, CUID, CarOwnName, Line,onecaruser);
                contentData.Columns.Remove("CID");
                contentData.Columns.Add("StartAddress");
                contentData.Columns.Add("EndAddress");

                List<CommLibrary.AddressConvert.DLngLat> corrds = new List<AddressConvert.DLngLat>();
                for (int i = 0; i < contentData.Rows.Count; i++)
                {
                    string sc = contentData.Rows[i]["StartCoord"].ToString();
                    sc = string.IsNullOrEmpty(sc) ? "0-0" : sc;
                    string slng = sc.Split('-')[1];
                    string slat = sc.Split('-')[0];
                    string ec = contentData.Rows[i]["EndCoord"].ToString();
                    ec = string.IsNullOrEmpty(ec) ? "0-0" : ec;
                    string elng = ec.Split('-')[1];
                    string elat = ec.Split('-')[0];
                    CommLibrary.AddressConvert.DLngLat sdl = new AddressConvert.DLngLat();
                    sdl.Lat = double.Parse(slat);
                    sdl.Lng = double.Parse(slng);
                    CommLibrary.AddressConvert.DLngLat edl = new AddressConvert.DLngLat();
                    edl.Lat = double.Parse(elat);
                    edl.Lng = double.Parse(elng);
                    corrds.Add(sdl);
                    corrds.Add(edl);
                }
                string[] address = AddressConvert.AddConvertBatch(corrds);
                for (int i = 0; i < contentData.Rows.Count; i++)
                {
                    contentData.Rows[i]["StartAddress"] = address[i * 2];
                    contentData.Rows[i]["EndAddress"] = address[i * 2 + 1];
                }


                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "所属企业", "车辆用途", "运营线路", "开始时间", "开始速度", "结束时间", "结束速度", "开始经纬度", "结束经纬度", "开始地址", "结束地址" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "车辆运行统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        /// 调用存储过程查询车辆运行统计数据
        /// </summary>
        /// <param name="sysflag">系统标志</param>
        /// <param name="cid">车辆ID</param>
        /// <param name="stime">开始时间</param>
        /// <param name="etime">结束时间</param>
        /// <returns></returns>
        public static DataTable GetOperationStatisticsByIds(string sysflag, string uid, string stime, string etime, string carnum, string cuid, string carownname, string line,string onecaruser)
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
            SqlParameter[] Parameters = { new SqlParameter("@UID", uid), new SqlParameter("@T_TimeBegin", stime), new SqlParameter("@T_TimeEnd", etime), new SqlParameter("@CarNum", carnum), new SqlParameter("@CUID", cuid), new SqlParameter("@CarOwnName", carownname), new SqlParameter("@Line", line), new SqlParameter("@OneCarUser", onecaruser) };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QC_CarRunStatusTest"), Parameters, null, 3600).Tables[0];
            //DataTable tempdt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QC_CarRunStatus"), Parameters, null, 100).Tables[0];
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
            //int rc = resdt.Rows.Count;
            //return resdt;
        }
    }
}