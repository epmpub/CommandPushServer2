using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace CommandPushServer
{

    public static class MQExtensions
    {
        public static void SendT<T>(this PublisherSocket socket, T src)
        {
            var json = JsonConvert.SerializeObject(src);
            socket.SendFrame(json);
        }

        public static T ReceiveT<T>(this PublisherSocket socket)
        {
            var json = socket.ReceiveFrameString();
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        //public static void SendT<T>(this SubscriberSocket socket, T src)
        //{
        //    var json = JsonConvert.SerializeObject(src);
        //    socket.Send(json);
        //}

        //public static T ReceiveT<T>(this SubscriberSocket socket)
        //{
        //    var json = socket.ReceiveString();
        //    T obj = JsonConvert.DeserializeObject<T>(json);
        //    return obj;
        //}

    }


    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 3)
            {
                Console.WriteLine("Usage:PushServ.exe <Title> <SchedulerTime> <Times>");
                return;
            }
            else
            {

            }


            using (var pubSocket = new PublisherSocket())
            {
                Console.WriteLine("IP Binding Successfully.");
                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind("tcp://*:8081");
                Console.WriteLine("Begin to Push...");

                int Inteval = -1;
                bool b = int.TryParse(args[1], out Inteval);
                if (!b)
                {
                    return;
                }

                int times = 0;

                b = int.TryParse(args[2], out times);

                if (!b)
                {
                    return;
                }

                for (int i = 0; i < times; i++)
                {
                    var command = new CommandX { Id = 1, Name = args[0], Url = "www.baidu.com/index.html" };

                    pubSocket.SendT(command);

                    Console.Write(">");

                    Thread.Sleep(Inteval * 1000);

                    Console.WriteLine($"Push {i + 1} Done of {times}.");

                }

                Console.WriteLine("All Done.");

            }
        }
    }
}
