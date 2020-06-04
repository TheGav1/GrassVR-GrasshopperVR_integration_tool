using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_datascene : MonoBehaviour
{
    public float Scale=1;
    public float floar_level = 0;
    public bool data_recived = false;
    public Vector3 sceneposition = new Vector3(0,0,0);

    //generate the object as not descrutable on load
    //(following Unity documentation https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Object.DontDestroyOnLoad.html )
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DataObject");
        
        //check if the object already exist [if for not wanted reason the data level is re loaded]
        //with this check the data object could be moved to loas selection and model check scene as multiple istances could not be created
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
