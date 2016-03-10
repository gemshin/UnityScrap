using UnityEngine;
using System.Collections;

namespace ZKit.PathFinder
{
    /// <summary>
    /// 길찾기에 사용되는 2D 노드.
    /// </summary>
    public class Node2D
    {
        protected int _id;
        public int ID { get { return _id; } }

        protected int _x, _y;
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public Point Index { get { return new Point(_x, _y); } }

        protected float _height;
        public float Height { get { return _height; } }

        protected bool _canGo = false;
        public bool CanGo { get { return _canGo; } set { _canGo = value; } }

        public float H = 0f;    // Heuristics 
        public float G = 0f;    // 이동비용
        public float F = 0f;    // 지수.

        public Node2D Parent = null;

        public Node2D(int id, int x, int y, float height) { _id = id; _x = x; _y = y; _height = height; }
        public Node2D(int id, int x, int y, float height, bool canGo) { _id = id; _x = x; _y = y; _height = height; _canGo = canGo; }

        public void Initialize()
        {
            F = 0f;
            H = 0f;
            G = 0f;
            Parent = null;
            // _canGo = false;
        }
    }
}