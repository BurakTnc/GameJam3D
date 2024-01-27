using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class HexController : MonoBehaviour
    {
        private bool _isOccupied;
        private bool _isSelectable;

        public void Occupy(bool occupied)
        {
            _isOccupied = occupied;
            // if(_isOccupied)
            //     gameObject.SetActive(false);
        }

        public void SelectionHint(bool selectable, bool killHint=false)
        {
            _isSelectable = selectable;
            if (_isSelectable)
                if(killHint)
                     transform.localScale = Vector3.one * .7f;
                else
                    transform.localScale = Vector3.one * .5f;
            else
            {
                transform.localScale = Vector3.one;
            }
        }
        public bool IsOccupied() => _isOccupied;
        public bool IsSelectable() => _isSelectable;

        public void JumpHint()
        {
            SelectionHint(true,true);
        }
    }
}