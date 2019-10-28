using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace CMCS.CarTransport.WeightNotesPrint
{
    public class PrintAppConfig
    {
        public static string ConfigXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Common.AppConfig.xml");

        private static PrintAppConfig instance;

        public static PrintAppConfig GetInstance()
        {
            if (instance == null) instance = new PrintAppConfig();

            return instance;
        }

        private PrintAppConfig()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(ConfigXmlPath);

            this.fontSize = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/FontSize").InnerText, 17);
            this.rowMaxChaNums = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/RowMaxChaNums").InnerText,7);
            this.printNums = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/PrintNums").InnerText,1);
            this.leftPadding = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/LeftPadding").InnerText, 10);
        }

        private Int32 fontSize;
        /// <summary>
        /// 除开标题外下面内容字体大小，默认：17
        /// </summary>
        public Int32 FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        private Int32 rowMaxChaNums;
        /// <summary>
        /// 除开前面四个字和冒号后一行最多显示的汉字数量，默认：7
        /// </summary>
        public Int32 RowMaxChaNums
        {
            get { return rowMaxChaNums; }
            set { rowMaxChaNums = value; }
        }

        private Int32 printNums;
        /// <summary>
        /// 发票打印多少联
        /// </summary>
        public Int32 PrintNums
        {
            get { return printNums; }
            set { printNums = value; }
        }

        private Int32 leftPadding;
        /// <summary>
        /// 左边空余多少
        /// </summary>
        public Int32 LeftPadding
        {
            get { return leftPadding; }
            set { leftPadding = value; }
        }

        private Int32 ParseInt(Object obj, Int32 defaultValue)
        {
            if (obj == null) return defaultValue;
            Int32.TryParse(obj.ToString(), out defaultValue);
            return defaultValue;
        }
    }
}
