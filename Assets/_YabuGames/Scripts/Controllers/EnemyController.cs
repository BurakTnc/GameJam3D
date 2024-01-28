using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class EnemyController : MonoBehaviour
    {
        private int _turnPriority;
        private EnemyCollisionController _enemyCollision;
        private HexController _currentHex;
        private HexRootManager _parent;
        private bool _isDead;
        private List<EnemyController> _enemyList = new List<EnemyController>();
       [SerializeField] private LayerMask hexLayer;

       private void Awake()
       {
           _enemyCollision = GetComponent<EnemyCollisionController>();
       }

       private void OnEnable()
       {
           LevelSignals.Instance.OnEnemyTurn += PlayTurn;
       }

       private void OnDisable()
       {
           LevelSignals.Instance.OnEnemyTurn -= PlayTurn;
       }

       public void SetPriority(int id,HexRootManager parent)
       {
           _turnPriority = id;
           _parent = parent;
       }
       public void FireRay(Vector3 direction, float distance, PathCalculator pathCalculator)
        {
            var fixedPos = new Vector3(transform.position.x+(distance)*(direction.x) , 1, transform.position.z+(distance* direction.z));
            Debug.DrawRay(fixedPos+Vector3.up*.5f,new Vector3(0,-1,0),Color.blue,100);

            if (Physics.Raycast(transform.position, direction, out var hitCheck, 1000))
            {
                if (hitCheck.collider.CompareTag("Enemy"))
                {
                    return;
                }
            }
            if (Physics.Raycast(fixedPos,new Vector3(0,-1,0),out var hit,1000))
            {
                if (hit.collider.CompareTag("Hex"))
                {
                   var hex = hit.collider.GetComponent<HexController>();
                   pathCalculator.AddKillHint(hex);
                   hex.JumpHint();
                }
            }
        }

        private Vector3 GetDirection(int id)
        {
            var direction = Vector3.zero;
            switch (id)
            {
                case 1:
                    direction = new Vector3(1f, 0, 0);
                    break;
                case 2:
                    direction = new Vector3(-1f, 0, 0);
                    break;
                case 3:
                    direction = new Vector3(.5f, 0, 1);
                    break;
                case 4:
                    direction = new Vector3(-.5f, 0, 1);
                    break;
                case 5:
                    direction = new Vector3(.5f, 0, -1);
                    break;
                case 6:
                    direction = new Vector3(-.5f, 0, -1);
                    break;
                default:
                    break;
            }

            return direction;
        }

        private void PlayTurn()
        {
            if(_isDead) return;
            StartCoroutine(SelectPath());
        }
        private IEnumerator SelectPath()
        {
            var isSpawned = false;
            var origin = new Vector3(transform.position.x, 0, transform.position.z);
            yield return new WaitForSeconds(_turnPriority * .16f);
            Debug.Log(_turnPriority);
            while (!isSpawned)
            {
                
                var direction = GetDirection(Random.Range(1,7));
                if (Physics.Raycast(origin-Vector3.up*.2f,direction,out var hit,hexLayer))
                {
                    if (hit.collider.CompareTag("Hex"))
                    {
                        if (hit.collider.TryGetComponent(out HexController hexController))
                        {
                            if (!hexController.IsOccupied())
                            {
                                Move();
                                transform.DOMove(hexController.transform.position + Vector3.up * .3f, .15f)
                                    .SetEase(Ease.OutSine)
                                    .OnComplete(CountMove);
                                isSpawned = true;
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        private void CountMove()
        {
            _parent.CountMoveCount();
            _enemyCollision.onMove = false;
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

        private void Move()
        {
            Leave();
            _enemyCollision.onMove = true;
            _enemyCollision.hasSelected = false;
        }

        public void Die()
        {
            _isDead = true;
            Leave();
            _parent.CountMoveCount(true);
            transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InSine).OnComplete(() => Destroy(gameObject));
        }
    }
}