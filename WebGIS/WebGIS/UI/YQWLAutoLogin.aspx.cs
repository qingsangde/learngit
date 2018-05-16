using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using CommLibrary;
using System.Data;
using System.Text;
using System.Security.Cryptography;

namespace WebGIS.UI
{
    public partial class YQWLAutoLogin : System.Web.UI.Page
    {
        public static string GetMD5(string str)
        {
            //byte[] data = Encoding.GetEncoding("GB2312").GetBytes(str);
            // byte[] data = Encoding.Default.GetBytes(str);

            char[] temp = str.ToCharArray();
            byte[] data = new byte[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                data[i] = (byte)temp[i];
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            // return OutString.ToUpper();
            return OutString.ToLower();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            string user_name = Request["j_username"];
            string token = Request["token"];
            string ep_url = Request["ep_url"];
            string pass = Request["pass"];
            if (string.IsNullOrEmpty(user_name) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(ep_url) || string.IsNullOrEmpty(pass)) return;
            LogHelper.WriteInfo("YQWLAutoLogin:"+Request.Url.ToString());
            string json = "";
            string wd = GetMD5(user_name + "gps");
            LogHelper.WriteInfo("YQWLAutoLogin:本机MD5计算值" + wd);
            if (pass.Equals(wd))
            {

                LogHelper.WriteInfo("YQWLAutoLogin: MD5校验通过！"  );
                if (checkToken(user_name, token, ep_url, out json))
                {
                    Label1.Text = "合法性校验完成！";
                    ComSqlHelper csh = new ComSqlHelper();
                    SqlParameter[] Parameters = { new SqlParameter("@UserName", user_name) };
                    DataTable userdt = csh.FillDataSet("YQWL", WebProc.Proc("QWGProc_COM_UserInfoByName"), Parameters).Tables[0];
                    if (userdt.Rows.Count > 0)
                    {
                        string pw = userdt.Rows[0]["U_PassWd"].ToString();
                        string urlwe = "userLogin.html?account=" + user_name + "&password=" + pw + "&sysflag=YQWL";
                        Label1.Text += "正在进入系统……";
                        Response.Redirect(urlwe);

                    }
                    else
                    {
                        Label1.Text += "用户非本系统用户，请核对用户名。";
                    }
                }
                else
                {
                    Label1.Text = "合法性校验失败！远程回调校验失败。";
                }
            }
            else
            {
                Label1.Text = "合法性校验失败！用户名合法性校验失败。";
            }
        }

        private bool checkToken(string user_name, string token, string ep_url, out string json)
        {

            json = "";
            if (user_name == null || token == null || ep_url == null)
                return false;
            ep_url = "http://" + ep_url + "?cmd=getUserInfo&token=" + token;


            //   String ep_url= "http://10.133.92.23:7004/login.jsp?j_username=admin&token=0:11111&ep_url=10.133.92.23:7007/ep.do ";		
            json = CommLibrary.HttpUtility.Get(ep_url, "");

            if (json != null && json.IndexOf("\"user_name\":\"" + user_name + "\"") > 0)
            {
                LogHelper.WriteInfo("YQWLAutoLogin: 远程回调验证校验通过！");
                return true;
            }
            else
            {
                LogHelper.WriteInfo("YQWLAutoLogin: 远程回调验证校验失败！" + json);
                return false;
            }
        }
    }
}