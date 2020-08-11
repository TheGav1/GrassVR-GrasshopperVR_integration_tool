using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class LaserHandler : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
            Debug.Log(e.target.name+" was clicked");
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.name + " was entered");
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.name + " was exited");
    }
}
