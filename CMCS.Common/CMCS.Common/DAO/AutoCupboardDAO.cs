using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.AutoCupboard;
using CMCS.Common.Enums;
using CMCS.Common.Enums.AutoCupboard;

namespace CMCS.Common.DAO
{
    /// <summary>
    /// 存样柜业务
    /// </summary>
    public class AutoCupboardDAO
    {
        private static AutoCupboardDAO instance;

        public static AutoCupboardDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new AutoCupboardDAO();
            }

            return instance;
        }

        private AutoCupboardDAO()
        { }

        /// <summary>
        /// 添加存样柜命令
        /// </summary>
        /// <param name="code">样品码</param>
        /// <param name="machineCode">设备编码</param>
        /// <param name="operType">操作类型 存样 取样 弃样</param>
        /// <param name="operUser">操作人</param>
        /// <returns></returns>
        public bool SaveAutoCupboardCmd(string code, string machineCode, eCZPLX operType, string operUser = "自动")
        {
            InfCYGControlCMD cmd = new InfCYGControlCMD();
            cmd.PlanDate = DateTime.Now;
            cmd.Bill = "CYG" + DateTime.Now.ToString("yyMMdd") + CreateBillNumber().ToString().PadLeft(3, '0');
            cmd.OperPerson = operUser;
            cmd.OperType = operType.ToString();
            cmd.CodeNumber = code;
            cmd.MachineCode = machineCode;
            cmd.ResultCode = eEquInfCmdResultCode.默认.ToString();
            return CommonDAO.GetInstance().SelfDber.Insert(cmd) > 0;
        }

        /// <summary>
        /// 根据制样码获取存样柜命令执行结果
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public eEquInfCYGCmdResultCode GetAutoCupboardResult(string code)
        {
            eEquInfCYGCmdResultCode eResult = eEquInfCYGCmdResultCode.默认;
            InfCYGControlCMD result = Dbers.GetInstance().SelfDber.Entity<InfCYGControlCMD>("where CodeNumber=:CodeNumber order by CreateDate desc", new { CodeNumber = code });
            if (result != null)
                Enum.TryParse<eEquInfCYGCmdResultCode>(result.ResultCode, out eResult);
            return eResult;
        }

        /// <summary>
        /// 检测存样柜是否可以发送命令
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public bool CheckCanSaveCmd(string machineCode)
        {
            ///存样柜总体状态为3 就绪待机 时 可以发送存样柜命令
            return CommonDAO.GetInstance().GetSignalDataValue(machineCode, "总体状态") == "3";
        }

        /// <summary>
        /// 获取存样柜命令编号
        /// </summary>
        /// <returns></returns>
        public int CreateBillNumber()
        {
            int newbill = 1;
            try
            {
                newbill = Convert.ToInt32(Dbers.GetInstance().SelfDber.Entities<InfCYGControlCMD>(" where Bill like '" + "CYG" + DateTime.Now.ToString("yyMMdd") + "%'").Max(a => a.Bill).Substring(9, 3)) + 1;
            }
            catch (Exception)
            {
            }
            return newbill;
        }

        /// <summary>
        /// 根据样品编码转换为样瓶类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int YPLXChangeByCode(string code)
        {
            /*1=6mm样瓶
              2=3mm样瓶
              3=0.2mm样瓶
              4=总经理备查样*/

            string YPNum = code.Substring(code.Length - 2, 1);
            int YPLX = 0;
            switch (YPNum)
            {
                case "2":
                    YPLX = 3;
                    break;
                case "3":
                    YPLX = 2;
                    break;
                case "6":
                    YPLX = 1;
                    break;
                default:
                    YPLX = 4;
                    break;
            }
            return YPLX;
        }

        /// <summary>
        /// 根据制样编码转换制样类型
        /// </summary>
        /// <param name="makeCode"></param>
        /// <returns></returns>
        public string GetMakeTypeByMakeCode(string makeCode)
        {
            /*
             21 0.2mm一般分析实验样
             22 0.2mm备查样
             31 3mm存查样一
             32 3mm存查样二
             33 3mm存查样三
             34 3mm存查样四
             61 6mm全水分样一
             62 6mm全水分样二
             */
            string makeCodeSub = makeCode.Substring(makeCode.Length - 2, 2);
            string makeType = string.Empty;
            switch (makeCodeSub)
            {
                case "21":
                    makeType = "0.2mm一般分析实验样";
                    break;
                case "22":
                    makeType = "0.2mm备查样";
                    break;
                case "31":
                    makeType = "3mm存查样一";
                    break;
                case "32":
                    makeType = "3mm存查样二";
                    break;
                case "33":
                    makeType = "3mm存查样三";
                    break;
                case "34":
                    makeType = "3mm存查样四";
                    break;
                case "61":
                    makeType = "6mm全水分样一";
                    break;
                case "62":
                    makeType = "6mm全水分样二";
                    break;
                default:
                    break;
            }
            return makeType;
        }

        /// <summary>
        /// 获取当前可存样的存样柜
        /// </summary>
        /// <returns></returns>
        public string GetCYGMachineCode()
        {
            string cYG1Status = CommonDAO.GetInstance().GetSignalDataValue(GlobalVars.MachineCode_CYG1, eSignalDataName.总体状态.ToString());
            if (cYG1Status == ((int)eEquInfSystemStatus.就绪待机).ToString())
                return GlobalVars.MachineCode_CYG1;

            string cYG2Status = CommonDAO.GetInstance().GetSignalDataValue(GlobalVars.MachineCode_CYG2, eSignalDataName.总体状态.ToString());
            if (cYG2Status == ((int)eEquInfSystemStatus.就绪待机).ToString())
                return GlobalVars.MachineCode_CYG2;
            return string.Empty;
        }

        /// <summary>
        /// 检测存样柜的托盘是否已就位
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public bool CheckTPIsReady(string machineCode)
        {
            return CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.托盘状态.ToString()) == "1";
        }

        /// <summary>
        /// 检查存样柜是否就绪
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public bool CheckFree(string machineCode)
        {
            return CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.总体状态.ToString()) == ((int)eEquInfSystemStatus.就绪待机).ToString();
        }

        /// <summary>
        /// 检查存样柜是否故障
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public bool CheckCYGIsError(string machineCode)
        {
            return CommonDAO.GetInstance().GetSignalDataValue(machineCode, eSignalDataName.总体状态.ToString()) == ((int)eEquInfSystemStatus.发生故障).ToString();
        }
    }
}
