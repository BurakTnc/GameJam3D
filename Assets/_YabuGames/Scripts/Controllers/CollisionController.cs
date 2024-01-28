using System.Collections.Generic;
using _YabuGames.Scripts.Managers;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class CollisionController : MonoBehaviour
    {
        public List<EnemyController> _killedEnemies = new List<EnemyController>();
        public bool onMove;
        public bool hasSelected;
        public bool isGhost;
        
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        private void OnTriggerStay(Collider other)
        {
            var canPlace = other.CompareTag("Hex") && !onMove && !hasSelected;
            if (canPlace)
            {
                _playerController.Occupy(other.GetComponent<HexController>());
                hasSelected = true;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent(out EnemyController enemyController))
                {
                    if (!_killedEnemies.Contains(enemyController) && !isGhost)
                    {
                        ShakeManager.Instance.ShakeCamera(true);
                        _killedEnemies.Add(enemyController);
                    }
                }
            }
        }

        public List<EnemyController> GetKilledEnemies()
        {
            return _killedEnemies;
        }

        public void ClearEnemyList()
        {
            _killedEnemies.Clear();
        }
    }
}
