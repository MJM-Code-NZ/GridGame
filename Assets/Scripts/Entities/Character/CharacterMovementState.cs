using UnityEngine;

namespace MJM
{

    public abstract class CharacterMovementState
    {
        protected Character _character;

        public CharacterMovementState(Character character)
        {
            _character = character;
        }

        public abstract void Tick();
    }
}
