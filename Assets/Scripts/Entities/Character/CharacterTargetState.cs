using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace MJM
{
    public class CharacterTargetState : CharacterMovementState
    {
        public CharacterTargetState(Character character) : base(character) { }

        public override void Tick()
        {
            if (_character.CanAct())
            {
                int2 move = GameManager.Instance.WorldSystem.FindMoveToTarget(_character.Position, GameManager.Instance.EntitySystem.Target.Position);              

                if (move.x != 0 || move.y != 0)
                {
                    _character.Direction = WorldInfo.LookUpDirectionVector(move);

                    _character.UpdateRenderDirection();

                    int2 newPosition = _character.Position + move;
                   
                    GameManager.Instance.WorldSystem.SetCharacterInCell(_character.Position, null);

                    GameManager.Instance.WorldSystem.SetCharacterInCell(newPosition, _character);

                    _character.Cooldown = WorldInfo.DirectionCosts[_character.Direction];
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