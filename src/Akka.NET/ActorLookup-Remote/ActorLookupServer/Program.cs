using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using Akka.Remote;

namespace ActorLookupServer
{
	public class ReplyActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Console.WriteLine("Message from {0} - {1}", Sender.Path, message);
        }
    }
	
    public class NamedActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
        	Console.WriteLine("Hello my name is {0}", Self);
        }
    }

    public class Program
    {
		private static readonly Config config = ConfigurationFactory.ParseString(@"
akka {
	log-config-on-start = on
    stdout-loglevel = DEBUG
	loglevel = DEBUG
	actor {
		provider = ""Akka.Remote.RemoteActorRefProvider, Akka.remote""
	}
	remote {
        helios.tcp {
			maximum-frame-size = 4000000b
			transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
			transport-protocol = tcp
            port = 9001
            hostname = localhost
        }
    }
}
");
        List<string> names = new List<string>{ "Kevin", "Mike", "Aaron", "Carlos", "Bob", "Pat", "Jim", "David" };
        List<IActorRef> actors = new List<IActorRef>();

        public void Main(string[] args)
        {
			Console.WriteLine(config);
            using(var system = ActorSystem.Create("MyServer", config)){
            	foreach (var name in names)
            	{
					actors.Add(system.ActorOf<NamedActor>(name));
            	}
				//var test = system.ActorOf<NamedActor>("Kevin");
				//Console.WriteLine(test);
				Console.WriteLine("System Started...");
            	Console.WriteLine("Enter to exit...");
            	Console.ReadKey();
			}
        }
    }
}