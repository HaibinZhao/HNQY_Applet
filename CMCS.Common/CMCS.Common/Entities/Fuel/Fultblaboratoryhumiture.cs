using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 化验室温湿度统计表
    /// </summary>
    [Serializable]
    [CMCS.DapperDber.Attrs.DapperBind("fultblaboratoryhumiture")]
    public class Fultblaboratoryhumiture : EntityBase1
    {
        private Decimal _INFA_HEAT_TEMP;
        /// <summary>
        /// 入厂_量热室温度
        /// </summary>
        public virtual Decimal INFA_HEAT_TEMP { get { return _INFA_HEAT_TEMP; } set { _INFA_HEAT_TEMP = value; } }

        private Decimal _INFA_HEAT_HUMI;
        /// <summary>
        /// 入厂_量热室湿度
        /// </summary>
        public virtual Decimal INFA_HEAT_HUMI { get { return _INFA_HEAT_HUMI; } set { _INFA_HEAT_HUMI = value; } }

        private Decimal _INFA_SULFUR_TEMP;
        /// <summary>
        /// 入厂_测硫室温度
        /// </summary>
        public virtual Decimal INFA_SULFUR_TEMP { get { return _INFA_SULFUR_TEMP; } set { _INFA_SULFUR_TEMP = value; } }

        private Decimal _INFA_SULFUR_HUMI;
        /// <summary>
        /// 入厂_测硫室湿度
        /// </summary>
        public virtual Decimal INFA_SULFUR_HUMI { get { return _INFA_SULFUR_HUMI; } set { _INFA_SULFUR_HUMI = value; } }

        private Decimal _INFA_MOISTURE_TEMP;
        /// <summary>
        /// 入厂_水分室温度
        /// </summary>
        public virtual Decimal INFA_MOISTURE_TEMP { get { return _INFA_MOISTURE_TEMP; } set { _INFA_MOISTURE_TEMP = value; } }

        private Decimal _INFA_MOISTURE_HUMI;
        /// <summary>
        /// 入厂_水分室湿度
        /// </summary>
        public virtual Decimal INFA_MOISTURE_HUMI { get { return _INFA_MOISTURE_HUMI; } set { _INFA_MOISTURE_HUMI = value; } }

        private Decimal _INFA_PROXIMATE_TEMP;
        /// <summary>
        /// 入厂_工分室温度
        /// </summary>
        public virtual Decimal INFA_PROXIMATE_TEMP { get { return _INFA_PROXIMATE_TEMP; } set { _INFA_PROXIMATE_TEMP = value; } }

        private Decimal _INFA_PROXIMATE_HUMI;
        /// <summary>
        /// 入厂_工分室湿度
        /// </summary>
        public virtual Decimal INFA_PROXIMATE_HUMI { get { return _INFA_PROXIMATE_HUMI; } set { _INFA_PROXIMATE_HUMI = value; } }

        private Decimal _INFU_HEAT_TEMP;
        /// <summary>
        /// 入炉_量热室温度
        /// </summary>
        public virtual Decimal INFU_HEAT_TEMP { get { return _INFU_HEAT_TEMP; } set { _INFU_HEAT_TEMP = value; } }

        private Decimal _INFU_HEAT_HUMI;
        /// <summary>
        /// 入炉_量热室湿度
        /// </summary>
        public virtual Decimal INFU_HEAT_HUMI { get { return _INFU_HEAT_HUMI; } set { _INFU_HEAT_HUMI = value; } }

        private Decimal _INFU_SULFUR_TEMP;
        /// <summary>
        /// 入炉_测硫室温度
        /// </summary>
        public virtual Decimal INFU_SULFUR_TEMP { get { return _INFU_SULFUR_TEMP; } set { _INFU_SULFUR_TEMP = value; } }

        private Decimal _INFU_SULFUR_HUMI;
        /// <summary>
        /// 入炉_测硫室湿度
        /// </summary>
        public virtual Decimal INFU_SULFUR_HUMI { get { return _INFU_SULFUR_HUMI; } set { _INFU_SULFUR_HUMI = value; } }

        private Decimal _INFU_MOISTURE_TEMP;
        /// <summary>
        /// 入炉_水分室温度
        /// </summary>
        public virtual Decimal INFU_MOISTURE_TEMP { get { return _INFU_MOISTURE_TEMP; } set { _INFU_MOISTURE_TEMP = value; } }

        private Decimal _INFU_MOISTURE_HUMI;
        /// <summary>
        /// 入炉_水分室湿度
        /// </summary>
        public virtual Decimal INFU_MOISTURE_HUMI { get { return _INFU_MOISTURE_HUMI; } set { _INFU_MOISTURE_HUMI = value; } }

        private Decimal _INFU_PROXIMATE_TEMP;
        /// <summary>
        /// 入炉_工分室温度
        /// </summary>
        public virtual Decimal INFU_PROXIMATE_TEMP { get { return _INFU_PROXIMATE_TEMP; } set { _INFU_PROXIMATE_TEMP = value; } }

        private Decimal _INFU_PROXIMATE_HUMI;
        /// <summary>
        /// 入炉_工分室湿度
        /// </summary>
        public virtual Decimal INFU_PROXIMATE_HUMI { get { return _INFU_PROXIMATE_HUMI; } set { _INFU_PROXIMATE_HUMI = value; } }


    }
}
