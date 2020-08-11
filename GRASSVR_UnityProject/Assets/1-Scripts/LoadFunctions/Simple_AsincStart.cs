using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsincStart : MonoBehaviour
{
    public static AsincStart asincStart;
    bool gamestart;
    public int SceneNumber = 1;

    // Start is called before the first frame update
    void Awake()
    {
        if (!gamestart)//if awaken multiple time do only once
        {
            asincStart = this;
            SceneManager.LoadSceneAsync(SceneNumber, LoadSceneMode.Additive);
            gamestart = true;
        }
    }

    public void UnloadScene(int scene)
    {
        //StartCoroutine(Unload(scene));
        SceneManager.UnloadSceneAsync(scene);
    }

    IEnumerator Unload(int scene)
    {
        yield return null;

        SceneManager.UnloadScene(scene);
    }

}