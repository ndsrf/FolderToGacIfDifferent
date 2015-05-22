using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMalaga.AkkaExample.Common.Messages
{
    public class MessageAlreadyAnalysed
    {
        public string Message { get; private set; }
        public string Hashtag { get; private set; }
        public float Sentiment { get; private set; }

        public MessageAlreadyAnalysed(string msg, string hashtag, float sentiment)
        {
            Message = msg;
            Hashtag = hashtag;
            Sentiment = sentiment;
        }
    }
}
