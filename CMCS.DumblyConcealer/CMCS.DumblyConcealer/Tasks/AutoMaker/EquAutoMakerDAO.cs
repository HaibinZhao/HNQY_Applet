using System;
using CMCS.Common;
using CMCS.Common.DAO;
using CMCS.Common.Enums;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.AutoMaker.Entities;
using CMCS.Common.Entities.AutoMaker;
using CMCS.Common.Entities.Fuel;
using CMCS.DapperDber.Dbs.SqlServerDb;
using CMCS.DumblyConcealer.Tasks.AutoMaker.Enums;

namespace CMCS.DumblyConcealer.Tasks.AutoMaker
{
    /// <summary>
    /// 全自动制样机接口业务
    /// </summary>
    public class EquAutoMakerDAO
    {
        /// <summary>
        /// EquAutoMakerDAO
        /// </summary>
        /// <param name="machineCode">制样机编码</param>
        /// <param name="equDber">第三方数据库访问对象</param>
        public EquAutoMakerDAO(string machineCode, SqlServerDapperDber equDber)
        {
            this.MachineCode = machineCode;
            this.EquDber = equDber;
        }

        CommonDAO commonDAO = CommonDAO.GetInstance();

        /// <summary>
        /// 第三方数据库访问对象
        /// </summary>
        SqlServerDapperDber EquDber;
        /// <summary>
        /// 设备编码
        /// </summary>
        string MachineCode;
        /// <summary>
        /// 是否处于故障状态
        /// </summary>
        bool IsHitch = false;
        /// <summary>
        /// 上一次上位机心跳值
        /// </summary>
        string PrevHeartbeat = string.Empty;

        #region 数据转换方法（此处有点麻烦，后期调整接口方案）
        /// <summary>
        /// 转换成第三方接口-煤种
        /// </summary>
        /// <param name="fuelKindName">煤种</param>
        /// <returns></returns>
        public int ConvertToInfFuelKindName(string fuelKindName)
        {
            eFuelKindName enumResulr;
            if (Enum.TryParse(fuelKindName, out enumResulr))
                return (int)enumResulr;
            else
                return (int)eFuelKindName.其它;
        }

        /// <summary>
        /// 转换成第三方接口-水分
        /// </summary>
        /// <param name="mt">水分</param>
        /// <returns></returns>
        public int ConvertToInfMt(string mt)
        {
            // TODO

            eMt enumResulr;
            if (Enum.TryParse(mt, out enumResulr))
                return (int)enumResulr;
            else
                return (int)eMt.一般湿煤;
        }


        #endregion

        /// <summary>
        /// 同步实时信号到集中管控
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SyncSignal(Action<string, eOutputType> output)
        {
            int res = 0;

            foreach (EquQZDZYJSignal entity in this.EquDber.Entities<EquQZDZYJSignal>())
            {
                if (entity.TagName == GlobalVars.EquHeartbeatName) continue;

                // 当心跳检测为故障时，则不更新系统状态，保持 eSampleSystemStatus.发生故障
                if (entity.TagName == eSignalDataName.系统.ToString() && IsHitch) continue;
                if (entity.TagName == eSignalDataName.系统.ToString())
                {
                    if (entity.TagValue == "3")
                    {
                        entity.TagValue = "就绪待机";
                    }
                    if (entity.TagValue == "2")
                    {
                        entity.TagValue = "发生故障";
                    }
                    if (entity.TagValue == "1")
                    {
                        entity.TagValue = "正在运行";
                        res += commonDAO.SetSignalDataValue(this.MachineCode, "开始时间", entity.UpdateTime.ToString("HH:mm:ss"), entity.UpdateTime.ToString()) ? 1 : 0;
                    }
                    res += commonDAO.SetSignalDataValue(this.MachineCode, entity.TagName, entity.TagValue, entity.UpdateTime.ToString()) ? 1 : 0;
                }
                else if (entity.TagName == "制样编码")
                {
                    res += commonDAO.SetSignalDataValue(this.MachineCode, entity.TagName, entity.Remark, entity.UpdateTime.ToString()) ? 1 : 0;
                }
                else
                {
                    res += commonDAO.SetSignalDataValue(this.MachineCode, entity.TagName, entity.TagValue) ? 1 : 0;
                }
            }
            output(string.Format("同步实时信号 {0} 条", res), eOutputType.Normal);

            return res;
        }

        /// <summary>
        /// 获取上位机运行状态表 - 心跳值
        /// 每隔30s读取该值，如果数值不变化则表示设备上位机出现故障
        /// </summary>
        /// <returns></returns>
        public void SyncHeartbeatSignal()
        {
            //当他们制样机直接关机断网后,获取数据就会报错,这时候直接修改当前制样机的系统状态为故障
            try
            {
                EquQZDZYJDataFlag pDCYSignal = this.EquDber.Entity<EquQZDZYJDataFlag>();
                ChangeSystemHitchStatus((pDCYSignal != null && pDCYSignal.DataFlag == this.PrevHeartbeat));
            }
            catch {
                CommonDAO.GetInstance().SetSignalDataValue(this.MachineCode, eSignalDataName.系统.ToString(), eEquInfSamplerSystemStatus.发生故障.ToString());
            }
        }

