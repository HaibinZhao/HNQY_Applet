using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 入炉指令
    /// </summary>
    [Serializable]
    [CMCS.DapperDber.Attrs.DapperBind("FULTBINFURNACE")]
    public class CmcsInFurnace : EntityBase1
    {
        private String _IsUpload;
        /// <summary>
        /// 是否上传
        /// </summary>
        public virtual String IsUpload { get { return _IsUpload; } set { _IsUpload = value; } }

        private String _DataFrom;
        /// <summary>
        /// 数据来源
        /// </summary>
        public virtual String DataFrom { get { return _DataFrom; } set { _DataFrom = value; } }

        private String _Stages;
        /// <summary>
        /// 分期
        /// </summary>
        public virtual String Stages { get { return _Stages; } set { _Stages = value; } }

        private String _ClassCyc;
        /// <summary>
        /// 班次
        /// </summary>
        public virtual String ClassCyc { get { return _ClassCyc; } set { _ClassCyc = value; } }

        private String _DutyCyc;
        /// <summary>
        /// 值次
        /// </summary>
        public virtual String DutyCyc { get { return _DutyCyc; } set { _DutyCyc = value; } }

        private String _RecorderName;
        /// <summary>
        /// 记录人名称
        /// </summary>
        public virtual String RecorderName { get { return _RecorderName; } set { _RecorderName = value; } }

        private DateTime _RecordDate;
        /// <summary>
        /// 记录时间
        /// </summary>
        public virtual DateTime RecordDate { get { return _RecordDate; } set { _RecordDate = value; } }

        private String _MachineUnit;
        /// <summary>
        /// 入炉机组
        /// </summary>
        public virtual String MachineUnit { get { return _MachineUnit; } set { _MachineUnit = value; } }

        private String _CoalPot;
        /// <summary>
        /// 原煤仓
        /// </summary>
        public virtual String CoalPot { get { return _CoalPot; } set { _CoalPot = value; } }

        private String _CoalpotName;
        /// <summary>
        /// 原煤仓名称
        /// </summary>
        public virtual String CoalpotName { get { return _CoalpotName; } set { _CoalpotName = value; } }

        private String _ElecscaleName;
        /// <summary>
        /// 计量电子秤名称
        /// </summary>
        public virtual String ElecscaleName { get { return _ElecscaleName; } set { _ElecscaleName = value; } }

        private Decimal _StartReading;
        /// <summary>
        /// 电子秤起始读数
        /// </summary>
        public virtual Decimal StartReading { get { return _StartReading; } set { _StartReading = value; } }

        private Decimal _EndReading;
        /// <summary>
        /// 电子秤结束读数
        /// </summary>
        public virtual Decimal EndReading { get { return _EndReading; } set { _EndReading = value; } }

        private DateTime _StartTime;
        /// <summary>
        /// 入炉开始时间
        /// </summary>
        public virtual DateTime StartTime { get { return _StartTime; } set { _StartTime = value; } }

        private DateTime _EndTime;
        /// <summary>
        /// 入炉结束时间
        /// </summary>
        public virtual DateTime EndTime { get { return _EndTime; } set { _EndTime = value; } }

        private Decimal _TotalQty;
        /// <summary>
        /// 总上煤量(吨)
        /// </summary>
        public virtual Decimal TotalQty { get { return _TotalQty; } set { _TotalQty = value; } }

        private Decimal _TotalQty_Sb;
        /// <summary>
        /// 设备总上煤量(吨)
        /// </summary>
        public virtual Decimal TotalQty_Sb { get { return _TotalQty_Sb; } set { _TotalQty_Sb = value; } }

        private String _ExecState;
        /// <summary>
        /// 执行状态。未执行、执行中、执行结束
        /// </summary>
        public virtual String ExecState { get { return _ExecState; } set { _ExecState = value; } }

        private String _FuelSchemeID;
        /// <summary>
        /// 取煤方案id
        /// </summary>
        public virtual String FuelSchemeID { get { return _FuelSchemeID; } set { _FuelSchemeID = value; } }

        private String _WeightingQuality;
        /// <summary>
        /// 入炉加权煤质
        /// </summary>
        public virtual String WeightingQuality { get { return _WeightingQuality; } set { _WeightingQuality = value; } }

        private String _Quality;
        /// <summary>
        /// 入炉化验煤质
        /// </summary>
        public virtual String Quality { get { return _Quality; } set { _Quality = value; } }

        private Decimal _InitialPrice;
        /// <summary>
        /// 原煤价（元/吨）
        /// </summary>
        public virtual Decimal InitialPrice { get { return _InitialPrice; } set { _InitialPrice = value; } }

        private Decimal _StandardPrice;
        /// <summary>
        /// 标煤价（元/吨）
        /// </summary>
        public virtual Decimal StandardPrice { get { return _StandardPrice; } set { _StandardPrice = value; } }

        private String _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public virtual String Remark { get { return _Remark; } set { _Remark = value; } }
    }
}
