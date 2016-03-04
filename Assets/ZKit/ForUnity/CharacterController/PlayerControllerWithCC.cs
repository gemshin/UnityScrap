using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZKit.PathFinder;

namespace ZKit
{
    public class PlayerControllerWithCC : MonoBehaviour
    {
        public float _speed = 6f;

        Animator _anim;
        Rigidbody _rig;
        CharacterController _cc;
        int _pathMask;
        float _camRayLength = 10000f;

        Vector3 _moveTarget;
        Vector3 _dirTarget;

        bool _aniFlgMove = false;

        List<Vector3> _path;
        int _pathIndex = 0;

        void Awake()
        {
            _pathMask = LayerMask.GetMask("PATH");
            _anim = GetComponent<Animator>();
            _rig = GetComponent<Rigidbody>();
            _cc = GetComponent<CharacterController>();
            _moveTarget = transform.position;
            _dirTarget = Vector3.forward;
        }

        void OnDrawGizmos()
        {
            if ( _path != null && _path.Count > 1)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _path[0]);
                for (int i = 0; i < _path.Count - 1; ++i)
                {
                    Gizmos.DrawLine(_path[i], _path[i + 1]);
                }
            }
        }

        void FixedUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit pathHit;

                if (Physics.Raycast(camRay, out pathHit, _camRayLength, _pathMask))
                {
                    _path = JPS.Instance.Find(transform.position, pathHit.point);
                    if (_path.Count > 0)
                    {
                        //MoveTo(_path[0]);
                        _moveTarget = _path[_pathIndex = 0];
                    }
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
            {
                if (_path != null && _pathIndex + 1 < _path.Count)
                {
                    _moveTarget = _path[++_pathIndex];
                }
                else
                    _aniFlgMove = false;
            }

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
}