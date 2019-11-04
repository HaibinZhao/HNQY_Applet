using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CMCS.CommonADGS.Core
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public class SendMessage
    {
        public string UpLoadIdentifier;

        public List<DataColumnList> DataColumns;

        public List<DataRowList> DataRows;

        //public string BuildTableSQL;

        //public string BuildInsertSQL;

        //public string BuildUpdateSQL;
    }

    public class DataColumnList
    {
        public string ColumnName;

        public string DataType;

        public int MaxLength;
    }

    public class DataRowList
    {
        public string ColumnName;

        public string DataType;

        public string ColumnValue;
    }
}
