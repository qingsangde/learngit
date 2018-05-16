using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebGIS;
using CommLibrary;
using System.Data.SqlClient;
using System.Web;
using System.Reflection;
using System.Collections;

namespace SysService
{
    public class sta_onlinerate
    {
        public ResponseResult getCarOnlineRate_Month(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string sysuid;
            string onecaruser;
            string token;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("token") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("onecaruser"))
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
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                token = inparams["token"];


                DataTable dt = GetOnlineDate(sysflag, sysuid, stime, etime, onecaruser);
                DataTable[] resDt = GetOnlineRateMonth(token, stime, etime, dt);
                int Total = resDt.Length;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = resDt;
                res.isallresults = 0;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

        public ResponseResult getCarOnlineRate_Month_Export(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string stime;
            string etime;
            string sysflag;
            string sysuid;
            string onecaruser;
            string token;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("token") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("sysuid") || !inparams.Keys.Contains("onecaruser"))
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
                sysuid = inparams["sysuid"];
                sysflag = inparams["sysflag"];
                onecaruser = inparams["onecaruser"];
                token = inparams["token"];


                DataTable dt = GetOnlineDate(sysflag, sysuid, stime, etime, onecaruser);
                DataTable[] resDtArr = GetOnlineRateMonth(token, stime, etime, dt);

                DataTable titleDt = resDtArr[0];

