using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.AutoCupboard
{
    /// <summary>
    /// 实时样品表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("InfTBCYGSAM")]
    public class InfCYGSam : EntityBase1
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public virtual String MachineCode { get; set; }
        /// <summary>
        /// 样品码
        /// </summary>
        public virtual String Code { get; set; }
        /// <summary>
        /// 样瓶类型
        /// </summary>
        public virtual String SamType { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 是否有样瓶
        /// </summary>
        public virtual Decimal IsNew { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        public virtual Int32 CellIndex { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public virtual Int32 ColumnIndex { get; set; }
        /// <summary>
        /// 区域编号  左 1 右 2
        /// </summary>
        public virtual Int32 AreaNumber { get; set; }
        /// <summary>
        /// 柜号
        /// </summary>
        public virtual String Place { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public virtual Decimal DataFlag { get; set; }
    }
}
