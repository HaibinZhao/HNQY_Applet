using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 采样方案
    /// </summary>
    [Serializable]
    [CMCS.DapperDber.Attrs.DapperBind("FULTBSamplingScheme")]
    public class CmcsSamplingScheme : EntityBase1
    {
        private string _TRANSFERID;
        /// <summary>
        /// 调运计划ID
        /// </summary>
        public virtual string TRANSFERID { get { return _TRANSFERID; } set { _TRANSFERID = value; } }


        //private string _MineId;
        ///// <summary>
        ///// 供煤单位
        ///// </summary>
        //public virtual string MineId { get { return _MineId; } set { _MineId = value; } }


        //private string _FuelKindId;
        ///// <summary>
        ///// 供煤单位
        ///// </summary>
        //public virtual string FuelKindId { get { return _FuelKindId; } set { _FuelKindId = value; } }


        //private Decimal _CarCount;
        ///// <summary>
        ///// 预报总车数
        ///// </summary>
        //public virtual Decimal CarCount { get { return _CarCount; } set { _CarCount = value; } }


        private Int32 _PointCount;
        /// <summary>
        /// 采样点数
        /// </summary>
        public virtual Int32 PointCount { get { return _PointCount; } set { _PointCount = value; } }


        //private Decimal _CoalQuantity;
        ///// <summary>
        ///// 预报总煤量
        ///// </summary>
        //public virtual Decimal CoalQuantity { get { return _CoalQuantity; } set { _CoalQuantity = value; } }


        private string _INFACTORYBATCHID;
        /// <summary>
        /// 批次ID
        /// </summary>
        public virtual string INFACTORYBATCHID { get { return _INFACTORYBATCHID; } set { _INFACTORYBATCHID = value; } }
    }
}
