using System.Collections.Generic;
using System.Net.Http;

namespace Mercurial
{

    class Webhook
    {
        private string webhook;
        public Webhook(string userWebhook)
        {
            webhook = userWebhook;
        }
        public void Send(string content)
        {

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                {"content", content }
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.PostAsync(webhook, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
                }
            }

            catch
            {
            }

        }
    
}
