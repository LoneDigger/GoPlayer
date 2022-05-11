using Game.Bundle;
using Game.KCP;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace Game.Protocol
{
    internal class KcpProt : AbsProtocol
    {
        private readonly byte[] buffer = new byte[1024];
        private readonly UDPSession connection;

        public KcpProt(OnMessage handler) : base(handler)
        {
            connection = new UDPSession
            {
                AckNoDelay = true,
                WriteDelay = false
            };
        }

        public override bool IsConnected
        {
            get
            {
                return connection.IsConnected;
            }
        }

        public override void Connect()
        {
            connection.Connect("127.0.0.1", 6060);

            new Thread(() =>
            {
                int n;
                string str;
                BroadcastBundle b;

                while (connection.IsConnected)
                {
                    connection.Update();
                    try
                    {
                        n = connection.Recv(buffer, 0, buffer.Length);
                    }
                    catch (Exception e)
                    {
                        handler.Invoke(true, new BroadcastBundle());
                        Console.WriteLine(e.ToString());
                        return;
                    }

                    if (n == 0)
                        Thread.Sleep(10);
                    else if (n < 0)
                        break;
                    else
                    {
                        str = Encoding.UTF8.GetString(buffer, 0, n);
                        b = JsonConvert.DeserializeObject<BroadcastBundle>(str);
                        handler.Invoke(false, b);
                    }
                }
            }).Start();
        }

        public override void Dispose()
        {
            connection.Close();
        }

        public override void Write(byte[] bytes)
        {
            if (connection.IsConnected)
                connection.Send(bytes, 0, bytes.Length);
        }
    }
}
