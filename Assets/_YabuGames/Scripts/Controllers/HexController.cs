using System;
using _YabuGames.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace _YabuGames.Scripts.Controllers
{
    public class HexController : MonoBehaviour
    {
        [SerializeField] private Material neighborMat, jumpMat;
        [SerializeField] private GameObject fakeGoButton;
        [SerializeField] private GameObject fakeRewindButton;
        
        private MeshRenderer _meshRenderer;
        private Color _defaultColor;
        private bool _isOccupied;
        private bool _isSelectable;
        private bool _actingAsGoButton;
        private bool _actingAsRewindButton;
        private bool _onByPass;
       

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _defaultColor = GetComponent<MeshRenderer>().material.color;
        }

        private void OnMouseDown()
        {
            if (_actingAsGoButton)
            {
                ActAsGoButton(false);
                LevelSignals.Instance.OnGo?.Invoke();
            }

            if (_actingAsRewindButton)
            {
                ActAsRewindButton(false);
                LevelSignals.Instance.OnRewind?.Invoke();
            }
        }

        public void Occupy(bool occupied)
        {
            _isOccupied = occupied;
           if(_actingAsGoButton)
               ActAsGoButton(occupied);
        }

        public void SelectionHint(bool selectable, bool killHint=false)
        {
            if(_onByPass) return;
            
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

        public void ActAsGoButton(bool acting)
        {
            _actingAsGoButton = acting;
            fakeGoButton.SetActive(acting);
        }
        public void ActAsRewindButton(bool acting)
        {
            _actingAsRewindButton = acting;
            fakeRewindButton.SetActive(acting);
        }

        public void ByPass(bool onByPass)
        {
            _onByPass = onByPass;
        }
    }
}