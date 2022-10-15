using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] FSM_Manager _gameManagerFSM;

        [SerializeField] WarriorScript _player1Warrior;
        [SerializeField] WarriorScript _player2Warrior;
        private Vector3 _player1StartPosition;
        private Vector3 _player2StartPosition;

        [SerializeField] Transform _cameraTransform;
        [SerializeField] PlayerCamera _playerCamera;
        private  Vector3 _cameraStartPosition;

        [Header("Main Menu")]
        [SerializeField] GameObject _mainMenu;

        [Header("Gameplay screen messages")]
        [SerializeField] GameObject _readyMessage;
        [SerializeField] GameObject _fightMessage;
        [SerializeField] GameObject _redWinsMessage;
        [SerializeField] GameObject _blueWinsMessage;
        [SerializeField] GameObject _drawMessage;
        [SerializeField] float _readyMessageTime;
        [SerializeField] float _fightMessageTime;
        [SerializeField] float _timeToShowResults;
        [SerializeField] float _winnerMessageTime;

        public WarriorScript player1Warrior => _player1Warrior;
        public WarriorScript player2Warrior => _player2Warrior;

        public GameObject readyMessage => _readyMessage;
        public GameObject fightMessage => _fightMessage;
        public GameObject redWinsMessage => _redWinsMessage;
        public GameObject blueWinsMessage => _blueWinsMessage;
        public GameObject drawMessage => _drawMessage;
        public float readyMessageTime => _readyMessageTime;
        public float fightMessageTime => _fightMessageTime;
        public float timeToShowResults => _timeToShowResults;
        public float winnerMessageTime => _winnerMessageTime;

        private void Awake()
        {
            _player1StartPosition = _player1Warrior.position;
            _player2StartPosition = _player2Warrior.position;
            _cameraStartPosition = _cameraTransform.position;
        }

        public void ActiveMainMenu(bool active)
        {
            _mainMenu.SetActive(active);
        }

        public void DisableWarriors()
        {
            _player1Warrior.gameObject.SetActive(false);
            _player2Warrior.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            _gameManagerFSM.SetBool("PlayGame", true);
        }

        public void SetWarriors()
        {
            _player1Warrior.trans.position = _player1StartPosition;
            _player2Warrior.trans.position = _player2StartPosition;

            _player1Warrior.facingRight = true;
            _player2Warrior.facingRight = false;

            _player1Warrior.ResetWarrior();
            _player2Warrior.ResetWarrior();

            _player1Warrior.gameObject.SetActive(true);
            _player2Warrior.gameObject.SetActive(true);

            DisableWarriorsControllers();
        }

        public void SetCamera()
        {
            _playerCamera.canUpdate = false;
            _cameraTransform.position = _cameraStartPosition;
        }

        public void EnableWarriorsControllers()
        {
            _player1Warrior.EnableWarrior();
            _player2Warrior.EnableWarrior();
        }

        public void DisableWarriorsControllers()
        {
            _player1Warrior.DisableWarrior();
            _player2Warrior.DisableWarrior();
        }

        public void EnableCameraUpdate()
        {
            _playerCamera.canUpdate = true;
        }
    }
}
