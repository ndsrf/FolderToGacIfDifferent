using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace DotNetMalaga.AkkaExample.Service.TwitterInteraction
{
    class TwitterHandle
    {

        private IFilteredStream filteredStream;
        private IActorRef twitterActorRef;


        public TwitterHandle(IActorRef theActor)
        {
            twitterActorRef = theActor;
            TwitterCredentials.SetCredentials("71821903-oTTtcHhUq60QKuGeT30pfakyhvc4SXf37PlY5JGpZ",
                     "E1RaraSfXhYR9QeAUfAnKKyRSpn9aYd7LuUPVZlc4GUCs", "KQQICtGWY764hlN6N4OVPLX4I",
                     "SnZDnXBUwEadwTVGHA0ruuxxvvF4hAdupMLruBWcLa6H9hn8oP");
            filteredStream = Stream.CreateFilteredStream();
        }

        public void AddTrack(string hashtag)
        {
            filteredStream.StopStream();
            filteredStream.AddTrack(hashtag, tweet => twitterActorRef.Tell(new SocialMediaMaster.MessageToBeAnalysed(tweet.Text, hashtag)));
            filteredStream.StartStreamMatchingAnyConditionAsync();
        }

        public void RemoveTrack(string hashtag)
        {
            filteredStream.StopStream();
            filteredStream.RemoveTrack(hashtag);
            filteredStream.StartStreamMatchingAnyConditionAsync();
        }

        public void StopStream()
        {
            filteredStream.StopStream();
        }

    }
}
