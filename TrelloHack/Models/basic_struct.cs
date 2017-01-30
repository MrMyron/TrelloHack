using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CardInfo = TrelloHack.Models.BasicInfo;

namespace TrelloHack.Models
{
    public class BasicInfo
    {
        public string Name{ get; set; }

        public string Id { get; set; }

        public BasicInfo(string _name, string _id)
        {
            this.Name = _name;
            this.Id = _id;
        }

        //public BasicInfo() { }
    }

    public class ListInfo
    {
        public BasicInfo info;
        public List<CardInfo> mlist;

        public ListInfo(string _name, string _id)
        {
            info = new BasicInfo(_name, _id);
            mlist = new List<CardInfo>();
        }

        public void add(string cardname, string cardid)
        {
            this.mlist.Add(new CardInfo(cardname, cardid));
        }

        public int count()
        {
            return mlist.Count();
        }
    }

    public class BoardInfo
    {
        public BasicInfo info;
        public List<ListInfo> mlist;

        public BoardInfo(string _name, string _id)
        {
            info = new BasicInfo(_name, _id);
            mlist = new List<ListInfo>();
        }

        public void add(ListInfo info)
        {
            this.mlist.Add(info);
        }
    }
}