using Game.Bundle;

namespace Game.Module
{
    public class Player
    {
        public uint ID;
        public Point Point;

        public Player()
        {
        }

        public Player(uint id, Point p)
        {
            ID = id;
            Point = p;
        }
    }
}
