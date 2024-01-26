using System;
using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class OfflineTimeManager : MonoBehaviour
    {
        public static OfflineTimeManager Instance;
        
        private DateTime _logOutTime;
        private float _offlineSeconds;
        private string _logOutTimeStr;
        private float _liveTimer;

        public int GetOfflineSeconds() => (int)_offlineSeconds;
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
            CalculateOfflineTime();
        }

        private void OnApplicationQuit()
        {
            SaveLogOutTime();
            CoreGameSignals.Instance.OnSave?.Invoke();
        }
        private void CalculateOfflineTime()
        {
            var timeDifference = DateTime.Now - _logOutTime;
            _offlineSeconds = (float)timeDifference.TotalSeconds;
            _offlineSeconds = MathF.Round(_offlineSeconds, 0);
        }
        private void SaveLogOutTime()
        {
            _logOutTimeStr = DateTime.Now.ToString();
            PlayerPrefs.SetString("logOutTime", _logOutTimeStr);
            PlayerPrefs.SetFloat("liveTimer",_liveTimer);
            PlayerPrefs.Save();
        }
    }
}