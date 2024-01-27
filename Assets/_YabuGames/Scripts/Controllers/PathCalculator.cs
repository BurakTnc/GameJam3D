using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class PathCalculator : MonoBehaviour
    {
        [SerializeField] private float neighborRange;
        [SerializeField] private LayerMask hexLayer, enemyLayer;

        private Collider[] _scannedColliders = new Collider[7];
        private List<HexController> _killHints = new List<HexController>();
        private int _stepCount;

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color=Color.red;
        //     Gizmos.DrawWireSphere(transform.position,neighborRange);
        // }

        public IEnumerator Start()
        {
            yield return new WaitForSeconds(.25f);
            CheckNeighbors();
            yield return new WaitForSeconds(.10f);
            CheckEnemies();
        }

        public void CalculateGeneral()
        {
            CheckNeighbors();
            CheckEnemies();
        }

        private void CheckEnemies()
        {
            for (var i = 1; i < 7; i++)
            {
                var direction = GetDirection(i);
                FireRay(direction);
            }
        }
        private void FireRay(Vector3 direction)
        {
            var origin = new Vector3(transform.position.x, 0, transform.position.z); ;
            Debug.DrawRay(origin+Vector3.up*.5f,direction,Color.red,100);
            if (Physics.Raycast(origin,direction,out var hit,1000,enemyLayer))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (hit.collider.TryGetComponent(out EnemyController enemyController))
                    {
                        var distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                        enemyController.FireRay(direction,(int)distance,this);
                    }
                    
                }
            }
        }

        private Vector3 GetDirection(int id)
        {
            var direction = Vector3.zero;
            switch (id)
            {
                case 1:
                    direction = new Vector3(.5f, 0, 0);
                    break;
                case 2:
                    direction = new Vector3(-.5f, 0, 0);
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

        private void CheckNeighbors()
        {
            _scannedColliders = Physics.OverlapSphere(transform.position, neighborRange, hexLayer);
           
            foreach (var obj in _scannedColliders)
            {
                if (obj.TryGetComponent(out HexController hex))
                {
                    if(hex.IsOccupied())
                        continue;
                    hex.SelectionHint(true);
                }
            }
        }

        public void ResetHints()
        {
            foreach (var obj in _scannedColliders)
            {
                if (obj.TryGetComponent(out HexController hex))
                {
                    hex.SelectionHint(false);
                }
            }

            foreach (var hex in _killHints)
            {
                hex.SelectionHint(false);
            }

            for (var i = 0; i < _scannedColliders.Length; i++)
            {
                _scannedColliders[i] = null;
            }
            _killHints.Clear();
        }

        public void AddKillHint(HexController hex)
        {
            _killHints.Add(hex);
        }
    }
}