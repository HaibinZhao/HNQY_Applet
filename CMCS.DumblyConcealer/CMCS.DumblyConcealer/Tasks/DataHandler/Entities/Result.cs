using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.DumblyConcealer.Tasks.DataHandler.Entities
{
    class Result
    {
        private int errorCode;
        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }
        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }
        private object data;
        public object Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
