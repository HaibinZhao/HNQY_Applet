// 此代码由 NhGenerator v1.0.9.0 工具生成。

using System;
using System.Collections;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.CarTransport
{
    /// <summary>
    /// 汽车智能化-卸煤沟LED
    /// </summary>
    [Serializable]
    [CMCS.DapperDber.Attrs.DapperBind("CmcsTbUnLoadLED")]
    public class CmcsUnLoadLED : EntityBase1
    {
        private Int32 _Number;
        /// <summary>
        /// LED屏编号　
        /// </summary>
        public virtual Int32 UnLoadNumber { get { return _Number; } set { _Number = value; } }

        private String _Name;
        /// <summary>
        /// 卸煤沟名称
        /// </summary>
        public virtual String UnLoadName { get { return _Name; } set { _Name = value; } }

        private String _IP;
        /// <summary>
        /// IP 
        /// </summary>
        public virtual String IP { get { return _IP; } set { _IP = value; } }

        private String _CarNumber;
        /// <summary>
        /// 待显示车牌号 多个车牌号用 | 隔开 最多显示2个车牌号
        /// </summary>
        public virtual String CarNumber { get { return _CarNumber; } set { _CarNumber = value; } }

        private String _Batch;
        /// <summary>
        /// 当前批次
        /// </summary>
        public virtual String Batch { get { return _Batch; } set { _Batch = value; } }

        private Int32 _IsUse;
        /// <summary>
        /// 是否正在使用
        /// </summary>
        public virtual Int32 IsUse { get { return _IsUse; } set { _IsUse = value; } }
    }
}
