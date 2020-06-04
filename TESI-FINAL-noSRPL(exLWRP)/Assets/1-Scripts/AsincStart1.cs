using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class AsincStart1 : MonoBehaviour
{
    public static AsincStart1 asincStart;
    public bool AdditiveLoad = false;
    bool gamestart;
    public int LoadScene_Number = 1;
    public int UnloadScene_Number;

    // Start is called before the first frame update
    void Awake()
    {
        if (!gamestart)//if awaken multiple time do only once
        {
            asincStart = this;
            //SceneManager.LoadSceneAsync(SceneNumber, LoadSceneMode.Additive);
            GaviVR_LoadLevel.Begin(LoadScene_Number, AdditiveLoad);
            gamestart = true;
        }
    }

    public void UnloadScene(int scene)
    {
        //StartCoroutine(Unload(scene));
        SceneManager.UnloadSceneAsync(scene);
    }
/* //old not async so in coroutine
 //   IEnumerator Unload(int scene)
 //   {
 //       yield return null;
//
//        SceneManager.UnloadScene(scene);
//    }
*/
}
