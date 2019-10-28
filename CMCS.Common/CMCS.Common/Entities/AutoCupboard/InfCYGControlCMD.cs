using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.AutoCupboard
{
    /// <summary>
    /// 自动存样柜-操作任务命令表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("InfTBCYGCONTROLCMD")]
    public class InfCYGControlCMD : EntityBase1
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public virtual String MachineCode { get; set; }

        /// <summary>
        /// 计划时间
        /// </summary>
        public virtual DateTime PlanDate { get; set; }
        /// <summary>
        /// 命令编号 管控区别 不需要传入存样柜
        /// </summary>
        public virtual String Bill { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public virtual String OperPerson { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public virtual String OperType { get; set; }
        /// <summary>
        /// 样品编码
        /// </summary>
        public virtual String CodeNumber { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 标识 0 待同步 1 已同步 2 已发送至气动 3 已从存样柜同步结果 4 已从气动同步结果
        /// </summary>
        public virtual Decimal DataFlag { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual String ReMark { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public virtual DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public virtual DateTime EndTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual String Status { get; set; }
        /// <summary>
        /// 是否工作中
        /// </summary>
        public virtual String CanWorking { get; set; }
        /// <summary>
        /// 存样结果
        /// </summary>
        public virtual String ResultCode { get; set; }
    }
}
