using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Collections;
using SocketPackage;

using System.Text;
using CommLibrary.Proto;
using System.Runtime.Serialization.Formatters.Binary;
using CommLibrary;
using System.Threading;

namespace WebGIS
{
    public class RDSConfig
    {
        public static Hashtable RDSClient = new Hashtable();
        static string basepath = HttpRuntime.AppDomainAppPath.ToString();
        static FileSystemWatcher fswWatcher = null;
        public static DataTable dtRDSList = new DataTable();
        public static Hashtable UserOrderTable = new Hashtable();
        public static Hashtable OrderResultTable = new Hashtable();
        public static Hashtable OrderAndResultTime = new Hashtable();
        public static Dictionary<string, Dictionary<long, int>> AllCarsOnlineStatus = new Dictionary<string, Dictionary<long, int>>();
        private static void AddOrder(string sysflag, string token, long carid, uint dwPackageType, uint dwOperation1, string extfalg = "")
        {
            string key = sysflag + "|" + carid + "|" + dwPackageType + "|" + dwOperation1 + "|" + extfalg;
            if (UserOrderTable.ContainsKey(key))
            {
                UserOrderTable[key] += "," + token;

            }
            else
            {
                UserOrderTable.Add(key, token);
            }
            if (OrderAndResultTime.ContainsKey(key))
            {
                OrderAndResultTime[key] = DateTime.Now;
            }
            else
            {
                OrderAndResultTime.Add(key, DateTime.Now);
            }
        }
        private static void Manager()
        {
            while (true)
            {
                Thread.Sleep(10000);
                //检查所有实时信息服务socket连接
                foreach (RDSConfigModel rm in RDSClient.Values)
                {
                    TcpCli tc = rm.TcpClient;
                    try
                    {
                        if (tc.IsConnected)
                        {
                            tc.SendData(new byte[] { });
                        }
                        else
                        {
                            LogHelper.WriteInfo("RDS服务(" + rm.RDSMark + ")，连接中断准备重连！" + tc.IP + ":" + tc.Port);
                            tc.Connect();
                            if (tc.IsConnected)
                                LogHelper.WriteInfo("RDS服务(" + rm.RDSMark + ")，重连成功！" + tc.IP + ":" + tc.Port);
                            else
                                LogHelper.WriteInfo("RDS服务(" + rm.RDSMark + ")，重连失败！" + tc.IP + ":" + tc.Port);
                        }
                    }
                    catch (Exception ex)
                    {


                        LogHelper.WriteError("RDS服务(" + rm.RDSMark + ")，重连失败！" + tc.IP + ":" + tc.Port, ex);

                    }

                }
                //处理超时未处理的 命令或命令执行结果
                List<string> TimeoutOrderAndResult = new List<string>();
                foreach (string key in OrderAndResultTime.Keys)
                {
                    DateTime dt = (DateTime)OrderAndResultTime[key];
                    if (dt.AddMinutes(10) < DateTime.Now)
                    {
                        TimeoutOrderAndResult.Add(key);
                    }
                }
                foreach (string key in TimeoutOrderAndResult)
                {
                    OrderAndResultTime.Remove(key);
                }
                if (TimeoutOrderAndResult.Count > 0)
                    LogHelper.WriteInfo("清除超时的命令，或未处理的命令结果，总计：" + TimeoutOrderAndResult.Count);

            }
        }
        public static void Init()
        {
            fswWatcher = new FileSystemWatcher(basepath);
            //获取或设置要监视的更改类型。
            fswWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fswWatcher.Filter = "*.xml";
            //文件或目錄變更時事件
            fswWatcher.Changed += new FileSystemEventHandler(fswWatcher_Changed);
            Loaddata();
            InitScoketClient();

            Thread th = new Thread(Manager);
            th.Start();
            LogHelper.WriteInfo("与RDS服务连接维护线程启动");
        }

        private static void Loaddata()
        {

            DataSet ds = new DataSet();
            ds.ReadXml(basepath + "RDSConfig.xml");
            dtRDSList = ds.Tables[0];
        }
        /// <summary>
        /// 文件或目錄變更時事件的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void fswWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "RDSConfig.xml")
            {
                LogHelper.WriteInfo("RDS配置文件RDSConfig.xml发生修改重新建立所有连接！");
                Loaddata();
                foreach (RDSConfigModel val in RDSClient.Values)
                {
                    TcpCli tc = val.TcpClient;
                    tc.Close();
                    tc = null;
                }
                RDSClient.Clear();
                InitScoketClient();

            }
        }



        //public static int GetCarOnlineStatus(string sysflag, long cid)
        //{
        //    if (AllCarsOnlineStatus.ContainsKey(sysflag))
        //    {
        //        Dictionary<long, int> cos = AllCarsOnlineStatus[sysflag];
        //        if (cos.ContainsKey(cid))
        //        {
        //            return cos[cid];
        //        }
        //    }
        //    return 2;
        //}

