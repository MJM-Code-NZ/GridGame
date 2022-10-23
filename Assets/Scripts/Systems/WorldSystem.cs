using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM
{
    public class WorldSystem : GameSystem
    {
        public static event EventHandler<OnWorldMapEventArgs> OnUpdateWorldMapRender;

        public WorldMap WorldMap { get; private set; }
        //private WorldMap _worldMap;

        public override void Init()
        {
            SetupEvents();

            GenerateWorldMap();
        }

        private void SetupEvents()
        {
            GameManager.OnTick += Tick;
        }

        private void GenerateWorldMap()
        {
            WorldMap = new WorldMap(WorldInfo.WorldSize);

            for (int id = 0; id < WorldMap.Area; id++)
            {
                Cell cell = new Cell(id)
                {
                    Position = IdToPosition(id),
                    GroundType = GroundType.Ground1,
                    StructureType = StructureType.None,
                    OverlayType = OverlayType.None,
                    CharacterInCell = null
                };

                // Give map solid edges
                if (cell.Position.x <= -WorldMap.Size || cell.Position.x >= WorldMap.Size
                    || cell.Position.y <= 0 || cell.Position.y >= WorldMap.Width - 1)
                {
                    cell.GroundType = GroundType.Ground2;
                }

                cell.Passable = WorldInfo.GroundPassable[cell.GroundType];

                WorldMap.Cells.Add(cell);
            }

            if (WorldMap.Size > 6)
            {
                // Add map features

                SetCell(-WorldMap.Size + 2, 3, GroundType.Ground2);
                SetCell(-WorldMap.Size + 3, 2, GroundType.Ground2);

                SetCell(-WorldMap.Size + 2, WorldMap.Width - 4, GroundType.Ground2);
                SetCell(-WorldMap.Size + 3, WorldMap.Width - 3, GroundType.Ground2);

                SetCell(WorldMap.Size - 2, 3, GroundType.Ground2);
                SetCell(WorldMap.Size - 3, 2, GroundType.Ground2);

                SetCell(WorldMap.Size - 2, WorldMap.Width - 4, GroundType.Ground2);
                SetCell(WorldMap.Size - 3, WorldMap.Width - 3, GroundType.Ground2);

                SetCell(WorldMap.Size - 3, WorldMap.Size, GroundType.Ground2);
                SetCell(WorldMap.Size - 2, WorldMap.Size, GroundType.Ground2);
                SetCell(WorldMap.Size - 4, WorldMap.Size, GroundType.Ground2);
                SetCell(WorldMap.Size - 3, WorldMap.Size - 1, GroundType.Ground2);
                SetCell(WorldMap.Size - 3, WorldMap.Size + 1, GroundType.Ground2);

                SetCell(-WorldMap.Size + 3, WorldMap.Size, GroundType.Ground2);
                SetCell(-WorldMap.Size + 2, WorldMap.Size, GroundType.Ground2);
                SetCell(-WorldMap.Size + 4, WorldMap.Size, GroundType.Ground2);
                SetCell(-WorldMap.Size + 3, WorldMap.Size - 1, GroundType.Ground2);
                SetCell(-WorldMap.Size + 3, WorldMap.Size + 1, GroundType.Ground2);
            }

            OnUpdateWorldMapRender?.Invoke(this, new OnWorldMapEventArgs { WorldMap = WorldMap });
        }

        protected override void Tick(object sender, OnTickArgs eventArgs)
        {
            //            Debug.Log(eventArgs.Tick);
        }

        public override void Quit()
        {
            GameManager.OnTick -= Tick;
        }

        public Cell GetCell(int id)
        {
            if (id >= WorldMap.Area) return null;

            return WorldMap.Cells[id];
        }

        public Cell GetCell(int x, int y)
        {
            int cellId = PositionToId(x, y);

            return WorldMap.Cells[cellId];
        }

        public Cell GetCell(int2 position)
        {
            return GetCell(position.x, position.y);
        }

        public bool IsPassable(int x, int y)
        {
            if (OnMap(x, y))
            {
                Cell cell = GetCell(x, y);

                return cell.Passable;
            }
            else
            {
                return false;
            }
        }

        public bool IsPassable(int2 position)
        {
            return IsPassable(position.x, position.y);
        }

        public bool IsOccupied(int x, int y)
        {
            if (OnMap(x, y))
            {
                Cell cell = GetCell(x, y);

                if (cell.CharacterInCell != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsOccupied(int2 position)
        {
            return IsOccupied(position.x, position.y);
        }

        private void SetCell(int x, int y, GroundType groundType)
        {
            if (OnMap(x, y))
            {
                Cell cell = GetCell(x, y);
                cell.GroundType = groundType;
                cell.Passable = WorldInfo.GroundPassable[cell.GroundType];
            }
        }

        private void SetCell(int2 position, GroundType groundType)
        {
            SetCell(position.x, position.y, groundType);
        }

        private void SetCell(int x, int y, StructureType structureType)
        {
            if (OnMap(x, y))
            {
                Cell cell = GetCell(x, y);
                cell.StructureType = structureType;
                if (structureType != StructureType.None)
                {
                    cell.Passable = false;
                }
            }
        }

        public void SetCharacterInCell(int x, int y, Character character)
        {
            if (OnMap(x, y))
            {
                Cell cell = GetCell(x, y);

                if (character != null && cell.CharacterInCell != null)
                {
                    Debug.Log("Error cell already occupied" + x + y);
                }

                cell.CharacterInCell = character;
            }
        }

        public void SetCharacterInCell(int2 position, Character character)
        {
            SetCharacterInCell(position.x, position.y, character);
        }

        public bool OnMap(int x, int y)
        {
            bool insideHorizontalBounds = x >= -WorldMap.Size && x <= WorldMap.Size;
            bool insideVerticalBounds = y >= 0 && y <= WorldMap.Width;

            return insideHorizontalBounds && insideVerticalBounds;
        }

        public bool OnMap(int2 position)
        {
            return OnMap(position.x, position.y);
        }

        private int2 IdToPosition(int id)
        {
            int x = id % WorldMap.Width - WorldMap.Size;
            int y = id / WorldMap.Width;

            return new int2(x, y);
        }

        public int PositionToId(int x, int y)
        {
            return (x + WorldMap.Size) + WorldMap.Width * y;
        }

        public int PositionToId(int2 position)
        {
            return PositionToId(position.x, position.y);
        }

        public int2 GetOpenPosition()
        {
            int2 cellPosition;

            int i = 0;

            do
            {
                cellPosition = new int2(
                    Utils.RandomRange(-WorldMap.Size, WorldMap.Size),
                    Utils.RandomRange(0, WorldMap.Width - 1)
                    );
                i++;
                //Debug.Log(i);
            }
            while (!IsPassable(cellPosition) || IsOccupied(cellPosition) && i < 10000);

            if (i >= 10000)
            {
                Debug.Log("10000 failed iterations finding open position");
            }

            return cellPosition;
        }

        public bool IsPassableMove(int2 startPosition,int2 directionVector)
        {
            int2 endPosition = startPosition + directionVector;

            if (!IsPassable(endPosition))
            {
                return false;
            }

            bool cardinalMove = (directionVector.x == 0 || directionVector.y == 0);

            if (cardinalMove)
            {
                return true;
            }

            bool northPassable = IsPassable(startPosition + WorldInfo.DirectionVectors[Direction.N]);
            bool westPassable = IsPassable(startPosition + WorldInfo.DirectionVectors[Direction.W]);
            bool southPassable = IsPassable(startPosition + WorldInfo.DirectionVectors[Direction.S]);
            bool eastPassable = IsPassable(startPosition + WorldInfo.DirectionVectors[Direction.E]);

            if (directionVector.x == -1 && directionVector.y == 1) //Direction.NW
            {
                return northPassable || westPassable;
            }
            else if (directionVector.x == -1 && directionVector.y == -1) //Direction.SW
            {
                return southPassable || westPassable;
            }
            else if (directionVector.x == 1 && directionVector.y == -1) //Direction.SE
            {
                return southPassable || eastPassable;
            }
            else if (directionVector.x == 1 && directionVector.y == 1) //Direction.NE
            {
                return northPassable || eastPassable;
            }

            return false;
        }

        public bool IsPassableMove(int2 startPosition, Direction direction)
        {
            return IsPassableMove(startPosition, WorldInfo.DirectionVectors[direction]);
        }

        public int2 FindMoveToTarget(int2 startPosition, int2 targetPosition)
        {
            int2 positionDelta = new int2(targetPosition.x - startPosition.x,
                                       targetPosition.y - startPosition.y);

            // possible moves contain full length vectors not 1 unit length vectors
            List <int2> possibleMoves = new List<int2>();

            if (positionDelta.x == 0)
            {
                if (positionDelta.y == 0)
                {
                    // already at target
                    return new int2(0, 0);
                }
                else
                {
                    // already at x target

                    possibleMoves.Add(new int2(0, positionDelta.y));
                }
            }
            else if (positionDelta.y == 0)
            {
                // already at y target

                possibleMoves.Add(new int2(positionDelta.x, 0));
            }
            else
            {
                possibleMoves.Add(new int2(positionDelta.x, positionDelta.y));

                if (Math.Abs(positionDelta.x) >= Math.Abs(positionDelta.y))
                {
                    // x distance greater than y

                    possibleMoves.Add(new int2(positionDelta.x, 0));
                    possibleMoves.Add(new int2(0, positionDelta.y));
                }
                else
                {
                    // y distance greater than x

                    possibleMoves.Add(new int2(0, positionDelta.y));
                    possibleMoves.Add(new int2(positionDelta.x, 0));
                }
            }

            // at this point possible moves will contain either 0, 1 or 3 entries
            // adjustedMove will take possible moves and set x and y to absolute value of 1
            int2 adjustedMove = new int2();

            if (possibleMoves.Count == 3)
            {
                // Determine which of first 2 entries to try first

                int firstWeight = Math.Abs(possibleMoves[0].x)
                                + Math.Abs(possibleMoves[0].y)
                                - Math.Abs(possibleMoves[1].x)
                                - Math.Abs(possibleMoves[1].y);

                int secondWeight = Math.Abs(possibleMoves[1].x)
                                + Math.Abs(possibleMoves[1].y);

                if (Utils.RandomRange(1, firstWeight + secondWeight) <= firstWeight)
                {
                    //try 1st
                    adjustedMove = WorldInfo.VectorToCellVector(possibleMoves[0]);

                    if (IsPassableMove(startPosition, adjustedMove)  && !IsOccupied(startPosition + adjustedMove))
                    {
                        return adjustedMove;
                    }
                    //try 2nd
                    adjustedMove = WorldInfo.VectorToCellVector(possibleMoves[1]);

                    if (IsPassableMove(startPosition, adjustedMove) && !IsOccupied(startPosition + adjustedMove))
                    {
                        return adjustedMove;
                    }
                }
                else
                {
                    //try 2nd
                    adjustedMove = WorldInfo.VectorToCellVector(possibleMoves[1]);

                    if (IsPassableMove(startPosition, adjustedMove) && !IsOccupied(startPosition + adjustedMove))
                    {
                        return adjustedMove;
                    }
                    //try 1st
                    adjustedMove = WorldInfo.VectorToCellVector(possibleMoves[0]);

                    if (IsPassableMove(startPosition, adjustedMove) && !IsOccupied(startPosition + adjustedMove))
                    {
                        return adjustedMove;
                    }
                }
                // try 3rd
                adjustedMove = WorldInfo.VectorToCellVector(possibleMoves[2]);

                if (IsPassableMove(startPosition, adjustedMove) && !IsOccupied(startPosition + adjustedMove))
                {
                    return adjustedMove;
                }
            }
            else if (possibleMoves.Count == 1)
            {
                // try 1st
                adjustedMove = WorldInfo.VectorToCellVector(possibleMoves[0]);

                if (IsPassableMove(startPosition, adjustedMove) && !IsOccupied(startPosition + adjustedMove))
                {
                    return adjustedMove;
                }
            }
            else
            {
                // already at target
                return new int2(0, 0);
            }
            //catch error?
            return new int2(0, 0);

        }
    }
}
