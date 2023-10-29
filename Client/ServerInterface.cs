using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ServiceContract]
    public interface ServerInterface
    {
        [OperationContract]
        bool HasJob();

        [OperationContract]
        void SubmitJob(string job);

        [OperationContract]
        string GetResult();

        [OperationContract]
        void SubmitResult(string result);
    }
}
