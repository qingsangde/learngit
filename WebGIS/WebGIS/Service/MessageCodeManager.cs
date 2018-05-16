using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary;
using System.Threading;

namespace WebGIS.Service
{
    public class MessageCodeManager
    {
        static Thread th_msm = null;

        public static void SMSCodeManager()
        {
            if (th_msm == null)
            {
                th_msm = new Thread(SMSTimeOutManager);
                th_msm.IsBackground = true;
                th_msm.Start();
            }
        }


        #region 验证码维护 定时删除
        /// <summary>
        /// 验证码维护 定时删除
        /// </summary>
        private static void SMSTimeOutManager()
        {
            PhoneInfo p = new PhoneInfo();
            while (true)
            {
                try
                {
                    List<string> list_code = new List<string>();

                    //循环将其验证码信息添加到list中，以便循环对比使用
                    foreach (string smsCode in MessageCodeVerification.dictionMsg.Keys)
                    {
                        list_code.Add(smsCode);
                    }
                    foreach (string smsCode in list_code)
                    {
                        MessageCodeVerification.dictionMsg.TryGetValue(smsCode, out p);
                        //求两个时间之间的差值
                        int minutes = (int)((TimeSpan)DateTime.Now.Subtract(p.time)).TotalMinutes;
                        if (minutes > 10)
                        {
                            //如果当前系统时间大于静态数据中存储的时间，那么将其删除；
                            MessageCodeVerification.dictionMsg.Remove(smsCode);
                        }
                    }
                }
                catch (Exception ex)
                {

                    LogHelper.WriteError("SMSTimeoutManager执行异常", ex);
                }
                Thread.Sleep(60 * 10 * 1000);    //休眠10分钟
                //Thread.Sleep(60 * 1000);        //休眠1分钟
            }
        }

        #endregion


        #region 检查验证码是否存在
        /// <summary>
        /// 检查验证码是否存在
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool CheckSms(string smsCode,string mobileno)
        {
            bool flag = false;
            if (MessageCodeVerification.dictionMsg.ContainsKey(smsCode))
            {
                if(MessageCodeVerification.dictionMsg[smsCode].phone==mobileno)
                flag = true;
                

            }
            return flag;
        }
        #endregion


        #region 根据key删除信息
        /// <summary>
        /// 根据key删除信息
        /// </summary>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public static void DelSmsByCode(string smsCode)
        {
            MessageCodeVerification.dictionMsg.Remove(smsCode);
        }

        #endregion
    }
}