using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ServiceContract]
    public interface IRemoteService
    {
        [OperationContract]
        bool HasJob();

        [OperationContract]
        string GetJob();

        [OperationContract]
        void SubmitJob(string job);

        [OperationContract]
        void SubmitResult(string result);
    }
}
