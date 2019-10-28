using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.PneumaticTransfer_XMJS.Entities
{

    /// <summary>
    /// 智能存查样信息 
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("CMCSTBCYGSAM")]
    public class CmcsCYGSams : EntityBase1
    {
        /// <summary>
        /// 样品编码
        /// </summary>
        public String Code { get; set; }
        /// <summary>
        /// 样品类型
        /// </summary>
        public String SamType { get; set; }
        ///// <summary>
        ///// 存样人
        ///// </summary>
        //public String SaveUserName { get; set; }
        /// <summary>
        /// 存样时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 存放位置
        /// </summary>
        public String Place { get; set; }
    }
}
