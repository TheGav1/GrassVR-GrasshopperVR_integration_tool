using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule
{
    [SerializeField] private Pointer pointer = null;

    public PointerEventData Data { get; private set; } = null;

    protected override void Start()
    {
        Data = new PointerEventData(eventSystem);
        Data.position = new Vector2(pointer.Camera.pixelWidth / 2, pointer.Camera.pixelHeight / 2);
    }

    public override void Process()
    {
            eventSystem.RaycastAll(Data, m_RaycastResultCache);
            Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);

            HandlePointerExitAndEnter(Data, Data.pointerCurrentRaycast.gameObject);

            ExecuteEvents.Execute(Data.pointerDrag, Data, ExecuteEvents.dragHandler);
    }

    public void Press()
    {

        Data.pointerPressRaycast = Data.pointerCurrentRaycast;

        //set pointer variable Pres gameobj + drag gameobj
        Data.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(Data.pointerPressRaycast.gameObject);
        Data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(Data.pointerPressRaycast.gameObject);

        //start events
        ExecuteEvents.Execute(Data.pointerPress, Data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(Data.pointerDrag, Data, ExecuteEvents.beginDragHandler);
    }

    public void Release()
    {
        GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerClickHandler>(Data.pointerCurrentRaycast.gameObject);
        
        //if click
        if (Data.pointerPress == pointerRelease)
            ExecuteEvents.Execute(Data.pointerPress, Data, ExecuteEvents.pointerClickHandler);

        //end events
        ExecuteEvents.Execute(Data.pointerPress, Data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(Data.pointerDrag, Data, ExecuteEvents.endDragHandler);

        //null stored data
        Data.pointerPress = null;
        Data.pointerDrag = null;

        Data.pointerCurrentRaycast.Clear();
    }
}
