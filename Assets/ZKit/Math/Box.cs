﻿using UnityEngine;
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

        public bool Check2DDot(Vector2 dot)
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

        public bool Check2DLine(Vector2 start, Vector2 end)
        {
            Vector2 lineOriginStart = start - new Vector2(position.x, position.z);
            Vector2 lineOriginEnd = end - new Vector2(position.x, position.z);

            float rad = rotate.y * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;

            Vector2 originStart = new Vector2();
            Vector2 originEnd = new Vector2();
            originStart.x = (lineOriginStart.x * cos) + (lineOriginStart.y * -sin);
            originStart.y = (lineOriginStart.x * sin) + (lineOriginStart.y * cos);
            originEnd.x = (lineOriginEnd.x * cos) + (lineOriginEnd.y * -sin);
            originEnd.y = (lineOriginEnd.x * sin) + (lineOriginEnd.y * cos);

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