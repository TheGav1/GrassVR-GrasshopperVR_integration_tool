using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaviVR
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string Tag;
            public GameObject Prefab;
            public int Size;
        }
        #region Singelton
        public static ObjectPooler Instance;
        private void Awake()
        {
            Instance = this;
        }
        #endregion

        //ForceObjectPool
        //public GameObject ForcePrefab;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        public List<Pool> pools;
        // Start is called before the first frame update
        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            //add pools
            foreach (Pool pool in pools)
            {
                GameObject Tag = new GameObject(pool.Tag);
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.Size; i++)
                {//initialise the pool as inactive elements
                    GameObject Object = Instantiate(pool.Prefab);
                    Object.SetActive(false);

                    objectPool.Enqueue(Object);
                    Object.transform.parent = Tag.transform;
                }
                poolDictionary.Add(pool.Tag, objectPool);
            }
        }
        private void InitiateMore(string tag)
        {//if almost empty add initial size/4 new elements
            GameObject Tag = GameObject.Find(tag);
            Pool pool = pools.Find(item => item.Tag == tag);
            for (int i = 0; i < Mathf.Ceil(pool.Size/4); i++)
            {
                GameObject toAdd = Instantiate(pool.Prefab);
                toAdd.SetActive(false);
                poolDictionary[tag].Enqueue(toAdd);
            }
            
        }
        //Spawn function
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation=default)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.Log("Missing tag" + tag);
                return null;
            }
            //
            //check queue length - if=<6 ad more
            //
            if(poolDictionary[tag].Count<=6)
            {
                InitiateMore(tag);
            }

            GameObject toSpawn = poolDictionary[tag].Dequeue();//take from queue and erase from queue
            toSpawn.transform.position = position;
            toSpawn.transform.rotation = rotation;
            toSpawn.SetActive(true);
            //run OnObjectSpawn at spawn
            IPolledObject poolObj = toSpawn.GetComponent<IPolledObject>();
            if(poolObj != null)
            {
                poolObj.OnObjectSpawn();
            }

            /*if need to be requed directly
             * poolDictionary[tag].Enqueue(toSpawn);
             */
            return toSpawn;
        }
    }

}