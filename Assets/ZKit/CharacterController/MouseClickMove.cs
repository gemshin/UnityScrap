using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MouseClickMove : MonoBehaviour
{
    [SerializeField] GameObject _clickObj;

    Camera _cam;
    PlayerController _playerController;

	// Use this for initialization
	void Start ()
    {
        _cam = Camera.main;
        _playerController = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("PATH")))
            {
                if(_clickObj != null)
                    _clickObj.transform.position = hit.point;
                _playerController.MoveTo(hit.point);
            }
        }
	}
}
