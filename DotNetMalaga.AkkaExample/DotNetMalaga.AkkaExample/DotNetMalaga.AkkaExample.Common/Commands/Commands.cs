using Akka.Actor;

namespace DotNetMalaga.AkkaExample.Common.Commands
{
    public class AddHashtag
    {
        public string Hashtag { get; private set; }
        public IActorRef Subscriber { get; private set; }

        public AddHashtag(string hashtag, IActorRef subscriber)
        {
            Hashtag = hashtag;
            Subscriber = subscriber;
        }
    }
    public class RemoveHashtag
    {
        public string Hashtag { get; private set; }
        public IActorRef Subscriber { get; private set; }
        public RemoveHashtag(string hashtag, IActorRef subscriber)
        {
            Hashtag = hashtag;
            subscriber = subscriber;
        }
    }
    public class StartAnalysis
    {
    }
    public class StopAnalysis
    {
    }
}
