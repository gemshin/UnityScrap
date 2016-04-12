using UnityEngine;
using System.Collections;

namespace ZKit.Math.Geometry
{
    public class Box2D
    {
        public Vector3 position;
        public Vector2 size;
        public float rotate_y;

        public Vector2 Position2D { get { return new Vector2(position.x, position.z); } }

        public Box2D()
        {
            position = Vector3.zero;
            size = Vector2.one;
            rotate_y = 0f;
        }

        public Box2D(Vector3 Pos)
        {
            position = Pos;
            size = Vector2.one;
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

    public class Box3D
    {
        private Vector3 position;
        private Vector3 size;
        private Vector3 rotate;
        private Vector3[] vertices = new Vector3[8];

        public Vector3 Position
        {
            get { return position; }
            set { position = value; Recalculate(); }
        }

        public Vector3 Size
        {
            get { return size; }
            set { size = value; Recalculate(); }
        }

        public Vector3 Rotate
        {
            get { return rotate; }
            set { rotate = value; Recalculate(); }
        }

        private void Recalculate()
        {
            Vector3 halfSize = size * 0.5f;
            Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotate), Vector3.one);
            vertices[0] = mat.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y,  halfSize.z)) + position;
            vertices[1] = mat.MultiplyPoint3x4(new Vector3( halfSize.x, -halfSize.y,  halfSize.z)) + position;
            vertices[2] = mat.MultiplyPoint3x4(new Vector3(-halfSize.x,  halfSize.y,  halfSize.z)) + position;
            vertices[3] = mat.MultiplyPoint3x4(new Vector3( halfSize.x,  halfSize.y,  halfSize.z)) + position;
            vertices[4] = mat.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)) + position;
            vertices[5] = mat.MultiplyPoint3x4(new Vector3( halfSize.x, -halfSize.y, -halfSize.z)) + position;
            vertices[6] = mat.MultiplyPoint3x4(new Vector3(-halfSize.x,  halfSize.y, -halfSize.z)) + position;
            vertices[7] = mat.MultiplyPoint3x4(new Vector3( halfSize.x,  halfSize.y, -halfSize.z)) + position;
        }

        public Box3D()
        {
            position = Vector3.zero;
            size = Vector3.one;
            rotate = Vector3.zero;

            Recalculate();
        }

        public Box3D(Vector3 Pos)
        {
            position = Pos;
            size = Vector3.one;
            rotate = Vector3.zero;

            Recalculate();
        }

        public void DrawGizmo()
        {
            Quaternion rot = Quaternion.Euler(rotate);
            Gizmos.matrix = Matrix4x4.TRS(position, rot, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}