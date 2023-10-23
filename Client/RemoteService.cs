using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class RemoteService : MarshalByRefObject, IRemoteService
    {
        private Queue<string> jobQueue = new Queue<string>();

        public bool HasJob()
        {
            return jobQueue.Count > 0;
        }

        public string GetJob()
        {
            if (HasJob())
            {
                return jobQueue.Dequeue();
            }
            return null;
        }

        public void SubmitJob(string job)
        {
            jobQueue.Enqueue(job);
        }

        public void SubmitResult(string result)
        {
            // Process the result. For now, we'll just print it.
            Console.WriteLine($"Received result: {result}");
        }
    }
}
