using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class Box
    {
        public Vector3 position;
        public Vector2 size;
        public Vector3 rotate;

        public Box()
        {
            position = Vector3.zero;
            size = Vector3.one;
            rotate = Vector3.zero;
        }

        public Box(Vector3 Pos)
        {
            position = Pos;
            size = Vector3.one;
            rotate = Vector3.zero;
        }

        public bool CollisionDetect2DDot(Vector2 dot)
        {
            Vector2 dotOrigin = dot - new Vector2(position.x, position.z);
            float boxRadius = Mathf.Sqrt((size.x * size.x) + (size.y * size.y)) * 0.5f;
            if (dotOrigin.magnitude <= boxRadius)
            {
                float rad = rotate.y * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                float x = (dotOrigin.x * cos) + (dotOrigin.y * -sin);
                float y = (dotOrigin.x * sin) + (dotOrigin.y * cos);

                dotOrigin = new Vector2(x, y);

                if ((size.x * 0.5f) >= x && (size.x * -0.5f) <= x && (size.y * 0.5f) >= y && (size.y * -0.5f) <= y)
                    return true;
            }
            return false;
        }

        public bool CollisionDetect2DLine(Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 boxSpaceStart = lineStart - new Vector2(position.x, position.z);
            Vector2 boxSpaceEnd = lineEnd - new Vector2(position.x, position.z);

            float rad = rotate.y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            Vector2 originStart = new Vector2();
            Vector2 originEnd = new Vector2();
            originStart.x = (boxSpaceStart.x * cos) + (boxSpaceStart.y * -sin);
            originStart.y = (boxSpaceStart.x * sin) + (boxSpaceStart.y * cos);
            originEnd.x = (boxSpaceEnd.x * cos) + (boxSpaceEnd.y * -sin);
            originEnd.y = (boxSpaceEnd.x * sin) + (boxSpaceEnd.y * cos);

            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;

            Vector2 tl = new Vector2(-halfWidth, halfHeight);
            Vector2 tr = new Vector2(halfWidth, halfHeight);
            Vector2 bl = new Vector2(-halfWidth, -halfHeight);
            Vector2 br = new Vector2(halfWidth, -halfHeight);

            Vector2 pResult;
            if (MathUtil.Intersects(originStart, originEnd, tl, bl, out pResult))
                return true;
            if (MathUtil.Intersects(originStart, originEnd, tl, tr, out pResult))
                return true;
            if (MathUtil.Intersects(originStart, originEnd, tr, br, out pResult))
                return true;
            if (MathUtil.Intersects(originStart, originEnd, bl, br, out pResult))
                return true;

            return false;
        }

        public bool CollisionDetect2DRay(Vector2 lineStart, Vector2 direction)
        {
            Vector2 boxSpaceStart = lineStart - new Vector2(position.x, position.z);
            Vector2 boxSpaceEnd = (lineStart + (direction * 1000.0f)) - new Vector2(position.x, position.z);

            float rad = rotate.y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            Vector2 originStart = new Vector2();
            Vector2 originEnd = new Vector2();
            originStart.x = (boxSpaceStart.x * cos) + (boxSpaceStart.y * -sin);
            originStart.y = (boxSpaceStart.x * sin) + (boxSpaceStart.y * cos);
            originEnd.x = (boxSpaceEnd.x * cos) + (boxSpaceEnd.y * -sin);
            originEnd.y = (boxSpaceEnd.x * sin) + (boxSpaceEnd.y * cos);

            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;

            Vector2 tl = new Vector2(-halfWidth, halfHeight);
            Vector2 tr = new Vector2(halfWidth, halfHeight);
            Vector2 bl = new Vector2(-halfWidth, -halfHeight);
            Vector2 br = new Vector2(halfWidth, -halfHeight);

            Vector2 pResult;
            if (MathUtil.Intersects(originStart, originEnd, tl, bl, out pResult))
                return true;
            if (MathUtil.Intersects(originStart, originEnd, tl, tr, out pResult))
                return true;
            if (MathUtil.Intersects(originStart, originEnd, tr, br, out pResult))
                return true;
            if (MathUtil.Intersects(originStart, originEnd, bl, br, out pResult))
                return true;

            return false;
        }

        public bool CollisionDetect2DCircle(Vector2 circle_position, float circle_radius)
        {
            Vector2 circleOrigin = circle_position - new Vector2(position.x, position.z);
            float boxRadius = Mathf.Sqrt((size.x * size.x) + (size.y * size.y)) * 0.5f;
            if (circleOrigin.magnitude - circle_radius <= boxRadius)
            {
                float rad = rotate.y * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                float x = (circleOrigin.x * cos) + (circleOrigin.y * -sin);
                float y = (circleOrigin.x * sin) + (circleOrigin.y * cos);

                circleOrigin = new Vector2(x, y);

                float halfWidth = size.x * 0.5f;
                float halfHeight = size.y * 0.5f;

                Vector2 tl = new Vector2(-halfWidth, halfHeight);
                Vector2 tr = new Vector2(halfWidth, halfHeight);
                Vector2 bl = new Vector2(-halfWidth, -halfHeight);
                Vector2 br = new Vector2(halfWidth, -halfHeight);

                if ((circleOrigin - tl).magnitude <= circle_radius) return true;
                if ((circleOrigin - tr).magnitude <= circle_radius) return true;
                if ((circleOrigin - bl).magnitude <= circle_radius) return true;
                if ((circleOrigin - br).magnitude <= circle_radius) return true;
            }
            return false;
        }

        public void DrawGizmo()
        {
            Quaternion rot = Quaternion.AngleAxis(rotate.y, Vector3.up);
            Gizmos.matrix = Matrix4x4.TRS(position, rot, new Vector3(size.x, 1f, size.y));

            float halfX = 0.5f;
            float halfZ = 0.5f;
            Gizmos.DrawLine(new Vector3(-halfX, 0f, halfZ), new Vector3(halfX, 0f, halfZ));
            Gizmos.DrawLine(new Vector3(-halfX, 0f, -halfZ), new Vector3(halfX, 0f, -halfZ));
            Gizmos.DrawLine(new Vector3(-halfX, 0f, halfZ), new Vector3(-halfX, 0f, -halfZ));
            Gizmos.DrawLine(new Vector3(halfX, 0f, halfZ), new Vector3(halfX, 0f, -halfZ));

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}