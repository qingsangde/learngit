using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysService
{
    /// <summary>
    /// 指令下发结果
    /// </summary>
    public class SendResult
    {
        public string CID { get; set; }
        public string TNO { get; set; }
        public string CarNo { get; set; }
        public string Time { get; set; }
        public bool Res { get; set; }
        public string Desc { get; set; }
    }
}