using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.DumblyConcealer.Tasks.AutoMaker.Entities
{
    /// <summary>
    /// 全自动制样机接口表 - 制样计划表
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("InfTbMakePlan")]
    public class EquQZDZYJPlan
    {
        public string Id { get; set; }

        public DateTime CreateDate { get; set; }

        private string _InFactoryBatchId;
        /// <summary>
        /// 批次Id
        /// </summary>
        public string InFactoryBatchId
        {
            get { return _InFactoryBatchId; }
            set { _InFactoryBatchId = value; }
        }

        private string _MakeCode;
        /// <summary>
        /// 制样码
        /// </summary>
        [CMCS.DapperDber.Attrs.DapperPrimaryKey]
        public string MakeCode
        {
            get { return _MakeCode; }
            set { _MakeCode = value; }
        }

        private string _FuelKindName;
        /// <summary>
        /// 煤种
        /// </summary>
        public string FuelKindName
        {
            get { return _FuelKindName; }
            set { _FuelKindName = value; }
        }

        private double _Mt;
        /// <summary>
        /// 水分
        /// </summary>
        public double Mt
        {
            get { return _Mt; }
            set { _Mt = value; }
        }

        private decimal _CoalSize;
        /// <summary>
        /// 煤炭粒度
        /// </summary>
        public decimal CoalSize
        {
            get { return _CoalSize; }
            set { _CoalSize = value; }
        }

        private String _MakeType;
        /// <summary>
        /// 制样方式
        /// </summary>
        public String MakeType { get { return String.IsNullOrEmpty(_MakeType) ? "1" : _MakeType; } set { _MakeType = value; } }

        private int _DataFlag;
        /// <summary>
        /// 标识符
        /// </summary>
        public int DataFlag
        {
            get { return _DataFlag; }
            set { _DataFlag = value; }
        }
    }
}
