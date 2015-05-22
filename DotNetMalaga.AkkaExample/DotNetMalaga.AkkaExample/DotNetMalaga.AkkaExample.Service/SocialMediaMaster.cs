using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Commands;
using DotNetMalaga.AkkaExample.Common.Messages;
using DotNetMalaga.AkkaExample.Service.TwitterInteraction;
using Tweetinvi;

namespace DotNetMalaga.AkkaExample.Service
{
    internal class SocialMediaMaster : ReceiveActor
    {
        #region messages
        internal class MessageToBeAnalysed
        {
            public string Message { get; private set; }
            public string Hashtag { get; private set; }

            public MessageToBeAnalysed(string msg, string hashtag)
            {
                Message = msg;
                Hashtag = hashtag;
            }
        }
        #endregion

        public class MultiValueDictionary<Key, Value> : Dictionary<Key, List<Value>>
        {
            public void Add(Key key, Value value)
            {
                List<Value> values;
                if (!this.TryGetValue(key, out values))
                {
                    values = new List<Value>();
                    this.Add(key, values);
                }
                values.Add(value);
            }

            public void RemoveWithKeyValue(Key key, Value value)
            {
                List<Value> values;
                if (this.TryGetValue(key, out values))
                {
                    values.Remove(value);
                }
            }

        }

        private readonly IActorRef _twitterActorRef = Context.ActorOf<StreamWoker>();
        private readonly IActorRef _sentimentAnalyserRef = Context.ActorOf<SentimentAnalyser>();
        private MultiValueDictionary<string, IActorRef> _hashtagSubscribers = new MultiValueDictionary<string, IActorRef>();

        public SocialMediaMaster()
        {
            Receive<AddHashtag>(hashtag =>
            {
                _hashtagSubscribers.Add(hashtag.Hashtag, hashtag.Subscriber);
                _twitterActorRef.Tell(hashtag, Self);
            });
            Receive<RemoveHashtag>(hashtag =>
            {
                _hashtagSubscribers.RemoveWithKeyValue(hashtag.Hashtag, hashtag.Subscriber);
                _twitterActorRef.Tell(hashtag, Self);
            });
            Receive<StartAnalysis>(analysis => _twitterActorRef.Tell(analysis, Self));
            Receive<StopAnalysis>(analysis => _twitterActorRef.Tell(analysis, Self));
            Receive<MessageToBeAnalysed>(msg => _sentimentAnalyserRef.Tell(msg));
            Receive<MessageAlreadyAnalysed>(msg =>
            {
                Console.WriteLine("Tweet received: " + msg.Hashtag + " Sentiment: " + msg.Sentiment);
                foreach (var actor in GetInterestedActorRefs(msg.Hashtag))
                {
                    actor.Tell(msg);
                }
            });
        }

        private IEnumerable<IActorRef> GetInterestedActorRefs(string hashtag)
        {
            return _hashtagSubscribers[hashtag];
        }
    }
}
