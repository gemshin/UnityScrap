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
            return box.CollisionDetect2DDot(new Vector2(position.x, position.z));
            //return CollisionDetect2DBox(new Vector2(box.position.x, box.position.z), box.size, box.rotate.y);
        }

        public bool CollisionDetect2DBox(Vector2 box_position, Vector2 box_size, float box_rotateY)
        {
            Vector2 boxOrigin = box_position - new Vector2(position.x, position.z);
            float boxRadius = Mathf.Sqrt((box_size.x * box_size.x) + (box_size.y * box_size.y)) * 0.5f;
            if (boxOrigin.magnitude - boxRadius <= radius)
            {
                float halfWidth = box_size.x * 0.5f;
                float halfHeight = box_size.y * 0.5f;

                Vector2 tl = new Vector2(-halfWidth, halfHeight);
                Vector2 tr = new Vector2(halfWidth, halfHeight);
                Vector2 bl = new Vector2(-halfWidth, -halfHeight);
                Vector2 br = new Vector2(halfWidth, -halfHeight);

                float rad = - box_rotateY * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                Vector2 ntl = new Vector2();
                Vector2 ntr = new Vector2();
                Vector2 nbl = new Vector2();
                Vector2 nbr = new Vector2();

                ntl.x = (tl.x * cos) + (tl.y * -sin);
                ntl.y = (tl.x * sin) + (tl.y * cos);

                ntr.x = (tr.x * cos) + (tr.y * -sin);
                ntr.y = (tr.x * sin) + (tr.y * cos);

                nbl.x = (bl.x * cos) + (bl.y * -sin);
                nbl.y = (bl.x * sin) + (bl.y * cos);

                nbr.x = (br.x * cos) + (br.y * -sin);
                nbr.y = (br.x * sin) + (br.y * cos);

                if ((boxOrigin + ntl).magnitude <= radius) return true;
                if ((boxOrigin + ntr).magnitude <= radius) return true;
                if ((boxOrigin + nbl).magnitude <= radius) return true;
                if ((boxOrigin + nbr).magnitude <= radius) return true;
            }
            return false;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCircle(position, up, radius);
        }
    }
}