using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys; 

namespace CMCS.Common.Entities.BaseInfo
{
    /// <summary>
    /// 基础信息-矿点
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("fultbmine")]
    public class CmcsMine : EntityBase1
    {
        /// <summary>
        /// 编码
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public virtual String IsUse { get; set; }

        public String Valid { get; set; }
        public String ReMark { get; set; }
        public Int32 Sequence { get; set; }
        
    }
}
