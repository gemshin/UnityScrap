using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class Box
    {
        public Vector3 position;
        public Vector2 size;
        public Vector3 rotate;

        public Vector2 Position2D { get { return new Vector2(position.x, position.z); } }

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
            Vector2 dotOrigin = dot - Position2D;
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
            Vector2 boxSpaceStart = lineStart - Position2D;
            Vector2 boxSpaceEnd = lineEnd - Position2D;

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
            Vector2 boxSpaceStart = lineStart - Position2D;
            Vector2 boxSpaceEnd = (lineStart + (direction * 1000.0f)) - Position2D;

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

        public bool CollisionDetect2DBox(Box box)
        {
            //float rad = box.rotate.y * Mathf.Deg2Rad;
            //float cos = Mathf.Cos(rad);
            //float sin = Mathf.Sin(rad);

            //float halfWidth = box.size.x * 0.5f;
            //float halfHeight = box.size.y * 0.5f;

            //Vector2 box_tl = new Vector2((-halfWidth * cos) + (halfHeight * -sin), (-halfWidth * sin) + (halfHeight * cos)) + box.Position2D;
            //Vector2 box_tr = new Vector2((halfWidth * cos) + (halfHeight * -sin), (halfWidth * sin) + (halfHeight * cos)) + box.Position2D;
            //Vector2 box_bl = new Vector2((-halfWidth * cos) + (-halfHeight * -sin), (-halfWidth * sin) + (-halfHeight * cos)) + box.Position2D;
            //Vector2 box_br = new Vector2((halfWidth * cos) + (-halfHeight * -sin), (halfWidth * sin) + (-halfHeight * cos)) + box.Position2D;

            //if (CollisionDetect2DDot(box_tl)) return true;
            //if (CollisionDetect2DDot(box_tr)) return true;
            //if (CollisionDetect2DDot(box_bl)) return true;
            //if (CollisionDetect2DDot(box_br)) return true;

            //rad = rotate.y * Mathf.Deg2Rad;
            //cos = Mathf.Cos(rad);
            //sin = Mathf.Sin(rad);

            //halfWidth = size.x * 0.5f;
            //halfHeight = size.y * 0.5f;

            //box_tl = new Vector2((-halfWidth * cos) + (halfHeight * -sin), (-halfWidth * sin) + (halfHeight * cos)) + box.Position2D;
            //box_tr = new Vector2((halfWidth * cos) + (halfHeight * -sin), (halfWidth * sin) + (halfHeight * cos)) + box.Position2D;
            //box_bl = new Vector2((-halfWidth * cos) + (-halfHeight * -sin), (-halfWidth * sin) + (-halfHeight * cos)) + box.Position2D;
            //box_br = new Vector2((halfWidth * cos) + (-halfHeight * -sin), (halfWidth * sin) + (-halfHeight * cos)) + box.Position2D;

            //if (box.CollisionDetect2DDot(box_tl)) return true;
            //if (box.CollisionDetect2DDot(box_tr)) return true;
            //if (box.CollisionDetect2DDot(box_bl)) return true;
            //if (box.CollisionDetect2DDot(box_br)) return true;

            return false;
        }

        public bool CollisionDetect2DCircle(Circle circle)
        {
            Vector2 circleOrigin = circle.Position2D - Position2D;

            float rad = rotate.y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float circle_distance_x = Mathf.Abs((circleOrigin.x * cos) + (circleOrigin.y * -sin));
            float circle_distance_y = Mathf.Abs((circleOrigin.x * sin) + (circleOrigin.y * cos));

            if (circle_distance_x > (size.x * 0.5f) + circle.radius) return false;
            if (circle_distance_y > (size.y * 0.5f) + circle.radius) return false;

            if (circle_distance_x <= (size.x * 0.5f)) return true;
            if (circle_distance_y <= (size.y * 0.5f)) return true;

            float corner_distance_sq = (circle_distance_x - (size.x * 0.5f)) * (circle_distance_x - (size.x * 0.5f))
                + (circle_distance_y - (size.y * 0.5f)) * (circle_distance_y - (size.y * 0.5f));

            return corner_distance_sq <= (circle.radius * circle.radius);
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