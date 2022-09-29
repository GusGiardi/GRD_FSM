using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorInputController : MonoBehaviour
{
    protected float _moveInput;

    protected bool _attackInput;
    protected bool _attackInputDown;
    protected bool _attackInputUp;

    protected bool _jumpInput;
    protected bool _jumpInputDown;
    protected bool _jumpInputUp;

    protected bool _defenseInput;
    protected bool _defenseInputDown;
    protected bool _defenseInputUp;

    protected bool _upInput;
    protected bool _downInput;

    public float moveInput => _moveInput;

    public bool attackInput => _attackInput;
    public bool attackInputDown => _attackInputDown;
    public bool attackInputUp => _attackInputUp;

    public bool jumpInput => _jumpInput;
    public bool jumpInputDown => _jumpInputDown;
    public bool jumpInputUp => _jumpInputUp;

    public bool defenseInput => _defenseInput;
    public bool defenseInputDown => _defenseInputDown;
    public bool defenseInputUp => _defenseInputUp;
    
    public bool upInput => _upInput;
    public bool downInput => _downInput;
}
