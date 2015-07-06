using UnityEngine;
using System.Collections;

namespace ZKit
{
    public class Box
    {
        public Vector3 position;
        public Vector2 size;
        public float rotate_y;

        public Vector2 Position2D { get { return new Vector2(position.x, position.z); } }

        public Box()
        {
            position = Vector3.zero;
            size = Vector3.one;
            rotate_y = 0f;
        }

        public Box(Vector3 Pos)
        {
            position = Pos;
            size = Vector3.one;
            rotate_y = 0f;
        }

        public void DrawGizmo()
        {
            Quaternion rot = Quaternion.AngleAxis(rotate_y, Vector3.up);
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