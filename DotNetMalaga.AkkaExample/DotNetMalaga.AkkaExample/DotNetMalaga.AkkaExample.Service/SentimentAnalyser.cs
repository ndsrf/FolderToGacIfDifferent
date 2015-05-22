using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Akka.Actor;
using DotNetMalaga.AkkaExample.Common.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetMalaga.AkkaExample.Service
{
    internal class SentimentAnalyser : ReceiveActor
    {
        public SentimentAnalyser()
        {
            Receive<SocialMediaMaster.MessageToBeAnalysed>(msg =>
            {
                var uriString = new StringBuilder("text=");
                uriString.Append(HttpUtility.UrlEncode(msg.Message));
                uriString.Append("&language=spa&apikey=b8b7313b-ff14-435e-9bad-1ddf61944d2c");

                var wc = new WebClient();
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers[HttpRequestHeader.Accept] = "application/json";

                wc.UploadStringTaskAsync(new Uri("https://api.idolondemand.com/1/api/sync/analyzesentiment/v1"), uriString.ToString()).ContinueWith<MessageAlreadyAnalysed>(task =>
                {
                    var json = JsonConvert.DeserializeObject<dynamic>(task.Result);
                    float score;
                    if (!float.TryParse(Convert.ToString(json.aggregate.score), out score))
                        score = 0;
                    return new MessageAlreadyAnalysed(msg.Message, msg.Hashtag, score);
                }).PipeTo(Sender);         
            });
        }
    }
}
