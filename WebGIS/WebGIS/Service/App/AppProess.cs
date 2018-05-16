using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;
using SysService;
using WebGIS.Service;

namespace WebGIS
{
    public class AppProess
    {
        private DataTable GetUserCars(string sysflag, string uid)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetUserCars"), Parameters).Tables[0];

            return dt;
        }
        private long GetCarCIDBySimCode(string sysflag, string sim)
        {
            long cid = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@SIMCode", sim) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetCidBySimcode"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cid = long.Parse(dt.Rows[0]["CID"].ToString());
            }
            return cid;
        }
        private long GetCarCID(string sysflag, string carno)
        {
            long cid = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@carno", carno), new SqlParameter("@lpc", "") };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QSProc_Car_SelectByCno"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cid = long.Parse(dt.Rows[0]["CID"].ToString());
            }
            return cid;
        }
        private long GetCidByTno(string sysflag, string tno)
        {
            long cid = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@tno", tno) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWCProc_LT_GetCidByTno"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cid = long.Parse(dt.Rows[0]["cid"].ToString());
            }
            return cid;
        }
        private string GetCarSIMCode(string sysflag, string cid)
        {
            string simcode = "";
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWCProc_LC_GetSimTnoByCid"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                simcode = dt.Rows[0]["SimCode"].ToString();
            }
            return simcode;
        }
        private long GetCarTNO(string sysflag, string cid)
        {
            long tno = 0;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_SC_GetTnoByCid"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                tno = long.Parse(dt.Rows[0]["tno"].ToString());
            }
            return tno;
        }
        private string GetCarCarno(string sysflag, string cid)
        {
            string carno = "";
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_SC_GetTnoByCid"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                carno = dt.Rows[0]["carno"].ToString();
            }
            return carno;
        }
        private int CheckUserNameMobileNum(string sysflag, string username, string mobilenum)
        {
            int res = 0;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@username", username), new SqlParameter("@mobilenum", mobilenum) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_CheckUserNameMobileNum"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = int.Parse(dt.Rows[0][0].ToString());
            }
            return res;
        }


        #region 根据用户名获取手机号码
        /// <summary>
        /// 根据用户名获取手机号码
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="username"></param>
        /// <param name="mobilenum"></param>
        /// <returns></returns>
        private DataTable CheckUserName(string sysflag, string username)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@username", username) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_CheckUserName"), Parameters).Tables[0];

            return dt;
        }
        #endregion

        private int GetUIDUserCheck(string sysflag, string usernameormobilenum, string pwd, out string username)
        {
            username = "";
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@nameormobile", usernameormobilenum) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetUserByNameORMobilenum"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string dbpwd = dt.Rows[0]["PASSWORD"].ToString();
                if (pwd == dbpwd)
                {
                    username = dt.Rows[0]["Name"].ToString();
                    return int.Parse(dt.Rows[0]["UID"].ToString());
                }
            }
            return 0;
        }
        private userlogin GetUserRolesAndRights(string sysflag, int uid, string logintype)
        {
            userlogin model = new userlogin();
            model.uid = uid.ToString();
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid), new SqlParameter("@logintype", logintype) };
            DataSet ds = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetUserRolesAndRights"), Parameters);
            DataTable dtrole = ds.Tables[0];
            DataTable dtrights = ds.Tables[1];
            foreach (DataRow dr in dtrole.Rows)
            {
                model.roles.Add(dr["RID"].ToString());
                model.rolenames.Add(dr["NAME"].ToString());
            }
            foreach (DataRow dr in dtrights.Rows)
            {
                model.rights.Add(dr["FID"].ToString());

            }
            return model;
        }
        private usermodel GetUserInfo(string sysflag, int uid)
        {
            usermodel model = new usermodel();
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid) };
            DataSet ds = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetUserInfo"), Parameters);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                model.uid = uid;
                model.pwd = dt.Rows[0]["PASSWORD"].ToString();
                model.uname = dt.Rows[0]["Name"].ToString();
                model.mobilenum = dt.Rows[0]["MobileNum"].ToString();
            }
            else
            {
                return null;
            }
            return model;
        }
        private void UserRegister(string sysflag, string username, string mobilenum, string pwd, string source)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@username", username),
                                        new SqlParameter("@mobilenum", mobilenum),
                                        new SqlParameter("@pwd", pwd),
                                        new SqlParameter("@source", source)};
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_UserRegister"), Parameters, true);

        }
        private void UserInfoUpdate(string sysflag, string uid, string username, string mobilenum, string pwd, string desc, string email, string fullname)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid),
                                          //  new SqlParameter("@username", username),
                                        new SqlParameter("@mobilenum", mobilenum),
                                        new SqlParameter("@pwd", pwd),
                                        new SqlParameter("@desc", desc),
                                        new SqlParameter("@email", email),
                                        new SqlParameter("@fullname", fullname)};
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_UserUpdate"), Parameters, true);

        }
        private void UserPwdChange(string sysflag, string uid, string pwd)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid),
                                        new SqlParameter("@pwd", @pwd)  };
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_UserChangePwd"), Parameters, true);

        }
        private void UserCarBind(string sysflag, string cid, string uid, string carno, string color, string cartype)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid),
                                        new SqlParameter("@uid", uid),
                                        new SqlParameter("@CarNo", carno),
                                        new SqlParameter("@CarColor", color),
                                        new SqlParameter("@VehicleType", cartype)};
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_CarBind"), Parameters, true);

        }
        private int DeleteUserCarBind(string sysflag, string cid, string uid)
        {
            int res = -1;
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@cid", cid),
                                        new SqlParameter("@uid", uid) };
            //csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_DeleteUserCars"), Parameters, true);
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_DeleteUserCars"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = int.Parse(dt.Rows[0][0].ToString());
            }
            return res;
        }
        //private void UserUpdateInfo(string sysflag, string uid, string username, string mobilenum, string memo, string pwd)
        //{
        //    ComSqlHelper csh = new ComSqlHelper();
        //    SqlParameter[] Parameters = { new SqlParameter("@uid", uid),
        //                                new SqlParameter("@username", @pwd) ,
        //                                new SqlParameter("@mobilenum", @pwd) ,
        //                                new SqlParameter("@memo", @pwd) ,
        //                                new SqlParameter("@pwd", @pwd)   
        //                                };
        //    csh.ExecuteSPNoQuery(WebProc.GetDBKey(sysflag), WebProc.Proc("QWGProc_App_UserUpdate"), Parameters, true);
        //}

        #region 验证码生成
        private string GenerateCheckCode()
        {

            int number;
            char code;
            string checkCode = String.Empty;
            System.Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                number = random.Next();
                //if (number % 2 == 0)
                //    code = (char)('0' + (char)(number % 10));
                //else
                //    code = (char)('A' + (char)(number % 26));
                code = (char)('0' + (char)(number % 10));
                checkCode += code.ToString();
            }


            return checkCode;
        }

        #endregion



        #region 用户登录
        public ResponseAppResult AppUserLogin(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("username") || inparams["username"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少username或username为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("pwd") || inparams["pwd"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少pwd或pwd为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("logintype") || inparams["logintype"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少logintype或logintype为空！", null);
                return Result;
            }
            try
            {
                string sysflag = inparams["sysflag"];
                string pwd = inparams["pwd"];
                string username = inparams["username"];
                string logintype = inparams["logintype"];

                string dbusername = "";
                int uid = GetUIDUserCheck(sysflag, username, pwd, out dbusername);

                if (uid > 0)
                {
                    userlogin ul = GetUserRolesAndRights(sysflag, uid, logintype);
                    ul.uname = dbusername;
                    Result = new ResponseAppResult(ResState.Success, "操作成功", ul);
                    loginlog(sysflag, uid.ToString(), username, "1", "", logintype.Trim());
                }
                else
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "用户名或密码错误！", null);
                    return Result;
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserRegister调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        private void loginlog(string sysflag,string uid,string uname,string usertype,string ip,string sysName)
        {
            if (sysName == "2")
            {
                sysName = "weixin";
            }
            else if (sysName == "0")
            {
                sysName = "android";
            }
            else if (sysName == "1")
            {
                sysName = "ios";
            }
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] ParametersLogin = new SqlParameter[6];
            ParametersLogin[0] = new SqlParameter("@UserID", Int32.Parse(uid));
            ParametersLogin[1] = new SqlParameter("@UserName", uname);
            ParametersLogin[2] = new SqlParameter("@UserType", usertype);
            ParametersLogin[3] = new SqlParameter("@IP", ip);
            ParametersLogin[4] = new SqlParameter("@Context", "用户登录成功！");
            ParametersLogin[5] = new SqlParameter("@SysName", sysName);
            csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("QWGProc_COM_WebLoginRecord"), ParametersLogin, false);
        }
        #endregion
        /// <summary>
        /// 自动判断用户所属系统,并登录
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppUserLogin2(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;
            Dictionary<string, userlogin> resarray = new Dictionary<string, userlogin>();
            if (!inparams.Keys.Contains("username") || inparams["username"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少username或username为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("pwd") || inparams["pwd"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少pwd或pwd为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("logintype") || inparams["logintype"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少logintype或logintype为空！", null);
                return Result;
            }
            try
            {

                string pwd = inparams["pwd"];
                string username = inparams["username"];
                string logintype = inparams["logintype"];
                string[] allsys = new string[] { "YQWL", "DPTEST" };
                foreach (string sysflag in allsys)
                {
                    string dbusername = "";
                    int uid = GetUIDUserCheck(sysflag, username, pwd, out dbusername);

                    if (uid > 0)
                    {
                        userlogin ul = GetUserRolesAndRights(sysflag, uid, logintype);
                        ul.uname = dbusername;
                        resarray.Add(sysflag, ul);
                        loginlog(sysflag, uid.ToString(), username, "1", "", logintype);
                    }
                }
                if (resarray.Count == 0)
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "用户名或密码错误！", null);
                    return Result;
                }
                else
                {
                    Result = new ResponseAppResult(ResState.Success, "操作成功", resarray);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserRegister调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }

        #region 用户注册
        public ResponseAppResult AppUserRegister(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("username") || inparams["username"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少username或username为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("pwd") || inparams["pwd"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少pwd或pwd为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("mobilenum") || inparams["mobilenum"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少mobilenum或mobilenum为空！", null);
                return Result;
            }
            //if (!inparams.Keys.Contains("smscode") || inparams["smscode"] == "")
            //{
            //    Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少smscode或smscode为空！", null);
            //    return Result;
            //}
            try
            {
                string sysflag = inparams["sysflag"];
                string mobilenum = inparams["mobilenum"];
                string pwd = inparams["pwd"];
                string username = inparams["username"];
                string source = inparams["source"];
                //string smscode = inparams["smscode"];

                //if (MessageCodeVerification.dictionMsg.ContainsKey(smscode))//smscode与静态变量中key做比较是否存在
                //{
                //验证用户名和手机号码是否重复
                int s = CheckUserNameMobileNum(sysflag, username, mobilenum);

                if (s == 0)
                {
                    UserRegister(sysflag, username, mobilenum, pwd, source);
                    //Result = new ResponseAppResult(ResState.Success, "操作成功", null);
                    string dbusername = "";
                    int uid = GetUIDUserCheck(sysflag, username, pwd, out dbusername);

                    if (uid > 0)
                    {
                        userlogin ul = GetUserRolesAndRights(sysflag, uid, source);
                        ul.uname = dbusername;
                        Result = new ResponseAppResult(ResState.Success, "操作成功", ul);

                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "注册失败", null);
                        return Result;
                    }

                }
                else if (s == 1)
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "用户名已存在！", null);
                    return Result;
                }
                else if (s == 2)
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "手机号码已经注册！", null);
                    return Result;
                }
                else if (s == 3)
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "用户已经注册！", null);
                    return Result;
                }
                //}
                //else
                //{ 
                //    Result = new ResponseAppResult(ResState.Success, "验证码错误！", null);
                //}
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserRegister调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion
        public ResponseAppResult AppGetAllOnline(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = Result = new ResponseAppResult(ResState.Success, "操作成功", RDSConfig.GetAllOnlineCars(inparams["sysflag"]));
            return Result;
        }

        #region 获取短信验证码
        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppSMSCode(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;
            if (!inparams.Keys.Contains("mobilenum") || inparams["mobilenum"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少mobilenum或mobilenum为空！", null);
                return Result;
            }
            try
            {
                string sysflag = inparams["sysflag"];
                string mobilenum = inparams["mobilenum"];
                string code = GenerateCheckCode();
                string respsms = CommonHelper.SendSmsVerCode(code, mobilenum);
                if (respsms == "")
                {
                    smscodemodel sd = new smscodemodel();
                    sd.smscode = code;
                    sd.dtsendtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    Result = new ResponseAppResult(ResState.Success, "操作成功", sd);

                    //发送短信成功后，将其验证码和手机号码进行保存；
                    PhoneInfo phone = new PhoneInfo();
                    phone.phone = mobilenum;
                    phone.time = DateTime.Now;
                    MessageCodeVerification.dictionMsg.Add(code, phone);

                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "短信发送失败！" + respsms, null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppSmscheckcode调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion


        #region  用户重置密码
        /// <summary>
        /// 用户重置密码
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppUserResetPwd(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;
            if (!inparams.Keys.Contains("username") || inparams["username"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少username或username为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("smscode") || inparams["smscode"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少smscode或smscode为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("newpwd") || inparams["newpwd"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少newpwd或newpwd为空！", null);
                return Result;
            }
            try
            {
                string sysflag = inparams["sysflag"];
                string username = inparams["username"];
                string smscode = inparams["smscode"];
                string newpwd = inparams["newpwd"];

                DataTable dt = CheckUserName(sysflag, username);    //查找该用户名是否存在（支持手机号码作为用户名查询）
                if (dt.Rows.Count > 0)
                {
                    string uid = dt.Rows[0]["UID"].ToString().Trim();
                    //该用户存在查找其手机号码
                    string mobileNum = dt.Rows[0]["MobileNum"].ToString().Trim();

                    if (MessageCodeManager.CheckSms(smscode, mobileNum))            //判断验证码是否存在于公共静态变量中
                    {

                        //调用重置密码方法
                        UserPwdChange(sysflag, uid, newpwd);
                        Result = new ResponseAppResult(ResState.Success, "操作成功！", null);

                        MessageCodeManager.DelSmsByCode(smscode);       //当前验证码存在于静态变量中，将其删除（避免生成重复的验证码）
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "验证码错误！", null);

                    }
                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "用户不存在", null);
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserResetPwd调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }

        #endregion


        #region 用户修改密码
        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppUserChangePwd(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;


            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("oldpwd") || inparams["oldpwd"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少oldpwd或oldpwd为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("newpwd") || inparams["newpwd"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少newpwd或newpwd为空！", null);
                return Result;
            }
            try
            {
                string sysflag = inparams["sysflag"];
                string newpwd = inparams["newpwd"];
                string uid = inparams["uid"];
                string oldpwd = inparams["oldpwd"];

                usermodel um = GetUserInfo(sysflag, int.Parse(uid));
                if (um != null)
                {
                    if (um.pwd == oldpwd)
                    {
                        UserPwdChange(sysflag, uid, newpwd);
                        Result = new ResponseAppResult(ResState.Success, "操作成功", null);
                    }
                    else
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "原密码错误", null);
                    }

                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "用户不存在", null);
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserChangePwd调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion

        #region 用户基本信息修改
        /// <summary>
        /// 用户基本信息修改
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppUserUpdate(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;


            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("username") || inparams["username"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少username或username为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("mobilenum") || inparams["mobilenum"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少mobilenum或mobilenum为空！", null);
                return Result;
            }
            try
            {
                string sysflag = inparams["sysflag"];
                string username = inparams["username"];
                string uid = inparams["uid"];
                string mobilenum = inparams["mobilenum"];
                string memo = inparams["memo"];
                //string pwd = inparams["pwd"];
                string email = inparams["email"];
                string fullname = inparams["username"];
                usermodel um = GetUserInfo(sysflag, int.Parse(uid));
                if (um != null)
                {
                    UserInfoUpdate(sysflag, uid, username, mobilenum, memo, um.pwd, email, fullname);
                    Result = new ResponseAppResult(ResState.Success, "操作成功", null);

                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "用户不存在", null);
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserUpdate调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion


        #region 用户车辆绑定
        /// <summary>
        /// 用户车辆绑定
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppUserCarBind(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("carno") || inparams["carno"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少carno或carno为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("sim") || inparams["sim"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少sim或sim为空！", null);
                return Result;
            }
            try
            {
                int value;
                string gocde = inparams.ContainsKey("gocde") ? inparams["gocde"] : "";  //隶属组编码
                string sysflag = inparams["sysflag"];
                string carno = inparams["carno"];
                string uid = inparams["uid"];
                string sim = inparams["sim"];
                string color = inparams["color"];
                string cartype = inparams["cartype"];
                long cid = GetCarCIDBySimCode(sysflag, sim);

                //===根据隶属组编码进行校验==
                if (!string.IsNullOrEmpty(gocde))
                {
                    if (CommLibrary.CRC16.CheckCode(gocde, out value))
                    {
                        //解析正确 调用车辆分组信息插入表；
                        GroupCarInsert(sysflag, value, Convert.ToInt32(uid), Convert.ToInt32(cid));
                    }
                    else
                    {
                        //解析错误
                        Result = new ResponseAppResult(ResState.OperationFailed, "隶属组编码不正确！", null);
                        return Result;
                    }
                }
                //===============================

                if (cid > 0)
                {
                    //用户车辆绑定
                    UserCarBind(sysflag, cid.ToString(), uid, carno, color, cartype);
                    Result = new ResponseAppResult(ResState.Success, "操作成功", null);
                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "SIM卡号码错误", null);
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserCarBind调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion


        #region 获取在线车辆
        public DataTable GetUserCrsbyOnline(string sysflag, string uid)
        {
            DataTable dt = GetUserCars(sysflag, uid);
            dt.Columns.Add("online");

            List<long> cids = new List<long>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cids.Add(long.Parse(dt.Rows[i]["cid"].ToString()));
            }

            monitor mon = new monitor();
            WebGIS.RealtimeDataServer.CarRealData[] realdata = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), cids.ToArray());
            Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
            foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
            {
                carOnlineStatus.Add(w.Carid, w.OnlineStatus);
            }

            foreach (DataRow dr in dt.Rows)
            {
                long cid = long.Parse(dr["cid"].ToString());
                if (carOnlineStatus.ContainsKey(cid))
                    dr["online"] = carOnlineStatus[cid];
                else
                {
                    dr["online"] = 2;
                }
            }
            return dt;
        }
        #endregion

        #region 获取用户车辆
        public ResponseAppResult AppGetUserCars(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;


            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];

                string uid = inparams["uid"];
                DataTable dt = GetUserCars(sysflag, uid);
                dt.Columns.Add("online");

                List<long> cids = new List<long>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cids.Add(long.Parse(dt.Rows[i]["cid"].ToString()));

                }

                monitor mon = new monitor();
                WebGIS.RealtimeDataServer.CarRealData[] realdata = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), cids.ToArray());
                Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                {
                    carOnlineStatus.Add(w.Carid, w.OnlineStatus);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    long cid = long.Parse(dr["cid"].ToString());
                    if (carOnlineStatus.ContainsKey(cid))
                        dr["online"] = carOnlineStatus[cid];
                    else
                    {
                        dr["online"] = 2;
                    }
                }
                ResList res = new ResList();
                res.isallresults = 1;
                res.records = dt;
                res.total = dt.Rows.Count;
                res.size = 0;
                res.page = 0;

                Result = new ResponseAppResult(ResState.Success, "操作成功", res);



            }
            catch (Exception ex)
            {
                LogHelper.WriteError("GetAppUserCars调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion




        #region 查询车辆列表
        /// <summary>
        /// 查询车辆列表
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppSeachUserCars(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            string top = "";
            string carno = "";
            string isonline = "";
            string isalarm = "";
            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }
            if (inparams.Keys.Contains("top"))
            {
                top = inparams["top"];
            }
            if (inparams.Keys.Contains("carno"))
            {
                carno = inparams["carno"];
            }
            if (inparams.Keys.Contains("isonline"))
            {
                isonline = inparams["isonline"];
            }

            try
            {

                string sysflag = inparams["sysflag"];

                string uid = inparams["uid"];
                DataTable dt = GetUserCars(sysflag, uid);
                dt.Columns.Add("online");
                DataColumn dc = new DataColumn("stno", typeof(string));
                dt.Columns.Add(dc);

                List<long> cids = new List<long>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["cid"] == null || dt.Rows[i]["cid"].ToString() == "") continue;
                    cids.Add(long.Parse(dt.Rows[i]["cid"].ToString()));
                    dt.Rows[i]["stno"] = dt.Rows[i]["tno"].ToString();
                }
                dt.Columns.Remove("tno");
                dc.ColumnName = "tno";

                //monitor mon = new  monitor();
                //WebGIS.RealtimeDataServer.CarRealData[] realdata = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), cids.ToArray());
                //Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                //foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                //{
                //    carOnlineStatus.Add(w.Carid, w.OnlineStatus);
                //}

                List<CarsStatus> realdata = RDSConfig.GetCarOnlineStatus(sysflag, cids.ToArray());
                Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                foreach (CarsStatus carStatus in realdata)
                {
                    carOnlineStatus.Add(carStatus.cid, carStatus.status);
                }

                int online = 0, offline = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["cid"] == null || dr["cid"].ToString() == "") continue;
                    long cid = long.Parse(dr["cid"].ToString());
                    if (carOnlineStatus.ContainsKey(cid))
                    {
                        dr["online"] = carOnlineStatus[cid];
                        if (carOnlineStatus[cid] == 1)
                        {
                            online++;
                        }
                        else
                        {
                            offline++;
                        }
                    }
                    else
                    {
                        dr["online"] = 2;
                        offline++;
                    }
                }
                string sqlwhere = "1=1  ";
                if (carno != "")
                {
                    sqlwhere += " and  carno like '%" + carno + "%' ";
                }
                if (isonline == "1" || isonline == "2")
                {
                    sqlwhere += " and  online ='" + isonline + "'";
                }
                DataRow[] drs = dt.Select(sqlwhere);
                usercarslist res = new usercarslist();


                int inttop = 0;
                if (top != "" && int.TryParse(top, out   inttop) && inttop > 0 && inttop < drs.Length)
                {
                    res.isallresults = 0;
                    res.records = ToDataTable(drs, inttop);
                    res.recordslenght = inttop;
                }
                else
                {
                    res.isallresults = 1;
                    res.records = ToDataTable(drs);
                    res.recordslenght = drs.Length;
                }

                res.total = dt.Rows.Count;
                res.offline = offline;
                res.online = online;


                Result = new ResponseAppResult(ResState.Success, "操作成功", res);



            }
            catch (Exception ex)
            {
                LogHelper.WriteError("GetAppUserCars调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }

        #endregion


        #region  用户车辆检索带分页
        /// <summary>
        /// 用户车辆检索带分页
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppSeachForPages(Dictionary<string, string> inparams)
        {

            ResponseAppResult Result = null;
            string isonline = "";
            string carno = "";

            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }

            if (!inparams.Keys.Contains("page") || inparams["page"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少page或page为空！", null);
                return Result;
            }

            if (!inparams.Keys.Contains("size") || inparams["size"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少size或size为空！", null);
                return Result;
            }

            isonline = inparams.ContainsKey("isonline") ? inparams["isonline"] : "";    //是否显示在线 1.在线；2.离线
            carno = inparams.ContainsKey("carno") ? inparams["carno"] : "";//车牌号码

            try
            {
                string sysflag = inparams["sysflag"];
                string uid = inparams["uid"];
                int page = Convert.ToInt32(inparams["page"]);
                int size = Convert.ToInt32(inparams["size"]);
                DataTable dt = GetUserCars(sysflag, uid);

                dt.Columns.Add("online");
                DataColumn dc = new DataColumn("stno", typeof(string));
                dt.Columns.Add(dc);

                List<long> cids = new List<long>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["cid"] == null || dt.Rows[i]["cid"].ToString() == "") continue;
                    cids.Add(long.Parse(dt.Rows[i]["cid"].ToString()));
                    dt.Rows[i]["stno"] = dt.Rows[i]["tno"].ToString();
                }
                dt.Columns.Remove("tno");
                dc.ColumnName = "tno";

                //monitor mon = new monitor();
                //WebGIS.RealtimeDataServer.CarRealData[] realdata = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), cids.ToArray());
                //Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                //foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                //{
                //    carOnlineStatus.Add(w.Carid, w.OnlineStatus);
                //}


                List<CarsStatus> realdata = RDSConfig.GetCarOnlineStatus(sysflag, cids.ToArray());
                Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                foreach (CarsStatus carStatus in realdata)
                {
                    carOnlineStatus.Add(carStatus.cid, carStatus.status);
                }


                int online = 0, offline = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["cid"] == null || dr["cid"].ToString() == "") continue;
                    long cid = long.Parse(dr["cid"].ToString());
                    if (carOnlineStatus.ContainsKey(cid))
                    {
                        dr["online"] = carOnlineStatus[cid];
                        if (carOnlineStatus[cid] == 1)
                        {
                            online++;
                        }
                        else
                        {
                            offline++;
                        }
                    }
                    else
                    {
                        dr["online"] = 2;
                        offline++;
                    }
                }

                string sqlwhere = "1=1  ";
                if (carno != "")
                {
                    sqlwhere += " and  carno like '%" + carno + "%' ";
                }
                if (isonline == "1" || isonline == "2")
                {
                    sqlwhere += " and  online ='" + isonline + "'";
                }
                DataRow[] drs = dt.Select(sqlwhere);
                usercarslistPage res = new usercarslistPage();

                int current = 0;    //当前记录条数
                int count = drs.Length;
                int s_page = count % size == 0 ? count / size : (count / size) + 1; //一共有多少页
                if (s_page <= 1) res.isallresults = 0;       //所有数据
                else res.isallresults = 1;       //不是所有数据
                res.records = ToDataTableByPage(out current, drs, page, size);
                res.recordslenght = current;        //当前记录条数
                if (page > s_page)
                {
                    res.total = 0;     //返回的记录数
                }
                else
                {
                    res.total = drs.Length;     //返回的记录数
                }
                res.page = page;
                res.size = size;
                Result = new ResponseAppResult(ResState.Success, "操作成功", res);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppSeachForPages调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }

        #endregion

        #region 用户车辆在线状态统计
        /// <summary>
        /// 用户车辆在线状态统计
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppCarOnlineCount(Dictionary<string, string> inparams)
        {

            ResponseAppResult Result = null;
            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];

                string uid = inparams["uid"];
                DataTable dt = GetUserCars(sysflag, uid);
                dt.Columns.Add("online");
                DataColumn dc = new DataColumn("stno", typeof(string));
                dt.Columns.Add(dc);

                List<long> cids = new List<long>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["cid"] == null || dt.Rows[i]["cid"].ToString() == "") continue;
                    cids.Add(long.Parse(dt.Rows[i]["cid"].ToString()));
                    dt.Rows[i]["stno"] = dt.Rows[i]["tno"].ToString();
                }
                dt.Columns.Remove("tno");
                dc.ColumnName = "tno";
                //monitor mon = new monitor();
                //WebGIS.RealtimeDataServer.CarRealData[] realdata = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), cids.ToArray());
                //Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                //foreach (WebGIS.RealtimeDataServer.CarRealData w in realdata)
                //{
                //    carOnlineStatus.Add(w.Carid, w.OnlineStatus);
                //}

                List<CarsStatus> realdata = RDSConfig.GetCarOnlineStatus(sysflag, cids.ToArray());
                Dictionary<long, int> carOnlineStatus = new Dictionary<long, int>();
                foreach (CarsStatus carStatus in realdata)
                {
                    carOnlineStatus.Add(carStatus.cid, carStatus.status);
                }

                int online = 0, offline = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["cid"] == null || dr["cid"].ToString() == "") continue;
                    long cid = long.Parse(dr["cid"].ToString());
                    if (carOnlineStatus.ContainsKey(cid))
                    {
                        dr["online"] = carOnlineStatus[cid];
                        if (carOnlineStatus[cid] == 1)
                        {
                            online++;
                        }
                        else
                        {
                            offline++;
                        }
                    }
                    else
                    {
                        dr["online"] = 2;
                        offline++;
                    }
                }
                DataRow[] drs = dt.Select("");
                usercarsOnLine res = new usercarsOnLine();
                //res.records = ToDataTable(drs);
                res.total = dt.Rows.Count;
                res.offline = offline;
                res.online = online;

                Result = new ResponseAppResult(ResState.Success, "操作成功", res);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppCarOnlineCount调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        #endregion

        #region  复制表结构到新的DataTable中
        /// <summary>
        /// 复制表结构到新的DataTable中
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private DataTable ToDataTable(DataRow[] rows, int top = 0)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            int count = top > 0 && top < rows.Length ? top : rows.Length;
            for (int i = 0; i < count; i++)
                tmp.ImportRow(rows[i]); // 将DataRow添加到DataTable中
            return tmp;
        }


        /// <summary>
        /// 数据分页
        /// </summary>
        /// <param name="rows">数据库中读取的记录行</param>
        /// <param name="top">总记录条数</param>
        /// <param name="page">当前页数</param>
        /// <param name="size">每页显示的记录数</param>
        /// <returns></returns>
        private DataTable ToDataTableByPage(out int current, DataRow[] rows, int page = 0, int size = 0)
        {
            current = 0;
            int currentCount = (page - 1) * size;       //显示数据从第多少条记录开始

            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();
            //int count = top > 0 && top < rows.Length ? top : rows.Length;
            int count = rows.Length;

            int s_page = count % size == 0 ? count / size : (count / size) + 1; //一共有多少页
            if (page > s_page) return null;     //传入的页码大于本身页码数
            //每页显示的记录条数小于总记录数
            if (size < count)
            {
                current = 0;
                if (currentCount + size > count)   //当前记录数加每页显示的条数大于总记录数
                {
                    for (int i = currentCount; i < count; i++)
                    {
                        tmp.ImportRow(rows[i]); // 将DataRow添加到DataTable中
                        current++;
                    }
                }
                else
                {
                    for (int i = currentCount; i < currentCount + size; i++)
                    {
                        tmp.ImportRow(rows[i]); // 将DataRow添加到DataTable中
                        current++;
                    }
                }
            }
            else//每页显示的记录条数大于总记录数
            {
                current = 0;
                for (int i = currentCount; i < count; i++)
                {
                    tmp.ImportRow(rows[i]); // 将DataRow添加到DataTable中 }
                    current++;
                }
            }
            return tmp;
        }
        #endregion



        #region 车辆分组信息增加
        /// <summary>
        /// 车辆分组信息增加
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="username"></param>
        /// <param name="mobilenum"></param>
        /// <param name="pwd"></param>
        /// <param name="source"></param>
        private void GroupCarInsert(string sysflag, int gid, int uid, int cid)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@GID", gid),
                                        new SqlParameter("@UID", uid),
                                        new SqlParameter("@CID", cid)};
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GroupCar_Insert"), Parameters, true);
        }
        #endregion




        public ResponseAppResult AppUnwrapUserCars(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];
                string cid = inparams["cid"];
                string uid = inparams["uid"];
                int res = DeleteUserCarBind(sysflag, cid, uid);
                if (res == 1)
                {
                    Result = new ResponseAppResult(ResState.Success, "操作成功", null);
                }
                if (res == 0)
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "操作失败,该车辆属于管理组，个人用户无法删除，请联系管理员。", null);
                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AlarmCarBind调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppGetCarLastTrack(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }


            try
            {
                string sysflag = inparams["sysflag"];

                long cid = long.Parse(inparams["cid"]);
                monitor mon = new monitor();
                WebGIS.RealtimeDataServer.CarRealData[] RealData = mon.CarRealDataByCids(WebProc.GetAppSysflagKey(sysflag), new long[] { cid });
                if (RealData.Length > 0)
                {
                    appcarrealtimetrack wd = new appcarrealtimetrack();
                    wd.altitudemeters = RealData[0].AltitudeMeters.ToString();
                    wd.cid = RealData[0].Carid.ToString();
                    wd.heading = RealData[0].Heading.ToString();
                    wd.lat = RealData[0].Lati.ToString();
                    wd.longt = RealData[0].Long.ToString();
                    wd.onlinestatus = RealData[0].OnlineStatus.ToString();
                    wd.speed = RealData[0].Speed.ToString();
                    wd.summiles = RealData[0].SumMiles.ToString();
                    wd.tdatetime = RealData[0].TDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    wd.tno = RealData[0].TNO.ToString();
                    wd.alarmstr = RealData[0].AlarmStr.ToString();
                    wd.statusstr = RealData[0].StatusStr.ToString();
                    Result = new ResponseAppResult(ResState.Success, "操作成功", wd);
                }

                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "无轨迹信息", null);
                }



            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppGetCarLastTrack调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }
        public ResponseAppResult AppGetCarTrack(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;

            if (!inparams.Keys.Contains("cid") || inparams["cid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少cid或cid为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("st") || inparams["st"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少st或st为空！", null);
                return Result;
            }
            if (!inparams.Keys.Contains("et") || inparams["et"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少et或et！", null);
                return Result;
            }

            try
            {
                string sysflag = inparams["sysflag"];

                long cid = long.Parse(inparams["cid"]);
                string st = inparams["st"];
                string et = inparams["et"];
                string carno = GetCarCarno(WebProc.GetAppSysflagKey(sysflag), cid.ToString());
                Track track = new Track();

                DataTable dt = track.getTracks(WebProc.GetAppSysflagKey(sysflag), cid.ToString(), carno, st, et, 120, 4, "0");

                ResList res = new ResList();
                res.isallresults = 1;
                res.records = dt;
                res.total = dt.Rows.Count;
                res.size = 0;
                res.page = 0;

                Result = new ResponseAppResult(ResState.Success, "操作成功", res);





            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppGetCarTrack调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }

    }
    public class usermodel
    {
        public int uid;
        public string uname;
        public string pwd;
        public string mobilenum;
    }

    public class userlogin
    {
        public string uid;
        public string uname;
        public List<string> roles = new List<string>();
        public List<string> rolenames = new List<string>();
        public List<string> rights = new List<string>();
    }
    public class smscodemodel
    {
        public string smscode;
        public string dtsendtime;

    }
    public class appcarrealtimetrack
    {
        public string cid;
        public string tno;
        public string longt;
        public string lat;
        public string heading;
        public string speed;
        public string tdatetime;
        public string summiles;
        public string altitudemeters;
        public string onlinestatus;

        public string statusstr;
        public string alarmstr;


    }


    public class usercarsOnLine
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int total;
        public int online;
        public int offline;
    }
    public class usercarslist
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int total;
        /// <summary>
        /// 是否返回全部结果
        /// </summary>
        public int isallresults;
        public int online;
        public int offline;
        public int recordslenght;
        /// <summary>
        /// 数组/DataTable/List等
        /// </summary>
        public object records;
    }


    public class usercarslistPage
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int total;
        /// <summary>
        /// 是否返回全部结果
        /// </summary>
        public int isallresults;
        public int recordslenght;

        public int page;
        public int size;
        /// <summary>
        /// 数组/DataTable/List等
        /// </summary>
        public object records;
    }
}