using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.ServiceModel.Web;
using System.ServiceModel;


namespace RESTfulAPILib
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class APIService : IAPIService
    {
        APIService()
        {
        }


        #region TestEcho
        //[[ 테스트 관련 ]]
        // http://localhost:23333/APIService/TestEcho
        [WebInvoke(Method = "POST",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    UriTemplate = "TestEcho")]
        public RES_DEV_ECHO TestEcho(REQ_DEV_ECHO requestPacket)
        {
            var responseResult = new RES_DEV_ECHO();
            responseResult.Result = true;
            responseResult.ResData = string.Format("WaitSec: {0}, ReqData: {1}", requestPacket.WaitSec, requestPacket.ReqData);
            return responseResult;
        }
        #endregion

    }



}
