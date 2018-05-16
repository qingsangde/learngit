using System;
using System.Collections.Generic;
using System.Web;

using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Xml;
using System.Data;
using CommLibrary;


namespace WebGIS
{
    /// <summary>
    /// CarInfo 的摘要说明
    /// </summary>
    public class RourtSet : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string key = System.Web.HttpUtility.UrlDecode(context.Request["key"].ToString().Trim());

            string Operation = System.Web.HttpUtility.UrlDecode(context.Request["Operation"].ToString().Trim());

            string CarNo;
            string LisenseColor;
            string str;
            switch (Operation)
            {
                case "QueryRourtData":
                    context.Response.Write(getJsonQueryRourt(key));
                    break;
                case "QueryLine":
                    {
                        string LineID = System.Web.HttpUtility.UrlDecode(context.Request["LineID"].ToString().Trim());
                        context.Response.Write(getJsonQueryLineInfo(key, int.Parse(LineID)));
                        break;
                    }
                case "SaveLine":
                    {
                        string LineID = System.Web.HttpUtility.UrlDecode(context.Request["LineID"].ToString().Trim());
                        string Longs = System.Web.HttpUtility.UrlDecode(context.Request["Longs"].ToString().Trim());
                        string Lats = System.Web.HttpUtility.UrlDecode(context.Request["Lats"].ToString().Trim());
                        string[] longarray = Longs.Split(',');
                        string[] latarray = Lats.Split(',');

                        double[] Longitude = Array.ConvertAll<string, double>(longarray, delegate(string s) { return double.Parse(s.ToString()); });
                        double[] Latitude = Array.ConvertAll<string, double>(latarray, delegate(string s) { return double.Parse(s.ToString()); });
                        context.Response.Write(insertLine2DB(key, int.Parse(LineID), Longitude,Latitude));
                        break;
                    }
            }
        }


        private bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }

        private string getJsonQueryRourt(string key)
        {
            string r = "";
            //string strCid = "";
            try
            {
                ComSqlHelper oSqlUtil = new ComSqlHelper();
                SqlParameter[] oaPara;

                //参数构建
                oaPara = new SqlParameter[0];

                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, "QSProc_GetQsLine", oaPara, "RourtData", 30).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                    r = JsonConvert.SerializeObject(dt, timeConverter);
                }
            }
            catch (Exception)
            {
                r = "";
            }
            return r;
        }
        private string getJsonQueryLineInfo(string key, int lineid)
        {
            string r = "";
            //string strCid = "";
            try
            {
                ComSqlHelper oSqlUtil = new ComSqlHelper();
                SqlParameter[] oaPara;

                //参数构建
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@LineID", lineid);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, "[QSProc_GetLineInfo]", oaPara, "LineInfo", 30).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                    r = JsonConvert.SerializeObject(dt, timeConverter);
                }
            }
            catch (Exception)
            {
                r = "";
            }
            return r;
        }




        private string insertLine2DB(string key, int LineAutoID, double[] Longitude, double[] Latitude)
        {

            try
            {
                ComSqlHelper oSqlUtil = new ComSqlHelper();
                SqlParameter[] oaPara;
                //删除原有线路
                oaPara = new SqlParameter[1];
                oaPara[0] = new SqlParameter("@LineAutoID", LineAutoID);
                int f = oSqlUtil.ExecuteSPNoQuery(key, "QSProc_deleteLineInfo", oaPara, false);
                //插入新的线路
                //参数构建
                oaPara = new SqlParameter[3];
                for (int i = 0; i < Longitude.Length; i++)
                {
                    oaPara[0] = new SqlParameter("@LineAutoID", LineAutoID);
                    oaPara[1] = new SqlParameter("@Longitude", Longitude[i]);
                    oaPara[2] = new SqlParameter("@Latitude", Latitude[i]);

                    int w = oSqlUtil.ExecuteSPNoQuery(key, "QSProc_InsertLineInfo", oaPara, false);
                }

                return "true";


            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }





        private string getJsonCarData(string key, string cname)
        {
            string r = "";
            //string strCid = "";
            try
            {
                ComSqlHelper oSqlUtil = new ComSqlHelper();
                SqlParameter[] oaPara;

                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@uid", cname);
                oaPara[1] = new SqlParameter("@cid", "");
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, "JLJGPT_getUserCarLicenseColorInfo", oaPara, "CarData", 30).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                    r = JsonConvert.SerializeObject(dt, timeConverter);
                }
            }
            catch (Exception)
            {
                r = "";
            }
            return r;
        }

        private string getJsonQueryCarData(string key, string CID)
        {
            string r = "";
            //string strCid = "";
            try
            {
                ComSqlHelper oSqlUtil = new ComSqlHelper();
                SqlParameter[] oaPara;

                //参数构建
                oaPara = new SqlParameter[2];
                oaPara[0] = new SqlParameter("@cid", CID);
                //oaPara[1] = new SqlParameter("@cid", CID);
                DataTable dt = new DataTable();
                dt = oSqlUtil.FillDataSet(key, "JLJGPT_CarInforCarDataSelect", oaPara, "CarData", 30).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                    r = JsonConvert.SerializeObject(dt, timeConverter);
                }
            }
            catch (Exception)
            {
                r = "";
            }
            return r;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}