        public static List<long> GetAllOnlineCars(string sysflag)
        {
            List<long> cids = new List<long>();
            Dictionary<long, int> cos = AllCarsOnlineStatus[sysflag];
            foreach (long cid in cos.Keys)
            {
                if (cos[cid] == 1)
                {
                    cids.Add(cid);

                }
            }
            return cids;
        }
        /// <summary>
        /// 获取车辆在线状态
        /// </summary>
        /// <param name="sysflag"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public static List<CarsStatus> GetCarOnlineStatus(string sysflag, long[] cid)
        {
            List<CarsStatus> carsStatus_List = new List<CarsStatus>();
            Dictionary<long, int> cos = AllCarsOnlineStatus[sysflag];
            for (int i = 0; i < cid.Length; i++)
            {
                CarsStatus carsStatus = new CarsStatus();
                if (AllCarsOnlineStatus.ContainsKey(sysflag))
                {
                    if (cos.ContainsKey(cid[i]))
                    {
                        carsStatus.cid = cid[i];
                        carsStatus.status = cos[cid[i]];
                    }
                    else
                    {
                        carsStatus.cid = cid[i];
                        carsStatus.status = 2;
                    }
                }
                else
                {
                    carsStatus.cid = cid[i];
                    carsStatus.status = 2;
                }
                carsStatus_List.Add(carsStatus);
            }
            return carsStatus_List;
        }





        private static void SetCarOnlineStatus(string sysflag, long cid, int status)
        {
            if (AllCarsOnlineStatus.ContainsKey(sysflag))
            {
                Dictionary<long, int> cos = AllCarsOnlineStatus[sysflag];
                if (cos.ContainsKey(cid))
                {
                    cos[cid] = status;
                }
                else
                {
                    cos.Add(cid, status);
                }
            }

        }
        private static void initCarOnlineStatusMem(string[] sysflags)
        {
            foreach (string sysflag in sysflags)
            {
                if (!AllCarsOnlineStatus.ContainsKey(sysflag))
                {
                    Dictionary<long, int> cos = new Dictionary<long, int>();
                    AllCarsOnlineStatus.Add(sysflag, cos);
                }
            }
        }
        private static void InitScoketClient()
        {
            foreach (DataRow dr in dtRDSList.Rows)
            {
                RDSConfigModel rm = new RDSConfigModel();
                string sfs = dr["SysFlag"].ToString();
                rm.SysFlag = sfs.Split(',');
                initCarOnlineStatusMem(rm.SysFlag);
                string mincid = dr["MinCID"].ToString();
                string maxcid = dr["MaxCID"].ToString();
                long MinCID = 0, MaxCID = 0;
                long.TryParse(mincid, out MinCID);
                long.TryParse(maxcid, out MaxCID);
                rm.MinCID = MinCID;
                rm.MaxCID = MaxCID;
                rm.ID = dr["ID"].ToString();
                rm.IP = dr["IP"].ToString();
                rm.Port = dr["Port"].ToString();
                rm.RDSMark = dr["RDSMark"].ToString();
                rm.WCFUrl = dr["WCFUrl"].ToString();
                TcpCli tc = new TcpCli(rm.IP, int.Parse(rm.Port));
                tc.ConnectedServer += rm.EventConnectedServer;
                tc.DisConnectedServer += rm.EventDisConnectedServer;
                tc.ReceivedDatagram += new NetEvent(tc_ReceivedDatagram);
                try
                {
                    tc.Connect();
                    Thread.Sleep(4000);
                    if (tc.IsConnected)
                        rm.RunFlag = true;
                    else
                        rm.RunFlag = false;
                }
                catch
                {
                    rm.RunFlag = false;

                }


                rm.TcpClient = tc;

                RDSClient.Add(rm.ID, rm);

                if (rm.RunFlag)
                {
                    LogHelper.WriteInfo("初始化与RDS服务建立连接完成(" + rm.RDSMark + ")");
                }
                else
                {
                    LogHelper.WriteInfo("初始化与RDS服务建立连接失败(" + rm.RDSMark + ")，等待维护线程重连");
                }
            }
        }


