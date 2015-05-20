using UnityEngine;
using System.Collections;

namespace ZKit.PathFinder
{
    public enum CellType
    {
        CantMove = 0,
        Normal
    }

    public class CellData
    {
        private int _x = 0;
        private int _y = 0;
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public Point Index { get { return new Point(_x, _y); } }
        private float _height = 0f;
        public float Height { get { return _height; } set { _height = value; } }
        private CellType _type = CellType.Normal;
        public CellType Type { get { return _type; } set { _type = value; } }

        public CellData(Point index) { _x = index.x; _y = index.y; }
        public CellData(int x, int y) { _x = x; _y = y; }
    }
}