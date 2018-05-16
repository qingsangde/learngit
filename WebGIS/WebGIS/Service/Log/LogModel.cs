using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysService
{
    public class LogModel
    {
        public string SysFlag { get; set; }
        public int UserId { get; set; }
        public int? LogType { get; set; }
        public string LogContent { get; set; }
        public string LogResult { get; set; }
        public string OneCarUser { get; set; }
        public string SysName { get; set; }
    }
}