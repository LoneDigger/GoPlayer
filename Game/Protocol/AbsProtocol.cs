using Game.Bundle;
using System;

namespace Game.Protocol
{
    public abstract class AbsProtocol : IDisposable
    {
        public delegate void OnMessage(bool closed, BroadcastBundle b);

        public abstract bool IsConnected { get; }

        protected readonly OnMessage handler;

        public AbsProtocol(OnMessage handler)
        {
            this.handler = handler;
        }

        public abstract void Connect();

        public abstract void Write(byte[] bytes);

        public abstract void Dispose();
    }
}
