using UnityEngine;
using System.Collections;

namespace ZKit.Math.Geometry
{
    public class Capsule
    {
        public Vector3 position;
        public float height;
        public float radius;

        public Capsule()
        {
            position = Vector3.zero;
            height = 2f;
            radius = 0.5f;
        }

        public Capsule(Vector3 pos)
        {
            position = pos;
            height = 2f;
            radius = 0.5f;
        }

        public Circle GetCircle()
        {
            Circle ret = new Circle();
            ret.position = position;
            ret.radius = radius;

            return ret;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCapsule(position, height, radius);
        }
    }
}