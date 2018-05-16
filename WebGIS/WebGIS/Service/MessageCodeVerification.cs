using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGIS.Service
{
    public class MessageCodeVerification
    {
        public PhoneInfo phone = new PhoneInfo();
        public static  Dictionary<string, PhoneInfo> dictionMsg = new Dictionary<string, PhoneInfo>();
    }



    /// <summary>
    /// 手机号码信息类
    /// </summary>
    public class PhoneInfo
    {
        public string phone;    //手机号码
        public DateTime time;   //时间
    }
}