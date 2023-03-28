using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Enemy - Retreat Behaviour", menuName = "FSM Behaviours/Enemy/Retreat", order = 1)]
    public class EnemyAI_Retreat : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private EnemyAIController _myController;

        [SerializeField] float _maxJumpTime = 3f;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _myController = manager.GetComponent<EnemyAIController>();
        }

        public override void OnUpdate()
        {
            if (_myController.player.invincibilityTimeCounter <= 0)
            {
                _myFSM.SetBool("Retreat", false);
                return;
            }

            if (_myController.isOutOfStageBounds != 0)
            {
                if (!_myController.isFacingPlayer)
                {
                    _myController.FacePlayer();
                    return;
                }

                _myController.StopMoving();

                if (_myController.inPlayerAttackRange && (_myController.playerCharginAttack || _myController.playerAttacking))
                {
                    _myController.Defend();
                    return;
                }

                if (!_myController.player.onGround)
                {
                    _myController.UpDefense();
                    return;
                }

                _myController.StopDefending();
                return;
            }

            _myController.ReleaseAttack();
            _myController.StopDefending();

            _myController.MoveAwayFromPlayer();

            if (_myController.inPlayerAttackRange &&
                (_myController.playerCharginAttack || _myController.playerAttacking) &&
                _myController.myWarrior.position.y - _myController.player.position.y < 2)
            {
                _myController.Jump();

                _myController.Invoke("CancelJump", _maxJumpTime);
                return;
            }
        }
    }
}
