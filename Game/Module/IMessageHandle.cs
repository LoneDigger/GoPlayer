namespace Game.Module
{
    public interface IMessageHandle
    {
        void OnConnected();

        void OnClosed();
    }
}
