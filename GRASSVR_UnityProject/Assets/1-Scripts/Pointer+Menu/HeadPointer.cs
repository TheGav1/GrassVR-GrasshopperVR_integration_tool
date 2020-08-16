using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public Material selectedMat;
    public float sigthlength = 100f;
    public GameObject Selected;
    public float hoverMove = 0.5f;

    private void FixedUpdate()
    {
        RaycastHit seen;
        Ray direction = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(direction, out seen, sigthlength))
        {

            if (seen.collider.tag == "Button")
            {
                
                GameObject hit = seen.transform.gameObject;
                HitButton(hit);
                
            }
            Selected = seen.transform.gameObject;
        }
    }
    private void HitButton(GameObject hit)
    {
        Debug.Log("Hit");
        Renderer rend = hit.GetComponent<Renderer>();
        if(Selected!=hit)
        {
            Material orig = rend.material;
            if (rend != null)
                rend.material = selectedMat;
            StartCoroutine(updateOff(rend, orig));
        }
        //rend.material = orig;


        /*
        Vector3 Znew = hit.transform.position;
        Znew.z -= hoverMove;
        hit.transform.position = Znew;

        Znew.z += hoverMove*2;
        hit.transform.position = Znew;
        */
    }
    IEnumerator updateOff(Renderer rend, Material orig)
    {
        yield return new WaitForSeconds(1.0f);
        rend.material = orig;
    }
}
