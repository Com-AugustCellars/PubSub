using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Com.AugustCellars.CoAP;

namespace PubSubClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);

            CoapClient client = new CoapClient() {
                Uri = new Uri("coap://localhost"),
                UriPath = "/ps"
            };

            Response resp = client.Get();
            Console.WriteLine($"Discover ==> {resp.ToString()}");


            resp = client.Post("<topic1>;ct=40", MediaType.ApplicationLinkFormat);
            Console.WriteLine($"Create topic1 ==> {resp.ToString()}");

            client.UriPath = "/ps/topic1";
            resp = client.Post("<subtopic1>;ct=0;rt=temperature;obs", MediaType.ApplicationLinkFormat);
            Console.WriteLine($"Create subtopic1 ==> {resp.ToString()}");

            client.UriPath = "/ps";
            resp = client.Get();
            Console.WriteLine($"Discover ==> {resp.PayloadString}");


            client.UriPath = "/ps/topic1/subtopic1";
            resp = client.Post("Payload #1", MediaType.TextPlain);
            Console.WriteLine($"Post #1 ==> {resp.ToString()}");

            resp = client.Post("Payload #2", MediaType.TextPlain);
            Console.WriteLine($"Post #2 ==> {resp.ToString()}");

            resp = client.Post("Payload #3", MediaType.TextXml);
            Console.WriteLine($"Post #2 ==> {resp.ToString()}");


        }
    }
}
