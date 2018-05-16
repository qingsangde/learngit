using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Data;
using System.Data.SqlClient;




namespace WebGIS
{
    public class SessionManager
    {
         
        /// <summary>
        /// 调用全局用户会话服务标志位
        /// </summary>
        public static bool GSRunFlag = false;
        /// <summary>
        /// 全局会话服务远程调用对象
        /// </summary>
        //static WebGIS.GlobalSessionService.USService GlobalSession = new GlobalSessionService.USService();
        static WebGIS.GlobalSessionService.GSSWebService GlobalSession = new GlobalSessionService.GSSWebService();
        /// <summary>
        /// 默认会话超时时间
        /// </summary>
        public static int Timeout = 1800;
        /// <summary>
        /// 会话存储Hash表 Hashtable 要允许并发读但只能一个线程写
        /// </summary>
        private static Hashtable userhash = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 网站服务器标示，目前使用本机以太网IPv4地址做标示。
        /// </summary>
        private static string WebIpFlag = IPv4.GetEthernetIPv4();
        static Thread th = null;
        private static DataTable rightsid = new DataTable();

        /// <summary>
        /// 启动会话定时维护，用于处理超时用户，以及同步数据至 全局会话服务
        /// </summary>
        public static void StartManager()
        {
            try
            {
                string basepath = HttpRuntime.AppDomainAppPath.ToString();
                DataSet ds = new DataSet();
                ds.ReadXml(basepath + "RightSid.xml");
                rightsid = ds.Tables[0];
                if (th == null)
                {
                    th = new Thread(TimeoutManager);
                    th.IsBackground = true;
                    th.Start();

                }
            }
            catch (Exception ex)
            {

                LogHelper.WriteError("StartManager调用异常", ex);
            }
        }
        static ComSqlHelper csh = new ComSqlHelper();
        /// <summary>
        /// 用户退出系统时记录日志到数据库
        /// </summary>
        /// <param name="sm"></param>
        private static void logoutForDb(SessionModel sm)
        {
            string key = sm.sysflag;
            string uid = sm.uid.ToString();
           
            string ip = sm.userIP;
            string usertype = "";
            if (sm.onecaruser) //单车用户
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
        }
        static WebGIS.RealtimeDataServer.WCFServiceClient wc = new WebGIS.RealtimeDataServer.WCFServiceClient();
        /// <summary>
        /// 用户会话状态维护线程
        /// 同时对全局用户状态和实时信息服务状态进行检查
        /// </summary>         
        private static void TimeoutManager()
        {
            LogHelper.WriteInfo("启动会话维护TimeoutManager服务。");

            while (true)
            {
                string to = System.Configuration.ConfigurationManager.AppSettings["Timeout"].ToString();
                int.TryParse(to, out Timeout);
                
                //判断全局会话服务是否正常
                try
                {
                    string value = GlobalSession.GetData("1");
                    if (value == "1")
                    {
                        GSRunFlag = true;
                    }
                    else
                    {
                        GSRunFlag = false;
                    }
                }
                catch (Exception ex)
                {
                    GSRunFlag = false;
                    LogHelper.WriteError("全局会话服务调用异常", ex);
                }
                try
                {
                    //超时会话集合
                    List<string> timeouttokens = new List<string>();
                    //正常会话集合
                    List<string> tokens = new List<string>();
                    foreach (string token in userhash.Keys)
                    {
                        SessionModel sm = (SessionModel)userhash[token];
                        if ((DateTime.Now - sm.lastoper).TotalSeconds < Timeout)
                        {
                            tokens.Add(token);
                        }
                        else
                        {
                            timeouttokens.Add(token);
                        }
                    }
                    //
                    if (tokens.Count > 0)
                    {
                        string tks = "";
                        List<GlobalSessionService.SessionModel> gsms = new List<GlobalSessionService.SessionModel>();
                        foreach (string token in tokens)
                        {
                            tks += token + ",";
                            //构建全局会话管理数据模型
                            GlobalSessionService.SessionModel gsm = new GlobalSessionService.SessionModel();
                            SessionModelCopy((SessionModel)userhash[token], out  gsm);
                            gsms.Add((GlobalSessionService.SessionModel)gsm);
                        }
                        //判断全局会话服务是否需要同步数据
                        if (GSRunFlag && GlobalSession.IsActive(tks,  WebIpFlag) == "false")
                        {
                            //同步所有数据至全局会话服务
                            GlobalSession.SyncSessions(gsms.ToArray());
                            //LogHelper.WriteInfo("同步所有数据至全局会话服务:" + tks);
                        }
                    }
                    if (timeouttokens.Count > 0)
                    {
                        string timeouttks = "";
                        lock (userhash.SyncRoot)
                        {
                            foreach (string token in timeouttokens)
                            {

                                timeouttks += token + ",";
                                logoutForDb((SessionModel)userhash[token]);
                                userhash.Remove(token);

                            }
                        }
                        if (GSRunFlag)
                        {
                            //清除全局会话
                            GlobalSession.CloseSession(timeouttks, WebIpFlag);
                            //LogHelper.WriteInfo("清除全局会话服务:" + timeouttks);
                        }
                    }
                }
                catch (Exception ex)
                {

                    LogHelper.WriteError("TimeoutManager执行异常", ex);
                }
                Thread.Sleep(10 * 1000);
            }
        }
        /// <summary>
        /// 创建用户会话接口
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static string CreateSession(string sysflag,string  username, int uid, int[] rid, int group, int[] rights, DataTable cars,string userIP, bool onecaruser = false)
        {
            string token = Guid.NewGuid().ToString();
            SessionModel sm = new SessionModel();

            sm.uid = uid;
            sm.username = username;
            sm.rid = rid;
            sm.group = group;
            sm.token = token;
            sm.aids = rights;
            sm.cars = cars;
            sm.cars.TableName = "cars";
            sm.logintime = DateTime.Now;
            sm.lastoper = DateTime.Now;
            sm.sysflag = sysflag;
            sm.webserverflag = WebIpFlag;
            sm.onecaruser = onecaruser;
            sm.userIP = userIP;
            //添加到本地hash表
            lock (userhash.SyncRoot)
            {
                userhash.Add(token, sm);
            }
            try
            {
                //构建全局会话管理数据模型
                GlobalSessionService.SessionModel gsm = new GlobalSessionService.SessionModel();
                SessionModelCopy(sm, out  gsm);
                //添加数据到全局会话管理
                if (GSRunFlag)
                {
                    GlobalSession.SyncSession((GlobalSessionService.SessionModel)gsm);
                    //LogHelper.WriteInfo("新增用户数据至全局会话服务:" + token);
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteError("CreateSession执行异常", ex);
            }
            return token;
        }
        /// <summary>
        /// 关闭会话
        /// </summary>
        /// <param name="token"></param>
        public static void CloseSession(string token)
        {
            if (userhash.ContainsKey(token))
            {
                try
                {
                    SessionModel sm = (SessionModel)userhash[token];
                    //清除全局会话
                    if (GSRunFlag)
                    {
                        GlobalSession.CloseSession(token, sm.webserverflag);
                        //LogHelper.WriteInfo("移除用户数据至全局会话服务:" + token);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError("CloseSession移除用户数据至全局会话服务执行异常", ex);
                }
                lock (userhash.SyncRoot)
                {
                    userhash.Remove(token);
                }
            }

        }
        private static bool GetGlobalSessionToLocal(string token)
        {
            try
            {
                GlobalSessionService.SessionModel gsm = GlobalSession.GetSession(token, WebIpFlag);
                if (gsm != null)
                {
                    SessionModel sm = new SessionModel();
                    SessionModelCopy(gsm, out sm);
                    lock (userhash.SyncRoot)
                    {
                        userhash.Remove(token);
                        userhash.Add(token, sm);
                    }
                    //LogHelper.WriteInfo("全局会话服务用户数据到本地:" + token);
                    return true;
                }
                else
                {
                    //LogHelper.WriteInfo("全局会话无返回数据:" + token);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("GetGlobalSessionToLocal执行异常", ex);
                return false;
            }
        }
        /// <summary>
        /// 获取用户会话信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SessionModel GetSession(string token)
        {
            if (userhash.ContainsKey(token))
            {
                SessionModel sm = (SessionModel)userhash[token];
                return sm;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 判断会话状态是否活跃,如果活跃则重置最后操作时间
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool IsActiveAndReset(string token)
        {
            if (userhash.ContainsKey(token))
            {
                SessionModel sm = (SessionModel)userhash[token];
                if ((DateTime.Now - sm.lastoper).TotalSeconds < Timeout)
                {
                    sm.lastoper = DateTime.Now;
                    //sm.lastopersid = rid.ToString();
                    lock (userhash.SyncRoot)
                    {
                        userhash[token] = sm;
                    }
                    return true;
                }
                else
                {

                    logoutForDb((SessionModel)userhash[token]);
                    // userhash.Remove(token);
                    CloseSession(token);

                    return false;
                }
            }
            else
            {
                if (GSRunFlag && GetGlobalSessionToLocal(token))
                {
                    if (userhash.ContainsKey(token))
                    {
                        return IsActiveAndReset(token);
                    }
                    else
                    {
                        return false;
                    }
                }
                else

                    return false;
            }
        }
        public static bool IsActive(string token)
        {
            if (userhash.ContainsKey(token))
            {
                SessionModel sm = (SessionModel)userhash[token];
                if ((DateTime.Now - sm.lastoper).TotalSeconds < Timeout)
                {
                    return true;
                }
                else
                {
                    logoutForDb(sm);
                    CloseSession(token);
                    //userhash.Remove(token);
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// 操作权限认证
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static bool Authentication(string token, string sid)
        {
            //测试时使用
            return true;

            if (userhash.ContainsKey(token))
            {
                SessionModel sm = (SessionModel)userhash[token];
                if ((DateTime.Now - sm.lastoper).TotalSeconds < Timeout)
                {
                    int rid = 0;
                    DataRow[] drs = rightsid.Select("SIDs like '%" + sid + "%'");
                    foreach (DataRow dr in drs)
                    {
                        rid = int.Parse(dr["A_ID"].ToString());
                        if (sm.aids.Contains(rid))
                        {
                            sm.lastoper = DateTime.Now;
                            sm.lastopersid = rid.ToString();
                            lock (userhash.SyncRoot)
                            {
                                userhash[token] = sm;
                            }
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (GSRunFlag && GetGlobalSessionToLocal(token))
                {
                    return Authentication(token, sid);
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// 拷贝对象，将源对象中与目标对象中名称相同的属性拷贝过去
        /// </summary>
        /// <param name="origin">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="targetType">目标对象类型</param>
        protected static void SessionModelCopy(SessionModel origin, out GlobalSessionService.SessionModel target)
        {
            if (origin == null)
            {
                target = null;
                return;
            }
            target = new GlobalSessionService.SessionModel();
            target.username = origin.username;
            target.aids = origin.aids;
            target.cars = origin.cars;
            target.group = origin.group;
            target.lastoper = origin.lastoper;
            target.lastopersid = origin.lastopersid;
            target.logintime = origin.logintime;
            target.onecaruser = origin.onecaruser;
            target.rid = origin.rid;
            target.sysflag = origin.sysflag;
            target.token = origin.token;
            target.uid = origin.uid;
            target.userIP = origin.userIP;
            target.webserverflag = origin.webserverflag;

        }
        /// <summary>
        /// 拷贝对象，将源对象中与目标对象中名称相同的属性拷贝过去
        /// </summary>
        /// <param name="origin">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="targetType">目标对象类型</param>
        protected static void SessionModelCopy(GlobalSessionService.SessionModel origin, out SessionModel target)
        {
            if (origin == null)
            {
                target = null;
                return;
            }
            target = new SessionModel();
            target.username = origin.username;
            target.aids = origin.aids;
            target.cars = origin.cars;
            target.group = origin.group;
            target.lastoper = origin.lastoper;
            target.lastopersid = origin.lastopersid;
            target.logintime = origin.logintime;
            target.onecaruser = origin.onecaruser;
            target.rid = origin.rid;
            target.sysflag = origin.sysflag;
            target.token = origin.token;
            target.uid = origin.uid;
            target.webserverflag = origin.webserverflag;
            target.userIP = origin.userIP;

        }
    }

}