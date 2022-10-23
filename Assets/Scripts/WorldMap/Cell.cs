using Unity.Mathematics;

namespace MJM
{
    public class Cell
    {
        public int Id { get; private set; }
        public bool Passable { get; set; }
        public int2 Position { get; set; }

        public GroundType GroundType { get; set; }
        public StructureType StructureType { get; set; }
        public OverlayType OverlayType { get; set; }

        public Character CharacterInCell { get; set; }

        public Cell(int id)
        {
            Id = id;
        }
    }
}
