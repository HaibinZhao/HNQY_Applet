using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
//
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Enums;
using CMCS.DumblyConcealer.Enums;
using CMCS.Common.DAO;
using CMCS.Common;
using CMCS.Common.Enums;
using CMCS.Common.Enums.AutoCupboard;
using CMCS.Common.Entities.AutoCupboard;
using CMCS.Common.Enums.PneumaticTransfer;
using CMCS.Common.Entities.Inf;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS
{
    public class PneumaticTransfer_XMJS_DAO
    {
        private static PneumaticTransfer_XMJS_DAO instance;
        private static String MachineCode = GlobalVars.InterfaceType_XMJS_QD;
        AutoCupboardDAO autoCupboardDAO = AutoCupboardDAO.GetInstance();
        CommonDAO commonDAO = CommonDAO.GetInstance();
        public static PneumaticTransfer_XMJS_DAO GetInstance()
        {
            if (instance == null)
            {
                instance = new PneumaticTransfer_XMJS_DAO();
            }
            return instance;
        }

        private PneumaticTransfer_XMJS_DAO()
        { }


        /// <summary>
        /// 检测气动系统是否处于空闲状态
        /// </summary>
        public bool CheckFree()
        {
            InfQDStatus infqdstatuse = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDStatus>();
            return infqdstatuse != null && infqdstatuse.SamReady == 3;
        }

        /// <summary>
        /// 检测气动站点状态 触发气动的起始点  制样机/人工制样室 由该处触发
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int CheckSampleState(Action<string, eOutputType> output)
        {
            int res = 0;
            foreach (InfQDSampleStatus item in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<InfQDSampleStatus>("where SampleId is not null and SampleId !='' and IsRead=0 order by AddTime"))
            {
                output("气动站点检测到样瓶 等待执行...", eOutputType.Normal);
                while (!CheckFree())
                {
                    output("气动检测到有样瓶就位，但气动系统不处于就绪状态 等待中...", eOutputType.Important);
                    Thread.Sleep(2000);
                }

                string makeType = autoCupboardDAO.GetMakeTypeByMakeCode(item.SampleId);
                string[] assayType = CommonDAO.GetInstance().GetCommonAppletConfigString("气动传送至化验室的制样类型").Split('|');
                int zdBh=GetLisIndexByDeviceId(item.DeviceId);//获取起始站编号
                if (assayType.Contains(makeType) && zdBh!=3)//气动至化验室
                {
                    if (SendQDCmd(zdBh, (int)eDevices.化验室取样站, item.SampleId))
                    {
                        res++;
                        output("气动命令已发送", eOutputType.Important);
                        item.IsRead = 1;
                        DcDbers.GetInstance().PneumaticTransfer_Dber.Update(item);
                    }
                }
                else if (zdBh == 3 && item.Dest==2)//当起始站点是化验室,结束站点值等于2时候发送弃样
                {
                    if (SendQDCmd(zdBh, (int)eDevices.弃样站, item.SampleId))
                    {
                        item.IsRead = 1;
                        output("弃样的气动命令已发送", eOutputType.Normal);
                        DcDbers.GetInstance().PneumaticTransfer_Dber.Update(item);
                    }
                    
                }
                else//至存样柜
                {
                    string cYG = autoCupboardDAO.GetCYGMachineCode();
                    while (string.IsNullOrEmpty(cYG))
                    {
                        cYG = autoCupboardDAO.GetCYGMachineCode();
                        output("无存样柜可分配 等待中...", eOutputType.Important);
                        Thread.Sleep(10000);
                    }
                    if (autoCupboardDAO.SaveAutoCupboardCmd(item.SampleId, cYG, eCZPLX.存样))
                        output("存样命令已发送", eOutputType.Important);
                    while (!autoCupboardDAO.CheckTPIsReady(cYG))
                    {
                        if (autoCupboardDAO.CheckCYGIsError(cYG))
                        {
                            output("存样柜故障，本次命令取消", eOutputType.Important);
                            goto Stop;
                        }
                        output("等待托盘到位...", eOutputType.Important);
                        Thread.Sleep(10000);
                    }
                    if (SendQDCmd(GetLisIndexByDeviceId(item.DeviceId), GetLisIndexByMachineCode(cYG), item.SampleId))
                    {
                        res++;
                        output("气动命令已发送", eOutputType.Important);
                        item.IsRead = 1;
                        DcDbers.GetInstance().PneumaticTransfer_Dber.Update(item);
                    }
                Stop: break;
                }
            }
            return res;
        }

        /// <summary>
        /// 检测存样柜命令 取样 弃样 由该处触发
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int CheckCYGCmd(Action<string, eOutputType> output)
        {
            int res = 0;
            foreach (InfCYGControlCMD item in Dbers.GetInstance().SelfDber.Entities<InfCYGControlCMD>("where OperType!=:OperType and DataFlag=3 and trunc(CreateDate)=trunc(sysdate) order by CreateDate desc", new { OperType = eCZPLX.存样.ToString(), ResultCode = eEquInfCmdResultCode.成功.ToString() }))
            {
                if (item.OperType == eCZPLX.取样_气动口.ToString())
                {
                    while (!CheckFree())
                    {
                        output("检测到取样命令，但气动系统不处于就绪状态 等待中...", eOutputType.Important);
                        Thread.Sleep(2000);
                    }
                    while (!autoCupboardDAO.CheckTPIsReady(item.MachineCode))
                    {
                        if (autoCupboardDAO.CheckCYGIsError(item.MachineCode))
                        {
                            output("存样柜故障，本次命令取消", eOutputType.Important);
                            goto Stop;
                        }
                        output("等待托盘到位...", eOutputType.Important);
                        Thread.Sleep(10000);
                    }
                    if (SendQDCmd(GetLisIndexByMachineCode(item.MachineCode), (int)eDevices.化验室取样站, item.CodeNumber))
                    {
                        item.DataFlag = 2;
                        output("取样至化验室的气动命令已发送", eOutputType.Normal);
                        res += Dbers.GetInstance().SelfDber.Update(item);
                    }
                Stop: break;
                }
                else if (item.OperType == eCZPLX.弃样.ToString())
                {
                    while (!CheckFree())
                    {
                        output("检测到弃样命令，但气动系统不处于就绪状态 等待中...", eOutputType.Important);
                        Thread.Sleep(2000);
                    }
                    while (!autoCupboardDAO.CheckTPIsReady(item.MachineCode))
                    {
                        if (autoCupboardDAO.CheckCYGIsError(item.MachineCode))
                        {
                            output("存样柜故障，本次命令取消", eOutputType.Important);
                            goto Stop;
                        }
                        output("等待托盘到位...", eOutputType.Important);
                        Thread.Sleep(2000);
                    }
                    if (SendQDCmd(GetLisIndexByMachineCode(item.MachineCode), (int)eDevices.弃样站, item.CodeNumber))
                    {
                        item.DataFlag = 2;
                        output("弃样的气动命令已发送", eOutputType.Normal);
                        res += Dbers.GetInstance().SelfDber.Update(item);
                    }
                Stop: break;
                }

                //res += Dbers.GetInstance().SelfDber.Update(item);
            }

            output(string.Format("处理取样、弃样{0}条", res), eOutputType.Normal);
            return res;
        }

        /// <summary>
        /// 发送气动命令
        /// </summary>
        /// <param name="startDeviceId">起始站点编号</param>
        /// <param name="endDeviceId">终点站点编号</param>
        /// <returns></returns>
        public bool SendQDCmd(int startDeviceId, int endDeviceId, string sampleId)
        {
            InfQDBill infqdbill = new InfQDBill();
            infqdbill.OpStart = startDeviceId;
            infqdbill.OpEnd = endDeviceId;
            infqdbill.Operator_Code = 501;
            infqdbill.SampleId = sampleId;
            infqdbill.Send_Time = DateTime.Now;
            infqdbill.DataStatus = 0;
            infqdbill.readState = 0;
            return DcDbers.GetInstance().PneumaticTransfer_Dber.Insert(infqdbill) > 0;
        }

        /// <summary>
        /// 同步气动传输结果至存样柜命令
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SyncPnumResultToCYGCmd(Action<string, eOutputType> output)
        {
            int res = 0;
            foreach (InfQDBill item in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<InfQDBill>("where readState=0 and (DataStatus=1 or DataStatus=2) order by Send_Time"))
            {
                InfCYGControlCMD cmd = commonDAO.SelfDber.Entity<InfCYGControlCMD>("where CodeNumber=:CodeNumber and DataFlag=2 order by CreateDate desc", new { CodeNumber = item.SampleId });
                if (cmd != null)
                {
                    if (item.DataStatus == 1)
                        cmd.ResultCode = eEquInfCYGCmdResultCode.气动执行成功.ToString();
                    if (item.DataStatus == 2)
                        cmd.ResultCode = eEquInfCYGCmdResultCode.气动执行失败.ToString();
                    cmd.DataFlag = 4;
                    commonDAO.SelfDber.Update(cmd);
                }
                item.readState = 1;
                if (DcDbers.GetInstance().PneumaticTransfer_Dber.Update(item) > 0)
                {
                    res++;
                }
            }
            return res;
        }

        /// <summary>
        /// 根据站点名称获取站点编号
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public int GetLisIndexByDeviceName(string deviceName)
        {
            InfQDDeviceCode device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where DeviceName=@DeviceName", new { DeviceName = deviceName });
            if (device != null)
                return device.ListIndex;
            return 0;
        }

        /// <summary>
        /// 根据站点Id获取站点编号
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public int GetLisIndexByDeviceId(int deviceId)
        {
            InfQDDeviceCode device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where DeviceId=@DeviceId", new { DeviceId = deviceId });
            if (device != null)
                return device.ListIndex;
            return 0;
        }

        /// <summary>
        /// 根据站点站点编号获取站点名称
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public string GetDeviceNameByListIndex(decimal ListIndex)
        {
            InfQDDeviceCode device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where ListIndex=@ListIndex", new { ListIndex = ListIndex });
            if (device != null)
                return device.DeviceName;
            return "";
        }

        /// <summary>
        /// 根据集控设备获取气动站点设备编号
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public int GetLisIndexByMachineCode(string machineCode)
        {
            int deviceId = 0;
            InfQDDeviceCode Device = null;
            if (machineCode == GlobalVars.MachineCode_CYG1)
            {
                Device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where DeviceName='存查柜1(BB)'");
            }
            else if (machineCode == GlobalVars.MachineCode_CYG2)
                Device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where DeviceName='存查柜2(BC)'");
            else if (machineCode == "化验室")
                Device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where DeviceName='取样站(B0)'");
            else if (machineCode == "弃样站")
                Device = DcDbers.GetInstance().PneumaticTransfer_Dber.Entity<InfQDDeviceCode>("where DeviceName='弃样站(B1)'");
            if (Device != null)
                deviceId = Device.ListIndex;
            return deviceId;
        }



        /// <summary>
        /// 同步气动传输故障信息到集中管控
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public void SyncError(Action<string, eOutputType> output)
        {
            int res = 0;

            foreach (Warining_Info entity in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<Warining_Info>("where RDFLAG=0"))
            {
                if (!string.IsNullOrEmpty(entity.DeviceName))
                {
                    if (CommonDAO.GetInstance().SaveEquInfHitch(MachineCode, entity.CreateTime, entity.WarningDesc))
                    {
                        entity.RDFLAG = 1;
                        string sql =string.Format("update WARNING_INFO set RDFLAG=1    where  REC_NO={0}",entity.REC_NO);

                        DcDbers.GetInstance().PneumaticTransfer_Dber.Execute(sql);

                        res++;
                    }
                }
            }

            output(string.Format("同步故障信息记录 {0} 条", res), eOutputType.Normal);
        }


        /// <summary>
        /// 同步实时信号到集中管控
        /// </summary>
        /// <param name="output"></param>
        /// <param name="MachineCode">设备编码</param>
        /// <returns></returns>
        public int SyncSignal(Action<string, eOutputType> output)
        {
            int res = 0;

            foreach (InfQDDeviceRealtimeStatus entity in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<InfQDDeviceRealtimeStatus>())
            {

                res += commonDAO.SetSignalDataValue(MachineCode, entity.DeviceName, entity.DeviceStatus1.ToString()) ? 1 : 0;
            }

            foreach (InfQDStatus entity in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<InfQDStatus>())
            {

                res += commonDAO.SetSignalDataValue(MachineCode, "系统", entity.SamReady.ToString()) ? 1 : 0;
            }

            output(string.Format("{0}-同步实时信号 {1} 条", MachineCode, res), eOutputType.Normal);

            return res;
        }


        /// <summary>
        /// 同步气动传输记录
        /// </summary>
        /// <param name="output"></param>
        /// <param name="MachineCode">设备编码</param>
        /// <returns></returns>
        public int SyncTransmissionRecord(Action<string, eOutputType> output)
        {
            int res = 0;

            foreach (QD_Record_Tb entity in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<QD_Record_Tb>("where DataStatus=0"))
            {
           

               List<infRecord> list = commonDAO.SelfDber.Entities<infRecord>("where  StartTime=:StartTime", new { StartTime = entity.StartTime });
               if (list.Count == 0)
               {
                   infRecord infRecord = new infRecord();
                   infRecord.OpStart = GetDeviceNameByListIndex(entity.OpStart);
                   infRecord.OpEnd = GetDeviceNameByListIndex(entity.OpEnd);
                   infRecord.StartTime = entity.StartTime;
                   infRecord.EndTime = entity.EndTime;
                   infRecord.SampleStatus = entity.SampleStatus.Trim() == "1" ? "成功" : "失败";
                   infRecord.Code = entity.SampleId;
                   if (commonDAO.SelfDber.Insert(infRecord) > 0)
                   {
                       entity.DataStatus = 1;
                       DcDbers.GetInstance().PneumaticTransfer_Dber.Update(entity);
                       res++;
                   }
               }
               else {
                   list[0].OpStart = GetDeviceNameByListIndex(entity.OpStart);
                   list[0].OpEnd = GetDeviceNameByListIndex(entity.OpEnd);
                   list[0].StartTime = entity.StartTime;
                   list[0].EndTime = entity.EndTime;
                   list[0].SampleStatus = entity.SampleStatus.Trim() == "1" ? "成功" : "失败";
                   list[0].Code = entity.SampleId;
                   if (commonDAO.SelfDber.Update(list[0]) > 0)
                   {
                       entity.DataStatus = 1;
                       DcDbers.GetInstance().PneumaticTransfer_Dber.Update(entity);
                       res++;
                   }
               }
                //output(string.Format("同步传输记录 {0} 条", res), eOutputType.Normal);
            }
            output(string.Format("同步传输记录 {0} 条", res), eOutputType.Normal);
            //output(string.Format("{0}-同步实时信号 {1} 条", MachineCode, res), eOutputType.Normal);

            return res;
        }


        /// <summary>
        /// 同步气动传输中间表
        /// </summary>
        /// <param name="output"></param>
        /// <param name="MachineCode">设备编码</param>
        /// <returns></returns>
        //public int SyncTransmissionInterface(Action<string, eOutputType> output)
        //{
        //    int res = 0;

        //    foreach (InfQDBill entity in DcDbers.GetInstance().PneumaticTransfer_Dber.Entities<InfQDBill>("where readState=0"))
        //    {


        //        InfInterface InfInterface = commonDAO.SelfDber.Entity<InfInterface>("where  SampleId=:SampleId", new { SampleId = entity.SampleId });
        //        if (InfInterface!=null)
        //        {
        //            //InfInterface InfInterface = new InfInterface();
        //            InfInterface.OpStart = GetDeviceNameByListIndex(entity.OpStart);
        //            InfInterface.OpEnd = GetDeviceNameByListIndex(entity.OpEnd);
        //            InfInterface.SampleId = entity.SampleId;
        //            InfInterface.Send_Time = entity.Send_Time;
        //            InfInterface.DataStatus = getChuanShuZhuangTai(entity.DataStatus);
        //            if (commonDAO.SelfDber.Update(InfInterface) > 0)
        //            {
        //                entity.readState = 1;
        //                DcDbers.GetInstance().PneumaticTransfer_Dber.Update(entity);
        //                res++;
        //            }
        //        }
        //        else
        //        {
        //            InfInterface rulst = new InfInterface();
        //            rulst.OpStart = GetDeviceNameByListIndex(entity.OpStart);
        //            rulst.OpEnd = GetDeviceNameByListIndex(entity.OpEnd);
        //            rulst.SampleId = entity.SampleId;
        //            rulst.Send_Time = entity.Send_Time;
        //            rulst.DataStatus = getChuanShuZhuangTai(entity.DataStatus);
        //            if (commonDAO.SelfDber.Insert(rulst) > 0)
        //            {
        //                entity.readState = 1;
        //                DcDbers.GetInstance().PneumaticTransfer_Dber.Update(entity);
        //                res++;
        //            }
        //        }
        //        //output(string.Format("同步传输记录 {0} 条", res), eOutputType.Normal);
        //    }
        //    output(string.Format("同步传输中间表数据 {0} 条", res), eOutputType.Normal);
        //    //output(string.Format("{0}-同步实时信号 {1} 条", MachineCode, res), eOutputType.Normal);

        //    return res;
        //}



        public string getChuanShuZhuangTai(decimal ypType)
        {
            string zt = "";
            if (ypType == 0)
                zt = "待执行";
            if (ypType == 1)
                zt = "传输成功";
            if (ypType == 2)
                zt = "传输失败";
            if (ypType == 3)
                zt = "气动读取传输中";

            return zt;

        }

        //获取实时信号的状态名称
        private string getString(int nub) {

            string rulst = "";
            switch(nub){
                case 1:
                    rulst = "正常(绿)";
                    break;
                case 2:
                    rulst = "离线(灰)";
                    break;
                case 3:
                    rulst = "错误(红)";
                    break;
                case 4:
                    rulst = "正在传输(绿闪烁)";
                    break;
            }

            return rulst;
        
        }



        //获取实时信号的系统状态
        private string getXTString(int nub)
        {

            string rulst = "";
            switch (nub)
            {
                case 1:
                    rulst = "正在运行中";
                    break;
                case 2:
                    rulst = "故障中";
                    break;
                case 3:
                    rulst = "待机状态";
                    break;
            }

            return rulst;

        }
    }
}
