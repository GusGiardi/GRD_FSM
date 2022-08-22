using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private WarriorScript _currentOwner;
    private float _currentPower;
    private Vector2 _currentDirection;

    public WarriorScript currentOwner => _currentOwner;
    public float currentPower => _currentPower;
    public Vector2 currentDirection => _currentDirection;

    public void Activate(bool active, Vector2 direction, WarriorScript owner = null, float power = 0)
    {
        _currentOwner = owner;
        gameObject.SetActive(active);
        _currentPower = power;
        _currentDirection = direction;
    }

    public void Deactivate()
    {
        Activate(false, Vector2.zero);
    }
}
