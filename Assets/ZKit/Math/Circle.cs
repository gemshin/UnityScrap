using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class Circle
    {
        public Vector3 position;
        public float radius;

        public Vector2 position2D { get { return new Vector2(position.x, position.z); } }

        public Circle()
        {
            position = Vector3.zero;
            radius = 0.5f;
        }

        public Circle(Vector3 pos)
        {
            position = pos;
            radius = 0.5f;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCircle(position, Vector3.up, radius);
        }
    }
}