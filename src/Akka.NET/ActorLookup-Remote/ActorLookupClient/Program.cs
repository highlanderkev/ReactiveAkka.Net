using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using Akka.Remote;

namespace ActorLookupClient
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
            port = 9002
            hostname = localhost
		}
    }
}
");
        public void Main(string[] args)
        {	
			Console.WriteLine(config);
            using(var system = ActorSystem.Create("MyClient", config)){
				Console.WriteLine(system);
				Console.WriteLine("System Started: Please type a name to try to find an actor.");
            	system.ActorSelection("akka.tcp://MyServer@localhost:9001/user/Kevin").Tell("Hello");
				string input;
            	do
            	{
                	input = Console.ReadLine();
					system.ActorSelection("akka.tcp://MyServer@localhost:9001/user/" + input).Tell("Hello");
            	} while (input != string.Empty);
			}
        }
    }
}