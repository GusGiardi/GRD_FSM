using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInputController : WarriorInputController
{
    [SerializeField] float _timeToPressAttack;
    [SerializeField] float _pressTime;
    private float _timeCounter;
    private bool _press;

    private void Update()
    {
        //_defenseInput = true;
        //_upInput = true;

        _attackInputDown = false;
        _attackInputUp = false;

        _timeCounter += Time.deltaTime;
        if (!_press)
        {
            if (_timeCounter >= _timeToPressAttack)
            {
                _press = true;
                _attackInputDown = true;
                _timeCounter = 0;
            }
        }
        else
        {
            if (_timeCounter >= _pressTime)
            {
                _press = false;
                _attackInputUp = true;
                _timeCounter = 0;
            }
        }

        _attackInput = _press;
    }
}
