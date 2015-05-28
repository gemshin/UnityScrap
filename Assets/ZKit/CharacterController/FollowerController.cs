using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZKit.PathFinder;

namespace ZKit
{
    public class FollowerController : MonoBehaviour
    {
        public float _speed = 6f;

        Animator _anim;
        Rigidbody _rig;
        CharacterController _cc;
        int _pathMask;
        float _camRayLength = 100f;

        Transform _target;

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
            _target = GameObject.FindWithTag("Player").transform;
            //_moveTarget = transform.position;
            _dirTarget = Vector3.forward;
        }

        void OnDrawGizmos()
        {
            if (_path != null && _path.Count > 1)
            {
                Gizmos.DrawLine(transform.position, _path[0]);
                for (int i = 0; i < _path.Count - 1; ++i)
                {
                    Gizmos.DrawLine(_path[i], _path[i + 1]);
                }
            }
        }

        float _gapPFTime = 1f;
        float _accumPFTime = 1f;
        void FixedUpdate()
        {
            if (_gapPFTime <= _accumPFTime)
            {
                _accumPFTime = 0f;
                _path = JPS.Instance.Find(transform.position, _target.position);
                if (_path.Count > 0)
                {
                    _moveTarget = _path[_pathIndex = 0];
                }
            }

            _accumPFTime += Time.deltaTime;

            Vector3 m = _moveTarget - transform.position; m.y = 0f;
            if (m.magnitude > 2.0f)
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
    }
}