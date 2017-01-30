using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using TrelloHack.Models;

namespace TrelloHack.Controllers
{
    // interface, delete function index()
    public class TrelloController : Controller
    {
        //
        // GET: /Trello/

        /*public ActionResult Index()
        {
            return View();
        }*/

        public void callback(string oauth_token, string oauth_verifier)
        {
            try
            {
                string callback_host = Request.Url.Host;
                oauth_token = HttpUtility.HtmlEncode(oauth_token);
                oauth_verifier = HttpUtility.HtmlEncode(oauth_verifier);

                trello login = new trello();
                login.login(oauth_token, oauth_verifier);

                string host = Request.Url.Host;
                int port = Request.Url.Port;
                string redirecturl = "http://" + host + ":" + port + "/Board";
                Response.Redirect(Url.Content(redirecturl));
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        public JObject auth(string name)
        {
            try
            {
                name = HttpUtility.HtmlEncode(name);
                string callback_host = Request.Url.Host;
                int port = Request.Url.Port;
                string callback = "http://" + callback_host + ":" + port + "/trello/callback";
                trello auth = new trello();
                string authurl = auth.authorize(name, callback);

                JObject ret = new JObject();
                if (authurl.Length <= 0)
                {
                    ret.Add("retcode", -1);
                }
                else
                {
                    ret.Add("retcode", 0);
                    ret.Add("url", authurl);
                }
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            JObject obj = new JObject();

            return obj;
        }
    }
}
