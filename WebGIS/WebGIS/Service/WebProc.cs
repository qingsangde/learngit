using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;

namespace WebGIS
{
    class WebProc
    {
        public static DataTable dtProc = new DataTable();
        public static void Init()
        {
            string basepath = HttpRuntime.AppDomainAppPath.ToString();
            DataSet ds = new DataSet();
            ds.ReadXml(basepath + "Proc2.xml");
            dtProc = ds.Tables[0];
        }

        public static string Proc(string ID)
        {
            DataRow[] drs = dtProc.Select("ID='" + ID + "'");
            if (drs.Length == 1)
            {
                string se = drs[0]["ProcName"].ToString();
                return se.Replace("\r\n", "").Trim();
            }
            else
                return ID;
        }

        public static string GetAppSysflagKey(string sysflag)
        {
            string dbkey = sysflag;
            switch (sysflag)
            {

                case "A0EV":
                case "ALARM":
                    dbkey = "DPTEST"; break;
                case "LIUTE":
                    dbkey = "DPTEST"; break;


            }
            return dbkey;
        }
        public static string GetHttpSysflagKey(string sysflag)
        {
            //string dbkey = "DPTEST";
            string dbkey = sysflag;
            switch (sysflag)
            {

                case "A0EV":

                    dbkey = "DPTEST";
                    break;
                case "LIUTE":
                    dbkey = "JFJY"; break;
            }
            return dbkey;
        }
    }
}
