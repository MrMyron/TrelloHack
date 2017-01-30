using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Mvc;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace TrelloHack.Models
{
    // responsible for trello api format
    public class trello
    {
        private const string app_key = "9466eb7888f33b5b7f8c3ae9387264ce";
        private const string app_secret = "2931d168ede392d743d4a3c11196aed05138c40781f9b76281ffe2df22d51833";
        private const string redis_ip = "127.0.0.1";
        private const int redis_port = 6379;
        private const string oauth_token = "oauth_token";
        private const string oauth_token_secret = "oauth_token_secret";
        private const string member_name = "myron50";

        public string authorize(string name, string callback)
        {
            string url = "https://trello.com/1/OAuthGetRequestToken";   // get token url
            string nonce = "yQDMuXvdcEfQs2Mzf3XcT1r36WTULJls";

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long time = (long)(DateTime.Now - startTime).TotalSeconds;

            string param = "oauth_callback=" + callback + "&oauth_consumer_key=" + app_key + "&oauth_nonce=" + nonce
                + "&oauth_signature_method=PLAINTEXT&oauth_timestamp=" + time + "&oauth_version=1.0";

            url += "?" + param + "&oauth_signature=" + app_secret + "%26";
            string result = curl_get(url);

            string[] sArray = Regex.Split(result, "&");
            if (sArray.Count() < 0)
            {
                return "";
            }

            string oauthToken = sArray[0];
            string oauthSecret = sArray[1];
            // save token
            RedisClient client = new RedisClient(redis_ip, redis_port);
            client.FlushAll();

            string[] token = Regex.Split(oauthToken, "=");
            string[] secret = Regex.Split(oauthSecret, "=");

            client.Add<string>("oauth_token", token[1]);
            client.Add<string>("oauth_secret", secret[1]);

            string authorize_url = "https://trello.com/1/OAuthAuthorizeToken?" + oauthToken + "&name=" + name + "&scope=read%2Cwrite";
            return authorize_url;
        }

        public void login(string oauthtoken, string oauthverifier)
        {
            RedisClient client = new RedisClient(redis_ip, redis_port);
            string oauthsecret = client.Get<string>("oauth_secret");

            // get access token
            string url = "https://trello.com/1/OAuthGetAccessToken";
            string param = "oauth_token=" + oauthtoken + "&oauth_verifier=" + oauthverifier + "&oauth_signature_method=PLAINTEXT&oauth_signature=" + app_secret + "%26" + oauthsecret;
            url += "?" + param;

            string result = curl_get(url);

            string[] sArray = Regex.Split(result, "&");
            string[] token = Regex.Split(sArray[0], "=");
            string[] secret = Regex.Split(sArray[1], "=");

            // record token
            client.FlushAll();
            client.Add<string>(oauth_token, token[1]);
            client.Add<string>(oauth_token_secret, secret[1]);
        }

        public JObject get_board()
        {
            string url = "https://api.trello.com/1/members/" + member_name;
            string result = get(url, "&boards=all");

            JObject jsonObj = JObject.Parse(result);

            JObject ret = new JObject();
            JArray boards = (JArray)(jsonObj["boards"]);
            foreach (JObject obj in boards)
            {
                ret.Add((string)(obj["name"]), (string)(obj["id"]));
            }
            return ret;
        }

        public JArray getLists(string bid)
        {
            string url = "https://api.trello.com/1/boards/" + bid + "/lists";
            string lists = get(url);
            JArray jsonObj = JArray.Parse(lists);

            return jsonObj;
        }

        public JArray getCards(string lid)
        {
            string url = "https://api.trello.com/1/lists/" + lid + "/cards";
            string scards = get(url);
            JArray cards = JArray.Parse(scards);

            return cards;
        }

        public string get_comments(string cid)
        {
            string ccid = HttpUtility.HtmlEncode(cid);

            string url = "https://api.trello.com/1/cards/" + cid + "/actions";
            string result = get(url, "&filter=commentCard");
            JArray arr = JArray.Parse(result);
            string ret = "";
            foreach (JObject cmt in arr)
            {
                string id = (string)cmt["id"];
                string text = (string)(cmt["data"]["text"]);  
                string date = (string)(cmt["date"]);
                string creator = (string)(cmt["memberCreator"]["fullName"]);
                ret += text + "," + creator + "," + date + ";";
            }
            return ret;
        }

        public string add_comment(string cid, string comment)
        {
            string url = "https://api.trello.com/1/cards/" + cid + "/actions/comments";
            string result = post(url, "text=" + comment);
            return result;
        }

        private string post(string url, string postData)
        {
            RedisClient client = new RedisClient(redis_ip, redis_port);
            string oauthtoken = client.Get<string>("oauth_token");

            string checkinfo = "?key=" + app_key + "&token=" + oauthtoken;
            url += checkinfo;

            var request = (HttpWebRequest)WebRequest.Create(url);

            //var data = Encoding.ASCII.GetBytes(postData);
            var data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        private string get(string url, string others = "")
        {
            RedisClient client = new RedisClient(redis_ip, redis_port);
            string oauthtoken = client.Get<string>("oauth_token");
            string checkinfo = "?key=" + app_key + "&token=" + oauthtoken;
            url += checkinfo;
            if (others.Length > 0)
                url += others;

            HttpWebRequest request = WebRequest.CreateHttp(url);
            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();
            StreamReader reader = new StreamReader(s);
            string result = reader.ReadToEnd();
            return result;
        }

        private string curl_get(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            WebResponse response = request.GetResponse();

            Stream s = response.GetResponseStream();
            StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();
        }
    }
}