using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Entities.Sys
{
    /// <summary>
    /// 待上传处理事件
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("CmcsTbWaitForHandleEvent")]
    public class CmcsWaitForHandleEvent : EntityBase1
    {
        /// <summary>
        /// 事件代码
        /// </summary>
        public String EventCode { get; set; }

        /// <summary>
        /// 业务记录Id
        /// </summary>
        public String ObjectId { get; set; }

        /// <summary>
        /// 附加参数1
        /// </summary>
        public String ParamValue1 { get; set; }

        /// <summary>
        /// 附加参数2
        /// </summary>
        public String ParamValue2 { get; set; }

        /// <summary>
        /// 数据标识
        /// </summary>
        public Int32 DataFlag { get; set; }
    }
}
