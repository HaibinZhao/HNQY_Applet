using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.CarJxSampler.Entities
{
    [CMCS.DapperDber.Attrs.DapperBind("CMD_TB")]
    public class CMD_TB : EntityBase3
    {
        private string _MACHINECODE;
        private string _PAUSE;
        private string _RESUME;
        private string _FAULT_RESET;


        private string _EQUT_RESET;
        private string _DATASTATUS;

        /// <summary>
        /// 总体设备编号
        /// </summary>
        public string MACHINECODE
        {
            get { return _MACHINECODE; }
            set { _MACHINECODE = value; }
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public string PAUSE
        {
            get { return _PAUSE; }
            set { _PAUSE = value; }
        }
        /// <summary>
        /// 恢复运行
        /// </summary>
        public string RESUME
        {
            get { return _RESUME; }
            set { _RESUME = value; }
        }

        /// <summary>
        /// 故障复位
        /// </summary>
        public string FAULT_RESET
        {
            get { return _FAULT_RESET; }
            set { _FAULT_RESET = value; }
        }
        /// <summary>
        /// 设备复位
        /// </summary>
        public string EQUT_RESET
        {
            get { return _EQUT_RESET; }
            set { _EQUT_RESET = value; }
        }
        /// <summary>
        /// 0：未读取；1：已读取
        /// </summary>
        public string DATASTATUS
        {
            get { return _DATASTATUS; }
            set { _DATASTATUS = value; }
        }
    }
}