                DataTable contentData = resDtArr[1];

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = new string[titleDt.Rows.Count];
                for (int i = 0; i < titleDt.Rows.Count; i++)
                {
                    headerDataArray[i] = titleDt.Rows[i]["title"].ToString();
                }

                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "车辆上线率月统计表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
                return Result;
            }

        }

        private DataTable[] GetOnlineRateMonth(string token, string stime, string etime, DataTable dt)
        {
            DataTable[] resDt0 = new DataTable[2];
            DataTable titleDt = new DataTable();
            titleDt.Columns.Add("field");
            titleDt.Columns.Add("title");
            DataTable resDt = new DataTable();
            resDt.Columns.Add("CarNo");

            DataRow dr = titleDt.NewRow();
            dr["field"] = "CarNo";
            dr["title"] = "车牌号";
            titleDt.Rows.Add(dr);


            List<string> dateList = GetAllDate(stime, etime);

            foreach (string date in dateList)
            {
                string colName = date.Replace("-", "");
                resDt.Columns.Add(colName);
                DataRow drtitle = titleDt.NewRow();
                drtitle["field"] = colName;
                drtitle["title"] = date.Substring(8, 2) + "日";
                titleDt.Rows.Add(drtitle);
            }

            resDt.Columns.Add("NotOnlineDays");
            DataRow drtitle0 = titleDt.NewRow();
            drtitle0["field"] = "NotOnlineDays";
            drtitle0["title"] = "单车未上线天数";
            titleDt.Rows.Add(drtitle0);

            resDt.Columns.Add("OnlineDays");
            DataRow drtitle1 = titleDt.NewRow();
            drtitle1["field"] = "OnlineDays";
            drtitle1["title"] = "单车上线天数";
            titleDt.Rows.Add(drtitle1);

            resDt.Columns.Add("OnlineRate");
            DataRow drtitle2 = titleDt.NewRow();
            drtitle2["field"] = "OnlineRate";
            drtitle2["title"] = "单车上线率";
            titleDt.Rows.Add(drtitle2);

            resDt.Columns.Add("Remark");
            DataRow drtitle3 = titleDt.NewRow();
            drtitle3["field"] = "Remark";
            drtitle3["title"] = "备注";
            titleDt.Rows.Add(drtitle3);

            SessionModel sm = new SessionModel();
            sm = SessionManager.GetSession(token);
            DataTable cardt = sm.cars.Copy();
            int carcount = cardt.Rows.Count;
            if (cardt.Rows.Count > 0)
            {
                for (int i = 0; i < carcount; i++)
                {
                    DataRow newDr = resDt.NewRow();
                    newDr["CarNo"] = cardt.Rows[i]["carno"].ToString();
                    string cid = cardt.Rows[i]["cid"].ToString();
                    int notonlineCount = 0;
                    int onlineCount = 0;

                    foreach (string date in dateList)
                    {
                        DataRow[] drArr = dt.Select("CID=" + cid + " and STAT_DAY='" + date + "'");
                        if (drArr.Length == 0)
                        {
                            newDr[date.Replace("-", "")] = "未上线";
                            notonlineCount++;
                        }
                        else
                        {
                            newDr[date.Replace("-", "")] = "上线";
                            onlineCount++;
                        }
                    }

                    newDr["NotOnlineDays"] = notonlineCount.ToString();
                    newDr["OnlineDays"] = onlineCount.ToString();
                    newDr["OnlineRate"] = Math.Round(Convert.ToDouble(onlineCount) / Convert.ToDouble(notonlineCount + onlineCount), 2);
                    newDr["Remark"] = "";

                    resDt.Rows.Add(newDr);
                }


                DataRow dr1 = resDt.NewRow();
                dr1["CarNo"] = "当日上线车辆台数";
                DataRow dr2 = resDt.NewRow();
                dr2["CarNo"] = "当日未上线车辆台数";
                DataRow dr3 = resDt.NewRow();
                dr3["CarNo"] = "当日车辆上线率";
                int onlineCars = 0;
                foreach (string date in dateList)
                {
                    onlineCars = 0;
                    for (int i = 0; i < carcount; i++)
                    {
                        if (resDt.Rows[i][date.Replace("-", "")].ToString() == "上线")
                        {
                            onlineCars++;
                        }
                    }
                    dr1[date.Replace("-", "")] = onlineCars.ToString();
                    dr2[date.Replace("-", "")] = (carcount - onlineCars).ToString();
                    dr3[date.Replace("-", "")] = Math.Round(Convert.ToDouble(onlineCars) / Convert.ToDouble(carcount), 2);
                }
                dr1["NotOnlineDays"] = "";
                dr1["OnlineDays"] = "";
                dr1["OnlineRate"] = "";
                dr1["Remark"] = "";
                dr2["NotOnlineDays"] = "";
                dr2["OnlineDays"] = "";
                dr2["OnlineRate"] = "";
                dr2["Remark"] = "";
                dr3["NotOnlineDays"] = "";
                dr3["OnlineDays"] = "";
                dr3["OnlineRate"] = "";
                dr3["Remark"] = "";

                resDt.Rows.Add(dr1);
                resDt.Rows.Add(dr2);
                resDt.Rows.Add(dr3);
            }

            resDt0[0] = titleDt;
            resDt0[1] = resDt;
            return resDt0;
        }

        /// <summary>
        /// 获取两个日期间的所有日期yyyy-MM-dd
        /// </summary>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        private List<string> GetAllDate(string stime, string etime)
        {
            List<string> dateList = new List<string>();
            DateTime st = DateTime.Parse(stime);
            DateTime et = DateTime.Parse(etime);
            dateList.Add(st.ToString("yyyy-MM-dd"));
            DateTime thedate = st.AddDays(1);
            while (thedate < et)
            {
                dateList.Add(thedate.ToString("yyyy-MM-dd"));
                thedate = thedate.AddDays(1);
            }
            dateList.Add(et.ToString("yyyy-MM-dd"));

            return dateList;
        }


        private DataTable GetOnlineDate(string sysflag, string sysuid, string stime, string etime, string onecaruser)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@UID", sysuid), 
                                            new SqlParameter("@T_TimeBegin", stime), 
                                            new SqlParameter("@T_TimeEnd", etime), 
                                            new SqlParameter("@OneCarUser", onecaruser)
                                            
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_CarOnLineDate"), Parameters, null, 3600).Tables[0];
        }
    }
}
