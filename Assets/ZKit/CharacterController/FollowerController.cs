using UnityEngine;
using System.Collections;

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

        //Vector3 _moveTarget;
        Vector3 _dirTarget;

        bool _aniFlgMove = false;

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

        void FixedUpdate()
        {
            Vector3 m = _target.position - transform.position; m.y = 0f;
            if (m.magnitude > 2.0f)
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
    }
}