        /// <summary>
        /// 改变系统状态值
        /// </summary>
        /// <param name="isHitch">是否故障</param>
        public void ChangeSystemHitchStatus(bool isHitch)
        {
            IsHitch = isHitch;

            if (IsHitch) CommonDAO.GetInstance().SetSignalDataValue(this.MachineCode, eSignalDataName.系统.ToString(), eEquInfSamplerSystemStatus.发生故障.ToString());
        }

        /// <summary>
        /// 同步制样计划
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public void SyncPlan(Action<string, eOutputType> output)
        {
            int res = 0;

            // 集中管控 > 第三方 
            foreach (InfMakerPlan entity in AutoMakerDAO.GetInstance().GetWaitForSyncMakePlan(this.MachineCode))
            {
                bool isSuccess = false;
                // 需调整：计划中的煤种、水分、颗粒度等信息视接口而定
                EquQZDZYJPlan qZDZYJPlan = this.EquDber.Get<EquQZDZYJPlan>(entity.MakeCode);
                decimal cs = entity.CoalSize == "小粒度" ? 1 : 2;

                if (qZDZYJPlan == null)
                {
                    isSuccess = this.EquDber.Insert(new EquQZDZYJPlan
                    {
                        // 保持相同的Id
                        Id = entity.Id,
                        InFactoryBatchId = entity.InFactoryBatchId,
                        MakeCode = entity.MakeCode,
                        FuelKindName = ConvertToInfFuelKindName(entity.FuelKindName).ToString(),
                        Mt = ConvertToInfMt(entity.Mt),
                        CoalSize = cs,
                        CreateDate=DateTime.Now,
                        DataFlag = 0
                    }) > 0;
                }
                else
                {
                    qZDZYJPlan.Id = entity.Id;
                    qZDZYJPlan.FuelKindName = ConvertToInfFuelKindName(entity.FuelKindName).ToString();
                    qZDZYJPlan.Mt = ConvertToInfMt(entity.Mt);
                    qZDZYJPlan.CoalSize = cs;
                    qZDZYJPlan.DataFlag = 0;
                    isSuccess = this.EquDber.Update(qZDZYJPlan) > 0;
                }

                if (isSuccess)
                {
                    entity.SyncFlag = 1;
                    commonDAO.SelfDber.Update(entity);

                    res++;
                }
            }
            output(string.Format("{0}同步制样计划 {1} 条（集中管控 > 第三方）", this.MachineCode, res), eOutputType.Normal);
        }

        /// <summary>
        /// 制样控制命令表
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public void SyncCmd(Action<string, eOutputType> output)
        {
            int res = 0;

            // 集中管控 > 第三方 
            foreach (InfMakerControlCmd entity in AutoMakerDAO.GetInstance().GetWaitForSyncMakerControlCmd(this.MachineCode))
            {
                bool isSuccess = false;

                EquQZDZYJCmd qZDZYJCmd = this.EquDber.Get<EquQZDZYJCmd>(entity.Id);
                if (qZDZYJCmd == null)
                {
                    isSuccess = this.EquDber.Insert(new EquQZDZYJCmd
                    {
                        // 保持相同的Id
                        Id = entity.Id,
                        CmdCode = entity.CmdCode == "开始制样" ? "0" : "1",
                        MakeCode = entity.MakeCode,
                        ResultCode = Convert.ToInt32(eEquInfCmdResultCode.默认),
                        DataFlag = 0
                    }) > 0;
                }
                else isSuccess = true;

                if (isSuccess)
                {
                    entity.SyncFlag = 1;
                    commonDAO.SelfDber.Update(entity);

                    res++;
                }
            }
            output(string.Format("同步控制命令 {0} 条（集中管控 > 第三方）", res), eOutputType.Normal);

            res = 0;
            // 第三方 > 集中管控
            foreach (EquQZDZYJCmd entity in this.EquDber.Entities<EquQZDZYJCmd>("where DataFlag=1"))
            {
                InfMakerControlCmd makerControlCmd = commonDAO.SelfDber.Get<InfMakerControlCmd>(entity.Id);
                if (makerControlCmd == null || (makerControlCmd.DataFlag == 1 && entity.ResultCode != 0)) continue;

                // 更新执行结果等
                makerControlCmd.ResultCode = ((eEquInfCmdResultCode)entity.ResultCode).ToString();
                makerControlCmd.DataFlag = 1;

                if (commonDAO.SelfDber.Update(makerControlCmd) > 0)
                {
                    // 我方已读
                    entity.DataFlag = 2;
                    this.EquDber.Update(entity);

                    res++;
                }
            }
            output(string.Format("同步控制命令 {0} 条（第三方 > 集中管控）", res), eOutputType.Normal);
        }

