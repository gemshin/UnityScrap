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

            {
                float rad = rotate.y * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                float sx = (lineOriginStart.x * cos) + (lineOriginStart.y * -sin);
                float sy = (lineOriginStart.x * sin) + (lineOriginStart.y * cos);
                float ex = (lineOriginEnd.x * cos) + (lineOriginEnd.y * -sin);
                float ey = (lineOriginEnd.x * sin) + (lineOriginEnd.y * cos);
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