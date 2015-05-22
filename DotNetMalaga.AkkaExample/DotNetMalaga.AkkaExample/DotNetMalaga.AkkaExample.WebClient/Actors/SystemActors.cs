using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;

namespace DotNetMalaga.AkkaExample.WebClient.Actors
{
    public static class SystemActors
    {
        public static IActorRef SignalRActor = ActorRefs.Nobody;

        public static ActorSelection SentimentAnalyserActorRef;
    }
}