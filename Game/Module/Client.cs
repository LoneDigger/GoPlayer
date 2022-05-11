using Game.Bundle;
using Game.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Game.Protocol.AbsProtocol;

namespace Game.Module
{
    public class Client : IDisposable
    {
        //self
        private readonly Player Player = new Player();

        public Point Point
        {
            get
            {
                return Player.Point;
            }
        }

        public bool IsConnected
        {
            get
            {
                return prot.IsConnected;
            }
        }

        private readonly AbsProtocol prot;

        public Dictionary<uint, Player> Around { get; private set; }

        private readonly IMessageHandle handle;

        public Client(ProtocolType type, IMessageHandle handle)
        {
            this.handle = handle;
            Around = new Dictionary<uint, Player>();

            OnMessage handler = delegate (bool closed, BroadcastBundle message)
            {
                if (closed)
                {
                    handle.OnClosed();
                    return;
                }

                string str = message.Message.ToString();

                switch (message.Code)
                {
                    case APICode.AddBcstCode:
                        {
                            AddPlayerBundle add = JsonConvert.DeserializeObject<AddPlayerBundle>(str);
                            //Console.WriteLine("add " + add.ID);
                            lock (Around)
                                if (!Around.ContainsKey(add.ID))
                                    Around.Add(add.ID, new Player(add.ID, add.Point));
                        }
                        break;

                    case APICode.RemoveBcstCode:
                        {
                            RemovePlayerBundle remove = JsonConvert.DeserializeObject<RemovePlayerBundle>(str);
                            //Console.WriteLine("remove " + remove.ID);
                            lock (Around)
                                Around.Remove(remove.ID);
                        }
                        break;

                    case APICode.MoveBcstCode:
                        {
                            MoveBundle move = JsonConvert.DeserializeObject<MoveBundle>(str);
                            lock (Around)
                                if (Around.ContainsKey(move.ID))
                                    Around[move.ID].Point = move.Point;
                        }
                        break;

                    case APICode.AddAckCode:
                        {
                            AddPlayerBundle add = JsonConvert.DeserializeObject<AddPlayerBundle>(str);
                            Player.ID = add.ID;
                            Player.Point.X = add.Point.X;
                            Player.Point.Y = add.Point.Y;
                            handle.OnConnected();
                        }
                        break;

                    case APICode.DegreeAckCode:
                        {
                            DegreeAckBundle degree = JsonConvert.DeserializeObject<DegreeAckBundle>(str);
                            Player.Point.X = degree.Point.X;
                            Player.Point.Y = degree.Point.Y;
                        }
                        break;

                    case APICode.EchoReqCode:
                        Send(APICode.EchoAckCode, new EchoResponse());
                        break;

                    case APICode.CloseCode:
                        Dispose();
                        handle.OnClosed();
                        break;
                }
            };

            switch (type)
            {
                case ProtocolType.Websocket:
                    prot = new WsProt(handler);
                    break;

                case ProtocolType.Kcp:
                    prot = new KcpProt(handler);
                    break;
            }
        }

        public void Connect(string name)
        {
            prot.Connect();
            Send(APICode.AddReqCode, new AddRequestBundle
            {
                Name = name,
            });
        }

        public void Move(float degree)
        {
            Send(APICode.DegreeReqCode, new DegreeReqBundle
            {
                Degree = degree,
            });
        }

        private void Send(APICode code, object obj)
        {
            string text = JsonConvert.SerializeObject(new BroadcastBundle
            {
                Code = code,
                Message = obj,
            });
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            prot.Write(bytes);
        }

        public void Dispose()
        {
            Send(APICode.CloseCode, new CloseBundle());
            Around.Clear();
            prot.Dispose();
        }
    }
}
