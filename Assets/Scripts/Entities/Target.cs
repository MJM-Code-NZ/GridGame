using Unity.Mathematics;

namespace MJM
{
    public class Target
    {
        public int2 Position { get; private set; }

         public Target(int x, int y)
        {
            Position = new int2(x, y);           
        }
    }
}
