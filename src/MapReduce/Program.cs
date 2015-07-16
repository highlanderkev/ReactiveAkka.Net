using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;

using Akka;
using Akka.Actor;
using Akka.Routing;

namespace MapReduce
{
    public class Flush{}
    
    public class Map
    {
        public Map(string title, string content)
        {
            Title = title;
            Content = content;
        }
        public string Title { get; private set; }
        public string Content { get; private set; }
    }
    
    public class Reduce 
    {
        public Reduce(string title, string word)
        {
            Title = title;
            Word = word;
        }
        public string Title { get; private set; }
        public string Word { get; private set; }
    }

    public class MapActor : ReceiveActor
    {
        List<string> StopWords = new List<string>(new string[]{"a", "am", "an", "and", "are", "as", "at", "be", "do", "go", "if", "in", "is", "it", "of", "on", "the", "to"});
        List<IActorRef> ReduceActors = new List<IActorRef>();
        
        public MapActor(List<IActorRef> reduceActors)
        {
            ReduceActors = reduceActors;
            
            Receive<Map>(map => {
                string[] separators = {",", ".", "!", "?", ";", ":", " "};
                string[] words = map.Content.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if(!StopWords.Contains(word)){
                        var index = Math.Abs(word.GetHashCode()%ReduceActors.Count);
                        ReduceActors[index].Tell(new Reduce(map.Title, word));
                    }
                }
            });
            Receive<Flush>(x => {
                foreach(var actor in ReduceActors)
                {
                    actor.Tell(new Flush());
                }
            });
        }
    }
    
    public class ReduceActor : ReceiveActor
    {
        Dictionary<string, int> words = new Dictionary<string, int>();
        int remainingMappers = 0;
        
        public ReduceActor(int numMappers)
        {
            remainingMappers = numMappers;
            Receive<Reduce>(reduce => {
                //Console.WriteLine("Received reduce {0}", reduce.Word);
                if (words.ContainsKey(reduce.Word))
                {
                    words[reduce.Word] = words[reduce.Word] + 1;
                }else{
                    words.Add(reduce.Word, 1);
                }
            });
            Receive<Flush>(x => {
                remainingMappers = remainingMappers - 1;
                if(remainingMappers == 0){
                    StringBuilder sb = new StringBuilder();
                    sb.Append(String.Format("| Actor {0} |", Self.Path.Name));
                    foreach(var keypair in words){
                        sb.Append(String.Format("{0}",keypair));
                    }
                }
            });
        }
    }
    
    public class Program
    {
        public void Main(string[] args)
        {
            using (var system = ActorSystem.Create("MySystem"))
            {
                //var reduceActors = system.ActorOf(new RoundRobinPool(5, null, null, null, usePoolDispatcher: false).Props(Props.Create<ReduceActor>(5)));
                var reduceActors = new List<IActorRef>();
                for (int i = 0; i < 10; i++)
                {
                    reduceActors.Add(system.ActorOf(Props.Create<ReduceActor>(1)));
                }
                
                //var mapActors = system.ActorOf(new RoundRobinPool(5, null, null, null, usePoolDispatcher: false).Props(Props.Create<MapActor>(reduceActors)));
                var mapActor = system.ActorOf(Props.Create<MapActor>(reduceActors));
                
                var title = "Bleak House";
                var url = "http://reed.cs.depaul.edu/lperkovic/csc536/homeworks/gutenberg/pg1023.txt";
                var client = new WebClient();
                var content = client.DownloadString(url);
                //Console.WriteLine(content);
                
                mapActor.Tell(new Map(title, content));
                mapActor.Tell(new Flush());
                Console.ReadLine();
            }
        }
    }
}
