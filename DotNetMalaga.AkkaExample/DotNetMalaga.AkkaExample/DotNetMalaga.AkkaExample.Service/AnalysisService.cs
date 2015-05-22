using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using DotNetMalaga.AkkaExample.Service.TwitterInteraction;
using Topshelf;

namespace DotNetMalaga.AkkaExample.Service
{
    //class TestClient : ReceiveActor
    //{
    //    public TestClient()
    //    {
    //        Receive<SocialMediaMaster.MessageAlreadyAnalysed>(
    //            analysed => Console.WriteLine("ANALYSIS: " + analysed.Hashtag + " MESSAGE: " + analysed.Message + " POINTS: " + analysed.Sentiment));
    //    }
    //}

    class AnalysisService
    {
        protected ActorSystem AnalysisActorSystem;
        protected IActorRef SocialMediaMaster;

        public bool Start()
        {
            AnalysisActorSystem = ActorSystem.Create("analyzer");
            SocialMediaMaster = AnalysisActorSystem.ActorOf(Props.Create(() => new SocialMediaMaster()), "main");
            //var testClient = AnalysisActorSystem.ActorOf(Props.Create(() => new TestClient()), "testclient");

            //socialMediaMaster.Tell(new AddHashtag("PromesasElectoralesMonguer", testClient));

            return true;
        }

        public bool Stop()
        {
            AnalysisActorSystem.Shutdown();
            return true;
        }
    }
}
