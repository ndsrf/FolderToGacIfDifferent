using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using Tweetinvi;
using Tweetinvi.Core.Events.EventArguments;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Logic;

namespace DotNetMalaga.AkkaExample.Service.TwitterInteraction
{
    internal class StreamWoker : ReceiveActor, IWithUnboundedStash
    {
        #region messages
        internal class TweetReceived
        {
            public string Tweet { get; private set; }
            public TweetReceived(string tweet)
            {
                Tweet = tweet;
            }
        }

        #endregion

        public IStash Stash
        {
            get;
            set;
        }

        private TwitterHandle filteredStream;

        public StreamWoker()
        {
            filteredStream = new TwitterHandle(Self);
             //filteredStream.MatchingTweetReceived += (sender, args) => Console.WriteLine(args.Tweet.Text);
            //filteredStream.AddTrack("Barcelona", tweet => Console.Write("Barcelona - " + tweet.Creator));
            //filteredStream.AddTrack("Málaga", tweet => Console.Write("Malaga - " + tweet.Creator));
            //var t = filteredStream.StartStreamMatchingAllConditionsAsync().ContinueWith(task => Console.WriteLine(task))
            //filteredStream.AddTrack("Javier", tweet => Console.Write("Javier - " + tweet.Creator));
            //filteredStream.RemoveTrack("Javier");
            Streaming();
        }

        private void Streaming()
        {
            Receive<AddHashtag>(hashtag => filteredStream.AddTrack(hashtag.Hashtag));
            Receive<RemoveHashtag>(hashtag => filteredStream.RemoveTrack(hashtag.Hashtag));            
            Receive<SocialMediaMaster.MessageToBeAnalysed>(received =>
            {
                //Console.WriteLine("TWEET -- " + received.Message);
                Context.Parent.Tell(received);
                
            });
            Receive<StopAnalysis>(streaming => filteredStream.StopStream());

        }
    }
}
