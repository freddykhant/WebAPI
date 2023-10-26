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
        // Method to retrieve a job from the client's .NET Remoting server
        [OperationContract]
        JobClass GetJob();

        // Method to submit the result of a job execution back to the client's .NET Remoting server
        [OperationContract]
        void SubmitResult(string result);

        [OperationContract]
        bool AddJob(JobClass job);
    }

}
