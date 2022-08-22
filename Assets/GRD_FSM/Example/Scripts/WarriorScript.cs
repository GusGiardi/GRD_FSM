using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorScript : MonoBehaviour
{
    Transform _trans;
    Rigidbody2D _rb;
    FSM_Manager _myFSM;
    public Transform trans => _trans;
    public Rigidbody2D rb => _rb;

    [Header("Basic Movement")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _moveAcceleration;
    [SerializeField] float _airAcceleration;
    private float _currentVelocity = 0;
    [SerializeField] private bool _facingRight = true;

    [Header("Jump")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpAntecipationTime;
    [SerializeField] float _jumpRecoveryTime;
    [SerializeField] float _cancelJumpForce;

    [Header("Ground Detection")]
    [SerializeField] Vector2 _groundDetectionRayOrigin;
    [SerializeField] float _groundDetectionRayDistance;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] private bool _onGround = false;
    Ray2D _groundDetectionRay = new Ray2D();

    [Header("Attack")]
    [SerializeField] Transform _attackAreasParent;
    [SerializeField] AttackArea _myAttackArea;
    [SerializeField] float _attackPower;
    [SerializeField] float _maxChargeTime;
    [SerializeField] float _chargePowerMultiplier;
    [SerializeField] float _currentCharge;
    [SerializeField] float _attackCooldownTime;
    [SerializeField] bool _downThrust;

    [Header("Defense")]
    [SerializeField] bool _defending;
    [SerializeField] bool _upDefense;
    [SerializeField] float _defenseVelocityMultiplier;
    [SerializeField] float _maxDotProdForSuccessfulDefense;

    [Header("Damage")]
    [SerializeField] float _damageTime;
    [SerializeField] float _invincibilityTime;
    [SerializeField] LayerMask _damageLayer;

    [Header("Rendering")]
    [SerializeField] SpriteRenderer _spriteRenderer;

    public float moveSpeed => _moveSpeed;
    public float moveAcceleration => _moveAcceleration;
    public float currentVelocity { get => _currentVelocity; set => _currentVelocity = value; }

    public float jumpForce => _jumpForce;
    public float jumpAntecipationTime => _jumpAntecipationTime;
    public float jumpRecoveryTime => _jumpRecoveryTime;
    public float cancelJumpForce => _cancelJumpForce;

    public Vector2 groundDetectionRayOrigin => _groundDetectionRayOrigin;
    public float groundDetectionRayDistance => _groundDetectionRayDistance;
    public LayerMask groundLayer => _groundLayer;
    public bool onGround { get => _onGround; set => _onGround = value; }

    public float attackCooldownTime => _attackCooldownTime;
    public bool downThrust => _downThrust;

    public float damageTime => _damageTime;

    private void Awake()
    {
        _trans = transform;
        _rb = GetComponent<Rigidbody2D>();
        _myFSM = GetComponent<FSM_Manager>();
    }

    private void Update()
    {
        DetectGround();
        FlipToFacingDirection();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_currentVelocity, _rb.velocity.y);
    }

    #region Basic Movement
    public void Move(float direction)
    {
        float maxMoveSpeed = _moveSpeed * (_defending ? _defenseVelocityMultiplier : 1);
        float desiredVelocity = maxMoveSpeed * direction;
        float acceleration = _onGround ? _moveAcceleration : _airAcceleration;

        if (_currentVelocity > desiredVelocity)
        {
            _currentVelocity = Mathf.Max(
                _currentVelocity - acceleration * Time.deltaTime,
                desiredVelocity);
        }
        else if (_currentVelocity < desiredVelocity)
        {
            _currentVelocity = Mathf.Min(
                _currentVelocity + acceleration * Time.deltaTime,
                desiredVelocity);
        }

        if (!_defending && direction != 0)
        {
            _facingRight = direction > 0;
        }
    }

    public void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }

    private void DetectGround()
    {
        _groundDetectionRay.origin = _trans.TransformPoint(_groundDetectionRayOrigin);
        _groundDetectionRay.direction = Vector2.down;
        _onGround = Physics2D.Raycast(_groundDetectionRay.origin, _groundDetectionRay.direction, _groundDetectionRayDistance, _groundLayer);
        _myFSM.SetBool("OnGround", _onGround);

        if (_onGround)
        {
            DownThrust(false);
        }
    }
    #endregion

    #region Attack
    public void ChargeAttack()
    {
        _currentCharge = Mathf.Clamp(_currentCharge + Time.deltaTime / _maxChargeTime, 0, 1);
    }

    public void CancelAttackCharge()
    {
        _currentCharge = 0;
    }

    private void Attack()
    {
        _myAttackArea.Activate(
            true,
            Vector2.right * (_facingRight ? 1 : -1),
            this,
            _attackPower * Mathf.Lerp(1, _chargePowerMultiplier, _currentCharge));
        CancelAttackCharge();
    }

    public void EndAttack()
    {
        _myAttackArea.Deactivate();
    }

    public void DownThrust(bool active)
    {
        _downThrust = active;
    }
    #endregion

    #region Defense
    public void Defense(bool active)
    {
        _defending = active;
    }

    public void UpDefense(bool active)
    {
        _upDefense = active;
    }
    #endregion

    #region Health and Damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_damageLayer & (1 << collision.gameObject.layer)) != 0)
        {
            AttackArea attackArea = collision.GetComponent<AttackArea>();
            if (attackArea != null && attackArea.currentOwner != this)
            {
                ReceiveAttack(attackArea);
            }
        }
    }

    private void ReceiveAttack(AttackArea attackArea)
    {
        if (_defending)
        {
            Vector2 defenseDirection;
            if (_upDefense)
            {
                defenseDirection = Vector2.up;
            }
            else
            {
                defenseDirection = Vector2.right * (_facingRight ? 1 : -1);
            }

            if (Vector2.Dot(attackArea.currentDirection, defenseDirection) < _maxDotProdForSuccessfulDefense)
            {
                //Successful Defense
                return;
            }
        }

        TakeDamage(attackArea.currentPower);
    }

    private void TakeDamage(float damage)
    {
        CancelAttackCharge();
    }
    #endregion

    #region Rendering
    private void FlipToFacingDirection()
    {
        _spriteRenderer.flipX = !_facingRight;
        _attackAreasParent.localScale = new Vector3(_facingRight ? 1 : -1, 1, 1);
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.TransformPoint(_groundDetectionRayOrigin), Vector3.down * _groundDetectionRayDistance);
    }
#endif
}
