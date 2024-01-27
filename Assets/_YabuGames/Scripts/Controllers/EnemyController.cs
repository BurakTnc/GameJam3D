using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _YabuGames.Scripts.Controllers
{
    public class EnemyController : MonoBehaviour
    {
        private EnemyCollisionController _enemyCollision;
        private HexController _currentHex;

        public void FireRay(Vector3 direction, float distance, PathCalculator pathCalculator)
        {
            var fixedPos = new Vector3(transform.position.x+(distance)*(direction.x) , 1, transform.position.z+(distance* direction.z));
            Debug.DrawRay(fixedPos+Vector3.up*.5f,new Vector3(0,-1,0),Color.blue,100);
            
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

        private Vector3 GetRandomHex()
        {
            var id = Random.Range(1, 7);
            var direction = Vector3.zero;
            switch (id)
            {
                case 1:
                    direction = new Vector3(1, 0, 0);
                    break;
                case 2:
                    direction = new Vector3(-1, 0, 0);
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

        public void Die()
        {
            transform.DOScale(Vector3.zero, .3f).SetEase(Ease.InSine).OnComplete(() => Destroy(gameObject));
        }
    }
}