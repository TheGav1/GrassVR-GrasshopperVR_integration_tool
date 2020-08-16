using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PointerToggler : MonoBehaviour
{
    public SteamVR_Input_Sources m_Source = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean PointerTogButton = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PointerTog");
    public SteamVR_Action_Boolean MenuTogButton = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("menu");
    private bool Started = false;
    [SerializeField] private Pointer pointer = null;
    private VRInputModule PinterInputModule = null;

    private void Awake()
    {
        PinterInputModule = GetComponent<VRInputModule>();
    }
    private void Update()
    {
        //initialize the data in start of VRInput and close the pointer
        if (!Started)
        {
            PointerOff();
            Started = true;
        }
        // Press
        if (PointerTogButton.GetStateDown(m_Source))
            PointerOn();

        // Release
        if (PointerTogButton.GetStateUp(m_Source))
            PointerOff();
    }
    void PointerOn()
    {
        pointer.gameObject.SetActive(true);
        //PinterInputModule.DeactivateModule();
    }
    void PointerOff()
    {
        pointer.gameObject.SetActive(false);
        //PinterInputModule.ActivateModule();
    }
}
