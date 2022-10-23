using Unity.Mathematics;

namespace MJM
{
    public class Structure
    {
        public int Id { get; private set; }
        public int2 Position { get; private set; }

        public Direction Direction { get; private set; }

        public Faction Faction { get; private set; }

        public Structure(int x, int y, Direction direction, Faction faction)
        {        
            Position = new int2(x, y);
            Direction = direction;
            Faction = faction;      
        }
    }
}
