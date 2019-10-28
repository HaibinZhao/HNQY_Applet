using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.BeltSampler.Entities
{
    /// <summary>
    /// 皮带采样机接口表 - 实时集样罐
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("EquTbHCPDCYJBarrel")]
    public class EquPDCYJBarrel : EntityBase2
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

        private string barrelNumber;
        /// <summary>
        /// 罐号
        /// </summary>
        public string BarrelNumber
        {
            get { return barrelNumber; }
            set { barrelNumber = value; }
        }

        private string barrelType;
        /// <summary>
        /// 样罐类型
        /// </summary>
        public string BarrelType
        {
            get { return barrelType; }
            set { barrelType = value; }
        }

        private string inFactoryBatchId;
        /// <summary>
        /// 批次Id
        /// </summary>
        public string InFactoryBatchId
        {
            get { return inFactoryBatchId; }
            set { inFactoryBatchId = value; }
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

        private int sampleCount;
        /// <summary>
        /// 子样数
        /// </summary>
        public int SampleCount
        {
            get { return sampleCount; }
            set { sampleCount = value; }
        }

        private int isCurrent;
        /// <summary>
        /// 是当前进料罐
        /// </summary>
        public int IsCurrent
        {
            get { return isCurrent; }
            set { isCurrent = value; }
        }

        private string barrelStatus;
        /// <summary>
        /// 桶满状态 未满、已满、空桶 
        /// </summary>
        public string BarrelStatus
        {
            get { return barrelStatus; }
            set { barrelStatus = value; }
        }

        private DateTime updateTime;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = value; }
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
