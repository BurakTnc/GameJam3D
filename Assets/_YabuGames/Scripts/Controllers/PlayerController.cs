using System;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveDuration = .3f;

        private CollisionController _collisionController;
        private PathCalculator _pathCalculator;
        private Camera _camera;
        private HexController _currentHex;

        private void Awake()
        {
            _pathCalculator = GetComponent<PathCalculator>();
            _collisionController = GetComponent<CollisionController>();
            _camera=Camera.main;
        }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            var worldMousePos =
                _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
            var direction = worldMousePos - _camera.transform.position;

            if(!Input.GetMouseButtonDown(0))
                return;
            if (Physics.Raycast(_camera.transform.position, direction, out var hit, 1000)) 
            {
                if (hit.collider.CompareTag("Hex"))
                {
                    var hex = hit.collider.GetComponent<HexController>();
                    
                    if(!hex.IsSelectable())
                        return;
                    var hexPosition = hit.collider.transform.position;
                    var fixedPosition = new Vector3(hexPosition.x, 0, hexPosition.z);
                    Move(fixedPosition);
                }
            }
        }
        private void Move(Vector3 desiredPos)
        {
            Leave();
            _collisionController.onMove = true;
            _collisionController.hasSelected = false;
            //transform.DOMove(desiredPos, moveDuration).SetEase(Ease.InBack).OnComplete(KillEnemies);
            transform.DOJump(desiredPos, 1
                , 2, moveDuration).SetEase(Ease.InOutSine).OnComplete(KillEnemies);
        }

        private void KillEnemies()
        {
            _collisionController.onMove = false;
            var enemies =  _collisionController.GetKilledEnemies();
            _pathCalculator.ResetHints();
            foreach (var enemy in enemies)
            {
                enemy.Leave();
                enemy.Die();
            }
            _collisionController.ClearEnemyList();
            //_pathCalculator.CalculateGeneral();
            StartCoroutine(_pathCalculator.Start());
        }

        public void Occupy(HexController hex)
        {
            _currentHex = hex;
            hex.Occupy(true);
        }

        public void Leave()
        {
            _currentHex.Occupy(false);
            _currentHex.SelectionHint(false);
            _currentHex = null;
        }
    }
}