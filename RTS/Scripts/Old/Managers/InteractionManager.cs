using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

#region enums
public enum InteractionSource : int
{
    //left,right,wheel
    MouseButton1 = 0,
    MouseButton2 = 1,
    MouseButton3 = 2,
}
public enum InteractionType
{
    //Drag start:button click down and move
    //Drag move: draging
    //Drag end:button release
    MouseClick,
    MouseDoubleClick,
    DragStart,
    DragEnd,
    DragMove
}
#endregion enums
#region interface
public interface IInteractionManager
{
    void AddMouseListener(InteractionSource source, InteractionType interaction, Action<Vector2> eventHandler);
    void RemoveMouseListener(InteractionSource source, InteractionType interaction, Action<Vector2> eventHandler);
    void AddZoomListener(Action<float, Vector2> zoomStateAndPositionHandler);
    void RemoveZoomListener(Action<float, Vector2> zoomStateAndPositionHandler);
    void AddMoveListener(Action<Vector2> mouseMoveHandler);
    void RemoveMoveListener(Action<Vector2> mouseMoveHandler);

}
#endregion interface
public class InteractionManager : MonoBehaviour,IInteractionManager,ISerializationCallbackReceiver
{
    #region Instance
    public static IInteractionManager Instance;
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        Instance = this;
    }
    #endregion Instance
    #region Settings
    [System.Serializable]
    public class SettingFields
    {
        //Response time
        public float MouseClick_MaxReleaseTime = .3f;
        public float MouseClick_MaxDistance = 5f;
        public float MouseDoubleClick_MaxTimeForSecondClick = 1f;
    }
    public SettingFields Settings;
    #endregion Settings

    #region Events
    [System.Serializable]
    public class ResponseFields
    {
        //each action for three mouse buttons independent
        public Action<Vector2>[] MouseClicked = new Action<Vector2>[]
        {
                (m) => { },
                (m) => { },
                (m) => { }
        };
        public Action<Vector2>[] MouseDoubleClicked = new Action<Vector2>[]
        {
                (m) => { },
                (m) => { },
                (m) => { }
        };
        public Action<Vector2>[] MouseDragStart = new Action<Vector2>[]
        {
                (m) => { },
                (m) => { },
                (m) => { }
        };
        public Action<Vector2>[] MouseDragEnd = new Action<Vector2>[]
        {
                (m) => { },
                (m) => { },
                (m) => { }
        };
        public Action<Vector2>[] MouseDragMove = new Action<Vector2>[]
        {
                (m) => { },
                (m) => { },
                (m) => { }
        };
        public Action<float, Vector2> MouseZoomHandlers = (a, v2) => { };
        public Action<Vector2> MouseMoveHandler = (x) => { };
    }
    private ResponseFields Response = new ResponseFields();

    public void AddMouseListener(InteractionSource source, InteractionType interaction, Action<Vector2> eventHandler)
    {
        if (interaction == InteractionType.MouseClick) Response.MouseClicked[(int)source] += eventHandler;
        if (interaction == InteractionType.MouseDoubleClick) Response.MouseDoubleClicked[(int)source] += eventHandler;
        if (interaction == InteractionType.DragStart) Response.MouseDragStart[(int)source] += eventHandler;
        if (interaction == InteractionType.DragEnd) Response.MouseDragEnd[(int)source] += eventHandler;
        if (interaction == InteractionType.DragMove) Response.MouseDragMove[(int)source] += eventHandler;
    }
    public void RemoveMouseListener(InteractionSource source, InteractionType interaction, Action<Vector2> eventHandler)
    {
        if (interaction == InteractionType.MouseClick) Response.MouseClicked[(int)source] -= eventHandler;
        if (interaction == InteractionType.MouseDoubleClick) Response.MouseDoubleClicked[(int)source] -= eventHandler;
        if (interaction == InteractionType.DragStart) Response.MouseDragStart[(int)source] -= eventHandler;
        if (interaction == InteractionType.DragEnd) Response.MouseDragEnd[(int)source] -= eventHandler;
        if (interaction == InteractionType.DragMove) Response.MouseDragMove[(int)source] -= eventHandler;
    }
    public void AddZoomListener(Action<float, Vector2> zoomStateAndPositionHandler)
    {
        Response.MouseZoomHandlers += zoomStateAndPositionHandler;
    }
    public void RemoveZoomListener(Action<float, Vector2> zoomStateAndPositionHandler)
    {
        Response.MouseZoomHandlers -= zoomStateAndPositionHandler;
    }
    public void AddMoveListener(Action<Vector2> mouseMoveHandler)
    {
        Response.MouseMoveHandler += mouseMoveHandler;
    }
    public void RemoveMoveListener(Action<Vector2> mouseMoveHandler)
    {
        Response.MouseMoveHandler -= mouseMoveHandler;
    }
    #endregion Events
    #region MouseHandler
    [System.Serializable]
    public class PreviousFields
    {
        public Vector2 MousePosition = Vector2.one * float.MinValue;
        public float[] MouseReleaseTime = new float[3];
        public Vector2[] MouseReleasePosition = new Vector2[3];
        public float[] MousePressedTime = new float[3];
        public Vector2[] MousePressedPosition = new Vector2[3];
        public bool[] Down =new bool[] {false,false,false };
        public bool[] InDragState = new bool[] { false, false, false };
    }
    private PreviousFields Previous = new PreviousFields();
    private bool MouseMoved = false;
    private Vector2 MousePosition = Vector2.zero;
    private void MouseMoveHandle()
    {
        MousePosition = Input.mousePosition;

        if (Previous.MousePosition == MousePosition)
        {
            MouseMoved= false;
            return;
        }
        MouseMoved=true;
        Previous.MousePosition = MousePosition;

        var inDragState = Previous.InDragState[0] || Previous.InDragState[1] || Previous.InDragState[2];
        if (inDragState)
        {
            return;
        }
        Response.MouseMoveHandler.Invoke(MousePosition);
    }
    private void MouseZoomHandle() 
    {
        var inputDelta = Input.mouseScrollDelta;
        if (inputDelta == Vector2.zero)
        {
            return;
        }
        Response.MouseZoomHandlers.Invoke(inputDelta.y, MousePosition);
    }
    private void MouseCheckHandle(InteractionSource mouseButton)//get current mouse state(press?release?)
    {
        //get current mouse button
        int Button = (int)mouseButton;
        var currentButtonState = Input.GetMouseButton(Button);

        bool IsMousePress_InFrame = !Previous.Down[Button] && currentButtonState; //Is down and has state
        bool IsMouseRelease_InFrame = Previous.Down[Button] && !currentButtonState;
        Previous.Down[Button] = currentButtonState;//back the state

        if (IsMouseRelease_InFrame)
        {
            //single Click mouse button
            var time_between = Time.realtimeSinceStartup - Previous.MouseReleaseTime[Button];//the time between button Down and Up 
            var IsTime_InSingleClickTime = time_between <= Settings.MouseClick_MaxReleaseTime;
            Previous.MouseReleaseTime[Button] = Time.realtimeSinceStartup;

            //Double Click mouse button
            time_between = Time.realtimeSinceStartup - Previous.MouseReleaseTime[Button];//the time between button Down and Up but twice
            var IsTime_InDoubleClickTime = time_between <= Settings.MouseDoubleClick_MaxTimeForSecondClick;
            Previous.MouseReleaseTime[Button] = Time.realtimeSinceStartup;

            var MouseMovingDistance = Vector2.Distance(MousePosition, Previous.MousePressedPosition[Button]);
            if (MouseMovingDistance > Settings.MouseClick_MaxDistance)
            {
                IsTime_InSingleClickTime = false;
                IsTime_InDoubleClickTime = false;
            }
            if (IsTime_InDoubleClickTime)
            {
                //should be front cause double click include single click some situation
                IsTime_InSingleClickTime = false;
                Response.MouseDoubleClicked[Button](MousePosition);
            }
            if (IsTime_InSingleClickTime && !Previous.InDragState[Button])//should not in drag
            {
                Response.MouseClicked[Button](MousePosition);
            }
        }
        if (IsMousePress_InFrame)
        {
            Previous.MousePressedTime[Button] = Time.realtimeSinceStartup;
            Previous.MousePressedPosition[Button] = MousePosition;
        }
        if (currentButtonState)//if button press and hold
        {
            if (!Previous.InDragState[Button])
            {
                var time_betweenPressed = Time.realtimeSinceStartup - Previous.MousePressedTime[Button];
                var distance_betweenPressed = Vector2.Distance(MousePosition, Previous.MousePressedPosition[Button]);
                var Is_switchToDrag = time_betweenPressed > Settings.MouseClick_MaxReleaseTime || distance_betweenPressed > Settings.MouseClick_MaxDistance;//Detect time and distance is not around click
                if (Is_switchToDrag)
                {
                    Response.MouseDragStart[Button](Previous.MousePressedPosition[Button]);
                    Response.MouseDragMove[Button](MousePosition);
                    Previous.InDragState[Button] = true;
                }
            }
            else if (this.MouseMoved)//mouse is moving
            {
                Response.MouseDragMove[Button](MousePosition);
            }
        }
        if (IsMouseRelease_InFrame && Previous.InDragState[Button])//button released and in the drag state
        {
            Previous.InDragState[Button] = false;
            Response.MouseDragEnd[Button](MousePosition);
        }
    }
    #endregion MouseHandler

}
