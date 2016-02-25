using UnityEngine;
using System.Collections;

namespace ZKit
{
    public abstract class AbstractTargetFollower : MonoBehaviour
    {
        public enum UpdateType // The available methods of updating are:
        {
            FixedUpdate, // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate, // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate, // user must call to update camera
        }

        [SerializeField] private UpdateType m_UpdateType;         // stores the selected update type
        [SerializeField] protected Transform m_Target;            // The target object to follow

        protected Transform m_Cam; // the transform of the camera

        public Transform Target { get { return m_Target; } }
        public virtual void SetTarget(Transform newTransform) { m_Target = newTransform; }

        protected virtual void Awake()
        {
            // find the camera in the object hierarchy
            m_Cam = GetComponentInChildren<Camera>().transform;
        }

        // Use this for initialization
        void Start()
        {
            if (m_Target == null)
                FindAndTargetPlayer();
        }

        public void FindAndTargetPlayer()
        {
            // auto target an object tagged player, if no target has been assigned
            var targetObj = GameObject.FindGameObjectWithTag("Player");
            if (targetObj)
            {
                SetTarget(targetObj.transform);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (m_UpdateType == UpdateType.FixedUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        protected virtual void LateUpdate()
        {
            if (m_UpdateType == UpdateType.LateUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        public virtual void ManualUpdate()
        {
            if (m_UpdateType == UpdateType.ManualUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        protected abstract void FollowTarget(float deltaTime);
    }
}