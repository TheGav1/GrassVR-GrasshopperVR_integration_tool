using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.VR
{
    public class SceneDataLoad : MonoBehaviour
    {
        //data to be set
        public GameObject PlayerOrigin;
        public GameObject SceneOrigine;//position
        public GameObject GaviVR_Models;//The game object containing al the model objects
        private Data_datascene DataOrigine;

        private void Awake()
        {
            GameObject[] DataObj = GameObject.FindGameObjectsWithTag("DataObject");
            DataOrigine = DataObj[0].GetComponent<Data_datascene>();
        }

        void Start()
        {
            //load data to scene

            positionSave(DataOrigine.sceneposition, PlayerOrigin);
            positionSave(DataOrigine.sceneposition, SceneOrigine);

            scaleSave(DataOrigine.Scale, GaviVR_Models);
        }

        private void OnApplicationQuit()
        {
            //save data to DataGameObject
            DataOrigine.Scale = GaviVR_Models.transform.localScale.x;//Scale*vector3.one x=y=z
            DataOrigine.sceneposition = SceneOrigine.transform.position;
        }
        void positionSave(Vector3 Position, GameObject gameObject)
        {
            gameObject.transform.position = Position;
        }
        void scaleSave(float Scale, GameObject gameObject)
        {

            gameObject.transform.localScale = Vector3.one * Scale;
        }
    }
}
