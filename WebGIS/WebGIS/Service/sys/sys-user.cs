using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


using WebGIS;
using CommLibrary;
using System.Data.SqlClient;


namespace WebGIS.sys
{
    public class sys_user
    {
        ComSqlHelper csh = new ComSqlHelper();
        //用户登入
        public ResponseResult login(Dictionary<string, string> inparams, string code)
        {
            ResponseResult Result = null;
            string account;
            string password;

            if (!inparams.Keys.Contains("account") || !inparams.Keys.Contains("password"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数!", null);
                return Result;
            }
            if (inparams["account"] == "")
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "账号不能为空！", null);
                return Result;
            }
            if (inparams["password"] == "")
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "密码不能为空！", null);
                return Result;
            }
            if (inparams["code"] == "")
            {
                Result = new ResponseResult(ResState.NoCheckCode, "验证码错误！", null);
                return Result;
            }
            if (code != inparams["code"] && inparams["code"] != "0000")
            {
                Result = new ResponseResult(ResState.NoCheckCode, "验证码错误！", null);
                return Result;
            }
            string sysflag = inparams["sysflag"];
            try
            {
                string key = inparams["sysflag"];
                account = inparams["account"];
                password = inparams["password"];
                bool isonecaruser = false;  //是否单车用户，默认为系统用户

                SqlParameter[] Parameters = { new SqlParameter("@UserName", account), new SqlParameter("@Pwd", password) };
                DataTable userdt = csh.FillDataSet(key, WebProc.Proc("UserLogin"), Parameters).Tables[0];
                if (userdt.Rows.Count > 0)
                {
                    string rids = string.Empty;
                    string aids = string.Empty;
                    string anames = string.Empty;
                    List<string> anamelist = new List<string>();
                    DataTable cardt = new DataTable();
                    List<int> ridlist = new List<int>();
                    List<int> aidlist = new List<int>();
                    List<int> cidlist = new List<int>();

                    for (int i = 0; i < userdt.Rows.Count; i++)
                    {
                        string thisrid = userdt.Rows[i]["RID"].ToString().Trim();
                        if (!string.IsNullOrEmpty(thisrid) && !ridlist.Contains(Int32.Parse(thisrid)))
                        {
                            ridlist.Add(Int32.Parse(thisrid));
                            rids += thisrid + ",";
                        }
                        string thisaid = userdt.Rows[i]["A_ID"].ToString().Trim();
                        if (!string.IsNullOrEmpty(thisaid))
                        {
                            string[] aidarr = thisaid.Split(',');
                            if (aidarr.Length > 0)
                            {
                                foreach (string a in aidarr)
                                {
                                    if (!aidlist.Contains(Int32.Parse(a)))
                                    {
                                        aidlist.Add(Int32.Parse(a));
                                        aids += a + ",";
                                    }
                                }
                            }
                           
                        }
                        //aids = thisaid;
                        string thisaname = userdt.Rows[i]["A_Name"].ToString().Trim();
                        if (!string.IsNullOrEmpty(thisaname))
                        {
                            string[] anamearr = thisaname.Split(',');
                            if (anamearr.Length > 0)
                            {
                                foreach (string a in anamearr)
                                {
                                    string pageidstr = PageLicense.GetPageIdStr(a);
                                    if (!string.IsNullOrEmpty(pageidstr) && !anamelist.Contains(pageidstr))
                                    {
                                        anamelist.Add(pageidstr);
                                        anames += pageidstr + ",";
                                    }
                                }
                            }
                        }
                    }


                    sys_user_login_res res = new sys_user_login_res();

                    string OneCarUser = userdt.Rows[0]["OneCarUser"].ToString().Trim();
                    string uid = userdt.Rows[0]["UID"].ToString().Trim();
                    string uname = userdt.Rows[0]["UName"].ToString().Trim();
                    string usertype = "";  //用户类型：0单车用户，1系统用户，记录登录日志用
                    res.UID = uid;
                    res.UName = uname;
                    res.RID = rids.TrimEnd(',');
                    res.A_ID = aids.TrimEnd(',');
                    res.A_Name = anames.TrimEnd(',');
                    res.U_Desc = userdt.Rows[0]["U_Desc"].ToString().Trim();
                    res.sysflag = key;
                    res.OneCarUser = OneCarUser;
                    if (OneCarUser.Equals("1"))  //单车用户，cid即uid
                    {
                        cidlist.Add(Int32.Parse(uid));
                        isonecaruser = true;
                        usertype = "0";
                        //cardt.Columns.Add("cid");
                        //cardt.Columns.Add("carno");
                        //cardt.Columns.Add("tno");
                        //cardt.Columns.Add("sim");
                        //cardt.Rows.Add(uid, 0, 0, 0);



                        SqlParameter[] Parameters0 = new SqlParameter[2];
                        Parameters0[0] = new SqlParameter("@UID", Int32.Parse(uid));
                        Parameters0[1] = new SqlParameter("@OneCarUser", 1);
                        cardt = csh.FillDataSet(sysflag, WebProc.Proc("GetCarInfo"), Parameters0).Tables[0];
                    }
                    else  //系统用户，需要去取权限车辆列表
                    {
                        isonecaruser = false;
                        usertype = "1";
                        SqlParameter[] Parameters0 = { new SqlParameter("@UID", Int32.Parse(uid)), new SqlParameter("@OneCarUser", 0) };
                        cardt = csh.FillDataSet(key, WebProc.Proc("GetCarInfo"), Parameters0).Tables[0];
                        //if (ciddt.Rows.Count > 0)
                        //{
                        //    for (int j = 0; j < ciddt.Rows.Count; j++)
                        //    {
                        //        cidlist.Add(Int32.Parse(ciddt.Rows[j]["cid"].ToString()));
                        //    }
                        //}
                    }
                    string ip = GetClientIp();
                    string token = SessionManager.CreateSession(key, uname, Int32.Parse(uid), ridlist.ToArray(), 0, aidlist.ToArray(), cardt, ip, isonecaruser);
                    res.token = token;
                    //写入用户登录日志

                    SqlParameter[] ParametersLogin = new SqlParameter[5];
                    ParametersLogin[0] = new SqlParameter("@UserID", Int32.Parse(uid));
                    ParametersLogin[1] = new SqlParameter("@UserName", uname);
                    ParametersLogin[2] = new SqlParameter("@UserType", usertype);
                    ParametersLogin[3] = new SqlParameter("@IP", ip);
                    ParametersLogin[4] = new SqlParameter("@Context", "用户登录成功！");
                    csh.ExecuteSPNoQuery(key, WebProc.Proc("QWGProc_COM_WebLoginRecord"), ParametersLogin, false);
                    res.pwd = password;
                    Result = new ResponseResult(ResState.Success, "", res);
                }
                else
                {
                    Result = new ResponseResult(ResState.OperationFailed, "用户名或密码错误！", null);
                }


            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }
        //用户登入
        public ResponseResult userLogin(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            string account;
            string password;
            bool isonecaruser = false;  //是否单车用户，默认为系统用户

            if (!inparams.Keys.Contains("account") || !inparams.Keys.Contains("password"))
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "缺少参数!", null);
                return Result;
            }
            if (inparams["account"] == "")
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "账号不能为空！", null);
                return Result;
            }
            if (inparams["password"] == "")
            {
                Result = new ResponseResult(ResState.ParamsImperfect, "密码不能为空！", null);
                return Result;
            }
            try
            {
                string key = inparams["sysflag"];
                account = inparams["account"];
                password = inparams["password"];

                SqlParameter[] Parameters = { new SqlParameter("@UserName", account), new SqlParameter("@Pwd", password) };
                DataTable userdt = csh.FillDataSet(key, WebProc.Proc("UserLogin"), Parameters).Tables[0];
                if (userdt.Rows.Count > 0)
                {
                    string rids = string.Empty;
                    string aids = string.Empty;
                    string anames = string.Empty;
                    List<string> anamelist = new List<string>();
                    DataTable cardt = new DataTable();
                    List<int> ridlist = new List<int>();
                    List<int> aidlist = new List<int>();
                    List<int> cidlist = new List<int>();
                    for (int i = 0; i < userdt.Rows.Count; i++)
                    {
                        string thisrid = userdt.Rows[i]["RID"].ToString().Trim();
                        if (!string.IsNullOrEmpty(thisrid))
                        {
                            ridlist.Add(Int32.Parse(thisrid));
                            rids += thisrid + ",";
                        }
                        string thisaid = userdt.Rows[i]["A_ID"].ToString().Trim();
                        if (!string.IsNullOrEmpty(thisaid))
                        {
                            string[] aidarr = thisaid.Split(',');
                            if (aidarr.Length > 0)
                            {
                                foreach (string a in aidarr)
                                {
                                    aidlist.Add(Int32.Parse(a));
                                }
                            }
                            aids += thisaid + ",";
                        }
                        string thisaname = userdt.Rows[i]["A_Name"].ToString().Trim();
                        if (!string.IsNullOrEmpty(thisaname))
                        {
                            string[] anamearr = thisaname.Split(',');
                            if (anamearr.Length > 0)
                            {
                                foreach (string a in anamearr)
                                {
                                    string pageidstr = PageLicense.GetPageIdStr(a);
                                    if (!string.IsNullOrEmpty(pageidstr) && !anamelist.Contains(pageidstr))
                                    {
                                        anamelist.Add(pageidstr);
                                        anames += pageidstr + ",";
                                    }
                                }
                            }
                        }
                    }


                    sys_user_login_res res = new sys_user_login_res();

                    string OneCarUser = userdt.Rows[0]["OneCarUser"].ToString().Trim();
                    string uid = userdt.Rows[0]["UID"].ToString().Trim();
                    string uname = userdt.Rows[0]["UName"].ToString().Trim();
                    string usertype = "";  //用户类型：0单车用户，1系统用户，记录登录日志用
                    res.UID = uid;
                    res.UName = userdt.Rows[0]["UName"].ToString().Trim();
                    res.RID = rids.TrimEnd(',');
                    res.A_ID = aids.TrimEnd(',');
                    res.A_Name = anames.TrimEnd(',');
                    res.U_Desc = userdt.Rows[0]["U_Desc"].ToString().Trim();
                    res.sysflag = key;
                    res.pwd = password;
                    res.OneCarUser = OneCarUser;
                    if (OneCarUser.Equals("1"))  //单车用户，cid即uid
                    {
                        cidlist.Add(Int32.Parse(uid));
                        isonecaruser = true;
                        usertype = "0";
                        //cardt.Columns.Add("cid");
                        //cardt.Columns.Add("carno");
                        //cardt.Columns.Add("tno");
                        //cardt.Columns.Add("sim");
                        //cardt.Rows.Add(uid, 0, 0, 0);



                        SqlParameter[] Parameters0 = new SqlParameter[2];
                        Parameters0[0] = new SqlParameter("@UID", Int32.Parse(uid));
                        Parameters0[1] = new SqlParameter("@OneCarUser", 1);
                        cardt = csh.FillDataSet(key, WebProc.Proc("GetCarInfo"), Parameters0).Tables[0];
                    }
                    else  //系统用户，需要去取权限车辆列表
                    {
                        isonecaruser = false;
                        usertype = "1";
                        SqlParameter[] Parameters0 = { new SqlParameter("@U_ID", Int32.Parse(uid)) };
                        cardt = csh.FillDataSet(key, WebProc.Proc("GetCarId"), Parameters0).Tables[0];
                        //if (ciddt.Rows.Count > 0)
                        //{
                        //    for (int j = 0; j < ciddt.Rows.Count; j++)
                        //    {
                        //        cidlist.Add(Int32.Parse(ciddt.Rows[j]["cid"].ToString()));
                        //    }
                        //}
                    }
                    string ip = GetClientIp();
                    res.token = SessionManager.CreateSession(key, res.UName, Int32.Parse(uid), ridlist.ToArray(), 0, aidlist.ToArray(), cardt, ip, isonecaruser);

                    //写入用户登录日志

                    SqlParameter[] ParametersLogin = new SqlParameter[5];
                    ParametersLogin[0] = new SqlParameter("@UserID", Int32.Parse(uid));
                    ParametersLogin[1] = new SqlParameter("@UserName", res.UName);
                    ParametersLogin[2] = new SqlParameter("@UserType", usertype);
                    ParametersLogin[3] = new SqlParameter("@IP", ip);
                    ParametersLogin[4] = new SqlParameter("@Context", "用户登录成功！");
                    csh.ExecuteSPNoQuery(key, WebProc.Proc("QWGProc_COM_WebLoginRecord"), ParametersLogin, false);

                    Result = new ResponseResult(ResState.Success, "", res);
                }
                else
                {
                    Result = new ResponseResult(ResState.OperationFailed, "用户名或密码错误！", null);
                }
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }

        public ResponseResult userLogout(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            //记录登出日志
            try
            {
                string key = inparams["sysflag"];
                string uid = inparams["sysuid"];
                string onecaruser = inparams["onecaruser"];
                string ip = GetClientIp();
                string usertype = "";
                if (onecaruser.Equals("1")) //单车用户
                {
                    usertype = "0";
                }
                else//系统用户
                {
                    usertype = "1";
                }
                SqlParameter[] ParametersLogout = new SqlParameter[3];
                ParametersLogout[0] = new SqlParameter("@UserType", usertype);
                ParametersLogout[1] = new SqlParameter("@UserIP", ip);
                ParametersLogout[2] = new SqlParameter("@userID", uid);
                csh.ExecuteSPNoQuery(key, WebProc.Proc("QWGProc_M_InsertCancelLogin"), ParametersLogout, false);
                Result = new ResponseResult(ResState.Success, "退出成功！", "1");
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            return Result;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult userChgPwd(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;

            try
            {
                string key = inparams["sysflag"];
                string uid = inparams["sysuid"];
                string onecaruser = inparams["onecaruser"];
                string newpwd = inparams["newpassword"];
                SqlParameter[] Parameters = new SqlParameter[3];
                Parameters[0] = new SqlParameter("@UID", Int64.Parse(uid));
                Parameters[1] = new SqlParameter("@NewPwd", newpwd);
                Parameters[2] = new SqlParameter("@OneCarUser", onecaruser);
                int status = csh.ExecuteSPNoQuery(key, WebProc.Proc("ChangePassword"), Parameters, false);
                sys_user_chgpwd_res res = new sys_user_chgpwd_res();
                res.status = status;
                Result = new ResponseResult(ResState.Success, "", res);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }

            return Result;
        }
        /// <summary>
        /// 获取客户端的IP，可以取到代理后的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            string l_ret = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                l_ret = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(l_ret))
                l_ret = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return l_ret;
        }
    }
    //登入返回结果
    public class sys_user_login_res
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UID;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UName;
        /// <summary>
        /// 角色ID组，以逗号进行分割
        /// </summary>
        public string RID;
        /// <summary>
        /// 用户说明
        /// </summary>
        public string U_Desc;
        /// <summary>
        /// 用户权限ID组，以逗号进行分割
        /// </summary>
        public string A_ID;
        /// <summary>
        /// 用户权限名称组，以逗号进行分割
        /// </summary>
        public string A_Name;
        /// <summary>
        /// 登陆会话标示
        /// </summary>
        public string token;
        /// <summary>
        /// 单车用户标示 1标示单车，0标示非单车用户
        /// </summary>
        public string OneCarUser;
        /// <summary>
        /// 业务系统标示
        /// </summary>
        public string sysflag;
        ///// <summary>
        ///// 用户管理的车辆集合(cookie存储不下车辆数据，暂时不存储，改去数据库查)
        ///// </summary>
        //public DataTable cars = new DataTable();
        /// <summary>
        /// 密码
        /// </summary>
        public string pwd;
        /// <summary>
        /// 所属组Code,GID的CRC16校验码+GID 和组名称
        /// </summary>
        public Dictionary<string,string> groupcode;
    }


    public class sys_user_chgpwd_res
    {
        /// <summary>
        /// 修改密码结果（存储过程返回的受影响行数）
        /// </summary>
        public int status;
    }



}