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
        private Queue<JobClass> jobQueue = new Queue<JobClass>();
        private object jobQueueLock = new object();

        public bool HasJob()
        {
            lock (jobQueueLock)
            {
                return jobQueue.Count > 0;
            }
        }

        public JobClass GetJob()
        {
            lock (jobQueueLock)
            {
                if (HasJob())
                {
                    var job = jobQueue.Dequeue();
                    job.Status = "In Progress";
                    return job;
                }
                return null;
            }
        }

        public void SubmitJob(JobClass job)
        {
            lock (jobQueueLock)
            {
                jobQueue.Enqueue(job);
            }
        }

        public void SubmitResult(string result)
        {
            // Process the result. For now, we'll just print it.
            Console.WriteLine($"Received result: {result}");
        }
    }

}
