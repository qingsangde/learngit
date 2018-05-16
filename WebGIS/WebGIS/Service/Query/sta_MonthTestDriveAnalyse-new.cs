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
    public class sta_MonthTestDriveAnalyse_new
    {
        public ResponseResult getSFCar(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;
            string stime;
            string area;
            string province;
            string dealername;
            string dealercode;
            string cartype;
            string carno;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("sysflag"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "查询年月不能为空", null);
                    return Result;
                }


                sysflag = inparams["sysflag"];
                stime = inparams["stime"];
                


                area = inparams["area"];
                province = inparams["province"];
                dealername = inparams["dealername"];
                dealercode = inparams["dealercode"];
                cartype = inparams["cartype"];
                carno = inparams["carno"];



                //调用存储过程查询车辆启动熄火数据
                //todo
                DataTable dt = GetStatusCars(sysflag, stime, area, province, dealername, dealercode, cartype, carno);

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

        public static DataTable GetStatusCars(string sysflag, string stime, string area, string province,
            string dealername, string dealercode, string cartype, string carno
            )
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
            DataTable dt = csh.FillDataSet(sysflag, WebProc.Proc("X80Proc_MonthAnalyse_List_Select_New"), Parameters, null, 1800).Tables[0];


            DataTable dt1 = new DataTable();
            dt1.Columns.Add("text", typeof(string));
            dt1.Columns.Add("sum", typeof(string));
            for (int i = 1; i < 13; i++)
            {
                dt1.Columns.Add("D" + i, typeof(string));
            }

            #region 12列数据初始化
            DataRow Row1 = dt1.NewRow();
            Row1["text"] = "试乘试驾次数";
            dt1.Rows.Add(Row1);

            DataRow Row2 = dt1.NewRow();
            Row2["text"] = "试乘试驾行驶里程(km)";
            dt1.Rows.Add(Row2);

            DataRow Row3 = dt1.NewRow();
            Row3["text"] = "总行驶里程(km)";
            dt1.Rows.Add(Row3);

            DataRow Row4 = dt1.NewRow();
            Row4["text"] = "试乘试驾里程占比";
            dt1.Rows.Add(Row4);

            DataRow Row5 = dt1.NewRow();
            Row5["text"] = "试乘试驾时间(s)";
            dt1.Rows.Add(Row5);

            DataRow Row6 = dt1.NewRow();
            Row6["text"] = "总行驶时间(s)";
            dt1.Rows.Add(Row6);

            DataRow Row7 = dt1.NewRow();
            Row7["text"] = "试乘试驾时间占比";
            dt1.Rows.Add(Row7);

            DataRow Row8 = dt1.NewRow();
            Row8["text"] = "驶出活动范围次数";
            dt1.Rows.Add(Row8);


            DataRow Row9 = dt1.NewRow();
            Row9["text"] = "驶出活动范围里程(km)";
            dt1.Rows.Add(Row9);

            DataRow Row10 = dt1.NewRow();
            Row10["text"] = "驶出活动范围时间(s)";
            dt1.Rows.Add(Row10);

            DataRow Row11 = dt1.NewRow();
            Row11["text"] = "驶出活动范围里程占比";
            dt1.Rows.Add(Row11);

            DataRow Row12 = dt1.NewRow();
            Row12["text"] = "驶出活动范围时间占比";
            dt1.Rows.Add(Row12);

            #endregion

            #region 添加日数据

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int d = Int32.Parse(dt.Rows[i]["MMonth"].ToString().Trim());

                dt1.Rows[0]["D" + d] = dt.Rows[i]["DCOUNT"].ToString().Trim();
                dt1.Rows[1]["D" + d] = dt.Rows[i]["DMILES"].ToString().Trim();
                dt1.Rows[2]["D" + d] = dt.Rows[i]["DTMILES"].ToString().Trim();
                dt1.Rows[3]["D" + d] = dt.Rows[i]["MILESPCT"].ToString().Trim();

                dt1.Rows[4]["D" + d] = dt.Rows[i]["DTIME"].ToString().Trim();
                dt1.Rows[5]["D" + d] = dt.Rows[i]["DTTIME"].ToString().Trim();
                dt1.Rows[6]["D" + d] = dt.Rows[i]["TIMESPCT"].ToString().Trim();
                dt1.Rows[7]["D" + d] = dt.Rows[i]["RCOUNT"].ToString().Trim();


                dt1.Rows[8]["D" + d] = dt.Rows[i]["RMILES"].ToString().Trim();
                dt1.Rows[9]["D" + d] = dt.Rows[i]["RTIME"].ToString().Trim();
                dt1.Rows[10]["D" + d] = dt.Rows[i]["OFFMILESPCT"].ToString().Trim();
                dt1.Rows[11]["D" + d] = dt.Rows[i]["OFFTIMESPCT"].ToString().Trim();

            }
            #endregion

            #region 添加合计

            for (int i = 0; i < dt1.Rows.Count; i++)
            {

                Double sum = 0;

                for (int h = 1; h < 13; h++)
                {
                    if (dt1.Rows[i]["D" + h].ToString().Trim() != "")
                    {
                        sum = sum + Double.Parse(dt1.Rows[i]["D" + h].ToString().Trim());
                    }
                }

                dt1.Rows[i]["sum"] = sum;

            }

            if (dt.Rows.Count > 0)
            {
                dt1.Rows[3]["sum"] = Math.Round(Double.Parse(dt1.Rows[1]["sum"].ToString().Trim()) / Double.Parse(dt1.Rows[2]["sum"].ToString().Trim()), 2);

                dt1.Rows[6]["sum"] = Math.Round(Double.Parse(dt1.Rows[4]["sum"].ToString().Trim()) / Double.Parse(dt1.Rows[5]["sum"].ToString().Trim()), 2);

                dt1.Rows[10]["sum"] = Math.Round(Double.Parse(dt1.Rows[8]["sum"].ToString().Trim()) / Double.Parse(dt1.Rows[2]["sum"].ToString().Trim()), 2);

                dt1.Rows[11]["sum"] = Math.Round(Double.Parse(dt1.Rows[9]["sum"].ToString().Trim()) / Double.Parse(dt1.Rows[5]["sum"].ToString().Trim()), 2);
            }


            #endregion




            return dt1;
        }




        public ResponseResult getSFCaroutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string sysflag;

            string stime;
            string area;
            string province;
            string dealername;
            string dealercode;
            string cartype;
            string carno;

            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("sysflag"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["stime"] == "")
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

                stime = inparams["stime"];
               
                area = inparams["area"];
                province = inparams["province"];
                dealername = inparams["dealername"];
                dealercode = inparams["dealercode"];
                cartype = inparams["cartype"];
                carno = inparams["carno"];

                //调用存储过程查询启动熄火车辆列表
                //todo
                DataTable contentData = GetStatusCars(sysflag, stime, area, province, dealername, dealercode, cartype, carno);

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "时间/日期", "合计", "1月", "2月", "3月", "4月" , "5月", "6月", "7月", "8月" , "9月",
                                               "10月", "11月", "12月" 
                                           };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "试乘试驾月分析表" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
    }
}