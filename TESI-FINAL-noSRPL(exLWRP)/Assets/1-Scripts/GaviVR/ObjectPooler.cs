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
        public GameObject ForcePrefab;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        public List<Pool> pools;
        // Start is called before the first frame update
        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            //add pools
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < pool.Size; i++)
                {//initialise the pool as inactive elements
                    GameObject Object = Instantiate(pool.Prefab);
                    Object.SetActive(false);
                    objectPool.Enqueue(Object);
                }
                poolDictionary.Add(pool.Tag, objectPool);
            }
        }
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, float intensity=-1)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.Log("Missing tag" + tag);
                return null;
            }
            GameObject toSpawn = poolDictionary[tag].Dequeue();//take from queue and erase from queue
            toSpawn.transform.position = position;
            toSpawn.transform.rotation = rotation;
            if (toSpawn.CompareTag("Force"))
                if (intensity < 0)
                {
                    Debug.Log("Invalid intensity=> set intensity = 1");
                    toSpawn.GetComponent<ForceData>().intensity = 1;
                }
                else
                {
                    toSpawn.GetComponent<ForceData>().intensity = intensity;
                }

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