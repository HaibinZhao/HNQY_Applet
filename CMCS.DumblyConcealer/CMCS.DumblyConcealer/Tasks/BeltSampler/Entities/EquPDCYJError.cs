using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.BeltSampler.Entities
{
    /// <summary>
    /// 故障信息表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("EquTbHCPDCYJError")]
    public class EquPDCYJError : EntityBase2
    { 
        private string machineCode;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string MachineCode
        {
            get { return machineCode; }
            set { machineCode = value; }
        }

        private string _ErrorCode;
        /// <summary>
        /// 故障代码
        /// </summary>
        public string ErrorCode
        {
            get { return _ErrorCode; }
            set { _ErrorCode = value; }
        }

        private DateTime _ErrorTime;
        /// <summary>
        /// 故障时间
        /// </summary>
        public DateTime ErrorTime
        {
            get { return _ErrorTime; }
            set { _ErrorTime = value; }
        }

        private string _ErrorDescribe;
        /// <summary>
        /// 故障描述
        /// </summary>
        public string ErrorDescribe
        {
            get { return _ErrorDescribe; }
            set { _ErrorDescribe = value; }
        }

        private int dataFlag;
        /// <summary>
        /// 标识符
        /// </summary>
        public int DataFlag
        {
            get { return dataFlag; }
            set { dataFlag = value; }
        }
    }
}
