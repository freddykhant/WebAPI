﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    public class JobClass
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime CompletionTime { get; set; }
    }
}
