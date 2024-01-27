using System;
using UnityEngine;
using UnityEngine.Events;

namespace _YabuGames.Scripts.Signals
{
    public class LevelSignals : MonoBehaviour
    {
        public UnityAction OnGo = delegate { };
        public UnityAction OnRewind = delegate { };
        #region Singleton

        public static LevelSignals Instance;

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