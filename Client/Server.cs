using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Server
    {
        private static bool hasJob;
        private static string job;
        private static string result;

        public bool HasJob()
        {
            return hasJob;
        }

        public void SubmitJob(string j)
        {
            job = j;
            hasJob = true;
        }

        public void SubmitResult(string res)
        {
            result = res;
            hasJob = false;
        }

        public string GetResult()
        {
            return result;
        }
    }
}
