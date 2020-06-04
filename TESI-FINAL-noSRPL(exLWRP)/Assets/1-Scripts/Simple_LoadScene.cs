using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class Simple_LoadScene : MonoBehaviour
{
    public int SceneNumber;
    bool loaded=false;

    private void OnTriggerEnter()
    {
        if (!loaded)
        {
            
            //SceneManager.LoadSceneAsync(SceneNumber, LoadSceneMode.Additive);
            loaded = true;
        }
    }
}
