using Game.Bundle;
using Newtonsoft.Json;

namespace Game.Protocol
{
    internal class WsProt : AbsProtocol
    {
        private readonly WebSocketSharp.WebSocket ws;

        public override bool IsConnected
        {
            get
            {
                return ws.IsAlive;
            }
        }

        public WsProt(OnMessage handler) : base(handler)
        {
            ws = new WebSocketSharp.WebSocket("ws://127.0.0.1:6060/add");
        }

        private void Ws_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            BroadcastBundle b = JsonConvert.DeserializeObject<BroadcastBundle>(e.Data);
            handler.Invoke(false, b);
        }

        public override void Connect()
        {
            ws.Connect();
            ws.OnMessage += Ws_OnMessage;
        }

        public override void Dispose()
        {
            ws.Close();
        }

        public override void Write(byte[] bytes)
        {
            ws.Send(bytes);
        }
    }
}
