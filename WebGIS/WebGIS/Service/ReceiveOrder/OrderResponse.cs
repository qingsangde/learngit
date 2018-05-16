using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using WebGIS;
using System.IO;
using CommLibrary.Proto;
using CommLibrary.ProtocolDataModel;
using CommLibrary;
using System.Data;
using System.Data.SqlClient;

namespace SysService
{
    public class OrderResponse
    {
        public static Hashtable OrderResponseHs = new Hashtable();//存储指令回传结果，key为uid|cid|sysflag|dwPackageType|dwOperation1|nCMD,获取时，如果包含这个key，则更新value，如果不包含，则添加key/value，value值为ResponseModel类型数据
        public static Hashtable ImageResponseCarHs = new Hashtable();//存储指令回传结果中有照片数据的车辆cid,key为uid|cid|sysflag
        public static Hashtable PositionResponseCarHs = new Hashtable();//存放指令回传结果中，有点名定位数据的车辆cid,key为uid|cid|sysflag

        /// <summary>
        /// 轮询调用获取指令结果
        /// </summary>
        /// <param name="inparams"></param>
        /// <returns></returns>
        public ResponseResult QueryOrderResultOut(Dictionary<string, string> inparams)
        {
            ResponseResult Result = null;
            try
            {

                List<ResponseTransmissionModel> transmissionResponseList = new List<ResponseTransmissionModel>();
                string token = inparams["token"];
                string uid = inparams["sysuid"];
                string[] visitedphotocids = inparams["visitedphotocids"].Split(',');//已经查看过最新照片的cid
                string[] visitedpositioncids = inparams["visitedpositioncids"].Split(',');//已经访问过最新定位的cid
                string[] visitedrecordkeys = inparams["visitedrecordkeys"].Split(',');//已经查看过记录仪回传数据的key

                SessionModel sm = new SessionModel();
                sm = SessionManager.GetSession(token);

                //首先处理已经查看完的指令回传数据
                if (visitedphotocids.Length > 0)
                {
                    int n1 = visitedphotocids.Length;
                    for (int i = 0; i < n1; i++)
                    {
                        if (ImageResponseCarHs.ContainsKey(visitedphotocids[i].ToString()))
                        {
                            ImageResponseCarHs.Remove(visitedphotocids[i].ToString());
                        }
                    }
                }
                if (visitedpositioncids.Length > 0)
                {
                    int n2 = visitedpositioncids.Length;
                    for (int j = 0; j < n2; j++)
                    {
                        if (PositionResponseCarHs.ContainsKey(visitedpositioncids[j].ToString()))
                        {
                            PositionResponseCarHs.Remove(visitedpositioncids[j].ToString());
                        }
                    }
                }
                if (visitedrecordkeys.Length > 0)
                {
                    int n3 = visitedrecordkeys.Length;
                    for (int k = 0; k < n3; k++)
                    {
                        if (OrderResponseHs.ContainsKey(visitedrecordkeys[k].ToString()))
                        {
                            OrderResponseHs.Remove(visitedrecordkeys[k].ToString());
                        }
                    }
                }




                List<OrderResultModel> orms = SendOrderHander.GetOrderResults(token);
                if (orms.Count == 0)
                {
                    return null;
                }

                

                //然后循环指令获取结果
                foreach (OrderResultModel orm in orms)
                {
                    if (orm == null) continue;
                    if (orm.dwPackageType == 30)
                    {
                        if (orm.dwOperation1 == 10)  //点名响应
                        {
                            string positionkey = token + "|" + orm.cid.ToString() + "|" + orm.sysflag;
                            if (!PositionResponseCarHs.ContainsKey(positionkey))//判断点名响应Hash表中，是否有这个车，如果没有，则加入，有则掠过
                            {

                                PositionResponseCarHs.Add(positionkey, orm.cid);

                            }
                        }
                        else if (orm.dwOperation1 == 33) //拍照响应
                        {
                            string imagekey = token + "|" + orm.cid.ToString() + "|" + orm.sysflag;
                            if (!ImageResponseCarHs.ContainsKey(imagekey))//判断拍照响应Hash表中，是否有这个车，如果没有，则加入，有则掠过
                            {

                                ImageResponseCarHs.Add(imagekey, orm.cid);

                            }
                        }
                        else if (orm.dwOperation1 == 99) //透传指令响应
                        {
                            ResponseTransmissionModel tranModel = new ResponseTransmissionModel();
                            string key = token + "|" + orm.cid.ToString() + "|" + orm.sysflag + "|" + orm.dwPackageType.ToString() + "|" + orm.dwOperation1.ToString() + "|";
                            string TransID = "";
                            MemoryStream ms = new MemoryStream(orm.ResData.byteContext);
                            CTS_TransmissionProtocol Response = ProtoBuf.Serializer.Deserialize<CTS_TransmissionProtocol>(ms);
                            ms.Close();
                            TransID = Response.nTransID.ToString();
                            key = key + TransID;
                            switch (Response.sData[1])
                            {
                                case 0x05:// 表示设置还款日期命令
                                    {
                                        if (Response.sData[2] == 0x00)
                                        {
                                            //成功
                                            tranModel.cid = orm.cid;
                                            tranModel.carno = getOneCarNo(orm.cid.ToString(), token);
                                            tranModel.sysflag = orm.sysflag;
                                            tranModel.transmissionDesc = "设置还款日期及提醒日期";
                                            tranModel.transmissionResult = "成功";
                                            //UpdateJFQZExtend_TransmissionResponse(orm.sysflag, orm.cid, 5);
                                        }
                                        else
                                        {
                                            //失败
                                            tranModel.cid = orm.cid;
                                            tranModel.carno = getOneCarNo(orm.cid.ToString(), token);
                                            tranModel.sysflag = orm.sysflag;
                                            tranModel.transmissionDesc = "设置还款日期及提醒日期";
                                            tranModel.transmissionResult = "失败";
                                        }
                                        break;
                                    }
                                case 0x06:// 表示激活/关闭发动机销贷功能
                                    {
                                        if (Response.sData[2] == 0x00)
                                        {
                                            //成功
                                            tranModel.cid = orm.cid;
                                            tranModel.carno = getOneCarNo(orm.cid.ToString(), token);
                                            tranModel.sysflag = orm.sysflag;
                                            tranModel.transmissionDesc = Response.sData[3] == 0x01 ? "激活发动机销贷功能" : "关闭发动机销贷功能";
                                            tranModel.transmissionResult = "成功";
                                        }
                                        else
                                        {
                                            //失败
                                            tranModel.cid = orm.cid;
                                            tranModel.carno = getOneCarNo(orm.cid.ToString(), token);
                                            tranModel.sysflag = orm.sysflag;
                                           // tranModel.transmissionDesc = "激活/关闭发动机销贷功能";
                                            tranModel.transmissionDesc = Response.sData[3] == 0x01 ? "激活发动机销贷功能" : "关闭发动机销贷功能";
                                            tranModel.transmissionResult = "失败";
                                        }
                                        break;
                                    }
                                case 0x07:// 表示锁车/解锁功能
                                    {
                                        if (Response.sData[2] == 0x00)
                                        {
                                            //成功
                                            tranModel.cid = orm.cid;
                                            tranModel.carno = getOneCarNo(orm.cid.ToString(), token);
                                            tranModel.sysflag = orm.sysflag;
                                            tranModel.transmissionDesc =Response.sData[3]==0x01? "锁车":"解锁";
                                            tranModel.transmissionResult = "成功";
                                            //UpdateJFQZExtend_TransmissionResponse(orm.sysflag, orm.cid, 7);
                                        }
                                        else
                                        {
                                            //失败
                                            tranModel.cid = orm.cid;
                                            tranModel.carno = getOneCarNo(orm.cid.ToString(), token);
                                            tranModel.sysflag = orm.sysflag;
                                            tranModel.transmissionDesc = Response.sData[3] == 0x01 ? "锁车" : "解锁";
                                            tranModel.transmissionResult = "失败";
                                        }
                                        break;
                                    }
                                //case 0x08:// 表示车辆状态上传
                                //    { break; }
                            }
                            transmissionResponseList.Add(tranModel);

                        }
                        #region 行驶记录仪数据采集回传
                        else if (orm.dwOperation1 == 41) //行驶记录仪数据采集回传
                        {
                            //long recordkey = orm.cid;
                            //if (!RecordResponseCarHs.ContainsKey(recordkey))//判断记录仪响应Hash表中，是否有这个车，如果没有，则加入，有则掠过
                            //{
                            //    RecordResponseCarHs.Add(recordkey, orm.cid);
                            //}
                            string key = token + "|" + orm.cid.ToString() + "|" + orm.sysflag + "|" + orm.dwPackageType.ToString() + "|" + orm.dwOperation1.ToString() + "|";
                            string nCMD = "";
                            string cmdDesc = "";
                            MemoryStream ms = new MemoryStream(orm.ResData.byteContext);
                            CTS_DriveRecordDataUpResponse Response = ProtoBuf.Serializer.Deserialize<CTS_DriveRecordDataUpResponse>(ms);
                            ms.Close();
                            nCMD = Response.nCMD.ToString();
                            key = key + nCMD;
                            MemoryStream ms2 = new MemoryStream(Response.sDataBlock);
                            object subdata = null;
                            object convertdata = null;
                            switch (Response.nCMD)
                            {
                                case 0x00://命令字0x00  采集记录仪执行标准版本
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderVersionUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DrivingRecorderVersionUpModel a = new CTS_DrivingRecorderVersionUpModel();
                                        a.dwTNO = ((CTS_DrivingRecorderVersionUp)subdata).dwTNO;
                                        a.nAnswerSerialID = ((CTS_DrivingRecorderVersionUp)subdata).nAnswerSerialID;
                                        a.nMSGID = ((CTS_DrivingRecorderVersionUp)subdata).nMSGID;
                                        a.Year = ((CTS_DrivingRecorderVersionUp)subdata).Year;
                                        a.ReviseNO = ((CTS_DrivingRecorderVersionUp)subdata).ReviseNO;
                                        convertdata = a;
                                        cmdDesc = "采集记录仪执行标准版本";
                                        break;
                                    }
                                case 0x01://命令字0x01  采集当前驾驶人信息(机动车驾驶证号码)
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DriverCodeDrivingLicUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DriverCodeDrivingLicUpModel b = new CTS_DriverCodeDrivingLicUpModel();
                                        b.dwTNO = ((CTS_DriverCodeDrivingLicUp)subdata).dwTNO;
                                        b.nDriverCode = ((CTS_DriverCodeDrivingLicUp)subdata).nDriverCode;
                                        if (((CTS_DriverCodeDrivingLicUp)subdata).sDrivingLicNum != null)
                                        {
                                            b.sDrivingLicNum = System.Text.Encoding.Default.GetString(((CTS_DriverCodeDrivingLicUp)subdata).sDrivingLicNum).Replace("\0", ""); ;
                                        }
                                        else
                                        {
                                            b.sDrivingLicNum = "";
                                        }
                                        convertdata = b;
                                        cmdDesc = "采集当前驾驶人信息";
                                        break;
                                    }
                                case 0x02://命令字0x02  采集记录仪的实时时钟
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_CurrentTimeDataUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_CurrentTimeDataUpModel c = new CTS_CurrentTimeDataUpModel();
                                        c.dwTNO = ((CTS_CurrentTimeDataUp)subdata).dwTNO;
                                        c.nDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(((CTS_CurrentTimeDataUp)subdata).nDateTime)).ToLocalTime();
                                        convertdata = c;
                                        cmdDesc = "采集记录仪的实时时钟";
                                        break;
                                    }
                                case 0x03://命令字0x03  采集累计行程里程
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_CarSumMilesUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_CarSumMilesUpModel d = new CTS_CarSumMilesUpModel();
                                        d.dwTNO = ((CTS_CarSumMilesUp)subdata).dwTNO;
                                        d.nInitialMilesValue = ((CTS_CarSumMilesUp)subdata).nInitialMilesValue;
                                        d.nSumMilesValue = ((CTS_CarSumMilesUp)subdata).nSumMilesValue;
                                        d.nInstallDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(((CTS_CarSumMilesUp)subdata).nInstallDateTime)).ToLocalTime();
                                        d.nRealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(((CTS_CarSumMilesUp)subdata).nRealDateTime)).ToLocalTime();
                                        convertdata = d;
                                        cmdDesc = "采集累计行驶里程";
                                        break;
                                    }
                                case 0x04://命令字0x04  采集记录仪脉冲系数
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_CarCharacterQuotientUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_CarCharacterQuotientUpModel e = new CTS_CarCharacterQuotientUpModel();
                                        e.dwTNO = ((CTS_CarCharacterQuotientUp)subdata).dwTNO;
                                        e.nCharacterQuotientValue = ((CTS_CarCharacterQuotientUp)subdata).nCharacterQuotientValue;
                                        e.nRealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(((CTS_CarCharacterQuotientUp)subdata).nRealDateTime)).ToLocalTime();
                                        convertdata = e;
                                        cmdDesc = "采集记录仪脉冲系数";
                                        break;
                                    }
                                case 0x05://命令字0x05  采集车辆信息(车辆识别代号、机动车号牌号码、机动车号牌分类)
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_CarRecorderVinPlateAndTypeUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_CarRecorderVinPlateAndTypeUpModel f = new CTS_CarRecorderVinPlateAndTypeUpModel();
                                        f.dwTNO = ((CTS_CarRecorderVinPlateAndTypeUp)subdata).dwTNO;
                                        if (((CTS_CarRecorderVinPlateAndTypeUp)subdata).sVinNum != null)
                                        {
                                            f.sVinNum = System.Text.Encoding.Default.GetString(((CTS_CarRecorderVinPlateAndTypeUp)subdata).sVinNum).Replace("\0", ""); ;
                                        }
                                        else
                                        {
                                            f.sVinNum = "";
                                        }
                                        if (((CTS_CarRecorderVinPlateAndTypeUp)subdata).sCarPlateNum != null)
                                        {
                                            f.sCarPlateNum = System.Text.Encoding.Default.GetString(((CTS_CarRecorderVinPlateAndTypeUp)subdata).sCarPlateNum).Replace("\0", ""); ;
                                        }
                                        else
                                        {
                                            f.sCarPlateNum = "";
                                        }
                                        if (((CTS_CarRecorderVinPlateAndTypeUp)subdata).sCarPlateType != null)
                                        {
                                            f.sCarPlateType = System.Text.Encoding.Default.GetString(((CTS_CarRecorderVinPlateAndTypeUp)subdata).sCarPlateType).Replace("\0", ""); ;
                                        }
                                        else
                                        {
                                            f.sCarPlateType = "";
                                        }
                                        convertdata = f;
                                        cmdDesc = "采集车辆信息";
                                        break;
                                    }

                                case 0x06://命令字0x06  采集记录仪状态信号配置信息
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderStatusConfigUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DrivingRecorderStatusConfigUpModel g = new CTS_DrivingRecorderStatusConfigUpModel();
                                        g.dwTNO = ((CTS_DrivingRecorderStatusConfigUp)subdata).dwTNO;
                                        g.nAnswerSerialID = ((CTS_DrivingRecorderStatusConfigUp)subdata).nAnswerSerialID;
                                        g.nMSGID = ((CTS_DrivingRecorderStatusConfigUp)subdata).nMSGID;
                                        g.nStatus = ((CTS_DrivingRecorderStatusConfigUp)subdata).nStatus;
                                        g.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(((CTS_DrivingRecorderStatusConfigUp)subdata).RealDateTime)).ToLocalTime();
                                        List<CTS_Semaphore_ItemModel> list = new List<CTS_Semaphore_ItemModel>();
                                        List<CTS_DrivingRecorderStatusConfigUp.CTS_Semaphore_Item> sublist = ((CTS_DrivingRecorderStatusConfigUp)subdata).SemaphoreItem;
                                        foreach (CTS_DrivingRecorderStatusConfigUp.CTS_Semaphore_Item item in sublist)
                                        {
                                            CTS_Semaphore_ItemModel itemmodel = new CTS_Semaphore_ItemModel();
                                            string sname = "";
                                            if (item.sname != null)
                                            {
                                                sname = System.Text.Encoding.Default.GetString(item.sname).Replace("\0", "");
                                            }
                                            itemmodel.sname = sname;
                                            itemmodel.bitIndex = item.bitIndex;
                                            list.Add(itemmodel);
                                        }
                                        g.SemaphoreList = list;
                                        g.sStatus = ConvertStatus(g.nStatus, list);
                                        convertdata = g;
                                        cmdDesc = "采集记录仪状态信号配置信息";
                                        break;
                                    }
                                case 0x07://命令字0x07  采集记录仪唯一性编号
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderIdentifierUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DrivingRecorderIdentifierUpModel h = new CTS_DrivingRecorderIdentifierUpModel();
                                        h.dwTNO = ((CTS_DrivingRecorderIdentifierUp)subdata).dwTNO;
                                        h.nAnswerSerialID = ((CTS_DrivingRecorderIdentifierUp)subdata).nAnswerSerialID;
                                        h.nMSGID = ((CTS_DrivingRecorderIdentifierUp)subdata).nMSGID;
                                        h.nDatetime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(((CTS_DrivingRecorderIdentifierUp)subdata).nDatetime)).ToLocalTime();
                                        h.nSerialNO = ((CTS_DrivingRecorderIdentifierUp)subdata).nSerialNO.ToString().PadLeft(8, '0');
                                        if (((CTS_DrivingRecorderIdentifierUp)subdata).s3cCode != null)
                                        {
                                            h.s3cCode = System.Text.Encoding.Default.GetString(((CTS_DrivingRecorderIdentifierUp)subdata).s3cCode).Replace("\0", ""); ;
                                        }
                                        else
                                        {
                                            h.s3cCode = "";
                                        }

                                        if (((CTS_DrivingRecorderIdentifierUp)subdata).sProductType != null)
                                        {
                                            h.sProductType = System.Text.Encoding.Default.GetString(((CTS_DrivingRecorderIdentifierUp)subdata).sProductType).Replace("\0", ""); ;
                                        }
                                        else
                                        {
                                            h.sProductType = "";
                                        }
                                        convertdata = h;
                                        cmdDesc = "采集记录仪唯一性编号";
                                        break;
                                    }
                                case 0x08://命令字0x08  采集指定的行驶速度记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderSpeedRecordUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DrivingRecorderSpeedRecordUpModel aa = new CTS_DrivingRecorderSpeedRecordUpModel();
                                        aa.dwTNO = ((CTS_DrivingRecorderSpeedRecordUp)subdata).dwTNO;
                                        aa.nAnswerSerialID = ((CTS_DrivingRecorderSpeedRecordUp)subdata).nAnswerSerialID;
                                        aa.nMSGID = ((CTS_DrivingRecorderSpeedRecordUp)subdata).nAnswerSerialID;
                                        List<CTS_DrivingRecorderSpeedRecordUp.CTS_OneMinute_Item> OneMinuteItemL = ((CTS_DrivingRecorderSpeedRecordUp)subdata).OneMinuteItem;
                                        List<CTS_OneMinute_ItemModel> list = new List<CTS_OneMinute_ItemModel>();
                                        foreach (CTS_DrivingRecorderSpeedRecordUp.CTS_OneMinute_Item item in OneMinuteItemL)
                                        {
                                            CTS_OneMinute_ItemModel itemmodel = new CTS_OneMinute_ItemModel();
                                            itemmodel.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.RealDateTime)).ToLocalTime();
                                            if (item.sDriverLicenseCode != null)
                                            {
                                                itemmodel.sDriverLicenseCode = System.Text.Encoding.Default.GetString(item.sDriverLicenseCode).Replace("\0", ""); ;
                                            }
                                            else
                                            {
                                                itemmodel.sDriverLicenseCode = "";
                                            }
                                            List<CTS_SpeedStatusItemModel> sublist = new List<CTS_SpeedStatusItemModel>();
                                            List<CTS_DrivingRecorderSpeedRecordUp.CTS_OneMinute_Item.CTS_SpeedStatusItem> SpeedStatusItemL = item.SpeedStatusItem;
                                            foreach (CTS_DrivingRecorderSpeedRecordUp.CTS_OneMinute_Item.CTS_SpeedStatusItem subitem in SpeedStatusItemL)
                                            {
                                                CTS_SpeedStatusItemModel subitemmodel = new CTS_SpeedStatusItemModel();
                                                subitemmodel.index = subitem.index;
                                                subitemmodel.speed = subitem.speed;
                                                subitemmodel.status = subitem.status;
                                                subitemmodel.time = itemmodel.RealDateTime.AddSeconds(Convert.ToDouble(subitem.index));
                                                sublist.Add(subitemmodel);
                                            }
                                            itemmodel.SpeedStatusItemList = sublist;
                                            list.Add(itemmodel);
                                        }
                                        aa.OneMinuteItemList = list;
                                        convertdata = aa;
                                        cmdDesc = "采集指定的行驶速度记录";
                                        break;
                                    }
                                case 0x09://命令字0x09  采集指定的位置信息记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DrivingRecorderPositionRecordUp>(ms2);
                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DrivingRecorderPositionRecordUpModel bb = new CTS_DrivingRecorderPositionRecordUpModel();
                                        bb.dwTNO = ((CTS_DrivingRecorderPositionRecordUp)subdata).dwTNO;
                                        bb.nAnswerSerialID = ((CTS_DrivingRecorderPositionRecordUp)subdata).nAnswerSerialID;
                                        bb.nMSGID = ((CTS_DrivingRecorderPositionRecordUp)subdata).nMSGID;
                                        List<CTS_ONE_HOUR_ItemModel> list = new List<CTS_ONE_HOUR_ItemModel>();
                                        List<CTS_DrivingRecorderPositionRecordUp.CTS_ONE_HOUR_Item> CTS_ONE_HOUR_ItemL = ((CTS_DrivingRecorderPositionRecordUp)subdata).OneHourItem;
                                        foreach (CTS_DrivingRecorderPositionRecordUp.CTS_ONE_HOUR_Item item in CTS_ONE_HOUR_ItemL)
                                        {
                                            CTS_ONE_HOUR_ItemModel itemmodel = new CTS_ONE_HOUR_ItemModel();
                                            itemmodel.BeginDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.BeginDateTime));
                                            List<CTS_SpeedPositionItemModel> sublist = new List<CTS_SpeedPositionItemModel>();
                                            List<CTS_DrivingRecorderPositionRecordUp.CTS_ONE_HOUR_Item.CTS_SpeedPositionItem> CTS_SpeedPositionItemL = item.SpeedPositionItem;
                                            foreach (CTS_DrivingRecorderPositionRecordUp.CTS_ONE_HOUR_Item.CTS_SpeedPositionItem subitem in CTS_SpeedPositionItemL)
                                            {
                                                CTS_SpeedPositionItemModel subitemmodel = new CTS_SpeedPositionItemModel();
                                                subitemmodel.Lati = subitem.Lati;
                                                subitemmodel.Long = subitem.Long;
                                                subitemmodel.AltitudeMeters = subitem.AltitudeMeters;
                                                subitemmodel.AverageSpeed = subitem.AverageSpeed;
                                                subitemmodel.ItemNO = subitem.ItemNO;
                                                subitemmodel.time = itemmodel.BeginDateTime.AddMinutes(Convert.ToDouble(subitem.ItemNO));
                                                sublist.Add(subitemmodel);
                                                //未完
                                            }
                                            itemmodel.SpeedPositionItemList = sublist;
                                            list.Add(itemmodel);
                                        }
                                        bb.OneHourItemList = list;
                                        convertdata = bb;

                                        cmdDesc = "采集指定的位置信息记录";
                                        break;
                                    }
                                case 0x10://命令字0x10  采集指定的事故疑点记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_AccidentDoubtRecordUp>(ms2);

                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_AccidentDoubtRecordUpModel cc = new CTS_AccidentDoubtRecordUpModel();
                                        cc.dwTNO = ((CTS_AccidentDoubtRecordUp)subdata).dwTNO;
                                        cc.nAnswerSerialID = ((CTS_AccidentDoubtRecordUp)subdata).nAnswerSerialID;
                                        cc.nMSGID = ((CTS_AccidentDoubtRecordUp)subdata).nMSGID;
                                        List<SpecifiedAccidentDoubtItemModel> list = new List<SpecifiedAccidentDoubtItemModel>();
                                        List<CTS_AccidentDoubtRecordUp.SpecifiedAccidentDoubtItem> SpecifiedAccidentDoubtItemL = ((CTS_AccidentDoubtRecordUp)subdata).AccidentDoubtItem;
                                        foreach (CTS_AccidentDoubtRecordUp.SpecifiedAccidentDoubtItem item in SpecifiedAccidentDoubtItemL)
                                        {
                                            SpecifiedAccidentDoubtItemModel itemmodel = new SpecifiedAccidentDoubtItemModel();
                                            itemmodel.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.RealDateTime)).ToLocalTime();
                                            if (item.sDriverLicenseCode != null)
                                            {
                                                itemmodel.sDriverLicenseCode = System.Text.Encoding.Default.GetString(item.sDriverLicenseCode).Replace("\0", ""); ;
                                            }
                                            else
                                            {
                                                itemmodel.sDriverLicenseCode = "";
                                            }
                                            itemmodel.Lati = item.Lati;
                                            itemmodel.Long = item.Long;
                                            itemmodel.AltitudeMeters = item.AltitudeMeters;
                                            List<UnitStatusItemModel> sublist = new List<UnitStatusItemModel>();
                                            List<CTS_AccidentDoubtRecordUp.SpecifiedAccidentDoubtItem.UnitStatusItem> UnitStatusItemL = item.SpeedStatusItem;
                                            foreach (CTS_AccidentDoubtRecordUp.SpecifiedAccidentDoubtItem.UnitStatusItem subitem in UnitStatusItemL)
                                            {
                                                UnitStatusItemModel subitemmodel = new UnitStatusItemModel();
                                                subitemmodel.index = subitem.index;
                                                subitemmodel.speed = subitem.speed;
                                                subitemmodel.status = subitem.status;
                                                subitemmodel.time = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.RealDateTime) - 0.2 * subitem.index).ToLocalTime();
                                                sublist.Add(subitemmodel);
                                            }
                                            itemmodel.UnitStatusItemList = sublist;
                                            list.Add(itemmodel);
                                        }
                                        cc.SpecifiedAccidentDoubtItemList = list;
                                        convertdata = cc;
                                        cmdDesc = "采集指定的事故疑点记录";
                                        break;
                                    }
                                case 0x11://命令字0x11  采集指定的超时驾驶记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_OverTimeDrivedRecordUp>(ms2);

                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_OverTimeDrivedRecordUpModel dd = new CTS_OverTimeDrivedRecordUpModel();
                                        dd.dwTNO = ((CTS_OverTimeDrivedRecordUp)subdata).dwTNO;
                                        dd.nAnswerSerialID = ((CTS_OverTimeDrivedRecordUp)subdata).nAnswerSerialID;
                                        dd.nMSGID = ((CTS_OverTimeDrivedRecordUp)subdata).nMSGID;
                                        List<CTS_DrivingRecord_ItemModel> list = new List<CTS_DrivingRecord_ItemModel>();
                                        List<CTS_OverTimeDrivedRecordUp.CTS_DrivingRecord_Item> CTS_DrivingRecord_ItemL = ((CTS_OverTimeDrivedRecordUp)subdata).DrivingRecordItem;
                                        foreach (CTS_OverTimeDrivedRecordUp.CTS_DrivingRecord_Item item in CTS_DrivingRecord_ItemL)
                                        {
                                            CTS_DrivingRecord_ItemModel itemmodel = new CTS_DrivingRecord_ItemModel();
                                            itemmodel.BeginTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.BeginTime)).ToLocalTime();
                                            itemmodel.EndTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.EndTime)).ToLocalTime();
                                            if (item.sDriverLicenseCode != null)
                                            {
                                                itemmodel.sDriverLicenseCode = System.Text.Encoding.Default.GetString(item.sDriverLicenseCode).Replace("\0", ""); ;
                                            }
                                            else
                                            {
                                                itemmodel.sDriverLicenseCode = "";
                                            }
                                            itemmodel.Begin_Lati = item.Begin_Lati;
                                            itemmodel.Begin_Long = item.Begin_Long;
                                            itemmodel.Begin_AltitudeMeters = item.Begin_AltitudeMeters;
                                            itemmodel.End_Lati = item.End_Lati;
                                            itemmodel.End_Long = item.End_Long;
                                            itemmodel.End_AltitudeMeters = item.End_AltitudeMeters;
                                            list.Add(itemmodel);
                                        }
                                        dd.CTS_DrivingRecord_ItemList = list;
                                        convertdata = dd;
                                        cmdDesc = "采集指定的超时驾驶记录";
                                        break;
                                    }
                                case 0x12://命令字0x12 采集指定的驾驶人身份记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_DriverLoginRecordUp>(ms2);

                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_DriverLoginRecordUpModel ee = new CTS_DriverLoginRecordUpModel();
                                        ee.dwTNO = ((CTS_DriverLoginRecordUp)subdata).dwTNO;
                                        ee.nAnswerSerialID = ((CTS_DriverLoginRecordUp)subdata).nAnswerSerialID;
                                        ee.nMSGID = ((CTS_DriverLoginRecordUp)subdata).nMSGID;
                                        List<CTS_LoginRecord_ItemModel> list = new List<CTS_LoginRecord_ItemModel>();
                                        List<CTS_DriverLoginRecordUp.CTS_LoginRecord_Item> CTS_LoginRecord_ItemL = ((CTS_DriverLoginRecordUp)subdata).LoginItem;
                                        foreach (CTS_DriverLoginRecordUp.CTS_LoginRecord_Item item in CTS_LoginRecord_ItemL)
                                        {
                                            CTS_LoginRecord_ItemModel itemmodel = new CTS_LoginRecord_ItemModel();
                                            itemmodel.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.RealDateTime)).ToLocalTime();
                                            itemmodel.type = item.type;
                                            if (item.sDriverLicenseCode != null)
                                            {
                                                itemmodel.sDriverLicenseCode = System.Text.Encoding.Default.GetString(item.sDriverLicenseCode).Replace("\0", ""); ;
                                            }
                                            else
                                            {
                                                itemmodel.sDriverLicenseCode = "";
                                            }
                                            list.Add(itemmodel);
                                        }
                                        ee.CTS_LoginRecord_ItemList = list;
                                        convertdata = ee;

                                        cmdDesc = "采集指定的驾驶人身份记录";
                                        break;
                                    }
                                case 0x13://命令字0x13  采集指定的外部供电记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_PowerSupplyRecordUp>(ms2);

                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_PowerSupplyRecordUpModel ff = new CTS_PowerSupplyRecordUpModel();
                                        ff.dwTNO = ((CTS_PowerSupplyRecordUp)subdata).dwTNO;
                                        ff.nAnswerSerialID = ((CTS_PowerSupplyRecordUp)subdata).nAnswerSerialID;
                                        ff.nMSGID = ((CTS_PowerSupplyRecordUp)subdata).nMSGID;
                                        List<CTS_PowerSupplyRecord_ItemModel> list = new List<CTS_PowerSupplyRecord_ItemModel>();
                                        List<CTS_PowerSupplyRecordUp.CTS_PowerSupplyRecord_Item> CTS_PowerSupplyRecord_ItemL = ((CTS_PowerSupplyRecordUp)subdata).SupplyItem;
                                        foreach (CTS_PowerSupplyRecordUp.CTS_PowerSupplyRecord_Item item in CTS_PowerSupplyRecord_ItemL)
                                        {
                                            CTS_PowerSupplyRecord_ItemModel itemmodel = new CTS_PowerSupplyRecord_ItemModel();
                                            itemmodel.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.RealDateTime)).ToLocalTime();
                                            itemmodel.type = item.type;
                                            list.Add(itemmodel);
                                        }
                                        ff.CTS_PowerSupplyRecord_ItemList = list;
                                        convertdata = ff;

                                        cmdDesc = "采集指定的外部供电记录";
                                        break;
                                    }
                                case 0x14://命令字0x14  采集指定的参数修改记录
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_ParamModifyRecordUp>(ms2);

                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_ParamModifyRecordUpModel gg = new CTS_ParamModifyRecordUpModel();
                                        gg.dwTNO = ((CTS_ParamModifyRecordUp)subdata).dwTNO;
                                        gg.nAnswerSerialID = ((CTS_ParamModifyRecordUp)subdata).nAnswerSerialID;
                                        gg.nMSGID = ((CTS_ParamModifyRecordUp)subdata).nMSGID;
                                        List<CTS_ParamModifyRecord_ItemModel> list = new List<CTS_ParamModifyRecord_ItemModel>();
                                        List<CTS_ParamModifyRecordUp.CTS_ParamModifyRecord_Item> CTS_ParamModifyRecord_ItemL = ((CTS_ParamModifyRecordUp)subdata).ModifyItem;
                                        foreach (CTS_ParamModifyRecordUp.CTS_ParamModifyRecord_Item item in CTS_ParamModifyRecord_ItemL)
                                        {
                                            CTS_ParamModifyRecord_ItemModel itemmodel = new CTS_ParamModifyRecord_ItemModel();
                                            itemmodel.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.RealDateTime)).ToLocalTime();
                                            itemmodel.EndDateTime = item.EndDateTime;
                                            list.Add(itemmodel);
                                        }
                                        gg.CTS_ParamModifyRecord_ItemList = list;
                                        convertdata = gg;

                                        cmdDesc = "采集指定的参数修改记录";
                                        break;
                                    }
                                case 0x15://命令字0x15  采集指定的速度状态日志
                                    {
                                        subdata = ProtoBuf.Serializer.Deserialize<CTS_SpeedStatusLogUp>(ms2);

                                        //新建对象类，转义subdata，以处理时间及字符串类型数据，避免乱码
                                        CTS_SpeedStatusLogUpModel hh = new CTS_SpeedStatusLogUpModel();
                                        hh.dwTNO = ((CTS_SpeedStatusLogUp)subdata).dwTNO;
                                        hh.nAnswerSerialID = ((CTS_SpeedStatusLogUp)subdata).nAnswerSerialID;
                                        hh.nMSGID = ((CTS_SpeedStatusLogUp)subdata).nMSGID;
                                        List<CTS_SpeedStatus_ItemModel> list = new List<CTS_SpeedStatus_ItemModel>();
                                        List<CTS_SpeedStatusLogUp.CTS_SpeedStatus_Item> CTS_SpeedStatus_ItemL = ((CTS_SpeedStatusLogUp)subdata).SpeedStatusItem;
                                        foreach (CTS_SpeedStatusLogUp.CTS_SpeedStatus_Item item in CTS_SpeedStatus_ItemL)
                                        {
                                            CTS_SpeedStatus_ItemModel itemmodel = new CTS_SpeedStatus_ItemModel();
                                            itemmodel.BeginDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.BeginDateTime)).ToLocalTime();
                                            itemmodel.EndDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(item.EndDateTime)).ToLocalTime();
                                            itemmodel.SpeedStatus = item.SpeedStatus;
                                            List<CTS_Speed_MatchModel> sublist = new List<CTS_Speed_MatchModel>();
                                            List<CTS_SpeedStatusLogUp.CTS_SpeedStatus_Item.CTS_Speed_Match> CTS_Speed_MatchL = item.SpeedMatch;
                                            foreach (CTS_SpeedStatusLogUp.CTS_SpeedStatus_Item.CTS_Speed_Match subitem in CTS_Speed_MatchL)
                                            {
                                                CTS_Speed_MatchModel subitemmodel = new CTS_Speed_MatchModel();
                                                subitemmodel.RealDateTime = DateTime.Parse("1970-01-01 00:00:00").AddSeconds(Convert.ToDouble(subitem.RealDateTime)).ToLocalTime();
                                                subitemmodel.RecordSpeed = subitem.RecordSpeed;
                                                subitemmodel.RerferenceSpeed = subitem.RerferenceSpeed;
                                                sublist.Add(subitemmodel);
                                            }
                                            itemmodel.CTS_Speed_MatchList = sublist;
                                            list.Add(itemmodel);
                                        }
                                        hh.CTS_SpeedStatus_ItemList = list;
                                        convertdata = hh;

                                        cmdDesc = "采集指定的速度状态日志";
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }

                            ResponseModel rm = new ResponseModel();
                            rm.cid = orm.cid;
                            DataRow[] rows = sm.cars.Select("cid = " + orm.cid.ToString());
                            if (rows.Length > 0)
                            {
                                rm.carno = rows[0]["carno"].ToString();
                            }
                            rm.sysflag = orm.sysflag;
                            rm.dwPackageType = orm.dwPackageType;
                            rm.dwOperation1 = orm.dwOperation1;
                            rm.nCMD = Response.nCMD;
                            rm.cmdDesc = cmdDesc;
                            rm.ResponseData = convertdata; ;
                            if (!OrderResponseHs.ContainsKey(key))
                            {
                                OrderResponseHs.Add(key, rm);//不存在，则添加
                            }

                        }
                        #endregion
                    }
                }

                OrderResponseCarResult orc = new OrderResponseCarResult();
                List<ResponseModel> recordResponseList = new List<ResponseModel>();
                List<long> imageResponseList = new List<long>();
                List<long> positionResponseList = new List<long>();
                foreach (string poskey in PositionResponseCarHs.Keys)
                {
                    string[] poskeyArr = poskey.Split('|');
                    if (poskeyArr[0].Equals(token))
                    {
                        positionResponseList.Add(Convert.ToInt64(PositionResponseCarHs[poskey]));
                    }
                }

                foreach (string imgkey in ImageResponseCarHs.Keys)
                {
                    string[] imgkeyArr = imgkey.Split('|');
                    if (imgkeyArr[0].Equals(token))
                    {
                        imageResponseList.Add(Convert.ToInt64(ImageResponseCarHs[imgkey]));
                    }
                }

                foreach (string reckey in OrderResponseHs.Keys)
                {
                    string[] reckeyArr = reckey.Split('|');
                    if (reckeyArr[0].Equals(token))
                    {
                        recordResponseList.Add((ResponseModel)OrderResponseHs[reckey]);
                    }
                }
                orc.PositionResponseList = positionResponseList; // new List<long>((long[])(new ArrayList(PositionResponseCarHs.Values)).ToArray(typeof(long)));
                orc.ImageResponseList = imageResponseList; // new List<long>((long[])(new ArrayList(ImageResponseCarHs.Values)).ToArray(typeof(long)));
                orc.RecordResponseList = recordResponseList;// new List<ResponseModel>((ResponseModel[])(new ArrayList(OrderResponseHs.Values)).ToArray(typeof(ResponseModel)));
                
                orc.TransmissionResponseList = transmissionResponseList;
                Result = new ResponseResult(ResState.Success, "", orc);
            }
            catch (Exception ex)
            {
                Result = new ResponseResult(ResState.OperationFailed, ex.Message, null);
            }
            return Result;
        }

        private string ConvertStatus(int p, List<CTS_Semaphore_ItemModel> list)
        {
            string res = "";
            foreach (CTS_Semaphore_ItemModel item in list)
            {
                long pow = Convert.ToInt64(Math.Pow(2, item.bitIndex));
                if ((p & pow) == pow)
                {
                    res += "D" + item.bitIndex + ":" + item.sname + "有信号.";
                }
                else
                {
                    res += "D" + item.bitIndex + ":" + item.sname + "无信号.";
                }
            }
            return res;

        }



        private string getOneCarNo(string cid, string token)
        {
            string TheCarNo = "";
            try
            {
                SessionModel sm = new SessionModel();
                sm = SessionManager.GetSession(token);
                DataRow[] rows = sm.cars.Select("cid = " + cid);
                if (rows.Length > 0)
                {
                    TheCarNo = rows[0]["carno"].ToString();
                }
            }
            catch (Exception)
            {
                TheCarNo = "";
            }
            return TheCarNo;
        }


        /// <summary>
        /// 销贷透传指令记录更新
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="RepaymentDate"></param>
        /// <param name="RemindDays"></param>
        /// <param name="DisconTimes"></param>
        /// <param name="SetSpeed"></param>
        /// <param name="LockDesc"></param>
        /// <param name="UnlockDesc"></param>
        private void UpdateJFQZExtend_TransmissionResponse(string sysflag, long cid, int type)
        {
            ComSqlHelper csh = new ComSqlHelper();
            SqlParameter[] Parameters = { 
                                            new SqlParameter("@cid", cid), 
                                            new SqlParameter("@type", type)
                                        };
            csh.ExecuteSPNoQuery(sysflag, WebProc.Proc("QWGProc_JFQZ_UpdateJFQZExtend_TransmissionResponse"), Parameters, false);
        }
    }

    public class ResponseModel
    {
        public long cid { get; set; }
        public string carno { get; set; }
        public string sysflag { get; set; }
        public uint dwPackageType { get; set; }
        public uint dwOperation1 { get; set; }
        public uint nCMD { get; set; } //记录仪回传命令字
        public string cmdDesc { get; set; } //命令字说明
        public object ResponseData { get; set; }
    }

    public class ResponseTransmissionModel
    {
        public long cid { get; set; }
        public string carno { get; set; }
        public string sysflag { get; set; }
        public string transmissionDesc { get; set; }
        public string transmissionResult { get; set; }
    }

    public class ResponseCar
    {
        public long cid { get; set; }
        public string tno { get; set; }
        public string carno { get; set; }
    }

    public class OrderResponseCarResult
    {
        public List<ResponseModel> RecordResponseList { get; set; }
        public List<long> ImageResponseList { get; set; }
        public List<long> PositionResponseList { get; set; }
        public List<ResponseTransmissionModel> TransmissionResponseList { get; set; }
    }
}