using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IRemoteService
    {
        bool HasJob();
        string GetJob();
        void SubmitJob(string job);
        void SubmitResult(string result);
    }
}
