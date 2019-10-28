using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Enums.AutoCupboard
{
    /// <summary>
    /// 第三方设备接口 - 存样柜命令执行结果
    /// </summary>
    public enum eEquInfCYGCmdResultCode
    {
        默认 = 0,
        存样柜执行成功 = 1,
        存样柜执行失败 = 2,
        气动执行成功 = 3,
        气动执行失败 = 4
    }
}
