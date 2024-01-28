using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
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
        public List<Vector3> _wayPoints = new List<Vector3>();
        private Vector3 _initialPosition;
        private Vector3 _ghostPosition;
        private bool _sequenceBegan;
        private LineRenderer _lineRenderer;
        private HexController _previousHex;
        public int _stepID;
        public int _jumpCount;
        public List<HexController> _prevList = new List<HexController>();
        private SphereCollider _collider;
        private bool _onRewind;
        private HexRootManager _hexRoot;

        private void Awake()
        {
            _hexRoot = GameObject.Find("HexRoot").GetComponent<HexRootManager>();
            _collider = GetComponent<SphereCollider>();
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
            LevelSignals.Instance.OnRewind += Rewind;
        }

        private void OnDisable()
        {
            LevelSignals.Instance.OnGo -= BeginMoveSequence;
            LevelSignals.Instance.OnRewind -= Rewind;
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
                    if (hex.isJumpHint) 
                        _pathCalculator.onJumpKill = true;
                    MoveSequence(fixedPosition,hex.isJumpHint);
                }
            }
        }
        private void MoveSequence(Vector3 desiredPos,bool onJump = true, bool onRewind = false)
        {
            if (!_sequenceBegan)
            {
                _ghostPosition = transform.position;
                _ghost.transform.position = _ghostPosition;
            }

            _jumpCount = 0;
            _onRewind = onRewind;
            _collisionController.isGhost = false;
            if (onJump)
            {
                _stepID++;
                _collisionController.isGhost = true;
            }
            _ghost.SetActive(onJump);
            Leave();
            _initialPosition = transform.position;
            _collisionController.onMove = true;
            _collisionController.hasSelected = false;
            
            transform.DOMove(desiredPos, moveDuration-.4f).SetEase(Ease.Linear).OnComplete(()=>PausePath(onJump,onRewind));
            if (!onRewind)
            {
                _wayPoints.Add(desiredPos);
                _sequenceBegan = true;
            }
        }

        private void BeginMoveSequence()
        {
            _collisionController.isGhost = false;
            _stepID = 0;
            _lineRenderer.positionCount = 1;
            _ghost.SetActive(false);
            transform.position = _ghostPosition;
            _collisionController.ClearEnemyList();
            Leave();
            var moveSeq = DOTween.Sequence();
            foreach (var pos in _wayPoints)
            {
                moveSeq.Append(transform.DOJump(pos, 1
                    , 2, moveDuration).SetEase(Ease.InOutSine).OnComplete(() => KillEnemies())).SetDelay(.15f);
            }
        }

        private void KillEnemies(bool onJump =false)
        {
            if (onJump)
            {
                _jumpCount++;
            }

            _collisionController.onMove = false;
            _collisionController.hasSelected = false;
            _wayPoints.Clear();
            _sequenceBegan = false;
            var enemies =  _collisionController.GetKilledEnemies();
            _pathCalculator.ResetHints();
            foreach (var enemy in enemies)
            {
                enemy.Die();
            }
            _collisionController.ClearEnemyList();
            _pathCalculator.onJumpKill = false;
            
            if(_jumpCount< _stepID) return;
            StartCoroutine(SpawnDelay());
        }

        private IEnumerator SpawnDelay()
        {
            yield return new WaitForSeconds(0);
            _hexRoot.Spawn();
        }
        private void PausePath(bool onJump, bool onRewind =false)
        {
            if (!onJump && !onRewind)
            {
                KillEnemies(onJump);
                return;
            }

            _collider.enabled = true;
            _collisionController.onMove = false;
            _pathCalculator.ResetHints();
            StartCoroutine(_pathCalculator.Start());
            _lineRenderer.positionCount = _wayPoints.Count + 1;
            _lineRenderer.SetPosition(0,_ghostPosition);
            for (var i = 0; i < _wayPoints.Count; i++)
            {
                _lineRenderer.SetPosition(i+1,_wayPoints[i]);
            }
        }

        private void Rewind()
        {
            _collisionController.ClearEnemyList();
            _collider.enabled = false;
            _stepID -= 2;
            if (_wayPoints.Count>0)
            {
                _wayPoints.RemoveAt(_wayPoints.Count - 1);
            }
            
            var desiredPos = _prevList[_stepID + 1].transform.position + Vector3.up * .3f;
            MoveSequence(desiredPos,false,true);
            if (_onRewind)
            {
                _currentHex.ActAsGoButton(false);
                _currentHex.ByPass(false);
                _currentHex = _prevList[_stepID];
                if (_stepID > 0)
                {
                    _currentHex.ActAsGoButton(true);
                }
                
            }
            if (_stepID>0)
            {
                _prevList[_stepID-1].ActAsRewindButton(true);
            }
            _prevList.RemoveAt(_prevList.Count-1);
            if (_stepID == 0)
                _pathCalculator.onJumpKill = false;

        }
        public void Occupy(HexController hex)
        {
            if(_onRewind) return;
            
            if(_previousHex)
                _previousHex.ByPass(true);
            _currentHex = hex;
            hex.Occupy(true);
            if (!_sequenceBegan) return;
            if (_currentHex) 
                _currentHex.ActAsGoButton(true);
            if(_previousHex)
                _previousHex.ActAsRewindButton(true);
            
        }

        public void Leave()
        {
            if(_onRewind) return;
            if(!_currentHex)
                return;
            if (_previousHex)
            {
                _previousHex.ByPass(false);
                _previousHex.ActAsRewindButton(false);
            }
                 
            _previousHex = _currentHex;
            _prevList.Add(_previousHex);
            _currentHex.Occupy(false);
            _currentHex.SelectionHint(false);
            _currentHex = null;
        }
    }
}