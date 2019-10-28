using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 入厂煤制样明细表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("cmcstbrcmakedetail")]
    public class CmcsRCMakeDetail : EntityBase1
    {
        private string _MakeId;

        /// <summary>
        /// 入厂煤制样Id
        /// </summary>
        public string MakeId
        {
            get { return _MakeId; }
            set { _MakeId = value; }
        }
        private string _BarrelCode;

        /// <summary>
        /// 样罐编码
        /// </summary>
        public string BarrelCode
        {
            get { return _BarrelCode; }
            set { _BarrelCode = value; }
        }
        private string _BackupCode;

        /// <summary>
        /// 备用码
        /// </summary>
        public string BackupCode
        {
            get { return _BackupCode; }
            set { _BackupCode = value; }
        }
        private string _SampleType;

        /// <summary>
        /// 样品类型
        /// </summary>
        public string SampleType
        {
            get { return _SampleType; }
            set { _SampleType = value; }
        }
        private double _Weight;

        /// <summary>
        /// 样重（克）
        /// </summary>
        public double Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        private decimal _CheckWeight;

        /// <summary>
        /// 校验样重
        /// </summary>
        public decimal CheckWeight
        {
            get { return _CheckWeight; }
            set { _CheckWeight = value; }
        }

        /// <summary>
        /// 装罐时间
        /// </summary>
        public DateTime BarrelTime { get; set; }

        [DapperDber.Attrs.DapperIgnore]
        public CmcsRCMake TheRCMake
        {
            get
            {
                return Dbers.GetInstance().SelfDber.Get<CmcsRCMake>(this.MakeId);
            }
        }
    }
}
