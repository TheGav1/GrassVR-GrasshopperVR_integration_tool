using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.VR
{
    public class SceneStartDataLoad : MonoBehaviour
    {
        //data to be set
        public GameObject PlayerOrigin;
        public GameObject SceneOrigine;//position
        public GameObject GaviVR_Model;
        public Data_datascene DataOrigine;

        void Awake()
        {
            //possible find DataOrigine with tag

            positionSave(DataOrigine.sceneposition, PlayerOrigin);
            positionSave(DataOrigine.sceneposition, SceneOrigine);
            scaleSave(DataOrigine.Scale, SceneOrigine);

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
