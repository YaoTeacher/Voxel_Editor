using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    public class GridDirection
    {
        public readonly Vector3Int Vector;

        private GridDirection(int x, int y,int z)
        {
            Vector = new Vector3Int(x, y,z);
        }

        public static implicit operator Vector3Int(GridDirection direction)
        {
            return direction.Vector;
        }

        public static GridDirection GetDirectionFromV3I(Vector3Int vector)
        {
            return CardinalAndIntercardinalDirections.DefaultIfEmpty(None).FirstOrDefault(direction => direction == vector);
        }

        //public static readonly GridDirection downNone = new GridDirection(0, -1, 0);
        public static readonly GridDirection downNorth = new GridDirection(0, -1, 1);
        public static readonly GridDirection downSouth = new GridDirection(0, -1, -1);
        public static readonly GridDirection downEast = new GridDirection(1, -1, 0);
        public static readonly GridDirection downWest = new GridDirection(-1, -1, 0);
        public static readonly GridDirection downNorthEast = new GridDirection(1, -1, 1);
        public static readonly GridDirection downNorthWest = new GridDirection(-1, -1, 1);
        public static readonly GridDirection downSouthEast = new GridDirection(1, -1, -1);
        public static readonly GridDirection downSouthWest = new GridDirection(-1, -1, -1);

        public static readonly GridDirection None = new GridDirection(0,0, 0);
        public static readonly GridDirection North = new GridDirection(0, 0, 1);
        public static readonly GridDirection South = new GridDirection(0, 0, -1);
        public static readonly GridDirection East = new GridDirection(1, 0, 0);
        public static readonly GridDirection West = new GridDirection(-1, 0, 0);
        public static readonly GridDirection NorthEast = new GridDirection(1, 0, 1);
        public static readonly GridDirection NorthWest = new GridDirection(-1, 0, 1);
        public static readonly GridDirection SouthEast = new GridDirection(1, 0, -1);
        public static readonly GridDirection SouthWest = new GridDirection(-1, 0, -1);

    //public static readonly GridDirection upNone = new GridDirection(0, 1, 0);
    public static readonly GridDirection upNorth = new GridDirection(0, 1, 1);
    public static readonly GridDirection upSouth = new GridDirection(0, 1, -1);
    public static readonly GridDirection upEast = new GridDirection(1, 1, 0);
    public static readonly GridDirection upWest = new GridDirection(-1, 1, 0);
    public static readonly GridDirection upNorthEast = new GridDirection(1, 1, 1);
    public static readonly GridDirection upNorthWest = new GridDirection(-1, 1, 1);
    public static readonly GridDirection upSouthEast = new GridDirection(1, 1, -1);
    public static readonly GridDirection upSouthWest = new GridDirection(-1, 1, -1);

    public static readonly List<GridDirection> CardinalDirections = new List<GridDirection>
        {
            North,
            East,
            South,
            West
        };

        public static readonly List<GridDirection> CardinalAndIntercardinalDirections = new List<GridDirection>
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };

        public static readonly List<GridDirection> AllDirections = new List<GridDirection>
        {
            None,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };
    }