        /// <summary>
        /// 同步制样 出样明细信息到集中管控
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public void SyncMakeDetail(Action<string, eOutputType> output)
        {
            int res = 0;

            foreach (EquQZDZYJDetail entity in this.EquDber.Entities<EquQZDZYJDetail>("where DataFlag=0 order by CreateDate asc"))
            {
                if (SyncToRCMakeDetail(entity))
                {
                    InfMakerRecord makerecord = commonDAO.SelfDber.Entity<InfMakerRecord>("where BarrelCode=:BarrelCode", new { BarrelCode = entity.BarrelCode });
                    if (makerecord == null)
                    {
                        if (AutoMakerDAO.GetInstance().SaveMakerRecord(new InfMakerRecord
                        {
                            InterfaceType = CommonDAO.GetInstance().GetMachineInterfaceTypeByCode(this.MachineCode),
                            MachineCode = this.MachineCode,
                            MakeCode = entity.MakeCode,
                            BarrelCode = entity.BarrelCode,
                            YPType = commonDAO.ConvertToJxzyYPLX(entity.YPType),
                            YPWeight = entity.YPWeight,
                            StartTime = entity.StartTime,
                            EndTime = entity.EndTime,
                            MakeUser = entity.MakeUser,
                            DataFlag = 1
                        }))
                        {
                            entity.DataFlag = 1;
                            this.EquDber.Update(entity);
                            res++;

                            // 需调整：启动传输调度计划需根据现场情况而定
                            // 插入气动传输调度计划
                            //EquAutoCupboardDAO.GetInstance().AddNewSendSampleId(entity.BarrelCode, entity.YPType, eCmdCode.制样机1, eCmdCode.存样柜);
                        }
                    }
                    else
                    {
                        makerecord.MakeCode = entity.MakeCode;
                        makerecord.BarrelCode = entity.BarrelCode;
                        makerecord.YPType = commonDAO.ConvertToJxzyYPLX(entity.YPType);
                        makerecord.YPWeight = entity.YPWeight;
                        makerecord.StartTime = entity.StartTime;
                        makerecord.EndTime = entity.EndTime;
                        makerecord.MakeUser = entity.MakeUser;
                        makerecord.DataFlag = 1;
                        if (commonDAO.SelfDber.Update(makerecord) > 0)
                        {
                            entity.DataFlag = 1;
                            this.EquDber.Update(entity);
                            res++;
                        }
                    }
                }
            }

            output(string.Format("同步出样明细记录 {0} 条", res), eOutputType.Normal);
        }

        /// <summary>
        /// 同步制样 故障信息到集中管控
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public void SyncError(Action<string, eOutputType> output)
        {
            int res = 0;

            foreach (EquQZDZYJError entity in this.EquDber.Entities<EquQZDZYJError>("where DataFlag=0"))
            {
                if (CommonDAO.GetInstance().SaveEquInfHitch(this.MachineCode, entity.ErrorTime, entity.ErrorDescribe))
                {
                    entity.DataFlag = 1;
                    this.EquDber.Update(entity);

                    res++;
                }
            }

            output(string.Format("同步故障信息记录 {0} 条", res), eOutputType.Normal);
        }

        /// <summary>
        /// 同步样品信息到集中管控入厂煤制样明细表
        /// </summary>
        /// <param name="makeDetail"></param>
        private bool SyncToRCMakeDetail(EquQZDZYJDetail makeDetail)
        {
            //string makeCode = GetMakeCodeByMakePlanID(makeDetail.MakePlanId);
            CmcsRCMake rCMake = commonDAO.SelfDber.Entity<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = makeDetail.MakeCode.ToUpper() });
            if (rCMake != null)
            {
                // 修改制样结束时间
                rCMake.MakeStyle = eMakeType.机械制样.ToString();
                if (rCMake.GetDate < makeDetail.EndTime) rCMake.GetDate = makeDetail.EndTime;
                if (rCMake.GetDate != rCMake.CreateDate && rCMake.GetDate > makeDetail.StartTime)
                {
                    rCMake.GetDate = DateTime.Now;
                    rCMake.GetDate = makeDetail.StartTime;
                }
                rCMake.MakeStartTime = makeDetail.StartTime;
                rCMake.MakeEndTime = (string.IsNullOrEmpty(rCMake.MakeEndTime.ToString()) || rCMake.MakeEndTime < makeDetail.EndTime) ? makeDetail.EndTime : rCMake.MakeEndTime;
                rCMake.LyWeight =Math.Round(makeDetail.LyWeight/10m,1);


                commonDAO.SelfDber.Update(rCMake);
                CmcsRCMakeDetail rCMakeDetail = commonDAO.SelfDber.Entity<CmcsRCMakeDetail>("where MakeId=:MakeId and SampleType=:SampleType", new { MakeId = rCMake.Id, SampleType = commonDAO.ConvertToJxzyYPLX(makeDetail.YPType) });
                if (rCMakeDetail != null)
                {
                    rCMakeDetail.BarrelCode = makeDetail.BarrelCode;
                    //rCMakeDetail.CheckWeight = Convert.ToDecimal(makeDetail.YPWeight);
                    rCMakeDetail.BarrelTime = makeDetail.EndTime;
                    rCMakeDetail.OperDate = DateTime.Now;
                    rCMakeDetail.Weight = makeDetail.YPWeight;
                    return commonDAO.SelfDber.Update(rCMakeDetail) > 0;
                }
            }

            return false;
        }
    }
}
