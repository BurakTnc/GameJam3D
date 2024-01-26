using _YabuGames.Scripts.Signals;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private int _level;

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

        #region Subscribtions
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            CoreGameSignals.Instance.OnSave += Save;
        }

        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnSave -= Save;
        }

        #endregion

        private void GetValues()
        {
            _level = PlayerPrefs.GetInt("level", 0);
        }

        private void Save()
        {
            PlayerPrefs.SetInt("level", _level);
        }

        private void Win()
        {
            _level++;
            CoreGameSignals.Instance.OnSave?.Invoke();
        }
    }
}