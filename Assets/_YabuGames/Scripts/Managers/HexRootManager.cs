using System;
using System.Collections;
using System.Collections.Generic;
using _YabuGames.Scripts.Controllers;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Managers
{
    public class HexRootManager : MonoBehaviour
    {
        public int killHints;
        public bool onKillJump;

        [SerializeField] private int[] killTarget;
        [SerializeField] private int[] moveTarget;
        private int _maxEnemyCountAtATime = 6;
        public int _currentEnemyCount;
        private int _enemyMoveCount;
        private int _totalEnemyCount;
        private int _killCount;
        private int _moveCount;
        private PathCalculator _pathCalculator;
        private int _level;
        private bool _initialized;
        private List<EnemyController> _enemyList = new List<EnemyController>();
        private readonly List<HexController> _hexagons = new List<HexController>();

        private void Awake()
        {
            GetValues();
            _pathCalculator = GameObject.Find("Player").GetComponent<PathCalculator>();
            for (var i = 0; i < transform.childCount; i++)
            {
                var hex = transform.GetChild(i).GetComponent<HexController>();
                _hexagons.Add(hex);
            }
        }

        private void Start()
        {
            Spawn();
            UIManager.Instance.SetKillText(_killCount,killTarget[_level]);
        }

        public void Spawn()
        {
            StartCoroutine(SpawnEnemy());
        }

        public void CountMoveCount(bool died =false)
        {
            if (died)
            {
                _currentEnemyCount--;
                _killCount++;
                UIManager.Instance.SetKillText(_killCount,killTarget[_level]);
                if (_killCount >= killTarget[_level])
                {
                    CoreGameSignals.Instance.OnLevelWin?.Invoke();
                    _level++;
                    Save();
                }
                return;
            }
            
            _enemyMoveCount++;
            if (_enemyMoveCount == _currentEnemyCount)
            {
                StartCoroutine(_pathCalculator.Start());
                _enemyMoveCount = 0;
            }
                
        }

        private void GetValues()
        {
            _level = PlayerPrefs.GetInt("level", 0);
            _moveCount = moveTarget[_level];
        }
        private void Save()
        {
            PlayerPrefs.SetInt("level",_level);
        }

        private IEnumerator SpawnEnemy()
        {
            if (_initialized)
            {
                _moveCount--;
                UIManager.Instance.SetMoveText(_moveCount);
                if (_moveCount <= 0)
                {
                    CoreGameSignals.Instance.OnLevelFail?.Invoke();
                    yield break;
                }
            }

            while ( _currentEnemyCount< _maxEnemyCountAtATime)
            {
                var randomHex = Random.Range(0, _hexagons.Count);
                var hex = _hexagons[randomHex];
                if (!hex.IsOccupied())
                {
                    if (_enemyList.Count>0)
                    {
                        for (var i = 0; i < _enemyList.Count; i++)
                        {
                            _enemyList[i].SetPriority(i+1,this);
                        }
                    }
                    var enemy = Instantiate(Resources.Load<GameObject>("Spawnables/Enemy_2")).transform;
                    enemy.localScale = Vector3.zero;
                    enemy.position = hex.transform.position + Vector3.up * 1.3f;
                    enemy.DOMove(hex.transform.position + Vector3.up * .3f, .3f)
                        .OnComplete(() => SpawnParticle(enemy));
                    enemy.DOScale(Vector3.one * 30, .3f).SetEase(Ease.OutBack);
                    _enemyList.Add(enemy.GetComponent<EnemyController>());
                    _currentEnemyCount++;
                    enemy.GetComponent<EnemyController>().SetPriority(_currentEnemyCount,this);
                    _totalEnemyCount++;
                }

                yield return null;
            }

            if (_initialized)
            {
                yield return new WaitForSeconds(.5f);
                LevelSignals.Instance.OnEnemyTurn?.Invoke();
            }

            _initialized = true;
        }
        private void SpawnParticle(Transform enemy)
        {
            var particle = Instantiate(Resources.Load<GameObject>("Spawnables/SpawnFX")).transform;
            particle.position = enemy.position;
            particle.gameObject.SetActive(true);
        }
    }
   
}