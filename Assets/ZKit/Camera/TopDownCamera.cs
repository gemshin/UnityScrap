using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace ZKit
{
    public class TopDownCamera : AbstractTouchCamera
    {
        private float m_zoom = 10f;
        private Transform m_transform;

        //public Transform transform { get { return m_transform; } }

        protected virtual void Awake()
        {
            base.Awake();
            m_Cam.LookAt(transform.position + Vector3.down);
        }

        protected override void TouchProcess(float deltaTime)
        {
            float h = 0f;
            float v = 0f;
            float z = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel");

            if (CrossPlatformInputManager.GetButton("Fire1"))
            {
                h = CrossPlatformInputManager.GetAxis("Mouse X");
                v = CrossPlatformInputManager.GetAxis("Mouse Y");
            }

            if (CrossPlatformInputManager.GetButton("Fire1") || z != 0f)
            {
                if( z != 0f )
                    m_zoom -= z * 300f * deltaTime;

                Vector3 newPos = new Vector3(transform.position.x - h, m_zoom, transform.position.z - v);
                transform.position = newPos;
            }
        }
    }
}