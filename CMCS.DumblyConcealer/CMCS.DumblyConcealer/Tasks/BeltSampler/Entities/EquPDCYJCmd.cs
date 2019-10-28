using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.BeltSampler.Entities
{
    /// <summary>
    /// 皮带采样机接口表 - 控制命令
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("EquTbHCPDCYJCmd")]
    public class EquPDCYJCmd : EntityBase2
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

        private string cmdCode;
        /// <summary>
        /// 命令代码
        /// </summary>
        public string CmdCode
        {
            get { return cmdCode; }
            set { cmdCode = value; }
        }

        private string sampleCode;
        /// <summary>
        /// 采样码
        /// </summary>
        public string SampleCode
        {
            get { return sampleCode; }
            set { sampleCode = value; }
        }

        private string resultCode;
        /// <summary>
        /// 执行结果
        /// </summary>
        public string ResultCode
        {
            get { return resultCode; }
            set { resultCode = value; }
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
