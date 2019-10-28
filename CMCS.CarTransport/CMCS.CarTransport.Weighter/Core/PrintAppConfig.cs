using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace CMCS.CarTransport.Weighter
{
    public class PrintAppConfig
    {
        public static string ConfigXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Print.AppConfig.xml");

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

            this.imgLeftPadding = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/ImgLeftPadding").InnerText, 100);
            this.imgTopPadding = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/ImgTopPadding").InnerText, 10);
            this.imgSize = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/ImgSize").InnerText, 3);
            this.charToImg = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/CharToImg").InnerText, 5);
            this.charLeftPadding = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/CharLeftPadding").InnerText, 10);
            this.fontSize = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/FontSize").InnerText, 14);
            this.rowMaxChaNums = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/RowMaxChaNums").InnerText, 21);
            this.printNums = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/PrintNums").InnerText, 1);
            this.charLineSpacing = ParseInt(xdoc.SelectSingleNode("CommonAppConfig/CharLineSpacing").InnerText, 10);
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

        private Int32 imgLeftPadding;
        /// <summary>
        /// 二维码左边空余多少,单位：像素
        /// </summary>
        public Int32 ImgLeftPadding
        {
            get { return imgLeftPadding; }
            set { imgLeftPadding = value; }
        }


        private Int32 imgTopPadding;
        /// <summary>
        /// 二维码上面空余多少,单位：像素
        /// </summary>
        public Int32 ImgTopPadding
        {
            get { return imgTopPadding; }
            set { imgTopPadding = value; }
        }

        private Int32 imgSize;
        /// <summary>
        /// 二维码宽度
        /// </summary>
        public Int32 ImgSize
        {
            get { return imgSize; }
            set { imgSize = value; }
        }

        private Int32 charToImg;
        /// <summary>
        /// 下方明码距二维码的距离,单位：像素
        /// </summary>
        public Int32 CharToImg
        {
            get { return charToImg; }
            set { charToImg = value; }
        }

        private Int32 charLeftPadding;
        /// <summary>
        /// 下方明码左边空余位置,单位：像素
        /// </summary>
        public Int32 CharLeftPadding
        {
            get { return charLeftPadding; }
            set { charLeftPadding = value; }
        }

        private Int32 charLineSpacing;
        /// <summary>
        /// 行间距
        /// </summary>
        public Int32 CharLineSpacing
        {
            get { return charLineSpacing; }
            set { charLineSpacing = value; }
        }





        private Int32 ParseInt(Object obj, Int32 defaultValue)
        {
            if (obj == null) return defaultValue;
            Int32.TryParse(obj.ToString(), out defaultValue);
            return defaultValue;
        }

        public decimal ParseDecimal(Object obj)
        {
            if (obj == null) return 0m;
            decimal defaultValue = 0m;
            decimal.TryParse(obj.ToString(), out defaultValue);
            return defaultValue;
        }
    }
}
