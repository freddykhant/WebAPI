using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Client's Job Board Server");

            // Register the HTTP channel
            HttpChannel channel = new HttpChannel(8100);
            ChannelServices.RegisterChannel(channel, false);

            // Register the RemoteService for .NET Remoting
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteService),
                "RemoteService",
                WellKnownObjectMode.Singleton
            );

            Console.WriteLine("Server is online. Press Enter to stop.");
            Console.ReadLine();
        }
    }
}

