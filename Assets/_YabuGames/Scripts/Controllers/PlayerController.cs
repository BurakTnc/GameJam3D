using System;
using System.Collections.Generic;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveDuration = .3f;

        private GameObject _ghost;
        private CollisionController _collisionController;
        private PathCalculator _pathCalculator;
        private Camera _camera;
        public HexController _currentHex;
        private List<Vector3> _wayPoints = new List<Vector3>();
        private Vector3 _initialPosition;
        private Vector3 _ghostPosition;
        private bool _sequenceBegan;
        private LineRenderer _lineRenderer;
        private HexController _previousHex;

        private void Awake()
        {
            _ghost = transform.GetChild(0).gameObject;
            _pathCalculator = GetComponent<PathCalculator>();
            _collisionController = GetComponent<CollisionController>();
            _camera=Camera.main;
            _initialPosition = transform.position;
            _ghost.transform.SetParent(null);
            _lineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            LevelSignals.Instance.OnGo += BeginMoveSequence;
        }

        private void OnDisable()
        {
            LevelSignals.Instance.OnGo -= BeginMoveSequence;
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
                    MoveSequence(fixedPosition);
                }
            }
        }
        private void MoveSequence(Vector3 desiredPos)
        {
            if (!_sequenceBegan)
            {
                _ghostPosition = transform.position;
                _ghost.transform.position = _ghostPosition;
            }
                
            _ghost.SetActive(true);
            Leave();
            _initialPosition = transform.position;
            _collisionController.onMove = true;
            _collisionController.hasSelected = false;
            transform.DOMove(desiredPos, moveDuration-.4f).SetEase(Ease.Linear).OnComplete(PausePath);
           _wayPoints.Add(desiredPos);
           _sequenceBegan = true;
           
           if (_currentHex) 
               _currentHex.ActAsGoButton(true);
        }

        private void BeginMoveSequence()
        {
            _lineRenderer.positionCount = 1;
            _ghost.SetActive(false);
            transform.position = _ghostPosition;
            _collisionController.ClearEnemyList();
            Leave();
            var moveSeq = DOTween.Sequence();
            foreach (var pos in _wayPoints)
            {
                moveSeq.Append(transform.DOJump(pos, 1
                    , 2, moveDuration).SetEase(Ease.InOutSine).OnComplete(KillEnemies)).SetDelay(.15f);
            }
            
        }

        private void KillEnemies()
        {
            _collisionController.onMove = false;
            _wayPoints.Clear();
            _sequenceBegan = false;
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

        private void PausePath()
        {
            _collisionController.onMove = false;
            _pathCalculator.ResetHints();
            StartCoroutine(_pathCalculator.Start());
            _lineRenderer.positionCount = _wayPoints.Count + 1;
            _lineRenderer.SetPosition(0,_ghostPosition);
            for (var i = 0; i < _wayPoints.Count; i++)
            {
                _lineRenderer.SetPosition(i+1,_wayPoints[i]);
            }

            // for (var i = 0; i < _pathIndex-1; i++)
            // {
            //     if(i == 0)
            //         _lineRenderer.SetPosition(0,transform.position);
            //     else
            //         _lineRenderer.SetPosition(i,_wayPoints[i]);
            // }
        }

        public void Occupy(HexController hex)
        {
            if(_previousHex)
                _previousHex.ByPass(true);
            _currentHex = hex;
            hex.Occupy(true);
            if (!_sequenceBegan) return;
            if (_currentHex) 
                _currentHex.ActAsGoButton(true);
        }

        public void Leave()
        {
            if(!_currentHex)
                return;
            _currentHex.ByPass(false);
            _previousHex = _currentHex;
            _currentHex.Occupy(false);
            _currentHex.SelectionHint(false);
            _currentHex = null;
        }
    }
}