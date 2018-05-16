using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CommLibrary;
using System.Data.SqlClient;
using SysService;


namespace WebGIS.Service.App
{
    public class AppvdProess
    {

        public userinfos appvduser = null;   //用户实体信息
        public appvduserinfo appvdreturn = new appvduserinfo();        //用户登录返回的信息实体类

        #region APP用户注册
        /// <summary>
        /// APP用户注册
        /// </summary>
        /// <param name="inparams">JSON格式的数据参数</param>
        /// <returns></returns>
        public ResponseAppResult AppvdUserRegister(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;
            userinfos appvdInfo = new userinfos();
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
            if (!inparams.Keys.Contains("smscode") || inparams["smscode"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少smscode或smscode为空！", null);
                return Result;
            }

            try
            {
                //必填项
                appvdInfo.username = inparams["username"].Trim();
                appvdInfo.pwd = inparams["pwd"].Trim();
                appvdInfo.mobilenum = inparams["mobilenum"].Trim();
                appvdInfo.source = "1";     //默认为手机APP用户注册类型
                string sysflag = inparams["sysflag"];
                //非必填项
                appvdInfo.memo = inparams.ContainsKey("memo") ? inparams["memo"] : "";
                appvdInfo.email = inparams.ContainsKey("email") ? inparams["email"] : "";
                appvdInfo.fullname = inparams.ContainsKey("fullname") ? inparams["fullname"] : "";
                appvdInfo.sex = inparams.ContainsKey("sex") ? inparams["sex"] : "";
                appvdInfo.phoneno = inparams.ContainsKey("phoneno") ? inparams["phoneno"] : "";
                appvdInfo.id_card_no = inparams.ContainsKey("id_card_no") ? inparams["id_card_no"] : "";
                appvdInfo.nativeplace = inparams.ContainsKey("nativeplace") ? inparams["nativeplace"] : "";
                appvdInfo.adress = inparams.ContainsKey("adress") ? inparams["adress"] : "";

                string smscode = inparams["smscode"];

                if (MessageCodeManager.CheckSms(smscode, appvdInfo.mobilenum))            //判断验证码是否存在于公共静态变量中
                {

                    int s = CheckUserNameMobileNum(sysflag, appvdInfo.username, appvdInfo.mobilenum);//验证用户名和手机号码是否重复
                    if (s == 0)//不存在相同的用户名和手机号码
                    {
                        UserRegister(appvdInfo, sysflag);
                        string dbusername = "";
                        int uid = GetUIDUserCheck(sysflag, appvdInfo.username, appvdInfo.pwd, out dbusername);

                        if (uid > 0)
                        {
                            appvduserreg ul = new appvduserreg();
                            ul.uid = uid.ToString();
                            ul.username = appvdInfo.username;
                            ul.pwd = appvdInfo.pwd;
                            Result = new ResponseAppResult(ResState.Success, "操作成功", ul);

                        }
                        else
                        {
                            Result = new ResponseAppResult(ResState.OperationFailed, "注册失败", null);
                            //return Result;
                        }
                        MessageCodeManager.DelSmsByCode(smscode);       //当前验证码存在于静态变量中，将其删除（避免生成重复的验证码）
                    }
                    else if (s == 1)  //用户名存在
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "手机号码已经注册！", null);
                        return Result;
                    }
                    else if (s == 2)//手机号码存在
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "用户名已存在！", null);
                        return Result;
                    }
                    else if (s == 3)//用户已经注册
                    {
                        Result = new ResponseAppResult(ResState.OperationFailed, "用户已经注册！", null);
                        return Result;
                    }
                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "验证码错误！", null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppvdUserRegister调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;

        }


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="appvdInfo">用户实体对象</param>
        private void UserRegister(userinfos appvdInfo, string sysflag)
        {

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@username", appvdInfo.username),
                                        new SqlParameter("@mobilenum", appvdInfo.mobilenum),
                                        new SqlParameter("@pwd", appvdInfo.pwd),
                                        new SqlParameter("@source", appvdInfo.source),


                                        new SqlParameter("@email", appvdInfo.email),
                                        new SqlParameter("@memo", appvdInfo.memo),
                                        new SqlParameter("@fullname", appvdInfo.fullname),
                                        new SqlParameter("@sex", appvdInfo.sex),
                                        new SqlParameter("@phoneno",appvdInfo.phoneno),
                                        new SqlParameter("@id_card_no", appvdInfo.id_card_no),
                                        new SqlParameter("@nativeplace", appvdInfo.nativeplace),
                                        new SqlParameter("@adress", appvdInfo.adress)
                                        };
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_Appvd_UserRegister"), Parameters, true);
        }
        #endregion

        #region 验证用户名和手机号码
        /// <summary>
        /// 验证用户名和手机号码
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="username"></param>
        /// <param name="mobilenum"></param>
        /// <returns></returns>
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
        #endregion

        #region 根据用户名和手机号码获取用户编号
        /// <summary>
        /// 根据用户名和手机号码获取用户编号
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="usernameormobilenum"></param>
        /// <param name="pwd"></param>
        /// <param name="username"></param>
        /// <returns></returns>
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
        #endregion

        #region 根据用户名和手机号码获取用户实体信息
        /// <summary>
        /// 根据用户名和手机号码获取用户实体信息
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="usernameormobilenum"></param>
        /// <param name="pwd"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        private void GetUAppIDUserCheck(string sysflag, string usernameormobilenum, string pwd)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@nameormobile", usernameormobilenum), new SqlParameter("@pwd", pwd) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_Appvd_GetUserByNameORMobilenum"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                appvduser = new userinfos();
                string dbpwd = dt.Rows[0]["PASSWORD"].ToString();
                appvduser.uid = dt.Rows[0]["UID"].ToString();
                appvduser.username = dt.Rows[0]["NAME"].ToString().Trim();
                appvduser.pwd = dt.Rows[0]["PASSWORD"].ToString().Trim();
                appvduser.mobilenum = dt.Rows[0]["MobileNum"].ToString().Trim();
                appvduser.source = dt.Rows[0]["SourceId"].ToString();
                appvduser.memo = dt.Rows[0]["MEMO"].ToString();
                appvduser.email = dt.Rows[0]["Email"].ToString();
                appvduser.fullname = dt.Rows[0]["FullName"].ToString();
                appvduser.sex = dt.Rows[0]["SEX"].ToString();
                appvduser.phoneno = dt.Rows[0]["PHONENO"].ToString();
                appvduser.id_card_no = dt.Rows[0]["ID_CARD_NO"].ToString();
                appvduser.adress = dt.Rows[0]["ADDRESS"].ToString();
                appvduser.nativeplace = dt.Rows[0]["NATIVEPLACE"].ToString();

                appvdreturn.userinfo = appvduser;
            }
        }

        #endregion

        #region APP用户登录
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

            try
            {
                string sysflag = inparams["sysflag"];
                string pwd = inparams["pwd"];
                string username = inparams["username"];
                string logintype = inparams.ContainsKey("type") ? inparams["type"] : inparams["logintype"];
                GetUAppIDUserCheck(sysflag, username, pwd);
                if (appvduser != null)
                {
                    int uid = Convert.ToInt32(appvduser.uid);
                    GetUserRolesAndRightsAndCars(sysflag, uid, logintype);
                    Result = new ResponseAppResult(ResState.Success, "操作成功", appvdreturn);
                    loginlog(sysflag, uid.ToString(), username, "1", "", logintype.Trim());
                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "用户名或密码错误！", null);
                    return Result;
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppUserLogin调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }
        private void loginlog(string sysflag, string uid, string uname, string usertype, string ip, string sysName)
        {
            try
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
            catch (Exception ex)
            {
                LogHelper.WriteError("loginlog调用异常", ex);
            }
        }
        #endregion

        #region 获取用户角色、用户车辆列表、用户APP功能模块权限列表
        private void GetUserRolesAndRightsAndCars(string sysflag, int uid, string logintype)
        {

            appvduser.uid = uid.ToString();
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid), new SqlParameter("@logintype", logintype) };
            DataSet ds = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_Appvd_GetUserRolesAndRights"), Parameters);
            DataTable dtrole = ds.Tables[0];
            DataTable dtrights = ds.Tables[1];
            foreach (DataRow dr in dtrole.Rows)     //获取角色列表
            {
                roleinfo rolei = new roleinfo();
                rolei.roleid = dr["RID"].ToString();
                rolei.rolename = dr["NAME"].ToString();
                appvdreturn.rolelist.Add(rolei);
            }
            foreach (DataRow dr in dtrights.Rows)   //获取APP功能模块权限列表
            {
                apkinfo apk = new apkinfo();
                apk.apkid = dr["APK_ID"].ToString();
                apk.apkname = dr["APK_NAME"].ToString();
                apk.apkpath = dr["APK_PATH"].ToString();
                apk.apkversion = dr["APK_VERSION"].ToString();
                apk.publishdate = dr["PUBLISH_DATE"].ToString();
                apk.isupdate = dr["IS_UPDATE"].ToString();
                appvdreturn.apkright.Add(apk);
            }

            SqlParameter[] Parameters_car = { new SqlParameter("@uid", uid) };
            AppProess ap = new AppProess();
            DataTable dt_carlist = ap.GetUserCrsbyOnline(sysflag, appvduser.uid);
            foreach (DataRow dr in dt_carlist.Rows)   //获取用户车辆数据权限列表
            {
                carsinfo cars = new carsinfo();
                cars.cid = dr["cid"].ToString();
                cars.tno = dr["tno"].ToString();
                cars.carno = dr["carno"].ToString().Trim();
                cars.sim = dr["sim"].ToString();
                cars.color = dr["color"].ToString();
                cars.cartype = dr["cartype"].ToString();
                cars.online = dr["online"].ToString();
                appvdreturn.carlist.Add(cars);
            }
        }
        #endregion

        #region APP用户信息修改
        /// <summary>
        /// APP用户信息修改
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseAppResult AppvdUserUpdate(Dictionary<string, string> inparams)
        {
            ResponseAppResult Result = null;
            userinfos appvdInfo = new userinfos();
            if (!inparams.Keys.Contains("uid") || inparams["uid"] == "")
            {
                Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少uid或uid为空！", null);
                return Result;
            }
            //if (!inparams.Keys.Contains("mobilenum") || inparams["mobilenum"] == "")
            //{
            //    Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少mobilenum或mobilenum为空！", null);
            //    return Result;
            //}

            try
            {
                string sysflag = inparams["sysflag"];
                appvdInfo.uid = inparams["uid"];


                userinfos uinfo = GetUAppIDUserCheckByUpt(inparams["sysflag"], inparams["uid"]);

                //必填项
                string mobilenum_new = inparams.ContainsKey("mobilenum") && inparams["mobilenum"].Trim() != "" ? inparams["mobilenum"] : uinfo.mobilenum;
                string username_new = inparams.ContainsKey("username") && inparams["username"].Trim() != "" ? inparams["username"] : uinfo.username;
                appvdInfo.mobilenum = mobilenum_new;
                appvdInfo.username = username_new;



                if (uinfo != null)
                {

                    //非必填项 如果不包含指定的参数值，那么读取该实体的数据信息，并赋值
                    appvdInfo.memo = inparams.ContainsKey("memo") ? inparams["memo"] : uinfo.memo;

                    appvdInfo.pwd = inparams.ContainsKey("pwd") ? inparams["pwd"] : uinfo.pwd;
                    appvdInfo.email = inparams.ContainsKey("email") ? inparams["email"] : uinfo.email;
                    appvdInfo.fullname = inparams.ContainsKey("fullname") ? inparams["fullname"] : uinfo.email;
                    appvdInfo.sex = inparams.ContainsKey("sex") ? inparams["sex"] : uinfo.sex;
                    appvdInfo.phoneno = inparams.ContainsKey("phoneno") ? inparams["phoneno"] : uinfo.phoneno;
                    appvdInfo.id_card_no = inparams.ContainsKey("id_card_no") ? inparams["id_card_no"] : uinfo.id_card_no;
                    appvdInfo.nativeplace = inparams.ContainsKey("nativeplace") ? inparams["nativeplace"] : uinfo.nativeplace;
                    appvdInfo.adress = inparams.ContainsKey("adress") ? inparams["adress"] : uinfo.adress;


                    if (mobilenum_new != uinfo.mobilenum || username_new != uinfo.username)   //判断是否要修改手机号码;
                    {

                        int s = CheckUserNameMobileNum(sysflag, username_new, mobilenum_new);//如果修改，验证当前新手机号码是否与数据库中存在的手机号码重复

                        if (s == 0)
                        {
                            UserInfoUpdate(appvdInfo, sysflag);
                            Result = new ResponseAppResult(ResState.Success, "操作成功！", null);

                            return Result;
                        }
                        else //表示新手机号码和用户名都存在
                        {
                            Result = new ResponseAppResult(ResState.OperationFailed, "手机号码,或用户名已存在！", null);
                        }
                    }
                    else
                    {

                        UserInfoUpdate(appvdInfo, sysflag);
                        Result = new ResponseAppResult(ResState.Success, "操作成功！", null);
                    }
                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "用户不存在", null);

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppvdUserUpdate调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }
        #endregion

        #region 根据用户ID获取用户实体信息
        /// <summary>
        /// 根据用户ID获取用户实体信息
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="usernameormobilenum"></param>
        /// <param name="pwd"></param>
        /// <param name="username"></param>
        /// <returns>返回用户对象实体</returns>
        private userinfos GetUAppIDUserCheckByUpt(string sysflag, string uid)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uid) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_App_GetUserInfo"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                userinfos appvduser = new userinfos();
                appvduser.uid = dt.Rows[0]["UID"].ToString().Trim();
                appvduser.username = dt.Rows[0]["NAME"].ToString().Trim();
                appvduser.pwd = dt.Rows[0]["PASSWORD"].ToString().Trim();
                appvduser.mobilenum = dt.Rows[0]["MobileNum"].ToString().Trim();


                appvduser.memo = dt.Rows[0]["MEMO"].ToString();
                appvduser.email = dt.Rows[0]["Email"].ToString();
                appvduser.fullname = dt.Rows[0]["FullName"].ToString();
                appvduser.sex = dt.Rows[0]["Sex"].ToString();
                appvduser.phoneno = dt.Rows[0]["PHONENO"].ToString();
                appvduser.id_card_no = dt.Rows[0]["ID_CARD_NO"].ToString();
                appvduser.nativeplace = dt.Rows[0]["NATIVEPLACE"].ToString();
                appvduser.adress = dt.Rows[0]["ADDRESS"].ToString();

                return appvduser;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 用户信息修改
        /// <summary>
        /// 用户信息修改
        /// </summary>
        /// <param name="uinfo"></param>
        public void UserInfoUpdate(userinfos uinfo, string sysflag)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@uid", uinfo.uid),
                                        new SqlParameter("@username",uinfo.username),
                                        new SqlParameter("@pwd",uinfo.pwd),
                                        new SqlParameter("@mobilenum",uinfo.mobilenum),
                                        new SqlParameter("@memo", uinfo.memo),
                                        new SqlParameter("@email", uinfo.email),
                                        new SqlParameter("@fullname", uinfo.fullname),
                                        new SqlParameter("@sex", uinfo.sex),
                                        new SqlParameter("@phoneno", uinfo.phoneno),
                                        new SqlParameter("@id_card_no", uinfo.id_card_no),
                                        new SqlParameter("@adress", uinfo.adress),
                                        new SqlParameter("@nativeplace", uinfo.nativeplace),
                                        
                                        };
            csh.ExecuteSPNoQuery(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_Appvd_UserUpdate"), Parameters, true);
        }
        #endregion


        #region APP获取指定的APK版本
        /// <summary>
        /// APP获取指定的APK版本
        /// </summary>
        /// <param name="inparams">JSON格式的数据参数</param>
        /// <returns></returns>
        public ResponseAppResult AppvdGetApkVersion(Dictionary<string, string> inparams)
        {

            ResponseAppResult Result = null;
            userinfos appvdInfo = new userinfos();
            try
            {
                if (!inparams.Keys.Contains("apkid") || inparams["apkid"] == "")
                {
                    Result = new ResponseAppResult(ResState.ParamsImperfect, "缺少apkid或apkid为空！", null);
                    return Result;
                }
                apkinfo info = GetApkVersionByID(inparams["sysflag"], inparams["apkid"]);
                if (info != null)
                {
                    Result = new ResponseAppResult(ResState.Success, "操作成功", info);
                }
                else
                {
                    Result = new ResponseAppResult(ResState.OperationFailed, "当前APK编号不存在！", null);
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("AppvdGetApkVersion调用异常", ex);
                Result = new ResponseAppResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }
        #endregion

        #region 根据APK编号获取信息
        /// <summary>
        /// 根据APK编号获取信息
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="apkId"></param>
        /// <returns></returns>
        public apkinfo GetApkVersionByID(string sysflag, string apkId)
        {
            apkinfo info = new apkinfo();

            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { new SqlParameter("@apkid", apkId) };
            DataTable dt = csh.FillDataSet(WebProc.GetAppSysflagKey(sysflag), WebProc.Proc("QWGProc_Appvd_GetAPKById"), Parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {

                info.apkid = dt.Rows[0]["APK_ID"].ToString().Trim();
                info.apkname = dt.Rows[0]["APK_NAME"].ToString().Trim();
                info.apkpath = dt.Rows[0]["APK_PATH"].ToString().Trim();
                info.apkversion = dt.Rows[0]["APK_VERSION"].ToString().Trim();
                info.publishdate = dt.Rows[0]["PUBLISH_DATE"].ToString().Trim();
                info.isupdate = dt.Rows[0]["IS_UPDATE"].ToString().Trim();
                return info;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    #region 用户注册 返回的信息
    public class appvduserreg
    {
        public string uid;
        public string username;
        public string pwd;
    }
    #endregion

    #region 用户登录 返回的信息
    public class appvduserinfo
    {
        public userinfos userinfo = new userinfos();
        public List<roleinfo> rolelist = new List<roleinfo>();//用户角色列表
        public List<carsinfo> carlist = new List<carsinfo>();//车辆列表
        public List<apkinfo> apkright = new List<apkinfo>();//用户APP功能模块权限列表
    }
    #endregion

    #region 用户车辆信息列表
    public class carsinfo
    {
        public string cid;
        public string tno;
        public string carno;
        public string sim;
        public string color;
        public string cartype;
        public string online;
    }
    #endregion

    #region 手机端版本信息类
    public class apkinfo
    {
        public string apkid;
        public string apkname;
        public string apkpath;
        public string apkversion;
        public string publishdate;
        public string isupdate;

    }
    #endregion

    #region 角色信息类
    public class roleinfo
    {
        public string roleid;
        public string rolename;
    }
    #endregion

    #region 用户信息类
    public class userinfos
    {
        public string uid;
        public string username;
        public string pwd;
        public string mobilenum;
        public string source;
        public string email;
        public string memo;
        public string fullname;
        public string sex;
        public string phoneno;
        public string id_card_no;
        public string adress;
        public string nativeplace;
        //public string sysflag;
    }
    #endregion

}