using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.Entities.Sys
{
    /// <summary>
    /// 小程序运行日志
    /// </summary>
    [CMCS.DapperDber.Attrs.DapperBind("cmcstbappletlog")]
    public class CmcsAppletLog : EntityBase1
    {
        private string appIdentifier;
        /// <summary>
        /// 程序唯一标识
        /// </summary>
        public string AppIdentifier
        {
            get { return appIdentifier; }
            set { appIdentifier = value; }
        }

        private string logLevel;
        /// <summary>
        /// 日志等级
        /// </summary>
        public string LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }

        private string title;
        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string content;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
    } 
}
