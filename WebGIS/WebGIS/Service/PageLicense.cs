using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WebGIS
{
    public class PageLicense
    {
        public static DataTable dtPage = new DataTable();
        public static void Init()
        {
            string basepath = HttpRuntime.AppDomainAppPath.ToString();
            DataSet ds = new DataSet();
            ds.ReadXml(basepath + "PageLicense.xml");
            dtPage = ds.Tables[0];
        }

        public static string GetPageIdStr(string name)
        {
            DataRow[] drs = dtPage.Select("PageName='" + name + "'");
            if (drs.Length == 1)
            {
                string se = drs[0]["PageID"].ToString();
                return se.Replace("\r\n", "").Trim();
            }
            else
                return "";
        }
    }
}