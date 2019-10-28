using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common;
using CMCS.Common.Entities.Sys;
using CMCS.DumblyConcealer.Tasks.CarSynchronous.Enums;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Fuel;
using CMCS.DumblyConcealer.Enums;
using CMCS.Common.DAO;
using System.Data;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using CMCS.DumblyConcealer.Tasks.DataHandler.Entities;


namespace CMCS.DumblyConcealer.Tasks.CarSynchronous
{
    /// <summary>
    /// 综合事件处理
    /// </summary>
    public class DataHandlerDAO
    {
        private static DataHandlerDAO instance;

        public static DataHandlerDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new DataHandlerDAO();
            }
            return instance;
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();

        private DataHandlerDAO()
        { }

        /// <summary>
        /// 开始处理
        /// </summary> 
        /// <returns></returns>
        public void Start(Action<string, eOutputType> output)
        {
            foreach (CmcsWaitForHandleEvent item in commonDAO.SelfDber.Entities<CmcsWaitForHandleEvent>("where DataFlag=0"))
            {
                bool isSuccess = false;

                eEventCode eventCode;
                bool a = Enum.TryParse<eEventCode>(item.EventCode, out eventCode);
                if (!Enum.TryParse<eEventCode>(item.EventCode, out eventCode)) continue;

                switch (eventCode)
                {
                    case eEventCode.汽车智能化_同步入厂煤运输记录到批次:

                        if (SyncToBatch(output, item.ObjectId))
                        {
                            isSuccess = true;

                            output(string.Format("事件：{0}  ObjectId：{1}", eEventCode.汽车智能化_同步入厂煤运输记录到批次.ToString(), item.ObjectId), eOutputType.Normal);
                        }

                        break;
                }

                if (isSuccess)
                {
                    item.DataFlag = 1;
                    commonDAO.SelfDber.Update(item);
                }
            }
        }

        /// <summary>
        /// 将汽车入厂煤运输记录同步到批次明细中
        /// </summary>
        /// <param name="transportId">汽车入厂煤运输记录Id</param>
        /// <returns></returns>
        private bool SyncToBatch(Action<string, eOutputType> output, string transportId)
        {
            bool res = false;
            CmcsBuyFuelTransport transport = commonDAO.SelfDber.Get<CmcsBuyFuelTransport>(transportId);
            if (transport == null) return false;

            CmcsInFactoryBatch batch = commonDAO.SelfDber.Get<CmcsInFactoryBatch>(transport.InFactoryBatchId);
            if (batch == null) return false;

            CmcsTransport truck = commonDAO.SelfDber.Entity<CmcsTransport>("where InFactoryBatchId=:InFactoryBatchId and PKID=:PKID", new { InFactoryBatchId = batch.Id, PKID = transport.Id });
            if (truck != null)
            {
                truck.TransportNo = transport.CarNumber;
                truck.OperDate = DateTime.Now;
                truck.TransportType = "汽车";
                truck.InfactoryTime = transport.CreateDate;
                truck.ArriveDate = transport.GrossTime;
                truck.TareDate = transport.TareTime;
                truck.DisBoardTime = transport.UploadTime;
                truck.OutFactoryTime = transport.OutFactoryTime;
                truck.TicketWeight = transport.TicketWeight;
                truck.GrossWeight = transport.GrossWeight;
                truck.SkinWeight = transport.TareWeight;
                truck.StandardWeight = transport.SuttleWeight;
                truck.KgWeight = transport.DeductWeight;
                truck.CheckQty = transport.SuttleWeight - transport.DeductWeight;
                truck.MarginWeight = transport.SuttleWeight - transport.DeductWeight - transport.TicketWeight;
                truck.InFactoryBatchId = transport.InFactoryBatchId;
                truck.PKID = transport.Id;
                truck.MeasureMan = "汽车智能化";
                res = commonDAO.SelfDber.Update(truck) > 0;
            }
            else
            {
                truck = new CmcsTransport()
                {
                    TransportNo = transport.CarNumber,
                    OperDate = DateTime.Now,
                    TransportType = "汽车",
                    InfactoryTime = transport.CreateDate,
                    ArriveDate = transport.GrossTime,
                    TareDate = transport.TareTime,
                    DisBoardTime = transport.UploadTime,
                    OutFactoryTime = transport.OutFactoryTime,
                    TicketWeight = transport.TicketWeight,
                    GrossWeight = transport.GrossWeight,
                    SkinWeight = transport.TareWeight,
                    StandardWeight = transport.SuttleWeight,
                    KgWeight = transport.DeductWeight,
                    CheckQty = transport.SuttleWeight - transport.DeductWeight,
                    MarginWeight = transport.SuttleWeight - transport.DeductWeight - transport.TicketWeight,
                    InFactoryBatchId = transport.InFactoryBatchId,
                    PKID = transport.Id,
                    MeasureMan = "汽车智能化"
                };

                res = commonDAO.SelfDber.Insert(truck) > 0;
            }

            if (res)
            {
                // 更新批次的量 

                List<CmcsTransport> trucks = commonDAO.SelfDber.Entities<CmcsTransport>("where InFactoryBatchId=:InFactoryBatchId", new { InFactoryBatchId = batch.Id });
                Decimal carNums = trucks.Count;
                Decimal ticketWeight = trucks.Sum(a => a.TicketWeight);
                Decimal grossWeight = trucks.Sum(a => a.GrossWeight);
                Decimal skinWeight = trucks.Sum(a => a.SkinWeight);
                Decimal standardWeight = trucks.Sum(a => a.StandardWeight);
                Decimal marginQty = trucks.Sum(a => a.MarginWeight);
                Decimal kgWeight = trucks.Sum(a => a.KgWeight);
                Decimal checkQty = trucks.Sum(a => a.CheckQty);
                Decimal railLost = trucks.Sum(a => a.RailLost);

                commonDAO.SelfDber.Execute("update " + DapperDber.Util.EntityReflectionUtil.GetTableName<CmcsInFactoryBatch>() + " set IsCheck=0,TICKETQTY=:TICKETQTY,SUTTLEWEIGHT=:SUTTLEWEIGHT,KGWEIGHT=:KGWEIGHT,CHECKQTY=:CHECKQTY,RAILLOST=:RAILLOST,TRANSPORTNUMBER=:TRANSPORTNUMBER,MARGINQTY=:MARGINQTY where Id=:Id",
                    new
                    {
                        Id = batch.Id,
                        TICKETQTY = ticketWeight,
                        SUTTLEWEIGHT = standardWeight,
                        KGWEIGHT = kgWeight,
                        CHECKQTY = checkQty,
                        RAILLOST = railLost,
                        TRANSPORTNUMBER = carNums,
                        MARGINQTY = marginQty,
                    });
            }

            return res;
        }



