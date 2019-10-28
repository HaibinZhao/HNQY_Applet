using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 采样机控制命令表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("SMPLECONTROLCMD")]
    public class SmpleConyrolCMD : EntityBase1
    {

        private String _MACHINECODE;
        /// <summary>
        /// 总体设备编号
        /// </summary>
        public String MACHINECODE { get { return _MACHINECODE; } set { _MACHINECODE = value; } }



        private String _PAUSE;
        /// <summary>
        ///暂停
        /// </summary>
        public String PAUSE { get { return _PAUSE; } set { _PAUSE = value; } }



        private String _RESUME;
        /// <summary>
        /// 恢复运行
        /// </summary>
        public String RESUME { get { return _RESUME; } set { _RESUME = value; } }



        private String _FAULT_RESET;
        /// <summary>
        /// 故障复位
        /// </summary>
        public String FAULT_RESET { get { return _FAULT_RESET; } set { _FAULT_RESET = value; } }



        private String _EQUT_RESET;
        /// <summary>
        /// 设备复位
        /// </summary>
        public String EQUT_RESET { get { return _EQUT_RESET; } set { _EQUT_RESET = value; } }



        private String _DATASTATUS;
        /// <summary>
        /// 0：未同步；1：已同步
        /// </summary>
        public String DATASTATUS { get { return _DATASTATUS; } set { _DATASTATUS = value; } }
    }
}
