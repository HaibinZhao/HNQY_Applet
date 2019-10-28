using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.BeltSampler.Entities
{
    /// <summary>
    /// 皮带采样机接口表 - 采样计划
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("EquTbHCPDCYJPlan")]
    public class EquPDCYJPlan : EntityBase2
    {
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

        private int mt;
        /// <summary>
        /// 水分
        /// </summary>
        public int Mt
        {
            get { return mt; }
            set { mt = value; }
        }

        private double ticketWeight;
        /// <summary>
        /// 该批次矿发量
        /// </summary>
        public double TicketWeight
        {
            get { return ticketWeight; }
            set { ticketWeight = value; }
        }

        private int carCount;
        /// <summary>
        /// 该批次车节数
        /// </summary>
        public int CarCount
        {
            get { return carCount; }
            set { carCount = value; }
        }

        private double coalSize;
        /// <summary>
        /// 颗粒度
        /// </summary>
        public double CoalSize
        {
            get { return coalSize; }
            set { coalSize = value; }
        }

        private string gatherType;
        /// <summary>
        /// 集样方式  底卸式、密码罐
        /// </summary>
        public string GatherType
        {
            get { return gatherType; }
            set { gatherType = value; }
        }

        private DateTime startTime;
        /// <summary>
        /// 采样开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        private DateTime endTime;
        /// <summary>
        /// 采样结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private string sampleUser;
        /// <summary>
        /// 采样员
        /// </summary>
        public string SampleUser
        {
            get { return sampleUser; }
            set { sampleUser = value; }
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
