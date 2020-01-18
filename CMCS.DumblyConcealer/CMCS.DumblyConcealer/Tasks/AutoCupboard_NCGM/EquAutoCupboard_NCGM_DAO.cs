using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
//
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Entities;
using CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM.Enums;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Enums;
using CMCS.Common;
using CMCS.DumblyConcealer.Enums;
using CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities;
using CMCS.Common.DAO;
using CMCS.DapperDber.Util;
using CMCS.Common.Entities.AutoCupboard;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Fuel;
using CMCS.DumblyConcealer.Tasks.AutoMaker.Entities;
using CMCS.Common.Enums.AutoCupboard;
using CMCS.DapperDber.Dbs.SqlServerDb;
using CMCS.Common.Enums;
using System.Threading;
using CMCS.Common.Entities.AutoMaker;
using CMCS.Common.Entities.iEAA;

namespace CMCS.DumblyConcealer.Tasks.AutoCupboard_NCGM
{
	/// <summary>
	/// 光明存样柜业务
	/// </summary>
	public class EquAutoCupboard_NCGM_DAO
	{
		CommonDAO commonDAO = CommonDAO.GetInstance();
		string startAppList = string.Empty;//气动传输存样到化验室样品类型集合

		/// <summary>
		/// 光明存样柜业务
		/// </summary>
		/// <param name="machineCode">设备编码</param>
		/// <param name="equDber">数据库访问对象</param>
		public EquAutoCupboard_NCGM_DAO(string machineCode, SqlServerDapperDber equDber)
		{
			this.MachineCode = machineCode;
			this.EquDber = equDber;
		}

		/// <summary>
		/// 第三方数据库访问对象
		/// </summary>
		SqlServerDapperDber EquDber;
		AutoCupboardDAO autoCupboardDAO = AutoCupboardDAO.GetInstance();
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

		/// <summary>
		/// 转换样瓶类型
		/// </summary>
		/// <param name="makeType"></param>
		/// <returns></returns>
		public decimal ConvertMakeTypeToCYGType(string makeType)
		{
			if (makeType == "1")
			{
				return 1;
			}
			else if (makeType == "2")
			{
				return 2;
			}
			else if (makeType == "3" || makeType == "4")
			{
				return 3;
			}
			else if (makeType == "5" || makeType == "6" || makeType == "7")
			{
				return 4;
			}
			return 0;
		}

		/// <summary>
		/// 改变系统状态值
		/// </summary>
		/// <param name="isHitch">是否故障</param>
		public void ChangeSystemHitchStatus(bool isHitch)
		{
			IsHitch = isHitch;

			if (IsHitch)
			{
				CommonDAO.GetInstance().SetSignalDataValue(MachineCode, eSignalDataName.总体状态.ToString(), ((int)eEquInfSystemStatus.发生故障).ToString());
			}
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

			foreach (EquCYGSignal entity in this.EquDber.Entities<EquCYGSignal>("where DataFlag=0"))
			{
				if (entity.TagName == GlobalVars.EquHeartbeatName) continue;

				// 当心跳检测为故障时，则不更新系统状态，保持 eSampleSystemStatus.发生故障
				if (entity.TagName == eSignalDataName.总体状态.ToString() && IsHitch) continue;

				res += commonDAO.SetSignalDataValue(this.MachineCode, entity.TagName, entity.TagValue) ? 1 : 0;
			}
			output(string.Format("{0}-同步实时信号 {1} 条", this.MachineCode, res), eOutputType.Normal);

			return res;
		}

		/// <summary>
		/// 获取上位机运行状态表 - 心跳值
		/// 每隔30s读取该值，如果数值不变化则表示设备上位机出现故障
		/// </summary>
		/// <param name="MachineCode">设备编码</param>
		public void SyncHeartbeatSignal()
		{
			try
			{
				EquCYGDataFlag pDCYSignal = this.EquDber.Entity<EquCYGDataFlag>();
				ChangeSystemHitchStatus((pDCYSignal != null && pDCYSignal.DataFlag == this.PrevHeartbeat));
				if (pDCYSignal != null) this.PrevHeartbeat = pDCYSignal.DataFlag;
			}
			catch
			{
				ChangeSystemHitchStatus(true);
			}
		}

