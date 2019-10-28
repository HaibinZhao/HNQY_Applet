using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.WeighCheck.MakeChange.Enums
{
    /// <summary>
    /// 流程标识
    /// </summary>
    public enum eFlowFlag
    {
        等待扫码,
        样品登记,
        等待校验,
        校验成功,
        打印化验码
    }
}
