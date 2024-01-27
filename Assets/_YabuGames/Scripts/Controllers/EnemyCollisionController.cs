using System;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class EnemyCollisionController : MonoBehaviour
    {
        public bool onMove;
        public bool hasSelected;
        
        private EnemyController _enemyController;

        private void Awake()
        {
            _enemyController = GetComponent<EnemyController>();
        }

        private void OnTriggerStay(Collider other)
        {
            var canPlace = other.CompareTag("Hex") && !onMove && !hasSelected;
            if (canPlace)
            {
                _enemyController.Occupy(other.GetComponent<HexController>());
                hasSelected = true;
            }
        }
    }
}