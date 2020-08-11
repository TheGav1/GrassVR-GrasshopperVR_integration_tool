using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simple_UnloadScene : MonoBehaviour
{
    public int SceneNumber;
    bool unloaded;

    private void OnTriggerEnter()
    {
        if (!unloaded)
        {
            AsincStart1.asincStart.UnloadScene(SceneNumber);
            unloaded = true;
        }
    }
}
