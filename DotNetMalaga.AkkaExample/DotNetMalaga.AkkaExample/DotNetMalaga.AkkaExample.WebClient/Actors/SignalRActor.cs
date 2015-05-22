using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using DotNetMalaga.AkkaExample.Common.Messages;
using DotNetMalaga.AkkaExample.WebClient.SignarlR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotNetMalaga.AkkaExample.WebClient.Actors
{

    public class SignalRActor : ReceiveActor
    {
        #region Messages

        public class DebugCluster
        {
            public DebugCluster(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }

        #endregion

        private SignalRHub _hub;

        public SignalRActor()
        {
            Receive<DebugCluster>(debug => _hub.WriteRawMessage(string.Format("DEBUG: {0}", debug.Message)));
            Receive<String>(s =>
            {
                var commandAddHashtag = new AddHashtag(s, Self);
                SystemActors.SentimentAnalyserActorRef.Tell(commandAddHashtag);
            });
            Receive<MessageAlreadyAnalysed>(
                analysed => Debug.WriteLine("ANALYSIS: " + analysed.Hashtag + " MESSAGE: " + analysed.Message + " POINTS: " + analysed.Sentiment));

        }

        protected override void PreStart()
        {
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            _hub = hubManager.ResolveHub("sentimentHub") as SignalRHub;
        }


    }
}