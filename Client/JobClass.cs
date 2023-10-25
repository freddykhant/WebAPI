using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class JobClass
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime CompletionTime { get; set; }
    }
}
