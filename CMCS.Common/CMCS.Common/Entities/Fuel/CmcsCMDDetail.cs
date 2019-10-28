using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Entities.Fuel
{
    public class CmcsCMDDetail
    {
        /// <summary>
        /// 传输时间
        /// </summary>
        public virtual DateTime CreateDate { get; set; }
        /// <summary>
        /// 样品类型
        /// </summary>
        public virtual String SamType { get; set; }
        /// <summary>
        /// 样品编码
        /// </summary>
        public virtual String Code { get; set; }

        /// <summary>
        /// 传输状态
        /// </summary>
        public virtual String Status { get; set; }
    }
}
