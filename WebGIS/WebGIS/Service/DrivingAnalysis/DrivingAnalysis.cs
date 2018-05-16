using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGIS;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace SysService
{
    public class DrivingAnalysis
    {
        string SysFlag = "";
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult getData(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            string sysflag;
            string cid;
            string st;
            string et;
            if (!inparams.Keys.Contains("sysflag") || !inparams.Keys.Contains("cid") ||
                  !inparams.Keys.Contains("st") || !inparams.Keys.Contains("et") 
                 )
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数", null);
                return Result;
            }
            try
            {
                if (inparams["sysflag"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "系统标识错误", null);
                    return Result;
                }
                if (inparams["cid"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "请选择车辆", null);
                    return Result;
                }
                if (inparams["st"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "开始时间不能为空", null);
                    return Result;
                }
                if (inparams["et"] == "")
                {
                    Result = new ResponseResult(ResState.ParamsImperfect, "结束时间不能为空", null);
                    return Result;
                }
                sysflag = inparams["sysflag"];
                SysFlag = sysflag;
                cid = inparams["cid"];
               
                st = inparams["st"];
                et = inparams["et"];
             

                //调用存储过程查询车辆数据
                DataTable dt = GetTracksFromDB(sysflag, cid, st, et);

                DrivObj DObj = FormatData(dt);



                int Total = DObj.Details.Rows.Count;
                ResList res = new ResList();
                res.page = 0;
                res.size = 0;
                res.total = Total;
                res.records = DObj;




                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, "");
            }
            return Result;
        }

       

        #region 获取轨迹部分
        
        private DataTable GetTracksFromDB(string sysflag, string cid, string st, string et)
        {
            DataTable dt = null;
            try
            {
                ComSqlHelper csh = new ComSqlHelper();
                SqlParameter[] oaPara;
                //参数构建
                oaPara = new SqlParameter[3];
                oaPara[0] = new SqlParameter("@C_ID", cid);
                oaPara[1] = new SqlParameter("@StartTime", st);
                oaPara[2] = new SqlParameter("@EndTime", et);

                dt = csh.FillDataSet(sysflag, WebProc.Proc("QWGProc_QM_DrivingAnalysis"), oaPara).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DrivObj FormatData(DataTable dt)
        {
            DrivObj obj = new DrivObj();

            try
            {
                DataTable dr= analyzeTable(dt);
                obj.Details = dr;
                obj.Statistics = tongji(dr);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        private DataTable analyzeTable(DataTable dt)
        {
            dt.Columns.Add("DR", Type.GetType("System.String"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                dt.Rows[i]["DR"] = analyzeAlertor((byte[])(dt.Rows[i]["canData"]));
            }
            dt.Columns.Remove("canData");

            DataRow[] DRS = dt.Select("DR <> '正常'");

            //dt.Rows.Clear();

            DataTable dtNew = dt.Clone();
  


            foreach (var item in DRS)
            {
                DataRow dRow = dtNew.NewRow();

                dRow["CID"] = item["CID"];
                dRow["CanId"] = item["CanId"];
                dRow["T_DateTime"] = item["T_DateTime"];
                dRow["DR"] = item["DR"];
                dtNew.Rows.Add(dRow);

            }

            return dtNew;
        }


       






        private DataTable tongji(DataTable dt)
        {
            DataTable dtt = new DataTable();

            //急加速
            dtt.Columns.Add("jjs", typeof(int));
            //大油门
            dtt.Columns.Add("dym", typeof(int));
            //急刹车
            dtt.Columns.Add("jsc", typeof(int));
            //怠速时间过长
            dtt.Columns.Add("ds", typeof(int));
            //冷车高速行驶
            dtt.Columns.Add("lc", typeof(int));
            //水温异常
            dtt.Columns.Add("sw", typeof(int));
            //电压异常
            dtt.Columns.Add("dy", typeof(int));       

            DataRow dRow = dtt.NewRow();

            //DataRow[] DR_jjs = dt.Select("DR like '%急加速%'");
            //DataRow[] DR_dym = dt.Select("DR like '%大油门%'");
            //DataRow[] DR_jsc = dt.Select("DR like '%急刹车%'");
            //DataRow[] DR_ds = dt.Select("DR like '%怠速时间过长%'");
            //DataRow[] DR_lc = dt.Select("DR like '%冷车高速行驶%'");
            //DataRow[] DR_sw = dt.Select("DR like '%水温异常%'");
            //DataRow[] DR_dy = dt.Select("DR like '%电压异常%'");

            DataRow[] DR_jjs = dt.Select("DR like '%急加速%'");
            DataRow[] DR_dym = dt.Select("DR like '%大油门%'");
            DataRow[] DR_jsc = dt.Select("DR like '%急刹车%'");
            DataRow[] DR_ds = dt.Select("DR like '%怠速时间过长%'");
            DataRow[] DR_lc = dt.Select("DR like '%冷车高速行驶%'");
            DataRow[] DR_sw = dt.Select("DR like '%水温异常%'");
            DataRow[] DR_dy = dt.Select("DR like '%电压异常%'");

            dRow["jjs"] = DR_jjs.Length;
            dRow["dym"] = DR_dym.Length;
            dRow["jsc"] = DR_jsc.Length;
            dRow["ds"] = DR_ds.Length;
            dRow["lc"] = DR_lc.Length;
            dRow["sw"] = DR_sw.Length;
            dRow["dy"] = DR_dy.Length;
            dtt.Rows.Add(dRow);
            return dtt;
        }

      
        private string analyzeAlertor(byte[] value) {

            StringBuilder sb = new StringBuilder();
            LiuTeEbResolve lt = new LiuTeEbResolve();
            Dictionary<int, string> list = lt.analyzeAlertor(value);

            foreach (var item in list)
            {
                sb.Append(item.Value);
                sb.Append(";");
            }

            if (list.Count > 0)
            {
                return sb.ToString().Trim();
            }
            else
            {
                return "正常";
            }
        }
        #endregion
    }
}