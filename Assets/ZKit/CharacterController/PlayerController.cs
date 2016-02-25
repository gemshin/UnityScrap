using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    const float k_half = 0.5f;

    [SerializeField]
    float _movingTurnSpeed = 360;
    [SerializeField]
    float _stationaryTurnSpeed = 180;
    [SerializeField]
    float _moveSpeedMultiplier = 6f;
    [SerializeField]
    float _animSpeedMultiplier = 1f;
    [SerializeField]
    string[] _pathLayerName;
    [SerializeField]
    float _groundCheckDistance = 0.1f;
    //[SerializeField]
    //float _runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others

    Animator _animator;
    Rigidbody _rigidbody;
    CapsuleCollider _capsule;

    float _capsuleHeight;
    Vector3 _capsuleCenter;

    Vector3 _moveTarget;
    int _pathMask = 0;

    float _turnAmount;
    float _forwardAmount;

    bool _isGrounded;
    Vector3 _groundNormal;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
        _capsuleHeight = _capsule.height;
        _capsuleCenter = _capsule.center;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        _moveTarget = transform.position;
        if (_pathLayerName.Length > 0)
            _pathMask = LayerMask.GetMask(_pathLayerName);
    }

    void FixedUpdate()
    {
        Vector3 direction = _moveTarget - transform.position;
        if (direction.magnitude > 0.2f)
        {
            MoveDirection(direction.normalized);
        }
        else
        {
            _turnAmount = 0f;
            _forwardAmount = 0f;
            UpdateAnimator(Vector3.zero);
        }
    }

    void OnAnimatorMove()
    {
        if (_isGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (_animator.deltaPosition * _moveSpeedMultiplier) / Time.deltaTime;

            v.y = _rigidbody.velocity.y;
            _rigidbody.velocity = v;
        }
    }

    void UpdateAnimator(Vector3 direction)
    {
        // update the animator parameters
        _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
        _animator.SetBool("OnGround", _isGrounded);

        if (_isGrounded && direction.magnitude > 0)
        {
            _animator.speed = _animSpeedMultiplier;
        }
        else
        {
            _animator.speed = 1;
        }
    }

    void MoveDirection(Vector3 direction)
    {
        if (direction.magnitude > 1f) direction.Normalize();
        direction = transform.InverseTransformDirection(direction);
        CheckGroundStatus();
        direction = Vector3.ProjectOnPlane(direction, _groundNormal);
        _turnAmount = Mathf.Atan2(direction.x, direction.z);
        _forwardAmount = direction.z;

        ApplyExtraTurnRotation();
        UpdateAnimator(direction);
    }

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(_stationaryTurnSpeed, _movingTurnSpeed, _forwardAmount);
        transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * _groundCheckDistance), Color.red);
#endif
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, _groundCheckDistance))
        {
            _isGrounded = true;
            _groundNormal = hitInfo.normal;
            _animator.applyRootMotion = true;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
            _animator.applyRootMotion = false;
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _moveTarget = targetPosition;
    }
}
