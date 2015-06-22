using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class Circle
    {
        public Vector3 position;
        public float radius;
        public Vector3 up;

        public Circle()
        {
            position = Vector3.zero;
            radius = 0.5f;
            up = Vector3.up;
        }

        public Circle(Vector3 pos)
        {
            position = pos;
            radius = 0.5f;
            up = Vector3.up;
        }

        public bool Check2DDot(Vector2 dot)
        {
            Vector2 dotOrigin = dot - new Vector2(position.x, position.z);
            if (dotOrigin.magnitude <= radius)
                return true;
            return false;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCircle(position, up, radius);
        }
    }
}