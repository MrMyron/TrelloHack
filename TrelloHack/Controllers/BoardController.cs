using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrelloHack.Models;
using Newtonsoft.Json.Linq;

namespace TrelloHack.Controllers
{
    public class BoardController : Controller
    {
        //
        // GET: /Board/

        public ActionResult Index()
        {
            try
            {
                // get boards
                trello tl = new trello();

                List<BoardInfo> board_list = new List<BoardInfo>();
                JObject bids_info = tl.get_board();
                foreach (var bids in bids_info)
                {
                    string bname = bids.Key;
                    string bid = (string)(bids.Value);

                    //bname = bname.Substring(0,10);
                    BoardInfo boardinfo = new BoardInfo(bname, bid);
                    board_list.Add(boardinfo);
                }
                return View(board_list);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            EmptyResult re = new EmptyResult();

            return re;
        }

    }
}
