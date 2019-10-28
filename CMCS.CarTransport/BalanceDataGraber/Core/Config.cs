using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BalanceDataGraber.Core
{
    /// <summary>
    /// 程序配置
    /// </summary>
    public class Config
    {
        private static string ConfigXmlPath = "config.xml";

        private static Config instance;

        public static Config GetInstance()
        {
            return instance;
        }

        static Config()
        {
            instance = XOConverter.LoadConfig<Config>(ConfigXmlPath);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            XOConverter.SaveConfig(instance, ConfigXmlPath);
        }

        private int comIndex1;
        /// <summary>
        /// Com1
        /// </summary>
        public int ComIndex1
        {
            get { return comIndex1; }
            set { comIndex1 = value; }
        }

        private int comIndex2;
        /// <summary>
        /// Com2
        /// </summary>
        public int ComIndex2
        {
            get { return comIndex2; }
            set { comIndex2 = value; }
        }

        private int comIndex3;
        /// <summary>
        /// Com3
        /// </summary>
        public int ComIndex3
        {
            get { return comIndex3; }
            set { comIndex3 = value; }
        }

        private int comIndex4;
        /// <summary>
        /// Com4
        /// </summary>
        public int ComIndex4
        {
            get { return comIndex4; }
            set { comIndex4 = value; }
        }
    }
}
