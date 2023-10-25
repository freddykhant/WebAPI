using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Client's Job Board Server");

            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.MaxReceivedMessageSize = 2147483647;

            host = new ServiceHost(typeof(RemoteService));
            host.AddServiceEndpoint(typeof(IRemoteService), tcp, "net.tcp://0.0.0.0:8100/JobService");

            host.Open();
            Console.WriteLine("Server is online. Press Enter to stop.");
            Console.ReadLine();

            host.Close();
        }
    }
}
