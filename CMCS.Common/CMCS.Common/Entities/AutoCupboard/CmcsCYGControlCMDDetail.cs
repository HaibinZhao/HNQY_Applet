using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;
using CMCS.Common.Entities.AutoCupboard;

namespace CMCS.Common.Entities
{
    /// <summary>
    /// 集中管控设备
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("CMCSTBCYGCONTROLCMDDETAIL")]
    public class CmcsCYGControlCMDDetail : EntityBase1
    {
        public virtual String MachineCode { get; set; }
        /// <summary>
        /// 样品编码
        /// </summary>
        public virtual String Code { get; set; }
        /// <summary>
        /// 样品类型
        /// </summary>
        public virtual String SamType { get; set; }
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public virtual Decimal ResultCode { get; set; }
        public virtual String CYGControlCMDId { get; set; }
        public virtual String Status { get; set; }
        public virtual String Errors { get; set; }
        [DapperDber.Attrs.DapperIgnore]
        public CmcsCYGControlCMD TheCmcsCYGControlCMD
        {
            get
            {
                return Dbers.GetInstance().SelfDber.Get<CmcsCYGControlCMD>(this.CYGControlCMDId);
            }
        }
    }
}
