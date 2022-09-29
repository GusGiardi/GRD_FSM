using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    public class WarriorScript : MonoBehaviour
    {
        Transform _trans;
        Rigidbody2D _rb;
        FSM_Manager _myFSM;
        Animator _myAnimator;
        public Transform trans => _trans;
        public Rigidbody2D rb => _rb;
        public Animator myAnimator => _myAnimator;

        [SerializeField] WarriorInputController _controller;

        [Header("Basic Movement")]
        [SerializeField] float _moveSpeed;
        [SerializeField] float _moveAcceleration;
        [SerializeField] float _airAcceleration;
        private float _currentVelocity = 0;
        private float _lastMoveInput = 0;
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

        [Header("Enemy Head Detection")]
        [SerializeField] Vector2 _enemyHeadDetectionBoxSize;
        [SerializeField] LayerMask _otherPlayerLayer;
        [SerializeField] float _enemyHeadJumpForce;

        [Header("Attack")]
        [SerializeField] Transform _attackAreasParent;
        [SerializeField] AttackArea _myAttackArea;
        [SerializeField] AttackArea _downThrustAttackArea;
        [SerializeField] float _attackPower;
        [SerializeField] float _attackKnockback;
        [SerializeField] float _maxChargeTime;
        [SerializeField] float _chargePowerMultiplier;
        [SerializeField] float _currentCharge;
        [SerializeField] float _attackCooldownTime;
        [SerializeField] bool _downThrust;

        [Header("Defense")]
        [SerializeField] bool _defending;
        [SerializeField] bool _upDefense;
        [SerializeField] float _defenseVelocityMultiplier;
        [SerializeField] float _defenseKnockbackMultiplier;
        [SerializeField] float _maxDotProdForSuccessfulDefense;

        [Header("Damage")]
        [SerializeField] float _damageTime;
        private bool _takingDamage = false;
        [SerializeField] float _invincibilityTime;
        private float _invincibilityTimeCounter = 0;
        [SerializeField] LayerMask _damageLayer;
        [SerializeField] float _damageJumpForce;
        [SerializeField] Color _invincibilityColor;
        private float _currentKnockback = 0;

        [Header("Rendering")]
        [SerializeField] SpriteRenderer _spriteRenderer;

        public WarriorInputController controller => _controller;

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
        public bool takingDamage { get => _takingDamage; set => _takingDamage = value; }
        public float invincibilityTime => _invincibilityTime;
        public float invincibilityTimeCounter { get => _invincibilityTimeCounter; set => _invincibilityTimeCounter = value; }

        private void Awake()
        {
            _trans = transform;
            _rb = GetComponent<Rigidbody2D>();
            _myFSM = GetComponent<FSM_Manager>();
            _myAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            DetectGround();
            DetectEnemyHead();
            DecreaseKnockback();
            DecreaseInvincibility();

            FlipToFacingDirection();
            DrawInvincibility();
            UpdateAnimatorParameters();
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_currentVelocity + _currentKnockback, _rb.velocity.y);
        }

        #region Basic Movement
        public void Move(float direction)
        {
            _lastMoveInput = direction;

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

        private void DetectEnemyHead()
        {
            if (_onGround || _rb.velocity.y > 0)
                return;

            _groundDetectionRay.origin = _trans.TransformPoint(_groundDetectionRayOrigin);
            _groundDetectionRay.direction = Vector2.down;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(_groundDetectionRay.origin, _enemyHeadDetectionBoxSize, 0, _groundDetectionRay.direction, _groundDetectionRayDistance, _otherPlayerLayer);
            if(hits.Length > 1)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, 
                    Mathf.Min(-_rb.velocity.y + _enemyHeadJumpForce, _jumpForce));
            }
        }

        private void DecreaseKnockback()
        {
            float acceleration = _onGround ? _moveAcceleration : _airAcceleration;

            if (_currentKnockback > 0)
            {
                _currentKnockback = Mathf.Max(
                    _currentKnockback - acceleration * Time.deltaTime,
                    0);
            }
            else if (_currentKnockback < 0)
            {
                _currentKnockback = Mathf.Min(
                    _currentKnockback + acceleration * Time.deltaTime,
                    0);
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
                _attackPower * Mathf.Lerp(1, _chargePowerMultiplier, _currentCharge),
                _attackKnockback * Mathf.Lerp(1, _chargePowerMultiplier, _currentCharge) * (_facingRight ? 1 : -1));
            CancelAttackCharge();
        }

        public void EndAttack()
        {
            _myAttackArea.Deactivate();
        }

        public void DownThrust(bool active)
        {
            _downThrust = active;

            if (active)
            {
                _downThrustAttackArea.Activate(
                    true,
                    Vector2.down,
                    this,
                    _attackPower,
                    _attackKnockback * (_facingRight ? 1 : -1));
            }
            else
            {
                _downThrustAttackArea.Deactivate();
            }
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

        private void SuccessfulDefense(float damage, float knockback)
        {
            _currentKnockback = knockback * _defenseKnockbackMultiplier;
        }
        #endregion

        #region Health and Damage
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!_takingDamage && _invincibilityTimeCounter <= 0)
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
                    SuccessfulDefense(attackArea.currentPower, attackArea.currentKnockback);
                    return;
                }
            }

            TakeDamage(attackArea.currentPower, attackArea.currentKnockback);
        }

        private void TakeDamage(float damage, float knockback)
        {
            _currentKnockback = knockback;

            CancelAttackCharge();
            _myFSM.SetBool("Damage", true);
            _rb.velocity = new Vector2(_rb.velocity.x, _damageJumpForce);
        }

        private void DecreaseInvincibility()
        {
            if (_invincibilityTimeCounter > 0)
                _invincibilityTimeCounter -= Time.deltaTime;
        }
        #endregion

        #region Rendering
        private void FlipToFacingDirection()
        {
            _spriteRenderer.flipX = !_facingRight;
            _attackAreasParent.localScale = new Vector3(_facingRight ? 1 : -1, 1, 1);
        }

        private void DrawInvincibility()
        {
            if (_invincibilityTimeCounter > 0)
            {
                _spriteRenderer.color = _invincibilityColor;
            }
            else
            {
                _spriteRenderer.color = Color.white;
            }
        }

        private void UpdateAnimatorParameters()
        {
            _myAnimator.SetBool("OnGround", _onGround);
            _myAnimator.SetFloat("MoveSpeed", Mathf.Abs(_lastMoveInput));
            _myAnimator.SetFloat("VerticalVelocity", _rb.velocity.y);

            if (!_defending)
            {
                _myAnimator.SetFloat("Defense", 0);
            }
            else
            {
                if (!_upDefense)
                {
                    _myAnimator.SetFloat("Defense", 1);
                }
                else
                {
                    _myAnimator.SetFloat("Defense", 2);
                }
            }
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
}
