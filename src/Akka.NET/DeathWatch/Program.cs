using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka;
using Akka.Actor;

namespace DeathWatch
{
	
	public class CustomException: Exception
	{
		public CustomException(string message)
		: base(message){
		}
	}
	
	public class Worker : UntypedActor
	{
		public int count { get; set; }
		
		public Worker(){
			count = 0;
			StartCounting();
		}
		
		protected override void OnReceive(object message){
			Unhandled(message);
		}
		
		private void StartCounting(){
			while(count < 10){
				count++;
				Console.WriteLine("Worker Counting: {0}", count);
				if(count % 7 == 0){
					Console.WriteLine("Try to divide by zero...");
					var x = count/0;
					//throw new CustomException("test");
				}
			}
		}
	}
	
	public class Supervisor : UntypedActor
	{
		private IActorRef child = Context.ActorOf<Worker>("Worker");
		private IActorRef lastSender = Context.System.DeadLetters;
		
		public Supervisor()
		{
			Context.Watch(child);
		}
		
		protected override void OnReceive(object message){
			if(message.Equals("kill")){
				Context.Stop(child);
				lastSender = Sender;
			}
			else if(message is Terminated){
				var t = (Terminated)message;
				
				if(t.ActorRef == child){
					lastSender.Tell("finished");
				}
			}
			else{
				Unhandled(message);
			}
		}
		
		protected override SupervisorStrategy SupervisorStrategy()
		{
			//return new OneForOneStrategy(x => { return Directive.Stop; });
			return new OneForOneStrategy(x => {
					if (x is DivideByZeroException)
					{
						return Directive.Stop;
					}
					else{
						return Directive.Restart;
					}
			});
		}
		
	}
	
	public class Program
	{
		public void Main(string[] args)
		{
			var system = ActorSystem.Create("MySystem");
			var super = system.ActorOf<Supervisor>("Supervisor");
			Console.WriteLine("System Started: Enter to kill worker counting.");
			
			Console.ReadKey();
			super.Tell("kill");
		}
	}
	
}
