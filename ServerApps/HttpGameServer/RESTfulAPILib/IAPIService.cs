using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;



namespace RESTfulAPILib
{
    [ServiceContract]
    public interface IAPIService
    {                
        [OperationContract]
        RES_DEV_ECHO TestEcho(REQ_DEV_ECHO requestPacket);
             
    }
}
