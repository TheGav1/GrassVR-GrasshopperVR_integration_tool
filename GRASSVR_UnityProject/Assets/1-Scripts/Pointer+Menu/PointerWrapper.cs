using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class SteamVRLaserWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer steamVrLaserPointer;
    private Interactable _hoveringInteractable;
    public bool spewDebugText = false;
    private Hand thisHand;

    //-------------------------------------------------
    // The Interactable object this Hand is currently hovering over
    //-------------------------------------------------
    public Interactable hoveringInteractable
    {
        get { return _hoveringInteractable; }
        set
        {
            if (_hoveringInteractable != value)
            {
                if (_hoveringInteractable != null)
                {
                    if (spewDebugText)
                        PointerDebugLog("HoverEnd " + _hoveringInteractable.gameObject);
                    _hoveringInteractable.SendMessage("OnHandHoverEnd", this, SendMessageOptions.DontRequireReceiver);

                    //Note: The _hoveringInteractable can change after sending the OnHandHoverEnd message so we need to check it again before broadcasting this message
                    if (_hoveringInteractable != null)
                    {
                        this.BroadcastMessage("OnParentHandHoverEnd", _hoveringInteractable, SendMessageOptions.DontRequireReceiver); // let objects attached to the hand know that a hover has ended
                    }
                }

                _hoveringInteractable = value;

                if (_hoveringInteractable != null)
                {
                    if (spewDebugText)
                        PointerDebugLog("PointerBegin " + _hoveringInteractable.gameObject);
                    _hoveringInteractable.SendMessage("OnHandHoverBegin", this, SendMessageOptions.DontRequireReceiver);

                    //Note: The _hoveringInteractable can change after sending the OnHandHoverBegin message so we need to check it again before broadcasting this message
                    if (_hoveringInteractable != null)
                    {
                        this.BroadcastMessage("OnParentHandHoverBegin", _hoveringInteractable, SendMessageOptions.DontRequireReceiver); // let objects attached to the hand know that a hover has begun
                    }
                }
            }
        }
    }

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
        UIElement UIel = e.target.GetComponent<UIElement>();
        if (UIel == null || thisHand == null)
        {
            return;
        }
        UIel.BroadcastMessage("OnButtonClick", thisHand);

        /*IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
        if (clickHandler == null)
        {
            return;
        }


        clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));*/
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {

        UIElement UIel = e.target.GetComponent<UIElement>();
        if (UIel == null || thisHand == null)
        {
            return;
        }
        UIel.BroadcastMessage("OnHandHoverEnd", thisHand);

        /*IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
        if (pointerExitHandler == null)
        {
            return;
        }

        pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));*/
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {//1400 hand HoverLock

        UIElement UIel = e.target.GetComponent<UIElement>();
        if (UIel == null || thisHand == null)
        {
            return;
        }
        UIel.BroadcastMessage("OnHandHoverBegin",thisHand);

        /*IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
        if (pointerEnterHandler == null)
        {
            return;
        }

        pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current)); */
    }
    void PointerDebugLog (string msg)
    {
        if (spewDebugText)
        {
            Debug.Log("<b>[SteamVR Interaction]</b> Pointer (" + this.name + "): " + msg);
        }
    }
}