        /// <summary>
        /// 同步首页总体信息 
        /// </summary>
        /// <param name="output"></param>
        public void SyncStorageTotal(Action<string, eOutputType> output)
        {
            output("开始同步首页总体信息", eOutputType.Normal);
            string sql = string.Empty;
            Int32 CarCount = 0;//当日入场车数
            Int32 SamplCarCount = 0;//机械采样机需要采样总车数
            Int32 SamplCarCount_YC = 0;//机械采样机已采样车数
            Int32 CarCount_GZCH = 0;//过重车衡车数
            Int32 CarCount_GQCH = 0;//过轻车衡车数
            Decimal checkqty = 0;//汽车当日来煤量
            #region 当日入场车数
            sql = "select count(*) from cmcstbbuyfueltransport t where to_char(t.infactorytime,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            DataTable dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                CarCount = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_汽车进厂车数", CarCount.ToString());
            #endregion


            #region 机械采样机已采车数
            sql = "select count(*) from cmcstbbuyfueltransport t where  t.samplingtype='机械采样' and  to_char(t.infactorytime,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "'  and to_char(t.samplingtime,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                SamplCarCount_YC = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_已采样车数", SamplCarCount_YC.ToString());
            #endregion

            #region 机械采样机需要采样总车数
            sql = "select count(*) from cmcstbbuyfueltransport t where  t.samplingtype='机械采样' and to_char(t.infactorytime,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                SamplCarCount = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_待采样车数", (SamplCarCount - SamplCarCount_YC).ToString());
            #endregion


            #region 过重车衡车数
            sql = "select count(*) from  cmcstbbuyfueltransport t where to_char(t.infactorytime,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "'  and t.stepname !=to_char('入厂') and t.stepname !=to_char('采样')";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                CarCount_GZCH = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_过重车数", CarCount_GZCH.ToString());
            #endregion


            #region 过轻车衡车数
            sql = "select count(*) from  cmcstbbuyfueltransport t where to_char(t.infactorytime,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "'  and t.stepname ='轻车'";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                CarCount_GQCH = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_过轻车数", CarCount_GQCH.ToString());
            #endregion

            #region 待卸车数
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_待卸车数", (CarCount_GZCH - CarCount_GQCH).ToString());
            #endregion

            #region 出厂车数
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_出厂车数", CarCount_GQCH.ToString());
            #endregion

            #region 入厂指标汽车煤量
            sql = "select nvl(sum(t.checkqty),0) as checkqty from fultbinfactorybatch t where to_char(t.factarrivedate,'yyyy-MM-dd')='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and t.batchtype='汽车'";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                checkqty = Decimal.Parse(dt.Rows[0][0].ToString() ?? "0");
            }

            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "总体信息_汽车来煤量", checkqty.ToString());
            #endregion

            #region 采样车数
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "采制化存指标_采样数", SamplCarCount_YC.ToString());
            #endregion

            #region 制样数
            int Makecount = 0;
            sql = "select count(*) from cmcstbrcmake t where t.makestarttime like '%" + DateTime.Now.ToString("yyyy-MM-dd") + "%'";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                Makecount = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "采制化存指标_制样数", (Makecount * 8).ToString());
            #endregion


            #region 制样数
            int AssayCount = 0;
            sql = "select count(*) from fultbrchyassay t where t.assaydate like '%" + DateTime.Now.ToString("yyyy-MM-dd") + "%'  and t.status='已审核'";
            dt = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                AssayCount = int.Parse(dt.Rows[0][0].ToString() ?? "0");
            }
            CommonDAO.GetInstance().SetSignalDataValue(GlobalVars.MachineCode_HomePage_1, "采制化存指标_化验数", AssayCount.ToString());
            #endregion

            output("结束同步首页总体信息", eOutputType.Normal);
        }


        #region 调用webAPI


        public void PlatAccessPointListByRegionIds(Action<string, eOutputType> output)
        {
            string res = SubEvent2();//实时获取燃料质检下面所有门禁信息数据写入小程序参数配置
            output(string.Format("同步同步燃料质检门禁点 {0} 个（集中管控 > 第三方）", res), eOutputType.Normal);
        }


        public void getPlatAcsHistory(Action<string, eOutputType> output)
        {
            string res = SubEvent();//实时获取燃料质检下面所有门禁信息数据写入小程序参数配置
            output(string.Format("同步门禁信息 {0} 条（集中管控 > 第三方）", res), eOutputType.Normal);
        }




        //入参转Json
        public string GetParamsJson(Dictionary<string, object> jsondataMap)
        {
            string jsonStr = "";
            jsonStr = SerializeObject(jsondataMap);
            return jsonStr;
        }

        public static string SerializeObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        //获取Token
        public string GetToken(string url, string jsondata, string secret)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(url + jsondata + secret); //将字符串解析成字节数组，随便按照哪种解析格式都行
            MD5 md5 = MD5.Create();  //使用MD5这个抽象类的Creat()方法创建一个虚拟的MD5类的对象。
            byte[] bufferNew = md5.ComputeHash(buffer); //使用MD5实例的ComputerHash()方法处理字节数组。
            string strNew = null;
            for (int i = 0; i < bufferNew.Length; i++)
            {
                strNew += bufferNew[i].ToString("x2");  //对bufferNew字节数组中的每个元素进行十六进制转换然后拼接成strNew字符串
            }
            return strNew.ToUpper();
        }
        //获取当时时间戳
        public static long GetTimestamp(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }



        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        //public static DateTime TimeStampToDateTime(string timeStamp)
        //{
        //    DateTime _dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
        //    return _dtStart.AddMilliseconds(Convert.ToInt64(timeStamp));
        //}


        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        //public static DateTime TimeStampToDateTime(string timeStamp)
        //{
        //    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 8, 0, 0));

        //    long lTime = long.Parse(timeStamp);
        //    TimeSpan toNow = new TimeSpan(lTime);
        //    return dtStart.Add(toNow);
        //}

        private DateTime TimeStampToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }



        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }
        public string SubEvent()
        {
            string IP = CommonDAO.GetInstance().GetCommonAppletConfigString("门禁厂家IP地址");
            string appkey = CommonDAO.GetInstance().GetCommonAppletConfigString("门禁系统appkey");
            string secret = CommonDAO.GetInstance().GetCommonAppletConfigString("门禁系统secret");
            string doorSyscodes = CommonDAO.GetInstance().GetCommonAppletConfigString("燃料质检门禁");
            string url = "/webapi/service/acs/getPlatAcsHistoryEventList";

            Dictionary<string, object> paramMaps = new Dictionary<string, object>();
            paramMaps.Add("appkey", appkey);
            paramMaps.Add("time", GetTimestamp(DateTime.Now));
            paramMaps.Add("pageNo", 1);
            paramMaps.Add("pageSize", 999);
            paramMaps.Add("startTime", GetTimestamp(DateTime.Now.Date.AddHours(-6)));
            paramMaps.Add("endTime", GetTimestamp(DateTime.Now));
            paramMaps.Add("doorSyscodes", doorSyscodes);


            string jsondata = GetParamsJson(paramMaps);
            string token = GetToken(url, jsondata, secret);
            try
            {

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(IP + url + "?token=" + token);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "Post";

                //请求参数转为字节流
                byte[] postBytes = Encoding.UTF8.GetBytes(jsondata);

                // this is important - make sure you specify type this way
                request.ContentType = "application/json; charset=UTF-8";
                request.Accept = "application/json";
                request.ContentLength = postBytes.Length;
                //request.CookieContainer = Cookies;
                //request.UserAgent = currentUserAgent;
                Stream requestStream = request.GetRequestStream();

                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();

                }
                Result res = DeserializeJsonToObject<Result>(result);
                string dataStr = "";
                //DoorData res = DeserializeJsonToObject<DoorData>(result);
                int nub = 0;
                if (res.ErrorCode == 0)
                {
                    dataStr = res.Data.ToString();
                    Dictionary<string, object> dataMap = DeserializeJsonToObject<Dictionary<string, object>>(res.Data.ToString());

                    List<DoorData> DoorDatas = DeserializeJsonToList<DoorData>(dataMap["rows"].ToString());

                    DoorDatas = DoorDatas.Where(a => a.eventName == "合法卡比对通过").ToList();
                    foreach (var item in DoorDatas)
                    {
                        CmcstbGuardinfo entity = commonDAO.SelfDber.Get<CmcstbGuardinfo>(item.acEventId.ToString());
                        if (entity == null)
                        {
                            CmcstbGuardinfo Guardinfo = new CmcstbGuardinfo();
                            Guardinfo.Id = item.acEventId.ToString();
                            Guardinfo.F_READDATE = TimeStampToDateTime((Convert.ToInt64(item.eventTime) / 1000).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            Guardinfo.F_CONSUMERID = item.personId.ToString();
                            Guardinfo.F_CONSUMERNAME = item.personName.ToString();
                            Guardinfo.F_READERID = item.doorId.ToString();
                            Guardinfo.F_READERNAME = item.doorName.ToString();
                            if (commonDAO.SelfDber.Insert(Guardinfo) > 0)
                            {
                                nub++;
                            }
                        }
                    }

                }
                return nub.ToString(); ;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 实时获取燃料质检下面所有门禁信息数据写入小程序参数配置
        /// </summary>
        public string SubEvent2()
        {

            string IP = CommonDAO.GetInstance().GetCommonAppletConfigString("门禁厂家IP地址");
            string appkey = CommonDAO.GetInstance().GetCommonAppletConfigString("门禁系统appkey");
            string secret = CommonDAO.GetInstance().GetCommonAppletConfigString("门禁系统secret");
            string url = "/webapi/service/acs/getPlatAccessPointListByRegionIds";
            long updateTime = GetTimestamp(DateTime.Now);
            string jsondata = "regionIds=42&appkey=" + appkey + "&time=" + GetTimestamp(DateTime.Now);
            string token = GetToken(url, jsondata, secret);
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(IP + url + "?" + jsondata + "&token=" + token);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "Get";

                //请求参数转为字节流
                byte[] postBytes = Encoding.UTF8.GetBytes(jsondata);

                // this is important - make sure you specify type this way
                request.ContentType = "application/json; charset=UTF-8";
                request.Accept = "application/json";


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();

                }
                Result res = DeserializeJsonToObject<Result>(result);
                string dataStr = "";

                int count = 0;
                if (res.ErrorCode == 0)
                {
                    dataStr = res.Data.ToString();
                    //Dictionary<string, object> dataMap = DeserializeJsonToObject<Dictionary<string, object>>(res.Data.ToString());
                    List<sysCodes> sysCodeData = DeserializeJsonToList<sysCodes>(res.Data.ToString());
                    string sysCodes = "";
                    foreach (var item in sysCodeData)
                    {
                        sysCodes += item.sysCode + ",";
                        count++;
                    }
                    commonDAO.SetAppletConfig(GlobalVars.CommonAppletConfigName, "燃料质检门禁", sysCodes.TrimEnd(','));
                }
                return count.ToString();

            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        #endregion
    }

    [Serializable]
    public class DoorData
    {
        public string acEventId { get; set; }//事件Id，唯一标识
        public string eventTime { get; set; }//操作时间
        public string personId { get; set; }//人员ID
        public string personName { get; set; }//人员名称
        public string eventName { get; set; }//进出
        public string doorId { get; set; }//门名称
        public string doorName { get; set; }//门名称
    }

    [Serializable]
    public class sysCodes
    {
        public string sysCode { get; set; }//全局唯一标识
    }
}
