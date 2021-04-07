using System;
using System.Threading.Tasks;
using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;
using Shared.Message;

namespace TestNetMQ.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using NetMQRuntime mqRuntime = new NetMQRuntime();
            mqRuntime.Run(Run());
        }

        static async Task Run()
        {
            Console.WriteLine("Starting ZMQ Server");

            using var server = new ResponseSocket(@"@tcp://localhost:5556");

            (byte[], bool) socketMsgRequest;

            while (true)
            {
                socketMsgRequest = await server.ReceiveFrameBytesAsync().ConfigureAwait(false);
                if (!socketMsgRequest.Item2)
                {
                    break;
                }
            }

            var receivedMsg = Message.Parser.ParseFrom(socketMsgRequest.Item1);

            Console.WriteLine("From Client: {0} - {1}", receivedMsg.Id, receivedMsg.Data);

            server.SendFrame(
                (new Message()
                {
                    Id = 2,
                    Data = "Response from server",
                })
                .ToByteArray());
        }
    }
}
