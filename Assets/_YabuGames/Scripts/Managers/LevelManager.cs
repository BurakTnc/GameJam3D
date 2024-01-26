using _YabuGames.Scripts.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _YabuGames.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

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
        
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        #region Subscribtons

        private void Subscribe()
        {
            
        }
        
        private void UnSubscribe()
        {
           
        }

        #endregion
        
        private void GetValues()
        {
           
        }

        private void LevelWin()
        {
          
        }

        private void Save()
        {
         
        }

       
    }
}