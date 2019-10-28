// 此代码由 NhGenerator v1.0.7.0 工具生成。

using System;
using System.Collections;
// 
using CMCS.Common.Entities.Sys;

namespace CMCS.Common.Entities.Fuel
{
    /// <summary>
    /// 煤质信息
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("fultbfuelquality")]
    public class CmcsFuelQuality : EntityBase1
    {

        private Decimal _Ad;
        /// <summary>
        /// 干燥基灰分(Ad)
        /// </summary>
        public Decimal Ad { get { return _Ad; } set { _Ad = value; } }

        private Decimal _Aad;
        /// <summary>
        /// 空气干燥基灰分(Aad)
        /// </summary>
        public Decimal Aad { get { return _Aad; } set { _Aad = value; } }


        private Decimal _Aar;
        /// <summary>
        /// Aar
        /// </summary>
        public Decimal Aar { get { return _Aar; } set { _Aar = value; } }


        //private Decimal _DT;
        ///// <summary>
        ///// 变形温度  DT(℃)  灰熔点
        ///// </summary>
        //public Decimal DT { get { return _DT; } set { _DT = value; } }



        //private Decimal _FT;
        ///// <summary>
        ///// 流动温度  FT(℃)  灰熔点
        ///// </summary>
        //public Decimal FT { get { return _FT; } set { _FT = value; } }

        //private Decimal _FC;
        ///// <summary>
        ///// 固定碳(FCad)
        ///// </summary>
        //public Decimal FC { get { return _FC; } set { _FC = value; } }
        private Decimal _FCad;
        /// <summary>
        /// 空气干燥基固定碳
        /// </summary>
        public Decimal FCad { get { return _FCad; } set { _FCad = value; } }



        private Decimal _FCd;
        /// <summary>
        /// 干燥基固定碳(FC,d)
        /// </summary>
        public Decimal FCd { get { return _FCd; } set { _FCd = value; } }

        private Decimal _FCar;
        /// <summary>
        /// 收到基固定碳(FC,ar)
        /// </summary>
        public Decimal FCar { get { return _FCar; } set { _FCar = value; } }



        private Decimal _Had;
        /// <summary>
        /// 空气干燥基氢值(Had)
        /// </summary>
        public Decimal Had { get { return _Had; } set { _Had = value; } }
        //private Decimal _HT;
        ///// <summary>
        ///// 半球温度  HT(℃)  灰熔点
        ///// </summary>
        //public Decimal HT { get { return _HT; } set { _HT = value; } }


        private Decimal _Hdaf;
        /// <summary>
        /// 干燥无灰基氢值(H,daf)
        /// </summary>
        public Decimal Hdaf { get { return _Hdaf; } set { _Hdaf = value; } }

        private Decimal _Har;
        /// <summary>
        /// 收到基氢值(H,ar)
        /// </summary>
        public Decimal Har { get { return _Har; } set { _Har = value; } }

        private Decimal _Hd;
        /// <summary>
        /// 干燥基氢值(H,d)
        /// </summary>
        public Decimal Hd { get { return _Hd; } set { _Hd = value; } }

        private Decimal _Mad;
        /// <summary>
        /// 内水分(Mad)
        /// </summary>
        public Decimal Mad { get { return _Mad; } set { _Mad = value; } }

        private Decimal _Mt;
        /// <summary>
        /// 全水
        /// </summary>
        public Decimal Mt { get { return _Mt; } set { _Mt = value; } }

        private Decimal _Nad;
        /// <summary>
        /// 空气干燥基氮值
        /// </summary>
        public Decimal Nad { get { return _Nad; } set { _Nad = value; } }


        private Decimal _Oad;
        /// <summary>
        /// 空气干燥基氧值(Oad)
        /// </summary>
        public Decimal Oad { get { return _Oad; } set { _Oad = value; } }


        private Decimal _QbAd;
        /// <summary>
        /// 弹筒热值
        /// </summary>
        public Decimal QbAd { get { return _QbAd; } set { _QbAd = value; } }

        private Decimal _QCal;
        /// <summary>
        /// 收到基低位热量(大卡)
        /// </summary>
        public Decimal QCal { get { return _QCal; } set { _QCal = value; } }

        private Decimal _QJ;
        /// <summary>
        /// 收到基低位热量(兆焦)
        /// </summary>
        public Decimal QJ { get { return _QJ; } set { _QJ = value; } }


        //private Decimal _Qnet;
        ///// <summary>
        ///// Qnet
        ///// </summary>
        //public Decimal Qnet { get { return _Qnet; } set { _Qnet = value; } }

        private Decimal _Qgrd;
        /// <summary>
        /// 干燥基高位热值(Qgr,d)MJ/kg
        /// </summary>
        public Decimal Qgrd { get { return _Qgrd; } set { _Qgrd = value; } }

        private Decimal _Qgrad;
        /// <summary>
        /// 高位热值(Qgr,ad)MJ/kg
        /// </summary>
        public Decimal Qgrad { get { return _Qgrad; } set { _Qgrad = value; } }

        //private Decimal _Qnetdaf;
        ///// <summary>
        ///// 干燥无灰基高位热量(Qnet,daf)MJ/kg
        ///// </summary>
        //public Decimal Qnetdaf { get { return _Qnetdaf; } set { _Qnetdaf = value; } }

        private Decimal _Qgrar;
        /// <summary>
        /// 收到基高位热量(Qgr,ar)MJ/kg
        /// </summary>
        public Decimal Qgrar { get { return _Qgrar; } set { _Qgrar = value; } }

        //private Decimal _Stadf;
        ///// <summary>
        ///// 干燥无灰基硫(St,daf)
        ///// </summary>
        //public Decimal Stadf { get { return _Stadf; } set { _Stadf = value; } }

        private Decimal _Std;
        /// <summary>
        /// 干燥基全硫(St,d)
        /// </summary>
        public Decimal Std { get { return _Std; } set { _Std = value; } }


        private Decimal _Stad;
        /// <summary>
        /// 空气干燥基硫(St,ad)
        /// </summary>
        public Decimal Stad { get { return _Stad; } set { _Stad = value; } }


        private Decimal _Star;
        /// <summary>
        /// 收到基全硫(St,ar)
        /// </summary>
        public Decimal Star { get { return _Star; } set { _Star = value; } }

        private Decimal _ST;
        /// <summary>
        /// 灰融点软化温度(ST)
        /// </summary>
        public Decimal ST { get { return _ST; } set { _ST = value; } }


        private Decimal _Vdaf;
        /// <summary>
        /// 挥发分(Vdaf)
        /// </summary>
        public Decimal Vdaf { get { return _Vdaf; } set { _Vdaf = value; } }

        private Decimal _Vad;
        /// <summary>
        /// 挥发分(Vad)
        /// </summary>
        public Decimal Vad { get { return _Vad; } set { _Vad = value; } }

        private Decimal _Var;
        /// <summary>
        /// Var
        /// </summary>
        public Decimal Var { get { return _Var; } set { _Var = value; } }

        private Decimal _Vd;
        /// <summary>
        /// 干燥基挥发分
        /// </summary>
        public Decimal Vd { get { return _Vd; } set { _Vd = value; } }


    }
}