		/// <summary>
		/// 同步存样柜命令
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public int SyncCYGCmd(Action<string, eOutputType> output)
		{
			int res = 0;
			foreach (InfCYGControlCMD entity in Dbers.GetInstance().SelfDber.Entities<InfCYGControlCMD>("where MachineCode=:MachineCode and DataFlag=0 order by CreateDate desc", new { MachineCode = this.MachineCode }))
			{
				//if (PneumaticTransfer_XMJS_DAO.GetInstance().CheckFree())//气动传输属于空闲状态才会发送存样柜命令
				//{

				EquCYGBill cmd = new EquCYGBill();
				eCZPLX operType;
				Enum.TryParse<eCZPLX>(entity.OperType, out operType);
				cmd.CZPLX = (int)operType;
				cmd.YPLX = autoCupboardDAO.YPLXChangeByCode(entity.CodeNumber);
				cmd.YPRFIDBM = entity.CodeNumber;
				cmd.CZMS = 1;
				cmd.DATAFLAG = 0;
				cmd.CZPFSSJ = DateTime.Now;
				if (this.EquDber.Insert(cmd) > 0)
				{
					res++;
					entity.DataFlag = 1;
					Dbers.GetInstance().SelfDber.Update(entity);
				}
				//}
			}
			output(string.Format("{0}-同步存样柜命令 {1} 条", this.MachineCode, res), eOutputType.Normal);

			return res;
		}

		/// <summary>
		/// 同步存样柜操作结果
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public int SyncCYGRecord(Action<string, eOutputType> output)
		{
			int res = 0;
			foreach (EquCYGBillRecord entity in this.EquDber.Entities<EquCYGBillRecord>("where YPRFIDBM is not null and (CZPJG=1 or CZPJG=2) and DATAFLAG=0 order by CreateDate desc"))
			{
				InfCYGControlCMD cmd = Dbers.GetInstance().SelfDber.Entity<InfCYGControlCMD>("where MachineCode=:MachineCode and CodeNumber=:CodeNumber and DataFlag!=0 order by CreateDate desc", new { MachineCode = this.MachineCode, CodeNumber = entity.YPRFIDBM });
				if (cmd != null)
				{
					cmd.UpdateTime = entity.CreateDate;
					cmd.StartTime = entity.KSSJ;
					cmd.EndTime = entity.JSSJ;
					cmd.DataFlag = 3;
					if (entity.CZPJG == 1)
						cmd.ResultCode = eEquInfCYGCmdResultCode.存样柜执行成功.ToString();
					else if (entity.CZPJG == 2)
						cmd.ResultCode = eEquInfCYGCmdResultCode.存样柜执行失败.ToString();
					if (Dbers.GetInstance().SelfDber.Update(cmd) > 0)
					{
						res++;
						entity.DATAFLAG = 1;
						this.EquDber.Update(entity);
					}
				}
			}
			output(string.Format("{0}-同步存样柜结果 {1} 条", this.MachineCode, res), eOutputType.Normal);

			return res;
		}

		/// <summary>
		/// 同步故障信息到集中管控
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public void SyncCYGError(Action<string, eOutputType> output)
		{
			int res = 0;

			foreach (EquCYGError entity in this.EquDber.Entities<EquCYGError>("where DataFlag=0"))
			{
				if (commonDAO.SaveEquInfHitch(this.MachineCode, entity.Err_Time, "故障代码 " + entity.ErrorCode + "，" + entity.ErrorDescribe))
				{
					entity.DataFlag = 1;
					this.EquDber.Update(entity);

					res++;
				}
			}

			output(string.Format("{0}-同步故障信息记录 {1} 条", this.MachineCode, res), eOutputType.Normal);
		}

