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
        JobClass GetJob();

        [OperationContract]
        void SubmitJob(JobClass job);

        [OperationContract]
        void SubmitResult(string result);
    }
}
