using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [FSM_Behaviour("Game Manager/Game Setup")]
    public class GameManager_GameSetupState : FSM_StateBehaviour
    {
        private FSM_Manager _myFSM;
        private GameManager _gameManager;

        private float _timeCounter;

        public override void Setup(FSM_Manager manager)
        {
            _myFSM = manager;
            _gameManager = manager.GetComponent<GameManager>();
        }

        public override void OnEnter()
        {
            _gameManager.SetCamera();
            _gameManager.SetWarriors();

            _timeCounter = 0;
            _gameManager.readyMessage.SetActive(true);
        }

        public override void OnUpdate()
        {
            _timeCounter += Time.deltaTime;
            if (_timeCounter >= _gameManager.readyMessageTime)
            {
                _myFSM.SetTrigger("GameSetupDone");
            }
        }

        public override void OnExit()
        {
            _gameManager.EnableWarriorsControllers();
            _gameManager.EnableCameraUpdate();

            _gameManager.readyMessage.SetActive(false);
        }
    }
}