		/// <summary>
		/// 同步存样柜数据
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public int SynCYGInfo(Action<string, eOutputType> output)
		{
			int res = 0;
			List<EquCYGSam> equcygsam = this.EquDber.Entities<EquCYGSam>("where DATEDIFF(DD,操作时间,GETDATE())=0 ");
			//List<EquCYGSam> equcygsam = this.EquDber.Entities<EquCYGSam>("where 1=1");
			foreach (EquCYGSam item in equcygsam)
			{
				InfCYGSam infcyg = Dbers.GetInstance().SelfDber.Entity<InfCYGSam>("where MachineCode=:MachineCode and Place=:Place order by UpdateTime", new { MachineCode = this.MachineCode, Place = item.柜号 });
				if (infcyg != null)
				{
					infcyg.UpdateTime = item.操作时间;
					infcyg.SamType = item.瓶子类型;
					infcyg.IsNew = item.柜子状态;
					infcyg.Code = item.样瓶编码;
					if (!string.IsNullOrEmpty(item.操作人员代码))
						infcyg.OperUser = item.操作人员代码;
					res += Dbers.GetInstance().SelfDber.Update(infcyg);
				}
				else
				{
					infcyg = new InfCYGSam();
					infcyg.UpdateTime = item.操作时间;
					infcyg.Place = item.柜号;
					infcyg.MachineCode = this.MachineCode;
					infcyg.SamType = item.瓶子类型;
					infcyg.IsNew = item.柜子状态;
					infcyg.Code = item.样瓶编码;
					if (!string.IsNullOrEmpty(item.操作人员代码))
						infcyg.OperUser = item.操作人员代码;
					res += Dbers.GetInstance().SelfDber.Insert(infcyg);
				}
			}
			output(string.Format("同步存样柜数据{0}条", res), eOutputType.Normal);
			return res;
		}

