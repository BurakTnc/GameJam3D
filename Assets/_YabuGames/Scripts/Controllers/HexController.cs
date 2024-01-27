using System;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class HexController : MonoBehaviour
    {
        [SerializeField] private Material neighborMat, jumpMat;

        private MeshRenderer _meshRenderer;
        private Color _defaultColor;
        private bool _isOccupied;
        private bool _isSelectable;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _defaultColor = GetComponent<MeshRenderer>().material.color;
        }

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
                if (killHint)
                    _meshRenderer.material.DOColor(jumpMat.color, .3f).SetEase(Ease.InSine);
                else
                    _meshRenderer.material.DOColor(neighborMat.color, .3f).SetEase(Ease.InSine);
            else
            {
                _meshRenderer.material.DOColor(_defaultColor, .3f).SetEase(Ease.InSine);
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