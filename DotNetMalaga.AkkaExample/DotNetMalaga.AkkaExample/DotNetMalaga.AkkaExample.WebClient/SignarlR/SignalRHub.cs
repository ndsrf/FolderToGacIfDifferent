using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Akka.Actor;
using DotNetMalaga.AkkaExample.WebClient.Actors;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DotNetMalaga.AkkaExample.WebClient.SignarlR
{
    [HubName("sentimentHub")]
    public class SignalRHub : Hub
    {
        //public void PushStatus(JobStatusUpdate update)
        //{
        //    WriteMessage(string.Format("[{0}]({1}) - {2} ({3}) [{4} elapsed]", DateTime.UtcNow, update.Job, update.Stats, update.Status, update.Elapsed));
        //}

        public void CrawlFailed(string reason)
        {
            WriteMessage(reason);
        }

        public void StartCrawl(string message)
        {
            SystemActors.SignalRActor.Tell(message, ActorRefs.Nobody);
        }

        internal void WriteRawMessage(string msg)
        {
            WriteMessage(msg);
        }

        internal static void WriteMessage(string message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            dynamic allClients = context.Clients.All.writeStatus(message);
        }
    }
}