using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSM_Behaviour("Player/InAir")]
public class Player_InAir : FSM_StateBehaviour
{
    FSM_Manager _myFSM;
    WarriorScript _myWarrior;
    Animator _myAnimator;

    public override void Setup(FSM_Manager manager)
    {
        _myFSM = manager;
        _myWarrior = manager.GetComponent<WarriorScript>();
        _myAnimator = manager.GetComponent<Animator>();
    }

    public override void OnUpdate()
    {
        _myWarrior.Move(Input.GetAxis("Horizontal"));
        _myWarrior.DownThrust(Input.GetKey(KeyCode.DownArrow));

        //Attack Controls
        if (Input.GetKeyDown(KeyCode.C))
        {
            _myWarrior.DownThrust(false);
            _myFSM.SetBool("AttackCharge", true);
        }
        _myAnimator.SetBool("DownThrust", _myWarrior.downThrust);
    }

    public override void OnExit()
    {
        _myAnimator.SetBool("DownThrust", false);
    }
}
