using UnityEngine;
using System.Collections;

namespace ZKit
{
    public abstract class AbstractTouchCamera : MonoBehaviour
    {
        public enum UpdateType // The available methods of updating are:
        {
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }

        [SerializeField] private UpdateType m_UpdateType;         // stores the selected update type

        protected Transform m_Cam; // the transform of the camera

        protected virtual void Awake()
        {
            // find the camera in the object hierarchy
            m_Cam = GetComponentInChildren<Camera>().transform;
        }

        protected abstract void TouchProcess(float deltaTime);

        private void FixedUpdate()
        {
            if (m_UpdateType == UpdateType.FixedUpdate)
            {
                TouchProcess(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (m_UpdateType == UpdateType.LateUpdate)
            {
                TouchProcess(Time.deltaTime);
            }
        }

        public void ManualUpdate()
        {
            if (m_UpdateType == UpdateType.ManualUpdate)
            {
                TouchProcess(Time.deltaTime);
            }
        }
    }
}