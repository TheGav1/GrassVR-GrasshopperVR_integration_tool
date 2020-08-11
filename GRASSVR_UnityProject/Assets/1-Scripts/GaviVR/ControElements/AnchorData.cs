using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaviVR
{
    public class AnchorData : MonoBehaviour
    {
        public GameObject PT = null;
        public GameObject EndPT = null;
        public float Strength = 10000;//10k=fix
        public Vector3 posPT = new Vector3();
        public Vector3 posEndPT =new Vector3();
        private void Update()
        {
            posPT = PT.transform.position;
            posEndPT = EndPT.transform.position;
        }
        public void NewData()
        {
            
            //update pt position to the anchor list

            //update end pt position to the anchor list

            //update Strength
        }
        
    }
}
