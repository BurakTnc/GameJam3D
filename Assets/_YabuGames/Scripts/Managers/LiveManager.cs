using System;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class LiveManager : MonoBehaviour
    {
        public static LiveManager Instance;
        
        [SerializeField] private float liveCoolDown = 60;
        
        private const int _maxLiveCount = 5;
        private int _offlineSeconds;
        private int _isTimerOn;
        private int _currentLiveCount;
        private float _liveTimer;
        private bool _onCountDown;
        private bool _isOfflineCalculated;

        public int GetLiveCount() => _currentLiveCount;
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

        private void Start()
        {
            _offlineSeconds = OfflineTimeManager.Instance.GetOfflineSeconds();
        }

        private void Update()
        {
            CountDown();
        }

        private void CheckLiveEarning()
        {
            //UISignals.Instance.OnShowLiveIcons?.Invoke(_currentLiveCount);
            //UISignals.Instance.OnTimerIsOn.Invoke(false);
            _isTimerOn = 0;
            Debug.Log("Offline Seconds: "+_offlineSeconds);
            if (_currentLiveCount >= _maxLiveCount)
            {
                _currentLiveCount = _maxLiveCount;
                return;
            }
            
            BeginLiveCountDown();
        }

        private void StartLevel()
        {
            SaveValues();
        }

        private void GetValues()
        {
            _currentLiveCount = PlayerPrefs.GetInt("liveCount", _maxLiveCount);
        }
        private void SaveValues()
        {
            _currentLiveCount = _currentLiveCount < 0 ? 0 : _currentLiveCount;
            PlayerPrefs.SetInt("liveCount", _currentLiveCount);
        }

        private void RefillLives()
        {
            _currentLiveCount = _maxLiveCount;
            CheckLiveEarning();
        }
        private void LoseALive()
        {
            _currentLiveCount--;
        }
        private void BeginLiveCountDown()
        {
            _onCountDown = true;

            _isTimerOn = 1;
            if (_liveTimer <= 0) 
                _liveTimer = liveCoolDown;
            //UISignals.Instance.OnTimerIsOn.Invoke(true);
        }

        private void CountDown()
        {
            if(!_onCountDown)
                return;
            
            if (_isTimerOn == 1 && !_isOfflineCalculated)
            {
                var earnedLives = _offlineSeconds/ liveCoolDown;
                var secondsLeft = _offlineSeconds % liveCoolDown;
                Debug.Log("earned Lives "+(int)earnedLives);
                _currentLiveCount += (int)earnedLives;
                _liveTimer -= _offlineSeconds;

                if (_liveTimer <= 0)
                {
                    if ((int)earnedLives == 0)
                    {
                        _currentLiveCount++;
                    }
                    else
                    {
                        _liveTimer = secondsLeft;
                    }
                }
                
                if (_currentLiveCount > _maxLiveCount)
                {
                    _currentLiveCount = _maxLiveCount;
                    _liveTimer = 0;
                }

                _isOfflineCalculated = true;
                _isTimerOn = 0;
                CheckLiveEarning();
                return;
            }
            
            if (_liveTimer <= 0)
            {
                _liveTimer = 0;
                _onCountDown = false;
                _currentLiveCount++;
                CheckLiveEarning();
                return;
            }
            
            _liveTimer -= Time.deltaTime;
            
            var minute = Mathf.Clamp(_liveTimer / 60, 0, 99);
            var second = Mathf.Clamp(_liveTimer % 60, 0, 59);
            var formattedTime = $"{(int)minute:00}:{second:00}";
            
            //UISignals.Instance.OnShowTimer?.Invoke(formattedTime);
        }

        public void SetLiveCount(int value)
        {
            _currentLiveCount += value;
            _currentLiveCount = Math.Clamp(_currentLiveCount, 0, _maxLiveCount);
        }
    }
}