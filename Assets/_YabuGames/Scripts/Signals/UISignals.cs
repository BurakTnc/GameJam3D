using System;
using UnityEngine;

namespace _YabuGames.Scripts.Signals
{
    public class UISignals : MonoBehaviour
    {
        #region Singleton

        public static UISignals Instance;
        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        #endregion
        
        
    }
}