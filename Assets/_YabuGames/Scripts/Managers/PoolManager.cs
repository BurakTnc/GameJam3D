using System;
using System.Collections.Generic;
using UnityEngine;

namespace _YabuGames.Scripts.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;

        [SerializeField] private ObjectPool[] pools;
        
        
        [Serializable]
        public struct ObjectPool
        { 
            public int poolSize;
            public string particleName;
            public Queue<GameObject> PooledObjects;
        }
        
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
            InitPool();
        }

        public GameObject GetParticle(int objectID, Vector3 desiredPosition)
        {
            if (objectID >= pools.Length)
                return null;
            
            var obj = pools[objectID].PooledObjects.Dequeue();
           
            obj.transform.position = desiredPosition;
            obj.SetActive(true);
            pools[objectID].PooledObjects.Enqueue(obj);
           
           return obj;
        }

        private void InitPool()
        {
            for (var j = 0; j < pools.Length; j++)
            {
                pools[j].PooledObjects = new Queue<GameObject>(); 
            
                var parent = new GameObject
                {
                    name = pools[j].particleName + " Pool"
                };
            
                for (var i = 0; i < pools[j].poolSize; i++)
                {
                    if (parent == null) 
                        continue;
                    var obj = Instantiate(Resources.Load<GameObject>($"Spawnables/{pools[j].particleName}"), parent.transform);
                    obj.SetActive(false);
                    pools[j].PooledObjects.Enqueue(obj);
                }
            }
        }
    }
}
