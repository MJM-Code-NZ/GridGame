using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


namespace MJM
{
    public class Character
    {
        private static int _nextCharacterId = 1;

        public static event EventHandler<OnCharacterEventArgs> OnUpdateCharacterRenderDirection;
        public static event EventHandler<OnCharacterEventArgs> OnUpdateCharacterRenderPosition;

        public int Id { get; private set; }
        public int2 Position { get; set; }
        
        public Direction Direction { get; set; }

        public Faction Faction { get; set; }

        public int Cooldown { get; set; }

        private readonly Dictionary<CharacterMovementStateType, CharacterMovementState> _movementStates;

        private CharacterMovementState _currentMovementState;

        public Character(int2 position, Direction direction, Faction faction)
        {
            Id = _nextCharacterId++;

            Position = position;
            Direction = direction;
            Faction = faction;

            Cooldown = Utils.RandomRange(0, 5);

            _movementStates = new Dictionary<CharacterMovementStateType, CharacterMovementState>
            {
                [CharacterMovementStateType.Idle] = new CharacterIdleState(this),
                [CharacterMovementStateType.Wander] = new CharacterWanderState(this),
                [CharacterMovementStateType.Target] = new CharacterTargetState(this),
            };

            _currentMovementState = _movementStates[CharacterMovementStateType.Idle];
        }

        public void Tick()
        {
            Cooldown--;

            _currentMovementState.Tick();
        }

        public bool CanAct()
        {
            return Cooldown <= 0;
        }

        public void SetMovementState(CharacterMovementStateType characterMovementStateType)
        {
            _currentMovementState = _movementStates[characterMovementStateType];
        }

        public void UpdateRenderDirection()
        {
            OnUpdateCharacterRenderDirection?.Invoke(this, new OnCharacterEventArgs { Character = this });
        }

        public void UpdateRenderPosition()
        {
            OnUpdateCharacterRenderPosition?.Invoke(this, new OnCharacterEventArgs { Character = this });
        }
    }
}
