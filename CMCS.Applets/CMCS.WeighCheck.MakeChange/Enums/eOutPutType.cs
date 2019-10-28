using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CMCS.WeighCheck.MakeChange.Enums
{
    /// <summary>
    /// 输出信息类型
    /// </summary>
    public enum eOutputType
    {
        /// <summary>
        /// 普通
        /// </summary>
        [Description("#BD86FA")]
        Normal,
        /// <summary>
        /// 重要
        /// </summary>
        [Description("#A50081")]
        Important,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("#F9C916")]
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("#DB2606")]
        Error
    }

}
