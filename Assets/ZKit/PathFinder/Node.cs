using UnityEngine;
using System.Collections;

namespace ZKit.PathFinder
{
    public class Node
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

        public float H = 0f;
        public float G = 0f;
        public float F = 0f;

        public Node Parent = null;

        public Node(int id, int x, int y, float height) { _id = id; _x = x; _y = y; _height = height; }
        public Node(int id, int x, int y, float height, bool canGo) { _id = id; _x = x; _y = y; _height = height; _canGo = canGo; }

        public void Initialize()
        {
            F = 0f;
            H = 0f;
            G = 0f;
            Parent = null;
            //            _canGo = false;
        }
    }
}