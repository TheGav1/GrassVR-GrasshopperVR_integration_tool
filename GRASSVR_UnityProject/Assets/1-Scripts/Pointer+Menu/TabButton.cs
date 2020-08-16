using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TabGroup Tabs;//Tabs reference
    public Image background;//Tab background color reference

    //Unity event from inspector UI
    public UnityEvent OnTabSelected, OnTabDeselected;

    //event functions Enter - Click - Exit call TabGroup functions
    public void OnPointerClick(PointerEventData eventData)
    {
        Tabs.OnTabClick(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tabs.OnTabEntre(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tabs.OnTabExit(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        //subscribe to tab group
        Tabs.Subscribe(this);
    }
    //add for custom select and deselect fiunctionalities (like for button function
    public void Select()
    {
        if (OnTabSelected != null)
        {
            OnTabSelected.Invoke();
        }
    }
    public void Deselect()
    {
        if (OnTabDeselected != null)
        {
            OnTabDeselected.Invoke();
        }
    }
}
