using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;
using Akka.Routing;
using DotNetMalaga.AkkaExample.WebClient.Actors;
using DotNetMalaga.AkkaExample.WebClient.Models;
using Microsoft.Ajax.Utilities;
using Props = Akka.Actor.Props;

namespace DotNetMalaga.AkkaExample.WebClient
{
    public class ActorConfig
    {
        public static void Register(out ActorSystem actorSystem)
        {
            actorSystem = ActorSystem.Create("webclient");
 
            SystemActors.SignalRActor = actorSystem.ActorOf(Props.Create(() => new SignalRActor()), "signalr");

            SystemActors.SentimentAnalyserActorRef = actorSystem.ActorSelection("akka.tcp://analyzer@localhost:8080/user/main");

            SystemActors.SignalRActor.Tell("malaga");
        }
    }
}