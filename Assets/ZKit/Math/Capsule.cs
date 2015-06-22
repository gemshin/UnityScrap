﻿using UnityEngine;
using System.Collections;

namespace ZKit
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

        public bool Check2DDot(Vector2 dot)
        {
            Vector2 dotOrigin = dot - new Vector2(position.x, position.z);
            if (dotOrigin.magnitude <= radius)
                return true;
            return false;
        }

        public void DrawGizmo()
        {
            GizmosExtension.DrawCapsule(position, height, radius);
        }
    }
}