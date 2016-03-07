using UnityEngine;

namespace ZKit.Math.Geometry
{
    public class Sector
    {
        public Vector3 position;
        public Vector3 up { get {return Vector3.up;} } // sector is 2D
        public float rotate_y;
        public float angle;
        public float radius;

        public Vector2 position2D { get { return new Vector2(position.x, position.z); } }
        
        public Vector3 forward { get {return new Vector3(-Mathf.Sin(-rotate_y * Mathf.Deg2Rad), 0f, Mathf.Cos(-rotate_y * Mathf.Deg2Rad));} }

        public Vector3 leftLine
        {
            get
            {
                float rad = (-rotate_y + angle * 0.5f) * Mathf.Deg2Rad;
                return new Vector3(-Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            }
        }
        public Vector3 rightLine
        {
            get
            {
                float rad = (-rotate_y - angle * 0.5f) * Mathf.Deg2Rad;
                return new Vector3(-Mathf.Sin(rad), 0f, Mathf.Cos(rad));
            }
        }

        public Sector()
        {
            position = Vector3.zero;
            rotate_y = 0f;
            angle = 90f;
            radius = 0.5f;
        }

        public Sector(Vector3 pos)
        {
            position = pos;
            rotate_y = 0f;
            angle = 90f;
            radius = 0.5f;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawSector(position, up, forward, radius, angle);
        }
    }
}