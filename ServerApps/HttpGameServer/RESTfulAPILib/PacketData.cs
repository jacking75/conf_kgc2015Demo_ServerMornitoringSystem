using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTfulAPILib
{
    #region REQ_DEV_ECHO
    public struct REQ_DEV_ECHO
    {
        public int WaitSec;
        public string ReqData;
    }

    public struct RES_DEV_ECHO
    {
        public bool Result;
        public string ResData;
    }
    #endregion

}
