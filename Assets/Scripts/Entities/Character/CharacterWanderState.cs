using Unity.Mathematics;

namespace MJM
{
    public class CharacterWanderState : CharacterMovementState
    {
        public CharacterWanderState(Character character) : base(character) { }

        public override void Tick()
        {
            if (_character.CanAct())
            {
                Direction newDirection = Utils.RandomEnumValue<Direction>();

                _character.Direction = newDirection;

                _character.UpdateRenderDirection();

                int2 newPosition = _character.Position + WorldInfo.DirectionVectors[newDirection];

                if (GameManager.Instance.WorldSystem.IsPassableMove(_character.Position, newDirection)
                    && !GameManager.Instance.WorldSystem.IsOccupied(newPosition))
                {
                    GameManager.Instance.WorldSystem.SetCharacterInCell(_character.Position, null);

                    GameManager.Instance.WorldSystem.SetCharacterInCell(newPosition, _character);

                    _character.Cooldown = WorldInfo.DirectionCosts[newDirection];
                    _character.Position = newPosition;

                    _character.UpdateRenderPosition();
                }
                else
                {
                    _character.Cooldown = 5;
                }
            }
        }
    }
}
