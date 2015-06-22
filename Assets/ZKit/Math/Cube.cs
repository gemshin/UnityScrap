using UnityEngine;

namespace ZKit
{
    public class Cube
    {
        public Vector3 position;
        public Vector3 size;
        public Vector3 rotate;

        public Cube()
        {
            position = Vector3.zero;
            size = Vector3.one;
            rotate = Vector3.zero;
        }

        public Cube(Vector3 pos)
        {
            position = pos;
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

        public void DrawGizmo()
        {
            Quaternion rot = Quaternion.AngleAxis(rotate.y, Vector3.up);
            Gizmos.matrix = Matrix4x4.TRS(position, rot, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, size);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}