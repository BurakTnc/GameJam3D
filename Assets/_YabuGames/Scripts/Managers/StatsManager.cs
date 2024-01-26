using System;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class StatsManager : MonoBehaviour
    {
        public static StatsManager Instance;
        public int money;

        private int _liveCount;
        
        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            #endregion
            GetValues();
        }

        private void GetValues()
        {
            PlayerPrefs.GetInt("money", 0);
        }

        private void SaveValues()
        {
            PlayerPrefs.SetInt("money",Money);
        }

        #region Properties

        public int Money 
        {
            get => money < 0 ? 0 : money;
            set => money += value;
        }

        public int LiveCount
        {
            get => _liveCount = LiveManager.Instance.GetLiveCount();
            set => LiveManager.Instance.SetLiveCount(value);
        }

        #endregion
    }
}
