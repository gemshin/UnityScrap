using UnityEngine;
using System.Collections;

public class ZCameraBase : MonoBehaviour
{
    protected Transform m_Cam; // the transform of the camera

    protected virtual void Awake()
    {
        // find the camera in the object hierarchy
        m_Cam = GetComponentInChildren<Camera>().transform;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
