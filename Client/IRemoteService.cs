using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IRemoteService
    {
        // Method to retrieve a job from the client's .NET Remoting server
        JobClass GetJob();

        // Method to submit the result of a job execution back to the client's .NET Remoting server
        void SubmitResult(string result);
    }

}
