using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommLibrary.Proto;

namespace WebGIS
{
    public class SendOrderHander
    {
        /// <summary>
        /// 下发立即牌照指令
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="tno"></param>
        /// <returns></returns>
        public static bool Send_CTS_ImageRequestDown(string sysflag, string token, long cid, long tno, uint ch=0)
        {
            QMDPartnerPackage package = RequestPackage.getImageRequestDown(tno,ch);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }
        /// <summary>
        /// 下发行驶记录数据采集命令
        /// 命令字0x00 采集记录仪执行标准版本
        /// 命令字0x01 采集当前驾驶人信息(机动车驾驶证号码)
        /// 命令字0x02 采集记录仪的实时时钟
        /// 命令字0x03  采集累计行程里程
        /// 命令字0x04  采集记录仪脉冲系数
        /// 命令字0x05  采集车辆信息(车辆识别代号、机动车号牌号码、机动车号牌分类)
        /// 命令字0x06  采集记录仪状态信号配置信息
        /// 命令字0x07  采集记录仪唯一性编号
        /// 命令字0x08  采集指定的行驶速度记录,有时间参数，跨度不超过1小时
        /// 命令字0x09  采集指定的位置信息记录,有时间参数，跨度不超过1小时
        /// 命令字0x10  采集指定的事故疑点记录,有时间参数，跨度不超过1分钟
        /// 命令字0x11  采集指定的超时驾驶记录,有时间参数，跨度不超过7天
        /// 命令字0x12  采集指定的驾驶人身份记录,有时间参数，跨度不超过7天
        /// 命令字0x13  采集指定的外部供电记录,有时间参数，跨度不超过7天
        /// 命令字0x14  采集指定的参数修改记录,有时间参数，跨度不超过7天
        /// 命令字0x15  采集指定的速度状态日志,有时间参数，跨度不超过1分钟
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="tno"></param>
        /// <param name="ncmd"></param>
        /// <param name="nType"></param>
        /// <param name="nBeginDateTime"></param>
        /// <param name="nEndDateTime"></param>
        /// <returns></returns>
        public static bool Send_CTS_DriveRecordDataCollectionRequest(string sysflag, string token, long cid, long tno, uint ncmd, uint nType = 0, long nBeginDateTime = 0, long nEndDateTime = 0)
        {
            QMDPartnerPackage package = RequestPackage.getDriveRecordDataCollectionRequest(tno, ncmd, nType, nBeginDateTime, nEndDateTime);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }

        public static bool Send_DriveRecordDownCMDDown_CharacterQuotient(string sysflag, string token, long cid, long tno, uint quotient)
        {
            QMDPartnerPackage package = RequestPackage.getDriveRecordDownCMDDown_CharacterQuotient(tno, quotient);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }

        public static bool Send_DriveRecordDownCMDDown_VIN_NUM_TYPE(string sysflag, string token, long cid, long tno, string vin, string platenum, string plateType)
        {
            QMDPartnerPackage package = RequestPackage.getDriveRecordDownCMDDown_VIN_NUM_TYPE(tno,vin,platenum,plateType);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }

        /// <summary>
        /// 下发车辆点名指令
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="tno"></param>
        /// <returns></returns>
        public static bool Send_CTS_TermSearchRequest(string sysflag, string token, long cid, long tno)
        {
            QMDPartnerPackage package = RequestPackage.getTermSearchRequest(tno);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }
        /// <summary>
        ///  下发透传指令
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="tno"></param>
        /// <param name="TransId">透传ID</param>
        /// <param name="TransmissionData">透传数据</param>
        /// <returns></returns>
        public static bool Send_CTS_TransmissionProtocol(string sysflag, string token, long cid, long tno, int TransId, byte[] TransmissionData)
        {
            QMDPartnerPackage package = RequestPackage.getCTS_TransmissionProtocol(tno, TransId, TransmissionData);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }
        /// <summary>
        ///终端参数设置指令       
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="tno"></param>
        /// <param name="paramlist"></param>
        /// <returns></returns>
        public static bool Send_CTS_SetTermParamDown(string sysflag, string token, long cid, long tno, List<CTS_SetTermParamDown.TerminalParamItem> paramlist)
        {
            QMDPartnerPackage package = RequestPackage.getSetTermParamDown(tno, paramlist);
            return RDSConfig.SendMsg(sysflag, token, cid, package);
        }
        /// <summary>
        ///终端参数设置指令（启明自定义扩展参数）       
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="token"></param>
        /// <param name="cid"></param>
        /// <param name="tno"></param>
        /// <param name="paramlist"></param>
        /// <returns></returns>
        public static bool Send_CTS_SetTermParamDownQMExtend(string sysflag, string token, long cid, long tno, List<CTS_SetTermParamDown.TerminalParamItem> paramlist)
        {
            foreach (var sa in paramlist)
            {
                List<CTS_SetTermParamDown.TerminalParamItem> paramlist_one = new List<CTS_SetTermParamDown.TerminalParamItem>();
                paramlist_one.Add(sa);
                QMDPartnerPackage package = RequestPackage.getSetTermParamDown(tno, paramlist_one, 85);
                bool sd=  RDSConfig.SendMsg(sysflag, token, cid, package);
                if (!sd)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取用户的指令执行结果，该方法需要轮询调用
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static List<OrderResultModel> GetOrderResults(string token)
        {
            if (RDSConfig.OrderResultTable.ContainsKey(token))
            {
                List<OrderResultModel> sd = new List<OrderResultModel>(((List<OrderResultModel>)RDSConfig.OrderResultTable[token]).ToArray());
                RDSConfig.OrderResultTable.Remove(token);
                return sd;
            }
            else
            {
                return  new List<OrderResultModel>();
            }
        }
    }
}