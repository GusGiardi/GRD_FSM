using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM.Examples
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        Camera _myCamera;
        Transform _cameraTransform;
        [SerializeField] Transform _playerTransform;
        [SerializeField] Transform _enemyTransform;

        [Header("Movement")]
        [SerializeField] float _movementLerpFactor = 0.02f;
        [SerializeField] float _playerHorizontalPosition = 0.4f;
        [SerializeField] float _enemyRelativePositionFactor = 1f;
        [SerializeField] float _enemyRelativePositionLerp = 0.05f;
        [SerializeField] Vector2 _stageLimits;

        private bool _canUpdate = true;

        public bool canUpdate { get => _canUpdate; set => _canUpdate = value; }

        private void Awake()
        {
            _myCamera = GetComponent<Camera>();
            _cameraTransform = transform;
        }

        private void Update()
        {
            if (_canUpdate)
            {
                UpdateCameraPosition();
            }
            else
            {
                if (_enemyTransform.position.x < _playerTransform.position.x)
                {
                    _enemyRelativePositionFactor = -1;
                }
                else
                {
                    _enemyRelativePositionFactor = 1;
                }
            }
        }

        private void UpdateCameraPosition()
        {
            float cameraHalfWidth = Screen.width * _myCamera.orthographicSize / Screen.height;
            float minHorizontalPosition = _stageLimits.x + cameraHalfWidth;
            float maxHorizontalPosition = _stageLimits.y - cameraHalfWidth;

            float playerRelativePosition = Mathf.Lerp(-cameraHalfWidth, cameraHalfWidth, _playerHorizontalPosition);

            UpdateEnemyRelativePositionFactor();
            playerRelativePosition *= _enemyRelativePositionFactor;

            Vector3 desiredCameraPosition = new Vector3(
                Mathf.Clamp(_playerTransform.position.x - playerRelativePosition, minHorizontalPosition, maxHorizontalPosition),
                _cameraTransform.position.y,
                _cameraTransform.position.z);
            _cameraTransform.position = Vector3.Lerp(
                _cameraTransform.position,
                desiredCameraPosition,
                _movementLerpFactor);
        }

        private void UpdateEnemyRelativePositionFactor()
        {
            if (_enemyTransform.position.x < _playerTransform.position.x)
            {
                _enemyRelativePositionFactor = Mathf.Lerp(_enemyRelativePositionFactor, -1, _enemyRelativePositionLerp);
            }
            else
            {
                _enemyRelativePositionFactor = Mathf.Lerp(_enemyRelativePositionFactor, 1, _enemyRelativePositionLerp);
            }
        }
    }
}
