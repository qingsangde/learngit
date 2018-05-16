using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using System.Data.SqlClient;
using WebGIS;
using System.Data;

namespace SysService
{
    public class sta_MonthTestDriveAnalyse
    {
        public ResponseResult getMonthAnalyseList(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;
            string SSalesRegion;
            string SSalesProvince;
            string SDealersName;
            string SDealersCode;
            string SCarType;
            string SCarLicence;
            string STime;
            string CIDs = "";
            try
            {
                sysflag = inparams["sysflag"];
                SSalesRegion = inparams["SSalesRegion"];
                SSalesProvince = inparams["SSalesProvince"];
                SDealersName = inparams["SDealersName"];
                SDealersCode = inparams["SDealersCode"];
                SCarType = inparams["SCarType"];
                SCarLicence = inparams["SCarLicence"];
                STime = inparams["STime"];

                //获取查询车辆ID
                DataTable dt = getCIDs(sysflag, SSalesRegion, SSalesProvince, SDealersName, SDealersCode, SCarType, SCarLicence);
                int Total = dt.Rows.Count;
                for (int i = 0; i < Total; i++)
                {
                    CIDs = CIDs + dt.Rows[i][0].ToString() + ",";
                }
                if (CIDs.Equals(""))
                {
                    Result = new ResponseResult(ResState.OperationFailed, "所查询经销商不存在试驾车数据！", "0");
                    return Result;
                }

                CIDs = CIDs.Substring(0, CIDs.Length - 1);

                //获取查询结果
                DataTable rs = getList(sysflag, CIDs, STime);

                if (rs.Rows.Count == 0)
                {
                    Result = new ResponseResult(ResState.OperationFailed, "未检索到数据！", "0");
                    return Result;
                }

                else
                {   /****************************生成未行转列的表格数据*******************************/
                    rs.Columns.Add("TDATE", typeof(string));
                    rs.Columns.Add("TMPERCENT", typeof(double));
                    rs.Columns.Add("TTPERCENT", typeof(double));
                    rs.Columns.Add("OMPERCENT", typeof(double));
                    rs.Columns.Add("OTPERCENT", typeof(double));
                    int ttcount = 0;
                    double ttdrivemiles = 0.00;
                    double ttotalmiles = 0.00;
                    int ttdrivetime = 0;
                    int ttotaltime = 0;
                    int tocount = 0;
                    double todrivemiles = 0.00;
                    int todrivetime = 0;

                    foreach (DataRow rr in rs.Rows)
                    {
                        rr["TDATE"] = rr["MONTH"].ToString() + "月";
                        rr["TMPERCENT"] = ((int)(double.Parse(rr["TDRIVEMILES"].ToString())
                                        / double.Parse(rr["TDRIVE_TOTAL_MILES"].ToString()) * 100 + 0.5)) / 100.00;
                        rr["TTPERCENT"] = ((int)(double.Parse(rr["TDRIVE_TIME"].ToString())
                                        / double.Parse(rr["TDRIVE_TOTAL_TIME"].ToString()) * 100 + 0.5)) / 100.00;
                        rr["OMPERCENT"] = ((int)(double.Parse(rr["ORANGE_MILES"].ToString())
                                        / double.Parse(rr["TDRIVE_TOTAL_MILES"].ToString()) * 100 + 0.5)) / 100.00;
                        rr["OTPERCENT"] = ((int)(double.Parse(rr["ORANGE_TIME"].ToString())
                                        / double.Parse(rr["TDRIVE_TOTAL_TIME"].ToString()) * 100 + 0.5)) / 100.00;
                        ttcount = ttcount + int.Parse(rr["TCOUNT"].ToString());
                        ttdrivemiles = ttdrivemiles + double.Parse(rr["TDRIVEMILES"].ToString());
                        ttotalmiles = ttotalmiles + double.Parse(rr["TDRIVE_TOTAL_MILES"].ToString());
                        ttdrivetime = ttdrivetime + int.Parse(rr["TDRIVE_TIME"].ToString());
                        ttotaltime = ttotaltime + int.Parse(rr["TDRIVE_TOTAL_TIME"].ToString());
                        tocount = tocount + int.Parse(rr["ORANGE_COUNT"].ToString());
                        todrivemiles = todrivemiles + double.Parse(rr["ORANGE_MILES"].ToString());
                        todrivetime = todrivetime + int.Parse(rr["ORANGE_TIME"].ToString());
                    }
                    DataRow r0 = rs.NewRow();
                    r0["TDATE"] = "合计";
                    r0["TCOUNT"] = ttcount;
                    r0["TDRIVEMILES"] = ttdrivemiles;
                    r0["TDRIVE_TOTAL_MILES"] = ttotalmiles;
                    r0["TMPERCENT"] = ((int)(ttdrivemiles / ttotalmiles * 100 + 0.5)) / 100.00;
                    r0["TDRIVE_TIME"] = ttdrivetime;
                    r0["TDRIVE_TOTAL_TIME"] = ttotaltime;
                    r0["TTPERCENT"] = ((int)(Convert.ToDouble(ttdrivetime)
                                    / Convert.ToDouble(ttotaltime) * 100 + 0.5)) / 100.00;
                    r0["ORANGE_COUNT"] = tocount;
                    r0["ORANGE_MILES"] = todrivemiles;
                    r0["ORANGE_TIME"] = todrivetime;
                    r0["OMPERCENT"] = ((int)(todrivemiles / ttotalmiles * 100 + 0.5)) / 100.00;
                    r0["OTPERCENT"] = ((int)(Convert.ToDouble(todrivetime)
                                    / Convert.ToDouble(ttotaltime) * 100 + 0.5)) / 100.00;
                    rs.Rows.InsertAt(r0, 0);
                    /****************************生成未行转列的表格数据完成*******************************/
                    DataTable[] resDt0 = new DataTable[2];
                    DataTable titleDt = new DataTable();        //field与之对应的名称
                    titleDt.Columns.Add("field");
                    titleDt.Columns.Add("title");
                    DataTable resDt = new DataTable();          //field对应数据
                    resDt.Columns.Add("TITLE");
                    DataRow dr0 = titleDt.NewRow();
                    dr0["field"] = "TITLE";
                    dr0["title"] = "时间/日期";
                    titleDt.Rows.Add(dr0);

                    foreach (DataRow rr in rs.Rows)
                    {
                        string colName = "";
                        if (rr["TDATE"].ToString().Equals("合计"))
                            colName = "合计";
                        else
                            colName = rr["MONTH"].ToString();
                        resDt.Columns.Add(colName);
                        DataRow dr14 = titleDt.NewRow();
                        dr14["field"] = colName;
                        dr14["title"] = rr["TDATE"].ToString();
                        titleDt.Rows.Add(dr14);
                    }
                    DataRow newDr2 = resDt.NewRow();
                    DataRow newDr3 = resDt.NewRow();
                    DataRow newDr4 = resDt.NewRow();
                    DataRow newDr5 = resDt.NewRow();
                    DataRow newDr6 = resDt.NewRow();
                    DataRow newDr7 = resDt.NewRow();
                    DataRow newDr8 = resDt.NewRow();
                    DataRow newDr9 = resDt.NewRow();
                    DataRow newDr10 = resDt.NewRow();
                    DataRow newDr11 = resDt.NewRow();
                    DataRow newDr12 = resDt.NewRow();
                    DataRow newDr13 = resDt.NewRow();
                    foreach (DataRow rr in rs.Rows)
                    {
                        string colName = "";
                        if (rr["TDATE"].ToString().Equals("合计"))
                            colName = "合计";
                        else
                            colName = rr["MONTH"].ToString();
                        newDr2["TITLE"] = "试乘试驾次数";
                        newDr2[colName] = rr["TCOUNT"].ToString();
                        newDr3["TITLE"] = "试乘试驾行驶里程(km)";
                        newDr3[colName] = rr["TDRIVEMILES"].ToString();
                        newDr4["TITLE"] = "总行驶里程(km)";
                        newDr4[colName] = rr["TDRIVE_TOTAL_MILES"].ToString();
                        newDr5["TITLE"] = "试乘试驾里程占比";
                        newDr5[colName] = rr["TMPERCENT"].ToString();
                        newDr6["TITLE"] = "试乘试驾时间(s)";
                        newDr6[colName] = rr["TDRIVE_TIME"].ToString();
                        newDr7["TITLE"] = "总行驶时间(s)";
                        newDr7[colName] = rr["TDRIVE_TOTAL_TIME"].ToString();
                        newDr8["TITLE"] = "试乘试驾时间占比";
                        newDr8[colName] = rr["TTPERCENT"].ToString();
                        newDr9["TITLE"] = "驶出活动范围次数";
                        newDr9[colName] = rr["ORANGE_COUNT"].ToString();
                        newDr10["TITLE"] = "驶出活动范围里程(km)";
                        newDr10[colName] = rr["ORANGE_MILES"].ToString();
                        newDr11["TITLE"] = "驶出活动范围时间(s)";
                        newDr11[colName] = rr["ORANGE_TIME"].ToString();
                        newDr12["TITLE"] = "驶出活动范围里程占比";
                        newDr12[colName] = rr["OMPERCENT"].ToString();
                        newDr13["TITLE"] = "驶出活动范围时间占比";
                        newDr13[colName] = rr["OTPERCENT"].ToString();
                    }
                    resDt.Rows.Add(newDr2);
                    resDt.Rows.Add(newDr3);
                    resDt.Rows.Add(newDr4);
                    resDt.Rows.Add(newDr5);
                    resDt.Rows.Add(newDr6);
                    resDt.Rows.Add(newDr7);
                    resDt.Rows.Add(newDr8);
                    resDt.Rows.Add(newDr9);
                    resDt.Rows.Add(newDr10);
                    resDt.Rows.Add(newDr11);
                    resDt.Rows.Add(newDr12);
                    resDt.Rows.Add(newDr13);
                    resDt0[0] = titleDt;
                    resDt0[1] = resDt;
                    ResList res = new ResList();
                    res.page = 0;
                    res.size = 0;
                    res.total = resDt0.Length;
                    res.records = resDt0;
                    res.isallresults = 0;
                    Result = new ResponseResult(ResState.Success, "", res);
                }

            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }

        public DataTable getCIDs(string key, string SSalesRegion, string SSalesProvince, string SDealersName, string SDealersCode, string SCarType, string SCarLicence)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[6];
                oaPara[0] = new SqlParameter("@SSalesRegion", SSalesRegion);
                oaPara[1] = new SqlParameter("@SSalesProvince", SSalesProvince);
                oaPara[2] = new SqlParameter("@SDealersName", SDealersName);
                oaPara[3] = new SqlParameter("@SDealersCode", SDealersCode);
                oaPara[4] = new SqlParameter("@SCarType", SCarType);
                oaPara[5] = new SqlParameter("@SCarLicence", SCarLicence);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("X80Proc_DayAnalyse_CIDs_Get"), oaPara, "fencetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable getList(string key, string CIDs, string STime)
        {
            ComSqlHelper oSqlUtil = new ComSqlHelper();
            try
            {
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@CIDs", CIDs);
                oaPara[1] = new SqlParameter("@STime", STime);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, WebProc.Proc("X80Proc_MonthAnalyse_List_Select"), oaPara, "fencetable", 30).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ResponseResult doExport(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;

            string STime;
            string SSalesRegion;
            string SSalesProvince;
            string SDealersName;
            string SDealersCode;
            string SCarType;
            string SCarLicence;

            if (!inparams.Keys.Contains("STime") || !inparams.Keys.Contains("sysflag"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["STime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "查询年月不能为空", null);
                    return Result;
                }
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }

                sysflag = inparams["sysflag"];
                STime = inparams["STime"];
                SSalesRegion = inparams["SSalesRegion"];
                SSalesProvince = inparams["SSalesProvince"];
                SDealersName = inparams["SDealersName"];
                SDealersCode = inparams["SDealersCode"];
                SCarType = inparams["SCarType"];
                SCarLicence = inparams["SCarLicence"];

                //调用存储过程查询数据列表
                //todo
                DataTable contentData = GetDayData(sysflag, STime, SSalesRegion, SSalesProvince, SDealersName, SDealersCode, SCarType, SCarLicence);

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "销售大区", "销售省区", "经销商代码", "经销商名称", "车型", "车牌号" ,
                                           
                                           "试乘试驾月份", "试乘试驾次数", "试乘试驾里程", "总行驶里程", "试乘试驾里程占比", "试乘试驾时间", "总行驶时间" ,

                                           "试乘试驾时间占比", "驶出活动范围次数", "驶出活动范围里程", "驶出活动范围时间", "驶出活动范围里程占比", "驶出活动范围时间占比","ID" 
                                           };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "试乘试驾日分析表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        public static DataTable GetDayData(string sysflag, string stime, string area, string province,
            string dealername, string dealercode, string cartype, string carno)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                          new SqlParameter("@stime", stime),  
                                          new SqlParameter("@area", area), 
                                          new SqlParameter("@province", province), 
                                          new SqlParameter("@dealername", dealername), 
                                          new SqlParameter("@dealercode", dealercode), 
                                          new SqlParameter("@cartype", cartype), 
                                          new SqlParameter("@carno", carno)
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("X80Proc_TestDriveMonthDataExport"), Parameters, null, 1800).Tables[0];

        }

    }
}