using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Views
{
    // <summary>
    /// 视图-入厂煤运输记录视图
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("View_RLSampling")]
    public class View_RLSampling
    {
        private String _SampleCode;

        /// <summary>
        /// 采样码
        /// </summary>
        public String SampleCode
        {
            get { return _SampleCode; }
            set { _SampleCode = value; }
        }

        private String _SamplingType;
        /// <summary>
        /// 采样方式
        /// </summary>
        public String SamplingType
        {
            get { return _SamplingType; }
            set { _SamplingType = value; }
        }

        private String _InfuranceId;
        /// <summary>
        /// 入炉ID
        /// </summary>
        public String InfuranceId
        {
            get { return _InfuranceId; }
            set { _InfuranceId = value; }
        }

        private DateTime _RecordDate;
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordDate
        {
            get { return _RecordDate; }
            set { _RecordDate = value; }
        }

        private String _ClassCyc;
        /// <summary>
        /// 班次
        /// </summary>
        public String ClassCyc
        {
            get { return _ClassCyc; }
            set { _ClassCyc = value; }
        }

        private String _DutyCyc;
        /// <summary>
        /// 值次
        /// </summary>
        public String DutyCyc
        {
            get { return _DutyCyc; }
            set { _DutyCyc = value; }
        }

        private String _CoalpotName;
        /// <summary>
        /// 煤仓
        /// </summary>
        public String CoalpotName
        {
            get { return _CoalpotName; }
            set { _CoalpotName = value; }
        }

        private String _RecorderName;
        /// <summary>
        /// 记录人员
        /// </summary>
        public String RecorderName
        {
            get { return _RecorderName; }
            set { _RecorderName = value; }
        }

    }
}
