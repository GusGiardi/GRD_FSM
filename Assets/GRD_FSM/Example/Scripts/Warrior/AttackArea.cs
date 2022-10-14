using UnityEngine;

namespace GRD.FSM.Examples
{
    public class AttackArea : MonoBehaviour
    {
        private WarriorScript _currentOwner;
        private float _currentPower;
        private float _currentKnockback;
        private Vector2 _currentDirection;

        public WarriorScript currentOwner => _currentOwner;
        public float currentPower => _currentPower;
        public float currentKnockback => _currentKnockback;
        public Vector2 currentDirection => _currentDirection;

        public bool isActive => gameObject.activeInHierarchy;

        public void Activate(bool active, Vector2 direction, WarriorScript owner = null, float power = 0, float knockback = 0)
        {
            _currentOwner = owner;
            gameObject.SetActive(active);
            _currentPower = power;
            _currentDirection = direction.normalized;
            _currentKnockback = knockback;
        }

        public void Deactivate()
        {
            Activate(false, Vector2.zero);
        }
    }
}
