﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace ZKit
{
    public class TopDownFollowCamera : AbstractTargetFollower
    {
        private float m_zoom = 0.1f;

        protected override void Awake()
        {
            base.Awake();
            m_Cam.LookAt(transform.position + Vector3.down);
        }

        protected override void FollowTarget(float deltaTime)
        {
            float z = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel");

            if (z != 0f) m_zoom -= z * 10f * deltaTime;

            transform.position = Vector3.Lerp( transform.position + (Vector3.up * m_zoom), Target.position, deltaTime*10f);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}