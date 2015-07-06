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

        public Box Get2Dbox()
        {
            Box ret = new Box();
            ret.position = position;
            ret.size = new Vector2(size.x, size.z);
            ret.rotate_y = rotate.y;

            return ret;
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