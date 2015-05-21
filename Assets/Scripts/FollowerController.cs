using UnityEngine;
using System.Collections;

public class FollowerController : MonoBehaviour
{
    public float _speed = 6f;

    Animator _anim;
    Rigidbody _rig;
    CharacterController _cc;
    int _pathMask;
    float _camRayLength = 100f;

    Vector3 _moveTarget;
    Vector3 _dirTarget;

    bool _aniFlgMove = false;

    void Awake()
    {
        _pathMask = LayerMask.GetMask("PATH");
        _anim = GetComponent<Animator>();
        _rig = GetComponent<Rigidbody>();
        _cc = GetComponent<CharacterController>();
        _moveTarget = transform.position;
        _dirTarget = Vector3.forward;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit pathHit;

            if (Physics.Raycast(camRay, out pathHit, _camRayLength, _pathMask))
            {
                MoveTo(pathHit.point);
            }
        }

        Vector3 m = _moveTarget - transform.position; m.y = 0f;
        if (m.magnitude > 0.2f)
        {
            m.Normalize();
            Move(m);
            Turning(m);
            _aniFlgMove = true;
        }
        else
            _aniFlgMove = false;

        Animating();
    }

    void Move(Vector3 direction)
    {
        _cc.SimpleMove(direction * _speed);
    }

    void Turning(Vector3 direction)
    {
        _rig.MoveRotation(Quaternion.LookRotation(direction));
    }

    void Animating()
    {
        _anim.SetBool("IsWalking", _aniFlgMove);
    }

    public void MoveTo(Vector3 targetPos)
    {
        _moveTarget = targetPos;
    }
}
