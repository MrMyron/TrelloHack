using TrelloHack.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Newtonsoft.Json.Linq;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace TrelloTest
{
    
    
    /// <summary>
    ///这是 trelloTest 的测试类，旨在
    ///包含所有 trelloTest 单元测试
    ///</summary>
    [TestClass()]
    public class trelloTest
    {
        private string auth_token = "6bf21161ca189a9533901f07b211ad304fd9781d4e698441d8a0aa17e7f4e792";
        private const string redis_ip = "127.0.0.1";
        private const int redis_port = 6379; 

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
            RedisClient client = new RedisClient(redis_ip, redis_port);
            client.Add<string>("oauth_token", auth_token);
        }
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///get_board 的测试
        ///</summary>
        // TODO: 确保 UrlToTest 特性指定一个指向 ASP.NET 页的 URL(例如，
        // http://.../Default.aspx)。这对于在 Web 服务器上执行单元测试是必需的，
        //无论要测试页、Web 服务还是 WCF 服务都是如此。
        [TestMethod()]
        [UrlToTest("http://localhost:3877")]
        public void get_boardTest()
        {
            trello target = new trello(); // TODO: 初始化为适当的值
            JObject expected = null; // TODO: 初始化为适当的值
            JObject actual;
            actual = target.get_board();
            foreach (var bids in actual)
            {
                string bname = bids.Key;
                string bid = (string)(bids.Value);
                Assert.AreNotEqual(bname, "", true);
                Assert.AreNotEqual(bid, "", true);
            }
        }

        /// <summary>
        ///getLists 的测试
        ///</summary>
        // TODO: 确保 UrlToTest 特性指定一个指向 ASP.NET 页的 URL(例如，
        // http://.../Default.aspx)。这对于在 Web 服务器上执行单元测试是必需的，
        //无论要测试页、Web 服务还是 WCF 服务都是如此。
        [TestMethod()]
        [UrlToTest("http://localhost:3877")]
        public void getListsTest()
        {
            trello target = new trello(); // TODO: 初始化为适当的值
            JArray expected = null; // TODO: 初始化为适当的值
            JObject actual;
            actual = target.get_board();
            foreach (var bids in actual)
            {
                string bname = bids.Key;
                string bid = (string)(bids.Value);

                JArray lists = target.getLists(bid);
                foreach (JObject card in lists)
                {
                    string cname = (string)(card["name"]);
                    string cid = (string)(card["id"]);
                    Assert.AreNotEqual(cname, "", true);
                    Assert.AreNotEqual(cid, "", true);
                }
            }
        }
    }
}
