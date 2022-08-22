using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/Damage")]
public class Player_DamageState : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;
    Animator _myAnimator;

    private float _damageTimeCounter;

    public override void Setup(FSM_Manager manager)
    {
        _myFSM = manager;
        _myWarrior = manager.GetComponent<WarriorScript>();
        _myAnimator = manager.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        _myAnimator.SetBool("Damage", true);
        _damageTimeCounter = _myWarrior.damageTime;
        _myWarrior.currentVelocity = 0;
    }

    public override void OnUpdate()
    {
        _damageTimeCounter -= Time.deltaTime;
        if (_damageTimeCounter <= 0)
        {
            _myFSM.SetBool("Damage", false);
            _myAnimator.SetBool("Damage", false);
        }
    }
}
