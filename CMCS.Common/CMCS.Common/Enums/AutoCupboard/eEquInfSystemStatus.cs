﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Enums.AutoCupboard
{
    /// <summary>
    /// 第三方设备接口 - 存样柜系统状态
    /// </summary>
    public enum eEquInfSystemStatus
    {
        正在运行 = 1,
        发生故障 = 2,
        就绪待机 = 3,
        正在卸样 = 4,
        离线状态 = 8
    }
}
