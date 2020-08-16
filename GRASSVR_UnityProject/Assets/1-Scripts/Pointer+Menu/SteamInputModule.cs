 using Valve.VR;

public class SteamInputModule : VRInputModule
{
    
    public SteamVR_Input_Sources m_Source = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean m_Click = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    public SteamVR_Action_Boolean PointerToggler = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("PointerTog");


    public override void Process()
    {
        base.Process();

        // Press
        if (m_Click.GetStateDown(m_Source))
            Press();

        // Release
        if (m_Click.GetStateUp(m_Source))
            Release();
    }

 
}
