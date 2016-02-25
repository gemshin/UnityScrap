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
        if( _clickObj == null)
        {
            _clickObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _clickObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //_clickObj.SetActive(false);
            BoxCollider bc = _clickObj.GetComponent<BoxCollider>();
            if( bc ) Destroy(bc);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("PATH")))
            {
                _clickObj.transform.position = hit.point;
                //GameObject clickObj = Instantiate(_clickObj, hit.point, Quaternion.identity) as GameObject;
                //clickObj.SetActive(true);
                //Destroy(clickObj, 0.5f);
                _playerController.MoveTo(hit.point);
            }
        }
	}
}
