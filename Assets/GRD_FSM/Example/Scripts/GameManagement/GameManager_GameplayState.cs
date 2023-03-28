using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [CreateAssetMenu(fileName = "Game Manager - Gameplay Behaviour", menuName = "FSM Behaviours/Game Manager/Gameplay Behaviour", order = 1)]
    public class GameManager_GameplayState : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private GameManager _gameManager;

        private float _fightMessageTimeCounter;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _gameManager = manager.GetComponent<GameManager>();
        }

        public override void OnEnter()
        {
            _gameManager.fightMessage.SetActive(true);
            _fightMessageTimeCounter = 0;
        }

        public override void OnUpdate()
        {
            if (_fightMessageTimeCounter < _gameManager.fightMessageTime)
            {
                _fightMessageTimeCounter += Time.deltaTime;
                if (_fightMessageTimeCounter >= _gameManager.fightMessageTime)
                {
                    _gameManager.fightMessage.SetActive(false);
                }
            }

            if (_gameManager.player1Warrior.currentHP <= 0 ||
                _gameManager.player2Warrior.currentHP <= 0)
            {
                _myFSM.SetTrigger("GameEnd");
            }
        }

        public override void OnExit()
        {
            _gameManager.DisableWarriorsControllers();
        }
    }
}
