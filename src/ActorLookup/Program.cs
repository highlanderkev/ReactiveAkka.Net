using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;

namespace ActorLookupServer
{
    public class Hello { }

    public class Query
    {
        public string Name { get; private set; }
        public Query(string name)
        {
            Name = name;
        }
    }

    public class NamedActor : ReceiveActor
    {
        public NamedActor()
        {
            Receive<Hello>(hello =>
            {
                Console.WriteLine("Hello my name is {0}", Self);
            });
        }
    }

    public class ActorLookup : ReceiveActor
    {
        public ActorLookup()
        {
            Receive<Query>(query =>
            {
                Console.WriteLine("Querying for Actor named: {0}", query.Name);
                try
                {
                    Context.ActorSelection("/user/" + query.Name).Tell(new Hello());
                }
                catch (Exception ex)
                {
					var exception = ex;
                    Console.WriteLine("Exception Occurred: {0} |\n Actor named: {1} not found.", ex.ToString(), query.Name);
                }
            });
        }
    }

    public class Program
    {
        List<string> names = new List<string>{ "Kevin", "Mike", "Aaron", "Carlos", "Bob", "Pat", "Jim", "David" };
        List<IActorRef> actors = new List<IActorRef>();

        public void Main(string[] args)
        {
            var system = ActorSystem.Create("MySystem");

            var actorlookup = system.ActorOf<ActorLookup>("actorlookup");

            foreach (var name in names)
            {
                actors.Add(system.ActorOf<NamedActor>(name));
            }

			Console.WriteLine("System Started: Please type a name to try to find an actor.");
            string input;
            do
            {
                input = Console.ReadLine();
                actorlookup.Tell(new Query(input));
            } while (input != string.Empty);
        }
    }
}