using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.AugustCellars.CoAP.Log;
using Com.AugustCellars.CoAP.Server;
using PubSub;

namespace PubSubServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Instance = new FileLogManager(Console.Out);
            LogManager.Level = LogLevel.All;

            CoapServer server = new CoapServer();

            PubSubResource pubsub = new PubSubResource("ps");

            server.Add(pubsub);

            server.Start();

            Console.ReadLine();

            server.Stop();
        }
    }
}
