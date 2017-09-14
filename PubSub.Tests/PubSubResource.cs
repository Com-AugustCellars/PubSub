using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.AugustCellars.CoAP;
using Com.AugustCellars.CoAP.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using Com.AugustCellars.CoAP.Server;
using Com.AugustCellars.CoAP.Server.Resources;

namespace PubSub.Tests
{

    [TestFixtureAttribute]
    public class PubSubResource
    {
        private CoapServer _server;
        private int _port;

        [SetUpAttribute]
        public void SetupServer()
        {
            _server = new CoapServer();
            CoAPEndPoint ep = new CoAPEndPoint(0);
            _server.AddEndPoint(ep);

            Resource r1 = new PubSub.PubSubResource("ps", true);
            _server.Add(r1);
            _server.Start();

            _port = ((System.Net.IPEndPoint) ep.LocalEndPoint).Port;
        }

        [TearDown]
        public void TeardownServer()
        {
            _server.Stop();
            _server.Dispose();
            _server = null;
        }

        [TestMethod]
        public void TopTierTest()
        {
            CoapClient client = new CoapClient(new Uri($"coap://localhost:{_port}/ps"));
            Response resp = client.Get();
            Assert.AreEqual(StatusCode.Content, resp.StatusCode);
            Assert.AreEqual(MediaType.ApplicationLinkFormat, resp.ContentType);
            List<WebLink> links = LinkFormat.Parse(resp.PayloadString).ToList();
            Assert.AreEqual(0, links.Count);

            resp = client.Post($"<topic1>;ct={MediaType.TextPlain}", MediaType.ApplicationLinkFormat);
            Assert.AreEqual(StatusCode.Created, resp.StatusCode);
            Assert.AreEqual("ps/topic1", resp.LocationPath);

            resp = client.Post($"<topic2>;ct={MediaType.ApplicationCbor}", MediaType.ApplicationLinkFormat);
            Assert.AreEqual(StatusCode.Created, resp.StatusCode);
            Assert.AreEqual("ps/topic2", resp.LocationPath);

            resp = client.Get();
            Assert.AreEqual(StatusCode.Content, resp.StatusCode);
            Assert.AreEqual(MediaType.ApplicationLinkFormat, resp.ContentType);
            links = LinkFormat.Parse(resp.PayloadString).ToList();
            Assert.AreEqual(2, links.Count);
            string[] linkStrings = resp.PayloadString.Split(',');

            Assert.AreEqual("</ps/topic2>;ct=60;obs", linkStrings[0]);
            Assert.AreEqual("</ps/topic1>;ct=0;obs", linkStrings[1]);
        }
    }
}
