using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM
{
    public static class WorldInfo
    {
        public static int WorldSize = 20;

        public static Dictionary<Direction, int2> DirectionVectors = new Dictionary<Direction, int2>
        {
            [Direction.N]  = new int2(+0, +1),
            [Direction.NW] = new int2(-1, +1),
            [Direction.W]  = new int2(-1, +0),
            [Direction.SW] = new int2(-1, -1),
            [Direction.S]  = new int2(+0, -1),
            [Direction.SE] = new int2(+1, -1),
            [Direction.E]  = new int2(+1, +0),
            [Direction.NE] = new int2(+1, +1),
        };

        public static Dictionary<Direction, int> DirectionCosts = new Dictionary<Direction, int>
        {
            [Direction.N] = 10,
            [Direction.NW] = 14,
            [Direction.W] = 10,
            [Direction.SW] = 14,
            [Direction.S] = 10,
            [Direction.SE] = 14,
            [Direction.E] = 10,
            [Direction.NE] = 14,
        };

        public static Dictionary<GroundType, bool> GroundPassable = new Dictionary<GroundType, bool>
        {
            [GroundType.Ground1] = true,
            [GroundType.Ground2] = false,
        };

        public static Direction RotateDirection(Direction initialDirection, int rotationAmount)
        {
           int finalValue = (int)initialDirection + rotationAmount;

            do
            {
                if (finalValue < 0)
                {
                    finalValue += 8;
                }
                else if (finalValue > 7)
                {
                    finalValue -= 8;
                }
            }
            while (finalValue < 0 || finalValue > 7);

            return (Direction)finalValue;
        }

        public static Direction LookUpDirectionVector(int2 lookupDirection)
        {
            int2 adjustedDirection = new int2(lookupDirection.x,  
                                            lookupDirection.y);  

            int i = 0;

            do
            {
                if (DirectionVectors[(Direction)i].x == adjustedDirection.x
                    && DirectionVectors[(Direction)i].y == adjustedDirection.y)
                {
                    return (Direction)i;
                }

                i++;
            }
            while (i < DirectionVectors.Count);

            Debug.Log("Failed direction vector lookup");

            return Direction.N;
        }

        //  Take int2 Vector representing a number of cells and reduce it to a maximum of 1 cell in each direction 
        public static int2 VectorToCellVector(int2 vector)
        {
            int2 adjustedMove = new int2();
            
            if (vector.x == 0)
            {
                adjustedMove.x = 0;
            }
            else
            {
                adjustedMove.x = vector.x / math.abs(vector.x);
            }

            if (vector.y == 0)
            {
                adjustedMove.y = 0;
            }
            else
            {
                adjustedMove.y = vector.y / math.abs(vector.y);
            }

            return adjustedMove;
        }
    }
}
