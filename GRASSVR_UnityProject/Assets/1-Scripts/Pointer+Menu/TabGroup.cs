using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    //list of the tabs auto popolated by the TabButton script using Subscribe()
    public List<TabButton> Tabs;
    //list of the page object to swape between: manualy add in order as the tab child elements
    public List<GameObject> SwapList;
    public TabButton SelectedTab;
    //sprites idle - enter - selected
    public Color Idle;
    public Color Hover;
    public Color Selected;

    private void Start()
    {
        OnTabClick(this.gameObject.transform.GetChild(0).GetComponent<TabButton>());
    }
    //add button
    public void Subscribe(TabButton tab)
    {
        //initialise
        if (Tabs == null)
        {
            Tabs = new List<TabButton>();
        }
        Tabs.Add(tab);
    }
    //interaction method called from TabButton
    public void OnTabEntre(TabButton tab)
    {
        ResetTabs();
        //change only if not selected already
        if (tab != SelectedTab)
        {
            tab.background.color = Hover;
        }
    }
    public void OnTabExit(TabButton tab)
    {
        ResetTabs();

    }
    public void OnTabClick(TabButton tab)
    {
        //if selected exist => deselect
        if (SelectedTab!=null)
        {
            SelectedTab.Deselect();
        }
        //select this tab from pointer action + select call function
        SelectedTab = tab;
        SelectedTab.Select();

        ResetTabs();
        tab.background.color = Selected;
        //as start does not hava a precise order the more relaible way to connect tabs to  pages is to use the order as siblings
        //element(index of this child element compared to the all list of child elements of the father element)
        int index = tab.transform.GetSiblingIndex();
        //activate corrispondent page
        for (int i = 0; i < SwapList.Count; i++)
        {
            if (i == index) { SwapList[i].SetActive(true); }
            else { SwapList[i].SetActive(false); }
        }
    }
    //reset Tab colour
    public void ResetTabs()
    {
        foreach (TabButton tab in Tabs)
        {
            //not reset selected
            if (SelectedTab != null && tab == SelectedTab) { continue;}
            tab.background.color = Idle;
        }
    }
}
