using System;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Google.Protobuf;
using Shared.Message;

namespace TestNetMQ.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            NetMQRuntime mqRuntime = new NetMQRuntime();
            mqRuntime.Run(Run(args));
        }

        static async Task Run(string[] args)
        {
            Console.WriteLine("Starting ZMQ Client");
            if (args == null || args.Length < 1)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: ./{0}", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine();
                args = new[] { "Hello" };
            }

            var arg = args[0];

            Message msg = new()
            {
                Data = arg,
                Id = 1
            };

            using var client = new RequestSocket(@">tcp://localhost:5556");

            var bufferedMsg = msg.ToByteArray();

            client.SendFrame(bufferedMsg);

            (byte[], bool) socketMsgResponse;

            while (true)
            {
                socketMsgResponse = await client.ReceiveFrameBytesAsync().ConfigureAwait(false);
                if (!socketMsgResponse.Item2)
                {
                    break;
                }
            }

            var receivedMsg = Message.Parser.ParseFrom(socketMsgResponse.Item1);

            Console.WriteLine("From Server: {0} - {1}", receivedMsg.Id, receivedMsg.Data);

            Console.WriteLine();
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
