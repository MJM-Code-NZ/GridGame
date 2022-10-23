using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM
{
    public class EntitySystem : GameSystem
    {
        public static event EventHandler<OnCharacterEventArgs> OnCreateCharacter;
        public static event EventHandler<OnStructureEventArgs> OnCreateStructure;
        public static event EventHandler<OnTargetEventArgs> OnCreateTarget;

        private List<Character> _characterList;
        private List<Structure> _structureList;

        public Target Target { get; private set; }

        public override void Init()
        {
            SetupEvents();

            CreateInitialStructures();
            
            CreateInitialCharacters();

            CreateInitialTarget(); 
        }

        private void SetupEvents()
        {
            GameManager.OnTick += Tick;

            Interface.OnUpdateMovementState += UpdateMovementStates;
        }

        private void CreateInitialStructures()
        {
            _structureList = new List<Structure>(2);

            Structure newStructure;

            newStructure = new Structure(0, 2, Direction.N, Faction.Red);

            _structureList.Add(newStructure);

            GameManager.Instance.WorldSystem.GetCell(0, 2).Passable = false;

            OnCreateStructure?.Invoke(this, new OnStructureEventArgs { Structure = newStructure });

            newStructure = new Structure(0, GameManager.Instance.WorldSystem.WorldMap.Width - 3, Direction.S, Faction.Blue);

            _structureList.Add(newStructure);

            GameManager.Instance.WorldSystem.GetCell(0, GameManager.Instance.WorldSystem.WorldMap.Width - 3).Passable = false;

            OnCreateStructure?.Invoke(this, new OnStructureEventArgs { Structure = newStructure });
        }
        
        private void CreateInitialCharacters()
        {
            _characterList = new List<Character>(EntityInfo.TotalCharacters);

            Character newCharacter;

            foreach (Structure structure in _structureList)
            {
                newCharacter = new Character(structure.Position + WorldInfo.DirectionVectors[structure.Direction], structure.Direction, structure.Faction);

                PlaceCharacterInWorld(newCharacter);

                Direction spawnDirection;

                spawnDirection = WorldInfo.RotateDirection(structure.Direction, -1);

                newCharacter = new Character(structure.Position + WorldInfo.DirectionVectors[spawnDirection], structure.Direction, structure.Faction);

                PlaceCharacterInWorld(newCharacter);

                spawnDirection = WorldInfo.RotateDirection(structure.Direction, +1);

                newCharacter = new Character(structure.Position + WorldInfo.DirectionVectors[spawnDirection], structure.Direction, structure.Faction);

                PlaceCharacterInWorld(newCharacter);
            }
        }

        private void PlaceCharacterInWorld(Character character)
        {
            GameManager.Instance.WorldSystem.SetCharacterInCell(character.Position, character);

            _characterList.Add(character);

            OnCreateCharacter?.Invoke(this, new OnCharacterEventArgs { Character = character });
        }

        protected override void Tick(object sender, OnTickArgs eventArgs)
        {
            foreach (Character character in _characterList)
            {
                character.Tick();
            }
        }

        private void CreateInitialTarget()
        {
            Target = new Target(0, GameManager.Instance.WorldSystem.WorldMap.Size);

            OnCreateTarget?.Invoke(this, new OnTargetEventArgs { Target = Target });
        }

        public override void Quit()
        {
            GameManager.OnTick -= Tick;

            Interface.OnUpdateMovementState -= UpdateMovementStates;
        }

        private void UpdateMovementStates(object sender, OnUpdateCharacterMovementStateArgs eventArgs)
        {
            foreach (Character character in _characterList)
            {
                character.SetMovementState(eventArgs.CharacterMovementStateType);
            }
        }
    }
}


