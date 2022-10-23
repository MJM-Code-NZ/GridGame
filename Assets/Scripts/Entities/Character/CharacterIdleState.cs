using UnityEngine;

namespace MJM
{
    public class CharacterIdleState : CharacterMovementState
    {
        private int _idleRotationDirection;
        
        public CharacterIdleState(Character character) : base(character)
        {
            _idleRotationDirection = 0;
        }

        public override void Tick()
        {
            if (_character.CanAct())
            {
                int idleChoice;

                if (_idleRotationDirection == 0)
                {
                    idleChoice = Utils.RandomRange(1, 5);

                    if (idleChoice > 2 )
                    {
                        // don't change direction - 5 tick pause
                        _character.Cooldown = Utils.RandomRange(5, 10);
                    }
                    else if (idleChoice == 1)
                    {
                        _idleRotationDirection = -1;
                        _character.Cooldown = 1;
                    }
                    else //idleChoice == 2
                    {
                        _idleRotationDirection = 1;
                        _character.Cooldown = 1;
                    }
                }
                else
                { 
                    idleChoice = Utils.RandomRange(1, 2);

                    if (idleChoice == 1)
                    {
                        // keep rotating
                        _character.Cooldown = 1;
                    }
                    else 
                    {
                        // stop rotating - 5 tick pause
                        _idleRotationDirection = 0;
                        _character.Cooldown = 5;
                    }
                }
                
                if (_idleRotationDirection != 0)
                {
                    _character.Direction = WorldInfo.RotateDirection(_character.Direction, _idleRotationDirection);

                    _character.UpdateRenderDirection();
                }            
            }
        }
    }
}
