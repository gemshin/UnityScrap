using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;

namespace ZKit
{
    public class TouchController : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Camera m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;

        // Use this for initialization
        void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void FixedUpdate()
        {
            //if (CrossPlatformInputManager.GetButton("Fire1"))
            //{
            //    RaycastHit hit;
            //    if (Physics.Raycast(m_Cam.ScreenPointToRay(Input.mousePosition), out hit, 10000, (1 << 8)))
            //    {
            //        m_Character.Move((hit.point - m_Character.transform.position), false, false);
            //    }
            //}

            if (CrossPlatformInputManager.GetButton("Fire1"))
            {
                CharacterController cc = gameObject.GetComponent<CharacterController>();
                cc.Move(Vector3.back * 0.001f);
            }
        }
    }
}