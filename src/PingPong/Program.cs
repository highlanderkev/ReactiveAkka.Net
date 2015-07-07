using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;

namespace PingPong
{
    public class Ping
    {
        public string toString(){
            return "Ping!";
        }
    }
    
    public class Pong
    {
        public string toString(){
            return "Pong!";
        }
    }
    
    public class Start
    {
        public IActorRef Actor { get; private set; }
        public Start(IActorRef actor){
            Actor = actor;
        }
    }
    
    public class PingPongActor : ReceiveActor
    {
        private int _received;
        private int _sent;
        public PingPongActor(){
            _received = 0;
            _sent = 0;
            
            Receive<Ping>(ping => {
                Console.WriteLine("Actor: Received {0} from {1}", ping.toString(), Sender);
                _received++;
                Sender.Tell(new Pong());
                _sent++;
                Console.WriteLine("Messages Sent {0} | Received {0}", _sent, _received);
            });
            
            Receive<Pong>(pong => {
                Console.WriteLine("Actor: Received {0} from {1}", pong.toString(), Sender);
                _received++;
                Sender.Tell(new Ping());
                _sent++;
                Console.WriteLine("Sent {0} | Received {0}", _sent, _received);
            });
            
            Receive<Start>(start => {
                Console.WriteLine("Starting PingPong...");
                start.Actor().Tell(new Ping()); 
            });
        }
    }
    public class Program
    {
        public void Main(string[] args)
        {
            var system = ActorSystem.Create("MySystem");
            
            var actorOne = system.ActorOf<PingPongActor>("actorOne");
            
            var actorTwo = system.ActorOf<PingPongActor>("actorTwo");
            
            actorOne.Tell(new Start(actorTwo));
            
            string input;
            do{
                input = Console.ReadLine();
            }while(input != string.Empty);
        }
    }
}