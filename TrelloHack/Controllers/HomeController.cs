using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrelloHack.Models;
using Newtonsoft.Json.Linq;

namespace TrelloHack.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(string bid, string name)
        {
            try
            {
                trello tl = new trello();
                List<BoardInfo> board_list = new List<BoardInfo>();
                JObject bids_info = tl.get_board();
                foreach (var bids in bids_info)
                {
                    string bname = bids.Key;
                    string tmpbid = (string)(bids.Value);

                    BoardInfo boardinfo = new BoardInfo(bname, tmpbid);
                    board_list.Add(boardinfo);

                    if (tmpbid == bid)
                    {
                        JArray lists = tl.getLists(bid);

                        foreach (JObject obj in lists)
                        {
                            string lname = (string)(obj["name"]);
                            string lid = (string)(obj["id"]);

                            JArray scards = tl.getCards(lid);
                            string cname;
                            string cid;
                            ListInfo tmpinfo = new ListInfo(lname, lid);
                            foreach (JObject card in scards)
                            {
                                cname = (string)(card["name"]);
                                cid = (string)(card["id"]);
                                /*if (cname.Length > 7)
                                {
                                    cname = cname.Substring(0, 7);
                                    cname += "...";
                                }*/
                                
                                tmpinfo.add(cname, cid);
                            }

                            boardinfo.add(tmpinfo);
                        }
                    }
                }
                ViewBag.board_name = name;
                ViewBag.board_id = bid;

                return View(board_list);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            EmptyResult re = new EmptyResult();

            return re;
            
            // get boards
            /*trello tl = new trello();


            List<BoardInfo> board_list = new List<BoardInfo>();
            JObject bids_info = tl.get_board();
            foreach (var bids in bids_info)
            {
                string bname = bids.Key;
                string bid = (string)(bids.Value);

                List<ListInfo> listinfo = new List<ListInfo>();
                JArray lists = tl.getLists(bid);

                BoardInfo boardinfo = new BoardInfo(bname, bid);

                foreach (JObject obj in lists)
                {
                    string lname = (string)(obj["name"]);
                    string lid = (string)(obj["id"]);

                    JArray scards = tl.getCards(lid);
                    string cname;
                    string cid;
                    ListInfo tmpinfo = new ListInfo(lname, lid);
                    foreach (JObject card in scards)
                    {
                        cname = (string)(card["name"]);
                        cid = (string)(card["id"]);
                        tmpinfo.add(cname, cid);

                    }

                    boardinfo.add(tmpinfo);
                }
                board_list.Add(boardinfo);
            }

            return View(board_list);
            */
        }

        public string get_comments(string cid)
        {
            try
            {
                trello tl = new trello();
                return tl.get_comments(cid);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            return "";
        }

        public string add_comment(string cid, string comment)
        {
            try
            {
                trello tl = new trello();
                return tl.add_comment(cid, comment);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            return "";
        }
    }
}
