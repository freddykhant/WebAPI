using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteService : IRemoteService
    {
        // Sample in-memory storage for jobs. In a real-world scenario, you might use a database or other persistent storage.
        private static List<JobClass> jobs = new List<JobClass>();

        public JobClass GetJob()
        {
            // Retrieve the first job with status "Ready"
            var job = jobs.FirstOrDefault(j => j.Status == "Ready");
            if (job != null)
            {
                job.Status = "In Progress"; // Update the status to indicate that the job is being processed
            }
            return job;
        }

        public void SubmitResult(string result)
        {
            // For simplicity, we're just printing the result. 
            // In a real-world scenario, you might want to store the result or perform other actions.
            Console.WriteLine($"Received result: {result}");
        }

        // Sample method to add a job to the in-memory storage (for testing purposes)
        public void AddJob(JobClass job)
        {
            jobs.Add(job);
        }
    }

}
