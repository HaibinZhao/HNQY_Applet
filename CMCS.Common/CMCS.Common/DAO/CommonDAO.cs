using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
//
using CMCS.Common.Entities;
using CMCS.Common.Entities.AutoMaker;
using CMCS.Common.Entities.BaseInfo;
using CMCS.Common.Entities.CarTransport;
using CMCS.Common.Entities.Fuel;
using CMCS.Common.Entities.iEAA;
using CMCS.Common.Entities.Inf;
using CMCS.Common.Entities.Sys;
using CMCS.Common.Enums;
using CMCS.Common.Utilities;
using CMCS.DapperDber.Dbs.OracleDb;
using CMCS.DapperDber.Util;
using System.Net.NetworkInformation;

namespace CMCS.Common.DAO
{
    /// <summary>
    /// 公共业务
    /// </summary>
    public class CommonDAO
    {
        private static CommonDAO instance;

        public static CommonDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new CommonDAO();
            }

            return instance;
        }

        private CommonDAO()
        { }

        public OracleDapperDber SelfDber
        {
            get { return Dbers.GetInstance().SelfDber; }
        }

        #region 编码管理

        /// <summary>
        /// 根据名称获取编码
        /// </summary>
        /// <param name="kindName">编码类别</param>
        /// <returns></returns>
        public List<CodeContent> GetCodeContentByKind(string kindName)
        {
            List<CodeContent> res = new List<CodeContent>();

            CodeKind codeKind = SelfDber.Entity<CodeKind>("where Kind=:Kind", new { Kind = kindName });
            if (codeKind != null) res = SelfDber.Entities<CodeContent>("where KindId=:KindId order by CodeOrder asc", new { KindId = codeKind.Id });

            return res;
        }

        #endregion

        #region 用户登录

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="userAccount"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User Login(string roleCode, string userAccount, string password)
        {
            return SelfDber.Query<User>("select t.* from sysamtbuser t inner join sysamtbparty_role a on t.partyid=a.partyid inner join sysamtbpartyrole b on b.id=a.roleid where (b.RoleCode=:RoleCode or b.RoleCode='0000') and t.UserAccount=:UserAccount and t.MDPassword=:MDPassword ", new { RoleCode = roleCode, UserAccount = userAccount, MDPassword = password }).FirstOrDefault();
        }

        /// <summary>
        /// 获取某角色下所有的用户
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns></returns>
        public List<User> GetAllSystemUser(string roleCode)
        {
            return SelfDber.Query<User>("select t.* from sysamtbuser t inner join sysamtbparty_role a on t.partyid=a.partyid inner join sysamtbpartyrole b on b.id=a.roleid where b.RoleCode=:RoleCode or b.RoleCode='0000' order by to_char(substr(t.username,0,1)) ", new { RoleCode = roleCode }).ToList();
        }

        /// <summary>
        /// 获取管理员
        /// </summary>
        /// <returns></returns>
        public User GetAdminUser()
        {
            return SelfDber.Entity<User>("where UserAccount=:UserAccount", new { UserAccount = GlobalVars.AdminAccount });
        }

        #endregion

        #region 权限判断

        /// <summary>
        /// 判断用户是否有权限
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="resourceCode">模块功能编码</param>
        /// <returns></returns>
        public bool HasResourcePowerByResCode(string userAccount, string resourceCode)
        {
            if (string.IsNullOrEmpty(userAccount) || string.IsNullOrEmpty(resourceCode)) return false;
            User user = SelfDber.Query<User>("select t.* from sysamtbuser t inner join sysamtbparty_role a on t.partyid=a.partyid inner join sysamtbpartyrole b on b.id=a.roleid where (b.RoleCode=:RoleCode or b.RoleCode='0000') and t.UserAccount=:UserAccount", new { RoleCode = resourceCode, UserAccount = userAccount }).FirstOrDefault();
            if (user != null) return true;
            return false;
        }

        #endregion

        #region 程序配置

        /// <summary>
        /// 获取程序配置
        /// </summary>
        /// <param name="appIdentifier">程序唯一标识</param>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public string GetAppletConfigString(string appIdentifier, string configName)
        {
            CmcsAppletConfig appletConfig = SelfDber.Entity<CmcsAppletConfig>("where AppIdentifier=:AppIdentifier and ConfigName=:ConfigName", new { AppIdentifier = appIdentifier, ConfigName = configName });
            if (appletConfig != null) return appletConfig.ConfigValue;

            return string.Empty;
        }

        /// <summary>
        /// 设置程序配置
        /// </summary>
        /// <param name="appIdentifier">程序唯一标识</param>
        /// <param name="configName">配置名称</param>
        /// <param name="configValue">值</param>
        /// <returns></returns>
        public bool SetAppletConfig(string appIdentifier, string configName, string configValue)
        {
            CmcsAppletConfig appletConfig = SelfDber.Entity<CmcsAppletConfig>("where AppIdentifier=:AppIdentifier and ConfigName=:ConfigName", new { AppIdentifier = appIdentifier, ConfigName = configName });
            if (appletConfig != null)
            {
                appletConfig.ConfigValue = configValue;
                return SelfDber.Update(appletConfig) > 0;
            }
            else
            {
                return SelfDber.Insert(new CmcsAppletConfig()
                {
                    AppIdentifier = appIdentifier,
                    ConfigName = configName,
                    ConfigValue = configValue
                }) > 0;
            }
        }

        /// <summary>
        /// 获取程序配置
        /// </summary> 
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public string GetAppletConfigString(string configName)
        {
            return GetAppletConfigString(CommonAppConfig.GetInstance().AppIdentifier, configName);
        }

        /// <summary>
        /// 获取程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public bool GetAppletConfigBoolen(string configName)
        {
            return Convert.ToBoolean(GetAppletConfigString(configName));
        }

        /// <summary>
        /// 获取程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public int GetAppletConfigInt32(string configName)
        {
            return Convert.ToInt32(GetAppletConfigString(configName));
        }



        /// <summary>
        /// 获取程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public DateTime GetAppletConfigDateTime(string appIdentifier, string configName)
        {
            return Convert.ToDateTime(GetAppletConfigString(configName));
        }

        /// <summary>
        /// 获取程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public double GetAppletConfigDouble(string configName)
        {
            return Convert.ToDouble(GetAppletConfigString(configName));
        }

        /// <summary>
        /// 设置程序配置
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="configValue"></param>
        /// <returns></returns>
        public bool SetAppletConfig(string configName, string configValue)
        {
            return SetAppletConfig(CommonAppConfig.GetInstance().AppIdentifier, configName, configValue);
        }

        /// <summary>
        /// 获取公共程序配置
        /// </summary> 
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public string GetCommonAppletConfigString(string configName)
        {
            return GetAppletConfigString(GlobalVars.CommonAppletConfigName, configName);
        }

        /// <summary>
        /// 获取公共程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public bool GetCommonAppletConfigBoolen(string configName)
        {
            return Convert.ToBoolean(GetCommonAppletConfigString(configName));
        }

        /// <summary>
        /// 获取公共程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public int GetCommonAppletConfigInt32(string configName)
        {
            return Convert.ToInt32(GetCommonAppletConfigString(configName));
        }

        /// <summary>
        /// 获取公共程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public DateTime GetCommonAppletConfigDateTime(string appIdentifier, string configName)
        {
            return Convert.ToDateTime(GetCommonAppletConfigString(configName));
        }

        /// <summary>
        /// 获取公共程序配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        public double GetCommonAppletConfigDouble(string configName)
        {
            return Convert.ToDouble(GetCommonAppletConfigString(configName));
        }

        /// <summary>
        /// 设置公共程序配置
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="configValue"></param>
        /// <returns></returns>
        public bool SetCommonAppletConfig(string configName, string configValue)
        {
            return SetAppletConfig(GlobalVars.CommonAppletConfigName, configName, configValue);
        }

        #endregion

        #region 运行日志

        /// <summary>
        /// 保存程序运行日志
        /// </summary>
        /// <param name="appletLogLevel">日志等级</param>
        /// <param name="title">日志标题</param>
        /// <param name="content">日志内容</param>
        /// <returns></returns>
        public bool SaveAppletLog(eAppletLogLevel appletLogLevel, string title, string content)
        {
            return SelfDber.Insert(new CmcsAppletLog()
            {
                AppIdentifier = CommonAppConfig.GetInstance().AppIdentifier,
                Title = title,
                LogLevel = appletLogLevel.ToString(),
                Content = content
            }) > 0;
        }

        #endregion

        #region 实时信号

        /// <summary>
        /// 获取实时信号
        /// </summary> 
        /// <param name="signalPrefix">信号前缀</param>
        /// <param name="signalName">信号名</param>
        /// <returns></returns>
        public string GetSignalDataValue(string signalPrefix, string signalName)
        {
            CmcsSignalData cmcsSignalData = SelfDber.Entity<CmcsSignalData>("where SignalPrefix=:SignalPrefix and SignalName=:SignalName order by UpdateTime desc", new { SignalPrefix = signalPrefix, SignalName = signalName });
            if (cmcsSignalData != null) return cmcsSignalData.SignalValue;

            return string.Empty;
        }

        /// <summary>
        /// 获取实时信号
        /// </summary> 
        /// <param name="signalPrefix">信号前缀</param>
        /// <param name="signalName">信号名</param>
        /// <returns></returns>
        public double GetSignalDataValueDouble(string signalPrefix, string signalName)
        {
            double res = 0;
            Double.TryParse(GetSignalDataValue(signalPrefix, signalName), out res);

            return res;
        }

        /// <summary>
        /// 获取实时信号
        /// </summary> 
        /// <param name="signalPrefix">信号前缀</param>
        /// <param name="signalName">信号名</param>
        /// <returns></returns>
        public int GetSignalDataValueInt32(string signalPrefix, string signalName)
        {
            int res = 0;
            Int32.TryParse(GetSignalDataValue(signalPrefix, signalName), out res);

            return res;
        }

        /// <summary>
        /// 获取实时信号
        /// </summary> 
        /// <param name="signalPrefix">信号前缀</param>
        /// <param name="signalName">信号名</param>
        /// <returns></returns>
        public bool GetSignalDataValueBoolean(string signalPrefix, string signalName)
        {
            Boolean res = false;
            Boolean.TryParse(GetSignalDataValue(signalPrefix, signalName), out res);

            return res;
        }

        /// <summary>
        /// 获取实时信号
        /// </summary> 
        /// <param name="signalPrefix">信号前缀</param>
        /// <param name="signalName">信号名</param>
        /// <returns></returns>
        public DateTime GetSignalDataValueDateTime(string signalPrefix, string signalName)
        {
            DateTime res = DateTime.MinValue;
            DateTime.TryParse(GetSignalDataValue(signalPrefix, signalName), out res);

            return res;
        }

        /// <summary>
        /// 设置实时信号
        /// </summary> 
        /// <param name="signalPrefix">信号前缀</param>
        /// <param name="signalName">信号名</param>
        /// <param name="signalValue">值</param>
        /// <returns></returns>
        public bool SetSignalDataValue(string signalPrefix, string signalName, string signalValue,string dtm="")
        {
            CmcsSignalData cmcsSignalData = SelfDber.Entity<CmcsSignalData>("where SignalPrefix=:SignalPrefix and SignalName=:SignalName order by UpdateTime desc", new { SignalPrefix = signalPrefix, SignalName = signalName });
            if (cmcsSignalData == null)
            {
                SelfDber.Insert(new CmcsSignalData
                {
                    SignalPrefix = signalPrefix,
                    SignalName = signalName,
                    SignalValue = signalValue,
                    UpdateTime =string.IsNullOrEmpty(dtm)?DateTime.Now:Convert.ToDateTime(dtm)
                });
            }

            return SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsSignalData>() + " set SignalValue=:SignalValue,UpdateTime=sysdate where SignalPrefix=:SignalPrefix and  SignalName=:SignalName", new { SignalPrefix = signalPrefix, SignalName = signalName, SignalValue = signalValue }) > 0;
        }

        #endregion

        #region 设备管理

        /// <summary>
        /// 根据设备编码获取设备
        /// </summary>
        /// <param name="machineCode">设备编码</param>
        /// <returns></returns>
        public CmcsCMEquipment GetCMEquipmentByMachineCode(string machineCode)
        {
            return SelfDber.Entity<CmcsCMEquipment>("where EquipmentCode=:EquipmentCode", new { EquipmentCode = machineCode });
        }

        /// <summary>
        /// 根据设备编码获取设备名称
        /// </summary>
        /// <param name="machineCode">设备编码</param>
        /// <returns></returns>
        public string GetMachineNameByCode(string machineCode)
        {
            CmcsCMEquipment entity = GetCMEquipmentByMachineCode(machineCode);
            return entity != null ? entity.EquipmentName : string.Empty;
        }

        /// <summary>
        /// 根据设备编码获取接口类型
        /// </summary>
        /// <param name="machineCode">设备编码</param>
        /// <returns></returns>
        public string GetMachineInterfaceTypeByCode(string machineCode)
        {
            CmcsCMEquipment entity = GetCMEquipmentByMachineCode(machineCode);
            return entity != null ? entity.InterfaceType : string.Empty;
        }

        /// <summary>
        /// 根据父节点设备编码获取所有子设备集合
        /// </summary>
        /// <param name="machineCode">设备编码</param>
        /// <returns></returns>
        public List<CmcsCMEquipment> GetChildrenMachinesByCode(string machineCode)
        {
            List<CmcsCMEquipment> list = new List<CmcsCMEquipment>();

            CmcsCMEquipment entity = GetCMEquipmentByMachineCode(machineCode);
            if (entity != null) list = SelfDber.Entities<CmcsCMEquipment>(" where ParentId=:ParentId  order by Sequence asc", new { ParentId = entity.Id });

            return list;
        }

        #endregion

        #region 第三方设备接口

        /// <summary>
        /// 获取未读第三方设备故障信息
        /// </summary>
        /// <param name="machineCode">设备编码</param>
        /// <returns></returns>
        public List<InfEquInfHitch> GetUnReadEquInfHitchs(string machineCode)
        {
            List<InfEquInfHitch> res = SelfDber.Entities<InfEquInfHitch>("where MachineCode=:machineCode and DataFlag=0", new { MachineCode = machineCode });
            SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<InfEquInfHitch>() + " set DataFlag=1 where MachineCode=:machineCode and DataFlag=0", new { MachineCode = machineCode });
            return res;
        }

        /// <summary>
        /// 获取当日第三方设备故障信息
        /// </summary>
        /// <param name="machineCode">设备编码</param>
        /// <param name="dtTime">时间</param>
        /// <returns></returns>
        public List<InfEquInfHitch> GetEquInfHitchsByTime(string machineCode, DateTime dtTime)
        {
            List<InfEquInfHitch> res = SelfDber.Entities<InfEquInfHitch>("where MachineCode=:machineCode and HitchTime like '%" + dtTime.ToString("yyyy-MM-dd") + "%' order by HitchTime desc", new { MachineCode = machineCode });
            return res;
        }

        /// <summary>
        /// 保存第三方设备接口 - 实时集样罐表
        /// </summary>
        /// <param name="equInfSampleBarrel"></param>
        /// <returns></returns>
        public bool SaveEquInfSampleBarrel(InfEquInfSampleBarrel equInfSampleBarrel)
        {
            InfEquInfSampleBarrel oldEquInfSampleBarrel = SelfDber.Entity<InfEquInfSampleBarrel>("where InterfaceType=:InterfaceType and MachineCode=:MachineCode and BarrelNumber=:BarrelNumber and BarrelType=:BarrelType"
                , new { InterfaceType = equInfSampleBarrel.InterfaceType, MachineCode = equInfSampleBarrel.MachineCode, BarrelNumber = equInfSampleBarrel.BarrelNumber, BarrelType = equInfSampleBarrel.BarrelType });

            if (oldEquInfSampleBarrel == null)
                return SelfDber.Insert(equInfSampleBarrel) > 0;
            else
            {
                oldEquInfSampleBarrel.BarrelNumber = equInfSampleBarrel.BarrelNumber;
                oldEquInfSampleBarrel.BarrelStatus = equInfSampleBarrel.BarrelStatus;
                oldEquInfSampleBarrel.InFactoryBatchId = equInfSampleBarrel.InFactoryBatchId;
                oldEquInfSampleBarrel.IsCurrent = equInfSampleBarrel.IsCurrent;
                oldEquInfSampleBarrel.SampleCode = equInfSampleBarrel.SampleCode;
                oldEquInfSampleBarrel.SampleCount = equInfSampleBarrel.SampleCount;
                oldEquInfSampleBarrel.UpdateTime = equInfSampleBarrel.UpdateTime;
                oldEquInfSampleBarrel.DataFlag = equInfSampleBarrel.DataFlag;

                return SelfDber.Update(oldEquInfSampleBarrel) > 0;
            }
        }

        /// <summary>
        /// 保持第三方设备接口 - 故障信息表
        /// </summary> 
        /// <param name="machineCode">设备编码</param>
        /// <param name="hitchTime">故障时间</param>
        /// <param name="hitchDescribe">故障描述</param>
        /// <returns></returns>
        public bool SaveEquInfHitch(string machineCode, DateTime hitchTime, string hitchDescribe)
        {
            return SelfDber.Insert(new InfEquInfHitch
            {
                DataFlag = 0,
                HitchDescribe = hitchDescribe,
                HitchTime = hitchTime,
                InterfaceType = GetMachineInterfaceTypeByCode(machineCode),
                MachineCode = machineCode
            }) > 0;
        }

        #endregion

        #region 系统消息弹框

        /// <summary>
        /// 获取当日未读取的异常信息，按先进先出、同一设备异常分组的原则,
        /// </summary>
        /// <returns></returns>
        public List<InfEquInfHitch> GetWarnEquInfHitch()
        {
            List<InfEquInfHitch> cmcsequinfhitch = SelfDber.Entities<InfEquInfHitch>(" where IsRead=0 and HitchTime like '%" + DateTime.Now.ToString("yyyy-MM-dd") + "%' order by HitchTime ");
            if (cmcsequinfhitch.Count > 0)
                return cmcsequinfhitch.GroupBy(a => a.MachineCode).First().ToList();
            else
                return new List<InfEquInfHitch>();
        }

        /// <summary>
        /// 根据异常时间查询异常信息
        /// </summary>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        public List<InfEquInfHitch> GetCmcsEquInfHitchs(DateTime dtStart, DateTime dtEnd)
        {
            List<InfEquInfHitch> equinfhitchs = SelfDber.Entities<InfEquInfHitch>(" where HitchTime>='" + dtStart + "' and HitchTime<='" + dtEnd + "' order by HitchTime ");
            return equinfhitchs;
        }

        /// <summary>
        /// 将异常信息值为已读
        /// </summary>
        /// <param name="EquInfHitchId"></param>
        public void UpdateReadEquInfHitch(string EquInfHitchId)
        {
            SelfDber.Execute(" update " + EntityReflectionUtil.GetTableName<InfEquInfHitch>() + " t set t.isread=1 where t.id='" + EquInfHitchId + "' ");
        }

        /// <summary>
        /// 获取当日未读取的管控系统消息
        /// </summary>
        /// <returns></returns>
        public CmcsSysMessage GetTodayTopSysMessage()
        {
            return SelfDber.Entity<CmcsSysMessage>("where MsgStatus=:MsgStatus and MsgTime like '%' || :MsgTime || '%' order by MsgTime", new { MsgStatus = eSysMessageStatus.默认.ToString(), MsgTime = DateTime.Now.ToString("yyyy-MM-dd") });
        }

        /// <summary>
        /// 更改系统消息的状态
        /// </summary>
        /// <param name="sysMessageId"></param>
        /// <param name="sysMessageStatus"></param>
        public void ChangeSysMessageStatus(string sysMessageId, eSysMessageStatus sysMessageStatus)
        {
            SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsSysMessage>() + " t set t.MsgStatus=:MsgStatus where t.Id=:Id", new { Id = sysMessageId, MsgStatus = sysMessageStatus.ToString() });
        }

        /// <summary>
        /// 将所有上次提醒数据置换为已处理
        /// </summary>
        public void ResetAllSysMessageStatus()
        {
            SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsSysMessage>() + " t set t.MsgStatus=:MsgStatus1 where t.MsgStatus=:MsgStatus1", new { MsgStatus1 = eSysMessageStatus.处理中.ToString(), MsgStatus2 = eSysMessageStatus.处理中.ToString() });
        }

        /// <summary>
        /// 添加提示信息
        /// </summary>
        /// <param name="msgCode">编码</param>
        /// <param name="msgContent">内容</param>
        /// <param name="windowTitle">标题</param>
        /// <param name="msgButton">按钮名称</param>
        /// <param name="msgWarnType">是否右下角弹出</param>
        /// <param name="isAutoClose">是否自动关闭</param>
        /// <param name="msgParam">JSON</param>
        /// <returns></returns>
        public bool SaveSysMessage(String msgCode, String msgContent, String windowTitle = "提示", String msgButton = "查看|取消", bool msgWarnType = true, bool isAutoClose = false, String msgParam = "")
        {
            if (!HasSysMessage(msgCode, msgContent))
            {
                return SelfDber.Insert(new CmcsSysMessage
                {
                    MsgTime = DateTime.Now,
                    MsgParam = msgParam,
                    MsgContent = msgContent,
                    WindowsTitle = windowTitle,
                    MsgWarnType = msgWarnType ? 1 : 0,
                    IsAutoClose = isAutoClose ? 1 : 0,
                    MsgCode = msgCode,
                    MsgButton = msgButton,
                }) > 0;
            }

            return false;
        }

        /// <summary>
        /// 是否存在未处理的数据
        /// </summary>
        /// <param name="msgCode">编码</param>
        /// <param name="msgContent">内容</param>
        /// <returns></returns>
        public bool HasSysMessage(String msgCode, String msgContent)
        {
            return SelfDber.Count<CmcsSysMessage>("where (MsgStatus=:MsgStatus1 or MsgStatus=:MsgStatus2) and MsgContent=:MsgContent and MsgCode=:MsgCode", new { MsgStatus1 = eSysMessageStatus.处理中.ToString(), MsgStatus2 = eSysMessageStatus.处理中.ToString(), MsgContent = msgContent, MsgCode = msgCode }) > 0;
        }

        #endregion

        #region 基础信息

        /// <summary>
        /// 根据供煤单位名称或首字母模糊查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlWhere">条件语句</param>
        /// <returns></returns>
        public List<CmcsSupplier> GetSupplierByNameOrChs(string name, string sqlWhere)
        {
            List<CmcsSupplier> res = SelfDber.Entities<CmcsSupplier>(sqlWhere);

            if (!string.IsNullOrEmpty(name))
            {
                return res.Where(a =>
                {
                    if (a.Name.ToUpper().Contains(name.ToUpper())) return true;
                    if (ChsSpeller.GetFirst(a.Name).ToUpper().Contains(name.ToUpper())) return true;

                    return false;
                }).ToList();
            }
            else
                return res;
        }

        /// <summary>
        /// 根据矿点名称或首字母模糊查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlWhere">条件语句</param>
        /// <returns></returns>
        public List<CmcsMine> GetMineByNameOrChs(string name, string sqlWhere)
        {
            List<CmcsMine> res = SelfDber.Entities<CmcsMine>(sqlWhere);

            if (!string.IsNullOrEmpty(name))
            {
                return res.Where(a =>
                {
                    if (a.Name.ToUpper().Contains(name.ToUpper())) return true;
                    if (ChsSpeller.GetFirst(a.Name).ToUpper().Contains(name.ToUpper())) return true;

                    return false;
                }).ToList();
            }
            else
                return res;
        }

        /// <summary>
        /// 根据运输单位名称或首字母模糊查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlWhere">条件语句</param>
        /// <returns></returns>
        public List<CmcsTransportCompany> GetTransportCompanyByNameOrChs(string name, string sqlWhere)
        {
            List<CmcsTransportCompany> res = SelfDber.Entities<CmcsTransportCompany>(sqlWhere);

            if (!string.IsNullOrEmpty(name))
            {
                return res.Where(a =>
                {
                    if (a.Name.ToUpper().Contains(name.ToUpper())) return true;
                    if (ChsSpeller.GetFirst(a.Name).ToUpper().Contains(name.ToUpper())) return true;

                    return false;
                }).ToList();
            }
            else
                return res;
        }

        /// <summary>
        /// 根据供货收货单位名称或首字母模糊查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sqlWhere">条件语句</param>
        /// <returns></returns>
        public List<CmcsSupplyReceive> GetSupplyReceiveByNameOrChs(string name, string sqlWhere)
        {
            List<CmcsSupplyReceive> res = SelfDber.Entities<CmcsSupplyReceive>(sqlWhere);

            if (!string.IsNullOrEmpty(name))
            {
                return res.Where(a =>
                {
                    if (a.UnitName.ToUpper().Contains(name.ToUpper())) return true;
                    if (ChsSpeller.GetFirst(a.UnitName).ToUpper().Contains(name.ToUpper())) return true;

                    return false;
                }).ToList();
            }
            else
                return res;
        }

        #endregion

        #region 车辆管理

        /// <summary>
        /// 根据车牌号获取车辆信息
        /// </summary>
        /// <param name="carNumber"></param>
        /// <returns></returns>
        public CmcsAutotruck GetAutotruckByCarNumber(string carNumber)
        {
            return SelfDber.Entity<CmcsAutotruck>("where CarNumber=:CarNumber", new { CarNumber = carNumber });
        }

        /// <summary>
        /// 根据标签卡号获取车辆信息
        /// </summary>
        /// <param name="carNumber"></param>
        /// <returns></returns>
        public CmcsAutotruck GetAutotruckByTagId(string tagId)
        {
            CmcsEPCCard ePCCard = SelfDber.Entity<CmcsEPCCard>("where TagId=:TagId", new { TagId = tagId });
            if (ePCCard != null) return SelfDber.Entity<CmcsAutotruck>("where EPCCardId=:EPCCardId", new { EPCCardId = ePCCard.Id });

            return null;
        }

        #endregion

        #region 程序远程控制

        /// <summary>
        /// 发送程序远程控制命令
        /// </summary>
        /// <param name="appIdentifier">程序唯一标识符</param>
        /// <param name="cmdCode">命令代码</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public bool SendAppRemoteControlCmd(string appIdentifier, string cmdCode, string param = "")
        {
            return SelfDber.Insert(new CmcsAppRemoteControlCmd
               {
                   AppIdentifier = appIdentifier,
                   CmdCode = cmdCode,
                   Param = param,
                   ResultCode = eEquInfCmdResultCode.默认.ToString(),
                   DataFlag = 0
               }) > 0;
        }

        /// <summary>
        /// 重置程序远程控制命令
        /// </summary>
        /// <param name="appIdentifier"></param>
        public void ResetAppRemoteControlCmd(string appIdentifier)
        {
            SelfDber.Execute("update " + EntityReflectionUtil.GetTableName<CmcsAppRemoteControlCmd>() + " set DataFlag=1 where AppIdentifier=:AppIdentifier", new { AppIdentifier = appIdentifier });
        }

        /// <summary>
        /// 获取最新的程序远程控制命令
        /// </summary>
        /// <param name="appIdentifier">程序唯一标识</param>
        /// <returns></returns>
        public CmcsAppRemoteControlCmd GetNewestAppRemoteControlCmd(string appIdentifier)
        {
            return SelfDber.Entity<CmcsAppRemoteControlCmd>("where AppIdentifier=:AppIdentifier and DataFlag=0 order by CreateDate asc", new { AppIdentifier = appIdentifier });
        }

        /// <summary>
        /// 获取最新的程序远程控制命令
        /// </summary>
        /// <param name="appIdentifier">程序唯一标识</param>
        /// <returns></returns>
        public CmcsAppRemoteControlCmd GetNewestAppRemoteControlCmd(string appIdentifier, string cmdCode)
        {
            return SelfDber.Entity<CmcsAppRemoteControlCmd>("where AppIdentifier=:AppIdentifier and CmdCode=:CmdCode order by CreateDate asc", new { AppIdentifier = appIdentifier, CmdCode = cmdCode });
        }

        /// <summary>
        /// 更改程序远程控制命令的执行结果
        /// </summary>
        /// <param name="appRemoteControlCmd"></param>
        /// <param name="cmdResultCode"></param>
        public void SetAppRemoteControlCmdResultCode(CmcsAppRemoteControlCmd appRemoteControlCmd, eEquInfCmdResultCode cmdResultCode)
        {
            appRemoteControlCmd.DataFlag = 1;
            appRemoteControlCmd.ResultCode = cmdResultCode.ToString();
            SelfDber.Update(appRemoteControlCmd);
        }

        #endregion

        #region 入厂煤批次

        /// <summary>
        /// 生成制定日期的批次编码
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <param name="dt">实际到厂时间</param>
        /// <returns></returns>
        public string CreateNewBatchNumber(string prefix, DateTime dt)
        {
            CmcsInFactoryBatch entity = SelfDber.Entity<CmcsInFactoryBatch>("where Batch like '%-'||to_char(:CreateDate,'YYYYMMDD')||'-%' and Batch like :Prefix || '%' order by Batch desc", new { CreateDate = dt, Prefix = prefix + "-" + dt.ToString("yyyyMMdd") });

            if (entity == null)
                return string.Format("{0}-{1}-01", prefix, dt.ToString("yyyyMMdd"));
            else
                return string.Format("{0}-{1}-{2}", prefix, dt.ToString("yyyyMMdd"), (Convert.ToInt16(entity.Batch.Replace(string.Format("{0}-{1}-", prefix, dt.ToString("yyyyMMdd")), "")) + 1).ToString().PadLeft(2, '0'));
        }

        #endregion

        #region 采制化三级编码

        /// <summary>
        /// 生成采样码，分段001-099
        /// 汽车入厂采样编码规则：RCCY20171121011 （入厂采样-时间-随机数三位）。
        /// </summary> 
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CreateNewSampleCode(DateTime dt)
        {
            string res = string.Empty;
            do
            {
                res = string.Format("{0}{1}", dt.ToString("yyyyMMdd"), new Random().Next(0, 100).ToString().PadLeft(3, '0'));
                res = "RCCY" + res;
            } while (SelfDber.Count<CmcsRCSampling>("where SampleCode=:SampleCode", new { SampleCode = res }) > 0);

            return res;
        }

        /// <summary>
        /// 生成制样码，分段100-199
        /// 汽车入厂制样编码规则：RCZY20171121011 （入厂制样-时间-随机数三位）。
        /// </summary>
        /// <returns></returns>
        public string CreateNewMakeCode(DateTime dt)
        {
            string res = string.Empty;
            do
            {
                res = string.Format("{0}{1}", dt.ToString("yyyyMMdd"), new Random().Next(100, 200).ToString().PadLeft(3, '0'));
                res = "RCZY" + res;
            } while (SelfDber.Count<CmcsRCMake>("where MakeCode=:MakeCode", new { MakeCode = res }) > 0);

            return res;
        }

        /// <summary>
        /// 生成制样明细样品码，分段300-499
        /// </summary>
        /// <returns></returns>
        public string CreateNewMakeBarrelCode(DateTime dt)
        {
            string res = string.Empty;
            do
            {
                res = string.Format("{0}{1}", dt.ToString("yyMMdd"), new Random().Next(300, 500).ToString().PadLeft(3, '0'));
            } while (SelfDber.Count<CmcsRCMakeDetail>("where BarrelCode=:BarrelCode", new { BarrelCode = res }) > 0);

            return res;
        }

        /// <summary>
        /// 生成化验码，分段200-299
        /// </summary>
        /// <returns></returns>
        public string CreateNewAssayCode(DateTime dt)
        {
            string res = string.Empty;
            do
            {
                res = string.Format("{0}{1}", dt.ToString("yyMMdd"), new Random().Next(200, 300).ToString().PadLeft(3, '0'));
            } while (SelfDber.Count<CmcsRCAssay>("where BillNumber=:AssayCode", new { AssayCode = res }) > 0);

            return res;
        }

        /// <summary>
        /// 创建采制化三级数据
        /// </summary>
        /// <param name="inFactoryBatch">批次</param>
        /// <param name="samplingType">采样方式</param>
        /// <param name="remark">备注</param>
        /// <param name="assayType">化验方式</param>
        /// <param name="Category">类别 入炉 入厂</param>
        /// <returns></returns>
        public CmcsRCSampling GCSamplingMakeAssay(CmcsInFactoryBatch inFactoryBatch, string samplingType, string remark, eAssayType assayType, string Category)
        {
            bool isSuccess = false;

            // 入厂煤采样
            //CmcsRCSampling rCSampling = SelfDber.Entity<CmcsRCSampling>("where InFactoryBatchId=:InFactoryBatchId and SamplingType=:SamplingType", new { InFactoryBatchId = inFactoryBatch.Id, SamplingType = samplingType });
            CmcsRCSampling rCSampling = SelfDber.Entity<CmcsRCSampling>("where InFactoryBatchId=:InFactoryBatchId and Remark='由汽车煤智能化自动创建'", new { InFactoryBatchId = inFactoryBatch.Id });
            if (rCSampling == null)
            {
                rCSampling = new CmcsRCSampling()
                {
                    InFactoryBatchId = inFactoryBatch.Id,
                    SamplingType = samplingType,
                    SamplingDate = DateTime.Now,
                    SamplingPle = "自动",
                    SampleCode = CreateNewSampleCode(inFactoryBatch.FactArriveDate),
                    Remark = remark,
                    SamplingCategory = Category == "入厂" ? "入厂采样" : "入炉采样"
                };

                isSuccess = SelfDber.Insert(rCSampling) > 0;
            }

            // 入厂煤制样
            CmcsRCMake rCMake = SelfDber.Entity<CmcsRCMake>("where SamplingId=:SamplingId", new { SamplingId = rCSampling.Id });
            if (rCMake == null)
            {
                rCMake = new CmcsRCMake()
                {
                    SamplingId = rCSampling.Id,
                    MakeStyle = "机器制样",
                    MakeStartTime = rCSampling.CreateDate,
                    MakeEndTime = rCSampling.CreateDate,
                    MakeCode = CreateNewMakeCode(rCSampling.CreateDate),
                    MakePle = "自动",
                    Remark = remark,
                    MakeCategory = Category == "入厂" ? "入厂制样" : "入炉制样"
                };

                isSuccess = SelfDber.Insert(rCMake) > 0;

                //生成制样信息的同时生成制样明细
                IList<CodeContent> codeContent = GetCodeContentByKind("样品类型");
                if (isSuccess && codeContent.Count > 0)
                {
                    foreach (var item in codeContent)
                    {
                        CmcsRCMakeDetail detail = new CmcsRCMakeDetail();
                        detail.MakeId = rCMake.Id;
                        detail.SampleType = item.Code;
                        isSuccess = SelfDber.Insert(detail) > 0;
                    }
                }
            }


            // 入厂煤化验
            CmcsRCAssay rCAssay = SelfDber.Entity<CmcsRCAssay>("where MakeId=:MakeId", new { MakeId = rCMake.Id });
            if (rCAssay == null)
            {
                // 入厂煤煤质
                CmcsFuelQuality fuelQuality = new CmcsFuelQuality()
                {
                    Id = Guid.NewGuid().ToString()
                };

                if (SelfDber.Insert(fuelQuality) > 0)
                {
                    rCAssay = new CmcsRCAssay()
                    {
                        MakeId = rCMake.Id,
                        AssayType = assayType.ToString(),
                        BillNumber = CreateNewAssayCode(rCMake.CreateDate),
                        InFactoryBatchId = inFactoryBatch.Id,
                        FuelQualityId = fuelQuality.Id,
                        AssayDate = rCMake.CreateDate,
                        Remark = remark,
                        AssayCategory = Category == "入厂" ? "入厂化验" : "入炉化验"
                    };

                    isSuccess = SelfDber.Insert(rCAssay) > 0;
                }
            }

            return rCSampling;
        }

        /// <summary>
        /// 根据采样单Id获取批次
        /// </summary>
        /// <returns></returns>
        /// <param name="rCSamplingId">采样单Id</param>
        public CmcsInFactoryBatch GetBatchByRCSamplingId(string rCSamplingId)
        {
            CmcsRCSampling rCSampling = SelfDber.Get<CmcsRCSampling>(rCSamplingId);
            if (rCSampling != null) return SelfDber.Get<CmcsInFactoryBatch>(rCSampling.InFactoryBatchId);

            return null;
        }

        #endregion

        #region 数据待处理事件

        /// <summary>
        /// 插入数据待处理事件
        /// </summary>
        /// <param name="eventCode">事件代码</param>
        /// <param name="objectId">业务记录Id</param>
        /// <param name="paramValue1">附加参数1</param>
        /// <param name="paramValue2">附加参数2</param>
        /// <returns></returns>
        public bool InsertWaitForHandleEvent(string eventCode, string objectId, string paramValue1 = "", string paramValue2 = "")
        {
            return SelfDber.Insert(new CmcsWaitForHandleEvent
            {
                EventCode = eventCode,
                ObjectId = objectId,
                ParamValue1 = paramValue1,
                ParamValue2 = paramValue2,
                DataFlag=0
            }) > 0;
        }

        /// <summary>
        /// 移除未完成运输记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <returns></returns>
        public bool RemoveUnFinishTransport(string transportId)
        {
            //删除临时运输记录
            CmcsUnFinishTransport unFinishTransport = Dbers.GetInstance().SelfDber.Entity<CmcsUnFinishTransport>("where TransportId=:TransportId", new { TransportId = transportId });
            if (unFinishTransport != null)
                return Dbers.GetInstance().SelfDber.Delete<CmcsUnFinishTransport>(unFinishTransport.Id) > 0;
            return false;
        }

        /// <summary>
        /// 删除扣吨记录
        /// </summary>
        /// <param name="transportId"></param>
        /// <returns></returns>
        public bool RemoveDeduct(string transportId)
        {
            //删除扣吨记录
            return Dbers.GetInstance().SelfDber.DeleteBySQL<CmcsBuyFuelTransportDeduct>("where TransportId=:TransportId", new { TransportId = transportId }) > 0;
        }

        #endregion

        #region 辅助工具

        /// <summary>
        /// 检测IP是否能Ping通
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool TestPing(string ip)
        {
            Ping testping = new Ping();
            PingReply pingReply = testping.Send(ip);
            if (pingReply.Status == IPStatus.Success)
                return true;
            return false;
        }
        #endregion

        #region 抓拍图片
        /// <summary>
        /// 获取待上传图片路径
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitForUploadPicture()
        {
            return SelfDber.Entities<CmcsTransportPicture>("where IsUpLoad=0").Select(a => a.PicturePath).ToList();
        }
        #endregion

        #region 采样机接口

        /// <summary>
        /// 获取采样机的集样罐清单
        /// </summary>
        /// <param name="machineCode">设备编号</param>
        /// <returns></returns>
        public List<InfEquInfSampleBarrel> GetEquInfSampleBarrels(string machineCode)
        {
            return Dbers.GetInstance().SelfDber.Entities<InfEquInfSampleBarrel>("where MachineCode=:MachineCode order by BarrelType,BarrelNumber asc", new { MachineCode = machineCode });
        }

        #endregion

        #region 制样机接口
        public String ConvertToJxzyYPLX(String OrderNum)
        {
            try
            {
                CodeContent codecontent = CommonDAO.GetInstance().GetCodeContentByKind("机器制样类型").Where(a => a.Code == OrderNum).FirstOrDefault();
                if (codecontent == null)
                {
                    return OrderNum.ToString();
                }
                return codecontent.Content;

            }
            catch (Exception)
            {
                return OrderNum.ToString();
            }
        }

        /// <summary>
        /// 序号获取样品名字
        /// </summary>
        /// <param name="OrderNum">样品编号</param>
        /// <returns></returns>
        public String ConvertToCygYPLX(int OrderNum)
        {
            try
            {
                CodeContent codecontent = CommonDAO.GetInstance().GetCodeContentByKind("样品类型").Where(a => a.CodeOrder == OrderNum).FirstOrDefault();
                if (codecontent == null)
                {
                    return OrderNum.ToString();
                }
                return codecontent.Content;

            }
            catch (Exception)
            {
                return OrderNum.ToString();
            }
        }
        /// <summary>
        /// 用名字获取样品类型序号
        /// </summary>
        /// <param name="CygYPLX">样品类型</param>
        /// <returns></returns>
        public int GetCygYPLXByName(String CygYPLX)
        {
            try
            {
                CodeContent codecontent = CommonDAO.GetInstance().GetCodeContentByKind("样品类型").Where(a => a.Content == CygYPLX).FirstOrDefault();
                if (codecontent == null)
                {
                    return 0;
                }
                return codecontent.CodeOrder;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion

        #region 智能存样柜自动弃样
        //根据样品类型获取该样品的过期天数
        public int getRemark(string YangpinType, string kind)
        {
            int remark = 0;
            string sql = @"select a.code,a.remark from syssmtbcodecontent a left join syssmtbcodekind b on a.kindid=b.id where b.kind='{0}' and a.code='{1}'";
            sql = string.Format(sql, kind, YangpinType);
            DataTable tb = Dbers.GetInstance().SelfDber.ExecuteDataTable(sql);
            if (tb.Rows.Count > 0)
            {
                remark = string.IsNullOrEmpty(tb.Rows[0]["remark"].ToString()) ? 0 : int.Parse(tb.Rows[0]["remark"].ToString());
            }
            return remark;
        }
        #endregion
    }
}