		//光明传过来的样品类型转换成对应我们数据库样品类型
		public string GetYangpinTypeByCode(string Code, string type)
		{
			string yangPinType = "";
			try
			{
				string sql = "select SamType from cmcstbcygcontrolcmddetail where Code={0}";
				sql = string.Format(sql, Code);
				string SamTypes = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql).Rows[0]["SamType"].ToString();

				string sql2 = "select a.code from syssmtbcodecontent a left join syssmtbcodekind b on a.kindid=b.id where b.kind='{0}' and a.content={1}";
				sql2 = string.Format(sql, "化验样样品类型", SamTypes);
				DataTable tb = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql2);
				yangPinType = tb.Rows[0]["code"].ToString();
			}
			catch
			{
				return type;
			}
			return yangPinType;
		}

		/// <summary>
		/// 托盘到位
		/// </summary>
		/// <returns></returns>
		public bool SamIsReady()
		{
			List<EquCYGSignal> infcygsignals = this.EquDber.Entities<EquCYGSignal>(" where TagName='托盘到位'");
			return infcygsignals.Count > 0 && infcygsignals[0].TagValue == "1";
		}

		/// <summary>
		/// 是否处于空闲状态
		/// </summary>
		public bool CheckFree()
		{
			List<EquCYGSignal> infcygsignals = this.EquDber.Entities<EquCYGSignal>(" where TagName='总体状态'");
			return infcygsignals.Count > 0 && infcygsignals[0].TagValue == "3";
		}

        /// <summary>
        /// 弃样处理
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public int SynCYGPut(Action<string, eOutputType> output)
        {
            int res = 0;
            int day = 90;
            try
            {
                day = commonDAO.GetCommonAppletConfigInt32("存样柜样品储存天数");
            }
            catch
            {
            }
            IList<InfCYGSam> cygSam = commonDAO.SelfDber.Entities<InfCYGSam>("where IsNew=1 and UpdateTime<:UpdateTime order by UpdateTime", new { UpdateTime = DateTime.Now.AddDays(-day + 1).Date });
            List<CodeContent> list = commonDAO.GetCodeContentByKind("化验样样品类型");
            foreach (InfCYGSam item in cygSam)
            {
                CodeContent entity = list.Where(a => a.Code == item.SamType).FirstOrDefault();

                if (entity != null)
                {
                    if (item.UpdateTime.AddDays(string.IsNullOrEmpty(entity.Remark) ? 90 : int.Parse(entity.Remark)) < DateTime.Now)//根据不同类型配置的过期天数自动弃样
                    {
                        output("检测到超期样品，开始处理...", eOutputType.Normal);
                        while (!CheckFree())
                        {
                            output("存样柜未就绪，等待中...", eOutputType.Normal);
                            Thread.Sleep(10000);
                        }
                        while (!PneumaticTransfer_XMJS_DAO.GetInstance().CheckFree())
                        {
                            output("气动未就绪，等待中...", eOutputType.Normal);
                            Thread.Sleep(10000);
                        }
                        AutoCupboardDAO.GetInstance().SaveAutoCupboardCmd(item.Code, item.MachineCode, eCZPLX.弃样);
                        output("弃样命令已发送", eOutputType.Normal);
                        eEquInfCYGCmdResultCode equInfCmdResultCode = eEquInfCYGCmdResultCode.默认;
                        while (equInfCmdResultCode == eEquInfCYGCmdResultCode.气动执行成功)
                        {
                            Thread.Sleep(10000);
                            output(equInfCmdResultCode.ToString().Replace("默认", "执行中"), eOutputType.Normal);
                            // 获取卸样命令的执行结果
                            equInfCmdResultCode = autoCupboardDAO.GetAutoCupboardResult(item.Code);
                        }
                        output(equInfCmdResultCode.ToString(), eOutputType.Important);
                        res++;
                        output("弃样处理成功", eOutputType.Important);
                    }
                }
            }
            return res;
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


		///// <summary>
		///// 同步样品信息到集中管控入厂煤制样明细表
		///// </summary>
		///// <param name="makeDetail"></param>
		//private bool SyncToRCMakeDetail(EquQZDZYJDetail makeDetail)
		//{
		//    //string makeCode = GetMakeCodeByMakePlanID(makeDetail.MakePlanId);
		//    CmcsRCMake rCMake = commonDAO.SelfDber.Entity<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = makeDetail.MakeCode.ToUpper() });
		//    if (rCMake != null)
		//    {
		//        // 修改制样结束时间
		//        rCMake.MakeStyle = eMakeType.机械制样.ToString();
		//        if (rCMake.GetDate < makeDetail.EndTime) rCMake.GetDate = makeDetail.EndTime;
		//        if (rCMake.GetDate != rCMake.CreateDate && rCMake.GetDate > makeDetail.StartTime)
		//        {
		//            rCMake.GetDate = DateTime.Now;
		//            rCMake.GetDate = makeDetail.StartTime;
		//        }
		//        rCMake.MakeStartTime = makeDetail.StartTime;
		//        rCMake.MakeEndTime = (string.IsNullOrEmpty(rCMake.MakeEndTime.ToString()) || rCMake.MakeEndTime < makeDetail.EndTime) ? makeDetail.EndTime : rCMake.MakeEndTime;



		//        commonDAO.SelfDber.Update(rCMake);
		//        CmcsRCMakeDetail rCMakeDetail = commonDAO.SelfDber.Entity<CmcsRCMakeDetail>("where MakeId=:MakeId and SampleType=:SampleType", new { MakeId = rCMake.Id, SampleType = commonDAO.ConvertToJxzyYPLX(makeDetail.YPType) });
		//        if (rCMakeDetail != null)
		//        {
		//            rCMakeDetail.BarrelCode = makeDetail.BarrelCode;
		//            //rCMakeDetail.CheckWeight = Convert.ToDecimal(makeDetail.YPWeight);
		//            rCMakeDetail.BarrelTime = makeDetail.EndTime;
		//            rCMakeDetail.OperDate = DateTime.Now;
		//            rCMakeDetail.Weight = makeDetail.YPWeight;
		//            return commonDAO.SelfDber.Update(rCMakeDetail) > 0;
		//        }
		//    }

		//    return false;
		//}


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
				rCMake.LyWeight = Math.Round(makeDetail.LyWeight / 10m, 1);


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
