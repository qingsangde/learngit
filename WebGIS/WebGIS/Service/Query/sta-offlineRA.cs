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
    public class sta_offlineRA
    {

        public ResponseResult getOfflineStatistic(Dictionary<string, string> inparams)
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
            string num;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("CarNum") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("CUID") || !inparams.Keys.Contains("CarOwnName") || !inparams.Keys.Contains("Line") || !inparams.Keys.Contains("num"))
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
                num = inparams["num"];
                DataTable dtBase = GetBaseTable(sysflag, sysuid, onecaruser, CarNum, CarOwnName, CUID, Line);
                DataTable dtStat = GetStatTable(sysflag, sysuid, onecaruser, CarNum, CarOwnName, CUID, Line, stime, etime);

                DataTable dtOrg = OrgnizeTableNew(dtStat);

                List<OfflineStat> offlineStatList = OrgnizeOfflineStat(dtBase, dtOrg, Int32.Parse(num));

                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = offlineStatList.Count;
                res.records = offlineStatList;
                res.isallresults = 1;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }

            return Result;
        }


        public ResponseResult getOfflineStatisticOutPut(Dictionary<string, string> inparams)
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
            string num;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("CarNum") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("CUID") || !inparams.Keys.Contains("CarOwnName") || !inparams.Keys.Contains("Line") || !inparams.Keys.Contains("num"))
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
                num = inparams["num"];
                DataTable dtBase = GetBaseTable(sysflag, sysuid, onecaruser, CarNum, CarOwnName, CUID, Line);
                DataTable dtStat = GetStatTable(sysflag, sysuid, onecaruser, CarNum, CarOwnName, CUID, Line, stime, etime);

                DataTable dtOrg = OrgnizeTableNew(dtStat);

                List<OfflineStat> offlineStatList = OrgnizeOfflineStat(dtBase, dtOrg, Int32.Parse(num));

                DataTable contentData = ListToDataTable(offlineStatList);

                contentData.Columns.Remove("CID");
                // 添加列
                contentData.Columns.Add("DurationStr");
                // 处理时间，转换为“时、分、秒”
                foreach (DataRow row in contentData.Rows)
                {
                    if ((int)row["OfflineDurationSum"] != 0)
                    {
                        row["DurationStr"] = AnalysisDuration((int)row["OfflineDurationSum"]);
                    }
                    else
                    {
                        row["DurationStr"] = "0";
                    }
                }

                contentData.Columns.Remove("OfflineDurationSum");



                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "所属企业", "车辆用途", "运营线路", "离线次数", "离线总时长" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "车辆离线统计" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        public ResponseResult getOfflineStatisticDetail(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string num;
            string cid;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("num"))
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
                num = inparams["num"];
                cid = inparams["cid"];

                DataTable dtStatDetail = GetStatDetailTable(sysflag, cid, stime, etime);

                DataTable dtDetailOrg = OrgnizeTableDetailNew(dtStatDetail);

                List<OfflineStatDetail> offlineStatDetailList = OrgnizeOfflineStatDetail(dtDetailOrg, Int32.Parse(num));

                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = offlineStatDetailList.Count;
                res.records = offlineStatDetailList;
                res.isallresults = 1;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }


            return Result;
        }

        public ResponseResult getOfflineStatisticDetailOutput(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string stime;
            string etime;
            string sysflag;
            string num;
            string cid;
            if (!inparams.Keys.Contains("stime") || !inparams.Keys.Contains("etime") || !inparams.Keys.Contains("cid") || !inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("num"))
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
                num = inparams["num"];
                cid = inparams["cid"];

                DataTable dtStatDetail = GetStatDetailTable(sysflag, cid, stime, etime);

                DataTable dtDetailOrg = OrgnizeTableDetailNew(dtStatDetail);

                List<OfflineStatDetail> offlineStatDetailList = OrgnizeOfflineStatDetail(dtDetailOrg, Int32.Parse(num));

                DataTable contentData = ListToDetailDataTable(offlineStatDetailList);

                contentData.Columns.Remove("CID");
                // 添加列
                contentData.Columns.Add("DurationStr");
                // 处理时间，转换为“时、分、秒”
                foreach (DataRow row in contentData.Rows)
                {
                    if ((int)row["OffDuration"] != 0)
                    {
                        row["DurationStr"] = AnalysisDuration((int)row["OffDuration"]);
                    }
                    else
                    {
                        row["DurationStr"] = "0";
                    }
                }

                contentData.Columns.Remove("OffDuration");


                contentData.Columns.Add("StartAddress");
                contentData.Columns.Add("EndAddress");

                List<CommLibrary.AddressConvert.DLngLat> corrds = new List<AddressConvert.DLngLat>();
                for (int i = 0; i < contentData.Rows.Count; i++)
                {
                    string sc = contentData.Rows[i]["FromAddress"].ToString();
                    sc = string.IsNullOrEmpty(sc) ? "0,0" : sc;
                    string slng = sc.Split(',')[0];
                    string slat = sc.Split(',')[1];
                    string ec = contentData.Rows[i]["ToAddress"].ToString();
                    ec = string.IsNullOrEmpty(ec) ? "0,0" : ec;
                    string elng = ec.Split(',')[0];
                    string elat = ec.Split(',')[1];
                    CommLibrary.AddressConvert.DLngLat sdl = new AddressConvert.DLngLat();
                    sdl.Lat = double.Parse(slat);
                    sdl.Lng = double.Parse(slng);
                    corrds.Add(sdl);
                    CommLibrary.AddressConvert.DLngLat edl = new AddressConvert.DLngLat();
                    edl.Lat = double.Parse(elat);
                    edl.Lng = double.Parse(elng);
                    corrds.Add(edl);
                }
                string[] address = AddressConvert.AddConvertBatch(corrds);
                for (int i = 0; i < contentData.Rows.Count; i++)
                {
                    contentData.Rows[i]["StartAddress"] = address[i * 2];
                    contentData.Rows[i]["EndAddress"] = address[i * 2 + 1];
                }

                contentData.Columns.Remove("FromAddress");
                contentData.Columns.Remove("ToAddress");

                NPOIHelper npoiHelper = new NPOIHelper();
                string[] headerDataArray = { "车牌号", "所属企业", "车辆用途", "运营线路", "开始时间", "结束时间", "离线时长", "开始地址", "结束地址" };
                string[][] contentDataArray = npoiHelper.convertDataTableToStringArray(contentData);
                npoiHelper.WorkbookName = "车辆离线统计明细" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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

        private List<OfflineStatDetail> OrgnizeOfflineStatDetail(DataTable dtStatDetail, int offInterval)
        {
            List<OfflineStatDetail> offlineStatDetailList = new List<OfflineStatDetail>();

            List<DataRow> delRowList = new List<DataRow>();
            foreach (DataRow row in dtStatDetail.Rows)
            {
                if ((int)row["OFF_DURATION"] > offInterval * 60)
                {
                    OfflineStatDetail offlineDetail = new OfflineStatDetail();
                    offlineDetail.CID = Convert.ToInt64(row["CID"].ToString());
                    offlineDetail.CarNo = row["CarNo"].ToString();
                    offlineDetail.CarOwnName = row["CarOwnName"].ToString();
                    offlineDetail.CUID = row["CUID"].ToString();
                    offlineDetail.Line = row["Line"].ToString();
                    offlineDetail.FromDate = (DateTime)row["FROM_DATE"];
                    offlineDetail.FromAddress = row["FROM_LONGITUDE"].ToString() + "," + row["FROM_LATITUDE"].ToString();
                    offlineDetail.ToDate = (DateTime)row["TO_DATE"];
                    offlineDetail.ToAddress = row["TO_LONGITUDE"].ToString() + "," + row["TO_LATITUDE"].ToString(); ;
                    offlineDetail.OffDuration = (int)row["OFF_DURATION"];

                    offlineStatDetailList.Add(offlineDetail);
                }
                else
                {
                    delRowList.Add(row);
                }
            }

            // 循环删除行
            foreach (DataRow row in delRowList)
            {
                dtStatDetail.Rows.Remove(row);
            }

            return offlineStatDetailList;
        }


        /// <summary>
        /// 组织查询结果中的时间-总表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable OrgnizeTableNew(DataTable dt)
        {
            DataTable resTable = dt.Clone();
            resTable.Rows.Clear();
            int rowCount = dt.Rows.Count;
            if (rowCount == 1)
            {
                resTable.ImportRow(dt.Rows[0]);
            }
            else
            {
                if (rowCount > 1)
                {
                    int i = 1;
                    while (i < rowCount)
                    {
                        DataRow dr0 = dt.Rows[i-1];  //前一行
                        DataRow dr1 = dt.Rows[i];    //本行
                        if (dr1.Field<long>("CID") == dr0.Field<long>("CID") && dr1.Field<DateTime>("FROM_DATE") == dr0.Field<DateTime>("TO_DATE") &&
                                dr1.Field<DateTime>("FROM_DATE").Hour == 0 &&
                                dr1.Field<DateTime>("FROM_DATE").Minute == 0 &&
                                dr1.Field<DateTime>("FROM_DATE").Second == 0)
                        {
                            DataRow drNew = resTable.NewRow();
                            drNew["CID"] = dr0["CID"];
                            drNew["FROM_DATE"] = dr0["FROM_DATE"];
                            drNew["FROM_LONGITUDE"] = dr0["FROM_LONGITUDE"];
                            drNew["FROM_LATITUDE"] = dr0["FROM_LATITUDE"];
                            drNew["TO_DATE"] = dr1["TO_DATE"];
                            drNew["TO_LONGITUDE"] = dr1["TO_LONGITUDE"];
                            drNew["TO_LATITUDE"] = dr1["TO_LATITUDE"];
                            drNew["OFF_DURATION"] = (int)dr1["OFF_DURATION"] + (int)dr0["OFF_DURATION"];
                            resTable.ImportRow(drNew);
                            i = i + 2;
                        }
                        else
                        {
                            resTable.ImportRow(dr0);
                            if (i == rowCount - 1)
                            {
                                resTable.ImportRow(dr1);
                            }
                            i = i + 1;
                        }
                    }                  
                }
            }

            return resTable;
        }

        /// <summary>
        /// 组织查询结果中的时间-明细
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable OrgnizeTableDetailNew(DataTable dt)
        {
            DataTable resTable = dt.Clone();
            resTable.Rows.Clear();
            int rowCount = dt.Rows.Count;
            if (rowCount == 1)
            {
                resTable.ImportRow(dt.Rows[0]);
            }
            else
            {
                if (rowCount > 1)
                {
                    int i = 1;
                    while (i < rowCount)
                    {
                        DataRow dr0 = dt.Rows[i - 1];  //前一行
                        DataRow dr1 = dt.Rows[i];    //本行
                        if (dr1.Field<long>("CID") == dr0.Field<long>("CID") && dr1.Field<DateTime>("FROM_DATE") == dr0.Field<DateTime>("TO_DATE") &&
                                dr1.Field<DateTime>("FROM_DATE").Hour == 0 &&
                                dr1.Field<DateTime>("FROM_DATE").Minute == 0 &&
                                dr1.Field<DateTime>("FROM_DATE").Second == 0)
                        {
                            DataRow drNew = resTable.NewRow();
                            drNew["CID"] = dr0["CID"];
                            drNew["CarNo"] = dr0["CarNo"];
                            drNew["CarOwnName"] = dr0["CarOwnName"];
                            drNew["CUID"] = dr0["CUID"];
                            drNew["Line"] = dr0["Line"];
                            drNew["FROM_DATE"] = dr0["FROM_DATE"];
                            drNew["FROM_LONGITUDE"] = dr0["FROM_LONGITUDE"];
                            drNew["FROM_LATITUDE"] = dr0["FROM_LATITUDE"];
                            drNew["TO_DATE"] = dr1["TO_DATE"];
                            drNew["TO_LONGITUDE"] = dr1["TO_LONGITUDE"];
                            drNew["TO_LATITUDE"] = dr1["TO_LATITUDE"];
                            drNew["OFF_DURATION"] = (int)dr1["OFF_DURATION"] + (int)dr0["OFF_DURATION"];
                            resTable.ImportRow(drNew);
                            i = i + 2;
                        }
                        else
                        {
                            resTable.ImportRow(dr0);
                            if (i == rowCount - 1)
                            {
                                resTable.ImportRow(dr1);
                            }
                            i = i + 1;
                        }
                    }
                }
            }

            return resTable;
        }



        /// <summary>
        /// 组织查询结果中的时间-废弃的
        /// </summary>
        /// <param name="dt"></param>
        void OrgnizeTable(ref DataTable dt)
        {
            try
            {
                // 选择结果集中的CID
                var cidQuery = (from t in dt.AsEnumerable()
                                where t.Field<int>("OFF_DURATION") != 0
                                select t.Field<long>("CID")).Distinct<long>();
                // 删除列集合
                List<DataRow> delRowList = new List<DataRow>();
                foreach (var cid in cidQuery)
                {
                    // 根据CID查询记录，倒序排列
                    var tblQuery = from t in dt.AsEnumerable()
                                   where t.Field<long>("CID").Equals(cid)
                                   orderby t.Field<DateTime>("FROM_DATE") descending
                                   select t;

                    for (int i = 0; i < tblQuery.Count(); i++)
                    {
                        // 取出第一行
                        DataRow outRow = tblQuery.ElementAt(i);
                        for (int j = i + 1; j < tblQuery.Count(); j++)
                        {
                            DataRow inRow = tblQuery.ElementAt(j);
                            // 判断结束时间是否和前一条的开始时间相等
                            // 并且时间为“****-**-** 00:00:00”，过滤掉一条定位信息的情况
                            if (outRow.Field<DateTime>("FROM_DATE") == inRow.Field<DateTime>("TO_DATE") &&
                                outRow.Field<DateTime>("FROM_DATE").Hour == 0 &&
                                outRow.Field<DateTime>("FROM_DATE").Minute == 0 &&
                                outRow.Field<DateTime>("FROM_DATE").Second == 0)
                            {
                                // 满足条件：赋值；把此列放入要删除列的集合
                                outRow["FROM_DATE"] = inRow["FROM_DATE"];
                                outRow["FROM_LONGITUDE"] = inRow["FROM_LONGITUDE"];
                                outRow["FROM_LATITUDE"] = inRow["FROM_LATITUDE"];
                                outRow["OFF_DURATION"] = (int)outRow["OFF_DURATION"] + (int)inRow["OFF_DURATION"];
                                delRowList.Add(inRow);
                            }
                            // 不满足条件，跳出循环
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                // 删除记录
                foreach (DataRow row in delRowList)
                {
                    dt.Rows.Remove(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        List<OfflineStat> OrgnizeOfflineStat(DataTable dtBase, DataTable dtStat, int offInterval)
        {

            var queryBase = from p in dtBase.AsEnumerable()
                            select new
                            {
                                CID = p.Field<long>("CID"),
                                CarNo = p.Field<string>("CarNo"),
                                CarOwnName = p.Field<string>("CarOwnName"),
                                CUID = p.Field<string>("CUID"),
                                Line = p.Field<string>("Line")
                            };

            var queryStat = from t in dtStat.AsEnumerable()
                            where t.Field<int>("OFF_DURATION") > offInterval * 60
                            group t by t.Field<long>("CID") into g
                            select new
                            {
                                CID = g.First().Field<long>("CID"),
                                OfflineFrequency = g.First().Field<int>("OFF_DURATION") == 0 ? 0 : g.Count(),
                                OfflineDurationSum = g.Sum(p => p.Field<int>("OFF_DURATION"))
                            };

            var query = from t in queryBase
                        join p in queryStat on t.CID equals p.CID
                        into g
                        from q in g.DefaultIfEmpty()
                        select new OfflineStat
                        {
                            CID = t.CID,
                            CarNo = t.CarNo,
                            CUID = t.CUID,
                            CarOwnName = t.CarOwnName,
                            Line = t.Line,
                            OfflineFrequency = q == null ? 0 : q.OfflineFrequency,
                            OfflineDurationSum = q == null ? 0 : q.OfflineDurationSum
                        };

            return query.ToList();

        }

        private DataTable GetBaseTable(string sysflag, string uid, string onecaruser, string carno, string carownname, string cuid, string line)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@UID", uid), 
                                            new SqlParameter("@CarNo", carno), 
                                            new SqlParameter("@CUID", cuid), 
                                            new SqlParameter("@CarOwnName", carownname), 
                                            new SqlParameter("@Line", line), 
                                            new SqlParameter("@OneCarUser", onecaruser)
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QSProc_GetUserCarInfoNew"), Parameters, null, 3600).Tables[0];
        }



        private DataTable GetStatTable(string sysflag, string uid, string onecaruser, string carno, string carownname, string cuid, string line, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@UID", uid), 
                                             new SqlParameter("@OneCarUser", onecaruser),
                                            new SqlParameter("@CarNum", carno), 
                                            new SqlParameter("@CUID", cuid), 
                                            new SqlParameter("@CarOwnName", carownname), 
                                            new SqlParameter("@Line", line), 
                                            new SqlParameter("@T_TimeBegin", stime), 
                                            new SqlParameter("@T_TimeEnd", etime)
                                           
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_OfflineStatRATest"), Parameters, null, 3600).Tables[0];
        }

        private DataTable GetStatDetailTable(string sysflag, string cid, string stime, string etime)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = {   
                                            new SqlParameter("@CID", cid), 
                                            new SqlParameter("@T_TimeBegin", stime), 
                                            new SqlParameter("@T_TimeEnd", etime)                                        
                                        };
            return csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_OfflineStatRADetail"), Parameters, null, 3600).Tables[0];
        }

        private string AnalysisDuration(int durationSum)
        {
            if (durationSum != 0)
            {
                int hour = 0;
                int minute = 0;
                int second = 0;
                hour = durationSum / 3600;
                minute = durationSum % 3600 / 60;
                second = durationSum % 60;
                return hour + "小时" + minute + "分" + second + "秒";
            }
            else return "0";
        }


        /// <summary>    
        /// 将集合类转换成DataTable    
        /// </summary>    
        /// <param name="list">集合</param>    
        /// <returns></returns>    
        private DataTable ListToDataTable(List<OfflineStat> list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        private DataTable ListToDetailDataTable(List<OfflineStatDetail> list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }



    public class OfflineStat
    {
        public long CID { get; set; }
        public string CarNo { get; set; }      
        public string CarOwnName { get; set; }
        public string CUID { get; set; }
        public string Line { get; set; }
        // 离线次数
        public int OfflineFrequency { get; set; }
        // 离线总时长
        public int OfflineDurationSum { get; set; }
    }

    public class OfflineStatDetail
    {
        public long CID { get; set; }
        public string CarNo { get; set; } 
        public string CarOwnName { get; set; }
        public string CUID { get; set; }
        public string Line { get; set; }
        public DateTime FromDate { get; set; }
        public string FromAddress { get; set; }
        public DateTime ToDate { get; set; }
        public string ToAddress { get; set; }
        public int OffDuration { get; set; }
    }
}