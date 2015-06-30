using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class Capsule
    {
        public Vector3 position;
        public float height;
        public float radius;

        public Capsule()
        {
            position = Vector3.zero;
            height = 2f;
            radius = 0.5f;
        }

        public Capsule(Vector3 pos)
        {
            position = pos;
            height = 2f;
            radius = 0.5f;
        }

        public bool CollisionDetect2DDot(Vector2 dot)
        {
            Vector2 dotOrigin = dot - new Vector2(position.x, position.z);
            if (dotOrigin.magnitude <= radius)
                return true;
            return false;
        }

        public bool CollisionDetect2DLine(Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 originStart = lineStart - new Vector2(position.x, position.z);
            Vector2 originEnd = lineEnd - new Vector2(position.x, position.z);

            if (originStart.magnitude <= radius) return true;
            if (originEnd.magnitude <= radius) return true;

            if (Vector2.Dot(originStart, originEnd - originStart) >= 0f) return false;
            if (Vector2.Dot(originEnd, originEnd - originStart) <= 0f) return false;

            float dr = (originEnd - originStart).magnitude;
            float D = originStart.x * originEnd.y - originEnd.x * originStart.y;
            float di = (radius * radius) * (dr * dr) - (D * D);

            if (di < 0) return false;
            return true;
        }

        public bool CollisionDetect2DRay(Vector2 lineStart, Vector2 direction)
        {
            Vector2 originStart = lineStart - new Vector2(position.x, position.z);
            Vector2 originEnd = (lineStart + direction.normalized) - new Vector2(position.x, position.z);

            if (originStart.magnitude <= radius) return true;
            if (Vector2.Dot(new Vector2(position.x, position.z) - originStart, originStart - originEnd) >= 0f) return false;
            float D = originStart.x * originEnd.y - originEnd.x * originStart.y;
            float di = (radius * radius) - (D * D);

            if (di < 0) return false;
            return true;
        }

        public bool CollisionDetect2DBox(Box box)
        {
            Circle circle = new Circle();
            circle.position = position;
            circle.radius = radius;
            circle.up = Vector3.up;
            return box.CollisionDetect2DCircle(circle);
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCapsule(position, height, radius);
        }
    }
}