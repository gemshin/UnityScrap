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
            return CollisionDetect2DBox(box.position, box.size, box.rotate.y);
        }

        public bool CollisionDetect2DBox(Vector2 box_position, Vector2 box_size, float box_rotateY)
        {
            Vector2 boxOrigin = box_position - new Vector2(position.x, position.z);
            float boxRadius = Mathf.Sqrt((box_size.x * box_size.x) + (box_size.y * box_size.y)) * 0.5f;
            if (boxOrigin.magnitude - boxRadius <= radius)
            {
                float rad = box_rotateY * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                float halfWidth = box_size.x * 0.5f;
                float halfHeight = box_size.y * 0.5f;

                Vector2 tl = new Vector2(-halfWidth, halfHeight);
                Vector2 tr = new Vector2(halfWidth, halfHeight);
                Vector2 bl = new Vector2(-halfWidth, -halfHeight);
                Vector2 br = new Vector2(halfWidth, -halfHeight);

                tl.x = (tl.x * cos) + (tl.y * -sin);
                tl.y = (tl.x * sin) + (tl.y * cos);

                tr.x = (tr.x * cos) + (tr.y * -sin);
                tr.y = (tr.x * sin) + (tr.y * cos);

                bl.x = (bl.x * cos) + (bl.y * -sin);
                bl.y = (bl.x * sin) + (bl.y * cos);

                br.x = (br.x * cos) + (br.y * -sin);
                br.y = (br.x * sin) + (br.y * cos);

                if ((boxOrigin + tl).magnitude <= radius) return true;
                if ((boxOrigin + tr).magnitude <= radius) return true;
                if ((boxOrigin + bl).magnitude <= radius) return true;
                if ((boxOrigin + br).magnitude <= radius) return true;
            }
            return false;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCapsule(position, height, radius);
        }
    }
}