using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientClass
    {
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public JobClass Job { get; set; }
    }
}
