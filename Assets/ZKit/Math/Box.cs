using UnityEngine;

namespace ZKit
{
    public class Box
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public Vector3 Rotate { get; set; }

        public Box()
        {
            Position = Vector3.zero;
            Size = Vector3.one;
            Rotate = Vector3.zero;
        }

        public Box(Vector3 Pos)
        {
            Position = Pos;
            Size = Vector3.one;
            Rotate = Vector3.zero;
        }

        public void DrawGizmo(bool wire = false)
        {
            if (Rotate != Vector3.zero)
            {
                Quaternion rot = Quaternion.AngleAxis(Rotate.y, Vector3.up);
                Gizmos.matrix = Matrix4x4.TRS(Position, rot, Vector3.one);
            }
            if( wire )
                Gizmos.DrawWireCube(Position, Size);
            else
                Gizmos.DrawCube(Position, Size);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}