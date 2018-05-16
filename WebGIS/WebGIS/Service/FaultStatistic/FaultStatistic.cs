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
    public class FaultStatistic
    {
        public ResponseResult QueryFaultDetailList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string cid;

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
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"];
                sysflag = inparams["sysflag"];

                DataTable originalDt = GetFaultDetailList(sysflag, cid, stime, etime);

                DataTable resultDt = FormatDetailData(originalDt);

                int Total = resultDt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = resultDt;
                Result = new ResponseResult(ResState.Success, "", res);

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }


        public ResponseResult ExportFaultDetailList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string cid;

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
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                stime = inparams["stime"];
                etime = inparams["etime"];
                cid = inparams["cid"];
                sysflag = inparams["sysflag"];

                DataTable originalDt = GetFaultDetailList(sysflag, cid, stime, etime);

                DataTable contentData = FormatDetailData(originalDt);

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "时间", "故障单元编号", "故障单元名称", "故障码" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "故障码统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\";
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

        private DataTable FormatDetailData(DataTable originalDt)
        {
            DataTable resultDt = new DataTable();

            DataColumn dc0 = new DataColumn();
            resultDt.Columns.Add(new DataColumn("CarNo", Type.GetType("System.String")));
            resultDt.Columns.Add(new DataColumn("T_DateTime", Type.GetType("System.DateTime")));
            resultDt.Columns.Add(new DataColumn("UnitCode", Type.GetType("System.String")));
            //resultDt.Columns.Add(new DataColumn("AsmName", Type.GetType("System.String")));
            resultDt.Columns.Add(new DataColumn("UnitName", Type.GetType("System.String")));
            resultDt.Columns.Add(new DataColumn("FaultyCodes", Type.GetType("System.String")));

            if (originalDt != null && originalDt.Rows.Count > 0)
            {
                AlertorResolve ar = new AlertorResolve();
                int count = originalDt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        OBDDiagn obdDiagn = ar.analyzeOBDDiagn((byte[])(originalDt.Rows[i]["vData"]));
                        List<CommLibrary.OBDDiagn.diagn> powerAsmList = obdDiagn.PowerAsmList;  //动力总成系统
                        List<CommLibrary.OBDDiagn.diagn> underpanAsmList = obdDiagn.UnderpanAsmList;  //底盘系统
                        List<CommLibrary.OBDDiagn.diagn> carBodyAsmList = obdDiagn.CarBodyAsmList;  //车身系统
                        List<CommLibrary.OBDDiagn.diagn> networkAsmList = obdDiagn.NetworkAsmList;  //网络系统
                        if (powerAsmList.Count > 0)
                        {
                            foreach (CommLibrary.OBDDiagn.diagn obddiagn in powerAsmList)
                            {
                                if (!obddiagn.FaultyCodes.Equals("00000000"))
                                {
                                    DataRow dr = resultDt.NewRow();
                                    dr["CarNo"] = originalDt.Rows[i]["CarNo"].ToString();
                                    dr["T_DateTime"] = Convert.ToDateTime(originalDt.Rows[i]["T_DateTime"]);
                                    //dr["AsmName"] = obddiagn.AsmName;
                                    dr["UnitName"] = obddiagn.UnitName;
                                    dr["UnitCode"] = obddiagn.UnitCode.ToString("X2");
                                    dr["FaultyCodes"] = obddiagn.FaultyCodes;
                                    resultDt.Rows.Add(dr);
                                }
                            }
                        }

                        if (underpanAsmList.Count > 0)
                        {
                            foreach (CommLibrary.OBDDiagn.diagn obddiagn in underpanAsmList)
                            {
                                if (!obddiagn.FaultyCodes.Equals("00000000"))
                                {
                                    DataRow dr = resultDt.NewRow();
                                    dr["CarNo"] = originalDt.Rows[i]["CarNo"].ToString();
                                    dr["T_DateTime"] = Convert.ToDateTime(originalDt.Rows[i]["T_DateTime"]);
                                    //dr["AsmName"] = obddiagn.AsmName;
                                    dr["UnitName"] = obddiagn.UnitName;
                                    dr["UnitCode"] = obddiagn.UnitCode.ToString("X2");
                                    dr["FaultyCodes"] = obddiagn.FaultyCodes;
                                    resultDt.Rows.Add(dr);
                                }
                            }
                        }

                        if (carBodyAsmList.Count > 0)
                        {
                            foreach (CommLibrary.OBDDiagn.diagn obddiagn in carBodyAsmList)
                            {
                                if (!obddiagn.FaultyCodes.Equals("00000000"))
                                {
                                    DataRow dr = resultDt.NewRow();
                                    dr["CarNo"] = originalDt.Rows[i]["CarNo"].ToString();
                                    dr["T_DateTime"] = Convert.ToDateTime(originalDt.Rows[i]["T_DateTime"]);
                                    //dr["AsmName"] = obddiagn.AsmName;
                                    dr["UnitName"] = obddiagn.UnitName;
                                    dr["UnitCode"] = obddiagn.UnitCode.ToString("X2");
                                    dr["FaultyCodes"] = obddiagn.FaultyCodes;
                                    resultDt.Rows.Add(dr);
                                }
                            }
                        }

                        if (networkAsmList.Count > 0)
                        {
                            foreach (CommLibrary.OBDDiagn.diagn obddiagn in networkAsmList)
                            {
                                if (!obddiagn.FaultyCodes.Equals("00000000"))
                                {
                                    DataRow dr = resultDt.NewRow();
                                    dr["CarNo"] = originalDt.Rows[i]["CarNo"].ToString();
                                    dr["T_DateTime"] = Convert.ToDateTime(originalDt.Rows[i]["T_DateTime"]);
                                    //dr["AsmName"] = obddiagn.AsmName;
                                    dr["UnitName"] = obddiagn.UnitName;
                                    dr["UnitCode"] = obddiagn.UnitCode.ToString("X2");
                                    dr["FaultyCodes"] = obddiagn.FaultyCodes;
                                    resultDt.Rows.Add(dr);
                                }
                            }
                        }

                    }
                    catch
                    {
                        //
                    }
                }

            }
            //resultDt.DefaultView.Sort = 
            DataView dv = resultDt.DefaultView;
            dv.Sort = "T_DateTime ASC,UnitCode ASC ";
            DataTable dt2 = dv.ToTable();
            return dt2;
        }

        private DataTable GetFaultDetailList(string sysflag, string cid, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@StartTime", stime), 
                                            new SqlParameter("@EndTime", etime)
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_FaultCodeStatisticDetail"), Parameters, null, 3600).Tables[0];
        }

        public ResponseResult QueryFaultList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string uid;
            string onecaruser;
            string carno;
            string simcode;
            string vehicletype;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag"))
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
                uid = inparams["uid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                carno = inparams["carno"];
                simcode = inparams["simcode"];
                vehicletype = inparams["vehicletype"];

                DataTable originalDt = GetFaultList(sysflag, uid, onecaruser, carno, simcode, vehicletype, stime, etime);

                DataTable resultDt = FormatData(originalDt);

                int Total = resultDt.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = resultDt;
                Result = new ResponseResult(ResState.Success, "", res);

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }

        public ResponseResult ExportFaultList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string uid;
            string onecaruser;
            string carno;
            string simcode;
            string vehicletype;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("sysflag"))
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
                uid = inparams["uid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                carno = inparams["carno"];
                simcode = inparams["simcode"];
                vehicletype = inparams["vehicletype"];

                DataTable originalDt = GetFaultList(sysflag, uid, onecaruser, carno, simcode, vehicletype, stime, etime);

                DataTable contentData = FormatData(originalDt);
                contentData.Columns.Remove("CID");
                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "SIM卡号", "车主电话", "终端号", "车辆类型", "故障数" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "故障码统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                // 设置导入内容
                npoiHelper.HeaderData = headerDataArray;
                npoiHelper.ContentData = contentDataArray;
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                string filePath = @"UI\Excel\";
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

        private DataTable FormatData(DataTable originalDt)
        {
            AlertorResolve ar = new AlertorResolve();
            DataTable resultDt = new DataTable();
            resultDt.Columns.Add("CID");
            resultDt.Columns.Add("CarNo");
            resultDt.Columns.Add("SimCode");
            resultDt.Columns.Add("CarOwnTel");
            resultDt.Columns.Add("TNO");
            resultDt.Columns.Add("VehicleType");
            resultDt.Columns.Add("FaultCount");

            DataView dataView = originalDt.DefaultView;
            DataTable carDt = dataView.ToTable(true, "CID", "CarNo", "SimCode", "CarOwnTel", "TNO", "VehicleType");

            if (carDt != null && carDt.Rows.Count > 0)
            {
                int carcount = carDt.Rows.Count;
                for (int i = 0; i < carcount; i++)
                {
                    int fCount = 0;
                    DataRow dr = resultDt.NewRow();
                    string CID = carDt.Rows[i]["CID"].ToString().Trim();
                    dr["CID"] = CID;
                    dr["CarNo"] = carDt.Rows[i]["CarNo"].ToString().Trim();
                    dr["SimCode"] = carDt.Rows[i]["SimCode"].ToString().Trim();
                    dr["CarOwnTel"] = carDt.Rows[i]["CarOwnTel"].ToString().Trim();
                    dr["TNO"] = carDt.Rows[i]["TNO"].ToString().Trim();
                    dr["VehicleType"] = carDt.Rows[i]["VehicleType"].ToString().Trim();

                    DataRow[] faultDrs = originalDt.Select("CID=" + CID);
                    if (faultDrs != null && faultDrs.Length > 0)
                    {
                        int count = faultDrs.Length;
                        for (int j = 0; j < count; j++)
                        {
                            OBDDiagn obdDiagn = ar.analyzeOBDDiagn((byte[])(faultDrs[j]["vData"]));
                            List<CommLibrary.OBDDiagn.diagn> powerAsmList = obdDiagn.PowerAsmList;  //动力总成系统
                            List<CommLibrary.OBDDiagn.diagn> underpanAsmList = obdDiagn.UnderpanAsmList;  //底盘系统
                            List<CommLibrary.OBDDiagn.diagn> carBodyAsmList = obdDiagn.CarBodyAsmList;  //车身系统
                            List<CommLibrary.OBDDiagn.diagn> networkAsmList = obdDiagn.NetworkAsmList;  //网络系统
                            if (powerAsmList.Count > 0)
                            {
                                foreach (CommLibrary.OBDDiagn.diagn obddiagn in powerAsmList)
                                {
                                    if (!obddiagn.FaultyCodes.Equals("00000000"))
                                    {
                                        fCount++;
                                    }
                                }
                            }

                            if (underpanAsmList.Count > 0)
                            {
                                foreach (CommLibrary.OBDDiagn.diagn obddiagn in underpanAsmList)
                                {
                                    if (!obddiagn.FaultyCodes.Equals("00000000"))
                                    {
                                        fCount++;
                                    }
                                }
                            }

                            if (carBodyAsmList.Count > 0)
                            {
                                foreach (CommLibrary.OBDDiagn.diagn obddiagn in carBodyAsmList)
                                {
                                    if (!obddiagn.FaultyCodes.Equals("00000000"))
                                    {
                                        fCount++;
                                    }
                                }
                            }

                            if (networkAsmList.Count > 0)
                            {
                                foreach (CommLibrary.OBDDiagn.diagn obddiagn in networkAsmList)
                                {
                                    if (!obddiagn.FaultyCodes.Equals("00000000"))
                                    {
                                        fCount++;
                                    }
                                }
                            }

                        }
                    }

                    dr["FaultCount"] = fCount;
                    resultDt.Rows.Add(dr);
                }

            }

            return resultDt;
        }

        private DataTable GetFaultList(string sysflag, string uid, string onecaruser, string carno, string simcode, string vehicletype, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@UID", uid), 
                                            new SqlParameter("@OneCarUser", onecaruser), 
                                            new SqlParameter("@CarNo", carno),
                                            new SqlParameter("@SIMCode", simcode), 
                                            new SqlParameter("@VehicleType", vehicletype), 
                                            new SqlParameter("@StartTime", stime),
                                            new SqlParameter("@EndTime", etime)
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_FaultCodeStatisticList"), Parameters, null, 3600).Tables[0];
        }
    }
}