        static void tc_ReceivedDatagram(object sender, NetEventArgs e)
        {
            //LogHelper.WriteInfo("接收到RDS推送数据包");
            List<byte[]> DataPakages = e.Client.Datagram;
            foreach (byte[] vdata in DataPakages)
            {
                byte[] recdata = vdata;
                if (recdata.Length < 28) return;
                byte[] sfdata = new byte[20];
                byte[] ciddata = new byte[8];
                byte[] data = new byte[recdata.Length - 20 - 8];
                Array.Copy(recdata, sfdata, 20);
                Array.Copy(recdata, 20, ciddata, 0, 8);
                Array.Copy(recdata, 28, data, 0, data.Length);

                string sysflag = Encoding.ASCII.GetString(sfdata);
                sysflag = sysflag.Replace("\0", "");
                long carid = System.BitConverter.ToInt64(ciddata, 0);
                QMDPartnerPackage datapag = null;
                if (checkPackage(data, ref datapag))
                {
                    //车辆在线通知信息包
                    if (datapag.dwOperation1 == 10101 && datapag.dwOperation2 == 20202)
                    {
                        MemoryStream msc = new MemoryStream(datapag.byteContext);
                        if (datapag.byteContext.Length > 0)
                        {
                            Dictionary<long, int> reqdata = ProtoBuf.Serializer.Deserialize<Dictionary<long, int>>(msc);
                            foreach (long cid in reqdata.Keys)
                            {
                                SetCarOnlineStatus(sysflag, cid, reqdata[cid]);
                            }
                        }
                    }

                    string key = sysflag + "|" + carid + "|" + datapag.dwPackageType + "|" + datapag.dwOperation1 + "|";
                    string extflag = "";
                    if (datapag.dwOperation1 == 41)//特殊处理行驶记录仪查询同时加入命令字做为指令细分
                    {
                        MemoryStream msc = new MemoryStream(datapag.byteContext);
                        CTS_DriveRecordDataUpResponse reqdata = ProtoBuf.Serializer.Deserialize<CTS_DriveRecordDataUpResponse>(msc);
                        key += reqdata.nCMD.ToString();
                        extflag = reqdata.nCMD.ToString();
                    }
                    else if (datapag.dwOperation1 == 99)//透传指令 下发加入透传id作为细分指令
                    {
                        MemoryStream msc = new MemoryStream(datapag.byteContext);
                        CTS_TransmissionProtocol reqdata = ProtoBuf.Serializer.Deserialize<CTS_TransmissionProtocol>(msc);
                        extflag = reqdata.nTransID.ToString("X2");
                        if (reqdata.nTransID == 0x0f01)//解放消贷控制指令特殊处理，因为上行和下行nTransID不一致。此处填充上行id方便回指令结果
                        {
                            extflag += reqdata.sData[0].ToString("X2") + reqdata.sData[1].ToString("X2");

                        }
                        key += extflag;
                    }
                    if (UserOrderTable.ContainsKey(key))
                    {
                        string tokens = UserOrderTable[key].ToString();
                        OrderResultModel orm = new OrderResultModel();
                        orm.cid = carid;
                        orm.dwOperation1 = datapag.dwOperation1;
                        orm.dwOperation2 = datapag.dwOperation2;
                        orm.dwPackageType = datapag.dwPackageType;
                        orm.ResData = datapag;
                        orm.sysflag = sysflag;
                        orm.resTime = DateTime.Now;
                        orm.extflag = extflag;
                        List<OrderResultModel> ormlist = new List<OrderResultModel>();
                        ormlist.Add(orm);
                        string[] tokenarray = tokens.Split(',');
                        foreach (string token in tokenarray)
                        {
                            //命令响应加入hash表
                            if (OrderResultTable.ContainsKey(token))
                            {
                                List<OrderResultModel> oldormlist = (List<OrderResultModel>)OrderResultTable[token];
                                oldormlist.Add(orm);
                                OrderResultTable[token] = oldormlist;
                            }
                            else
                            {
                                OrderResultTable.Add(token, ormlist);
                            }
                            //命令响应时间加入hash表
                            if (OrderAndResultTime.ContainsKey(token))
                            {
                                OrderAndResultTime[token] = DateTime.Now;
                            }
                            else
                            {
                                OrderAndResultTime.Add(token, DateTime.Now);
                            }
                            LogHelper.WriteInfo(sysflag + "接收指令执行结果：" + key);
                        }
                        UserOrderTable.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// 数据包晚自习检查
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rMsg"></param>
        /// <returns></returns>
        private static Boolean checkPackage(byte[] data, ref QMDPartnerPackage rMsg)
        {
            Boolean isFlag = false;
            try
            {
                //处理返回的数据包        
                MemoryStream ms = new MemoryStream(data);
                rMsg = ProtoBuf.Serializer.Deserialize<QMDPartnerPackage>(ms);
                isFlag = true;
            }
            catch (Exception ex)
            {
                isFlag = false;
            }
            return isFlag;
        }
        public static bool SendMsg(string sysflag, string token, long cid, QMDPartnerPackage package)
        {
            RDSConfigModel rcm = GetRDS(sysflag, cid);
            if (rcm == null) return false;
            TcpCli tc = rcm.TcpClient;
            try
            {
                if (tc.IsConnected)
                {


                    byte[] sfdata = Encoding.ASCII.GetBytes(sysflag);
                    byte[] ciddata = System.BitConverter.GetBytes(cid);
                    MemoryStream ms = new MemoryStream();
                    ProtoBuf.Serializer.Serialize(ms, package);
                    byte[] bytes = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(bytes, 0, bytes.Length);
                    ms.Close();
                    //将系统标示加入数据头部长度20字节,CID数据 8个字节
                    byte[] data = new byte[bytes.Length + 20 + 8];
                    Array.Copy(sfdata, data, sfdata.Length);
                    Array.Copy(ciddata, 0, data, 20, 8);
                    Array.Copy(bytes, 0, data, 28, bytes.Length);
                    tc.SendData(data);
                    if (package.dwPackageType == 30 && (package.dwOperation1 == 10 || package.dwOperation1 == 33 || package.dwOperation1 == 41 || package.dwOperation1 == 99))//需要回传数据的记录指令，以便于应答 时确认发送者
                    {
                        if (package.dwOperation1 == 41)//行驶记录仪参数查询特殊处理同时加入命令字做为指令细分
                        {
                            MemoryStream msc = new MemoryStream(package.byteContext);
                            CTS_DriveRecordDataCollectionRequest reqdata = ProtoBuf.Serializer.Deserialize<CTS_DriveRecordDataCollectionRequest>(msc);
                            AddOrder(sysflag, token, cid, package.dwPackageType, package.dwOperation1, reqdata.nCMD.ToString());
                        }
                        else if (package.dwOperation1 == 99)//透传指令 下发加入透传id作为细分指令
                        {
                            MemoryStream msc = new MemoryStream(package.byteContext);
                            CTS_TransmissionProtocol reqdata = ProtoBuf.Serializer.Deserialize<CTS_TransmissionProtocol>(msc);
                            string extflag = reqdata.nTransID.ToString("X2");
                            if (reqdata.nTransID == 0x8F02)//解放消贷控制指令特殊处理，因为上行和下行nTransID不一致。此处填充上行id方便回指令结果
                            {
                                int respnTransID = reqdata.nTransID == 0x8F02 ? 0x0f01 : reqdata.nTransID;
                                extflag = respnTransID.ToString("X2") + reqdata.sData[0].ToString("X2") + reqdata.sData[1].ToString("X2");
                                AddOrder(sysflag, token, cid, package.dwPackageType, package.dwOperation1, extflag);
                            }

                        }
                        else
                        {
                            AddOrder(sysflag, token, cid, package.dwPackageType, package.dwOperation1);
                        }
                    }
                    return true;
                }
                else
                {
                    LogHelper.WriteInfo(sysflag + "RDS连接中断发送数据失败(" + string.Join(",", rcm.SysFlag) + ")");
                    if (ReConnect(tc))
                    {
                        LogHelper.WriteInfo(sysflag + "RDS服务(" + rcm.RDSMark + ")重连成功发送数据！");
                        return SendMsg(sysflag, token, cid, package);
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(sysflag + "程序异常发送数据失败！ ", ex);
                return false;
            }

        }
        private static bool ReConnect(TcpCli tc)
        {
            try
            {
                tc.Connect();
                Thread.Sleep(4000);
                if (tc.IsConnected)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public static RDSConfigModel GetRDS(string SysFlag, long CID = 0)
        {
            DataRow[] drs = dtRDSList.Select("SysFlag like '%" + SysFlag + "%'");
            if (drs.Length > 0)
            {
                string ID = drs[0]["ID"].ToString();
                return (RDSConfigModel)RDSClient[ID];
            }
            else
                return null;
        }


    }
    public class OrderResultModel
    {
        public OrderResultModel Clone()
        {

            OrderResultModel objectReturn = null;
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    objectReturn = (OrderResultModel)formatter.Deserialize(stream);
                }
                catch (Exception e)
                {

                }
            }
            return objectReturn;
        }
        public long cid;
        public string sysflag;
        public uint dwPackageType;
        public uint dwOperation1;
        public uint dwOperation2;
        public QMDPartnerPackage ResData;
        public DateTime resTime;
        public string extflag;

    }
    public class RDSConfigModel
    {
        public string ID;
        public string RDSMark;
        public string IP;
        public string Port;
        public string[] SysFlag;
        public string WCFUrl;
        public long MinCID = 0;
        public long MaxCID = 0;
        public bool RunFlag = true;
        public TcpCli TcpClient;

        public void EventConnectedServer(object sender, NetEventArgs e)
        {
            RunFlag = true;
        }
        public void EventDisConnectedServer(object sender, NetEventArgs e)
        {
            RunFlag = false;
            TcpCli tc = (TcpCli)sender;
            tc.Connect();
        }
    }

    public class CarsStatus
    {
        public long cid;
        public int status;
    }
}