using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;

namespace GreetingInput
{
    
    public class Greet
    {
        public Greet(string who){
            Who = who;
        }
        public string Who { get; private set; }
    }
    
    public class GreetingActor : ReceiveActor
    {
        public GreetingActor(){
            Receive<Greet>(greet => Console.WriteLine("Hello {0}", greet.Who));
        }
    }
    
    
    public class Program
    {
        public void Main(string[] args)
        {
            var system = ActorSystem.Create("MySystem");
            
            var greeter = system.ActorOf<GreetingActor>("greeter");
            
            greeter.Tell(new Greet("World!"));
            
            string input;
            do{
                input = Console.ReadLine();
                greeter.Tell(new Greet(input));
            }while(input != string.Empty);
        }
    }
}