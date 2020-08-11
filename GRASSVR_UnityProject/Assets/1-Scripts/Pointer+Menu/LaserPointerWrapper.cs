using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class LaserPointerWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer steamVrLaserPointer;
    private Hand thisHand;
    public Interactable Interact;
    private GameObject hit, parent;
    public bool debugText = false;




    private void Awake()
    {
        thisHand = this.GetComponent<Hand>();
        steamVrLaserPointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        steamVrLaserPointer.PointerIn += OnPointerIn;
        steamVrLaserPointer.PointerOut += OnPointerOut;
        steamVrLaserPointer.PointerClick += OnPointerClick;
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        
        if (Interact == null || thisHand == null)
        {
            return;
        }
        if (debugText)
        {
            Debug.Log("Clicked" + Interact);
        }
        
        Interact.BroadcastMessage("OnButtonClick", thisHand, SendMessageOptions.DontRequireReceiver);
        
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        if (Interact == null || thisHand == null)
        {

            return;
        }
        Interact.SendMessage("OnHandHoverEnd", thisHand, SendMessageOptions.DontRequireReceiver);
        Interact = null;//exit=>null UIelem
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        //getUIelem
        hit = e.target.gameObject;
        Interact = hit.GetComponent<Interactable>();
        if (Interact == null)
        {
            if (debugText)
            {
                Debug.Log("NoInteractable on hit =" + hit);
            }
            Interact = hit.GetComponentInParent<Interactable>();
        }
        
        if (Interact == null || thisHand == null)
        {
            if (debugText)
            {
                Debug.Log("NoInteractable");
            }
            return;
        }
        if (debugText)
        {
            Debug.Log("Interactable =" + Interact);
        }
        Interact.SendMessage("OnHandHoverBegin", thisHand, SendMessageOptions.DontRequireReceiver);
    }

    /*protected virtual void Update()
    {
        if (UIel)
        {
            UIel.SendMessage("HandHoverUpdate", thisHand, SendMessageOptions.DontRequireReceiver);
        }
    }*/
}
