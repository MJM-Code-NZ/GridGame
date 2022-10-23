using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

namespace MJM
{
    public class WorldRender : MonoBehaviour
    {
        private RenderSettings _renderSettings;

        private Grid _grid;

        private Tilemap _groundTilemap;
        private Tilemap _overlayTilemap;

        private Dictionary<GroundType, Tile> _groundTiles;
        private Dictionary<OverlayType, Tile> _overlayTiles;

        private GameObject _charactersGameObject;
        private GameObject _structuresGameObject;
        private GameObject _targetsGameObject;

        private Dictionary<int, CharacterRenderData> _characterRenderData;

        private Dictionary<Faction, GameObject> _factionPrefab;
        private Dictionary<Faction, GameObject> _factionStructurePrefab;
        private GameObject _targetPrefab;

        // Awake is called on object creation
        void Awake()
        {
            _renderSettings = Resources.Load<RenderSettings>("Settings/RenderSettings");

            SetupEvents();

            SetupTilemapResources();

            SetupEntityResources();
        }

        private void SetupEvents()
        {
            WorldSystem.OnUpdateWorldMapRender += UpdateWorldMapRender;
            EntitySystem.OnCreateCharacter += CreateCharacterRenderData;
            EntitySystem.OnCreateStructure += CreateStructureRender;
            EntitySystem.OnCreateTarget += CreateTargetRender;

            Character.OnUpdateCharacterRenderDirection += OnUpdateCharacterRenderDirection;
            Character.OnUpdateCharacterRenderPosition += OnUpdateCharacterRenderPosition;
        }

        private void SetupTilemapResources()
        {
            _grid = GameObject.Find("Grid").GetComponent<Grid>();

            _groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();

            _groundTiles = new Dictionary<GroundType, Tile>
            {
                [GroundType.None] = null,
                [GroundType.Ground1] = Resources.Load<Tile>("Tiles/ground_1"),
                [GroundType.Ground2] = Resources.Load<Tile>("Tiles/ground_2"),
            };

            _overlayTilemap = GameObject.Find("Overlay").GetComponent<Tilemap>();

            _overlayTiles = new Dictionary<OverlayType, Tile>
            {
                [OverlayType.None] = null,      
            };
        }

        private void SetupEntityResources()
        {
            _charactersGameObject = GameObject.Find("World/Entities/Characters");

            _characterRenderData = new Dictionary<int, CharacterRenderData>();

            _factionPrefab = new Dictionary<Faction, GameObject>
            {
                [Faction.Red] = Resources.Load<GameObject>("Prefabs/RedPrefab"),
                [Faction.Blue] = Resources.Load<GameObject>("Prefabs/BluePrefab"),
            };

            _structuresGameObject = GameObject.Find("World/Entities/Structures"); 
            
            _factionStructurePrefab = new Dictionary<Faction, GameObject>
            {
                [Faction.Red] = Resources.Load<GameObject>("Prefabs/RedStructurePrefab"),
                [Faction.Blue] = Resources.Load<GameObject>("Prefabs/BlueStructurePrefab"),
            };

            _targetsGameObject = GameObject.Find("World/Entities/Targets");

            _targetPrefab = Resources.Load<GameObject>("Prefabs/TargetPrefab");
        }

        void OnDisable()
        {
            WorldSystem.OnUpdateWorldMapRender -= UpdateWorldMapRender;
            EntitySystem.OnCreateCharacter -= CreateCharacterRenderData;
            EntitySystem.OnCreateStructure -= CreateStructureRender;

            Character.OnUpdateCharacterRenderDirection -= OnUpdateCharacterRenderDirection;
            Character.OnUpdateCharacterRenderPosition -= OnUpdateCharacterRenderPosition;
        }

        private void UpdateWorldMapRender(object sender, OnWorldMapEventArgs eventArgs)
        {
            _groundTilemap.SetTile(new Vector3Int(0, 0, 0), _groundTiles[GroundType.Ground1]);

            foreach (Cell cell in eventArgs.WorldMap.Cells)
            {
                Vector3Int tilemapPosition = new Vector3Int(cell.Position.x, cell.Position.y, 0);

                _groundTilemap.SetTile(tilemapPosition, _groundTiles[cell.GroundType]);
                _overlayTilemap.SetTile(tilemapPosition, _overlayTiles[cell.OverlayType]);               
            }
        }

        private void CreateCharacterRenderData(object sender, OnCharacterEventArgs eventArgs)
        {
            Character character = eventArgs.Character;
            CharacterRenderData characterRenderData = new CharacterRenderData();

            Vector3 position = GridToWorld(character.Position);
            position.z = -character.Id * _renderSettings.EntitySpacing;

            characterRenderData.WorldGameObject = Instantiate(
                _factionPrefab[character.Faction],
                position,
                Quaternion.Euler(0, 0, 360 * (int)character.Direction / 8),
                _charactersGameObject.transform
                );

            _characterRenderData[character.Id] = characterRenderData;
        }

        private void CreateStructureRender(object sender, OnStructureEventArgs eventArgs)
        {
            Structure structure = eventArgs.Structure;

            Vector3 position = GridToWorld(structure.Position);
            position.z = -0.5f; // structure above character range

            Instantiate(
                _factionStructurePrefab[structure.Faction],
                position,
                Quaternion.identity,
                _structuresGameObject.transform
                );
        }

        private void CreateTargetRender(object sender, OnTargetEventArgs eventArgs)
        {
            Target target = eventArgs.Target;

            Vector3 position = GridToWorld(target.Position);
            position.z = -0.6f; // target above structure and character range

            Instantiate(
                _targetPrefab,
                position,
                Quaternion.identity,
                _targetsGameObject.transform
                );
        }

        private void OnUpdateCharacterRenderDirection(object sender, OnCharacterEventArgs eventArgs)
        {
            Character character = eventArgs.Character;
            CharacterRenderData characterRenderData = _characterRenderData[character.Id];
            
            characterRenderData.WorldGameObject.transform.rotation = Quaternion.Euler(0, 0, 360 * (int)character.Direction / 8);
        }

        private void OnUpdateCharacterRenderPosition(object sender, OnCharacterEventArgs eventArgs)
        {
            StartCoroutine(MoveCharacter(eventArgs.Character));
        }

        private IEnumerator MoveCharacter(Character character)
        {
            float timer = 0;
            float duration = character.Cooldown * GameInfo.TickDuration;

            CharacterRenderData characterRenderData = _characterRenderData[character.Id];

            Vector3 startPosition = characterRenderData.WorldGameObject.transform.position;

            Vector3 endPosition = GridToWorld(character.Position);
            endPosition.z = character.Id * _renderSettings.EntitySpacing;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, timer / duration);

                characterRenderData.WorldGameObject.transform.position = newPosition;

                yield return null;
            }

            characterRenderData.WorldGameObject.transform.position = endPosition;
        }

        private Vector3 GridToWorld(int x, int y)
        {
            Vector3 worldPosition = _grid.CellToWorld(new Vector3Int(x, y, 0));

            worldPosition.x += 1 / 2f;
            worldPosition.y += 1 / 2f;

            return worldPosition;
        }

        private Vector3 GridToWorld(int2 position)
        {
            return GridToWorld(position.x, position.y);
        }
    }
}
