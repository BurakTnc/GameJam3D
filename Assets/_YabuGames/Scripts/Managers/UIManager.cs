using System;
using _YabuGames.Scripts.Signals;
using TMPro;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject mainPanel, gamePanel, winPanel, losePanel, storePanel;
        [SerializeField] private TextMeshProUGUI[] moneyText;
        [SerializeField] private TextMeshProUGUI killText, moveText;


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

        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void Start()
        {
            SetMoneyTexts();
        }

        #region Subscribtions
        private void Subscribe()
        {
            CoreGameSignals.Instance.OnLevelWin += LevelWin;
            CoreGameSignals.Instance.OnLevelFail += LevelLose;
            CoreGameSignals.Instance.OnGameStart += OnGameStart;
        }
        private void UnSubscribe()
        {
            CoreGameSignals.Instance.OnLevelWin -= LevelWin;
            CoreGameSignals.Instance.OnLevelFail -= LevelLose;
            CoreGameSignals.Instance.OnGameStart -= OnGameStart;
        }
        #endregion

        public void SetKillText(int currentKill, int targetKill)
        {
            killText.text = $"{currentKill}/{targetKill}";
        }

        public void SetMoveText(int currentMove)
        {
            moveText.text = $"{currentMove}";
        }
        private void OnGameStart()
        {
            mainPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
        private void SetMoneyTexts()
        {
            if (moneyText.Length <= 0) return;

            foreach (var t in moneyText)
            {
                if (t)
                {
                    t.text = "$" + StatsManager.Instance.Money;
                }
            }
        }
        private void LevelWin()
        {
            gamePanel.SetActive(false);
            winPanel.SetActive(true);
            HapticManager.Instance.PlaySuccessHaptic();
        }

        private void LevelLose()
        {
            gamePanel.SetActive(false);
            gamePanel.SetActive(true);
            HapticManager.Instance.PlayFailureHaptic();
        }
        
    }
}
